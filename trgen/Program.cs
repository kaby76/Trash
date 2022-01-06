using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Trash
{

    public partial class Program
    {
        int return_value = 0;

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

        public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            HelpText helpText = null;
            if (errs.IsVersion())  //check if error is version request
                helpText = HelpText.AutoBuild(result);
            else
            {
                helpText = HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    h.Heading = "trgen";
                    h.Copyright = "Copyright (c) 2021 Ken Domino";
                    h.AddPreOptionsText(new Command().Help());
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
            }
            Console.Error.WriteLine(helpText);
        }

        private class MyError : Error
        {
            public MyError() : base(ErrorType.InvalidAttributeConfigurationError, true) { }
        }

        public void MainInternal(string[] args)
        {
            Config config = null;
            var cgen = new Command();
            // Parse options, stop if we see a bogus option, or something like --help.
            var result = new CommandLine.Parser().ParseArguments<Config>(args);
            bool stop = false;
            result.WithNotParsed(
                errs =>
                {
                    DisplayHelp(result, errs);
                    stop = true;
                });
            if (stop) return;
            result.WithParsed(o =>
            {
                config = o;
                if (File.Exists(cgen.ignore_file_name))
                {
                    var ignore = new StringBuilder();
                    var lines = File.ReadAllLines(cgen.ignore_file_name);
                    var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                    o.ignore_string = string.Join("|", ignore_lines);
                }

                // Overwrite the defaults with what was passed on the command line.
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }

                if (o.target != null && o.target == "Antlr4cs") config.name_space = "Test";
            });
            if (config.maven != null && !(bool)config.maven)
            {
                if (config.start_rule == null || config.start_rule == "")
                {
                    System.Console.Error.WriteLine("Missing --start-rule option.");
                    Program.DisplayHelp(result, new List<Error>() { new MyError() });
                    stop = true;
                }
            }
            if (stop) return;

            return_value = cgen.Execute(config);
        }
    }
}