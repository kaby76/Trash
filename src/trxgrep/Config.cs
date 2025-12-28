using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('e', "no-prs", Required = false, HelpText = "No parsing result sets.")]
    public bool NoParsingResultSets { get; set; }

    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
    public bool Format { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.33";

    [Value(0, Min = 1)] public IEnumerable<string> Expr { get; set; }
}
