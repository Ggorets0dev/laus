using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Laus;
using Laus.Models;
using WinHardwareSpecs;
using Newtonsoft.Json;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var specification = SpecMonitor.GetSpecification();

            string jsonString = specification.ToJson();

            Console.WriteLine(jsonString);

            // var specification2 = JsonConvert.DeserializeObject<Specification>(jsonString);

            Console.ReadLine();
        }
    }
}
