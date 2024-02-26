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
///     David Carver - STAR - bug 262765 - fixed implementation of fn:local-name according to spec.  
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// Returns the local part of the name of $arg as an xs:string that will either
	/// be the zero-length string or will have the lexical form of an xs:NCName.
	/// </summary>
	public class FnLocalName : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnLocalName.
		/// </summary>
		public FnLocalName() : base(new QName("local-name"), 0, 1)
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
			return local_name(args, ec);
		}

		/// <summary>
		/// Local-Name operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:local-name operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence local_name(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence local_name(ICollection args, EvaluationContext context)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get arg
			ResultSequence arg1 = null;

			if (cargs.Count == 0)
			{
				if (context.ContextItem == null)
				{
					throw DynamicError.contextUndefined();
				}
				else
				{
					arg1 = (AnyType) context.ContextItem;
				}
			}
			else
            {
                var i = cargs.GetEnumerator();
                i.MoveNext();
                arg1 = (ResultSequence) i.Current;

            }

			if (arg1.empty())
			{
				return new XSString("");
			}

			NodeType an = (NodeType) arg1.first();


			QName name = an.node_name();

			string sname = "";
			if (name != null)
			{
				sname = name.local();
			}

			return new XSString(sname);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnLocalName))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(SeqType.OCC_QMARK);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}
	}

}