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
            serializeOptions.WriteIndented = true;
            AntlrJson.ParsingResultSet[] data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            List<ParsingResultSet> results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var f = doc.FullPath;
                var type = parse_info.Parser.GrammarFileName;
                var out_type = config.Type;
                Dictionary<string, string> res = null;
                if (type == "ANTLRv3Parser.g4")
                {
                    var imp = new LanguageServer.ConvertAntlr3();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else if (type == "ANTLRv2Parser.g4")
                {
                    var imp = new LanguageServer.ConvertAntlr2();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else if (type == "BisonParser.g4")
                {
                    var imp = new LanguageServer.ConvertBison();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else if (type == "W3CebnfParser.g4")
                {
                    var imp = new LanguageServer.ConvertW3Cebnf();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else if (type == "LarkParser.g4")
                {
                    var imp = new LanguageServer.ConvertLark();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else if (type == "ANTLRv4Parser.g4")
                {
                    var imp = new LanguageServer.ConvertAntlr4();
                    res = imp.Try(doc.FullPath, doc.Code, out_type);
                }
                else
                {
                    System.Console.WriteLine("Unknown type for conversion.");
                }

                Docs.Class1.EnactEdits(res);
                foreach (var r in res)
                {
                    var new_fn = r.Key;
                    var new_code = r.Value;
                    var converted_doc = Docs.Class1.CreateDoc(new_fn, new_code);
                    var pr = ParsingResultsFactory.Create(converted_doc);
                    var node_arr = new IParseTree[0];
                    if (pr != null) node_arr = new IParseTree[] { pr.ParseTree };
                    var tuple = new ParsingResultSet()
                    {
                        Text = converted_doc.Code,
                        FileName = converted_doc.FullPath,
                        Stream = pr != null ? pr.TokStream : null,
                        Nodes = node_arr,
                        Lexer = pr != null ? pr.Lexer : null,
                        Parser = pr != null ? pr.Parser : null
                    };
                    results.Add(tuple);
                }
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
