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

	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Support for Single types.
	/// </summary>
	public class SingleType : XPathNode
	{

		private QName _type;
		private bool _qmark;

		/// <summary>
		/// Constructor for SingleType.
		/// </summary>
		/// <param name="type">
		///            QName type. </param>
		/// <param name="qmark">
		///            optional type? (true/false). </param>
		public SingleType(QName type, bool qmark)
		{
			_type = type;
			_qmark = qmark;
		}

		/// <summary>
		/// Default Constructor for SingleType.
		/// </summary>
		public SingleType(QName type) : this(type, false)
		{
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
		/// Set optional type.
		/// </summary>
		/// <returns> optional type value. </returns>
		public virtual bool qmark()
		{
			return _qmark;
		}

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		public virtual QName type()
		{
			return _type;
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