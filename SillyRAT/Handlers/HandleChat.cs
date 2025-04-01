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
    internal class HandleChat
    {
        public HandleChat(SillyClient SillyClient, Unpack unpack)
        {
            string formName = "Chat | Client ID: ";
            FormChat formChat = (FormChat)Application.OpenForms[formName + unpack.GetAsString("UID")];

            if (formChat != null)
            {
                if (formChat.SillyClient == null)
                {
                    formChat.SillyClient = SillyClient;
                }

                formChat.Invoke(new MethodInvoker(() =>
                {
                    ListViewItem meow = new ListViewItem(unpack.GetAsString("Message"));
                    meow.ImageIndex = 0;
                    formChat.listView1.Items.Add(meow);
                }));
            }
        }
    }
}