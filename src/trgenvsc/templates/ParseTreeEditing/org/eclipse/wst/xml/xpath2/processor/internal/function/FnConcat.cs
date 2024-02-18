using System.Collections;
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
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Sequence concatenation function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:concat($arg1 as xdt:anyAtomicType?, $arg2 as xdt:anyAtomicType?,
	/// ... ) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class accepts two or more xdt:anyAtomicType arguments and converts them
	/// to xs:string. It then returns the xs:string that is the concatenation of the
	/// values of its arguments after conversion. If any of the arguments is the
	/// empty sequence, the argument is treated as the zero-length string.
	/// </para>
	/// 
	/// <para>
	/// The concat() function is specified to allow an arbitrary number of arguments
	/// that are concatenated together.
	/// </para>
	/// </summary>
	public class FnConcat : Function
	{

		/// <summary>
		/// Constructor for FnConcat.
		/// </summary>
		public FnConcat() : base(new QName("concat"), 2, int.MaxValue)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            is evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the concatenation of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return concat(args);
		}

		/// <summary>
		/// Concatenate the arguments.
		/// </summary>
		/// <param name="args">
		///            are concatenated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of the concatenation of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence concat(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence concat(ICollection args)
		{

			// sanity check
			if (args.Count < 2)
			{
				DynamicError.throw_type_error();
			}

			ResultBuffer rs = new ResultBuffer();

			string result = "";

			// go through args
			StringBuilder buf = new StringBuilder();
			for (IEnumerator argi = args.GetEnumerator(); argi.MoveNext();)
			{
				ResultSequence arg = (ResultSequence) argi.Current;

				int size = arg.size();

				// sanity check
				if (size > 1)
				{
					DynamicError.throw_type_error();
				}

				if (size == 0)
				{
					continue;
				}

				Item at = arg.first();

				buf.Append(at.StringValue);

			}
			result = buf.ToString();

			rs.add(new XSString(result));

			return rs.Sequence;
		}
	}

}