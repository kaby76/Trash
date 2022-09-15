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
 * $Id: ExtensionHandlerJavaClass.java 469672 2006-10-31 21:56:19Z minchau $
 */

namespace org.apache.xalan.extensions
{

	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using ExtensionEvent = org.apache.xalan.trace.ExtensionEvent;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Represents an extension namespace for XPath that handles java classes.
	/// It is recommended that the class URI be of the form:
	/// <pre>
	///   xalan://fully.qualified.class.name
	/// </pre>
	/// However, we do not enforce this.  If the class name contains a
	/// a /, we only use the part to the right of the rightmost slash.
	/// In addition, we ignore any "class:" prefix.
	/// Provides functions to test a function's existence and call a function.
	/// Also provides functions to test an element's existence and call an
	/// element.
	/// 
	/// @author <a href="mailto:garyp@firstech.com">Gary L Peskin</a>
	/// @xsl.usage internal
	/// </summary>

	public class ExtensionHandlerJavaClass : ExtensionHandlerJava
	{

	  private Type m_classObj = null;

	  /// <summary>
	  /// Provides a default Instance for use by elements that need to call 
	  /// an instance method.
	  /// </summary>

	  private object m_defaultInstance = null;


	  /// <summary>
	  /// Construct a new extension namespace handler given all the information
	  /// needed. </summary>
	  /// <param name="namespaceUri"> the extension namespace URI that I'm implementing </param>
	  /// <param name="scriptLang">   language of code implementing the extension </param>
	  /// <param name="className">    the fully qualified class name of the class </param>
	  public ExtensionHandlerJavaClass(string namespaceUri, string scriptLang, string className) : base(namespaceUri, scriptLang, className)
	  {
		try
		{
		  m_classObj = getClassForName(className);
		}
		catch (ClassNotFoundException)
		{
		  // For now, just let this go.  We'll catch it when we try to invoke a method.
		}
	  }


	  /// <summary>
	  /// Tests whether a certain function name is known within this namespace.
	  /// Simply looks for a method with the appropriate name.  There is
	  /// no information regarding the arguments to the function call or
	  /// whether the method implementing the function is a static method or
	  /// an instance method. </summary>
	  /// <param name="function"> name of the function being tested </param>
	  /// <returns> true if its known, false if not. </returns>

