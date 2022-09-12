using System;
using System.Collections;

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
 * $Id: ExtensionHandlerJavaPackage.java 1225574 2011-12-29 15:49:16Z mrglavas $
 */
namespace org.apache.xalan.extensions
{


	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using ExtensionEvent = org.apache.xalan.trace.ExtensionEvent;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Represents an extension namespace for XPath that handles java packages
	/// that may be fully or partially specified.
	/// It is recommended that the class URI be of one of the following forms:
	/// <pre>
	///   xalan://partial.class.name
	///   xalan://
	///   http://xml.apache.org/xalan/java (which is the same as xalan://)
	/// </pre>
	/// However, we do not enforce this.  If the class name contains a
	/// a /, we only use the part to the right of the rightmost slash.
	/// In addition, we ignore any "class:" prefix.
	/// Provides functions to test a function's existence and call a function.
	/// Also provides functions to test an element's existence and call an
	/// element.
	/// 
	/// @author <a href="mailto:garyp@firstech.com">Gary L Peskin</a>
	/// 
	/// @xsl.usage internal
	/// </summary>


	public class ExtensionHandlerJavaPackage : ExtensionHandlerJava
	{

	  /// <summary>
	  /// Construct a new extension namespace handler given all the information
	  /// needed. 
	  /// </summary>
	  /// <param name="namespaceUri"> the extension namespace URI that I'm implementing </param>
	  /// <param name="scriptLang">   language of code implementing the extension </param>
	  /// <param name="className">    the beginning of the class name of the class.  This
	  ///                     should be followed by a dot (.) </param>
	  public ExtensionHandlerJavaPackage(string namespaceUri, string scriptLang, string className) : base(namespaceUri, scriptLang, className)
	  {
	  }


	  /// <summary>
	  /// Tests whether a certain function name is known within this namespace.
	  /// Since this is for a package, we concatenate the package name used when
	  /// this handler was created and the function name specified in the argument.
	  /// There is
	  /// no information regarding the arguments to the function call or
	  /// whether the method implementing the function is a static method or
	  /// an instance method. </summary>
	  /// <param name="function"> name of the function being tested </param>
	  /// <returns> true if its known, false if not. </returns>

	  public override bool isFunctionAvailable(string function)
	  {
		try
		{
		  string fullName = m_className + function;
		  int lastDot = fullName.LastIndexOf('.');
		  if (lastDot >= 0)
		  {
			Type myClass = getClassForName(fullName.Substring(0, lastDot));
			Method[] methods = myClass.GetMethods();
			int nMethods = methods.Length;
			function = fullName.Substring(lastDot + 1);
			for (int i = 0; i < nMethods; i++)
			{
			  if (methods[i].Name.Equals(function))
			  {
				return true;
			  }
			}
		  }
		}
		catch (ClassNotFoundException)
		{
		}

		return false;
	  }


	  /// <summary>
	  /// Tests whether a certain element name is known within this namespace.
	  /// Looks for a method with the appropriate name and signature.
	  /// This method examines both static and instance methods. </summary>
	  /// <param name="element"> name of the element being tested </param>
	  /// <returns> true if its known, false if not. </returns>

	  public override bool isElementAvailable(string element)
	  {
		try
		{
		  string fullName = m_className + element;
		  int lastDot = fullName.LastIndexOf('.');
		  if (lastDot >= 0)
		  {
			Type myClass = getClassForName(fullName.Substring(0, lastDot));
			Method[] methods = myClass.GetMethods();
			int nMethods = methods.Length;
			element = fullName.Substring(lastDot + 1);
			for (int i = 0; i < nMethods; i++)
			{
			  if (methods[i].Name.Equals(element))
			  {
				Type[] paramTypes = methods[i].ParameterTypes;
				if ((paramTypes.Length == 2) && paramTypes[0].IsAssignableFrom(typeof(org.apache.xalan.extensions.XSLProcessorContext)) && paramTypes[1].IsAssignableFrom(typeof(org.apache.xalan.templates.ElemExtensionCall)))
				{
				  return true;
				}
			  }
			}
		  }
		}
		catch (ClassNotFoundException)
		{
		}

		return false;
	  }


