using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;

namespace VRCSpeechToOSC
{
    /// <summary>
    /// Config class. This is singleton
    /// </summary>
    internal class Config
    {
        public const string OSC_TYPE_INT = "int";
        public const string OSC_TYPE_BOOL = "bool";
        public const string OSC_TYPE_FLOAT = "float";
        public const string OSC_TYPE_STRING = "string";

        private static ConfigValue? _instance;
        public static ConfigValue Instance
        {
            get
            {
                if (_instance == null) LoadFromYaml();
                return _instance!;
            }
        }

        public static string DefaultConfigFilePath => Path.Combine(AppContext.BaseDirectory, "config.yml");

        public static void LoadFromYaml(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = DefaultConfigFilePath;
            }
            if (!File.Exists(path))
            {
                Console.WriteLine($"[Error] Config file: {path} is missing!");
                Environment.Exit(1);
                return;
            }

            var yamlText = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build();
            var config = deserializer.Deserialize<ConfigValue>(yamlText);
            if (config != null)
            {
                _instance = config;
            }
        }

        public void SaveToYaml(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = DefaultConfigFilePath;
            }

            var serializer = new SerializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(this);
            File.WriteAllText(path, yaml);
        }
    }
}
