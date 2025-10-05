using System.Collections.Generic;

namespace VRCSpeechToOSC
{
    public class ConfigValue
    {
        public string TargetHost { get; set; } = string.Empty;
        public int TargetPort { get; set; }
        public List<CommandGroup> CommandGroups { get; set; } = new();
    }

    public class CommandGroup
    {
        public string Prefix { get; set; } = string.Empty;
        public List<CommandDefinition> Commands { get; set; } = new();
    }

    public class CommandDefinition
    {
        public List<string> Prompts { get; set; } = new();
        public string OscPath { get; set; }　= string.Empty;
        public object? Value { get; set; } // null許容: 値なしの場合がある
        public string? Type { get; set; }  // null許容: 値なしの場合がある
    }
}
