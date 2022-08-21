using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

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
///     David Carver - STAR - bug 262765 - renamed to correct function name. 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 285319 - fix UTF-8 escaping
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	public abstract class AbstractURIFunction : Function
	{
        internal static ArrayList _expected_args = null;

		protected internal static bool needs_escape(sbyte x, bool escape_delimiters, bool escape_space)
		{

			// These are identified as "unreserved" by [RFC 3986]: 
			if ('A' <= x && x <= 'Z')
			{
				return false;
			}
			if ('a' <= x && x <= 'z')
			{
				return false;
			}
			if ('0' <= x && x <= '9')
			{
				return false;
			}

			switch ((char)x)
			{
			// These are identified as "unreserved" by [RFC 3986]: 
			case '-':
			case '_':
			case '.':
			case '~':
				return false;

			// These are URI/IRI delimiters 	
			case '(':
			case ')':
			case '\'':
			case '*':
			case '!':
			case '#':
			case '%':
			case ';':
			case '/':
			case '?':
			case ':':
			case '@':
			case '&':
			case '=':
			case '+':
			case '$':
			case ',':
			case '[':
			case ']':
				return escape_delimiters;

			case ' ':
				return escape_space;

				// The rest should always be escaped: < > " space | ^ - and all the UTF-8 bytes
			default:
				return true;
			}
		}

		/// <summary>
		/// Apply the URI escaping rules to the arguments.
		/// </summary>
		/// <param name="args">
		///            have the URI escaping rules applied to them. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of applying the URI escaping rules to the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence escape_uri(java.util.Collection args, boolean escape) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence escape_uri(ICollection args, bool escape)
		{
			return escape_uri(args, escape, true);
		}

		/// <summary>
		/// Apply the URI escaping rules to the arguments.
		/// </summary>
		/// <param name="args">
		///            have the URI escaping rules applied to them. </param>
		/// <param name="escape_space"> TODO </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of applying the URI escaping rules to the arguments. </returns>
		//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence escape_uri(java.util.Collection args, boolean escape_delimiters, boolean escape_space) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence escape_uri(ICollection args, bool escape_delimiters, bool escape_space)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			IEnumerator argi = cargs.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
            argi.MoveNext();
            ResultSequence arg1 = (ResultSequence) argi.Current;

			if (arg1.empty())
			{
				return new XSString("");
			}

			AnyType aat = (AnyType) arg1.item(0);
			string str = aat.StringValue;

            var encoding = Encoding.UTF8;
			byte[] bytes = new byte[0];
			Array.Resize(ref bytes, encoding.GetByteCount(str));
            encoding.GetBytes(str, 0, str.Length, bytes, 0);
            StringBuilder sb = new StringBuilder();

			for (int i = 0; i < bytes.Length; i++)
			{
				byte x = bytes[i];

				if (needs_escape((sbyte)x, escape_delimiters, escape_space))
				{
					sb.Append("%");
					sb.Append((x & 0xFF).ToString("x").ToUpper());
				}
				else
				{
					sb.Append((char)x);
				}
			}

			return new XSString(sb.ToString());
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(AbstractURIFunction))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
				}
        
				return _expected_args;
			}
		}

		public AbstractURIFunction(QName name, int arity) : base(name, arity)
		{
		}

		public AbstractURIFunction(QName name, int min_arity, int max_arity) : base(name, min_arity, max_arity)
		{
		}

	}

}