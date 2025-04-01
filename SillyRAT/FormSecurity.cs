using Newtonsoft.Json;
using SillyRAT;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace RatonRAT
{
    public partial class FormSecurity : Form
    {
        public FormSecurity()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (textBox3.TextLength < 1)
            {
                MessageBox.Show("Insert a username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.TextLength < 5)
            {
                MessageBox.Show("Insert a SAFE password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveshit(textBox3.Text, textBox1.Text, DateTime.Now.ToString("dd/MM/yyyy"));
            CreateCertificate("SLLRaton", textBox1.Text);
            Environment.Exit(0);
        }

        private void saveshit(string username, string password, string date)
        {
            try
            {
                var portz = new { Username = username, Password = password, Date = date };
                string json = JsonConvert.SerializeObject(portz, Formatting.Indented);
                string filePath = "DONOTSHARE.json";
                File.WriteAllText(filePath, json);
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
                pfp();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "(Delete your certificate and DONOTSHARE.json hidden file)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void pfp()
        {
            try
            {
                if (pictureBox2.Image != null)
                {
                    string dataFolder = Path.Combine(Application.StartupPath, "Data");

                    string extension = ".png";
                    if (pictureBox2.Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                        extension = ".jpg";
                    else if (pictureBox2.Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                        extension = ".bmp";
                    else if (pictureBox2.Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                        extension = ".gif";

                    string filePath = Path.Combine(dataFolder, "pfp" + extension);
                    pictureBox2.Image.Save(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving pfp: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CreateCertificate(string certName, string certPassword)
        {
            try
            {
                using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(2048))
                {
                    var certRequest = new CertificateRequest("CN=" + certName, rsa, System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
                    X509Certificate2 certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

                    X509Store store = new X509Store(StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadWrite);

                    byte[] certData = certificate.Export(X509ContentType.Pfx, certPassword);
                    File.WriteAllBytes("SSLRaton.pfx", certData);

                    store.Close();

                    MessageBox.Show("Local profile created, your connection now is encrypted, please restart the program to take effects", "Certificate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FormSecurity_Load(object sender, EventArgs e)
        {
            new Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 10));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 10));
            pictureBox2.Region = new Region(CreateRoundPath(pictureBox2.ClientRectangle, 20));
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

        private void FormSecurity_Paint(object sender, PaintEventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Profile pictures|*.jpg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select a profile picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = Image.FromFile(openFileDialog.FileName);
                }
            }
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.White;
            textBox1.UseSystemPasswordChar = true;
            textBox1.Clear();
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.ForeColor = Color.White;
            textBox3.Clear();
        }
    }
}