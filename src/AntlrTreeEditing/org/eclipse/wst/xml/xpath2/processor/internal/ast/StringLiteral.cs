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
	/// The value of a string literal is an atomic value whose type is xs:string and
	/// whose value is the string denoted by the characters between the delimiting
	/// apostrophes or quotation marks. If the literal is delimited by apostrophes,
	/// two adjacent apostrophes within the literal are interpreted as a single
	/// apostrophe. Similarly, if the literal is delimited by quotation marks, two
	/// adjacent quotation marks within the literal are interpreted as one quotation
	/// mark
	/// 
	/// </summary>
	public class StringLiteral : Literal
	{
		private XSString _value;

		/// <summary>
		/// Constructor for StringLiteral
		/// </summary>
		/// <param name="value">
		///            string value </param>
		public StringLiteral(string value)
		{
			_value = new XSString(value);
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		/// <returns> string value </returns>
		public virtual string @string()
		{
			return _value.value();
		}

		/// <returns> xs:string value </returns>
		public virtual XSString value()
		{
			return _value;
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            return new List<XPathNode>();
        }

        public override string QuickInfo()
        {
            return @string();
        }
    }

}