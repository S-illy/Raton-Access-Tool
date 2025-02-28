using System;
using Stuff;
using Server.Connection;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RatonRAT.ClientForms
{
    public partial class FormWebcam : Form
    {
        private bool isMouseDown = false;

        private Point mouseOffset;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        public SillyClient SillyClient { get; set; }
        public FormWebcam()
        {
            InitializeComponent();
        }

        private void FormWebcam_Load(object sender, EventArgs e)
        {
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.Paint += Form_Paint;
            timer1.Start();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius - 1, rect.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DesignMode)
            {
                using (Pen pen = new Pen(Color.White, 3))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, CreateRoundPath(this.ClientRectangle, 25));
                }
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
                Clipboard.SetImage(pictureBox1.Image); 
        }

        private void takeAnotherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Webcam");
            SillyClient.Send(pack.Pacc());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "StopWebcam");
            SillyClient.Send(pack.Pacc());
            this.Close();
        }

        private void FormWebcam_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "StopWebcam");
            SillyClient.Send(pack.Pacc());
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Pack pack = new Pack();
            pack.Set("Packet", "StopWebcam");
            SillyClient.Send(pack.Pacc());
        }
            
    }
}
