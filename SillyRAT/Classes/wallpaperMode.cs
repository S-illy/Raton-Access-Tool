using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RatonRAT.Classes
{
    public class WallpaperMode
    {
        private static UserConfig userConfig;

        private static void LoadConfig()
        {
            string configFilePath = "Data/userConfig.json";
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                userConfig = JsonConvert.DeserializeObject<UserConfig>(json);
            }
            else
            {
                userConfig = new UserConfig { audioMute = false, darkMode = false, notificationMute = false, wallpaper = false };
            }
        }

        public void Apply(Form form)
        {
            LoadConfig();
            if (userConfig.wallpaper)
            {
                ApplyOpacity(form);
                string dataFolder = Path.Combine(Application.StartupPath, "Data");
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                string wallpaperPath = null;

                foreach (var extension in validExtensions)
                {
                    string potentialPath = Path.Combine(dataFolder, "wallpaper" + extension);
                    if (File.Exists(potentialPath))
                    {
                        wallpaperPath = potentialPath;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(wallpaperPath))
                {
                    try
                    {
                        Image wallpaperImage = Image.FromFile(wallpaperPath);
                        form.BackgroundImage = wallpaperImage;
                        form.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    catch
                    {
                        MessageBox.Show("Error: The selected file is not a valid image or could not be loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Error: The wallpaper file does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ApplyOpacity(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox || control is Button)
                {
                    if (!(control is Form))
                    {
                        control.BackColor = SetOpacity(control.BackColor, 128);
                        control.ForeColor = SetOpacity(control.ForeColor, 128);
                    }
                    if (control.HasChildren)
                        ApplyOpacity(control);
                }
                else if (control is Panel)
                {
                    if (!(control is Form) && control.Name != "superpanel")
                    {
                        control.BackColor = SetOpacity(control.BackColor, 128);
                        control.ForeColor = SetOpacity(control.ForeColor, 128);
                    }
                }
                else if (control is PictureBox)
                {
                    if (!(control is Form))
                    {
                        control.BackColor = SetOpacity(control.BackColor, 0);
                        control.ForeColor = SetOpacity(control.ForeColor, 128);
                    }
                    else if (control is Label)
                    {
                        control.BackColor = SetOpacity(control.BackColor, 0);
                        control.ForeColor = SetOpacity(control.ForeColor, 128);
                    }
                }
                else
                {
                    continue;
                }
            }
        }



        private Color SetOpacity(Color color, byte opacity)
        {
            return Color.FromArgb(opacity, color.R, color.G, color.B);
        }

        private class UserConfig
        {
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
            public bool wallpaper { get; set; }
        }
    }
}