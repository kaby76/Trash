using System.Diagnostics;
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
///     Jesper Steen Moeller - bug 262765 - fixes float handling for fn:number 
///     Mukul Gandhi - bug 298519 - improvements to fn:number implementation,
///                                 catering to node arguments. 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;

	/// <summary>
	/// Returns the value indicated by $arg or, if $arg is not specified, the context
	/// item after atomization, converted to an xs:double. If $arg or the context
	/// item cannot be converted to an xs:double, the xs:double value NaN is
	/// returned. If the context item is undefined an error is raised:
	/// [err:FONC0001].
	/// </summary>
	public class FnNumber : Function
	{
		/// <summary>
		/// Constructor for FnNumber.
		/// </summary>
		public FnNumber() : base(new QName("number"), 0, 1)
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

			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			ResultSequence argument = null;
			if (args.Count == 0)
			{
				argument = getResultSetForArityZero(ec);
			}
			else
            {
                var i = args.GetEnumerator();
                i.MoveNext();
                argument = (ResultSequence) i.Current;
            }

			return fn_number(argument, ec);
		}

		/// <summary>
		/// Number operation.
		/// </summary>
		/// <param name="arg">
		///            Result from the expressions evaluation. </param>
		/// <param name="dc">
		///            Result of dynamic context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:number operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.processor.internal.types.XSDouble fn_number(org.eclipse.wst.xml.xpath2.api.ResultSequence arg, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static XSDouble fn_number(ResultSequence arg, EvaluationContext ec)
		{

			if (arg.size() > 1)
			{
				throw new DynamicError(TypeError.invalid_type("bad argument passed to fn:number()"));
			}
			else if (arg.size() == 1)
			{
				Item at = arg.first();

				/*
				if (!(at instanceof AnyAtomicType))
					DynamicError.throw_type_error();
				*/

				if (at is AnyAtomicType)
				{
				  if ((at is XSDouble))
				  {
					 return (XSDouble)at;
				  }
				  else if ((at is XSFloat))
				  {
					  float value = ((XSFloat)at).float_value();
					  if (float.IsNaN(value))
					  {
						  return new XSDouble(double.NaN);
					  }
					  else if (value == float.NegativeInfinity)
					  {
						  return new XSDouble(double.NegativeInfinity);
					  }
					  else if (value == float.PositiveInfinity)
					  {
						  return new XSDouble(double.PositiveInfinity);
					  }
					  else
					  {
						  return new XSDouble((double)value);
					  }
				  }
				  else
				  {
					 XSDouble d = XSDouble.parse_double(at.StringValue);
					 return d != null ? d : new XSDouble(double.NaN);
				  }
				}
				else if (at is NodeType)
				{
					XSDouble d = XSDouble.parse_double((FnData.atomize(at)).StringValue);
					return d != null ? d : new XSDouble(double.NaN);
				}
			}
			else
			{
				return new XSDouble(double.NaN);
			}

			// unreach
			return null;
		}

	}

}