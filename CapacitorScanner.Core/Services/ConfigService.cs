using CapacitorScanner.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapacitorScanner.Core.Services
{
    public class ConfigService
    {
        private const string ConfigFile = "appsettings_desktop.json";

        public ConfigModel Config { get; private set; } = new ConfigModel();

        public  void LoadAsync()
        {
            string path = Path.Combine( ConfigFile);
            if (!File.Exists(path))
                using (_ = File.Create(path)) { }

            string text = File.ReadAllText(path);
            if (string.IsNullOrEmpty(text))
                 Save();
            ConfigModel? _config = JsonSerializer.Deserialize<ConfigModel>(text);
            if (_config is not null)
            {
                if (string.IsNullOrEmpty(_config.API_URL))
                    Config = new ConfigModel("http://10.89.1.99/api/", "pid/pibadgeverify", "pid/pistationactivity", "pid/view", "pid/pibadgeverifyStep2", "","scanner.db");
                else
                    Config = _config;
            }
            _config!.hostname = string.IsNullOrEmpty(_config.hostname) ? Dns.GetHostName() : _config.hostname;   
        }

        public  void Save()
        {

            string path = Path.Combine( ConfigFile);
            string json = JsonSerializer.Serialize(Config,new JsonSerializerOptions() { WriteIndented=true});
            File.WriteAllText(path,json);
        }
    }
}
