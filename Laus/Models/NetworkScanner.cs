using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Laus.Models
{
    /// <summary>
    /// Работа с локальными сетями
    /// </summary>
    static internal class NetworkScanner
    {
        static private readonly List<IPAddress> _addressesBlacklist = new List<IPAddress>();

        static NetworkScanner() 
        {
            try
            {
                var config = Config.Get();

                foreach (var address in config.AddressesBlacklist)
                    _addressesBlacklist.Add(IPAddress.Parse(address));
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("Не удается получить список игнорируемых IP адресов по причине отсутствия файла конфигурации", "Файл конфигурации отсутствует", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        static public List<DeviceViewModel> GetLanDevices(IPAddress selfAddress, ushort timeout)
        {
            var lanDevices = new List<DeviceViewModel>();

            string selfAddressStr = selfAddress.ToString();
            string baseAddress = selfAddressStr.Substring(0, selfAddressStr.LastIndexOf('.') + 1);

            var numberOfThreads = new ParallelOptions { MaxDegreeOfParallelism = 16 };
            Parallel.For(1, 255, numberOfThreads, SearchAvailableDevice);

            void SearchAvailableDevice(int count)
            {
                string strAddress = baseAddress + count.ToString();

                var pingAddress = IPAddress.Parse(strAddress);

                if (Equals(pingAddress, selfAddress) || _addressesBlacklist.Contains(pingAddress))
                    return;

                if (Client.Ping(strAddress, timeout))
                {
                    try
                    {
                        var client = new Client(strAddress);

                        var checkResult = client.CheckUser();

                        if (!checkResult.isApproved)
                            return;

                        var device = new DeviceViewModel{ IpAddress = strAddress, Alias = checkResult.alias };

                        lanDevices.Add(device);
                    }
                    catch { } // Skip device
                }
            }

            return lanDevices;
        }

        static public List<IPAddress> GetSelfAddresses()
        {
            var selfAddresses = new List<IPAddress>();

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var iface in interfaces)
            {
                if (iface.OperationalStatus != OperationalStatus.Up || iface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                var ipProperties = iface.GetIPProperties();
                var ipAddresses = ipProperties.UnicastAddresses;

                foreach (var ipAddress in ipAddresses)
                {
                    if (ipAddress.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    var myIpAddress = ipAddress.Address;
                    selfAddresses.Add(myIpAddress);
                }
            }

            return selfAddresses;
        }
    }
}
