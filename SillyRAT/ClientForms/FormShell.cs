using System;
using Server.Connection;
using System.Windows.Forms;
using Stuff;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace RatonRAT.ClientForms
{
    public partial class FormShell : Form
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
        public FormShell()
        {
            InitializeComponent();
        }

        private void FormShell_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 15));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 15));
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

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Type a command prompt command here then press enter :3")
            {
                textBox2.Text = "";
                textBox2.ForeColor = System.Drawing.SystemColors.GrayText;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = "Type a command prompt command here then press enter :3";
                textBox2.ForeColor = System.Drawing.SystemColors.GrayText;
            }
        }


        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (SillyClient != null || string.IsNullOrEmpty(textBox2.Text.Trim()))
                {
                    Pack ae = new Pack();
                    ae.Set("Packet", "Shell");
                    ae.Set("Command", textBox2.Text.Trim());
                    SillyClient.Send(ae.Pacc());
                    textBox2.Text = string.Empty;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
            if(textBox1.TextLength > 7000)
            {
                this.Close();
                MessageBox.Show("Too much text, please restart the hidden command", "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
