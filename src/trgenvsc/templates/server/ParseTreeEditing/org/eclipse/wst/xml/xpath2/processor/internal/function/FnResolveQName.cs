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
///     David Carver (STAR) - bug 288886 - add unit tests and fix fn:resolve-qname function
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Element = org.w3c.dom.Element;

	/// <summary>
	/// Returns an xs:QName value (that is, an expanded-QName) by taking an xs:string
	/// that has the lexical form of an xs:QName (a string in the form
	/// "prefix:local-name" or "local-name") and resolving it using the in-scope
	/// namespaces for a given element.
	/// </summary>
	public class FnResolveQName : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnResolveQName.
		/// </summary>
		public FnResolveQName() : base(new QName("resolve-QName"), 2)
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
			return resolve_QName(args, ec.StaticContext);
		}

		/// <summary>
		/// Resolve-QName operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="sc">
		///            Result of static context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:resolve-QName operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence resolve_QName(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.StaticContext sc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence resolve_QName(ICollection args, StaticContext sc)
		{

			//Collection cargs = Function.convert_arguments(args, expected_args());
			ICollection cargs = args;

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;

			if (arg1.empty())
			{
				return ResultBuffer.EMPTY;
			}

            argiter.MoveNext();
            ResultSequence arg2 = (ResultSequence) argiter.Current;

			string name = ((XSString) arg1.first()).value();

			QName qn = QName.parse_QName(name);

			if (qn == null)
			{
				throw DynamicError.lexical_error(null);
			}

			ElementType xselement = (ElementType) arg2.first();
			Element element = (Element) xselement.node_value();

			if (!string.ReferenceEquals(qn.prefix(), null))
			{
				string namespaceURI = element.lookupNamespaceURI(qn.prefix());

				if (string.ReferenceEquals(namespaceURI, null))
				{
					throw DynamicError.invalidPrefix();
				}
				qn.set_namespace(namespaceURI);
			}
			else
			{
				if (qn.local().Equals(element.LocalName) && element.isDefaultNamespace(element.NamespaceURI))
				{
					qn.set_namespace(element.NamespaceURI);
				}
			}


			return qn;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnResolveQName))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
					_expected_args.Add(new SeqType(new ElementType(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}