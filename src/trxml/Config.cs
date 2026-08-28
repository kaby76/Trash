using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("bundle", Required = false, HelpText = "Write a PAX bundle, replacing .pt members with .xml members.")]
    public bool Bundle { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "3.0.0";
}
