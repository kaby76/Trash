using CommandLine;
using System;
using System.IO;
using System.Text.Json;

namespace Trash
{
    public class Config
    {
        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
        public Config()
        {
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
}
