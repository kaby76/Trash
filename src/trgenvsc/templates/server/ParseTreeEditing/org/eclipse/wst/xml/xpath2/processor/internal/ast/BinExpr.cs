/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2009 Andrea Bittau, University College London, and others
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

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	/// <summary>
	/// Abstract class for a Binary operation.
	/// </summary>
	public abstract class BinExpr : Expr
	{
		// XXX: review hierarchy - strictly should be Expr
		// or rename to binop
		private XPathNode _left;
		private XPathNode _right;

		/// <summary>
		/// Constructor for BinExpr.
		/// </summary>
		/// <param name="l">
		///            left xpath node for the operation. </param>
		/// <param name="r">
		///            right xpath node for the operation. </param>
		public BinExpr(XPathNode l, XPathNode r)
		{
			_left = l;
			_right = r;
		}

		/// <summary>
		/// Left xpath node.
		/// </summary>
		/// <returns> Left node. </returns>
		public virtual XPathNode left()
		{
			return _left;
		}

		/// <summary>
		/// Right xpath node.
		/// </summary>
		/// <returns> Right node. </returns>
		public virtual XPathNode right()
		{
			return _right;
		}

		/// <summary>
		/// Set the left xpath node.
		/// </summary>
		/// <param name="n">
		///            Left node. </param>
		public virtual void set_left(XPathNode n)
		{
			_left = n;
		}

		/// <summary>
		/// Set the right xpath node.
		/// </summary>
		/// <param name="n">
		///            Right node. </param>
		public virtual void set_right(XPathNode n)
		{
			_right = n;
		}
	}

}