using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tmds.DBus.Protocol;

namespace AllowanceApp.Avalonia.Service
{
    public class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AllowanceApp", "Configuration");

        private readonly string _configFile;
        private Dictionary<string, string> _configData = [];

        public ConfigManager(string configName)
        {
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
            }
            _configFile = Path.Combine(ConfigPath, configName);
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (!File.Exists(_configFile))
            {
                File.WriteAllText(_configFile, "{ }");
            }
            var text = File.ReadAllText(_configFile);
            _configData = JsonSerializer.Deserialize<Dictionary<string, string>>(text) ?? [];
        }

        public void SaveConfig()
        {
            var text = JsonSerializer.Serialize(_configData);
            File.WriteAllText(_configFile, text);    
        }

        public string GetProperty(string key, string defaultValue, bool autoSave = false)
        {
            if (_configData.TryGetValue(key, out var value))
            {
                return value;
            }

            SetProperty(key, defaultValue, autoSave);
            return defaultValue;
        }

        public T GetProperty<T>(string key, T defaultValue, bool autoSave = false)
        {
            if (_configData.TryGetValue(key, out var value))
            {
                try
                {
                    T response = (T)Convert.ChangeType(value, typeof(T));
                    return response;
                }
                catch (Exception)
                {
                    // we'll fall through to the SetProperty call here and return the default value
                }
            }
            SetProperty(key, defaultValue, autoSave);
            return defaultValue;
        }

        public void SetProperty(string key, string value, bool autoSave = false)
        {
            _configData[key] = value;
            if (autoSave)
            {
                SaveConfig();
            }
        }

        public void SetProperty<T>(string key, T value, bool autoSave = false)
        {
            _configData[key] = value?.ToString() ?? "null";
            if (autoSave)
            {
                SaveConfig();
            }
        }
    }
}
