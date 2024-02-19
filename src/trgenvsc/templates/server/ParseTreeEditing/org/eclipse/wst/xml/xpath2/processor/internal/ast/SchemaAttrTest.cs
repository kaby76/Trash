using System;
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

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Support for Schema Attribute test.
	/// </summary>
	public class SchemaAttrTest : KindTest
	{
		private QName _arg;

		/// <summary>
		/// Constructor for SchemaAttrTest.
		/// </summary>
		/// <param name="arg">
		///            QName argument. </param>
		public SchemaAttrTest(QName arg)
		{
			_arg = arg;
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
		public virtual QName arg()
		{
			return _arg;
		}

		public override AnyType createTestType(ResultSequence rs, StaticContext sc)
		{
			// TODO Auto-generated method stub
			return null;
		}

		public override QName name()
		{
			return _arg;
		}

		public override bool Wild
		{
			get
			{
				return false;
			}
		}

		public override Type XDMClassType
		{
			get
			{
				// TODO Auto-generated method stub
				return null;
			}
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