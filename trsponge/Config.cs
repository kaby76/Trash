using CommandLine;
using System.Collections.Generic;

namespace Trash
{
    public class Config
    {
        [Value(0, Min = 2)]
        public IEnumerable<string> Files { get; set; }
    }
}
