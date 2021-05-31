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
        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trxml
Read a tree from stdin and write an XML represenation of it.

Example:
    trparse A.g4 | trxml
";
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
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                var nodes = parse_info.Nodes;
                var parser = parse_info.Parser;
                var lexer = parse_info.Lexer;
                var fn = parse_info.FileName;
                Document doc = Docs.Class1.CreateDoc(parse_info);
                foreach (var node in parse_info.Nodes)
                {
                    ParseTreeWalker.Default.Walk(new XmlWalk(parser), node);
                }
            }
        }
    }
}
