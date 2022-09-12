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
 * $Id: ExtensionHandlerGeneral.java 469672 2006-10-31 21:56:19Z minchau $
 */
namespace org.apache.xalan.extensions
{


	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMNodeList = org.apache.xml.dtm.@ref.DTMNodeList;
	using StringVector = org.apache.xml.utils.StringVector;
	using SystemIDResolver = org.apache.xml.utils.SystemIDResolver;
	using XPathProcessorException = org.apache.xpath.XPathProcessorException;
	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Class handling an extension namespace for XPath. Provides functions
	/// to test a function's existence and call a function
	/// 
	/// @author Sanjiva Weerawarana (sanjiva@watson.ibm.com)
	/// @xsl.usage internal
	/// </summary>
	public class ExtensionHandlerGeneral : ExtensionHandler
	{

	  /// <summary>
	  /// script source to run (if any) </summary>
	  private string m_scriptSrc;

	  /// <summary>
	  /// URL of source of script (if any) </summary>
	  private string m_scriptSrcURL;

	  /// <summary>
	  /// functions of namespace </summary>
	  private Hashtable m_functions = new Hashtable();

	  /// <summary>
	  /// elements of namespace </summary>
	  private Hashtable m_elements = new Hashtable();

	  // BSF objects used to invoke BSF by reflection.  Do not import the BSF classes
	  // since we don't want a compile dependency on BSF.

	  /// <summary>
	  /// BSF manager used to run scripts </summary>
	  private object m_engine;

	  /// <summary>
	  /// Engine call to invoke scripts </summary>
	  private Method m_engineCall = null;

	  // static fields

	  /// <summary>
	  /// BSFManager package name </summary>
	  private static string BSF_MANAGER;

	  /// <summary>
	  /// Default BSFManager name </summary>
	  private const string DEFAULT_BSF_MANAGER = "org.apache.bsf.BSFManager";

	  /// <summary>
	  /// Property name to load the BSFManager class </summary>
	  private const string propName = "org.apache.xalan.extensions.bsf.BSFManager";

	  /// <summary>
	  /// Integer Zero </summary>
	  private static readonly int? ZEROINT = new int?(0);

	  static ExtensionHandlerGeneral()
	  {
			  BSF_MANAGER = ObjectFactory.lookUpFactoryClassName(propName, null, null);

			  if (string.ReferenceEquals(BSF_MANAGER, null))
			  {
					  BSF_MANAGER = DEFAULT_BSF_MANAGER;
			  }
	  }

