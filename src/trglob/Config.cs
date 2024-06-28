using CommandLine;
using System.Collections.Generic;

namespace Trash;

public class Config
{
    [Option('v', "verbose", Required = false)]
    public bool Verbose { get; set; }

    [Value(0)] public IEnumerable<string> Files { get; set; }

    public Config()
    {
        this.Files = new List<string>();
    }

    public Config(Config copy)
    {
        var ty = typeof(Config);
        foreach (var prop in ty.GetProperties())
        {
            if (prop.GetValue(copy, null) != null)
            {
                prop.SetValue(this, prop.GetValue(copy, null));
            }
        }
    }

    public static readonly Config DEFAULT = new Config();
}
