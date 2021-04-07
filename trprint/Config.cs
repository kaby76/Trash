using CommandLine;
using System.Collections.Generic;

namespace Trash
{
	public class Config
	{
		[Option("version", Required = false, HelpText = "output version information and exit.")]
		public bool? Version { get; set; }
	}
}
