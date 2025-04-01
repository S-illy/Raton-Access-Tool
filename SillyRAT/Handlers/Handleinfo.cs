using Stuff;
using SillyRAT.ClientForms;
using Server.Connection;
using System.Collections.Generic;
using System.Windows.Forms;
using SillyRAT;
using System;
using System.Drawing;

namespace Server.Handlers
{
    internal class HandleInfo
    {
        public HandleInfo(SillyClient SillyClient, Unpack unpack)
        {
            string formname = "Client information | Client ID: ";
            FormInformation infoForm = (FormInformation)Application.OpenForms[formname + unpack.GetAsString("UID")];
            if (infoForm != null)
            {
                if (infoForm.SillyClient == null)
                {
                    infoForm.SillyClient = SillyClient;
                    infoForm.timer1.Start();
                }
                infoForm.Invoke(new MethodInvoker(() =>
                {
                    infoForm.listView1.Items.Clear();
                    Dictionary<string, object> dict = unpack.GetAll();
                    infoForm.listView1.BeginUpdate();

                    int itemCount = 0;

                foreach (var dictItem in dict)
                {
                    if (dictItem.Key == "UID" || dictItem.Key == "Packet") continue;

                    ListViewItem listViewItem = new ListViewItem(dictItem.Key);
                    listViewItem.SubItems.Add(dictItem.Value?.ToString() ?? "");

                    if (itemCount == 0)
                    {
                        listViewItem.ImageKey = "Username.png";
                    }
                    if (itemCount == 1)
                    {
                        listViewItem.ImageKey = "Domain.png";
                    }
                    if (itemCount == 2)
                    {
                        listViewItem.ImageKey = "Priv.png";
                    }
                        if (itemCount == 3)
                        {
                            listViewItem.ImageKey = "Calen.png";
                        }
                        if (itemCount == 4)
                        {
                            listViewItem.ImageKey = "OS.png";
                        }
                        if (itemCount == 5)
                            {
                                listViewItem.ImageKey = "desk.png";
                            }
                        if (itemCount == 6)
                        {
                            listViewItem.ImageKey = "ENGRANAJE.png";
                        }
                        if (itemCount == 7)
                        {
                            listViewItem.ImageKey = "memory.png";
                        }
                        if (itemCount == 8)
                        {
                            listViewItem.ImageKey = "dsfds.png";
                        }
                        if (itemCount == 9)
                        {
                            listViewItem.ImageKey = "gpu.png";
                        }
                        if (itemCount == 10)
                        {
                            listViewItem.ImageKey = "virus.png";
                        }
                        if (itemCount == 11)
                        {
                            listViewItem.ImageKey = "satel.png";
                        }
                        if (itemCount == 12)
                        {
                            listViewItem.ImageKey = "meridian.png";
                        }
                        if (itemCount == 13)
                        {
                            listViewItem.ImageKey = "laptop.png";
                        }
                        if (itemCount == 14)
                        {
                            listViewItem.ImageKey = "world.png";
                        }
                        if (itemCount == 15)
                        {
                            listViewItem.ImageKey = "world.png";
                        }
                        if (itemCount == 16)
                        {
                            listViewItem.ImageKey = "world.png";
                        }
                        if (itemCount == 17)
                        {
                            listViewItem.ImageKey = "world.png";
                        }
                        if (itemCount == 18)
                        {
                            listViewItem.ImageKey = "world.png";
                        }
                        if (itemCount == 19)
                        {
                            listViewItem.ImageKey = "Proxy.png";
                        }
                        if (itemCount == 20)
                        {
                            listViewItem.ImageKey = "Serv.png";
                        }
                        if (itemCount == 21)
                        {
                            listViewItem.ImageKey = "ID.png";
                        }
                        if (itemCount == 22)
                        {
                            listViewItem.ImageKey = "Admin.png";
                        }

                        infoForm.listView1.Items.Add(listViewItem);
                        itemCount++;
                    }

                    infoForm.listView1.EndUpdate();
                    Program.form2.Invoke(new Action(() =>
                    {
                        Program.form2.AddLog("Got the client information", Color.White);
                    }));
                    infoForm.label3.Visible = false;
                }));
            }
        }
    }
}
