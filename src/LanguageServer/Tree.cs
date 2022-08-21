namespace ZhangShashaCSharp
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using System.Collections.Generic;
    using System.Linq;

    public class Tree
	{

		public Node root;
		public Dictionary<int, Node> all_nodes = new Dictionary<int, Node>();
		// function l() which gives the leftmost leaf for a given node (identified by post-order number).
		Dictionary<int, int> ll = new Dictionary<int, int>();
		// list of keyroots for a tree root, i.e., nodes with a left sibling, or in the case of the root, the root itself.
		List<int> keyroots = new List<int>();
		// list of the labels of the nodes used for node comparison
		Dictionary<int, string> labels = new Dictionary<int, string>();
		public Dictionary<Node, IParseTree> antlr_nodes = new Dictionary<Node, IParseTree>();

		// the following constructor handles s-expression notation. E.g., ( f a ( b c ) )
		public Tree(IParseTree t, Parser parser, Lexer lexer)
		{
			root = Convert(t, parser, lexer);
		}

		private Node Convert(IParseTree t, Parser parser, Lexer lexer)
        {
			Node n = new Node();
			antlr_nodes[n] = t;
			if (t is ParserRuleContext tt)
            {
				n.label = parser.RuleNames[tt.RuleIndex];
				if (tt.children != null)
				{
					var ch = tt.children.Select(c => Convert(c, parser, lexer)).ToList();
					n.children = ch;
				}
				else n.children = new List<Node>();
			}
            else if (t is TerminalNodeImpl ff)
            {
				n.label = lexer.Vocabulary.GetSymbolicName(ff.Symbol.Type);
			}
			return n;
		}

        public void Traverse()
		{
			// put together an ordered list of node labels of the tree
			Traverse(root, labels);
		}

		private static Dictionary<int, string> Traverse(Node node, Dictionary<int, string> labels)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				labels = Traverse(node.children[i], labels);
			}
			labels.Add(node.postorder_number, node.label);
			return labels;
		}

		public void ComputePostOrderNumber()
		{
			// index each node in the tree according to traversal method
			ComputePostOrderNumber(root, 0);
		}

		private int ComputePostOrderNumber(Node node, int index)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				index = ComputePostOrderNumber(node.children[i], index);
			}
			index++;
			node.postorder_number = index;
			all_nodes[index] = node;
			return index;
		}

		public void ComputeLeftMostLeaf()
		{
			// put together a function which gives l()
			Leftmost();
			var z = new Dictionary<int, int>();
			ll = ComputeLeftMostLeaf(root, z);
		}

		private Dictionary<int, int> ComputeLeftMostLeaf(Node node, Dictionary<int, int> l)
		{
			for (int i = 0; i < node.children.Count; i++)
			{
				l = ComputeLeftMostLeaf(node.children[i], l);
			}
			l.Add(node.postorder_number, node.leftmost.postorder_number);
			return l;
		}

		private void Leftmost()
		{
			Leftmost(root);
		}

		private static void Leftmost(Node node)
		{
			if (node == null)
				return;
			for (int i = 0; i < node.children.Count; i++)
			{
				Leftmost(node.children[i]);
			}
			if (node.children.Count == 0)
			{
				node.leftmost = node;
			}
			else
			{
				node.leftmost = node.children[0].leftmost;
			}
		}

		public void Keyroots()
		{
			// calculate the keyroots
			for (int i = 1; i <= ll.Count; i++)
			{
				int flag = 0;
				for (int j = i + 1; j <= ll.Count; j++)
				{
					if (ll[j] == ll[i])
					{
						flag = 1;
					}
				}
				if (flag == 0)
				{
					this.keyroots.Add(i);
				}
			}
		}

		static int[,] tree_distance;
		static List<Operation>[,] tree_operations;

		public static (int, List<Operation>) ZhangShasha(Tree tree1, Tree tree2)
		{
			tree1.ComputePostOrderNumber();
			tree1.ComputeLeftMostLeaf();
			tree1.Keyroots();
			tree1.Traverse();

			tree2.ComputePostOrderNumber();
			tree2.ComputeLeftMostLeaf();
			tree2.Keyroots();
			tree2.Traverse();

			Dictionary<int, int> l1 = tree1.ll;
			List<int> keyroots1 = tree1.keyroots;
			Dictionary<int, int> l2 = tree2.ll;
			List<int> keyroots2 = tree2.keyroots;

			// space complexity of the algorithm
			tree_distance = new int[l1.Count + 1, l2.Count + 1];
			tree_operations = new List<Operation>[l1.Count + 1, l2.Count + 1];
			for (int m = 0; m <= l1.Count; ++m)
				for (int n = 0; n <= l2.Count; ++n)
					tree_operations[m, n] = new List<Operation>();

			// solve subproblems
			for (int i1 = 1; i1 <= keyroots1.Count; i1++)
			{
				for (int j1 = 1; j1 <= keyroots2.Count; j1++)
				{
					int i = keyroots1[i1 - 1];
					int j = keyroots2[j1 - 1];
					Treedist(l1, l2, i, j, tree1, tree2);
				}
			}

			return (tree_distance[l1.Count, l2.Count], tree_operations[l1.Count, l2.Count]);
		}

		private static void Treedist(Dictionary<int, int> l1, Dictionary<int, int> l2, int i, int j, Tree tree1, Tree tree2)
		{
			int[,] forest_distance = new int[l1.Count + 1, l2.Count + 1];
			List<Operation>[,] forest_operations = new List<Operation>[l1.Count + 1, l2.Count + 1];
			for (int m = 0; m < l1.Count + 1; ++m)
				for (int n = 0; n < l2.Count + 1; ++n)
					forest_operations[m, n] = new List<Operation>();

			// costs of the three atomic operations
			int Delete = 1;
			int Insert = 1;
			int Relabel = 1;

			for (int i1 = l1[i]; i1 <= i; i1++)
			{
				forest_distance[i1, 0] = forest_distance[i1 - 1, 0] + Delete;
				forest_operations[i1, 0] = new List<Operation>(forest_operations[i1 - 1, 0]);
				forest_operations[i1, 0].Add(new Operation() { O = Operation.Op.Delete, N1 = i1});
			}
			for (int j1 = l2[j]; j1 <= j; j1++)
			{
				forest_distance[0, j1] = forest_distance[0, j1 - 1] + Insert;
				forest_operations[0, j1] = new List<Operation>(forest_operations[0, j1 - 1]);
				forest_operations[0, j1].Add(new Operation() { O = Operation.Op.Insert, N2 = j1});
			}
			for (int i1 = l1[i]; i1 <= i; i1++)
			{
				for (int j1 = l2[j]; j1 <= j; j1++)
				{
					if (l1[i1] == l1[i] && l2[j1] == l2[j])
					{
						var z = i1 - 1 < l1[i] ? 0 : i1 - 1;
						var z2 = j1 - 1 < l2[j] ? 0 : j1 - 1;
						var i_temp = forest_distance[z, j1] + Delete;
						var i_list = new List<Operation>(forest_operations[z, j1]);
						i_list.Add(new Operation() { O = Operation.Op.Delete, N1 = i1 });
						var i_op = i_list;

						var j_temp = forest_distance[i1, z2] + Insert;
						var j_list = new List<Operation>(forest_operations[i1, z2]);
						j_list.Add(new Operation() { O = Operation.Op.Insert, N2 = j1 });
						var j_op = j_list;

						var cost = tree1.labels[i1] == tree2.labels[j1] ? 0 : Relabel;
						var k_temp = forest_distance[z, z2] + cost;
						var k_list = new List<Operation>(forest_operations[z, z2]);
						if (cost != 0)
							k_list.Add(new Operation() { O = Operation.Op.Change, N1 = i1, N2 = j1 });
						var k_op = k_list;

						if (i_temp < j_temp)
						{
							if (i_temp < k_temp)
							{
								forest_distance[i1, j1] = i_temp;
								forest_operations[i1, j1] = i_op;
							}
							else
							{
								forest_distance[i1, j1] = k_temp;
								forest_operations[i1, j1] = k_op;
							}
						}
						else
						{
							if (j_temp < k_temp)
							{
								forest_distance[i1, j1] = j_temp;
								forest_operations[i1, j1] = j_op;
							}
							else
							{
								forest_distance[i1, j1] = k_temp;
								forest_operations[i1, j1] = k_op;
							}
						}

						tree_distance[i1, j1] = forest_distance[i1, j1];
						tree_operations[i1, j1] = forest_operations[i1, j1];
					}
					else
					{
						var z = i1 - 1 < l1[i] ? 0 : i1 - 1;
						var z2 = j1 - 1 < l2[j] ? 0 : j1 - 1;
						var i_temp = forest_distance[z, j1] + Delete;
						var i_list = new List<Operation>(forest_operations[z, j1]);
						i_list.Add(new Operation() { O = Operation.Op.Delete, N1 = i1 });
						var i_op = i_list;

						var j_temp = forest_distance[i1, z2] + Insert;
						var j_list = new List<Operation>(forest_operations[i1, z2]);
						j_list.Add(new Operation() { O = Operation.Op.Insert, N2 = j1 });
						var j_op = j_list;

						var k_temp = forest_distance[z, z2] + tree_distance[i1, j1];
						var k_list = new List<Operation>(forest_operations[z, z2]);
						k_list.AddRange(tree_operations[i1, j1]);
						var k_op = k_list;

						if (i_temp < j_temp)
						{
							if (i_temp < k_temp)
							{
								forest_distance[i1, j1] = i_temp;
								forest_operations[i1, j1] = i_op;
							}
							else
							{
								forest_distance[i1, j1] = k_temp;
								forest_operations[i1, j1] = k_op;
							}
						}
						else
						{
							if (j_temp < k_temp)
							{
								forest_distance[i1, j1] = j_temp;
								forest_operations[i1, j1] = j_op;
							}
							else
							{
								forest_distance[i1, j1] = k_temp;
								forest_operations[i1, j1] = k_op;
							}
						}
					}
				}
			}

			tree_distance[i, j] = forest_distance[i, j];
			tree_operations[i, j] = forest_operations[i, j];
		}
	}
}

