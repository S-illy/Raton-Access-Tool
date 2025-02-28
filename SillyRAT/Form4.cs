using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.notification();
            label6.Text = "Bitcoin address copied, thanks!";
            label6.ForeColor = Color.LimeGreen;
            Clipboard.SetText("bc1qd2088hs8ajg9qv3m2f3p285e7r7ntmutygtux6");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            this.Close();
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

        private void Form4_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 10));
        }
    }
}
