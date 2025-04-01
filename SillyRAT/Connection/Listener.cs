using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server.Connection
{
    internal class Listen
    {
        TcpListener listener { get; set; }
        CancellationTokenSource cancellationTokenSource { get; set; }
        List<SillyClient> Sillyclients { get; set; }
        X509Certificate2 serverCertificate;

        public Listen(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            listener = new TcpListener(endPoint);
            serverCertificate = LoadCertificate();
        }

        public void Start()
        {
            Sillyclients = new List<SillyClient>();
            listener.Start();
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => { TcpClientCallback(); }, cancellationTokenSource.Token);
        }

        public List<SillyClient> getClients()
        {
            return Sillyclients;
        }

        public void Stop()
        {
            try
            {
                cancellationTokenSource?.Cancel();
                foreach (SillyClient client in Sillyclients)
                {
                    client?.Disconnect();
                }
                Sillyclients.Clear();
                listener?.Stop();
            }
            catch { }
        }

        private void TcpClientCallback()
        {
            while (true)
            {
                try
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Task.Run(() => HandleClient(tcpClient));
                }
                catch { continue; }
            }
        }

        private void HandleClient(TcpClient tcpClient)
        {
            try
            {
                SslStream sslStream = new SslStream(tcpClient.GetStream(), false);
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, true);
                Sillyclients.Add(new SillyClient(tcpClient, sslStream));
            }
            catch (Exception)
            {
                tcpClient.Close();
            }
        }

        private X509Certificate2 LoadCertificate()
        {
            string certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SSLRaton.pfx");
            string passwordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONOTSHARE.json");

            if (!File.Exists(certPath) || !File.Exists(passwordPath))
                throw new FileNotFoundException("SSL Certificate or password not found.");

            string password = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(passwordPath)).Password;
            return new X509Certificate2(certPath, password);
        }
    }
}
