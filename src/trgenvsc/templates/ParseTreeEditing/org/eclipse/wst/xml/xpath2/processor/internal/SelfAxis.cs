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
	/// Create a result sequence that contains the context node
	/// </summary>
	public class SelfAxis : ForwardAxis
	{

		/// <summary>
		/// create new rs and add the context node to it
		/// </summary>
		/// <param name="node">
		///            is the node type </param>
		public override void iterate(NodeType node, ResultBuffer copyInto, Node limitNode)
		{
			copyInto.add(node);
		}

		public override string name()
		{
			return "self";
		}

	}

}