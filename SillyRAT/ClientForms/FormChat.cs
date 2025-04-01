using Newtonsoft.Json;
using Server.Connection;
using SillyRAT;
using Stuff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormChat : Form
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
        public string Username { get; set; }

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_VERT = 1;

        public FormChat()
        {
            InitializeComponent();
        }

        private void FormChat_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            listView1.Region = new Region(CreateRoundPath(listView1.ClientRectangle, 15));
            textBox2.Region = new Region(CreateRoundPath(textBox2.ClientRectangle, 15));
            button5.Region = new Region(CreateRoundPath(button5.ClientRectangle, 15));
            timer1.Start();
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


        private void button5_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "CloseChat");
            SillyClient.Send(pack.Pacc());
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox2.ForeColor = Color.White;
        }

        static byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                byte[] imageBytes = ImageToByteArray(imageList1.Images[2]);
                ListViewItem meow = new ListViewItem("[" +Username+"]: " + textBox2.Text);
                meow.ImageIndex = 2;
                listView1.Items.Add(meow);
                Pack pack = new Pack();
                pack.Set("Packet", "ChatMessage");
                pack.Set("img", imageBytes);
                pack.Set("Message", "[" + Username + "]: " + textBox2.Text);
                SillyClient.Send(pack.Pacc());
                e.SuppressKeyPress = true;
                textBox2.Clear();
            }
        }

        private void saveChatLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clientsFolder = Program.ClientsFolder;
            string targetFolder = Path.Combine(clientsFolder, SillyClient.uid);

            if (Directory.Exists(targetFolder))
            {
                var logList = new List<Dictionary<string, string>>();

                foreach (ListViewItem item in listView1.Items)
                {
                    var logEntry = new Dictionary<string, string>
            {
                { "Message", item.Text },
            };
                    logList.Add(logEntry);
                }

                try
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string filePath = Path.Combine(targetFolder, $"clientchat_{timestamp}.json");

                    File.WriteAllText(filePath, JsonConvert.SerializeObject(logList, Formatting.Indented));

                    MessageBox.Show($"Chat logs exported successfully to:\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void copyMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(listView1.SelectedItems[0].Text);
        }
    }
}
