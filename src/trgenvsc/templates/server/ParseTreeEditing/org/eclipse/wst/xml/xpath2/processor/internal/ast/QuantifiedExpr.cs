using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
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

	// mite wanna do two separate classes at the end
	/// <summary>
	/// Support for Quantified expressions.
	/// </summary>
	public class QuantifiedExpr : Expr
	{
		/// <summary>
		/// Set internal value for SOME.
		/// </summary>
		public const int SOME = 0;
		/// <summary>
		/// Set internal value for ALL.
		/// </summary>
		public const int ALL = 1;

		private ICollection<VarExprPair> _var_expr_pairs;
		private Expr _return;
		private int _type;

		/// <summary>
		/// Constructor for QuantifiedExpr.
		/// </summary>
		/// <param name="type">
		///            Type (0 for SOME, 1 for ALL). </param>
		/// <param name="varexp">
		///            Expressions. </param>
		/// <param name="ret">
		///            Returned expression. </param>
		public QuantifiedExpr(int type, ICollection<VarExprPair> varexp, Expr ret)
		{
			_type = type;
			_var_expr_pairs = varexp;
			_return = ret;
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
		/// Support for Integer interface.
		/// </summary>
		/// <returns> Result of Int operation. </returns>
		public virtual int type()
		{
			return _type;
		}

		/// <summary>
		/// Support for Iterator inteface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual IEnumerator<VarExprPair> iterator()
		{
			return _var_expr_pairs.GetEnumerator();
		}

		/// <summary>
		/// Support for Expression interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr expr()
		{
			return _return;
		}

		/// <summary>
		/// Set next expression.
		/// </summary>
		/// <param name="e">
		///            Expression. </param>
		public virtual void set_expr(Expr e)
		{
			_return = e;
		}

		// used for normalization... basically just keep a "simple for"... no
		// pairs... collection will always have 1 element
		/// <summary>
		/// Normalization of expression pairs.
		/// </summary>
		public virtual void truncate_pairs()
		{
			bool first = true;
            ArrayList xx = new ArrayList();
			for (IEnumerator i = _var_expr_pairs.GetEnumerator(); i.MoveNext();)
			{
				if (!first)
                {
                    throw new Exception();
				//	i.remove();
				}

				first = false;
			}
		}

		/// <summary>
		/// Support for Collection interface.
		/// </summary>
		/// <returns> Expression pairs. </returns>
		public virtual ICollection<VarExprPair> ve_pairs()
		{
			return _var_expr_pairs;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            throw new System.NotImplementedException();
        }

        public override string QuickInfo()
        {
            throw new NotImplementedException();
        }
    }

}