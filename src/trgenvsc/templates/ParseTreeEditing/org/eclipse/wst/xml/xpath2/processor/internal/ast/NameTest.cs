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
using System.Diagnostics;
using System.Text;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Class for Name test operation.
	/// </summary>
	public class NameTest : NodeTest
	{
		private QName _name;

        private NameTest()
        {
        }

		/// <summary>
		/// Constructor for NameTest.
		/// </summary>
		/// <param name="name">
		///            QName to test. </param>
		public NameTest(QName name)
		{
			Debug.Assert(name != null);
			_name = name;
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
		/// <returns> Resulf of QName operation. </returns>
		public virtual QName name()
		{
			return _name;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            return new List<XPathNode>();
        }

        public override string QuickInfo()
        {
            return _name.StringValue;
        }
	}

}