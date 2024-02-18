using System.Diagnostics;
using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver (STAR) - initial API and implementation
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Summary: Returns the value of the default collation property from the static
	/// context. Components of the static context are discussed in Section C.1 Static
	/// Context Components.
	/// </para>
	/// 
	/// <para>
	/// Note:
	/// </para>
	/// 
	/// <para>
	/// The default collation property can never be undefined. If it is not
	/// explicitly defined, a system defined default can be invoked. If this is not
	/// provided, the Unicode code point collation
	/// (http://www.w3.org/2005/xpath-functions/collation/codepoint) is used.
	/// </para>
	/// 
	/// @author dcarver
	/// @since 1.1
	/// </summary>
	public class FnDefaultCollation : Function
	{

		public FnDefaultCollation() : base(new QName("default-collation"), 0)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());
			return new XSString(ec.DynamicContext.CollationProvider.DefaultCollation);
		}

	}

}