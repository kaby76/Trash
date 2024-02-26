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
///     Mukul Gandhi - bug 280554 - improvements to the function implementation
///     David Carver - bug 282096 - improvements for surrogate handling  
///     Jesper Steen Moeller - bug 282096 - clean up string storage and fix surrogate handling
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using CodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator;
	using StringCodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.StringCodePointIterator;

	/// <summary>
	/// Returns the sequence of code points that constitute an xs:string. If $arg is
	/// a zero-length string or the empty sequence, the empty sequence is returned.
	/// </summary>
	public class FnStringToCodepoints : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnStringToCodepoints.
		/// </summary>
		public FnStringToCodepoints() : base(new QName("string-to-codepoints"), 1)
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
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return string_to_codepoints(args);
		}

		/// <summary>
		/// Base-Uri operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:base-uri operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence string_to_codepoints(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence string_to_codepoints(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;
			if (arg1.empty())
			{
			   return ResultBuffer.EMPTY;
			}

			XSString xstr = (XSString) arg1.first();

			CodePointIterator cpi = new StringCodePointIterator(xstr.value());

			ResultBuffer rs = new ResultBuffer();
			for (int codePoint = cpi.current(); codePoint != org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator_Fields.DONE; codePoint = cpi.next())
			{
				   rs.add(new XSInteger(new System.Numerics.BigInteger(codePoint)));
			}
			return rs.Sequence;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnStringToCodepoints))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}