using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RatonRAT.Classes
{
    public class LightMode
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
                userConfig = new UserConfig { audioMute = false, darkMode = false, notificationMute = false };
            }
        }

        private Color formColor = ColorTranslator.FromHtml("#fbfbfe");
        private Color buttonColor = ColorTranslator.FromHtml("#f0f0f4");
        private Color panelColor = ColorTranslator.FromHtml("#bfbfc9");
        private Color labelColor = ColorTranslator.FromHtml("#2b2a33");
        private Color listViewColor = ColorTranslator.FromHtml("#cfcfd8");
        private Color textBoxColor = ColorTranslator.FromHtml("#cfcfd8");
        private Color comboBoxColor = ColorTranslator.FromHtml("#cfcfd8");
        private Color radioButtonColor = ColorTranslator.FromHtml("#f0f0f4");
        private Color mainFormColor = ColorTranslator.FromHtml("#e0e0e6");

        public void Apply(Form form)
        {
            LoadConfig();
            if (userConfig.darkMode)
            {
                if (form.Text == "RatonRAT")
                {
                    form.BackColor = mainFormColor;
                }
                else
                {
                    form.BackColor = formColor;
                }
                ApplyColors(form);
            }
        }

        private void ApplyColors(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button button)
                {
                    if (button.BackColor != Color.FromArgb(84, 255, 189) && button.BackColor != Color.FromArgb(226, 40, 80))
                    {
                        button.FlatAppearance.MouseDownBackColor = buttonColor;
                        button.FlatAppearance.MouseOverBackColor = buttonColor;
                        button.BackColor = buttonColor;
                        button.ForeColor = labelColor;
                    }
                }

                else if (control is Panel)
                {
                    if (control.Name == "thebestpanel")
                    {
                        control.BackColor = Color.Transparent;
                    } else if (control.Name == "superpanel")
                    {
                        control.BackColor = formColor;
                    }
                    else if (control.Name == "line")
                    {
                        control.BackColor = formColor;
                    }
                    else
                    {
                        control.BackColor = panelColor;
                    }
                }
                else if (control is Label)
                {
                    control.ForeColor = labelColor;
                }
                else if (control is ListView listView)
                {
                    listView.BackColor = listViewColor;
                    listView.ForeColor = labelColor;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = textBoxColor;
                    textBox.ForeColor = labelColor;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = comboBoxColor;
                    comboBox.ForeColor = labelColor;
                }
                else if (control is RadioButton radioButton)
                {
                    radioButton.BackColor = radioButtonColor;
                    radioButton.ForeColor = labelColor;
                }
                else if (control is ListBox listBox)
                {
                    listBox.BackColor = listViewColor;
                    listBox.ForeColor = labelColor;
                }


                if (!(control is Button))
                    control.ForeColor = labelColor;

                if (control.HasChildren)
                    ApplyColors(control);
            }
        }

        private class UserConfig
        {
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
        }
    }
}
