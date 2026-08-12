using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('e', "no-prs", Required = false, HelpText = "No parsing result sets.")]
    public bool NoParsingResultSets { get; set; }

    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option('q', "query", Required = false, HelpText = "File containing the XPath expression.")]
    public string QueryFile { get; set; }

    [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
    public bool Format { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "2.0.1";

    [Value(0)] public IEnumerable<string> Expr { get; set; }
}
