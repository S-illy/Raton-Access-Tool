using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using RatonRAT.ClientForms;
using Stuff;

namespace Server.Handlers
{
    internal class HandlePortManager
    {
        public HandlePortManager(SillyClient SillyClient, Unpack unpack)
        {
            string formName = "Port spy | Client ID: ";
            FormPorts portManagerForm = (FormPorts)Application.OpenForms[formName + unpack.GetAsString("UID")];

            if (portManagerForm != null)
            {
                if (portManagerForm.SillyClient == null)
                {
                    portManagerForm.SillyClient = SillyClient;
                }

                portManagerForm.Invoke(new MethodInvoker(() =>
                {
                    string[] ports = unpack.GetAsString("Ports").Split(new[] { "-=>" }, StringSplitOptions.RemoveEmptyEntries);
                    portManagerForm.listView1.Items.Clear();
                    portManagerForm.listView1.BeginUpdate();

                    for (int x = 0; x < ports.Length - 2; x += 3)
                    {
                        ListViewItem listViewItem = new ListViewItem(ports[x]);
                        if (ports[x + 2].ToUpper() == "ESTABLISHED")
                        {
                            listViewItem.ImageIndex = 0;
                        }
                        if (ports[x + 2].ToUpper() == "TIMEWAIT")
                        {
                            listViewItem.ImageIndex = 1;
                        }
                        if (ports[x + 2].ToUpper() == "CLOSEWAIT")
                        {
                            listViewItem.ImageIndex = 1;
                        }
                        if (ports[x + 2].ToUpper() == "LISTENING")
                        {
                            listViewItem.ImageIndex = 2;
                        }

                        listViewItem.SubItems.Add(ports[x + 1]);
                        listViewItem.SubItems.Add(ports[x + 2]);
                        portManagerForm.listView1.Items.Add(listViewItem);
                    }

                    portManagerForm.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    portManagerForm.listView1.EndUpdate();

                    foreach (ColumnHeader column in portManagerForm.listView1.Columns)
                    {
                        column.Width = portManagerForm.listView1.Width / portManagerForm.listView1.Columns.Count;
                    }

                    Program.form2.Invoke(new Action(() =>
                    {
                        Program.form2.AddLog("The client's open ports and states have been obtained", Color.White);
                    }));
                }));
            }
        }
    }

}