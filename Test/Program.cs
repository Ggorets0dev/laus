using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Laus;
using Laus.Models;

namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client();

            foreach (var ip in NetworkScanner.GetSelfAddresses())
            {
                Console.WriteLine(NetworkScanner.GetLanDevices(ip, timeout: 300).Count);
            }

            Console.ReadLine();
        }
    }
}