	  /// <summary>
	  /// Process a call to a function in the package java namespace.
	  /// There are three possible types of calls:
	  /// <pre>
	  ///   Constructor:
	  ///     packagens:class.name.new(arg1, arg2, ...)
	  /// 
	  ///   Static method:
	  ///     packagens:class.name.method(arg1, arg2, ...)
	  /// 
	  ///   Instance method:
	  ///     packagens:method(obj, arg1, arg2, ...)
	  /// </pre>
	  /// We use the following rules to determine the type of call made:
	  /// <ol type="1">
	  /// <li>If the function name ends with a ".new", call the best constructor for
	  ///     class whose name is formed by concatenating the value specified on
	  ///     the namespace with the value specified in the function invocation
	  ///     before ".new".</li>
	  /// <li>If the function name contains a period, call the best static method "method"
	  ///     in the class whose name is formed by concatenating the value specified on
	  ///     the namespace with the value specified in the function invocation.</li>
	  /// <li>Otherwise, call the best instance method "method"
	  ///     in the class whose name is formed by concatenating the value specified on
	  ///     the namespace with the value specified in the function invocation.
	  ///     Note that a static method of the same
	  ///     name will <i>not</i> be called in the current implementation.  This
	  ///     module does not verify that the obj argument is a member of the
	  ///     package namespace.</li>
	  /// </ol>
	  /// </summary>
	  /// <param name="funcName"> Function name. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="methodKey"> A key that uniquely identifies this class and method call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed. </param>
	  /// <returns> the return value of the function evaluation.
	  /// </returns>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object callFunction(String funcName, java.util.Vector args, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(string funcName, ArrayList args, object methodKey, ExpressionContext exprContext)
	  {

		string className;
		string methodName;
		Type classObj;
		object targetObject;
		int lastDot = funcName.LastIndexOf('.');
		object[] methodArgs;
		object[][] convertedArgs;
		Type[] paramTypes;

		try
		{
		  TransformerImpl trans = (exprContext != null) ? (TransformerImpl)exprContext.XPathContext.OwnerObject : null;
		  if (funcName.EndsWith(".new", StringComparison.Ordinal))
		  { // Handle constructor call

			methodArgs = new object[args.Count];
			convertedArgs = new object[1][];
			for (int i = 0; i < methodArgs.Length; i++)
			{
			  methodArgs[i] = args[i];
			}

			Constructor c = (methodKey != null) ? (Constructor) getFromCache(methodKey, null, methodArgs) : null;

			if (c != null)
			{
			  try
			  {
				paramTypes = c.ParameterTypes;
				MethodResolver.convertParams(methodArgs, convertedArgs, paramTypes, exprContext);
				return c.newInstance(convertedArgs[0]);
			  }
			  catch (InvocationTargetException ite)
			  {
				throw ite;
			  }
			  catch (Exception)
			  {
				// Must not have been the right one
			  }
			}
			className = m_className + funcName.Substring(0, lastDot);
			try
			{
			  classObj = getClassForName(className);
			}
			catch (ClassNotFoundException e)
			{
			  throw new TransformerException(e);
			}
			c = MethodResolver.getConstructor(classObj, methodArgs, convertedArgs, exprContext);
			if (methodKey != null)
			{
			  putToCache(methodKey, null, methodArgs, c);
			}

			if (trans != null && trans.Debug)
			{
				trans.TraceManager.fireExtensionEvent(new ExtensionEvent(trans, c, convertedArgs[0]));
				object result;
				try
				{
					result = c.newInstance(convertedArgs[0]);
				}
				catch (Exception e)
				{
					throw e;
				}
				finally
				{
					trans.TraceManager.fireExtensionEndEvent(new ExtensionEvent(trans, c, convertedArgs[0]));
				}
				return result;
			}
			else
			{
				return c.newInstance(convertedArgs[0]);
			}
		  }

		  else if (-1 != lastDot)
		  { // Handle static method call

			methodArgs = new object[args.Count];
			convertedArgs = new object[1][];
			for (int i = 0; i < methodArgs.Length; i++)
			{
			  methodArgs[i] = args[i];
			}
			Method m = (methodKey != null) ? (Method) getFromCache(methodKey, null, methodArgs) : null;

			if (m != null && !trans.Debug)
			{
			  try
			  {
				paramTypes = m.ParameterTypes;
				MethodResolver.convertParams(methodArgs, convertedArgs, paramTypes, exprContext);
				return m.invoke(null, convertedArgs[0]);
			  }
			  catch (InvocationTargetException ite)
			  {
				throw ite;
			  }
			  catch (Exception)
			  {
				// Must not have been the right one
			  }
			}
			className = m_className + funcName.Substring(0, lastDot);
			methodName = funcName.Substring(lastDot + 1);
			try
			{
			  classObj = getClassForName(className);
			}
			catch (ClassNotFoundException e)
			{
			  throw new TransformerException(e);
			}
			m = MethodResolver.getMethod(classObj, methodName, methodArgs, convertedArgs, exprContext, MethodResolver.STATIC_ONLY);
			if (methodKey != null)
			{
			  putToCache(methodKey, null, methodArgs, m);
			}

			if (trans != null && trans.Debug)
			{
				trans.TraceManager.fireExtensionEvent(m, null, convertedArgs[0]);
				object result;
				try
				{
					result = m.invoke(null, convertedArgs[0]);
				}
				catch (Exception e)
				{
					throw e;
				}
				finally
				{
					trans.TraceManager.fireExtensionEndEvent(m, null, convertedArgs[0]);
				}
				return result;
			}
			else
			{
				return m.invoke(null, convertedArgs[0]);
			}
		  }

		  else
		  { // Handle instance method call

			if (args.Count < 1)
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_INSTANCE_MTHD_CALL_REQUIRES, new object[]{funcName})); //"Instance method call to method " + funcName
										//+ " requires an Object instance as first argument");
			}
			targetObject = args[0];
			if (targetObject is XObject) // Next level down for XObjects
			{
			  targetObject = ((XObject) targetObject).@object();
			}
			methodArgs = new object[args.Count - 1];
			convertedArgs = new object[1][];
			for (int i = 0; i < methodArgs.Length; i++)
			{
			  methodArgs[i] = args[i + 1];
			}
			Method m = (methodKey != null) ? (Method) getFromCache(methodKey, targetObject, methodArgs) : null;

