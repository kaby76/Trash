namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System;
    using System.IO;
    using System.Text.Json;
    using Workspaces;

    class CXml
    {
        public Workspace _workspace { get; set; } = new Workspace();

        public Document ReadDoc(string path)
        {
            string file_name = path;
            Document document = _workspace.FindDocument(file_name);
            if (document == null)
            {
                throw new Exception("File does not exist.");
            }
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file_name))
                {
                    // Read the stream to a string, and write the string to the console.
                    string str = sr.ReadToEnd();
                    document.Code = str;
                }
            }
            catch (IOException)
            {
                throw;
            }
            return document;
        }

        public void Help()
        {
            System.Console.WriteLine(@"xml
Read a tree from stdin and write an XML represenation of it.

Example:
    . | xml
");
        }

        class XmlWalk : IParseTreeListener
        {
            int INDENT = 4;
            int level = 0;
            Parser parser;

            public XmlWalk(Parser p)
            {
                parser = p;
            }

            public void EnterEveryRule(ParserRuleContext ctx)
            {
                System.Console.WriteLine(
                    indent()
                    + "<" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
                ++level;
            }

            public void ExitEveryRule(ParserRuleContext ctx)
            {
                --level;
                System.Console.WriteLine(
                    indent()
                    + "</" + parser.RuleNames[ctx.RuleIndex]
                    + ">");
            }

            public void VisitErrorNode(IErrorNode node)
            {
                throw new NotImplementedException();
            }

            public void VisitTerminal(ITerminalNode node)
            {
                string value = node.GetText();
                {
                    System.Console.WriteLine(
                       indent()
                       + "<t>"
                       + node.GetText()
                       + "</t>");
                }
            }

            private String indent()
            {
                var result = new string(' ', level * INDENT);
                return result;
            }
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
            var parse_info = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet>(lines, serializeOptions);
            var nodes = parse_info.Nodes;
            var parser = parse_info.Parser;
            var lexer = parse_info.Lexer;
            var fn = parse_info.FileName;
            Document doc = null;
            if (!(fn == null || fn == "stdin"))
            {
                doc = _workspace.ReadDocument(fn);
            }
            foreach (var node in parse_info.Nodes)
            {
                ParseTreeWalker.Default.Walk(new XmlWalk(parser), node);
            }
        }
    }
}
