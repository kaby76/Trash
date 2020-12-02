namespace Trash.Commands
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Microsoft.Msagl.Drawing;
    using System.Collections.Generic;
    using System.Linq;

    class CAgl
    {
        public void Help()
        {
            System.Console.WriteLine(@"agl
Read a tree from stdin and open a Windows Form that displays the graph.

Example:
    . | agl
");
        }

        public Graph CreateGraph(IParseTree[] trees, IList<string> parserRules)
        {
            var graph = new Graph();
            foreach (var tree in trees)
            {
                if (tree != null)
                {
                    if (tree.ChildCount == 0)
                        graph.AddNode(tree.GetHashCode().ToString());
                    else
                        GraphEdges(graph, tree, tree.GetHashCode());
                    FormatNodes(graph, tree, parserRules, tree.GetHashCode());
                }
            }
            return graph;
        }

        private void GraphEdges(Graph graph, ITree tree, int base_hash_code)
        {
            for (var i = tree.ChildCount - 1; i > -1; i--)
            {
                var child = tree.GetChild(i);
                graph.AddEdge((base_hash_code + tree.GetHashCode()).ToString(),
                    (base_hash_code + child.GetHashCode()).ToString());

                GraphEdges(graph, child, base_hash_code);
            }
        }

        private void FormatNodes(Graph graph, ITree tree, IList<string> parserRules, int base_hash_code)
        {
            var node = graph.FindNode((base_hash_code + tree.GetHashCode()).ToString());
            if (node != null)
            {
                node.LabelText = Trees.GetNodeText(tree, parserRules);

                var ruleFailedAndMatchedNothing = false;

                if (tree is ParserRuleContext context)
                    ruleFailedAndMatchedNothing =
                       // ReSharper disable once ComplexConditionExpression
                       context.exception != null &&
                       context.Stop != null
                       && context.Stop.TokenIndex < context.Start.TokenIndex;

                if (tree is IErrorNode || ruleFailedAndMatchedNothing)
                    node.Label.FontColor = Color.Red;
                else
                    node.Label.FontColor = Color.Black;

                node.Attr.Color = Color.Black;

                //if (BackgroundColor.HasValue)
                //    node.Attr.FillColor = BackgroundColor.Value;

                node.Attr.Color = Color.Black;

                node.UserData = tree;
            }

            for (int i = 0; i < tree.ChildCount; i++)
                FormatNodes(graph, tree.GetChild(i), parserRules, base_hash_code);
        }

        public void Execute(Repl repl, ReplParser.AglContext tree, bool piped)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            var pair = repl.input_output_stack.Pop();
            var nodes = pair.Item1;
            var parser = pair.Item2;
            var doc = pair.Item3;
            var lines = pair.Item4;
            Microsoft.Msagl.Drawing.Graph graph = CreateGraph(nodes, parser.RuleNames.ToList());
            graph.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();
            viewer.Graph = graph;
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            form.ShowDialog();
        }
    }
}