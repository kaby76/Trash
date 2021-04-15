using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Value(0)]
        public string Expr { get; set; }

        [Value(1)]
        public string NewName { get; set; }
    }
}
