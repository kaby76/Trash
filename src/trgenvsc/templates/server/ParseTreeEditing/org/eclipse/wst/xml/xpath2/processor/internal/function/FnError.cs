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
///     Jesper Steen Moeller - bug 28149 - add remaining fn:error functionality
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// The fn:error function causes the evaluation of the outermost XQuery or
	/// transformation to stop. While this function never returns a value, an error,
	/// if it occurs, is returned to the external processing environment as an
	/// xs:anyURI or an xs:QName. The error xs:anyURI is derived from the error
	/// xs:QName. An error xs:QName with namespace URI NS and local part LP will be
	/// returned as the xs:anyURI NS#LP. The method by which the xs:anyURI or
	/// xs:QName is returned to the external processing environment is implementation
	/// dependent.
	/// </summary>
	public class FnError : Function
	{

		private static ArrayList _expected_args;
		private static ArrayList _expected_args1;

		// XXX overloaded...
		/// <summary>
		/// Constructor for FnError.
		/// </summary>
		public FnError() : base(new QName("error"), 0, 3)
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
			// Differentiate depending on whether there is one (required) argument or whatever.
			ICollection cargs = Function.convert_arguments(args, args.Count == 1 ? expected_args1() : expected_args());

			QName code = null;
			ResultSequence items = null;
			string description = null;

			// Iterate over the args
			IEnumerator it = cargs.GetEnumerator();

            if (it.MoveNext())
            {
                ResultSequence rsQName = (ResultSequence) it.Current;
				// for arity 2 and 3, the code is not mandatory, as in fn:code((), "description). Handle this:
				if (!rsQName.empty())
				{
					code = (QName)rsQName.first();
				}
			}
			// Next arg (if present) is the description
			if (it.MoveNext())
            {
                ResultSequence rsDescription = (ResultSequence) it.Current;
				description = ((XSString)rsDescription.first()).value();
			}
			// Final arg (if present) is the list of items
			if (it.MoveNext())
            {
                items = (ResultSequence) it.Current;
            }

			// Handle the code if missing
			if (code == null)
			{
				code = new QName("err", "FOER0000", "http://www.w3.org/2005/xqt-errors");
			}

			return error(code, description, items);
		}

		/// <summary>
		/// Error operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:error operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence error(org.eclipse.wst.xml.xpath2.processor.internal.types.QName error, String description, org.eclipse.wst.xml.xpath2.api.ResultSequence items) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence error(QName error, string description, ResultSequence items)
		{

			throw DynamicError.user_error(error.@namespace(), error.local(), description);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnError))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new QName(), SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(typeof(AnyType), SeqType.OCC_STAR));
				}
        
				return _expected_args;
			}
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args1()
		{
			lock (typeof(FnError))
			{
				if (_expected_args1 == null)
				{
					_expected_args1 = new ArrayList();
					_expected_args1.Add(new SeqType(new QName(), SeqType.OCC_NONE));
				}
        
				return _expected_args1;
			}
		}
	}

}