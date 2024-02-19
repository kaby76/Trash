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

using System.Collections.Generic;
using System.Text;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	/// <summary>
	/// Path expression walks tries to walk the path specified in its argument
	/// </summary>
	public class XPathExpr : Expr
	{
		private int _slashes;
        private bool _predicate;
		private StepExpr _expr;

		// single linked list
		private XPathExpr _next;

		/// <param name="slashes">
		///            is copied to _slashes </param>
		/// <param name="expr">
		///            is copied to _expr _next is made null as a result. </param>
		public XPathExpr(int slashes, bool is_predicate, StepExpr expr)
		{
			_slashes = slashes;
            _predicate = is_predicate;
			_expr = expr;
			_next = null;
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
		/// Add to tail of path
		/// </summary>
		// XXX: keep ref to last
		public virtual void add_tail(int slashes, StepExpr expr)
		{
			XPathExpr last = this;
			XPathExpr next = _next;

			while (next != null)
			{
				last = next;
				next = last.next();
			}

			XPathExpr item = new XPathExpr(slashes, false, expr);
			last.set_next(item);

		}

		/// <param name="count">
		///            is copied to _slashes </param>
		public virtual void set_slashes(int count)
		{
			_slashes = count;
		}

		/// <returns> XPath expression _next </returns>
		public virtual XPathExpr next()
		{
			return _next;
		}

		/// <summary>
		/// an XPath expression, n is copied to _next
		/// </summary>
		public virtual void set_next(XPathExpr n)
		{
			_next = n;
		}

		/// <returns> Step expression _expr </returns>
		public virtual StepExpr expr()
		{
			return _expr;
		}

		/// <returns> int _slashes </returns>
		public virtual int slashes()
		{
			return _slashes;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
			if (_next != null)
    			return new List<XPathNode>() {_expr, _next};
            else
				return new List<XPathNode>() { _expr };
		}

        public override string QuickInfo()
        {
            return _slashes.ToString() + " slashes";

        }
    }

}