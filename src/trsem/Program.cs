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
                Environment.ExitCode = 1;
                // Write something to avoid https://github.com/kaby76/Domemtech.Trash/issues/134
                // and https://github.com/dotnet/runtime/issues/50780
                System.Console.Out.WriteLine();
                System.Console.Out.Flush();
                System.Console.Out.Close();
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
                    h.Heading = "trsem";
                    h.Copyright = "Copyright (c) 2023 Ken Domino";
                    h.AddPreOptionsText(new Command().Help());
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
            }
            Console.WriteLine(helpText);
        }

        private class MyError : Error
        {
            public MyError() : base(ErrorType.InvalidAttributeConfigurationError, true) { }
        }

        public void MainInternal(string[] args)
        {
            var config = new Config();

            // Get default from OS, or just default.
            config.output_directory = "Generated/";
            config.target = TargetType.CSharp;

            // Get any defaults from ~/.trgen.rc

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
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            });

            return_value = new Command().Execute(config);
        }
    }
}