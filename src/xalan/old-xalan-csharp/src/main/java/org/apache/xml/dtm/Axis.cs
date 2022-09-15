/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: Axis.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{

	/// <summary>
	/// Specifies values related to XPath Axes.
	/// <para>The ancestor, descendant, following, preceding and self axes partition a
	/// document (ignoring attribute and namespace nodes): they do not overlap
	/// and together they contain all the nodes in the document.</para>
	/// 
	/// </summary>
	public sealed class Axis
	{

	  /// <summary>
	  /// The ancestor axis contains the ancestors of the context node;
	  ///  the ancestors of the context node consist of the parent of context
	  ///  node and the parent's parent and so on; thus, the ancestor axis will
	  ///  always include the root node, unless the context node is the root node.
	  /// </summary>
	  public const int ANCESTOR = 0;

	  /// <summary>
	  /// the ancestor-or-self axis contains the context node and the ancestors of
	  ///  the context node; thus, the ancestor axis will always include the
	  ///  root node.
	  /// </summary>
	  public const int ANCESTORORSELF = 1;

	  /// <summary>
	  /// the attribute axis contains the attributes of the context node; the axis
	  ///  will be empty unless the context node is an element.
	  /// </summary>
	  public const int ATTRIBUTE = 2;

	  /// <summary>
	  /// The child axis contains the children of the context node. </summary>
	  public const int CHILD = 3;

	  /// <summary>
	  /// The descendant axis contains the descendants of the context node;
	  ///  a descendant is a child or a child of a child and so on; thus the
	  ///  descendant axis never contains attribute or namespace nodes.
	  /// </summary>
	  public const int DESCENDANT = 4;

	  /// <summary>
	  /// The descendant-or-self axis contains the context node and the
	  ///  descendants of the context node.
	  /// </summary>
	  public const int DESCENDANTORSELF = 5;

	  /// <summary>
	  /// the following axis contains all nodes in the same document as the
	  ///  context node that are after the context node in document order, excluding
	  ///  any descendants and excluding attribute nodes and namespace nodes.
	  /// </summary>
	  public const int FOLLOWING = 6;

	  /// <summary>
	  /// The following-sibling axis contains all the following siblings of the
	  ///  context node; if the context node is an attribute node or namespace node,
	  ///  the following-sibling axis is empty.
	  /// </summary>
	  public const int FOLLOWINGSIBLING = 7;

	  /// <summary>
	  /// The namespace axis contains the namespace nodes of the context node; the
	  ///  axis will be empty unless the context node is an element.
	  /// </summary>
	  public const int NAMESPACEDECLS = 8;

	  /// <summary>
	  /// The namespace axis contains the namespace nodes of the context node; the
	  ///  axis will be empty unless the context node is an element.
	  /// </summary>
	  public const int NAMESPACE = 9;

	  /// <summary>
	  /// The parent axis contains the parent of the context node,
	  ///  if there is one.
	  /// </summary>
	  public const int PARENT = 10;

	  /// <summary>
	  /// The preceding axis contains all nodes in the same document as the context
	  ///  node that are before the context node in document order, excluding any
	  ///  ancestors and excluding attribute nodes and namespace nodes
	  /// </summary>
	  public const int PRECEDING = 11;

	  /// <summary>
	  /// The preceding-sibling axis contains all the preceding siblings of the
	  ///  context node; if the context node is an attribute node or namespace node,
	  ///  the preceding-sibling axis is empty.
	  /// </summary>
	  public const int PRECEDINGSIBLING = 12;

	  /// <summary>
	  /// The self axis contains just the context node itself. </summary>
	  public const int SELF = 13;

	  /// <summary>
	  /// A non-xpath axis, traversing the subtree including the subtree
	  ///  root, descendants, attributes, and namespace node decls.
	  /// </summary>
	  public const int ALLFROMNODE = 14;

	  /// <summary>
	  /// A non-xpath axis, traversing the the preceding and the ancestor nodes, 
	  /// needed for inverseing select patterns to match patterns.
	  /// </summary>
	  public const int PRECEDINGANDANCESTOR = 15;

	  // ===========================================
	  // All axis past this are absolute.

	  /// <summary>
	  /// A non-xpath axis, returns all nodes in the tree from and including the 
	  /// root.
	  /// </summary>
	  public const int ALL = 16;

	  /// <summary>
	  /// A non-xpath axis, returns all nodes that aren't namespaces or attributes, 
	  /// from and including the root.
	  /// </summary>
	  public const int DESCENDANTSFROMROOT = 17;

	  /// <summary>
	  /// A non-xpath axis, returns all nodes that aren't namespaces or attributes, 
	  /// from and including the root.
	  /// </summary>
	  public const int DESCENDANTSORSELFFROMROOT = 18;

	  /// <summary>
	  /// A non-xpath axis, returns root only.
	  /// </summary>
	  public const int ROOT = 19;

	  /// <summary>
	  /// A non-xpath axis, for functions.
	  /// </summary>
	  public const int FILTEREDLIST = 20;

	  /// <summary>
	  /// A table to identify whether an axis is a reverse axis;
	  /// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private static readonly bool[] isReverse_Renamed = new bool[] {true, true, false, false, false, false, false, false, false, false, false, true, true, false};

		/// <summary>
		/// The names of the axes for diagnostic purposes. </summary>
		private static readonly string[] names = new string[] {"ancestor", "ancestor-or-self", "attribute", "child", "descendant", "descendant-or-self", "following", "following-sibling", "namespace-decls", "namespace", "parent", "preceding", "preceding-sibling", "self", "all-from-node", "preceding-and-ancestor", "all", "descendants-from-root", "descendants-or-self-from-root", "root", "filtered-list"};

	  public static bool isReverse(int axis)
	  {
		  return isReverse_Renamed[axis];
	  }

		public static string getNames(int index)
		{
			return names[index];
		}

		public static int NamesLength
		{
			get
			{
				return names.Length;
			}
		}

	}

}