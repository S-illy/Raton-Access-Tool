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
            if (textBox1.TextLength < 5)
            {
                MessageBox.Show("Insert a SAFE password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveshit(textBox1.Text);
            CreateCertificate("SLLRaton", textBox1.Text);

            this.Hide();
            this.Opacity = 0;
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void saveshit(string password)
        {
            try
            {
                var portz = new { Password = password };
                string json = JsonConvert.SerializeObject(portz, Formatting.Indented);
                string filePath = "DONOTSHARE.json";
                File.WriteAllText(filePath, json);
                File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "(Delete your certificate and DONOTSHARE.json hidden file)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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

                    MessageBox.Show("The SSLRaton certificate has been created and saved. Don't share it and don't delete it!", "Certificate", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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
    }
}