	  public override bool isFunctionAvailable(string function)
	  {
		System.Reflection.MethodInfo[] methods = m_classObj.GetMethods();
		int nMethods = methods.Length;
		for (int i = 0; i < nMethods; i++)
		{
		  if (methods[i].getName().Equals(function))
		  {
			return true;
		  }
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
		System.Reflection.MethodInfo[] methods = m_classObj.GetMethods();
		int nMethods = methods.Length;
		for (int i = 0; i < nMethods; i++)
		{
		  if (methods[i].getName().Equals(element))
		  {
			Type[] paramTypes = methods[i].getParameterTypes();
			if ((paramTypes.Length == 2) && paramTypes[0].IsAssignableFrom(typeof(org.apache.xalan.extensions.XSLProcessorContext)) && paramTypes[1].IsAssignableFrom(typeof(org.apache.xalan.templates.ElemExtensionCall)))
			{
			  return true;
			}
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Process a call to a function in the java class represented by
	  /// this <code>ExtensionHandlerJavaClass<code>.
	  /// There are three possible types of calls:
	  /// <pre>
	  ///   Constructor:
	  ///     classns:new(arg1, arg2, ...)
	  /// 
	  ///   Static method:
	  ///     classns:method(arg1, arg2, ...)
	  /// 
	  ///   Instance method:
	  ///     classns:method(obj, arg1, arg2, ...)
	  /// </pre>
	  /// We use the following rules to determine the type of call made:
	  /// <ol type="1">
	  /// <li>If the function name is "new", call the best constructor for
	  ///     class represented by the namespace URI</li>
	  /// <li>If the first argument to the function is of the class specified
	  ///     in the namespace or is a subclass of that class, look for the best
	  ///     method of the class specified in the namespace with the specified
	  ///     arguments.  Compare all static and instance methods with the correct
	  ///     method name.  For static methods, use all arguments in the compare.
	  ///     For instance methods, use all arguments after the first.</li>
	  /// <li>Otherwise, select the best static or instance method matching
	  ///     all of the arguments.  If the best method is an instance method,
	  ///     call the function using a default object, creating it if needed.</li>
	  /// </ol>
	  /// </summary>
	  /// <param name="funcName"> Function name. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="methodKey"> A key that uniquely identifies this class and method call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed. </param>
	  /// <returns> the return value of the function evaluation. </returns>
	  /// <exception cref="TransformerException"> </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object callFunction(String funcName, java.util.Vector args, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(string funcName, ArrayList args, object methodKey, ExpressionContext exprContext)
	  {

		object[] methodArgs;
		object[][] convertedArgs;
		Type[] paramTypes;

		try
		{
		  TransformerImpl trans = (exprContext != null) ? (TransformerImpl)exprContext.XPathContext.OwnerObject : null;
		  if (funcName.Equals("new"))
		  { // Handle constructor call

			methodArgs = new object[args.Count];
			convertedArgs = new object[1][];
			for (int i = 0; i < methodArgs.Length; i++)
			{
			  methodArgs[i] = args[i];
			}
			System.Reflection.ConstructorInfo c = null;
			if (methodKey != null)
			{
			  c = (System.Reflection.ConstructorInfo) getFromCache(methodKey, null, methodArgs);
			}

			if (c != null && !trans.Debug)
			{
			  try
			  {
				paramTypes = c.getParameterTypes();
				MethodResolver.convertParams(methodArgs, convertedArgs, paramTypes, exprContext);
				return c.Invoke(convertedArgs[0]);
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
			c = MethodResolver.getConstructor(m_classObj, methodArgs, convertedArgs, exprContext);
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
					result = c.Invoke(convertedArgs[0]);
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
				return c.Invoke(convertedArgs[0]);
			}
		  }

		  else
		  {

			int resolveType;
			object targetObject = null;
			methodArgs = new object[args.Count];
			convertedArgs = new object[1][];
			for (int i = 0; i < methodArgs.Length; i++)
			{
			  methodArgs[i] = args[i];
			}
			System.Reflection.MethodInfo m = null;
			if (methodKey != null)
			{
			  m = (System.Reflection.MethodInfo) getFromCache(methodKey, null, methodArgs);
			}

			if (m != null && !trans.Debug)
			{
			  try
			  {
				paramTypes = m.getParameterTypes();
				MethodResolver.convertParams(methodArgs, convertedArgs, paramTypes, exprContext);
				if (Modifier.isStatic(m.getModifiers()))
				{
				  return m.invoke(null, convertedArgs[0]);
				}
				else
				{
				  // This is tricky.  We get the actual number of target arguments (excluding any
				  //   ExpressionContext).  If we passed in the same number, we need the implied object.
				  int nTargetArgs = convertedArgs[0].Length;
				  if (paramTypes[0].IsAssignableFrom(typeof(ExpressionContext)))
				  {
					nTargetArgs--;
				  }
				  if (methodArgs.Length <= nTargetArgs)
				  {
					return m.invoke(m_defaultInstance, convertedArgs[0]);
				  }
				  else
				  {
					targetObject = methodArgs[0];

					if (targetObject is XObject)
					{
					  targetObject = ((XObject) targetObject).@object();
					}

					return m.invoke(targetObject, convertedArgs[0]);
				  }
				}
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

			if (args.Count > 0)
			{
			  targetObject = methodArgs[0];

			  if (targetObject is XObject)
			  {
				targetObject = ((XObject) targetObject).@object();
			  }

			  if (m_classObj.IsAssignableFrom(targetObject.GetType()))
			  {
				resolveType = MethodResolver.DYNAMIC;
			  }
			  else
			  {
				resolveType = MethodResolver.STATIC_AND_INSTANCE;
			  }
			}
			else
			{
			  targetObject = null;
			  resolveType = MethodResolver.STATIC_AND_INSTANCE;
			}

			m = MethodResolver.getMethod(m_classObj, funcName, methodArgs, convertedArgs, exprContext, resolveType);
			if (methodKey != null)
			{
			  putToCache(methodKey, null, methodArgs, m);
			}

			if (MethodResolver.DYNAMIC == resolveType)
			{ // First argument was object type
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
			else // First arg was not object.  See if we need the implied object.
			{
			  if (Modifier.isStatic(m.getModifiers()))
			  {
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
			  {
				if (null == m_defaultInstance)
				{
				  if (trans != null && trans.Debug)
				  {
					trans.TraceManager.fireExtensionEvent(new ExtensionEvent(trans, m_classObj));
					try
					{
						m_defaultInstance = System.Activator.CreateInstance(m_classObj);
					}
					catch (Exception e)
					{
						throw e;
					}
					finally
					{
						trans.TraceManager.fireExtensionEndEvent(new ExtensionEvent(trans, m_classObj));
					}
				  }
				  else
				  {
					  m_defaultInstance = System.Activator.CreateInstance(m_classObj);
				  }
				}
				if (trans != null && trans.Debug)
				{
				  trans.TraceManager.fireExtensionEvent(m, m_defaultInstance, convertedArgs[0]);
				  object result;
				  try
				  {
					result = m.invoke(m_defaultInstance, convertedArgs[0]);
				  }
				  catch (Exception e)
				  {
					throw e;
				  }
				  finally
				  {
					trans.TraceManager.fireExtensionEndEvent(m, m_defaultInstance, convertedArgs[0]);
				  }
				  return result;
				}
				else
				{
				  return m.invoke(m_defaultInstance, convertedArgs[0]);
				}
			  }
			}

		  }
		}
		catch (InvocationTargetException ite)
		{
		  Exception resultException = ite;
		  Exception targetException = ite.getTargetException();

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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object callFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector args, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(FuncExtFunction extFunction, ArrayList args, ExpressionContext exprContext)
	  {
		return callFunction(extFunction.FunctionName, args, extFunction.MethodKey, exprContext);
	  }

	  /// <summary>
	  /// Process a call to this extension namespace via an element. As a side
	  /// effect, the results are sent to the TransformerImpl's result tree. 
	  /// We invoke the static or instance method in the class represented by
	  /// by the namespace URI.  If we don't already have an instance of this class,
	  /// we create one upon the first call.
	  /// </summary>
	  /// <param name="localPart">      Element name's local part. </param>
	  /// <param name="element">        The extension element being processed. </param>
	  /// <param name="transformer">      Handle to TransformerImpl. </param>
	  /// <param name="stylesheetTree"> The compiled stylesheet tree. </param>
	  /// <param name="methodKey">      A key that uniquely identifies this element call. </param>
	  /// <exception cref="IOException">           if loading trouble </exception>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void processElement(String localPart, org.apache.xalan.templates.ElemTemplateElement element, org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xalan.templates.Stylesheet stylesheetTree, Object methodKey) throws TransformerException, java.io.IOException
	  public override void processElement(string localPart, ElemTemplateElement element, TransformerImpl transformer, Stylesheet stylesheetTree, object methodKey)
	  {
		object result = null;

		System.Reflection.MethodInfo m = (System.Reflection.MethodInfo) getFromCache(methodKey, null, null);
		if (null == m)
		{
		  try
		  {
			m = MethodResolver.getElementMethod(m_classObj, localPart);
			if ((null == m_defaultInstance) && !Modifier.isStatic(m.getModifiers()))
			{
			  if (transformer.Debug)
			  {
				transformer.TraceManager.fireExtensionEvent(new ExtensionEvent(transformer, m_classObj));
				try
				{
				  m_defaultInstance = System.Activator.CreateInstance(m_classObj);
				}
				catch (Exception e)
				{
				  throw e;
				}
				finally
				{
				  transformer.TraceManager.fireExtensionEndEvent(new ExtensionEvent(transformer, m_classObj));
				}
			  }
			  else
			  {
				m_defaultInstance = System.Activator.CreateInstance(m_classObj);
			  }
			}
		  }
		  catch (Exception e)
		  {
			// e.printStackTrace ();
			throw new TransformerException(e.Message, e);
		  }
		  putToCache(methodKey, null, null, m);
		}

		XSLProcessorContext xpc = new XSLProcessorContext(transformer, stylesheetTree);

		try
		{
		  if (transformer.Debug)
		  {
			transformer.TraceManager.fireExtensionEvent(m, m_defaultInstance, new object[] {xpc, element});
			try
			{
			  result = m.invoke(m_defaultInstance, new object[] {xpc, element});
			}
			catch (Exception e)
			{
			  throw e;
			}
			finally
			{
			  transformer.TraceManager.fireExtensionEndEvent(m, m_defaultInstance, new object[] {xpc, element});
			}
		  }
		  else
		  {
			result = m.invoke(m_defaultInstance, new object[] {xpc, element});
		  }
		}
		catch (InvocationTargetException e)
		{
		  Exception targetException = e.getTargetException();

		  if (targetException is TransformerException)
		  {
			throw (TransformerException)targetException;
		  }
		  else if (targetException != null)
		  {
			throw new TransformerException(targetException.Message, targetException);
		  }
		  else
		  {
			throw new TransformerException(e.Message, e);
		  }
		}
		catch (Exception e)
		{
		  // e.printStackTrace ();
		  throw new TransformerException(e.Message, e);
		}

		if (result != null)
		{
		  xpc.outputToResultTree(stylesheetTree, result);
		}

	  }

	}

}