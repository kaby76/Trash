using System.Collections.Generic;
using CommandLine;

namespace Trash
{
    public class Config
    {
        [Value(0)]
        public IEnumerable<string> Files { get; set; }
    }
}
