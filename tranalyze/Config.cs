using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('s', "start-rules", Required = false, HelpText = "Start rule names.")]
        public IEnumerable<string> start_rules { get; set; }
    }
}
