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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;

	/// <summary>
	/// Support for To operation.
	/// </summary>
	public class OpTo : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for OpTo.
		/// </summary>
		public OpTo() : base(new QName("to"), 2)
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
			Debug.Assert(args.Count == 2);

			// Iterator i = args.iterator();

			// return op_to( (ResultSequence) i.next(), (ResultSequence) i.next());
			return op_to(args);
		}

		/// <summary>
		/// Op-To operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence op_to(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence op_to(ICollection args)
		{
			// convert arguments
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arguments
			IEnumerator iter = cargs.GetEnumerator();
            iter.MoveNext();
            ResultSequence r = (ResultSequence) iter.Current;
			int one = (int)((XSInteger) r.first()).int_value();
            iter.MoveNext();
            r = (ResultSequence) iter.Current;
			if (r == null || r.first() == null)
			{
				return ResultBuffer.EMPTY;
			}
            int two = (int)((XSInteger) r.first()).int_value();

			if (one > two)
			{
				return ResultBuffer.EMPTY;
			}

			// inclusive first and last
			if (one == two)
			{
				return new XSInteger(new System.Numerics.BigInteger(one));
			}
			/*
			 * for(one++; one <= two; one++) { rs.add(new XSInteger(one)); }
			 * 
			 * return rs;
			 */
			return new RangeResultSequence(one, two);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(OpTo))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
        
					SeqType st = new SeqType(new XSInteger());
        
					_expected_args.Add(st);
					_expected_args.Add(st);
				}
				return _expected_args;
			}
		}
	}

}