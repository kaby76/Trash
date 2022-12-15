using CommandLine;

namespace Trash
{
	public class Config
	{
		[Option('f', "file", Required = false)]
		public string File { get; set; }

		[Option("fmt", Required = false)]
		public bool Format { get; set; }

		[Option('v', "verbose", Required = false)]
		public bool Verbose { get; set; }
	}
}
