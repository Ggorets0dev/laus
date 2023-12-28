using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Laus.Models
{
    public class Config
    {
        static private readonly string _configPath = "./config.json";

        public List<string> AddressesBlacklist { get; set; }

        static public Config Get()
        {
            var serializer = new JsonSerializer();

            using (var streamReader = new StreamReader(_configPath))
            {
                using (var textReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize<Config>(textReader);
                }
            }
        }
    }
}
 