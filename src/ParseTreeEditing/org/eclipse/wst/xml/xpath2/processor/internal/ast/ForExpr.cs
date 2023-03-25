using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

	/// <summary>
	/// Class for the For expression.
	/// </summary>
	public class ForExpr : Expr
	{
		private ICollection<VarExprPair> _var_expr_pairs;
		private Expr _return;

		/// <summary>
		/// Constructor for ForExpr.
		/// </summary>
		/// <param name="varexp">
		///            Expressions. </param>
		/// <param name="ret">
		///            Return expression. </param>
		public ForExpr(ICollection<VarExprPair> varexp, Expr ret)
		{
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
		/// Support for Iterator interface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual IEnumerator<VarExprPair> iterator()
		{
			return _var_expr_pairs.GetEnumerator();
		}

		/// <summary>
		/// Support for Expr interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr expr()
		{
			return _return;
		}

		/// <summary>
		/// Set Expression.
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

			for (IEnumerator i = _var_expr_pairs.GetEnumerator(); i.MoveNext();)
			{
			//	i.Current;
				if (!first)
				{
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
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
			var result = new List<XPathNode>();
			foreach (var p in this._var_expr_pairs)
            {
                Expr e = p.expr();
				result.Add(e);
            }
			return result;
        }

        public override string QuickInfo()
        {
			return "_for " + String.Join(" ", this._var_expr_pairs.Select(t=>t.ToString())) + " " + this._return.QuickInfo();
		}
    }

}