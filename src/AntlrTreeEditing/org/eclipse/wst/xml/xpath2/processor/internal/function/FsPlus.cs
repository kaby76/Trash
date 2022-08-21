using System;
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
///     Jesper Steen Moller  - Bug 286062 - Add type promotion for numeric operators  
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDecimal = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDecimal;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;

	/// <summary>
	/// Class for Plus function.
	/// </summary>
	public class FsPlus : Function
	{
		/// <summary>
		/// Constructor for FsPlus.
		/// </summary>
		public FsPlus() : base(new QName("plus"), 2)
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
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			return fs_plus(args);
		}

		/// <summary>
		/// Convert and promote arguments for operation.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="sc"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of conversion. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static java.util.Collection convert_args(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private static ICollection convert_args(ICollection args)
		{
			var result = new ArrayList();

			// Keep track of numeric types for promotion
			bool has_float = false;
			bool has_double = false;

			// atomize arguments
			for (IEnumerator i = args.GetEnumerator(); i.MoveNext();)
			{
				ResultSequence rs = FnData.atomize((ResultSequence) i.Current);

				if (rs.empty())
				{
					return new ArrayList();
				}

				if (rs.size() > 1)
				{
					throw new DynamicError(TypeError.invalid_type(null));
				}

				AnyType arg = (AnyType) rs.item(0);

				if (arg is XSUntypedAtomic)
				{
					arg = new XSDouble(arg.StringValue);
				}

				if (arg is XSDouble)
				{
					has_double = true;
				}
				if (arg is XSFloat)
				{
					has_float = true;
				}
				result.Add(ResultBuffer.wrap(arg));
			}

			if (has_double)
			{
				has_float = false;
			}

			if (has_double || has_float)
			{
				var result2 = new ArrayList();

				// promote arguments
				for (IEnumerator i = result.GetEnumerator(); i.MoveNext();)
				{
					ResultSequence rs = (ResultSequence) i.Current;

					Item arg = rs.item(0);

					if (has_double && (arg is XSFloat))
					{
						arg = new XSDouble(((XSFloat)arg).float_value());
					}
					else if (has_double && (arg is XSDecimal))
					{
						arg = new XSDouble(((XSDecimal)arg).double_value());
					}
					else if (has_float && (arg is XSDecimal))
					{
						arg = new XSFloat((float)((XSDecimal)arg).double_value());
					}
					result2.Add(arg);
				}
				return result2;
			}

			return result;
		}


		/// <summary>
		/// General operation on the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="sc"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence fs_plus(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence fs_plus(ICollection args)
		{
			return do_math_op(args, typeof(MathPlus), "plus");
		}

		/// <summary>
		/// Unary operation on the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of the operation. </returns>
		public static ResultSequence fs_plus_unary(ICollection args)
		{

			// make sure we got only one arg
			if (args.Count != 1)
			{
				DynamicError.throw_type_error();
			}
            var i = args.GetEnumerator();
            i.MoveNext();
            ResultSequence arg = (ResultSequence) i.Current;

			// make sure we got only one numeric atom
			if (arg.size() != 1)
			{
				DynamicError.throw_type_error();
			}
			Item at = arg.first();
			if (!(at is NumericType))
			{
				DynamicError.throw_type_error();
			}

			// no-op
			return arg;
		}

		// voodoo
		/// <summary>
		/// Mathematical operation on the arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="type">
		///            type of arguments. </param>
		/// <param name="mname">
		///            Method name for template simulation. </param>
		/// <param name="sc"> </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
		public static ResultSequence do_math_op(ICollection args, Type type, string mname)
        {
            throw new Exception();
            //// sanity check args + convert em
            //if (args.Count != 2)
            //{
            //	DynamicError.throw_type_error();
            //}

            //ICollection cargs = convert_args(args);

            //if (cargs.Count == 0)
            //{
            //	return ResultBuffer.EMPTY;
            //}

            //// make sure arugments are good [at least the first one]
            //IEnumerator argi = cargs.GetEnumerator();
            //         argi.MoveNext();
            //Item arg = ((ResultSequence) argi.Current).item(0);
            //         argi.MoveNext();
            //ResultSequence arg2 = (ResultSequence) argi.Current;

            //if (!(type.IsInstanceOfType(arg)))
            //{
            //	DynamicError.throw_type_error();
            //}

            //// here is da ownage
            //try
            //{
            //	Type[] margsdef = new Type[] {typeof(ResultSequence)};
            //	Method method = null;

            //	method = type.GetMethod(mname, margsdef);

            //	object[] margs = new object[] {arg2};
            //	return (ResultSequence) method.invoke(arg, margs);
            //         }
            //catch
            //         {
            //             throw;
            //         }
            //return null; // unreach!
        }
	}

}