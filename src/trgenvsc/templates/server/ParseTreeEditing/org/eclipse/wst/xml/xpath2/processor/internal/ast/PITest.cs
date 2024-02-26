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
	using PIType = org.eclipse.wst.xml.xpath2.processor.@internal.types.PIType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Class for Processing Instruction support.
	/// </summary>
	public class PITest : KindTest
	{
		private string _arg;

		/// <summary>
		/// Constructor for PITest.
		/// </summary>
		/// <param name="arg">
		///            instruction argument. </param>
		public PITest(string arg)
		{
			_arg = arg;
		}

		/// <summary>
		/// Default Constructor for PITest.
		/// </summary>
		public PITest() : this(null)
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
		/// Support for String arguments.
		/// </summary>
		/// <returns> Result of String operation. </returns>
		public virtual string arg()
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
				return typeof(PIType);
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