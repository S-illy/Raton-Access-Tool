using Newtonsoft.Json;
using Server.Connection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormKeylogger : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_VERT = 1;
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

        public FormKeylogger()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormKeylogger_Load(object sender, EventArgs e)
        {
            this.Paint += Form_Paint;
            timer1.Start();
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            ShowScrollBar(listView1.Handle, SB_VERT, true);
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

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string time = selectedItem.SubItems[0].Text;
                string key = selectedItem.SubItems[1].Text;
                string textToCopy = $"Time: {time}, Key(s): {key}";
                Clipboard.SetText(textToCopy);
            }
        }

        private void saveLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logDict = new Dictionary<string, string>();

            foreach (ListViewItem item in listView1.Items)
            {
                string date = item.SubItems[0].Text;
                string key = item.SubItems[1].Text;

                logDict[date] = key;
            }

            string json = JsonConvert.SerializeObject(logDict, Formatting.Indented);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = "json";
            saveFileDialog.FileName = "logs.json";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, json);
            }
        }
    }
}