	  /// <summary>
	  /// Construct a new extension namespace handler given all the information
	  /// needed.
	  /// </summary>
	  /// <param name="namespaceUri"> the extension namespace URI that I'm implementing </param>
	  /// <param name="elemNames"> Vector of element names </param>
	  /// <param name="funcNames">    string containing list of functions of extension NS </param>
	  /// <param name="scriptLang"> Scripting language of implementation </param>
	  /// <param name="scriptSrcURL"> URL of source script </param>
	  /// <param name="scriptSrc">    the actual script code (if any) </param>
	  /// <param name="systemId">
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ExtensionHandlerGeneral(String namespaceUri, org.apache.xml.utils.StringVector elemNames, org.apache.xml.utils.StringVector funcNames, String scriptLang, String scriptSrcURL, String scriptSrc, String systemId) throws javax.xml.transform.TransformerException
	  public ExtensionHandlerGeneral(string namespaceUri, StringVector elemNames, StringVector funcNames, string scriptLang, string scriptSrcURL, string scriptSrc, string systemId) : base(namespaceUri, scriptLang)
	  {


		if (elemNames != null)
		{
		  object junk = new object();
		  int n = elemNames.size();

		  for (int i = 0; i < n; i++)
		  {
			string tok = elemNames.elementAt(i);

			m_elements[tok] = junk; // just stick it in there basically
		  }
		}

		if (funcNames != null)
		{
		  object junk = new object();
		  int n = funcNames.size();

		  for (int i = 0; i < n; i++)
		  {
			string tok = funcNames.elementAt(i);

			m_functions[tok] = junk; // just stick it in there basically
		  }
		}

		m_scriptSrcURL = scriptSrcURL;
		m_scriptSrc = scriptSrc;

		if (!string.ReferenceEquals(m_scriptSrcURL, null))
		{
		  URL url = null;
		  try
		  {
			url = new URL(m_scriptSrcURL);
		  }
		  catch (java.net.MalformedURLException mue)
		  {
			int indexOfColon = m_scriptSrcURL.IndexOf(':');
			int indexOfSlash = m_scriptSrcURL.IndexOf('/');

			if ((indexOfColon != -1) && (indexOfSlash != -1) && (indexOfColon < indexOfSlash))
			{
			  // The url is absolute.
			  url = null;
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_COULD_NOT_FIND_EXTERN_SCRIPT, new object[]{m_scriptSrcURL}), mue); //"src attribute not yet supported for "
			  //+ scriptLang);
			}
			else
			{
			  try
			  {
				url = new URL(new URL(SystemIDResolver.getAbsoluteURI(systemId)), m_scriptSrcURL);
			  }
			  catch (java.net.MalformedURLException mue2)
			  {
				throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_COULD_NOT_FIND_EXTERN_SCRIPT, new object[]{m_scriptSrcURL}), mue2); //"src attribute not yet supported for "
			  //+ scriptLang);
			  }
			}
		  }
		  if (url != null)
		  {
			try
			{
			  URLConnection uc = url.openConnection();
			  System.IO.Stream @is = uc.InputStream;
			  sbyte[] bArray = new sbyte[uc.ContentLength];
			  @is.Read(bArray, 0, bArray.Length);
			  m_scriptSrc = StringHelperClass.NewString(bArray);

			}
			catch (IOException ioe)
			{
			  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_COULD_NOT_FIND_EXTERN_SCRIPT, new object[]{m_scriptSrcURL}), ioe); //"src attribute not yet supported for "
			  //+ scriptLang);
			}
		  }

		}

		object manager = null;
		try
		{
		  manager = ObjectFactory.newInstance(BSF_MANAGER, ObjectFactory.findClassLoader(), true);
		}
		catch (ObjectFactory.ConfigurationError e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}

		if (manager == null)
		{
		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_INIT_BSFMGR, null)); //"Could not initialize BSF manager");
		}

		try
		{
		  Method loadScriptingEngine = manager.GetType().GetMethod("loadScriptingEngine", new Type[]{typeof(string)});

		  m_engine = loadScriptingEngine.invoke(manager, new object[]{scriptLang});

		  Method engineExec = m_engine.GetType().GetMethod("exec", new Type[]{typeof(string), Integer.TYPE, Integer.TYPE, typeof(object)});

		  // "Compile" the program
		  engineExec.invoke(m_engine, new object[]{"XalanScript", ZEROINT, ZEROINT, m_scriptSrc});
		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);

		  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_CMPL_EXTENSN, null), e); //"Could not compile extension", e);
		}
	  }

	  /// <summary>
	  /// Tests whether a certain function name is known within this namespace. </summary>
	  /// <param name="function"> name of the function being tested </param>
	  /// <returns> true if its known, false if not. </returns>
	  public override bool isFunctionAvailable(string function)
	  {
		return (m_functions[function] != null);
	  }

	  /// <summary>
	  /// Tests whether a certain element name is known within this namespace. </summary>
	  /// <param name="element"> name of the element being tested </param>
	  /// <returns> true if its known, false if not. </returns>
	  public override bool isElementAvailable(string element)
	  {
		return (m_elements[element] != null);
	  }

	  /// <summary>
	  /// Process a call to a function.
	  /// </summary>
	  /// <param name="funcName"> Function name. </param>
	  /// <param name="args">     The arguments of the function call. </param>
	  /// <param name="methodKey"> A key that uniquely identifies this class and method call. </param>
	  /// <param name="exprContext"> The context in which this expression is being executed.
	  /// </param>
	  /// <returns> the return value of the function evaluation.
	  /// </returns>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object callFunction(String funcName, java.util.Vector args, Object methodKey, ExpressionContext exprContext) throws javax.xml.transform.TransformerException
	  public override object callFunction(string funcName, ArrayList args, object methodKey, ExpressionContext exprContext)
	  {

		object[] argArray;

		try
		{
		  argArray = new object[args.Count];

		  for (int i = 0; i < argArray.Length; i++)
		  {
			object o = args[i];

			argArray[i] = (o is XObject) ? ((XObject) o).@object() : o;
			o = argArray[i];
			if (null != o && o is DTMIterator)
			{
			  argArray[i] = new DTMNodeList((DTMIterator)o);
			}
		  }

		  if (m_engineCall == null)
		  {
			m_engineCall = m_engine.GetType().GetMethod("call", new Type[]{typeof(object), typeof(string), typeof(object[])});
		  }

		  return m_engineCall.invoke(m_engine, new object[]{null, funcName, argArray});
		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);

		  string msg = e.Message;

		  if (null != msg)
		  {
			if (msg.StartsWith("Stopping after fatal error:", StringComparison.Ordinal))
			{
			  msg = msg.Substring("Stopping after fatal error:".Length);
			}

			// System.out.println("Call to extension function failed: "+msg);
			throw new TransformerException(e);
		  }
		  else
		  {

			// Should probably make a TRaX Extension Exception.
			throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_CANNOT_CREATE_EXTENSN, new object[]{funcName, e})); //"Could not create extension: " + funcName
								   //+ " because of: " + e);
		  }
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
	  /// </summary>
	  /// <param name="localPart">      Element name's local part. </param>
	  /// <param name="element">        The extension element being processed. </param>
	  /// <param name="transformer">      Handle to TransformerImpl. </param>
	  /// <param name="stylesheetTree"> The compiled stylesheet tree. </param>
	  /// <param name="methodKey"> A key that uniquely identifies this class and method call.
	  /// </param>
	  /// <exception cref="XSLProcessorException"> thrown if something goes wrong
	  ///            while running the extension handler. </exception>
	  /// <exception cref="MalformedURLException"> if loading trouble </exception>
	  /// <exception cref="FileNotFoundException"> if loading trouble </exception>
	  /// <exception cref="IOException">           if loading trouble </exception>
	  /// <exception cref="TransformerException">          if parsing trouble </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void processElement(String localPart, org.apache.xalan.templates.ElemTemplateElement element, org.apache.xalan.transformer.TransformerImpl transformer, org.apache.xalan.templates.Stylesheet stylesheetTree, Object methodKey) throws javax.xml.transform.TransformerException, java.io.IOException
	  public override void processElement(string localPart, ElemTemplateElement element, TransformerImpl transformer, Stylesheet stylesheetTree, object methodKey)
	  {

		object result = null;
		XSLProcessorContext xpc = new XSLProcessorContext(transformer, stylesheetTree);

		try
		{
		  ArrayList argv = new ArrayList(2);

		  argv.Add(xpc);
		  argv.Add(element);

		  result = callFunction(localPart, argv, methodKey, transformer.XPathContext.ExpressionContext);
		}
		catch (XPathProcessorException e)
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