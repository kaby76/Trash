using System;
using System.Text.RegularExpressions;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver - bug 262765 - initial API and implementation
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	public abstract class AbstractRegExFunction : Function
	{
		protected internal const string validflags = "smix";

		public AbstractRegExFunction(QName name, int arity) : base(name, arity)
		{
		}

		public AbstractRegExFunction(QName name, int min_arity, int max_arity) : base(name, min_arity, max_arity)
		{
		}

		protected internal static bool matches(string pattern, string flags, string src)
		{
			bool fnd = false;
			if (pattern.IndexOf("-[", StringComparison.Ordinal) != -1)
			{
				pattern = pattern.Replace("\\-\\[", "&&[^");
			}
			var m = compileAndExecute(pattern, flags, src);
			while (m.Count > 0)
			{
				fnd = true;
			}
			return fnd;
		}

		protected internal static MatchCollection regex(string pattern, string flags, string src)
		{
            var matcher = compileAndExecute(pattern, flags, src);
			return matcher;
		}

		private static MatchCollection compileAndExecute(string pattern, string flags, string src)
        {
            RegexOptions flag = default;
            //RegexOptions flag = Pattern.UNIX_LINES;
			//if (!string.ReferenceEquals(flags, null))
			//{
			//	if (flags.IndexOf("m", StringComparison.Ordinal) >= 0)
			//	{
			//		flag = flag | Pattern.MULTILINE;
			//	}
			//	if (flags.IndexOf("s", StringComparison.Ordinal) >= 0)
			//	{
			//		flag = flag | Pattern.DOTALL;
			//	}
			//	if (flags.IndexOf("i", StringComparison.Ordinal) >= 0)
			//	{
			//		flag = flag | Pattern.CASE_INSENSITIVE;
			//	}

			//	if (flags.IndexOf("x", StringComparison.Ordinal) >= 0)
			//	{
			//		flag = flag | Pattern.COMMENTS;
			//	}
			//}

			Regex p = new Regex(pattern, flag);
            MatchCollection matches = p.Matches(src);
            return matches;
        }

	}

}