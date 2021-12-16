namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    class Command
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("trstrip.readme.md"))
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
            serializeOptions.Converters.Add(new ParseTreeConverter());
            serializeOptions.WriteIndented = true;
            var data = JsonSerializer.Deserialize<ParsingResultSet[]>(lines, serializeOptions);
            var results = new List<ParsingResultSet>();
            foreach (var parse_info in data)
            {
                var doc = Docs.Class1.CreateDoc(parse_info);
                var res = Transform.Strip(doc);
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
                results.Add(tuple);
            }
            string js1 = JsonSerializer.Serialize(results.ToArray(), serializeOptions);
            System.Console.WriteLine(js1);
        }
    }
}
