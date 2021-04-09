namespace Trash
{
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using LanguageServer;
    using System.Text.Json;

    class CStrip
    {
        public void Help()
        {
            System.Console.WriteLine(@"
This program is part of the Trash toolkit.

trstrip
Replaces the grammar at the top of stack with one that has all comments, labels, and
action blocks removed. The resulting grammar is a basic CFG. Once completed, you can write
the grammar out using 'write'.

Example:
    trparse A.g4 | trstrip
");
        }

        public void Execute(Config config)
        {
            string lines = null;
            for (; ; )
            {
                lines = System.Console.In.ReadToEnd();
                if (lines != null && lines != "") break;
            }
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            AntlrJson.ParsingResultSet parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var doc = Docs.Class1.CreateDoc(parse_info);
            var results = LanguageServer.Transform.Strip(doc);
            Docs.Class1.EnactEdits(results);

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
