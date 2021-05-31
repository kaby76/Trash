namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    class CTokens
    {
        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trtokens
Print out the tokens for the parse tree.

Example:
    trparse A.g4 | trtokens
";
        }

        private string Reconstruct(IParseTree tree)
        {
            Stack<IParseTree> stack = new Stack<IParseTree>();
            stack.Push(tree);
            StringBuilder sb = new StringBuilder();
            while (stack.Any())
            {
                var n = stack.Pop();
                if (n is TerminalNodeImpl term)
                {
                    sb.Append(term.Symbol.ToString());
                }
                else
                    for (int i = n.ChildCount - 1; i >= 0; i--)
                    {
                        stack.Push(n.GetChild(i));
                    }
            }
            return sb.ToString();
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
                foreach (var node in nodes)
                {
                    System.Console.WriteLine(Reconstruct(node));
                }
            }
        }
    }
}
