using CommandLine;

namespace Trash
{
    public class Config
    {
        [Option('v', "verbose", Required = false)]
        public bool Verbose { get; set; }
        public Config() { }

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

	[Option("version", Required = false)]
	public string Version { get; set; } = "0.23.37";
    }
}
