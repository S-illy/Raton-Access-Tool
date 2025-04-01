using System;
using Stuff;
using System.Drawing;
using Server.Connection;
using System.Windows.Forms;
using System.IO;
using SillyRAT;
using System.Drawing.Drawing2D;

namespace RatonRAT.ClientForms
{
    public partial class FormWallpaper : Form
    {
        public SillyClient SillyClient { get; set; }

        private bool isMouseDown = false;

        private Point mouseOffset;

        private void FormWallpaper_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void FormWallpaper_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void FormWallpaper_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        public FormWallpaper()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a wallpaper";
                openFileDialog.Filter = "Windows supported|*.jpg;*.jpeg;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string selectedImagePath = openFileDialog.FileName;
                        string fileName = System.IO.Path.GetFileName(selectedImagePath);
                        label2.Text = fileName;
                        pictureBox1.BackgroundImage = Image.FromFile(selectedImagePath);

                        byte[] imageBytes = File.ReadAllBytes(selectedImagePath);
                        string imageBase64 = Convert.ToBase64String(imageBytes);

                        Pack pack = new Pack();
                        pack.Set("Packet", "Wallpaper");
                        pack.Set("Image", imageBase64);
                        SillyClient.Send(pack.Pacc());
                        MessageBox.Show("Wallpaper sent to the client!", "Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog("Wallpaper sent to client successfully!", Color.Purple);
                        }));
                    } catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message
                            , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Program.form2.Invoke(new Action(() =>
                        {
                            Program.form2.AddLog($"New exception: {ex.Message} | The operation is now cancelled", Color.FromArgb(255, 164, 54));
                        }));
                        return;
                    }
                }
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormWallpaper_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            this.Paint += Form_Paint;
            timer1.Start();
            this.MouseDown += FormWallpaper_MouseDown;
            this.MouseMove += FormWallpaper_MouseMove;
            this.MouseUp += FormWallpaper_MouseUp;
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 15));
            button2.Region = new Region(CreateRoundPath(button1.ClientRectangle, 15));
            pictureBox1.Region = new Region(CreateRoundPath(pictureBox1.ClientRectangle, 15));
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
