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
///     Jesper Moller - bug 280555 - Add pluggable collation support
///     David Carver (STAR) - bug 262765 - fixed collation and comparison issues.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns a sequence of positive integers giving the positions within the
	/// sequence $seqParam of items that are equal to $srchParam.
	/// </summary>
	public class FnIndexOf : AbstractCollationEqualFunction
	{

		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnIndexOf.
		/// </summary>
		public FnIndexOf() : base(new QName("index-of"), 2, 3)
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
			return index_of(args, ec.DynamicContext);
		}

		/// <summary>
		/// Obtain a comparable type.
		/// </summary>
		/// <param name="at">
		///            expression of any type. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static CmpEq get_comparable(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType at) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private static CmpEq get_comparable(AnyType at)
		{
			if (at is NodeType)
			{
				XSString nodeString = new XSString(at.StringValue);
				return nodeString;
			}

			if (!(at is AnyAtomicType))
			{
				DynamicError.throw_type_error();
			}

			if (!(at is CmpEq))
			{
				throw DynamicError.not_cmp(null);
			}

			return (CmpEq) at;
		}

		/// <summary>
		/// Index-Of operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="dynamicContext"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:index-of operation. </returns>
		public static ResultSequence index_of(ICollection args, DynamicContext dc)
		{
			Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg1 = (ResultSequence) citer.Current;
            citer.MoveNext();
            ResultSequence arg2 = (ResultSequence) citer.Current;

			if (arg1 == null || arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

			// sanity chex
			if (arg2 == null || arg2.size() != 1)
			{
				DynamicError.throw_type_error();
			}

			string collationUri = dc.CollationProvider.DefaultCollation;
            if (citer.MoveNext())
            {
                ResultSequence arg3 = (ResultSequence) citer.Current;
				if (!arg3.empty())
				{
					XSString collation = (XSString) arg3.first();
					collationUri = collation.StringValue;
				}
			}

			ResultBuffer rb = new ResultBuffer();
			AnyAtomicType at = (AnyAtomicType)arg2.first();

			get_comparable(at);

			int index = 1;

			for (var i = arg1.iterator(); i.MoveNext();)
			{
				AnyType cmptype = (AnyType) i.Current;
				get_comparable(cmptype);

				if (!(at is CmpEq))
				{
					continue;
				}

				if (isBoolean(cmptype, at))
				{
					XSBoolean boolat = (XSBoolean) cmptype;
					if (boolat.eq(at, dc))
					{
						rb.add(new XSInteger(new System.Numerics.BigInteger(index)));
					}
				}
				else
				{

				if (isNumeric(cmptype, at))
				{
					NumericType numericat = (NumericType) at;
					if (numericat.eq(cmptype, dc))
					{
						rb.add(new XSInteger(new System.Numerics.BigInteger(index)));
					}
				}
				else
				{

				if (isDuration(cmptype, at))
				{
					XSDuration durat = (XSDuration) at;
					if (durat.eq(cmptype, dc))
					{
						rb.add(new XSInteger(new System.Numerics.BigInteger(index)));
					}
				}
				else
				{

				if (at is QName && cmptype is QName)
				{
					QName qname = (QName)at;
					if (qname.eq(cmptype, dc))
					{
						rb.add(new XSInteger(new System.Numerics.BigInteger(index)));
					}
				}
				else
				{

				if (needsStringComparison(cmptype, at))
				{
					XSString xstr1 = new XSString(cmptype.StringValue);
					XSString itemStr = new XSString(at.StringValue);
					if (FnCompare.compare_string(collationUri, xstr1, itemStr, dc).Equals(System.Numerics.BigInteger.Zero))
					{
						rb.add(new XSInteger(new System.Numerics.BigInteger(index)));
					}
				}
				}
				}
				}
				}

				index++;
			}

			return rb.Sequence;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnIndexOf))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(typeof(AnyType), SeqType.OCC_STAR);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(typeof(AnyAtomicType), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}

	}

}