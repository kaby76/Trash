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
///     David Carver - bug 298535 - Attribute instance of improvements 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Class for Document testing.
	/// </summary>
	public class DocumentTest : KindTest
	{
		/// <summary>
		/// Set internal value for NONE.
		/// </summary>
		public const int NONE = 0;
		/// <summary>
		/// Set internal value for ELEMENT.
		/// </summary>
		public const int ELEMENT = 1;
		/// <summary>
		/// Set internal value for SCHEMA_ELEMENT.
		/// </summary>
		public const int SCHEMA_ELEMENT = 2;

		// XXX: polymorphism
		private int _type;

		private AttrElemTest _etest;
		private SchemaElemTest _schema_etest;

		/// <summary>
		/// Constructor for DocumentTest.
		/// </summary>
		/// <param name="type">
		///            Type of element to test. </param>
		/// <param name="arg">
		///            xpath object to test. </param>
		public DocumentTest(int type, object arg)
		{
			_etest = null;
			_schema_etest = null;

			_type = type;
			switch (_type)
			{
			case ELEMENT:
				_etest = (AttrElemTest) arg;
				break;
			case SCHEMA_ELEMENT:
				_schema_etest = (SchemaElemTest) arg;
				break;
			}
		}

		/// <summary>
		/// Default Constructor for DocumentTest.
		/// </summary>
		public DocumentTest() : this(NONE, null)
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
		/// Get test type.
		/// </summary>
		/// <returns> Type of test. </returns>
		public virtual int type()
		{
			return _type;
		}

		/// <summary>
		/// Element test.
		/// </summary>
		/// <returns> Element test. </returns>
		public virtual AttrElemTest elem_test()
		{
			return _etest;
		}

		/// <summary>
		/// Schema element test.
		/// </summary>
		/// <returns> Schema element test. </returns>
		public virtual SchemaElemTest schema_elem_test()
		{
			return _schema_etest;
		}

		public override AnyType createTestType(ResultSequence rs, StaticContext sc)
		{
			// TODO Auto-generated method stub
			return null;
		}

		public override QName name()
		{
			// TODO Auto-generated method stub
			return null;
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
				return typeof(DocType);
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