using CommandLine;

namespace Trash;

public class Config
{
    [Option('d', "display-source", Required = false, HelpText = "In --text mode, display the source name with the tree.")]
    public bool DisplayName { get; set; }
    
    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option('a', "antlr-style", Required = false, HelpText = "Output tree exactly as Antlr ToStringTree() style (terminal token types omitted).")]
    public bool AntlrStyle { get; set; }

    [Option('A', "antlr-style-with-token-types", Required = false, HelpText = "Output an Antlr-like parenthesized tree including terminal token types.")]
    public bool AntlrStyleWithTokenTypes { get; set; }

    [Option('i', "indent-style", Required = false, HelpText = "Output tree as plain indented style.")]
    public bool IndentStyle { get; set; }

    [Option("paren-indent-style", Required = false, HelpText = "Output tree as parenthesized indented style.")]
    public bool ParenIndentStyle { get; set; }

    [Option('b', "block-style", Required = false, HelpText = "Output tree as block style.")]
    public bool BlockTreeStyle { get; set; }

    [Option("bundle", Required = false, HelpText = "Compatibility alias for the default PAX/tar bundle output.")]
    public bool Bundle { get; set; }

    [Option("text", Required = false, HelpText = "Write human-readable tree text instead of a PAX/tar bundle.")]
    public bool Text { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "4.2.0";
}
