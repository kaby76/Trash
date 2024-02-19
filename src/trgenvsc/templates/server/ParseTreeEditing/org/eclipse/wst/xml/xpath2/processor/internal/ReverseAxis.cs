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

	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;

	/// <summary>
	/// An axis that only ever contains the context node or nodes that are before the
	/// context node in document order is a reverse axis.
	/// </summary>
	public abstract class ReverseAxis : Axis
	{
		public abstract string name();
		public abstract void iterate(types.NodeType node, org.eclipse.wst.xml.xpath2.api.ResultBuffer copyInto, org.w3c.dom.Node limitNode);

		/// <returns> new element type </returns>
		// should always be element i fink
		public virtual NodeType principal_node_kind()
		{
			return new ElementType();
		}
	}

}