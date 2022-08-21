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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// The ancestor-or-self axis contains the context node and the ancestors of the
	/// context node, this always includes the root node.
	/// </summary>
	// multiple inheretance might be cool here =D
	public class AncestorOrSelfAxis : ReverseAxis
	{

		/// <summary>
		/// Get ancestor nodes of the context node and the context node itself.
		/// </summary>
		/// <param name="node">
		///            is the type of node. </param>
		public override void iterate(NodeType node, ResultBuffer copyInto, Node limitNode)
		{
			// get ancestors
			AncestorAxis aa = new AncestorAxis();
			aa.iterate(node, copyInto, null);

			// add self
			copyInto.add(node);
		}

		public override string name()
		{
			return "ancestor-or-self";
		}
	}

}