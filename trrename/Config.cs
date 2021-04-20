using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('f', "file", Required = false)]
        public string File { get; set; }

        [Option('e', "expr", Required = false)]
        public string Expr { get; set; }

        [Option('n', "new-name", Required = false)]
        public string NewName { get; set; }

        [Option('r', "rename-map", Required = false)]
        public string RenameMap { get; set; }
    }
}
