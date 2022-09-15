using System;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: XNumber.java 469368 2006-10-31 04:41:36Z minchau $
 */
namespace org.apache.xpath.objects
{
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathContext = org.apache.xpath.XPathContext;
	using XPathVisitor = org.apache.xpath.XPathVisitor;

	/// <summary>
	/// This class represents an XPath number, and is capable of
	/// converting the number to other types, such as a string.
	/// @xsl.usage general
	/// </summary>
	[Serializable]
	public class XNumber : XObject
	{
		internal new const long serialVersionUID = -2720400709619020193L;

	  /// <summary>
	  /// Value of the XNumber object.
	  ///  @serial         
	  /// </summary>
	  internal double m_val;

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="d"> Value of the object </param>
	  public XNumber(double d) : base()
	  {

		m_val = d;
	  }

	  /// <summary>
	  /// Construct a XNodeSet object.
	  /// </summary>
	  /// <param name="num"> Value of the object </param>
	  public XNumber(Number num) : base()
	  {


		m_val = (double)num;
		Object = num;
	  }

	  /// <summary>
	  /// Tell that this is a CLASS_NUMBER.
	  /// </summary>
	  /// <returns> node type CLASS_NUMBER  </returns>
	  public override int Type
	  {
		  get
		  {
			return CLASS_NUMBER;
		  }
	  }

	  /// <summary>
	  /// Given a request type, return the equivalent string.
	  /// For diagnostic purposes.
	  /// </summary>
	  /// <returns> type string "#NUMBER"  </returns>
	  public override string TypeString
	  {
		  get
		  {
			return "#NUMBER";
		  }
	  }

	  /// <summary>
	  /// Cast result object to a number.
	  /// </summary>
	  /// <returns> the value of the XNumber object </returns>
	  public override double num()
	  {
		return m_val;
	  }

