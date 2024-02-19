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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Support for Except operation on node types.
	/// </summary>
	public class OpExcept : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for OpExcept.
		/// </summary>
		public OpExcept() : base(new QName("except"), 2)
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
			Debug.Assert(args.Count >= min_arity() && args.Count <= max_arity());

			return op_except(args);
		}

		/// <summary>
		/// Op-Except operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence op_except(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence op_except(ICollection args)
		{
			ResultBuffer rs = new ResultBuffer();

			// convert arguments
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arguments
			IEnumerator iter = cargs.GetEnumerator();
            iter.MoveNext();
            ResultSequence one = (ResultSequence) iter.Current;
            iter.MoveNext();
			ResultSequence two = (ResultSequence) iter.Current;

			// XXX lame
			for (IEnumerator i = one.iterator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;
				bool found = false;

				// death
				for (var j = two.iterator(); j.MoveNext();)
				{
					NodeType node2 = (NodeType) j.Current;

					if (node.node_value() == node2.node_value())
					{
						found = true;
						break;
					}

				}
				if (!found)
				{
					rs.add(node);
				}
			}
			rs = NodeType.linarize(rs);

			return rs.Sequence;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(OpExcept))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
        
					SeqType st = new SeqType(SeqType.OCC_STAR);
        
					_expected_args.Add(st);
					_expected_args.Add(st);
				}
				return _expected_args;
			}
		}
	}

}