using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('c', "clobber", Required = false, Default = false)]
    public bool Clobber { get; set; }

    [Option('o', "output-directory", Required = false)]
    public string OutputDirectory { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("bundle", Required = false, HelpText = "Materialize an ordinary POSIX PAX artifact bundle (input is detected automatically).")]
    public bool Bundle { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "3.2.0";
}
