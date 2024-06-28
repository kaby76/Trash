using CommandLine;

namespace Trash;

public class Config
{
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('a', "antlr-style", Required = false, HelpText = "Output tree as Antlr ToStringTree() style.")]
    public bool AntlrStyle { get; set; }
}