			if (m != null)
			{
			  try
			  {
				paramTypes = m.ParameterTypes;
				MethodResolver.convertParams(methodArgs, convertedArgs, paramTypes, exprContext);
				return m.invoke(targetObject, convertedArgs[0]);
			  }
			  catch (InvocationTargetException ite)
			  {
				throw ite;
			  }
			  catch (Exception)
			  {
				// Must not have been the right one
			  }
			}
			classObj = targetObject.GetType();
			m = MethodResolver.getMethod(classObj, funcName, methodArgs, convertedArgs, exprContext, MethodResolver.INSTANCE_ONLY);
			if (methodKey != null)
			{
			  putToCache(methodKey, targetObject, methodArgs, m);
			}

			if (trans != null && trans.Debug)
			{
				trans.TraceManager.fireExtensionEvent(m, targetObject, convertedArgs[0]);
				object result;
				try
				{
					result = m.invoke(targetObject, convertedArgs[0]);
				}
				catch (Exception e)
				{
					throw e;
				}
				finally
				{
					trans.TraceManager.fireExtensionEndEvent(m, targetObject, convertedArgs[0]);
				}
				return result;
			}
			else
			{
				return m.invoke(targetObject, convertedArgs[0]);
			}
		  }
		}
		catch (InvocationTargetException ite)
		{
		  Exception resultException = ite;
		  Exception targetException = ite.TargetException;

		  if (targetException is TransformerException)
		  {
			throw ((TransformerException)targetException);
		  }
		  else if (targetException != null)
		  {
			resultException = targetException;
		  }

		  throw new TransformerException(resultException);
		}
		catch (Exception e)
		{
		  // e.printStackTrace();
		  throw new TransformerException(e);
		}
	  }

	  /// <summary>
	  /// Process a call to an XPath extension function
	  /// </summary>
	  /// <param name="extFunction"> The XPath extension function </param>
	  /// <param name="args"> The arguments of the function call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed. </param>
	  /// <returns> the return value of the function evaluation. </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object callFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector args, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(FuncExtFunction extFunction, ArrayList args, ExpressionContext exprContext)
	  {
		return callFunction(extFunction.FunctionName, args, extFunction.MethodKey, exprContext);
	  }

	  /// <summary>
	  /// Process a call to this extension namespace via an element. As a side
	  /// effect, the results are sent to the TransformerImpl's result tree.
	  /// For this namespace, only static element methods are currently supported.
	  /// If instance methods are needed, please let us know your requirements. </summary>
	  /// <param name="localPart">      Element name's local part. </param>
	  /// <param name="element">        The extension element being processed. </param>
	  /// <param name="transformer">      Handle to TransformerImpl. </param>
	  /// <param name="stylesheetTree"> The compiled stylesheet tree. </param>
	  /// <param name="methodKey">      A key that uniquely identifies this element call. </param>
	  /// <exception cref="IOException">           if loading trouble </exception>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processElement(String localPart, org.apache.xalan.templates.ElemTemplateElement element, org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xalan.templates.Stylesheet stylesheetTree, Object methodKey) throws javax.xml.transform.TransformerException, java.io.IOException
	  public override void processElement(string localPart, ElemTemplateElement element, TransformerImpl transformer, Stylesheet stylesheetTree, object methodKey)
	  {
		object result = null;
		Type classObj;

		Method m = (Method) getFromCache(methodKey, null, null);
		if (null == m)
		{
		  try
		  {
			string fullName = m_className + localPart;
			int lastDot = fullName.LastIndexOf('.');
			if (lastDot < 0)
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_ELEMENT_NAME, new object[]{fullName})); //"Invalid element name specified " + fullName);
			}
			try
			{
			  classObj = getClassForName(fullName.Substring(0, lastDot));
			}
			catch (ClassNotFoundException e)
			{
			  throw new TransformerException(e);
			}
			localPart = fullName.Substring(lastDot + 1);
			m = MethodResolver.getElementMethod(classObj, localPart);
			if (!Modifier.isStatic(m.Modifiers))
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_ELEMENT_NAME_METHOD_STATIC, new object[]{fullName})); //"Element name method must be static " + fullName);
			}
		  }
		  catch (Exception e)
		  {
			// e.printStackTrace ();
			throw new TransformerException(e);
		  }
		  putToCache(methodKey, null, null, m);
		}

		XSLProcessorContext xpc = new XSLProcessorContext(transformer, stylesheetTree);

		try
		{
		  if (transformer.Debug)
		  {
			  transformer.TraceManager.fireExtensionEvent(m, null, new object[] {xpc, element});
			try
			{
				result = m.invoke(null, new object[] {xpc, element});
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				transformer.TraceManager.fireExtensionEndEvent(m, null, new object[] {xpc, element});
			}
		  }
		  else
		  {
			result = m.invoke(null, new object[] {xpc, element});
		  }
		}
		catch (InvocationTargetException ite)
		{
		  Exception resultException = ite;
		  Exception targetException = ite.TargetException;

		  if (targetException is TransformerException)
		  {
			throw ((TransformerException)targetException);
		  }
		  else if (targetException != null)
		  {
			resultException = targetException;
		  }

		  throw new TransformerException(resultException);
		}
		catch (Exception e)
		{
		  // e.printStackTrace ();
		  throw new TransformerException(e);
		}

		if (result != null)
		{
		  xpc.outputToResultTree(stylesheetTree, result);
		}

	  }

	}

}