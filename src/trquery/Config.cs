using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('i', "input", Required = false, HelpText = "Query as input string.")]
        public string Input { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Value(0)]
        public IEnumerable<string> Query { get; set; }
    }
}
