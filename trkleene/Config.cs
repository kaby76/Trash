using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Value(0, Min = 1)]
        public IEnumerable<string> Expr { get; set; }
    }
}
