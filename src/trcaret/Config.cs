using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option('H', "with-filename", Required = false, HelpText = "Print the file name for each match.")]
        public bool WithFileName { get; set; }
                
        [Option('p', "prefix", Required = false)]
        public bool Prefix { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

	[Option("version", Required = false)]
	public string Version { get; set; } = "0.23.38";
    }
}