	  /// <summary>
	  /// Evaluate expression to a number.
	  /// </summary>
	  /// <returns> 0.0
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public double num(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override double num(XPathContext xctxt)
	  {

		return m_val;
	  }

	  /// <summary>
	  /// Cast result object to a boolean.
	  /// </summary>
	  /// <returns> false if the value is NaN or equal to 0.0 </returns>
	  public override bool @bool()
	  {
		return (double.IsNaN(m_val) || (m_val == 0.0)) ? false : true;
	  }

	//  /**
	//   * Cast result object to a string.
	//   *
	//   * @return "NaN" if the number is NaN, Infinity or -Infinity if
	//   * the number is infinite or the string value of the number.
	//   */
	//  private static final int PRECISION = 16;
	//  public String str()
	//  {
	//
	//    if (Double.isNaN(m_val))
	//    {
	//      return "NaN";
	//    }
	//    else if (Double.isInfinite(m_val))
	//    {
	//      if (m_val > 0)
	//        return "Infinity";
	//      else
	//        return "-Infinity";
	//    }
	//
	//    long longVal = (long)m_val;
	//    if ((double)longVal == m_val)
	//      return Long.toString(longVal);
	//
	//
	//    String s = Double.toString(m_val);
	//    int len = s.length();
	//
	//    if (s.charAt(len - 2) == '.' && s.charAt(len - 1) == '0')
	//    {
	//      return s.substring(0, len - 2);
	//    }
	//
	//    int exp = 0;
	//    int e = s.indexOf('E');
	//    if (e != -1)
	//    {
	//      exp = Integer.parseInt(s.substring(e + 1));
	//      s = s.substring(0,e);
	//      len = e;
	//    }
	//
	//    // Calculate Significant Digits:
	//    // look from start of string for first digit
	//    // look from end for last digit
	//    // significant digits = end - start + (0 or 1 depending on decimal location)
	//
	//    int decimalPos = -1;
	//    int start = (s.charAt(0) == '-') ? 1 : 0;
	//    findStart: for( ; start < len; start++ )
	//    {
	//      switch (s.charAt(start))
	//      {
	//      case '0':
	//        break;
	//      case '.':
	//        decimalPos = start;
	//        break;
	//      default:
	//        break findStart;
	//      }
	//    }
	//    int end = s.length() - 1;
	//    findEnd: for( ; end > start; end-- )
	//    {
	//      switch (s.charAt(end))
	//      {
	//      case '0':
	//        break;
	//      case '.':
	//        decimalPos = end;
	//        break;
	//      default:
	//        break findEnd;
	//      }
	//    }
	//
	//    int sigDig = end - start;
	//
	//    // clarify decimal location if it has not yet been found
	//    if (decimalPos == -1)
	//      decimalPos = s.indexOf('.');
	//
	//    // if decimal is not between start and end, add one to sigDig
	//    if (decimalPos < start || decimalPos > end)
	//      ++sigDig;
	//
	//    // reduce significant digits to PRECISION if necessary
	//    if (sigDig > PRECISION)
	//    {
	//      // re-scale BigDecimal in order to get significant digits = PRECISION
	//      BigDecimal num = new BigDecimal(s);
	//      int newScale = num.scale() - (sigDig - PRECISION);
	//      if (newScale < 0)
	//        newScale = 0;
	//      s = num.setScale(newScale, BigDecimal.ROUND_HALF_UP).toString();
	//
	//      // remove trailing '0's; keep track of decimalPos
	//      int truncatePoint = s.length();
	//      while (s.charAt(--truncatePoint) == '0')
	//        ;
	//
	//      if (s.charAt(truncatePoint) == '.')
	//      {
	//        decimalPos = truncatePoint;
	//      }
	//      else
	//      {
	//        decimalPos = s.indexOf('.');
	//        truncatePoint += 1;
	//      }
	//
	//      s = s.substring(0, truncatePoint);
	//      len = s.length();
	//    }
	//
	//    // Account for exponent by adding zeros as needed 
	//    // and moving the decimal place
	//
	//    if (exp == 0)
	//       return s;
	//
	//    start = 0;
	//    String sign;
	//    if (s.charAt(0) == '-')
	//    {
	//      sign = "-";
	//      start++;
	//    }
	//    else
	//      sign = "";
	//
	//    String wholePart = s.substring(start, decimalPos);
	//    String decimalPart = s.substring(decimalPos + 1);
	//
	//    // get the number of digits right of the decimal
	//    int decimalLen = decimalPart.length();
	//
	//    if (exp >= decimalLen)
	//      return sign + wholePart + decimalPart + zeros(exp - decimalLen);
	//
	//    if (exp > 0)
	//      return sign + wholePart + decimalPart.substring(0, exp) + "."
	//             + decimalPart.substring(exp);
	//
	//    return sign + "0." + zeros(-1 - exp) + wholePart + decimalPart;
	//  }

	  /// <summary>
	  /// Cast result object to a string.
	  /// </summary>
	  /// <returns> "NaN" if the number is NaN, Infinity or -Infinity if
	  /// the number is infinite or the string value of the number. </returns>
	  public override string str()
	  {

		if (double.IsNaN(m_val))
		{
		  return "NaN";
		}
		else if (double.IsInfinity(m_val))
		{
		  if (m_val > 0)
		  {
			return "Infinity";
		  }
		  else
		  {
			return "-Infinity";
		  }
		}

		double num = m_val;
		string s = Convert.ToString(num);
		int len = s.Length;

		if (s[len - 2] == '.' && s[len - 1] == '0')
		{
		  s = s.Substring(0, len - 2);

		  if (s.Equals("-0"))
		  {
			return "0";
		  }

		  return s;
		}

		int e = s.IndexOf('E');

		if (e < 0)
		{
		  if (s[len - 1] == '0')
		  {
			return s.Substring(0, len - 1);
		  }
		  else
		  {
			return s;
		  }
		}

		int exp = int.Parse(s.Substring(e + 1));
		string sign;

		if (s[0] == '-')
		{
		  sign = "-";
		  s = s.Substring(1);

		  --e;
		}
		else
		{
		  sign = "";
		}

		int nDigits = e - 2;

		if (exp >= nDigits)
		{
		  return sign + s.Substring(0, 1) + s.Substring(2, e - 2) + zeros(exp - nDigits);
		}

		// Eliminate trailing 0's - bugzilla 14241
		while (s[e-1] == '0')
		{
		  e--;
		}

		if (exp > 0)
		{
		  return sign + s.Substring(0, 1) + s.Substring(2, exp) + "." + s.Substring(2 + exp, e - (2 + exp));
		}

		return sign + "0." + zeros(-1 - exp) + s.Substring(0, 1) + s.Substring(2, e - 2);
	  }


	  /// <summary>
	  /// Return a string of '0' of the given length
	  /// 
	  /// </summary>
	  /// <param name="n"> Length of the string to be returned
	  /// </param>
	  /// <returns> a string of '0' with the given length </returns>
	  private static string zeros(int n)
	  {
		if (n < 1)
		{
		  return "";
		}

		char[] buf = new char[n];

		for (int i = 0; i < n; i++)
		{
		  buf[i] = '0';
		}

		return new string(buf);
	  }

	  /// <summary>
	  /// Return a java object that's closest to the representation
	  /// that should be handed to an extension.
	  /// </summary>
	  /// <returns> The value of this XNumber as a Double object </returns>
	  public override object @object()
	  {
		if (null == m_obj)
		{
		  Object = new double?(m_val);
		}
		return m_obj;
	  }

	  /// <summary>
	  /// Tell if two objects are functionally equal.
	  /// </summary>
	  /// <param name="obj2"> Object to compare this to
	  /// </param>
	  /// <returns> true if the two objects are equal 
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
	  public override bool Equals(XObject obj2)
	  {

		// In order to handle the 'all' semantics of 
		// nodeset comparisons, we always call the 
		// nodeset function.
		int t = obj2.Type;
		try
		{
			if (t == XObject.CLASS_NODESET)
			{
			  return obj2.Equals(this);
			}
			else if (t == XObject.CLASS_BOOLEAN)
			{
			  return obj2.@bool() == @bool();
			}
			else
			{
			   return m_val == obj2.num();
			}
		}
		catch (javax.xml.transform.TransformerException te)
		{
		  throw new org.apache.xml.utils.WrappedRuntimeException(te);
		}
	  }

	  /// <summary>
	  /// Tell if this expression returns a stable number that will not change during 
	  /// iterations within the expression.  This is used to determine if a proximity 
	  /// position predicate can indicate that no more searching has to occur.
	  /// 
	  /// </summary>
	  /// <returns> true if the expression represents a stable number. </returns>
	  public override bool StableNumber
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <seealso cref="org.apache.xpath.XPathVisitable.callVisitors(ExpressionOwner, XPathVisitor)"/>
	  public override void callVisitors(ExpressionOwner owner, XPathVisitor visitor)
	  {
		  visitor.visitNumberLiteral(owner, this);
	  }


	}

}