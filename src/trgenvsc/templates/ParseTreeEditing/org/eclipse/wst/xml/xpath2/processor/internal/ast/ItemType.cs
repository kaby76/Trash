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
	/// Support for Item node type.
	/// </summary>
	public class ItemType : XPathNode
	{
		/// <summary>
		/// Set internal value for ITEM.
		/// </summary>
		public const int ITEM = 0;
		/// <summary>
		/// Set internal value for QNAME.
		/// </summary>
		public const int QNAME = 1;
		/// <summary>
		/// Set internal value for KINDTEST.
		/// </summary>
		public const int KINDTEST = 2;
		private int _type;

		private QName _qname;
		private KindTest _ktest;

        public const int MAPTEST = 3;
		public const int FUNCTIONTEST = 4;
		public const int ARRAYTEST = 5;
		public const int ATOMICORUNIONTEST = 6;
		public const int PARENTHESIZEDITEMTYPE = 7;

		// XXX: polymorphism
		/// <summary>
		/// Constructor for ItemType.
		/// </summary>
		/// <param name="type">
		///            Type. </param>
		/// <param name="value">
		///            Object value. </param>
		public ItemType(int type, object value)
		{
			_qname = null;
			_ktest = null;

			_type = type;

			switch (type)
			{
			case QNAME:
				_qname = (QName) value;
				break;
			case KINDTEST:
				_ktest = (KindTest) value;
				break;
			}
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
		/// Support for Type interface.
		/// </summary>
		/// <returns> Result of Type operation. </returns>
		public virtual int type()
		{
			return _type;
		}

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		public virtual QName qname()
		{
			return _qname;
		}

		/// <summary>
		/// Support KindTest interface.
		/// </summary>
		/// <returns> Result of KindTest operation. </returns>
		public virtual KindTest kind_test()
		{
			return _ktest;
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