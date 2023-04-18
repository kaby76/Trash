using ParseTreeEditing.UnvParseTreeDOM;

namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trconvert.readme.md"))
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
                for (; ; )
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
            AntlrJson.ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            List<ParsingResultSet> results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var text = parse_info.Text;
                var fn = parse_info.FileName;
                var trees = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                if (config.Verbose)
                {
                    foreach (var n in trees)
                        System.Console.WriteLine(TreeOutput.OutputTree(n, lexer, parser).ToString());
                }
                // Get original source file extension and derive type.
                var ext = Path.GetExtension(fn);
                var parser_type = ext switch
                {
                    ".g4" => "antlr4",
                    ".g3" => "antlr3",
                    ".g2" => "antlr2",
                    ".gram" => "pegen",
                    ".lark" => "lark",
                    ".rex" => "rex",
                    ".y" => "bison",
                    _ => throw new Exception("Unknown file extension, cannot load in a built-in parser.")
                };

                Dictionary<string, string> res = null;
                //if (parser_type == "antlr3")
                //{
                //    var imp = new LanguageServer.ConvertAntlr3();
                //    res = imp.Try(trees, parser, fn, "antlr4");
                //}
                //else if (parser_type == "antlr2")
                //{
                //    var imp = new LanguageServer.ConvertAntlr2();
                //    res = imp.Try(doc.FullPath, doc.Code, out_type);
                //}
                //else if (parser_type == "bison")
                //{
                //    var imp = new LanguageServer.ConvertBison();
                //    res = imp.Try(doc.FullPath, doc.Code, out_type);
                //}
                //else if (parser_type == "W3CebnfParser.g4")
                //{
                //    var imp = new LanguageServer.ConvertW3Cebnf();
                //    res = imp.Try(doc.FullPath, doc.Code, out_type);
                //}
                //else if (parser_type == "lark")
                //{
                //    var imp = new LanguageServer.ConvertLark();
                //    res = imp.Try(doc.FullPath, doc.Code, out_type);
                //}
                //else
                {
                    System.Console.WriteLine("Unknown type for conversion.");
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
