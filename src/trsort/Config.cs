using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false, HelpText = "Read parse tree data from file instead of stdin.")]
        public string File { get; set; }

        [Option("fmt", Required = false, HelpText = "Output formatted parsing results set.")]
        public bool Format { get; set; }

        [Option('a', "alphabetic", Required = false, Default = false, HelpText = "Sort parser rules alphabetically (default when no other sort specified).")]
        public bool Alphabetic { get; set; }

        [Option("bfs", Required = false, Default = false, HelpText = "Sort parser rules in BFS order from the named start rule.")]
        public bool Bfs { get; set; }

        [Option("dfs", Required = false, Default = false, HelpText = "Sort parser rules in DFS order from the named start rule.")]
        public bool Dfs { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }

        [Option("version", Required = false)]
        public string Version { get; set; } = "3.2.0";

        [Value(0, Required = false, HelpText = "Name of the start rule (optional for --bfs and --dfs; auto-detected when omitted).")]
        public string Expr { get; set; }
    }
}
