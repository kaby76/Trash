using System;
using System.Text;

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
 * $Id: MethodResolver.java 1225263 2011-12-28 18:36:36Z mrglavas $
 */
namespace org.apache.xalan.extensions
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMNodeIterator = org.apache.xml.dtm.@ref.DTMNodeIterator;
	using XObject = org.apache.xpath.objects.XObject;
	using XRTreeFrag = org.apache.xpath.objects.XRTreeFrag;
	using XString = org.apache.xpath.objects.XString;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;
	using NodeIterator = org.w3c.dom.traversal.NodeIterator;

	/// <summary>
	/// Utility class to help resolve method overloading with Xalan XSLT 
	/// argument types.
	/// </summary>
	public class MethodResolver
	{

	  /// <summary>
	  /// Specifies a search for static methods only.
	  /// </summary>
	  public const int STATIC_ONLY = 1;

	  /// <summary>
	  /// Specifies a search for instance methods only.
	  /// </summary>
	  public const int INSTANCE_ONLY = 2;

	  /// <summary>
	  /// Specifies a search for both static and instance methods.
	  /// </summary>
	  public const int STATIC_AND_INSTANCE = 3;

	  /// <summary>
	  /// Specifies a Dynamic method search.  If the method being
	  /// evaluated is a static method, all arguments are used.
	  /// Otherwise, it is an instance method and only arguments
	  /// beginning with the second argument are used.
	  /// </summary>
	  public const int DYNAMIC = 4;

	  /// <summary>
	  /// Given a class, figure out the resolution of 
	  /// the Java Constructor from the XSLT argument types, and perform the 
	  /// conversion of the arguments. </summary>
	  /// <param name="classObj"> the Class of the object to be constructed. </param>
	  /// <param name="argsIn"> An array of XSLT/XPath arguments. </param>
	  /// <param name="argsOut"> An array of the exact size as argsIn, which will be 
	  /// populated with converted arguments if a suitable method is found. </param>
	  /// <returns> A constructor that will work with the argsOut array. </returns>
	  /// <exception cref="TransformerException"> may be thrown for Xalan conversion
	  /// exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Constructor getConstructor(Class classObj, Object[] argsIn, Object[][] argsOut, ExpressionContext exprContext) throws NoSuchMethodException, SecurityException, javax.xml.transform.TransformerException
	  public static Constructor getConstructor(Type classObj, object[] argsIn, object[][] argsOut, ExpressionContext exprContext)
	  {
		Constructor bestConstructor = null;
		Type[] bestParamTypes = null;
		Constructor[] constructors = classObj.GetConstructors();
		int nMethods = constructors.Length;
		int bestScore = int.MaxValue;
		int bestScoreCount = 0;
		for (int i = 0; i < nMethods; i++)
		{
		  Constructor ctor = constructors[i];
		  Type[] paramTypes = ctor.ParameterTypes;
		  int numberMethodParams = paramTypes.Length;
		  int paramStart = 0;
		  bool isFirstExpressionContext = false;
		  int scoreStart;
		  // System.out.println("numberMethodParams: "+numberMethodParams);
		  // System.out.println("argsIn.length: "+argsIn.length);
		  // System.out.println("exprContext: "+exprContext);
		  if (numberMethodParams == (argsIn.Length + 1))
		  {
			Type javaClass = paramTypes[0];
			// System.out.println("first javaClass: "+javaClass.getName());
			if (javaClass.IsSubclassOf(typeof(ExpressionContext)))
			{
			  isFirstExpressionContext = true;
			  scoreStart = 0;
			  paramStart++;
			  // System.out.println("Incrementing paramStart: "+paramStart);
			}
			else
			{
			  continue;
			}
		  }
		  else
		  {
			  scoreStart = 1000;
		  }

		  if (argsIn.Length == (numberMethodParams - paramStart))
		  {
			// then we have our candidate.
			int score = scoreMatch(paramTypes, paramStart, argsIn, scoreStart);
			// System.out.println("score: "+score);
			if (-1 == score)
			{
			  continue;
			}
			if (score < bestScore)
			{
			  // System.out.println("Assigning best ctor: "+ctor);
			  bestConstructor = ctor;
			  bestParamTypes = paramTypes;
			  bestScore = score;
			  bestScoreCount = 1;
			}
			else if (score == bestScore)
			{
			  bestScoreCount++;
			}
		  }
		}

		if (null == bestConstructor)
		{
		  throw new NoSuchMethodException(errString("function", "constructor", classObj, "", 0, argsIn));
		}
		/// <summary>
		///* This is commented out until we can do a better object -> object scoring 
		/// else if (bestScoreCount > 1)
		///  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_MORE_MATCH_CONSTRUCTOR, new Object[]{classObj.getName()})); //"More than one best match for constructor for "
		///                                                               + classObj.getName());
		/// **
		/// </summary>
		else
		{
		  convertParams(argsIn, argsOut, bestParamTypes, exprContext);
		}

		return bestConstructor;
	  }


	  /// <summary>
	  /// Given the name of a method, figure out the resolution of 
	  /// the Java Method from the XSLT argument types, and perform the 
	  /// conversion of the arguments. </summary>
	  /// <param name="classObj"> The Class of the object that should have the method. </param>
	  /// <param name="name"> The name of the method to be invoked. </param>
	  /// <param name="argsIn"> An array of XSLT/XPath arguments. </param>
	  /// <param name="argsOut"> An array of the exact size as argsIn, which will be 
	  /// populated with converted arguments if a suitable method is found. </param>
	  /// <returns> A method that will work with the argsOut array. </returns>
	  /// <exception cref="TransformerException"> may be thrown for Xalan conversion
	  /// exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Method getMethod(Class classObj, String name, Object[] argsIn, Object[][] argsOut, ExpressionContext exprContext, int searchMethod) throws NoSuchMethodException, SecurityException, javax.xml.transform.TransformerException
	  public static Method getMethod(Type classObj, string name, object[] argsIn, object[][] argsOut, ExpressionContext exprContext, int searchMethod)
	  {
		// System.out.println("---> Looking for method: "+name);
		// System.out.println("---> classObj: "+classObj);
		if (name.IndexOf("-", StringComparison.Ordinal) > 0)
		{
		  name = replaceDash(name);
		}
		Method bestMethod = null;
		Type[] bestParamTypes = null;
		Method[] methods = classObj.GetMethods();
		int nMethods = methods.Length;
		int bestScore = int.MaxValue;
		int bestScoreCount = 0;
		bool isStatic;
		for (int i = 0; i < nMethods; i++)
		{
		  Method method = methods[i];
		  // System.out.println("looking at method: "+method);
		  int xsltParamStart = 0;
		  if (method.Name.Equals(name))
		  {
			isStatic = Modifier.isStatic(method.Modifiers);
			switch (searchMethod)
			{
			  case STATIC_ONLY:
				if (!isStatic)
				{
				  continue;
				}
				break;

			  case INSTANCE_ONLY:
				if (isStatic)
				{
				  continue;
				}
				break;

			  case STATIC_AND_INSTANCE:
				break;

			  case DYNAMIC:
				if (!isStatic)
				{
				  xsltParamStart = 1;
				}
			break;
			}
			int javaParamStart = 0;
			Type[] paramTypes = method.ParameterTypes;
			int numberMethodParams = paramTypes.Length;
			bool isFirstExpressionContext = false;
			int scoreStart;
			// System.out.println("numberMethodParams: "+numberMethodParams);
			// System.out.println("argsIn.length: "+argsIn.length);
			// System.out.println("exprContext: "+exprContext);
			int argsLen = (null != argsIn) ? argsIn.Length : 0;
			if (numberMethodParams == (argsLen - xsltParamStart + 1))
			{
			  Type javaClass = paramTypes[0];
			  if (javaClass.IsSubclassOf(typeof(ExpressionContext)))
			  {
				isFirstExpressionContext = true;
				scoreStart = 0;
				javaParamStart++;
			  }
			  else
			  {
				continue;
			  }
			}
			else
			{
				scoreStart = 1000;
			}

			if ((argsLen - xsltParamStart) == (numberMethodParams - javaParamStart))
			{
			  // then we have our candidate.
			  int score = scoreMatch(paramTypes, javaParamStart, argsIn, scoreStart);
			  // System.out.println("score: "+score);
			  if (-1 == score)
			  {
				continue;
			  }
			  if (score < bestScore)
			  {
				// System.out.println("Assigning best method: "+method);
				bestMethod = method;
				bestParamTypes = paramTypes;
				bestScore = score;
				bestScoreCount = 1;
			  }
			  else if (score == bestScore)
			  {
				bestScoreCount++;
			  }
			}
		  }
		}

		if (null == bestMethod)
		{
		  throw new NoSuchMethodException(errString("function", "method", classObj, name, searchMethod, argsIn));
		}
		/// <summary>
		///* This is commented out until we can do a better object -> object scoring 
		/// else if (bestScoreCount > 1)
		///  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_MORE_MATCH_METHOD, new Object[]{name})); //"More than one best match for method " + name);
		/// **
		/// </summary>
		else
		{
		  convertParams(argsIn, argsOut, bestParamTypes, exprContext);
		}

		return bestMethod;
	  }

	  /// <summary>
	  /// To support EXSLT extensions, convert names with dash to allowable Java names: 
	  /// e.g., convert abc-xyz to abcXyz.
	  /// Note: dashes only appear in middle of an EXSLT function or element name.
	  /// </summary>
	  private static string replaceDash(string name)
	  {
		char dash = '-';
		StringBuilder buff = new StringBuilder("");
		for (int i = 0; i < name.Length; i++)
		{
		  if (name[i] == dash)
		  {
		  }
		  else if (i > 0 && name[i - 1] == dash)
		  {
			buff.Append(char.ToUpper(name[i]));
		  }
		  else
		  {
			buff.Append(name[i]);
		  }
		}
		return buff.ToString();
	  }

	  /// <summary>
	  /// Given the name of a method, figure out the resolution of 
	  /// the Java Method </summary>
	  /// <param name="classObj"> The Class of the object that should have the method. </param>
	  /// <param name="name"> The name of the method to be invoked. </param>
	  /// <returns> A method that will work to be called as an element. </returns>
	  /// <exception cref="TransformerException"> may be thrown for Xalan conversion
	  /// exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static Method getElementMethod(Class classObj, String name) throws NoSuchMethodException, SecurityException, javax.xml.transform.TransformerException
	  public static Method getElementMethod(Type classObj, string name)
	  {
		// System.out.println("---> Looking for element method: "+name);
		// System.out.println("---> classObj: "+classObj);
		Method bestMethod = null;
		Method[] methods = classObj.GetMethods();
		int nMethods = methods.Length;
		int bestScoreCount = 0;
		for (int i = 0; i < nMethods; i++)
		{
		  Method method = methods[i];
		  // System.out.println("looking at method: "+method);
		  if (method.Name.Equals(name))
		  {
			Type[] paramTypes = method.ParameterTypes;
			if ((paramTypes.Length == 2) && paramTypes[1].IsAssignableFrom(typeof(org.apache.xalan.templates.ElemExtensionCall)) && paramTypes[0].IsAssignableFrom(typeof(org.apache.xalan.extensions.XSLProcessorContext)))
			{
			  if (++bestScoreCount == 1)
			  {
				bestMethod = method;
			  }
			  else
			  {
				break;
			  }
			}
		  }
		}

		if (null == bestMethod)
		{
		  throw new NoSuchMethodException(errString("element", "method", classObj, name, 0, null));
		}
		else if (bestScoreCount > 1)
		{
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_MORE_MATCH_ELEMENT, new object[]{name})); //"More than one best match for element method " + name);
		}

		return bestMethod;
	  }


	  /// <summary>
	  /// Convert a set of parameters based on a set of paramTypes. </summary>
	  /// <param name="argsIn"> An array of XSLT/XPath arguments. </param>
	  /// <param name="argsOut"> An array of the exact size as argsIn, which will be 
	  /// populated with converted arguments. </param>
	  /// <param name="paramTypes"> An array of class objects, of the exact same 
	  /// size as argsIn and argsOut. </param>
	  /// <exception cref="TransformerException"> may be thrown for Xalan conversion
	  /// exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void convertParams(Object[] argsIn, Object[][] argsOut, Class[] paramTypes, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public static void convertParams(object[] argsIn, object[][] argsOut, Type[] paramTypes, ExpressionContext exprContext)
	  {
		// System.out.println("In convertParams");
		if (paramTypes == null)
		{
		  argsOut[0] = null;
		}
		else
		{
		  int nParams = paramTypes.Length;
		  argsOut[0] = new object[nParams];
		  int paramIndex = 0;
		  if ((nParams > 0) && paramTypes[0].IsSubclassOf(typeof(ExpressionContext)))
		  {
			argsOut[0][0] = exprContext;
			// System.out.println("Incrementing paramIndex in convertParams: "+paramIndex);
			paramIndex++;
		  }

		  if (argsIn != null)
		  {
			for (int i = argsIn.Length - nParams + paramIndex ; paramIndex < nParams; i++, paramIndex++)
			{
			  // System.out.println("paramTypes[i]: "+paramTypes[i]);
			  argsOut[0][paramIndex] = convert(argsIn[i], paramTypes[paramIndex]);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Simple class to hold information about allowed conversions 
	  /// and their relative scores, for use by the table below.
	  /// </summary>
	  internal class ConversionInfo
	  {
		internal ConversionInfo(Type cl, int score)
		{
		  m_class = cl;
		  m_score = score;
		}

		internal Type m_class; // Java class to convert to.
		internal int m_score; // Match score, closer to zero is more matched.
	  }

	  private const int SCOREBASE = 1;

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_UNKNOWN
	  /// (i.e. some unknown Java object) to allowed Java types.
	  /// </summary>
	  private static readonly ConversionInfo[] m_javaObjConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(Double.TYPE, 11),
		  new ConversionInfo(Float.TYPE, 12),
		  new ConversionInfo(Long.TYPE, 13),
		  new ConversionInfo(Integer.TYPE, 14),
		  new ConversionInfo(Short.TYPE, 15),
		  new ConversionInfo(Character.TYPE, 16),
		  new ConversionInfo(Byte.TYPE, 17),
		  new ConversionInfo(typeof(string), 18)
	  };

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_BOOLEAN
	  /// to allowed Java types.
	  /// </summary>
	  private static readonly ConversionInfo[] m_booleanConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(Boolean.TYPE, 0),
		  new ConversionInfo(typeof(Boolean), 1),
		  new ConversionInfo(typeof(object), 2),
		  new ConversionInfo(typeof(string), 3)
	  };

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_NUMBER
	  /// to allowed Java types.
	  /// </summary>
	  private static readonly ConversionInfo[] m_numberConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(Double.TYPE, 0),
		  new ConversionInfo(typeof(Double), 1),
		  new ConversionInfo(Float.TYPE, 3),
		  new ConversionInfo(Long.TYPE, 4),
		  new ConversionInfo(Integer.TYPE, 5),
		  new ConversionInfo(Short.TYPE, 6),
		  new ConversionInfo(Character.TYPE, 7),
		  new ConversionInfo(Byte.TYPE, 8),
		  new ConversionInfo(Boolean.TYPE, 9),
		  new ConversionInfo(typeof(string), 10),
		  new ConversionInfo(typeof(object), 11)
	  };

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_STRING
	  /// to allowed Java types.
	  /// </summary>
	  private static readonly ConversionInfo[] m_stringConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(typeof(string), 0),
		  new ConversionInfo(typeof(object), 1),
		  new ConversionInfo(Character.TYPE, 2),
		  new ConversionInfo(Double.TYPE, 3),
		  new ConversionInfo(Float.TYPE, 3),
		  new ConversionInfo(Long.TYPE, 3),
		  new ConversionInfo(Integer.TYPE, 3),
		  new ConversionInfo(Short.TYPE, 3),
		  new ConversionInfo(Byte.TYPE, 3),
		  new ConversionInfo(Boolean.TYPE, 4)
	  };

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_RTREEFRAG
	  /// to allowed Java types.
	  /// </summary>
	  private static readonly ConversionInfo[] m_rtfConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(typeof(NodeIterator), 0),
		  new ConversionInfo(typeof(NodeList), 1),
		  new ConversionInfo(typeof(Node), 2),
		  new ConversionInfo(typeof(string), 3),
		  new ConversionInfo(typeof(object), 5),
		  new ConversionInfo(Character.TYPE, 6),
		  new ConversionInfo(Double.TYPE, 7),
		  new ConversionInfo(Float.TYPE, 7),
		  new ConversionInfo(Long.TYPE, 7),
		  new ConversionInfo(Integer.TYPE, 7),
		  new ConversionInfo(Short.TYPE, 7),
		  new ConversionInfo(Byte.TYPE, 7),
		  new ConversionInfo(Boolean.TYPE, 8)
	  };

	  /// <summary>
	  /// Specification of conversions from XSLT type CLASS_NODESET
	  /// to allowed Java types.  (This is the same as for CLASS_RTREEFRAG)
	  /// </summary>
	  private static readonly ConversionInfo[] m_nodesetConversions = new ConversionInfo[]
	  {
		  new ConversionInfo(typeof(NodeIterator), 0),
		  new ConversionInfo(typeof(NodeList), 1),
		  new ConversionInfo(typeof(Node), 2),
		  new ConversionInfo(typeof(string), 3),
		  new ConversionInfo(typeof(object), 5),
		  new ConversionInfo(Character.TYPE, 6),
		  new ConversionInfo(Double.TYPE, 7),
		  new ConversionInfo(Float.TYPE, 7),
		  new ConversionInfo(Long.TYPE, 7),
		  new ConversionInfo(Integer.TYPE, 7),
		  new ConversionInfo(Short.TYPE, 7),
		  new ConversionInfo(Byte.TYPE, 7),
		  new ConversionInfo(Boolean.TYPE, 8)
	  };

	  /// <summary>
	  /// Order is significant in the list below, based on 
	  /// XObject.CLASS_XXX values.
	  /// </summary>
	  private static readonly ConversionInfo[][] m_conversions = new ConversionInfo[][] {m_javaObjConversions, m_booleanConversions, m_numberConversions, m_stringConversions, m_nodesetConversions, m_rtfConversions};

	  /// <summary>
	  /// Score the conversion of a set of XSLT arguments to a 
	  /// given set of Java parameters.
	  /// If any invocations of this function for a method with 
	  /// the same name return the same positive value, then a conflict 
	  /// has occured, and an error should be signaled. </summary>
	  /// <param name="javaParamTypes"> Must be filled with valid class names, and 
	  /// of the same length as xsltArgs. </param>
	  /// <param name="xsltArgs"> Must be filled with valid object instances, and 
	  /// of the same length as javeParamTypes. </param>
	  /// <returns> -1 for no allowed conversion, or a positive score 
	  /// that is closer to zero for more preferred, or further from 
	  /// zero for less preferred. </returns>
	  public static int scoreMatch(Type[] javaParamTypes, int javaParamsStart, object[] xsltArgs, int score)
	  {
		if ((xsltArgs == null) || (javaParamTypes == null))
		{
		  return score;
		}
		int nParams = xsltArgs.Length;
		for (int i = nParams - javaParamTypes.Length + javaParamsStart, javaParamTypesIndex = javaParamsStart; i < nParams; i++, javaParamTypesIndex++)
		{
		  object xsltObj = xsltArgs[i];
		  int xsltClassType = (xsltObj is XObject) ? ((XObject)xsltObj).Type : XObject.CLASS_UNKNOWN;
		  Type javaClass = javaParamTypes[javaParamTypesIndex];

		  // System.out.println("Checking xslt: "+xsltObj.getClass().getName()+
		  //                   " against java: "+javaClass.getName());

		  if (xsltClassType == XObject.CLASS_NULL)
		  {
			// In Xalan I have objects of CLASS_NULL, though I'm not 
			// sure they're used any more.  For now, do something funky.
			if (!javaClass.IsPrimitive)
			{
			  // Then assume that a null can be used, but give it a low score.
			  score += 10;
			  continue;
			}
			else
			{
			  return -1; // no match.
			}
		  }

		  ConversionInfo[] convInfo = m_conversions[xsltClassType];
		  int nConversions = convInfo.Length;
		  int k;
		  for (k = 0; k < nConversions; k++)
		  {
			ConversionInfo cinfo = convInfo[k];
			if (javaClass.IsAssignableFrom(cinfo.m_class))
			{
			  score += cinfo.m_score;
			  break; // from k loop
			}
		  }

		  if (k == nConversions)
		  {
			// If we get here, we haven't made a match on this parameter using 
			// the ConversionInfo array.  We now try to handle the object -> object
			// mapping which we can't handle through the array mechanism.  To do this,
			// we must determine the class of the argument passed from the stylesheet.

			// If we were passed a subclass of XObject, representing one of the actual
			// XSLT types, and we are here, we reject this extension method as a candidate
			// because a match should have been made using the ConversionInfo array.  If we 
			// were passed an XObject that encapsulates a non-XSLT type or we
			// were passed a non-XSLT type directly, we continue.

			// The current implementation (contributed by Kelly Campbell <camk@channelpoint.com>)
			// checks to see if we were passed an XObject from the XSLT stylesheet.  If not,
			// we use the class of the object that was passed and make sure that it will
			// map to the class type of the parameter in the extension function.
			// If we were passed an XObject, we attempt to get the class of the actual
			// object encapsulated inside the XObject.  If the encapsulated object is null,
			// we judge this method as a match but give it a low score.  
			// If the encapsulated object is not null, we use its type to determine
			// whether this java method is a valid match for this extension function call.
			// This approach eliminates the NullPointerException in the earlier implementation
			// that resulted from passing an XObject encapsulating the null java object.

			// TODO:  This needs to be improved to assign relative scores to subclasses,
			// etc. 

			if (XObject.CLASS_UNKNOWN == xsltClassType)
			{
			  Type realClass = null;

			  if (xsltObj is XObject)
			  {
				object realObj = ((XObject) xsltObj).@object();
				if (null != realObj)
				{
				  realClass = realObj.GetType();
				}
				else
				{
				  // do the same as if we were passed XObject.CLASS_NULL
				  score += 10;
				  continue;
				}
			  }
			  else
			  {
				realClass = xsltObj.GetType();
			  }

			  if (javaClass.IsAssignableFrom(realClass))
			  {
				score += 0; // TODO: To be assigned based on subclass "distance"
			  }
			  else
			  {
				return -1;
			  }
			}
			else
			{
			  return -1;
			}
		  }
		}
		return score;
	  }

	  /// <summary>
	  /// Convert the given XSLT object to an object of 
	  /// the given class. </summary>
	  /// <param name="xsltObj"> The XSLT object that needs conversion. </param>
	  /// <param name="javaClass"> The type of object to convert to.
	  /// @returns An object suitable for passing to the Method.invoke 
	  /// function in the args array, which may be null in some cases. </param>
	  /// <exception cref="TransformerException"> may be thrown for Xalan conversion
	  /// exceptions. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object convert(Object xsltObj, Class javaClass) throws javax.xml.transform.TransformerException
	  internal static object convert(object xsltObj, Type javaClass)
	  {
		if (xsltObj is XObject)
		{
		  XObject xobj = ((XObject)xsltObj);
		  int xsltClassType = xobj.Type;

		  switch (xsltClassType)
		  {
		  case XObject.CLASS_NULL:
			return null;

		  case XObject.CLASS_BOOLEAN:
		  {
			  if (javaClass == typeof(string))
			  {
				return xobj.str();
			  }
			  else
			  {
				return xobj.@bool() ? true : false;
			  }
		  }
			// break; Unreachable
			  goto case org.apache.xpath.objects.XObject.CLASS_NUMBER;
		  case XObject.CLASS_NUMBER:
		  {
			  if (javaClass == typeof(string))
			  {
				return xobj.str();
			  }
			  else if (javaClass == Boolean.TYPE)
			  {
				return xobj.@bool() ? true : false;
			  }
			  else
			  {
				return convertDoubleToNumber(xobj.num(), javaClass);
			  }
		  }
			// break; Unreachable

			  goto case org.apache.xpath.objects.XObject.CLASS_STRING;
		  case XObject.CLASS_STRING:
		  {
			  if ((javaClass == typeof(string)) || (javaClass == typeof(object)))
			  {
				return xobj.str();
			  }
			  else if (javaClass == Character.TYPE)
			  {
				string str = xobj.str();
				if (str.Length > 0)
				{
				  return new char?(str[0]);
				}
				else
				{
				  return null; // ??
				}
			  }
			  else if (javaClass == Boolean.TYPE)
			  {
				return xobj.@bool() ? true : false;
			  }
			  else
			  {
				return convertDoubleToNumber(xobj.num(), javaClass);
			  }
		  }
			// break; Unreachable

			  goto case org.apache.xpath.objects.XObject.CLASS_RTREEFRAG;
		  case XObject.CLASS_RTREEFRAG:
		  {
			  // GLP:  I don't see the reason for the isAssignableFrom call
			  //       instead of an == test as is used everywhere else.
			  //       Besides, if the javaClass is a subclass of NodeIterator
			  //       the condition will be true and we'll create a NodeIterator
			  //       which may not match the javaClass, causing a RuntimeException.
			  // if((NodeIterator.class.isAssignableFrom(javaClass)) ||
			  if ((javaClass == typeof(NodeIterator)) || (javaClass == typeof(object)))
			  {
				DTMIterator dtmIter = ((XRTreeFrag) xobj).asNodeIterator();
				return new DTMNodeIterator(dtmIter);
			  }
			  else if (javaClass == typeof(NodeList))
			  {
				return ((XRTreeFrag) xobj).convertToNodeset();
			  }
			  // Same comment as above
			  // else if(Node.class.isAssignableFrom(javaClass))
			  else if (javaClass == typeof(Node))
			  {
				DTMIterator iter = ((XRTreeFrag) xobj).asNodeIterator();
				int rootHandle = iter.nextNode();
				DTM dtm = iter.getDTM(rootHandle);
				return dtm.getNode(dtm.getFirstChild(rootHandle));
			  }
			  else if (javaClass == typeof(string))
			  {
				return xobj.str();
			  }
			  else if (javaClass == Boolean.TYPE)
			  {
				return xobj.@bool() ? true : false;
			  }
			  else if (javaClass.IsPrimitive)
			  {
				return convertDoubleToNumber(xobj.num(), javaClass);
			  }
			  else
			  {
				DTMIterator iter = ((XRTreeFrag) xobj).asNodeIterator();
				int rootHandle = iter.nextNode();
				DTM dtm = iter.getDTM(rootHandle);
				Node child = dtm.getNode(dtm.getFirstChild(rootHandle));

				if (javaClass.IsAssignableFrom(child.GetType()))
				{
				  return child;
				}
				else
				{
				  return null;
				}
			  }
		  }
			// break; Unreachable

			  goto case org.apache.xpath.objects.XObject.CLASS_NODESET;
		  case XObject.CLASS_NODESET:
		  {
			  // GLP:  I don't see the reason for the isAssignableFrom call
			  //       instead of an == test as is used everywhere else.
			  //       Besides, if the javaClass is a subclass of NodeIterator
			  //       the condition will be true and we'll create a NodeIterator
			  //       which may not match the javaClass, causing a RuntimeException.
			  // if((NodeIterator.class.isAssignableFrom(javaClass)) ||
			  if ((javaClass == typeof(NodeIterator)) || (javaClass == typeof(object)))
			  {
				return xobj.nodeset();
			  }
			  // Same comment as above
			  // else if(NodeList.class.isAssignableFrom(javaClass))
			  else if (javaClass == typeof(NodeList))
			  {
				return xobj.nodelist();
			  }
			  // Same comment as above
			  // else if(Node.class.isAssignableFrom(javaClass))
			  else if (javaClass == typeof(Node))
			  {
				// Xalan ensures that iter() always returns an
				// iterator positioned at the beginning.
				DTMIterator ni = xobj.iter();
				int handle = ni.nextNode();
				if (handle != org.apache.xml.dtm.DTM_Fields.NULL)
				{
				  return ni.getDTM(handle).getNode(handle); // may be null.
				}
				else
				{
				  return null;
				}
			  }
			  else if (javaClass == typeof(string))
			  {
				return xobj.str();
			  }
			  else if (javaClass == Boolean.TYPE)
			  {
				return xobj.@bool() ? true : false;
			  }
			  else if (javaClass.IsPrimitive)
			  {
				return convertDoubleToNumber(xobj.num(), javaClass);
			  }
			  else
			  {
				DTMIterator iter = xobj.iter();
				int childHandle = iter.nextNode();
				DTM dtm = iter.getDTM(childHandle);
				Node child = dtm.getNode(childHandle);
				if (javaClass.IsAssignableFrom(child.GetType()))
				{
				  return child;
				}
				else
				{
				  return null;
				}
			  }
		  }
			// break; Unreachable

			// No default:, fall-through on purpose
		break;
		  } // end switch
		  xsltObj = xobj.@object();

		} // end if if(xsltObj instanceof XObject)

		// At this point, we have a raw java object, not an XObject.
		if (null != xsltObj)
		{
		  if (javaClass == typeof(string))
		  {
			return xsltObj.ToString();
		  }
		  else if (javaClass.IsPrimitive)
		  {
			// Assume a number conversion
			XString xstr = new XString(xsltObj.ToString());
			double num = xstr.num();
			return convertDoubleToNumber(num, javaClass);
		  }
		  else if (javaClass == typeof(Type))
		  {
			return xsltObj.GetType();
		  }
		  else
		  {
			// Just pass the object directly, and hope for the best.
			return xsltObj;
		  }
		}
		else
		{
		  // Just pass the object directly, and hope for the best.
		  return xsltObj;
		}
	  }

	  /// <summary>
	  /// Do a standard conversion of a double to the specified type. </summary>
	  /// <param name="num"> The number to be converted. </param>
	  /// <param name="javaClass"> The class type to be converted to. </param>
	  /// <returns> An object specified by javaClass, or a Double instance. </returns>
	  internal static object convertDoubleToNumber(double num, Type javaClass)
	  {
		// In the code below, I don't check for NaN, etc., instead 
		// using the standard Java conversion, as I think we should 
		// specify.  See issue-runtime-errors.
		if ((javaClass == Double.TYPE) || (javaClass == typeof(Double)))
		{
		  return new double?(num);
		}
		else if (javaClass == Float.TYPE)
		{
		  return new float?(num);
		}
		else if (javaClass == Long.TYPE)
		{
		  // Use standard Java Narrowing Primitive Conversion
		  // See http://java.sun.com/docs/books/jls/html/5.doc.html#175672
		  return new long?((long)num);
		}
		else if (javaClass == Integer.TYPE)
		{
		  // Use standard Java Narrowing Primitive Conversion
		  // See http://java.sun.com/docs/books/jls/html/5.doc.html#175672
		  return new int?((int)num);
		}
		else if (javaClass == Short.TYPE)
		{
		  // Use standard Java Narrowing Primitive Conversion
		  // See http://java.sun.com/docs/books/jls/html/5.doc.html#175672
		  return new short?((short)num);
		}
		else if (javaClass == Character.TYPE)
		{
		  // Use standard Java Narrowing Primitive Conversion
		  // See http://java.sun.com/docs/books/jls/html/5.doc.html#175672
		  return new char?((char)num);
		}
		else if (javaClass == Byte.TYPE)
		{
		  // Use standard Java Narrowing Primitive Conversion
		  // See http://java.sun.com/docs/books/jls/html/5.doc.html#175672
		  return new sbyte?((sbyte)num);
		}
		else // Some other type of object
		{
		  return new double?(num);
		}
	  }


	  /// <summary>
	  /// Format the message for the NoSuchMethodException containing 
	  /// all the information about the method we're looking for.
	  /// </summary>
	  private static string errString(string callType, string searchType, Type classObj, string funcName, int searchMethod, object[] xsltArgs) // "method" or "constructor" -  "function" or "element"
	  {
		string resultString = "For extension " + callType + ", could not find " + searchType + " ";
		switch (searchMethod)
		{
		  case STATIC_ONLY:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return resultString + "static " + classObj.FullName + "." + funcName + "([ExpressionContext,] " + errArgs(xsltArgs, 0) + ").";

		  case INSTANCE_ONLY:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return resultString + classObj.FullName + "." + funcName + "([ExpressionContext,] " + errArgs(xsltArgs, 0) + ").";

		  case STATIC_AND_INSTANCE:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return resultString + classObj.FullName + "." + funcName + "([ExpressionContext,] " + errArgs(xsltArgs, 0) + ").\n" + "Checked both static and instance methods.";

		  case DYNAMIC:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			return resultString + "static " + classObj.FullName + "." + funcName + "([ExpressionContext, ]" + errArgs(xsltArgs, 0) + ") nor\n" + classObj + "." + funcName + "([ExpressionContext,] " + errArgs(xsltArgs, 1) + ").";

		  default:
			if (callType.Equals("function")) // must be a constructor
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  return resultString + classObj.FullName + "([ExpressionContext,] " + errArgs(xsltArgs, 0) + ").";
			}
			else // must be an element call
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  return resultString + classObj.FullName + "." + funcName + "(org.apache.xalan.extensions.XSLProcessorContext, " + "org.apache.xalan.templates.ElemExtensionCall).";
			}
		}

	  }


	  private static string errArgs(object[] xsltArgs, int startingArg)
	  {
		StringBuilder returnArgs = new StringBuilder();
		for (int i = startingArg; i < xsltArgs.Length; i++)
		{
		  if (i != startingArg)
		  {
			returnArgs.Append(", ");
		  }
		  if (xsltArgs[i] is XObject)
		  {
			returnArgs.Append(((XObject) xsltArgs[i]).TypeString);
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			returnArgs.Append(xsltArgs[i].GetType().FullName);
		  }
		}
		return returnArgs.ToString();
	  }

	}

}