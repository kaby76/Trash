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
///     David Carver (STAR) - bug 262765 - Was not handling xml loaded dynamically in variables. 
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     Lukasz Wycisk - bug 361803 - NodeType:dom_to_xpath and null value
/// ******************************************************************************
/// </summary>

using System.Text;

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// The child axis contains the children of the context node.
	/// </summary>
	public class ChildAxis : ForwardAxis
	{

		/// <summary>
		/// Retrieves the context node's children.
		/// </summary>
		/// <param name="node">
		///            is the type of node. </param>
		public override void iterate(NodeType node, ResultBuffer copyInto, Node limitNode)
		{
			addChildren(node, copyInto, false);
		}

		protected internal virtual void addChildren(NodeType node, ResultBuffer copyInto, bool recurse)
		{
			NodeList nl = null;

			// only document and element nodes have children
			if (node is DocType)
			{
				nl = ((DocType) node).value().ChildNodes;
			}
			if (node is ElementType)
			{
				nl = ((ElementType) node).value().ChildNodes;
			}

			// add the children to the result
			if (nl != null)
			{
				for (int i = 0; i < nl.Length; i++)
				{
					Node dnode = nl.item(i);
					NodeType n = NodeType.dom_to_xpath(dnode, node.TypeModel);

					if (n != null)
					{
						copyInto.add(n);

						if (recurse)
						{
							addChildren(n, copyInto, recurse);
						}
					}
				}
			}
		}

		public override string name()
		{
			return "child";
		}
    }

}