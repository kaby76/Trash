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
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// 
	/// <summary>
	/// <para>
	/// String comparison function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:compare($comparand1 as xs:string?, $comparand2 as xs:string?) as
	/// xs:integer?
	/// </para>
	/// 
	/// <para>
	/// This class returns -1, 0, or 1, depending on whether the value of $comparand1
	/// is respectively less than, equal to, or greater than the value of
	/// $comparand2.
	/// </para>
	/// 
	/// <para>
	/// If the value of $comparand2 begins with a string that is equal to the value
	/// of $comparand1 (according to the collation that is used) and has additional
	/// code points following that beginning string, then the result is -1. If the
	/// value of $comparand1 begins with a string that is equal to the value of
	/// $comparand2 and has additional code points following that beginning string,
	/// then the result is 1.
	/// </para>
	/// 
	/// <para>
	/// If either argument is the empty sequence, the result is the empty sequence.
	/// </para>
	/// </summary>
	public class FnCompare : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor of FnCompare.
		/// </summary>
		public FnCompare() : base(new QName("compare"), 2, 3)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            is evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the comparison of the arguments. </returns>
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return compare(args, ec.DynamicContext);
		}

		/// <summary>
		/// Compare the arguments.
		/// </summary>
		/// <param name="args">
		///            are compared (optional 3rd argument is the collation) </param>
		/// <param name="dynamicContext">
		/// 	       Current dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of the comparison of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence compare(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence compare(ICollection args, DynamicContext context)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;
            argiter.MoveNext();
			ResultSequence arg2 = (ResultSequence) argiter.Current;

			string collationUri = context.CollationProvider.DefaultCollation;
			if (argiter.MoveNext())
            {
                ResultSequence collArg = (ResultSequence) argiter.Current;
				collationUri = collArg.first().StringValue;
			}

			XSString xstr1 = arg1.empty() ? null : (XSString) arg1.first();
			XSString xstr2 = arg2.empty() ? null : (XSString) arg2.first();

			System.Numerics.BigInteger result = compare_string(collationUri, xstr1, xstr2, context);
			if (result != null)
			{
				return ResultSequenceFactory.create_new(new XSInteger(result));
			}
			else
			{
				return ResultSequenceFactory.create_new();
			}
		}

		public static System.Numerics.BigInteger compare_string(string collationUri, XSString xstr1, XSString xstr2, DynamicContext context)
		{
			var collator = context.CollationProvider.getCollation(collationUri);
			if (collator == null)
			{
				throw DynamicError.unsupported_collation(collationUri);
			}

			if (xstr1 == null || xstr2 == null)
			{
				return default;
			}

			int ret = collator.Compare(xstr1.value(), xstr2.value());

			if (ret == 0)
			{
				return System.Numerics.BigInteger.Zero;
			}
			else if (ret < 0)
			{
				return new System.Numerics.BigInteger(-1);
			}
			else
			{
				return System.Numerics.BigInteger.One;
			}
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnCompare))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}