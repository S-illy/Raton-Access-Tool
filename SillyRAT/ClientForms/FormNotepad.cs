using System;
using Server.Connection;
using Stuff;
using System.Windows.Forms;
using SillyRAT;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RatonRAT.ClientForms
{
    public partial class FormNotepad : Form
    {
        public SillyClient SillyClient { get; set; }

        private bool isMouseDown = false;

        private Point mouseOffset;

        private void FormNotepad_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void FormNotepad_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void FormNotepad_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }


        public FormNotepad()
        {
            InitializeComponent();
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
        private void FormTTS_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Paint += Form_Paint;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            timer1.Start();
            this.MouseDown += FormNotepad_MouseDown;
            this.MouseMove += FormNotepad_MouseMove;
            this.MouseUp += FormNotepad_MouseUp;
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.TextLength < 1)
            {
                MessageBox.Show("Please insert some text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox2.TextLength < 3)
            {
                MessageBox.Show("Please make a longer title", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Pack pack = new Pack();
            pack.Set("Packet", "Notepad");
            pack.Set("Content", textBox1.Text);
            pack.Set("Title", textBox1.Text);
            SillyClient.Send(pack.Pacc());
            MessageBox.Show("Notepad sent to the client!", "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("Notepad Sent to client!", Color.Purple);
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
