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
	/// Support for IF expressions.
	/// </summary>
	public class IfExpr : Expr
	{
		private ICollection<Expr> _exprs;
		private Expr _then;
		private Expr _else;

		/// <summary>
		/// Constructor for IfExpr.
		/// </summary>
		/// <param name="exps">
		///            Condition expressions. </param>
		/// <param name="t">
		///            If true expressions. </param>
		/// <param name="e">
		///            If false/else expressions. </param>
		public IfExpr(ICollection<Expr> exps, Expr t, Expr e)
		{
			_exprs = exps;
			_then = t;
			_else = e;
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

		/// <summary>
		/// Support for Expression interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr then_clause()
		{
			return _then;
		}

		/// <summary>
		/// Support for Expression interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr else_clause()
		{
			return _else;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            throw new System.NotImplementedException();
        }

        public override string QuickInfo()
        {
            throw new System.NotImplementedException();
        }
    }

}