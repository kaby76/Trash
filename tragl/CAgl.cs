namespace Trash
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using AntlrJson;
    using Microsoft.Msagl.Drawing;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    class CAgl
    {
        public string Help()
        {
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("tragl.readme.md"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static Graph CreateGraph(IParseTree[] trees, IList<string> parserRules, IList<string> lexerRules)
        {
            var graph = new Graph();
            foreach (var tree in trees)
            {
                if (tree != null)
                {
                    var base_hash_code = tree.GetHashCode();
                    graph.AddNode((base_hash_code + tree.GetHashCode()).ToString());
                    if (tree.ChildCount != 0)
                        GraphEdges(graph, tree, tree.GetHashCode());
                    FormatNodes(graph, tree, parserRules, lexerRules, tree.GetHashCode());
                }
            }
            return graph;
        }

        private static void GraphEdges(Graph graph, ITree tree, int base_hash_code)
        {
            for (var i = tree.ChildCount - 1; i > -1; i--)
            {
                var child = tree.GetChild(i);
                graph.AddEdge((base_hash_code + tree.GetHashCode()).ToString(),
                    (base_hash_code + child.GetHashCode()).ToString());

                GraphEdges(graph, child, base_hash_code);
            }
        }

        private static void FormatNodes(Graph graph, ITree tree, IList<string> parserRules, IList<string> lexerRules, int base_hash_code)
        {
            var node = graph.FindNode((base_hash_code + tree.GetHashCode()).ToString());
            if (node != null)
            {
                node.LabelText = Trees.GetNodeText(tree, parserRules);
                var ruleFailedAndMatchedNothing = false;
                if (tree is ParserRuleContext context)
                {
                    ruleFailedAndMatchedNothing =
                       // ReSharper disable once ComplexConditionExpression
                       context.exception != null &&
                       context.Stop != null
                       && context.Stop.TokenIndex < context.Start.TokenIndex;
                }
                else if (tree is TerminalNodeImpl term)
                {
                    var token = term.Symbol.Type;
                    var token_value = term.Symbol.Text;
                    node.LabelText = token > 0 ?
                        (lexerRules[token-1] + "/" + token_value) : "EOF";
                }
                if (tree is IErrorNode || ruleFailedAndMatchedNothing)
                    node.Label.FontColor = Color.Red;
                else
                    node.Label.FontColor = Color.Black;
                node.Attr.Color = Color.Black;
                node.UserData = tree;
                //if (BackgroundColor.HasValue)
                //    node.Attr.FillColor = BackgroundColor.Value;
            }

            for (int i = 0; i < tree.ChildCount; i++)
                FormatNodes(graph, tree.GetChild(i), parserRules, lexerRules, base_hash_code);
        }

        public static void DoWork(object p)
        {
            var parse_info = (ParsingResultSet)p;
            var nodes = parse_info.Nodes;
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            Microsoft.Msagl.Drawing.Graph graph = CreateGraph(nodes, parse_info.Parser.RuleNames.ToList(), parse_info.Lexer.RuleNames.ToList());
            graph.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();
            viewer.Graph = graph;
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            form.ShowDialog();
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
            serializeOptions.Converters.Add(new AntlrJson.ParseTreeConverter());
            serializeOptions.WriteIndented = false;
            var data = JsonSerializer.Deserialize<AntlrJson.ParsingResultSet[]>(lines, serializeOptions);
            foreach (var parse_info in data)
            {
                // Thread thread1 = new Thread(DoWork);
                // thread1.Start(parse_info);
                DoWork(parse_info);
            }
        }
    }
}