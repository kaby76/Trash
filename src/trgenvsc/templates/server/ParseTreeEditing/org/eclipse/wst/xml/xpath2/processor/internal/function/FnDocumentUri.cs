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
///     Mukul Gandhi - bug274731 - implementation of fn:document-uri function
///     Jesper Moller- bug 281159 - fix document loading and resolving URIs 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;

	/// <summary>
	/// Returns the value of the document-uri property for $arg as defined by the
	/// dm:document-uri accessor function defined in Section 6.1.2 AccessorsDM. If
	/// $arg is the empty sequence, the empty sequence is returned. Returns the empty
	/// sequence if the node is not a document node or if its document-uri property
	/// is a relative URI. Otherwise, returns an absolute URI expressed as an
	/// xs:string.
	/// </summary>
	public class FnDocumentUri : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnDocumentUri.
		/// </summary>
		public FnDocumentUri() : base(new QName("document-uri"), 1)
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
			return document_uri(args);
		}

		/// <summary>
		/// Document-Uri operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:document-uri operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence document_uri(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence document_uri(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());
            var i = cargs.GetEnumerator();
            i.MoveNext();
            ResultSequence arg1 = (ResultSequence) i.Current;

			if (arg1 == null || arg1.empty())
			{
			  return ResultBuffer.EMPTY;
			}

			NodeType nt = (NodeType) arg1.first();

			if (!(nt is DocType))
			{
			  return ResultBuffer.EMPTY;
			}

			DocType dt = (DocType) nt;
			string documentURI = dt.value().DocumentURI;

			if (!string.ReferenceEquals(documentURI, null))
			{
				XSAnyURI docUri = new XSAnyURI(documentURI);
				return docUri;
			}
			return ResultBuffer.EMPTY;
		}

		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnDocumentUri))
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