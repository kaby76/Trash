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

        [Option('a', "alphabetic", Required = false, Default = true)]
        public bool Alphabetic { get; set; }

        [Option('b', "bottom", Required = false, Default = false)]
        public bool Bottom { get; set; }

        [Option('t', "top", Required = false, Default = false)]
        public bool Top { get; set; }

        [Option("bfs", Required = false, Default = false)]
        public bool Bfs { get; set; }

        [Option("dfs", Required = false, Default = false)]
        public bool Dfs { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	[Option("version", Required = false)]
	public string Version { get; set; } = "0.23.41";
    }
}
