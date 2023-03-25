/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2012 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     Lukasz Wycisk - bug 361803 - NodeType:dom_to_xpath and null value
/// ******************************************************************************
/// </summary>

using org.w3c.dom;

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using Attr = org.w3c.dom.Attr;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// the parent axis contains the sequence returned by the dm:parent accessor in,
	/// which returns the parent of the context node, or an empty sequence if the
	/// context node has no parent
	/// </summary>
	public class ParentAxis : ReverseAxis
	{

		/// <summary>
		/// returns parent accessors of the context node
		/// </summary>
		/// <param name="node">
		///            is the node type. </param>
		/// <exception cref="dc">
		///             is the Dynamic context. </exception>
		public override void iterate(NodeType node, ResultBuffer copyInto, Node limitNode)
		{
			Node n = node.node_value();

			if (limitNode != null && limitNode.isSameNode(n))
			{
				// no further, we have reached the limit node
				return;
			}

			Node parent = findParent(n);

			// if a parent exists... add it
			if (parent != null)
			{
				NodeType nodeType = NodeType.dom_to_xpath(parent, node.TypeModel);
				if (nodeType != null)
				{
					copyInto.add(nodeType);
				}
			}
		}

		public virtual Node findParent(Node n)
		{
			Node parent = n.ParentNode;

			// special case attribute elements...
			// in this case... the parent is the element which owns the attr
			if (n.NodeType == NodeConstants.ATTRIBUTE_NODE)
			{
				Attr att = (Attr) n;

				parent = att.OwnerElement;
			}
			return parent;
		}

		public override string name()
		{
			return "parent";
		}
	}

}