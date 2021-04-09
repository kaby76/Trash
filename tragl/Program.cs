using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

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

		void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
		{
			HelpText helpText = null;
			if (errs.IsVersion())  //check if error is version request
				helpText = HelpText.AutoBuild(result);
			else
			{
				helpText = HelpText.AutoBuild(result, h =>
				{
					h.AdditionalNewLineAfterOption = false;
					h.Heading = "tragl";
					h.Copyright = "Copyright (c) 2021 Ken Domino"; //change copyright text
					//h.AddPreOptionsText(new CTokens().Help());
					return HelpText.DefaultParsingErrorsHandler(result, h);
				}, e => e);
			}
			Console.Error.WriteLine(helpText);
		}

		public void MainInternal(string[] args)
		{
			var config = new Config();
		}
	}
}
