using CommandLine;

namespace trinterp;

public class Config
{
    [Option('o', "output-directory", Required = false, HelpText = "Output directory for .interp and .tokens files.")]
    public string OutputDirectory { get; set; } = ".";

    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option("actions-in-interp", Required = false, HelpText = "Append grammar actions and semantic predicates as strings to the .interp file.")]
    public bool ActionsInInterp { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "1.0.0";
}
