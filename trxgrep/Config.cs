using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option("expr", Required = true)]
        public string Expr { get; set; }
    }
}
