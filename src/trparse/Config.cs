using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Value(0)] public IEnumerable<string> Files { get; set; }

    [Option('a', "ambig", Required = false, HelpText = "Output ambiguous parse trees.")]
    public bool Ambig { get; set; }

    [Option('d', "dll", Required = false, HelpText = "Search for parser in dll with this specified name.")]
    public string Dll { get; set; } = "Test";

    [Option("no-prs", Required = false, HelpText = "Output parse errors only. No parsing result sets.")]
    public bool NoParsingResultSets { get; set; }

    [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
    public bool Format { get; set; }

    [Option('g', "encoding", Required = false, HelpText = "Set the encoding for the grammar.")]
    public string Encoding { get; set; }

    [Option("group", Required = false, HelpText = "Group by file name.")]
    public bool GroupBy { get; set; }

    [Option('i', "input", Required = false, HelpText = "Parse input string.")]
    public string Input { get; set; }

    [Option('l', "line", Required = false, HelpText = "Include line/column information in parse tree.")]
    public bool LineNumbers { get; set; }

    [Option('p', "parser", Required = false,
        HelpText = "Location of pre-built parser (aka the trgen Generated/ directory)")]
    public string ParserLocation { get; set; }

    [Option('q', "quiet", Required = false, Default = false, HelpText = "Do not output anything; only set error code.")]
    public bool Quiet { get; set; }

    [Option('t', "type", Required = false, HelpText = "Override type of parse.")]
    public string Type { get; set; }

    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Option("version", Required = false)]
    public string Version { get; set; } = "0.23.41";

    [Option('x', Required = false, HelpText = "Read input file names from stdin.")]
    public bool ReadFileNameStdin { get; set; }

    [Option("xf", Required = false, HelpText = "Read input file names from file provided on command line.")]
    public string ReadFileNameFile { get; set; }
}
