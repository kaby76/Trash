using System.Text;
using java.text;

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
///    David Carver - initial API and implementation
///    Jesper Steen Moller - bug 283404 - fixed locale
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{
	using DecimalFormat = java.text.DecimalFormat;
	using DecimalFormatSymbols = java.text.DecimalFormatSymbols;
	using Locale = java.util.Locale;

	/// <summary>
	/// This is an XPath specific implementation of DecimalFormat to handle
	/// some of the xpath specific formatting requirements.   Specifically
	/// it allows for E# to be represented to indicate that the exponent value
	/// is optional.  Otherwise all existing DecimalFormat patterns are handled
	/// as is.
	/// @author dcarver </summary>
	/// <seealso cref= 1.1
	///  </seealso>
	public class XPathDecimalFormat : DecimalFormat
	{

		/// 
		private const long serialVersionUID = -8229885955864187400L;
		private const string NEG_INFINITY = "-INF";
		private const string POS_INFINITY = "INF";

		public XPathDecimalFormat(string pattern) : base(pattern, new DecimalFormatSymbols(Locale.US))
		{
			// Xpath hardcodes this to US locale
		}

		/// <summary>
		/// Formats the string dropping a Zero Exponent Value if it exists. </summary>
		/// <param name="obj">
		/// @return </param>
		public virtual string xpathFormat(object obj)
		{
			return formatXPath(obj);
		}

		private string formatXPath(object obj)
		{
			string curPattern = toPattern();
			string newPattern = curPattern.Replace("E0", "");
			if (obj is float?)
			{
				return formatFloatValue(obj, curPattern, newPattern);
			}
			if (obj is double?)
			{
				return formatDoubleValue(obj, curPattern, newPattern);
			}
			return base.format(obj, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		private string formatDoubleValue(object obj, string curPattern, string newPattern)
		{
			double? doubleValue = (double?) obj;
			if (isDoubleNegativeInfinity(doubleValue))
			{
				return NEG_INFINITY;
			}
			if (isDoublePositiveInfinity(doubleValue))
			{
				return POS_INFINITY;
			}
			doubleXPathPattern(obj, curPattern, newPattern);
			return format(obj, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		private void doubleXPathPattern(object obj, string curPattern, string newPattern)
		{
			decimal doubValue = new decimal((((double) obj)));
            decimal.TryParse("-1E6", out decimal minValue);
            decimal.TryParse("1E6", out decimal maxValue);
			if (doubValue.CompareTo(minValue) > 0 && doubValue.CompareTo(maxValue) < 0)
			{
				applyPattern(newPattern);
			}
			else
			{ //if (doubValue.compareTo(minValue) < 0) {
				applyPattern(curPattern.Replace("0\\.#", "0.0"));
			}
		}

		private bool isDoublePositiveInfinity(double? doubleValue)
		{
			return doubleValue.Value == double.PositiveInfinity;
		}

		private bool isDoubleNegativeInfinity(double? doubleValue)
		{
			return doubleValue.Value == double.NegativeInfinity;
		}

		private string formatFloatValue(object obj, string curPattern, string newPattern)
		{
			float? floatValue = (float?) obj;
			if (isFloatNegInfinity(floatValue))
			{
				return NEG_INFINITY;
			}
			if (isFloatPosInfinity(floatValue))
			{
				return POS_INFINITY;
			}
			floatXPathPattern(curPattern, newPattern, floatValue);
			return format(obj, new StringBuilder(), new FieldPosition(0)).ToString();
		}

		private bool isFloatPosInfinity(float? floatValue)
		{
			return floatValue.Value == float.PositiveInfinity;
		}

		private bool isFloatNegInfinity(float? floatValue)
		{
			return floatValue.Value == float.NegativeInfinity;
		}

		private void floatXPathPattern(string curPattern, string newPattern, float? floatValue)
		{
			if (floatValue.Value > -1E6f && floatValue.Value < 1E6f)
			{

				applyPattern(newPattern);
			}
			else if (floatValue.Value <= -1E6f)
			{
				applyPattern(curPattern.Replace("0\\.#", "0.0"));
			}
		}


	}

}