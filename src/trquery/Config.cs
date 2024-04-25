using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('c', "commands", Required = false, HelpText = "Read xquery commands from file instead of command line arg.")]
        public string CommandFile { get; set; }

        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Value(0)]
        public IEnumerable<string> Query { get; set; }
    }
}
