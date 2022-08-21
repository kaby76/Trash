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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns an xs:QName with the namespace URI given in $paramURI. If $paramURI
	/// is the empty string or the empty sequence, it represents "no namespace". The
	/// prefix (or absence of a prefix) in $paramQName is retained in the returned
	/// xs:QName value. The local name in the result is taken from the local part of
	/// $paramQName.
	/// </summary>
	public class FnQName : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnQName.
		/// </summary>
		public FnQName() : base(new QName("QName"), 2)
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
			return resolve_QName(args, ec.StaticContext);
		}

		/// <summary>
		/// Resolve the QName of the given arguments.
		/// </summary>
		/// <param name="args">
		///            Result from teh expressions evaluation. </param>
		/// <param name="sc">
		///            Result of static context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the fn:QName operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence resolve_QName(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.StaticContext sc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence resolve_QName(ICollection args, StaticContext sc)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;

			string ns = null;
			if (!(arg1 == null || arg1.empty()))
			{
				ns = ((XSString) arg1.first()).value();
			}

            argiter.MoveNext();
            ResultSequence arg2 = (ResultSequence) argiter.Current;
			string name = ((XSString) arg2.first()).value();

			QName qn = QName.parse_QName(name);
			if (qn == null)
			{
				throw DynamicError.lexical_error(null);
			}
			qn.set_namespace(ns);

			return qn;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnQName))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}