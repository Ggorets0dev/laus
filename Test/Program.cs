using System;
using System.Collections.Generic;
using System.Linq;
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
            var selfAddresses = NetworkScanner.GetSelfAddresses();

            var firstAddress = selfAddresses[0];
            var secondAddress = selfAddresses[1];

            //Console.WriteLine($"First IP is {firstAddress}");
            //Console.WriteLine($"Second IP is {secondAddress}");

            foreach( var address in NetworkScanner.GetLanDevices(secondAddress, 100)) 
            {
                Console.WriteLine(address.ToString());
            }

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}
