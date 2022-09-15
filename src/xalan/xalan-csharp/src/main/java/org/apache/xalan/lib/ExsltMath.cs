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
 * $Id: ExsltMath.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{
	using NodeSet = org.apache.xpath.NodeSet;

	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// This class contains EXSLT math extension functions.
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:math="http://exslt.org/math"
	/// </pre>
	/// 
	/// The documentation for each function has been copied from the relevant
	/// EXSLT Implementer page.
	/// </summary>
	/// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"
	/// 
	/// @xsl.usage general/>
	public class ExsltMath : ExsltBase
	{
	  // Constants
	  private static string PI = "3.1415926535897932384626433832795028841971693993751";
	  private static string E = "2.71828182845904523536028747135266249775724709369996";
	  private static string SQRRT2 = "1.41421356237309504880168872420969807856967187537694";
	  private static string LN2 = "0.69314718055994530941723212145817656807550013436025";
	  private static string LN10 = "2.302585092994046";
	  private static string LOG2E = "1.4426950408889633";
	  private static string SQRT1_2 = "0.7071067811865476";

	  /// <summary>
	  /// The math:max function returns the maximum value of the nodes passed as the argument. 
	  /// The maximum value is defined as follows. The node set passed as an argument is sorted 
	  /// in descending order as it would be by xsl:sort with a data type of number. The maximum 
	  /// is the result of converting the string value of the first node in this sorted list to 
	  /// a number using the number function. 
	  /// <para>
	  /// If the node set is empty, or if the result of converting the string values of any of the 
	  /// nodes to a number is NaN, then NaN is returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nl"> The NodeList for the node-set to be evaluated.
	  /// </param>
	  /// <returns> the maximum value found, NaN if any node cannot be converted to a number.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static double max(NodeList nl)
	  {
		if (nl == null || nl.getLength() == 0)
		{
		  return Double.NaN;
		}

		double m = - double.MaxValue;
		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node n = nl.item(i);
		  double d = toNumber(n);
		  if (double.IsNaN(d))
		  {
			return Double.NaN;
		  }
		  else if (d > m)
		  {
			m = d;
		  }
		}

		return m;
	  }

	  /// <summary>
	  /// The math:min function returns the minimum value of the nodes passed as the argument. 
	  /// The minimum value is defined as follows. The node set passed as an argument is sorted 
	  /// in ascending order as it would be by xsl:sort with a data type of number. The minimum 
	  /// is the result of converting the string value of the first node in this sorted list to 
	  /// a number using the number function. 
	  /// <para>
	  /// If the node set is empty, or if the result of converting the string values of any of 
	  /// the nodes to a number is NaN, then NaN is returned.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nl"> The NodeList for the node-set to be evaluated.
	  /// </param>
	  /// <returns> the minimum value found, NaN if any node cannot be converted to a number.
	  /// </returns>
	  /// <seealso cref="<a href="http://www.exslt.org/">EXSLT</a>"/>
	  public static double min(NodeList nl)
	  {
		if (nl == null || nl.getLength() == 0)
		{
		  return Double.NaN;
		}

		double m = double.MaxValue;
		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node n = nl.item(i);
		  double d = toNumber(n);
		  if (double.IsNaN(d))
		  {
			return Double.NaN;
		  }
		  else if (d < m)
		  {
			m = d;
		  }
		}

		return m;
	  }

	  /// <summary>
	  /// The math:highest function returns the nodes in the node set whose value is the maximum 
	  /// value for the node set. The maximum value for the node set is the same as the value as 
	  /// calculated by math:max. A node has this maximum value if the result of converting its 
	  /// string value to a number as if by the number function is equal to the maximum value, 
	  /// where the equality comparison is defined as a numerical comparison using the = operator.
	  /// <para>
	  /// If any of the nodes in the node set has a non-numeric value, the math:max function will 
	  /// return NaN. The definition numeric comparisons entails that NaN != NaN. Therefore if any 
	  /// of the nodes in the node set has a non-numeric value, math:highest will return an empty 
	  /// node set. 
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nl"> The NodeList for the node-set to be evaluated.
	  /// </param>
	  /// <returns> node-set with nodes containing the maximum value found, an empty node-set
	  /// if any node cannot be converted to a number. </returns>
	  public static NodeList highest(NodeList nl)
	  {
		double maxValue = max(nl);

		NodeSet highNodes = new NodeSet();
		highNodes.ShouldCacheNodes = true;

		if (double.IsNaN(maxValue))
		{
		  return highNodes; // empty Nodeset
		}

		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node n = nl.item(i);
		  double d = toNumber(n);
		  if (d == maxValue)
		  {
			highNodes.addElement(n);
		  }
		}
		return highNodes;
	  }

	  /// <summary>
	  /// The math:lowest function returns the nodes in the node set whose value is the minimum value 
	  /// for the node set. The minimum value for the node set is the same as the value as calculated 
	  /// by math:min. A node has this minimum value if the result of converting its string value to 
	  /// a number as if by the number function is equal to the minimum value, where the equality 
	  /// comparison is defined as a numerical comparison using the = operator.
	  /// <para>
	  /// If any of the nodes in the node set has a non-numeric value, the math:min function will return 
	  /// NaN. The definition numeric comparisons entails that NaN != NaN. Therefore if any of the nodes 
	  /// in the node set has a non-numeric value, math:lowest will return an empty node set.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="nl"> The NodeList for the node-set to be evaluated.
	  /// </param>
	  /// <returns> node-set with nodes containing the minimum value found, an empty node-set
	  /// if any node cannot be converted to a number.
	  ///  </returns>
	  public static NodeList lowest(NodeList nl)
	  {
		double minValue = min(nl);

		NodeSet lowNodes = new NodeSet();
		lowNodes.ShouldCacheNodes = true;

		if (double.IsNaN(minValue))
		{
		  return lowNodes; // empty Nodeset
		}

		for (int i = 0; i < nl.getLength(); i++)
		{
		  Node n = nl.item(i);
		  double d = toNumber(n);
		  if (d == minValue)
		  {
			lowNodes.addElement(n);
		  }
		}
		return lowNodes;
	  }

	  /// <summary>
	  /// The math:abs function returns the absolute value of a number.
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The absolute value of the number </returns>
	   public static double abs(double num)
	   {
		 return Math.Abs(num);
	   }

	  /// <summary>
	  /// The math:acos function returns the arccosine value of a number.
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The arccosine value of the number </returns>
	   public static double acos(double num)
	   {
		 return Math.Acos(num);
	   }

	  /// <summary>
	  /// The math:asin function returns the arcsine value of a number. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The arcsine value of the number </returns>
	   public static double asin(double num)
	   {
		 return Math.Asin(num);
	   }

	  /// <summary>
	  /// The math:atan function returns the arctangent value of a number. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The arctangent value of the number </returns>
	   public static double atan(double num)
	   {
		 return Math.Atan(num);
	   }

	  /// <summary>
	  /// The math:atan2 function returns the angle ( in radians ) from the X axis to a point (y,x). 
	  /// </summary>
	  /// <param name="num1"> The X axis value </param>
	  /// <param name="num2"> The Y axis value </param>
	  /// <returns> The angle (in radians) from the X axis to a point (y,x) </returns>
	   public static double atan2(double num1, double num2)
	   {
		 return Math.Atan2(num1, num2);
	   }

	  /// <summary>
	  /// The math:cos function returns cosine of the passed argument. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The cosine value of the number </returns>
	   public static double cos(double num)
	   {
		 return Math.Cos(num);
	   }

	  /// <summary>
	  /// The math:exp function returns e (the base of natural logarithms) raised to a power. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The value of e raised to the given power </returns>
	   public static double exp(double num)
	   {
		 return Math.Exp(num);
	   }

	  /// <summary>
	  /// The math:log function returns the natural logarithm of a number. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The natural logarithm of the number </returns>
	   public static double log(double num)
	   {
		 return Math.Log(num);
	   }

	  /// <summary>
	  /// The math:power function returns the value of a base expression taken to a specified power. 
	  /// </summary>
	  /// <param name="num1"> The base </param>
	  /// <param name="num2"> The power </param>
	  /// <returns> The value of the base expression taken to the specified power </returns>
	   public static double power(double num1, double num2)
	   {
		 return Math.Pow(num1, num2);
	   }

	  /// <summary>
	  /// The math:random function returns a random number from 0 to 1. 
	  /// </summary>
	  /// <returns> A random double from 0 to 1 </returns>
	   public static double random()
	   {
		 return MathHelper.NextDouble;
	   }

	  /// <summary>
	  /// The math:sin function returns the sine of the number. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The sine value of the number </returns>
	   public static double sin(double num)
	   {
		 return Math.Sin(num);
	   }

	  /// <summary>
	  /// The math:sqrt function returns the square root of a number. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The square root of the number </returns>
	   public static double sqrt(double num)
	   {
		 return Math.Sqrt(num);
	   }

	  /// <summary>
	  /// The math:tan function returns the tangent of the number passed as an argument. 
	  /// </summary>
	  /// <param name="num"> A number </param>
	  /// <returns> The tangent value of the number </returns>
	   public static double tan(double num)
	   {
		 return Math.Tan(num);
	   }

	  /// <summary>
	  /// The math:constant function returns the specified constant to a set precision. 
	  /// The possible constants are:
	  /// <pre>
	  ///  PI
	  ///  E
	  ///  SQRRT2
	  ///  LN2
	  ///  LN10
	  ///  LOG2E
	  ///  SQRT1_2
	  /// </pre> </summary>
	  /// <param name="name"> The name of the constant </param>
	  /// <param name="precision"> The precision </param>
	  /// <returns> The value of the specified constant to the given precision </returns>
	   public static double constant(string name, double precision)
	   {
		 string value = null;
		 if (name.Equals("PI"))
		 {
		   value = PI;
		 }
		 else if (name.Equals("E"))
		 {
		   value = E;
		 }
		 else if (name.Equals("SQRRT2"))
		 {
		   value = SQRRT2;
		 }
		 else if (name.Equals("LN2"))
		 {
		   value = LN2;
		 }
		 else if (name.Equals("LN10"))
		 {
		   value = LN10;
		 }
		 else if (name.Equals("LOG2E"))
		 {
		   value = LOG2E;
		 }
		 else if (name.Equals("SQRT1_2"))
		 {
		   value = SQRT1_2;
		 }

		 if (!string.ReferenceEquals(value, null))
		 {
		   int bits = (int)(new double?(precision));

		   if (bits <= value.Length)
		   {
			 value = value.Substring(0, bits);
		   }

		   return (Convert.ToDouble(value));
		 }
		 else
		 {
		   return Double.NaN;
		 }

	   }

	}

}