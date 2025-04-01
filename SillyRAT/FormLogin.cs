using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT
{
    public partial class FormLogin : Form
    {
        public string Password { get; set; }
        public string SillyClient { get; set; }

        public FormLogin()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            this.Close();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            label2.Text = "Access to " + SillyClient;
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button2.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 10));
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
            Server.Classes.SFX.click();
            if (textBox1.Text == Password)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            } else
            {
                MessageBox.Show("Invalid password", "Auth", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                return;
            }
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormLogin_Paint(object sender, PaintEventArgs e)
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
