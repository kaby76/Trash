using AntlrJson;
using ParseTreeEditing.UnvParseTreeDOM;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Trash
{
    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("tranalyze.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Execute(Config config)
        {
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from stdin");
                }

                for (;;)
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }

                lines = lines.Trim();
            }
            else
            {
                if (config.Verbose)
                {
                    System.Console.Error.WriteLine("reading from file >>>" + config.File + "<<<");
                }

                lines = File.ReadAllText(config.File);
            }

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParsingResultSetSerializer());
            serializeOptions.WriteIndented = config.Format;
            serializeOptions.MaxDepth = 10000;
            AntlrJson.ParsingResultSet[] data =
                JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (AntlrJson.ParsingResultSet parse_info in data)
            {
                if (config.Verbose) LoggerNs.TimedStderrOutput.WriteLine("starting deserialization");
                var trees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                if (config.Verbose)
                {
                    foreach (var n in trees)
                        System.Console.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
                }
                AnalyzeDoc();
            }
        }


        public void AnalyzeDoc()
        {
            // Find p : a q* r b where q =>* r or
            //      p : a q+ r b where q =>* r
        }
    }
}
