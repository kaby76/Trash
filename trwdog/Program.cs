namespace Trash
{
    using CommandLine;
    using CommandLine.Text;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class Program
    {
        public string SetupFfn = ".trwdog.rc";

        static void Main(string[] args)
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
                    h.Heading = "trwdog";
                    h.Copyright = "Copyright (c) 2021 Ken Domino"; //change copyright text
                    h.AddPreOptionsText(new Command().Help());
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
            }
            Console.Error.WriteLine(helpText);
        }

        public void MainInternal(string[] args)
        {
            // Find point in sequence of args and split options from program.
            var divide = 0;
            for (int i = 0; i < args.Length; ++i, divide = i)
            {
                var arg = args[i];
                if (arg[0] == '-' || arg[0] == '/')
                {
                    if (arg.ToLower() == "--timeout" || arg.ToLower() == "-t")
                        ++i;
                    continue;
                }
                else
                {
                    break;
                }
            }
            var command = args.Skip(divide).ToArray();
            var opts = new string[divide];
            Array.Copy(args, opts, divide);
            var config = new Config();

            // Get default from OS, or just default.
            config.Timeout = 300;

            // Get any defaults from ~/.trwdog.rc
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + SetupFfn);
                var o = JsonSerializer.Deserialize<Config>(jsonString);
                var ty = typeof(Config);
                foreach (var prop in ty.GetProperties())
                {
                    if (prop.GetValue(o, null) != null)
                    {
                        prop.SetValue(config, prop.GetValue(o, null));
                    }
                }
            }

            var result = new CommandLine.Parser().ParseArguments<Config>(opts);
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
			new Command().Execute(config, command);
        }
    }
}
