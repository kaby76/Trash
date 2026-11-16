using CommandLine;
using System;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string? File { get; set; }

        [Value(0, MetaName = "grammar-files", HelpText = "Use 'dotnet trash parse' for grammar files and pipe its bundle to combine.")]
        public IEnumerable<string> Inputs { get; set; } = Array.Empty<string>();

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	[Option("version", Required = false)]
	public string Version { get; set; } = "4.13.0";
    }
}
