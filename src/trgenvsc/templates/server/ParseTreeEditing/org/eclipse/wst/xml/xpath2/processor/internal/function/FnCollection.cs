using System;
using System.Collections;
using java.net;

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
	using URI = java.net.URI;

    using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using Document = org.w3c.dom.Document;

	/// <summary>
	/// Summary: This function takes an xs:string as argument and returns a sequence
	/// of nodes obtained by interpreting $arg as an xs:anyURI and resolving it
	/// according to the mapping specified in Available collections described in
	/// Section C.2 Dynamic Context Components. If Available collections provides a
	/// mapping from this string to a sequence of nodes, the function returns that
	/// sequence. If Available collections maps the string to an empty sequence, 
	/// then the function returns an empty sequence. If Available collections
	/// provides no mapping for the string, an error is raised [err:FODC0004]. If
	/// $arg is not specified, the function returns the sequence of the nodes in the
	/// default collection in the dynamic context. See Section C.2 Dynamic Context
	/// ComponentsXP. If the value of the default collection is undefined an error
	/// is raised [err:FODC0002].
	/// 
	/// If the $arg is a relative xs:anyURI, it is resolved against the value of the
	/// base-URI property from the static context. If $arg is not a valid xs:anyURI,
	/// an error is raised [err:FODC0004].
	/// 
	/// If $arg is the empty sequence, the function behaves as if it had been called
	/// without an argument. See above.
	/// 
	/// By default, this function is ·stable·. This means that repeated calls on the
	/// function with the same argument will return the same result. However, for
	/// performance reasons, implementations may provide a user option to evaluate
	/// the function without a guarantee of stability. The manner in which any such
	/// option is provided is ·implementation-defined·. If the user has not selected
	/// such an option, a call to this function must either return a stable result or
	/// must raise an error: [err:FODC0003].
	/// </summary>
	public class FnCollection : Function
	{
		private static ArrayList _expected_args = null;

		public const string DEFAULT_COLLECTION_URI = "http://www.w3.org/2005/xpath-functions/collection/default";

		/// <summary>
		/// Constructor for FnDoc.
		/// </summary>
		public FnCollection() : base(new QName("collection"), 0, 1)
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
			return collection(args, ec);
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
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence collection(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence collection(ICollection args, EvaluationContext ec)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			// get args
			IEnumerator argiter = cargs.GetEnumerator();
			ResultSequence arg1 = null;

			string uri = DEFAULT_COLLECTION_URI;
			if (argiter.MoveNext())
            {
                arg1 = (ResultSequence) argiter.Current;
				uri = ((XSString) arg1.first()).value();
			}

			try
			{
				new URI(uri);
			}
			catch
			{
				throw DynamicError.doc_not_found(null);
			}

			if (uri.IndexOf(":", StringComparison.Ordinal) < 0)
			{
				throw DynamicError.invalidCollectionArgument();
			}


			URI resolved = ec.DynamicContext.resolveUri(uri);
			if (resolved == null)
			{
				throw DynamicError.invalid_doc(null);
			}

			ResultSequence rs = getCollection(uri, ec);
			if (rs.empty())
			{
				throw DynamicError.doc_not_found(null);
			}

			return rs;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnCollection))
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

		private static ResultSequence getCollection(string uri, EvaluationContext ec)
		{
			ResultBuffer rs = new ResultBuffer();
			var collectionMap = ec.DynamicContext.Collections;
			IList docList = (IList) collectionMap[uri];
			for (int i = 0; i < docList.Count; i++)
			{
				Document doc = (Document) docList[i];
				rs.add(new DocType(doc, ec.StaticContext.TypeModel));
			}
			return rs.Sequence;

		}
	}

}