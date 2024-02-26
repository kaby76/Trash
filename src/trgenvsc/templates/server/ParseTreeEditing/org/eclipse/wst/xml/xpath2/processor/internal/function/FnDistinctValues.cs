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
///     David Carver (STAR) - bug 262765 - fixed distinct-values comparison logic.
///                           There is probably an easier way to do the comparison.
/// <<<<<<< FnDistinctValues.java
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// =======
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
///     Mukul Gandhi - bug 339025 - fixes to fn:distinct-values function. ability to find distinct values on node items.
/// >>>>>>> 1.6
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns the sequence that results from removing from $arg all but one of a
	/// set of values that are eq to one other. Values that cannot be compared, i.e.
	/// the eq operator is not defined for their types, are considered to be
	/// distinct. Values of type xdt:untypedAtomic are compared as if they were of
	/// type xs:string. The order in which the sequence of values is returned is
	/// implementation dependent.
	/// </summary>
	public class FnDistinctValues : AbstractCollationEqualFunction
	{
		/// <summary>
		/// Constructor for FnDistinctValues.
		/// </summary>
		public FnDistinctValues() : base(new QName("distinct-values"), 1, 2)
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
			return distinct_values(args, ec.DynamicContext);
		}

		/// <summary>
		/// Distinct-values operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:distinct-values operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence distinct_values(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence distinct_values(ICollection args, DynamicContext context)
		{

			ResultBuffer rs = new ResultBuffer();

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg1 = (ResultSequence) citer.Current;
			ResultSequence arg2 = ResultBuffer.EMPTY;
			if (citer.MoveNext())
            {
                arg2 = (ResultSequence) citer.Current;
            }

			string collationURI = context.CollationProvider.DefaultCollation;
			if (!(arg2 == null || arg2.empty()))
			{
				XSString collation = (XSString) arg2.item(0);
				collationURI = collation.StringValue;
			}

			for (var iter = arg1.iterator(); iter.MoveNext();)
			{
				AnyAtomicType atomizedItem = (AnyAtomicType) FnData.atomize((Item) iter.Current);
				if (!contains(rs, atomizedItem, context, collationURI))
				{
					rs.add(atomizedItem);
				}
			}

			return rs.Sequence;
		}

		/// <summary>
		/// Support for Contains interface.
		/// </summary>
		/// <param name="rs">
		///            input1 expression sequence. </param>
		/// <param name="item">
		///            input2 expression of any atomic type. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
		protected internal static bool contains(ResultBuffer rs, AnyAtomicType item, DynamicContext context, string collationURI)
		{
			if (!(item is CmpEq))
			{
				return false;
			}

			return hasValue(rs, item, context, collationURI);
		}

	}

}