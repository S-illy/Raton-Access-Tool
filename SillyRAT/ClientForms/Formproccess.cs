using System;
using Stuff;
using Server.Connection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RatonRAT.ClientForms
{
    public partial class Formproccess : Form
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

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_VERT = 1;
        public SillyClient SillyClient { get; set; }
        public Formproccess()
        {
            InitializeComponent();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void Command(string command)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "ProcessSpy");
                pack.Set("Command", command);
                pack.Set("ProcessId", Convert.ToInt32(listViewItem.Tag));
                SillyClient.Send(pack.Pacc());
            }
        }

        private void killToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Kill");
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "ProcessSpy");
            pack.Set("Command", "List");
            SillyClient.Send(pack.Pacc());
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Pause");
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Resume");
        }

        private void processInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "ProcessSpy");
                pack.Set("Command", "Info");
                pack.Set("ProcessId", Convert.ToInt32(listViewItem.Tag));
                SillyClient.Send(pack.Pacc());
            }
        }

        private void Formproccess_Load(object sender, EventArgs e)
        {
            timer1.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.Paint += Form_Paint;
            ShowScrollBar(listView1.Handle, SB_VERT, true);
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
