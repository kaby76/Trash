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
///     Jesper Steen Moller - bug 281159 - fix document loading and resolving URIs 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Document = org.w3c.dom.Document;

	/// <summary>
	/// Retrieves a document using an xs:anyURI supplied as an xs:string. If $uri is
	/// not a valid xs:anyURI, an error is raised [err:FODC0005]. If it is a relative
	/// URI Reference, it is resolved relative to the value of the base URI property
	/// from the static context. The resulting absolute URI Reference is cast to an
	/// xs:string. If the Available documents discussed in Section 2.1.2 Dynamic
	/// ContextXP provides a mapping from this string to a document node, the
	/// function returns that document node. If the Available documents maps the
	/// string to an empty sequence, then the function returns an empty sequence. If
	/// the Available documents provides no mapping for the string, an error is
	/// raised [err:FODC0005].
	/// </summary>
	public class FnDoc : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnDoc.
		/// </summary>
		public FnDoc() : base(new QName("doc"), 1)
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
			return doc(args, ec);
		}

		/// <summary>
		/// Doc operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="dc">
		///            Result of dynamic context operation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:doc operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence doc(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence doc(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
            argiter.MoveNext();
            ResultSequence arg1 = (ResultSequence) argiter.Current;

			if (arg1 == null || arg1.empty())
			{
				return ResultSequenceFactory.create_new();
			}

			string uri = ((XSString) arg1.item(0)).value();

			DynamicContext dc = ec.DynamicContext;
			var resolved = dc.resolveUri(uri);
			if (resolved == null)
			{
				throw DynamicError.invalid_doc(null);
			}

			Document doc = dc.getDocument(resolved);
			if (doc == null)
			{
				throw DynamicError.doc_not_found(null);
			}

			return new DocType(doc, ec.StaticContext.TypeModel);
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnDoc))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					SeqType arg = new SeqType(new XSString(), SeqType.OCC_QMARK);
					_expected_args.Add(arg);
				}
        
				return _expected_args;
			}
		}
	}

}