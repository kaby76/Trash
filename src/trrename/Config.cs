using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('e', "expr", Required = false, Default = "//(parserRuleSpec | lexerRuleSpec)//(RULE_REF | TOKEN_REF)")]
        public string Expr { get; set; }

        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('r', "rename-map", Required = false, Default = null)]
        public string RenameMap { get; set; }

        [Option('R', "rename-map-file", Required = false, Default = null)]
        public string RenameMapFile { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
    }
}
