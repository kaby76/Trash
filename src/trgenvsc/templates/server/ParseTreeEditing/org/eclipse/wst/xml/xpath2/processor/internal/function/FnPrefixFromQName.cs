using System.Collections;
using java.xml;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Mukul Gandhi, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Mukul Gandhi - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{
	using XMLConstants = java.xml.XMLConstants;

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSNCName = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSNCName;

	/// <summary>
	/// Returns an xs:NCName representing the prefix for $arg. If $arg is the empty
	/// sequence, or $arg has no prefix, an empty sequence is returned.
	/// </summary>
	public class FnPrefixFromQName : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnPrefixFromQName
		/// </summary>
		public FnPrefixFromQName() : base(new QName("prefix-from-QName"), 1)
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
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return prefix(args, ec.StaticContext);
		}

		/// <summary>
		/// Prefix-from-QName operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:prefix-from-QName operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence prefix(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.StaticContext sc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence prefix(ICollection args, StaticContext sc)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arg
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1.empty())
			{
			  return ResultBuffer.EMPTY;
			}

			QName qname = (QName) arg1.first();

			string prefix = qname.prefix();

			if (!string.ReferenceEquals(prefix, null))
			{
				if (!XMLConstants.NULL_NS_URI.Equals(sc.NamespaceContext.getNamespaceURI(prefix)))
				{
					  return new XSNCName(prefix);
				}
				else
				{
					throw DynamicError.invalidPrefix();
				}
			}
			return ResultBuffer.EMPTY;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnPrefixFromQName))
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