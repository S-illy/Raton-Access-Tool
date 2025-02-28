using Server.Connection;
using Stuff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormMessageBox : Form
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
        public FormMessageBox()
        {
            InitializeComponent();
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
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

        private void FormMessageBox_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "Error", "Warning", "Information" });
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "OK", "OK, Cancel", "Yes, No", "Yes, No, Cancel", "Retry, Cancel", "Abort, Retry, Ignore" });
            comboBox2.SelectedIndex = 0;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.TextLength < 1)
            {
                MessageBox.Show("Please insert some text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox2.TextLength < 1)
            {
                MessageBox.Show("Please insert some text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Pack pack = new Pack();
            pack.Set("Packet", "MsgBox");
            pack.Set("Icon", comboBox1.SelectedItem.ToString());
            pack.Set("Options", comboBox2.SelectedItem.ToString());
            pack.Set("Title", textBox2.Text.ToString());
            pack.Set("Message", textBox1.Text.ToString());
            SillyClient.Send(pack.Pacc());
            MessageBox.Show("Message box sent!", "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
