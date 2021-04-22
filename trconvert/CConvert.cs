namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    class CConvert
    {
        public string Help()
        {
            return @"convert (antlr2 | antlr3 | antlr4 | bison | ebnf)?
Convert the parsed grammar file at the top of stack into Antlr4 syntax. If the type of
grammar cannot be inferred from the file suffix, a type can be supplied with the command.
The resulting Antlr4 grammar replaces the top of stack.

Example:
    (top of stack contains a grammar file that is Antlr2 or 3, Bison, or EBNF syntax.)
    combine antlr3
";
        }

        public void Execute(Config config)
        {
            string lines = null;
            if (!(config.File != null && config.File != ""))
            {
                for (; ; )
                {
                    lines = System.Console.In.ReadToEnd();
                    if (lines != null && lines != "") break;
                }
            }
            else
            {
                lines = File.ReadAllText(config.File);
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var doc = Docs.Class1.CreateDoc(parse_info);
            var f = doc.FullPath;
            var type = parse_info.Parser.GrammarFileName;
            Dictionary<string, string> res = null;
            if (type == "ANTLRv3Parser.g4")
            {
                var imp = new LanguageServer.Antlr3Import();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "ANTLRv2Parser.g4")
            {
                var imp = new LanguageServer.Antlr2Import();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "bison")
            {
                var imp = new LanguageServer.BisonImport();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "ebnf")
            {
                var imp = new LanguageServer.W3CebnfImport();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "antlr4")
            {
                System.Console.WriteLine("Cannot convert an Antlr4 file to Antlr4.");
            }
            else
            {
                System.Console.WriteLine("Unknown type for conversion.");
            }
            
            Docs.Class1.EnactEdits(res);

            var pr = ParsingResultsFactory.Create(doc);
            IParseTree pt = pr.ParseTree;
            var tuple = new ParsingResultSet()
            {
                Text = doc.Code,
                FileName = doc.FullPath,
                Stream = pr.TokStream,
                Nodes = new IParseTree[] { pt },
                Lexer = pr.Lexer,
                Parser = pr.Parser
            };
            string js1 = JsonSerializer.Serialize(tuple, serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
