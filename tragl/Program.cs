using CommandLine;
using System;

namespace Trash
{

	public class Program
	{

		public static void Main(string[] args)
		{
			try
			{
				new Program().MainInternal(args);
			}
			catch (Exception e)
			{
				System.Console.Error.WriteLine(e.ToString());
				System.Environment.Exit(1);
			}
		}

		public void MainInternal(string[] args)
		{
			var config = new Config();
		}
	}
}
