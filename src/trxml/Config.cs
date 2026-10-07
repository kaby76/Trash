using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("bundle", Required = false, HelpText = "Compatibility alias for the default PAX/tar bundle output.")]
    public bool Bundle { get; set; }

    [Option("text", Required = false, HelpText = "Write XML text instead of a PAX/tar bundle.")]
    public bool Text { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "4.1.0";
}
