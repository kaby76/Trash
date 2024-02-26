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
	/// The value of a numeric literal containing "." but no e or E character is an
	/// atomic value of type xs:decimal
	/// 
	/// </summary>
	public class DecimalLiteral : NumericLiteral
	{
		private XSDecimal _value;

		/// <summary>
		/// Constructor for DecimalLiteral
		/// </summary>
		/// <param name="value">
		///            double value </param>
		public DecimalLiteral(decimal value)
		{
			_value = new XSDecimal(value);
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		/// <returns> xs:decimal value </returns>
		public virtual XSDecimal value()
		{
			return _value;
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