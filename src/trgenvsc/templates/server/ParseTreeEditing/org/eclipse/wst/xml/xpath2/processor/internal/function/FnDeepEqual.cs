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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// The function assesses whether two sequences are deep-equal to each other. To
	/// be deep-equal, they must contain items that are pairwise deep-equal; and for
	/// two items to be deep-equal, they must either by atomic values that compare
	/// equal, or nodes of the same kind, with the same name, whose children are
	/// deep-equal. This is defined in more detail below. The $collation argument
	/// identifies a collation which is used at all levels of recursion when strings
	/// are compared (but not when names are compared), according to the rules in
	/// 7.3.1 Collations in the specification.
	/// </summary>
	public class FnDeepEqual : AbstractCollationEqualFunction
	{
		/// <summary>
		/// Constructor for FnDeepEqual.
		/// </summary>
		public FnDeepEqual() : base(new QName("deep-equal"), 2, 3)
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
			return deep_equal(args, ec);
		}

		/// <summary>
		/// Deep-Equal expression operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="context">
		///            Dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:deep-equal operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence deep_equal(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence deep_equal(ICollection args, EvaluationContext context)
		{

			// get args
			IEnumerator citer = args.GetEnumerator();
            citer.MoveNext();
            ResultSequence arg1 = (ResultSequence) citer.Current;
            citer.MoveNext();
            ResultSequence arg2 = (ResultSequence) citer.Current;
			ResultSequence arg3 = null;
			string collationURI = context.StaticContext.CollationProvider.DefaultCollation;
			if (citer.MoveNext())
            {
                arg3 = (ResultSequence) citer.Current;
				if (!(arg3 == null || arg3.empty()))
				{
					collationURI = arg3.first().StringValue;
				}
			}

			bool result = deep_equal(arg1, arg2, context, collationURI);

			return ResultSequenceFactory.create_new(new XSBoolean(result));
		}

		/// <summary>
		/// Deep-Equal boolean operation.
		/// </summary>
		/// <param name="one">
		///            input1 xpath expression/variable. </param>
		/// <param name="two">
		///            input2 xpath expression/variable. </param>
		/// <param name="context">
		///            Current dynamic context </param>
		/// <returns> Result of fn:deep-equal operation. </returns>
		public static bool deep_equal(ResultSequence one, ResultSequence two, EvaluationContext context, string collationURI)
		{
			if (one.empty() && two.empty())
			{
				return true;
			}

			if (one.size() != two.size())
			{
				return false;
			}

			var onei = one.iterator();
			var twoi = two.iterator();

			while (onei.MoveNext())
			{
				AnyType a = (AnyType) onei.Current;
                twoi.MoveNext();
                AnyType b = (AnyType) twoi.Current;

				if (!deep_equal(a, b, context, collationURI))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Deep-Equal boolean operation for inputs of any type.
		/// </summary>
		/// <param name="one">
		///            input1 xpath expression/variable. </param>
		/// <param name="two">
		///            input2 xpath expression/variable. </param>
		/// <param name="context"> </param>
		/// <returns> Result of fn:deep-equal operation. </returns>
		public static bool deep_equal(AnyType one, AnyType two, EvaluationContext context, string collationURI)
		{
			if ((one is AnyAtomicType) && (two is AnyAtomicType))
			{
				return deep_equal_atomic((AnyAtomicType) one, (AnyAtomicType) two, context.DynamicContext, collationURI);
			}

			else if (((one is AnyAtomicType) && (two is NodeType)) || ((one is NodeType) && (two is AnyAtomicType)))
			{
				return false;
			}
			else if ((one is NodeType) && (two is NodeType))
			{
				return deep_equal_node((NodeType) one, (NodeType) two, context);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Deep-Equal boolean operation for inputs of any atomic type.
		/// </summary>
		/// <param name="one">
		///            input1 xpath expression/variable. </param>
		/// <param name="two">
		///            input2 xpath expression/variable. </param>
		/// <returns> Result of fn:deep-equal operation. </returns>
		public static bool deep_equal_atomic(AnyAtomicType one, AnyAtomicType two, DynamicContext context, string collationURI)
		{
			if (!(one is CmpEq))
			{
				return false;
			}
			if (!(two is CmpEq))
			{
				return false;
			}

			CmpEq a = (CmpEq) one;

			try
			{
				if (isNumeric(one, two))
				{
					NumericType numeric = (NumericType) one;
					if (numeric.eq(two, context))
					{
						return true;
					}
					else
					{
						XSString value1 = new XSString(one.StringValue);
						if (value1.eq(two, context))
						{
							return true;
						}
					}
				}

				if (a.eq(two, context))
				{
					return true;
				}

				if (needsStringComparison(one, two))
				{
					XSString xstr1 = new XSString(one.StringValue);
					XSString xstr2 = new XSString(two.StringValue);
					if (FnCompare.compare_string(collationURI, xstr1, xstr2, context).Equals(System.Numerics.BigInteger.Zero))
					{
						return true;
					}
				}
				return false;
			}
			catch (DynamicError)
			{
				return false; // XXX ???
			}
		}

		/// <summary>
		/// Deep-Equal boolean operation for inputs of node type.
		/// </summary>
		/// <param name="one">
		///            input1 xpath expression/variable. </param>
		/// <param name="two">
		///            input2 xpath expression/variable. </param>
		/// <returns> Result of fn:deep-equal operation. </returns>
		public static bool deep_equal_node(NodeType one, NodeType two, EvaluationContext context)
		{
			Node a = one.node_value();
			Node b = two.node_value();

			if (a.isEqualNode(b))
			{
				return true;
			}

			return false;
		}
	}

}