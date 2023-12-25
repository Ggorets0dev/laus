using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Laus.Models
{
    static public class NetworkScanner // TODO: make internal
    {
        static public List<IPAddress> GetLanDevices(IPAddress selfAddress, ushort timeout)
        {
            var lanDevices = new List<IPAddress>();

            string selfAddressStr = selfAddress.ToString();
            string baseAddress = selfAddressStr.Substring(0, selfAddressStr.LastIndexOf('.')) + '.';

            var numberOfThreads = new ParallelOptions { MaxDegreeOfParallelism = 16 };
            Parallel.For(1, 255, numberOfThreads, SearchAvailableDevice);

            void SearchAvailableDevice(int count)
            {
                var pingAddress = IPAddress.Parse(baseAddress + count);

                if (Equals(pingAddress, selfAddress))
                    return;

                try
                {
                    var pingReply = new Ping().Send(pingAddress, timeout);

                    if (pingReply == null || pingReply.Status != IPStatus.Success)
                        return;

                    lanDevices.Add(pingAddress);
                }
                catch
                {
                    return;
                }
            }

            return lanDevices;
        }

        static public List<IPAddress> GetSelfAddresses() // TODO: make private
        {
            var selfAddresses = new List<IPAddress>();

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.OperationalStatus == OperationalStatus.Up && iface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    var ipProperties = iface.GetIPProperties();
                    var ipAddresses = ipProperties.UnicastAddresses;

                    foreach (var ipAddress in ipAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            var myIpAddress = ipAddress.Address;
                            selfAddresses.Add(myIpAddress);
                        }
                    }
                }
            }

            return selfAddresses;
        }
    }
}
