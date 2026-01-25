using CommandLine;

namespace Trash;

public class Config
{
    [Option('d', "display-source", Required = false, HelpText = "Display the name of the source for the tree so it can be easily identified.")]
    public bool DisplayName { get; set; }
    
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('a', "antlr-style", Required = false, HelpText = "Output tree as Antlr ToStringTree() style.")]
    public bool AntlrStyle { get; set; }

    [Option('i', "indent-style", Required = false, HelpText = "Output tree as plain indented style.")]
    public bool IndentStyle { get; set; }

    [Option("paren-indent-style", Required = false, HelpText = "Output tree as parenthesized indented style.")]
    public bool ParenIndentStyle { get; set; }

    [Option('b', "block-style", Required = false, HelpText = "Output tree as block style.")]
    public bool BlockTreeStyle { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.41";
}
