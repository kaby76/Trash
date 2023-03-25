/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using AttrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AttrType;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using Attr = org.w3c.dom.Attr;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// The attribute axis contains the attributes of the context node. The axis will
	/// be empty unless the context node is an element.
	/// </summary>
	public class AttributeAxis : ForwardAxis
	{

		/// <summary>
		/// Retrieves the context node's attributes.
		/// </summary>
		/// <param name="node">
		///            is the type of node. </param>
		public override void iterate(NodeType node, ResultBuffer copyInto, Node limitNode)
		{
			// only elements have attributes
			if (!(node is ElementType))
			{
				return;
			}

			// get attributes
			ElementType elem = (ElementType) node;
			NamedNodeMap attrs = elem.value().Attributes;

            if (attrs == null) return;

			// add attributes
			for (int i = 0; i < attrs.Length; i++)
			{
				Attr attr = (Attr) attrs.item(i);

				copyInto.add(NodeType.dom_to_xpath(attr, node.TypeModel));
			}
		}

		/// <summary>
		/// Retrieves the node's principle node kind.
		/// </summary>
		/// <returns> The type of node. </returns>
		public override NodeType principal_node_kind()
		{
			return new AttrType();
		}

		public override string name()
		{
			return "attribute";
		}
	}

}