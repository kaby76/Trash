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
            var cgen = new Command();
            var config = new Config();

            // Get default from OS, or just default.
            config.line_translation = Command.GetLineTranslationType();
            config.env_type = Command.GetEnvType();
            config.path_sep = Command.GetPathSep();
            config.antlr_tool_path = Command.GetAntlrToolPath();
            config.target = TargetType.CSharp;
            config.tool_grammar_files_pattern = "^(?!.*(/Generated|/target|/examples)).+g4$";
            config.output_directory = "Generated/";
            config.flatten = false;

            // Get any defaults from ~/.trgen.rc
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (System.IO.File.Exists(home + Path.DirectorySeparatorChar + cgen.SetupFfn))
            {
                var jsonString = System.IO.File.ReadAllText(home + Path.DirectorySeparatorChar + cgen.SetupFfn);
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

            if (File.Exists(cgen.ignore_file_name))
            {
                var ignore = new StringBuilder();
                var lines = File.ReadAllLines(cgen.ignore_file_name);
                var ignore_lines = lines.Where(l => !l.StartsWith("//")).ToList();
                cgen.ignore_string = string.Join("|", ignore_lines);
            }

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

                if (o.target != null && o.target == TargetType.Antlr4cs) config.name_space = "Test";
                if (o.name_space != null) config.name_space = o.name_space;
                if (o.flatten != null) config.flatten = o.flatten;
                if (o.all_source_pattern != null) config.all_source_pattern = config.all_source_pattern;
                else config.all_source_pattern =
                    "^(?!.*(" +
                     (cgen.ignore_string != null ? cgen.ignore_string + "|" : "")
                     + "ignore/|Generated/|target/|examples/|.git/|.gitignore|"
                     + Command.AllButTargetName((TargetType)config.target)
                     + "/)).+"
                     + "$";
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