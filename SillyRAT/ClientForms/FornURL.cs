using Server.Connection;
using Stuff;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FornURL : Form
    {
        public SillyClient SillyClient { get; set; }

        private bool isMouseDown = false;

        private Point mouseOffset;

        private void FormURL_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void FormURL_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void FormURL_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public FornURL()
        {
            InitializeComponent();
        }

        private void FornURL_Load(object sender, EventArgs e)
        {
            this.Paint += Form_Paint;
            this.MouseDown += FormURL_MouseDown;
            this.MouseMove += FormURL_MouseMove;
            this.MouseUp += FormURL_MouseUp;
            timer1.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Uri uriResult;
            bool isValidUrl = Uri.TryCreate(textBox1.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (isValidUrl)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Website");
                pack.Set("URL", textBox1.Text.ToString());
                SillyClient.Send(pack.Pacc());
            }
            else
            {
                MessageBox.Show("Please insert a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }
    }
}
