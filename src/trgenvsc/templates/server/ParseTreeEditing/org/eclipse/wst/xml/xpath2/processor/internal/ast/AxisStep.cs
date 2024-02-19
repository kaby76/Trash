using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	/// Class for AxisStep, this generates a sequence of zero or more nodes. These
	/// nodes are always returned in Document Order. This can be Forward Step or
	/// Reverse Step.
	/// </summary>
	public class AxisStep : StepExpr, IEnumerable
	{
		private Step _step;
		private ICollection<ICollection<Expr>> _exprs;

		/// <summary>
		/// Constructor for AxisStep.
		/// </summary>
		/// <param name="step">
		///            Defines forward/reverse step. </param>
		/// <param name="exprs">
		///            Collection of xpath expressions. </param>
		public AxisStep(Step step, ICollection<ICollection<Expr>> exprs)
		{
			_step = step;
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
		/// Advances to next step.
		/// </summary>
		/// <returns> Previous step. </returns>
		public virtual Step step()
		{
			return _step;
		}

		/// <summary>
		/// Set the step direction.
		/// </summary>
		public virtual void set_step(Step s)
		{
			_step = s;
		}

		/// <summary>
		/// Interator.
		/// </summary>
		/// <returns> Iterated expressions. </returns>
		public virtual IEnumerator<ICollection<Expr>> iterator()
		{
			return _exprs.GetEnumerator();
		}

		/// <summary>
		/// Determines size of expressions.
		/// </summary>
		/// <returns> Size of expressions. </returns>
		public virtual int predicate_count()
		{
			return _exprs.Count;
		}

        public IEnumerator GetEnumerator()
        {
            return _exprs.GetEnumerator();
        }

        public override ICollection<XPathNode> GetAllChildren()
        {
            var list = new List<XPathNode>();
            list.Add(_step);
            foreach (var col in _exprs)
            {
                foreach (var e in col)
                    list.Add((XPathNode)e);
            }
            return list;
        }

        public override string QuickInfo()
        {
            return "";
        }
    }

}