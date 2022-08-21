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


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;

	/// <summary>
	/// Returns the namespace URI for $arg as an xs:string. If $arg is the empty
	/// sequence, the empty sequence is returned. If $arg is in no namespace, the
	/// zero-length string is returned.
	/// </summary>
	public class FnNamespaceUriFromQName : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnNamespaceUriFromQName.
		/// </summary>
		public FnNamespaceUriFromQName() : base(new QName("namespace-uri-from-QName"), 1)
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
			return @namespace(args);
		}

		/// <summary>
		/// Namespace-uri-from-QName operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:namespace-uri-from-QName operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence namespace(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence @namespace(ICollection args)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arg
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			QName qname = (QName) arg1.first();

			string ns = qname.@namespace();

			if (string.ReferenceEquals(ns, null))
			{
				ns = "";
			}
			return new XSAnyURI(ns);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnNamespaceUriFromQName))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new QName(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}
	}

}