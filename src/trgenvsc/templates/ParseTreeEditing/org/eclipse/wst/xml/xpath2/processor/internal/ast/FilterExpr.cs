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
	/// A filter expression consists simply of a primary expression followed by zero
	/// or more predicates. The result of the filter expression consists of all the
	/// items returned by the primary expression for which all the predicates are
	/// true. If no predicates are specified, the result is simply the result of the
	/// primary expression. This result may contain nodes, atomic values, or any
	/// combination of these. The ordering of the items returned by a filter
	/// expression is the same as their order in the result of the primary
	/// expression. Context positions are assigned to items based on their ordinal
	/// position in the result sequence. The first context position is 1.
	/// </summary>
	public class FilterExpr : StepExpr, IEnumerable
	{
		private PrimaryExpr _pexpr;
		private ICollection<Expr> _exprs;

		/// <summary>
		/// Constructor of FilterExpr.
		/// </summary>
		/// <param name="pexpr">
		///            is copied to _pexpr. </param>
		/// <param name="exprs">
		///            is copied to _exprs.  </param>
		public FilterExpr(PrimaryExpr pexpr, ICollection<Expr> exprs)
		{
			_pexpr = pexpr;
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
		/// Get the primary expression.
		/// </summary>
		/// <returns> The primary expression. </returns>
		public virtual PrimaryExpr primary()
		{
			return _pexpr;
		}

		/// <summary>
		/// Get the next predicate.
		/// </summary>
		/// <returns> The next predicate. </returns>
		public virtual IEnumerator<Expr> iterator()
		{
			return _exprs.GetEnumerator();
		}

		/// <summary>
		/// Set a new primary expression.
		/// </summary>
		/// <param name="e">
		///            is set as the new primary expression. </param>
		public virtual void set_primary(PrimaryExpr e)
		{
			_pexpr = e;
		}

		/// <summary>
		/// Count the number of predicates.
		/// </summary>
		/// <returns> The size of the collection of predicates. </returns>
		public virtual int predicate_count()
		{
			return _exprs.Count;
		}

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
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