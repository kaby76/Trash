namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CConvert
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
                var imp = new LanguageServer.ConvertAntlr3();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "ANTLRv2Parser.g4")
            {
                var imp = new LanguageServer.ConvertAntlr2();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "BisonParser.g4")
            {
                var imp = new LanguageServer.ConvertBison();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "W3CebnfParser.g4")
            {
                var imp = new LanguageServer.ConvertW3Cebnf();
                res = imp.Try(doc.FullPath, doc.Code);
            }
            else if (type == "ANTLRv4Parser.g4")
            {
                System.Console.WriteLine("Cannot convert an Antlr4 file to Antlr4.");
            }
            else
            {
                System.Console.WriteLine("Unknown type for conversion.");
            }
            
            Docs.Class1.EnactEdits(res);
            var new_fn = res.Where(d => d.Key.EndsWith(".g4")).First().Key;
            var new_code = res.Where(d => d.Key.EndsWith(".g4")).First().Value;
            var converted_doc = Docs.Class1.CreateDoc(new_fn, new_code);
            var pr = ParsingResultsFactory.Create(converted_doc);
            IParseTree pt = pr.ParseTree;
            var tuple = new ParsingResultSet()
            {
                Text = converted_doc.Code,
                FileName = converted_doc.FullPath,
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
