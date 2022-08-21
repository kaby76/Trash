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
	/// This is the interface class for an Axis.
	/// 
	/// An axis defines the "direction of movement" for a step between a context node
	/// and another node that is reachable via the axis.
	/// </summary>
	public interface Axis
	{
		/// <summary>
		/// Get elements and attributes.
		/// </summary>
		/// <param name="node">
		///            is the type of node. </param>
		/// <param name="copyInto"> TODO </param>
		/// <param name="limitNode"> TODO </param>
		void iterate(NodeType node, ResultBuffer copyInto, Node limitNode);

		/// <summary>
		/// Get the principle kind of node.
		/// </summary>
		/// <returns> The principle node kind. </returns>
		NodeType principal_node_kind();

		/// <summary>
		/// Returns the title of the sequence as it appears in the XPath source
		/// </summary>
		/// <returns> Axis title </returns>
		string name();
	}

}