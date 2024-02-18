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
///     David Carver - STAR - bug 262765 - renamed to correct function name. 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 285319 - fix UTF-8 escaping, and fix arity bug
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
	/// Usage: fn:escape-html-uri($uri-part as xs:string?, $escape-reserved as xs:boolean)
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
	/// The set of characters that are escaped depends on the setting of the boolean
	/// argument $escape-reserved.
	/// </para>
	/// 
	/// <para>
	/// If $uri-part is the empty sequence, returns the zero-length string.
	/// </para>
	/// 
	/// <para>
	/// If $escape-reserved is true, all characters are escaped other than the lower
	/// case letters a-z, the upper case letters A-Z, the digits 0-9, the PERCENT
	/// SIGN "%" and the NUMBER SIGN "#", as well as certain other characters:
	/// specifically, HYPHEN-MINUS ("-"), LOW LINE ("_"), FULL STOP ".", EXCLAMATION
	/// MARK "!", TILDE "~", ASTERISK "*", APOSTROPHE "'", LEFT PARENTHESIS "(", and
	/// RIGHT PARENTHESIS ")".
	/// </para>
	/// 
	/// <para>
	/// If $escape-reserved is false, additional characters are added to the list of
	/// characters that are not escaped. These are the following: SEMICOLON ";",
	/// SOLIDUS "/", QUESTION MARK "?", COLON ":", COMMERCIAL AT "@", AMPERSAND "&",
	/// EQUALS SIGN "=", PLUS SIGN "+", DOLLAR SIGN "$", COMMA ",", LEFT SQUARE
	/// BRACKET "[" and RIGHT SQUARE BRACKET "]".
	/// </para>
	/// 
	/// <para>
	/// To ensure that escaped URIs can be compared using string comparison
	/// functions, this function must always generate hexadecimal values using the
	/// upper-case letters A-F.
	/// </para>
	/// 
	/// <para>
	/// Generally, $escape-reserved should be set to true when escaping a string that
	/// is to form a single part of a URI, and to false when escaping an entire URI
	/// or URI reference.
	/// </para>
	/// 
	/// <para>
	/// Since this function does not escape the PERCENT SIGN "%" and this character
	/// is not allowed in a URI, users wishing to convert character strings, such as
	/// file names, that include "%" to a URI should manually escape "%" by replacing
	/// it with "%25".
	/// </para>
	/// </summary>
	public class FnEscapeHTMLUri : AbstractURIFunction
	{
		/// <summary>
		/// Constructor for FnEscape-html-Uri.
		/// </summary>
		public FnEscapeHTMLUri() : base(new QName("escape-html-uri"), 1)
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
			return escape_uri(args, false, false);
		}
	}

}