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
            var config = Config.Get();

            Console.WriteLine(config.AddressesBlacklist[0]);

            Console.ReadLine();
        }
    }
}
