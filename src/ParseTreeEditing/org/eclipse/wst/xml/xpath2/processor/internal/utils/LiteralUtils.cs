using System.Text;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Steen M�ller and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{

	/// <summary>
	/// String literal utilities
	/// 
	/// 
	/// </summary>
	public class LiteralUtils
	{

		/// <summary>
		/// Unquotes a quoted string, changing double quotes into single quotes as well.
		/// Examples (string delimited by > and <):
		///  >"A"< becomes >A< 
		///  >'B'< becomes >B< 
		///  >"A""B"< becomes >A"B<
		///  >"A""B"< becomes >A"B<
		///  >'A''''B'< becomes >A''B<
		///  >"A''''B"< becomes >A''''B< </summary>
		/// <param name="quotedString"> A quoted string possibly containing escaped quotes </param>
		/// <returns> unquoted and unescaped string </returns>
		public static string unquote(string quotedString)
		{
			int inputLength = quotedString.Length;
			char quoteChar = quotedString[0];
			if (quotedString.IndexOf(quoteChar, 1) == inputLength - 1)
			{
				// The trivial case where there's no quotes in the middle of the string.
				return quotedString.Substring(1, (inputLength - 1) - 1);
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < inputLength - 1; ++i)
			{
				char ch = quotedString[i];
				sb.Append(ch);
				if (ch == quoteChar)
				{
					++i; // Skip past second quote char (ensured by the lexer)
				}
			}
			return sb.ToString();
		}
	}

}