using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trash;

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
            System.Console.Out.WriteLine();
            System.Console.Out.Flush();
            System.Console.Out.Close();
        }
    }

    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        HelpText helpText = null;
        if (errs.IsVersion()) //check if error is version request
            helpText = HelpText.AutoBuild(result);
        else
        {
            helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "trglob";
                h.Copyright = "Copyright (c) 2023 Ken Domino";
                h.AddPreOptionsText(new Command().Help());
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
        }

        Console.WriteLine(helpText);
    }

    private class MyError : Error
    {
        public MyError() : base(ErrorType.InvalidAttributeConfigurationError, true)
        {
        }
    }

    public void MainInternal(string[] args)
    {
        Config config = new Config();
        var cgen = new Command();
        // Parse options, stop if we see a bogus option, or something like --help.
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
            config = o;
            // Overwrite the defaults with what was passed on the command line.
            var ty = typeof(Config);
            foreach (var prop in ty.GetProperties())
            {
                if (prop.GetValue(o, null) != null)
                {
                    prop.SetValue(config, prop.GetValue(o, null));
                }
            }
        });
        if (stop) return;
        return_value = cgen.Execute(config);
    }
}
