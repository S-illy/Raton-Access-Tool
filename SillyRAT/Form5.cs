using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

namespace SillyRAT
{
    public partial class Form5 : Form
    {
        private Form2 form2;
        public Form5(Form2 formm)
        {
            InitializeComponent();
            LoadJson();
            form2 = formm;
        }
        private void LoadJson()
        {
            string filePath = Path.Combine("Data", "blocked.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
                foreach (var item in jsonObject.IP)
                {
                    listBox1.Items.Add(item.ToString());
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (!listBox1.Items.Contains(textBox1.Text))
                {
                    listBox1.Items.Add(textBox1.Text);
                    form2.AddLog($"IP blocked by the user: {textBox1.Text}", Color.DarkGoldenrod);
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("This IP address is already in the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (listBox1.SelectedItem != null)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
            else
            {
                MessageBox.Show("Please select a IP Address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            var ipList = new List<string>();

            foreach (var item in listBox1.Items)
            {
                ipList.Add(item.ToString());
            }

            var jsonObject = new
            {
                IP = ipList
            };

            string filePath = Path.Combine("Data", "blocked.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            File.WriteAllText(filePath, json);

            MessageBox.Show("Configuration saved successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 10));
            button3.Region = new Region(CreateRoundPath(button3.ClientRectangle, 10));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 10));
            button5.Region = new Region(CreateRoundPath(button5.ClientRectangle, 10));
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
        private void Form5_Paint(object sender, PaintEventArgs e)
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            listBox1.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            this.Close();
        }
    }
}
