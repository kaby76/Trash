namespace Trash
{
    using Antlr4.Runtime.Tree;
    using LanguageServer;
    using System.Text;
    using System.Text.Json;

    class CTree
    {
        public string Help()
        {
            return @"
This program is part of the Trash toolkit.

trtree
Reads a tree from stdin and prints the tree as an indented node list.

Example:
    trparse A.g4 | trtree
";
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
            foreach (var in_tuple in data)
            {
                var nodes = in_tuple.Nodes;
                var lexer = in_tuple.Lexer;
                var parser = in_tuple.Parser;
                StringBuilder sb = new StringBuilder();
                foreach (var node in nodes)
                {
                    TerminalNodeImpl x = TreeEdits.LeftMostToken(node);
                    var ts = x.Payload.TokenSource;
                    sb.AppendLine();
                    sb.AppendLine(
                        TreeOutput.OutputTree(
                            node,
                            lexer,
                            parser,
                            null).ToString());
                }
                System.Console.WriteLine(sb.ToString());
            }
        }
    }
}
