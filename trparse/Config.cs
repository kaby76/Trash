using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option("version", Required = false, HelpText = "output version information and exit.")]
        public bool? Version { get; set; }

        [Value(0, Min = 1)]
        public IEnumerable<string> Grammars { get; set; }

        [Option("type", Required = false)]
        public string? Type { get; set; }
    }
}
