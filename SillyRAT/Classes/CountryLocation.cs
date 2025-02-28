using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

public class CountryLocation
{
    public static (double Lat, double Lon)? GetCapitalCoordinates(string countryName)
    {
        try
        {
            using (WebClient wc = new WebClient())
            {
                string apiUrl = $"https://restcountries.com/v3.1/name/{Uri.EscapeDataString(countryName)}";
                string json = wc.DownloadString(apiUrl);

                JArray jsonArray = JArray.Parse(json);
                if (jsonArray.Count > 0)
                {
                    var capitalInfo = jsonArray[0]["capitalInfo"];
                    if (capitalInfo != null && capitalInfo["latlng"] != null)
                    {
                        double lat = (double)capitalInfo["latlng"][0];
                        double lon = (double)capitalInfo["latlng"][1];
                        return (lat, lon);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("We can't get the flag: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return null;
    }
}
