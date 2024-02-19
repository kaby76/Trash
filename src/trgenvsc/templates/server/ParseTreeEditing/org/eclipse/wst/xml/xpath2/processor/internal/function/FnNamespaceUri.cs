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
///     David Carver - bug 262765 - corrected implementation according to spec. 
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
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;

	/// <summary>
	/// Returns the namespace URI of the xs:QName of $arg.
	/// </summary>
	public class FnNamespaceUri : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnNamespaceUri.
		/// </summary>
		public FnNamespaceUri() : base(new QName("namespace-uri"), 0, 1)
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
			return namespace_uri(args, ec);
		}

		/// <summary>
		/// Namespace-Uri operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:namespace-uri operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence namespace_uri(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence namespace_uri(ICollection args, EvaluationContext context)
		{

			ICollection cargs = Function.convert_arguments(args, expected_args());

			ResultSequence arg1 = null;
			if (cargs.Count == 0)
			{
				if (context.ContextItem == null)
				{
					throw DynamicError.contextUndefined();
				}
				arg1 = (AnyType) context.ContextItem;
			}
			else
			{
				// get arg
                var i = cargs.GetEnumerator();
                i.MoveNext();
                arg1 = (ResultSequence)i.Current;
			}

			if (arg1.empty())
			{
				return new XSAnyURI("");
			}

			NodeType an = (NodeType) arg1.first();

			QName name = an.node_name();

			string sname = "";
			if (name != null)
			{
				sname = name.@namespace();
			}

			return new XSAnyURI(sname);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnNamespaceUri))
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