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
        static void Main(string[] args)
        {
            foreach (var inf in NetworkScanner.GetSelfAddresses())
                NetworkScanner.GetLanDevices(selfAddress: inf, timeout: 100);

            Console.ReadLine();
        }
    }
}
