using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 David Carver, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver - STAR - bug 285321 - initial api and implementation 
///     Jesper Steen Moeller - bug 285319 - fix UTF-8 escaping
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// <para>
	/// Function to apply URI escaping rules.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:encode-for-uri($uri-part as xs:string?)
	/// as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class applies the URI escaping rules (with one exception), to the string
	/// supplied as $uri-part, which typically represents all or part of a URI. The
	/// effect of the function is to escape a set of identified characters in the
	/// string. Each such character is replaced in the string by an escape sequence,
	/// which is formed by encoding the character as a sequence of octets in UTF-8,
	/// and then representing each of these octets in the form %HH, where HH is the
	/// hexadecimal representation of the octet.
	/// </para>
	/// 
	/// <para>
	/// If $uri-part is the empty sequence, returns the zero-length string.
	/// </para>
	/// 
	/// 
	/// <para>
	/// To ensure that escaped URIs can be compared using string comparison
	/// functions, this function must always generate hexadecimal values using the
	/// upper-case letters A-F.
	/// </para>
	/// </summary>
	public class FnEncodeForURI : AbstractURIFunction
	{
		/// <summary>
		/// Constructor for FnEscape-for-Uri.
		/// </summary>
		public FnEncodeForURI() : base(new QName("encode-for-uri"), 1, 1)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the arguments after application of the URI
		///         escaping rules. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
		{
			return escape_uri(args, true, true);
		}

	}

}