using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Laus.Models
{
    /// <summary>
    /// Настройки, сохраняемые в ПЗУ устройства
    /// </summary>
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

        /// <summary>
        /// Получить объект файла конфигурации по заранее известному пути
        /// </summary>
        /// <returns>Конфигурация</returns>
        static public Config Get()
        {
            if (!Exist())
                throw new FileNotFoundException();

            var serializer = new JsonSerializer();

            using (var streamReader = new StreamReader(_configPath))
            {
                using (var textReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize<Config>(textReader);
                }
            }
        }

        /// <summary>
        /// Проверить, существует ли файл конфигурации по заранее известному пути
        /// </summary>
        /// <returns>Наличие файла конфигурации</returns>
        static public bool Exist() => File.Exists(_configPath);

        /// <summary>
        /// Сохранить файл конфигурации по заранее известному пути
        /// </summary>
        public void Save()
        {
            string objJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            
            var writeStream = new StreamWriter(new FileStream(_configPath, FileMode.Create, FileAccess.Write));
            
            writeStream.Write(objJson);
            writeStream.Close();
        }
    }
}
 