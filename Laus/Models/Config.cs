using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Laus.Models
{
    internal class Config
    {
        static private readonly string _configPath = "./config.json";
        static private readonly ushort _maxTimeoutMs = 60_000;
        static private readonly byte _maxAliasLength = 15;

        static public ushort MaxTimeoutMs => _maxTimeoutMs;
        static public byte MaxAliasLength => _maxAliasLength;

        public List<string> AddressesBlacklist { get; set; }
        public ushort TimeoutMs { get; set; }
        public string Alias { get; set; }

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

        public void Save()
        {
            string objJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            
            var writeStream = new StreamWriter(new FileStream(_configPath, FileMode.Create, FileAccess.Write));
            
            writeStream.Write(objJson);
            writeStream.Close();
        }
    }
}
 