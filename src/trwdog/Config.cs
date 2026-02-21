using CommandLine;

namespace Trash;

public class Config
{
    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('t', "timeout", Required = false, HelpText = "Max time in seconds for command to run")]
    public int? Timeout { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.42";
}
