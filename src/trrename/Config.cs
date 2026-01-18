using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Value(0)] public IEnumerable<string> RenameMap { get; set; }

    [Option('e', "expr", Required = false, Default = "//(parserRuleSpec | lexerRuleSpec)//(RULE_REF | TOKEN_REF)")]
    public string Expr { get; set; }

    [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
    public string File { get; set; }

    [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
    public bool Format { get; set; }

    [Option('R', "rename-map-file", Required = false, Default = null)]
    public string RenameMapFile { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.36";
}
