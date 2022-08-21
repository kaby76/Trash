namespace ZhangShashaCSharp
{
	using System;
	using System.Collections.Generic;

	public class Node
	{
		public String label; // node label
		public int postorder_number; // preorder index
						  // note: trees need not be binary
		public List<Node> children = new List<Node>();
		public Node leftmost; // used by the recursive O(n) leftmost() function

		public Node()
		{

		}

		public Node(String label)
		{
			this.label = label;
		}
	}
}
