using System;
using Server.Connection;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Stuff;
using System.IO;

namespace RatonRAT.ClientForms
{
    public partial class FormSound : Form
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
        public FormSound()
        {
            InitializeComponent();
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            timer1.Start();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Supported by RatonRAT|*.mp3";
                openFileDialog.Title = "Select an audio file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    label1.Text = openFileDialog.FileName;
                    label1.ForeColor = Color.LimeGreen;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(label1.Text) || !File.Exists(label1.Text))
                {
                    MessageBox.Show("Select a valid file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] audioBytes = File.ReadAllBytes(label1.Text);
                Pack pack = new Pack();
                pack.Set("Packet", "PlayAudio");
                pack.Set("Audio", audioBytes);
                SillyClient.Send(pack.Pacc());

                MessageBox.Show("The audio was sent!", "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormSound_Load(object sender, EventArgs e)
        {

        }
    }
}
