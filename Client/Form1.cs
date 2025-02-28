using System;
using Client.Connection;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button2.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));

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
            SillyClient.Disconnect();
        }
    }
}
