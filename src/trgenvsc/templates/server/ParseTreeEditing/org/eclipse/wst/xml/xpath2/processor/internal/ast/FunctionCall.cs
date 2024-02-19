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

	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Class for Function Call support.
	/// </summary>
	public class FunctionCall : PrimaryExpr, IEnumerable
	{
		private QName _name;
		private ICollection<Expr> _args;
		private Function _function;

		/// <summary>
		/// Constructor for FunctionCall.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="args">
		///            Collection of arguments. </param>
		public FunctionCall(QName name, ICollection<Expr> args)
		{
			_name = name;
			_args = args;
		}

		public virtual Function function()
		{
			return _function;
		}

		public virtual void set_function(Function _function)
		{
			this._function = _function;
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
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		public virtual QName name()
		{
			return _name;
		}

		/// <summary>
		/// Support for Iterator interface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual IEnumerator<Expr> iterator()
		{
			return _args.GetEnumerator();
		}

		/// <summary>
		/// Support for Arity interface.
		/// </summary>
		/// <returns> Result of Arity operation. </returns>
		public virtual int arity()
		{
			return _args.Count;
		}

        public IEnumerator GetEnumerator()
        {
            return _args.GetEnumerator();
        }

        public override ICollection<XPathNode> GetAllChildren()
        {
            var result = new List<XPathNode>();
            foreach (var c in _args) result.Add(c);
            return result;
        }

        public override string QuickInfo()
        {
            return _name.ToString();
        }
    }

}