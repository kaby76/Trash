using System.Collections;
using System.Collections.Generic;
using System.Text;

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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	/// <summary>
	/// Class for parethesized expressions support.
	/// </summary>
	public class ParExpr : PrimaryExpr, IEnumerable
	{
		private ICollection<Expr> _exprs;

		/// <summary>
		/// Constructor for ParExpr.
		/// </summary>
		/// <param name="exprs">
		///            Expressions. </param>
		public ParExpr(ICollection<Expr> exprs)
		{
			_exprs = exprs;
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		/// <summary>
		/// Support for Iterator interface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual IEnumerator<Expr> iterator()
		{
			return _exprs.GetEnumerator();
		}

        public IEnumerator GetEnumerator()
        {
            return _exprs.GetEnumerator();
        }

		public override ICollection<XPathNode> GetAllChildren()
        {
			var list = new List<XPathNode>();
            foreach (var x in _exprs)
            {
				list.Add(x);
            }
            return list;
        }

        public override string QuickInfo()
        {
            return "";
        }
    }

}