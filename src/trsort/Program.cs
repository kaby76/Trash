using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Environment.ExitCode = 1;
                System.Console.Out.WriteLine();
                System.Console.Out.Flush();
                System.Console.Out.Close();
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
                    h.Heading = "trsort";
                    h.Copyright = "Copyright (c) 2023 Ken Domino"; //change copyright text
                    h.AddPreOptionsText(new Command().Help());
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
            }
            Console.WriteLine(helpText);
        }

        public void MainInternal(string[] args)
        {
            //foreach (var arg in args)
            //    System.Console.Error.WriteLine("arg " + arg);
            var config = new Config();
            var result = new CommandLine.Parser().ParseArguments<Config>(args);
            bool stop = false;
            result.WithNotParsed(
                errs =>
                {
                    if (errs.Any(x => x.GetType() == typeof(VersionRequestedError)))
                    {
                        System.Console.Out.WriteLine(config.Version);
                    }
                    else if (errs.Any(x => x.GetType() == typeof(HelpRequestedError)))
                    {
                        DisplayHelp(result, errs);
                    }
                    else
                    {
                        System.Console.Error.WriteLine("Error parsing command line: " + errs);
                    }
                    stop = true;
                });
            if (stop) return;
            result.WithParsed(o =>
            {
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            });
            new Command().Execute(config);
        }
    }
}
