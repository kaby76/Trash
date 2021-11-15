using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
		[Value(0)]
	    public IEnumerable<string> Files { get; set; }

        [Option('i', "input", Required = false)]
        public string Input { get; set; }

        [Option('t', "type", Required = false)]
        public string Type { get; set; }

        [Option('s', "start-rule", Required = false, HelpText = "Start rule name.")]
        public string start_rule { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
    }
}
