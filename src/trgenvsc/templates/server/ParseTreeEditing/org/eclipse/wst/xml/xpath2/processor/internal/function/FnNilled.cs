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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Returns an xs:boolean indicating whether the argument node is "nilled". If
	/// the argument is not an element node, returns the empty sequence.
	/// </summary>
	public class FnNilled : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnNilled.
		/// </summary>
		public FnNilled() : base(new QName("nilled"), 0, 1)
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
			return nilled(args);
		}

		/// <summary>
		/// Nilled operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:nilled operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence nilled(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence nilled(ICollection args)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());


            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence)i.Current;

            if (arg1 == null || arg1.empty())
			{
				return arg1;
			}
			NodeType nt = (NodeType) arg1.first();

			return nt.nilled();
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnNilled))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}
	}

}