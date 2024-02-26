using System;
using System.Collections;
using java.net;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver - initial API and implementation from the PsychoPath XPath 2.0 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{
	using URI = java.net.URI;

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;

	/// <summary>
	/// The purpose of this function is to enable a relative URI to be resolved
	/// against an absolute URI. The first form of this function resolves $relative
	/// against the value of the base-uri property from the static context. If the
	/// base-uri property is not initialized in the static context an error is raised
	/// [err:FONS0005].
	/// 
	/// If $relative is a relative URI reference, it is resolved against $base, or
	/// the base-uri property from the static context, using an algorithm such as the
	/// ones described in [RFC 2396] or [RFC 3986], and the resulting absolute URI
	/// reference is returned. An error may be raised [err:FORG0009] in the
	/// resolution process. If $relative is an absolute URI reference, it is returned
	/// unchanged. If $relative or $base is not a valid xs:anyURI an error is raised
	/// [err:FORG0002]. If $relative is the empty sequence, the empty sequence is
	/// returned.
	/// </summary>
	public class FnResolveURI : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnBaseUri.
		/// </summary>
		public FnResolveURI() : base(new QName("resolve-uri"), 1, 2)
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
			return resolveURI(args, ec);
		}

		/// <summary>
		/// Resolve-URI operation.
		/// </summary>
		/// <param name="args">
		///            Result from the expressions evaluation. </param>
		/// <param name="d_context">
		///            Dynamic context </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of fn:resolve-uri operation. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence resolveURI(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence resolveURI(ICollection args, EvaluationContext ec)
		{
			if (ec.StaticContext.BaseUri == null)
			{
				throw DynamicError.noBaseURI();
			}

			ICollection cargs = args;
			IEnumerator argit = cargs.GetEnumerator();
            argit.MoveNext();
            ResultSequence relativeRS = (ResultSequence) argit.Current;
			ResultSequence baseUriRS = null;
			if (argit.MoveNext())
            {
                baseUriRS = (ResultSequence) argit.Current;
            }

			if (relativeRS.empty())
			{
				return ResultBuffer.EMPTY;
			}

			Item relativeURI = relativeRS.first();
			string resolvedURI = null;

			if (baseUriRS == null)
			{
				resolvedURI = resolveURI(ec.StaticContext.BaseUri.ToString(), relativeURI.StringValue);
			}
			else
			{
				Item baseURI = baseUriRS.first();
				resolvedURI = resolveURI(baseURI.StringValue, relativeURI.StringValue);
			}

			return new XSAnyURI(resolvedURI);
		}

		private static string resolveURI(string @base, string relative)
		{
			string resolved = null;
			try
			{
				URI baseURI = new URI(@base);
				resolved = baseURI.resolve(relative).ToString();
			}
			catch (Exception)
			{
				throw DynamicError.errorResolvingURI();
			}
			return resolved;
		}


		/// <summary>
		/// Obtain a list of expected arguments.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnResolveURI))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}