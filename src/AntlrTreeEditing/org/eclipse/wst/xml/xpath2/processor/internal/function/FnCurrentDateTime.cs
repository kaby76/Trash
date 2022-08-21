using System.Diagnostics;
using System.Collections;

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
///     Jesper S Moller - bug 286452 - always return the stable date/time from dynamic context
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDateTime;
	using XSDayTimeDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDayTimeDuration;

	/// <summary>
	/// Returns the xs:dateTime (with timezone) from the dynamic context. (See
	/// Section C.2 Dynamic Context ComponentsXP.) This is a xs:dateTime that is
	/// current at some time during the evaluation of a query or transformation in
	/// which fn:current-dateTime() is executed. This function is stable. The precise
	/// instant during the query or transformation represented by the value of
	/// fn:current-dateTime() is implementation dependent.
	/// </summary>
	public class FnCurrentDateTime : Function
	{
		/// <summary>
		/// Constructor for FnCurrentDateTime.
		/// </summary>
		public FnCurrentDateTime() : base(new QName("current-dateTime"), 0)
		{
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of evaluation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return current_dateTime(args, ec.DynamicContext);
		}

		/// <summary>
		/// Current-Date-Time operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="dc">
		///            Result of dynamic context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:current-dateTime operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence current_dateTime(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence current_dateTime(ICollection args, DynamicContext dc)
		{
			Debug.Assert(args.Count == 0);

			var d = dc.TimezoneOffset;
			XSDayTimeDuration tz = new XSDayTimeDuration(0, d.Hours, d.Minutes, 0.0, d.Sign == -1);

			AnyType res = new XSDateTime(dc.CurrentDateTime, tz);

			return ResultSequenceFactory.create_new(res);
		}
	}

}