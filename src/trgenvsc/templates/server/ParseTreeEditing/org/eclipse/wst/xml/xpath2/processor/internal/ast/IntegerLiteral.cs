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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using System.Collections.Generic;
using System.Text;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;

	/// <summary>
	/// The value of a numeric literal containing no "." anad no e or E character is
	/// an atomic value of type xs:integer
	/// 
	/// </summary>
	public class IntegerLiteral : NumericLiteral
	{
		private XSInteger _value;

		/// <summary>
		/// Constructor for IntegerLiteral
		/// </summary>
		/// <param name="i">
		///            integer value </param>
		public IntegerLiteral(System.Numerics.BigInteger i)
		{
			_value = new XSInteger(i);
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		/// <returns> xs:integer value </returns>
		public virtual XSInteger value()
		{
			return _value;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            return new List<XPathNode>();
        }

        public override string QuickInfo()
        {
            return value().ToString();
        }
    }

}