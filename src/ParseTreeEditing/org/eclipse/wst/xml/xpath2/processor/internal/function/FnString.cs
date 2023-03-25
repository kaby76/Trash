using System.Collections;
using System.Diagnostics;

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
///     Mukul Gandhi - bug 274471 - improvements to fn:string function (support for arity 0) 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moller  - bug 281938 - handle context and empty sequences correctly
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns the value of $arg represented as a xs:string. If no argument is
	/// supplied, this function returns the string value of the context item (.).
	/// </summary>
	public class FnString : Function
	{
		/// <summary>
		/// Constructor for FnString.
		/// </summary>
		public FnString() : base(new QName("string"), 0, 1)
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
			return @string(args, ec);
		}

		/// <summary>
		/// String operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:string operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence string(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence @string(ICollection args, EvaluationContext ec)
		{

			Debug.Assert(args.Count == 0 || args.Count == 1);

			ResultSequence arg1 = null;

			if (args.Count == 0)
			{
				// support for arity = 0
				return getResultSetForArityZero(ec);
			}
			else
            {
                var i = args.GetEnumerator();
                i.MoveNext();
                arg1 = (ResultSequence) i.Current;
            }

			// sanity check args
			if (arg1.size() > 1)
			{
				throw new DynamicError(TypeError.invalid_type(null));
			}

			ResultBuffer rs = new ResultBuffer();
			if (arg1.empty())
			{
				rs.add(new XSString(""));
			}
			else
			{
				Item at = arg1.first();
				rs.add(new XSString(at.StringValue));
			}

			return rs.Sequence;
		}

	}

}