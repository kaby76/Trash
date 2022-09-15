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
 * $Id: FuncDocument.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{



	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XMLString = org.apache.xml.utils.XMLString;
	using Expression = org.apache.xpath.Expression;
	using NodeSetDTM = org.apache.xpath.NodeSetDTM;
	using SourceTreeManager = org.apache.xpath.SourceTreeManager;
	using XPathContext = org.apache.xpath.XPathContext;
	using Function2Args = org.apache.xpath.functions.Function2Args;
	using WrongNumberArgsException = org.apache.xpath.functions.WrongNumberArgsException;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Execute the Doc() function.
	/// 
	/// When the document function has exactly one argument and the argument
	/// is a node-set, then the result is the union, for each node in the
	/// argument node-set, of the result of calling the document function with
	/// the first argument being the string-value of the node, and the second
	/// argument being a node-set with the node as its only member. When the
	/// document function has two arguments and the first argument is a node-set,
	/// then the result is the union, for each node in the argument node-set,
	/// of the result of calling the document function with the first argument
	/// being the string-value of the node, and with the second argument being
	/// the second argument passed to the document function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncDocument : Function2Args
	{
		internal new const long serialVersionUID = 2483304325971281424L;

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		int context = xctxt.CurrentNode;
		DTM dtm = xctxt.getDTM(context);

		int docContext = dtm.getDocumentRoot(context);
		XObject arg = (XObject) this.Arg0.execute(xctxt);

		string @base = "";
		Expression arg1Expr = this.Arg1;

		if (null != arg1Expr)
		{

		  // The URI reference may be relative. The base URI (see [3.2 Base URI]) 
		  // of the node in the second argument node-set that is first in document 
		  // order is used as the base URI for resolving the 
		  // relative URI into an absolute URI. 
		  XObject arg2 = arg1Expr.execute(xctxt);

		  if (XObject.CLASS_NODESET == arg2.Type)
		  {
			int baseNode = arg2.iter().nextNode();

			if (baseNode == org.apache.xml.dtm.DTM_Fields.NULL)
			{
				// See http://www.w3.org/1999/11/REC-xslt-19991116-errata#E14.
				// If the second argument is an empty nodeset, this is an error.
				// The processor can recover by returning an empty nodeset.
				  warn(xctxt, XSLTErrorResources.WG_EMPTY_SECOND_ARG, null);
				  XNodeSet nodes = new XNodeSet(xctxt.DTMManager);
				   return nodes;
			}
			else
			{
				DTM baseDTM = xctxt.getDTM(baseNode);
				@base = baseDTM.DocumentBaseURI;
			}
			// %REVIEW% This doesn't seem to be a problem with the conformance
			// suite, but maybe it's just not doing a good test?
	//        int baseDoc = baseDTM.getDocument();
	//
	//        if (baseDoc == DTM.NULL /* || baseDoc instanceof Stylesheet  -->What to do?? */)
	//        {
	//
	//          // base = ((Stylesheet)baseDoc).getBaseIdentifier();
	//          base = xctxt.getNamespaceContext().getBaseIdentifier();
	//        }
	//        else
	//          base = xctxt.getSourceTreeManager().findURIFromDoc(baseDoc);
		  }
		  else
		  {
			//Can not convert other type to a node-set!;
			arg2.iter();
		  }
		}
		else
		{

		  // If the second argument is omitted, then it defaults to 
		  // the node in the stylesheet that contains the expression that 
		  // includes the call to the document function. Note that a 
		  // zero-length URI reference is a reference to the document 
		  // relative to which the URI reference is being resolved; thus 
		  // document("") refers to the root node of the stylesheet; 
		  // the tree representation of the stylesheet is exactly 
		  // the same as if the XML document containing the stylesheet 
		  // was the initial source document.
		  assertion(null != xctxt.NamespaceContext, "Namespace context can not be null!");
		  @base = xctxt.NamespaceContext.BaseIdentifier;
		}

		XNodeSet nodes = new XNodeSet(xctxt.DTMManager);
		NodeSetDTM mnl = nodes.mutableNodeset();
		DTMIterator iterator = (XObject.CLASS_NODESET == arg.Type) ? arg.iter() : null;
		int pos = org.apache.xml.dtm.DTM_Fields.NULL;

		while ((null == iterator) || (org.apache.xml.dtm.DTM_Fields.NULL != (pos = iterator.nextNode())))
		{
		  XMLString @ref = (null != iterator) ? xctxt.getDTM(pos).getStringValue(pos) : arg.xstr();

		  // The first and only argument was a nodeset, the base in that
		  // case is the base URI of the node from the first argument nodeset. 
		  // Remember, when the document function has exactly one argument and
		  // the argument is a node-set, then the result is the union, for each
		  // node in the argument node-set, of the result of calling the document
		  // function with the first argument being the string-value of the node,
		  // and the second argument being a node-set with the node as its only 
		  // member.
		  if (null == arg1Expr && org.apache.xml.dtm.DTM_Fields.NULL != pos)
		  {
			DTM baseDTM = xctxt.getDTM(pos);
			@base = baseDTM.DocumentBaseURI;
		  }

		  if (null == @ref)
		  {
			continue;
		  }

		  if (org.apache.xml.dtm.DTM_Fields.NULL == docContext)
		  {
			error(xctxt, XSLTErrorResources.ER_NO_CONTEXT_OWNERDOC, null); //"context does not have an owner document!");
		  }

		  // From http://www.ics.uci.edu/pub/ietf/uri/rfc1630.txt
		  // A partial form can be distinguished from an absolute form in that the
		  // latter must have a colon and that colon must occur before any slash
		  // characters. Systems not requiring partial forms should not use any
		  // unencoded slashes in their naming schemes.  If they do, absolute URIs
		  // will still work, but confusion may result.
		  int indexOfColon = @ref.indexOf(':');
		  int indexOfSlash = @ref.indexOf('/');

		  if ((indexOfColon != -1) && (indexOfSlash != -1) && (indexOfColon < indexOfSlash))
		  {

			// The url (or filename, for that matter) is absolute.
			@base = null;
		  }

		  int newDoc = getDoc(xctxt, context, @ref.ToString(), @base);

		  // nodes.mutableNodeset().addNode(newDoc);  
		  if (org.apache.xml.dtm.DTM_Fields.NULL != newDoc)
		  {
			// TODO: mnl.addNodeInDocOrder(newDoc, true, xctxt); ??
			if (!mnl.contains(newDoc))
			{
			  mnl.addElement(newDoc);
			}
		  }

		  if (null == iterator || newDoc == org.apache.xml.dtm.DTM_Fields.NULL)
		  {
			break;
		  }
		}

		return nodes;
	  }

	  /// <summary>
	  /// Get the document from the given URI and base
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state. </param>
	  /// <param name="context"> The current context node </param>
	  /// <param name="uri"> Relative(?) URI of the document </param>
	  /// <param name="base"> Base to resolve relative URI from.
	  /// </param>
	  /// <returns> The document Node pointing to the document at the given URI
	  /// or null
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: int getDoc(org.apache.xpath.XPathContext xctxt, int context, String uri, String super) throws javax.xml.transform.TransformerException
	  internal virtual int getDoc(XPathContext xctxt, int context, string uri, string @base)
	  {

		// System.out.println("base: "+base+", uri: "+uri);
		SourceTreeManager treeMgr = xctxt.SourceTreeManager;
		Source source;

		int newDoc;
		try
		{
		  source = treeMgr.resolveURI(@base, uri, xctxt.SAXLocator);
		  newDoc = treeMgr.getNode(source);
		}
		catch (IOException ioe)
		{
		  throw new TransformerException(ioe.Message, (SourceLocator)xctxt.SAXLocator, ioe);
		}
		catch (TransformerException te)
		{
		  throw new TransformerException(te);
		}

		if (org.apache.xml.dtm.DTM_Fields.NULL != newDoc)
		{
		  return newDoc;
		}

		// If the uri length is zero, get the uri of the stylesheet.
		if (uri.Length == 0)
		{
		  // Hmmm... this seems pretty bogus to me... -sb
		  uri = xctxt.NamespaceContext.BaseIdentifier;
		  try
		  {
			source = treeMgr.resolveURI(@base, uri, xctxt.SAXLocator);
		  }
		  catch (IOException ioe)
		  {
			throw new TransformerException(ioe.Message, (SourceLocator)xctxt.SAXLocator, ioe);
		  }
		}

		string diagnosticsString = null;

		try
		{
		  if ((null != uri) && (uri.Length > 0))
		  {
			newDoc = treeMgr.getSourceTree(source, xctxt.SAXLocator, xctxt);

			// System.out.println("newDoc: "+((Document)newDoc).getDocumentElement().getNodeName());
		  }
		  else
		  {
			warn(xctxt, XSLTErrorResources.WG_CANNOT_MAKE_URL_FROM, new object[]{((string.ReferenceEquals(@base, null)) ? "" : @base) + uri}); //"Can not make URL from: "+((base == null) ? "" : base )+uri);
		  }
		}
		catch (Exception throwable)
		{

		  // throwable.printStackTrace();
		  newDoc = org.apache.xml.dtm.DTM_Fields.NULL;

		  // path.warn(XSLTErrorResources.WG_ENCODING_NOT_SUPPORTED_USING_JAVA, new Object[]{((base == null) ? "" : base )+uri}); //"Can not load requested doc: "+((base == null) ? "" : base )+uri);
		  while (throwable is org.apache.xml.utils.WrappedRuntimeException)
		  {
			throwable = ((org.apache.xml.utils.WrappedRuntimeException) throwable).Exception;
		  }

		  if ((throwable is System.NullReferenceException) || (throwable is System.InvalidCastException))
		  {
			throw new org.apache.xml.utils.WrappedRuntimeException((Exception) throwable);
		  }

		  StringWriter sw = new StringWriter();
		  PrintWriter diagnosticsWriter = new PrintWriter(sw);

		  if (throwable is TransformerException)
		  {
			TransformerException spe = (TransformerException) throwable;

			{
			  Exception e = spe;

			  while (null != e)
			  {
				if (null != e.Message)
				{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				  diagnosticsWriter.println(" (" + e.GetType().FullName + "): " + e.Message);
				}

				if (e is TransformerException)
				{
				  TransformerException spe2 = (TransformerException) e;

				  SourceLocator locator = spe2.Locator;
				  if ((null != locator) && (null != locator.SystemId))
				  {
					diagnosticsWriter.println("   ID: " + locator.SystemId + " Line #" + locator.LineNumber + " Column #" + locator.ColumnNumber);
				  }

				  e = spe2.Exception;

				  if (e is org.apache.xml.utils.WrappedRuntimeException)
				  {
					e = ((org.apache.xml.utils.WrappedRuntimeException) e).Exception;
				  }
				}
				else
				{
				  e = null;
				}
			  }
			}
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			diagnosticsWriter.println(" (" + throwable.GetType().FullName + "): " + throwable.Message);
		  }

		  diagnosticsString = throwable.Message; //sw.toString();
		}

		if (org.apache.xml.dtm.DTM_Fields.NULL == newDoc)
		{

		  // System.out.println("what?: "+base+", uri: "+uri);
		  if (null != diagnosticsString)
		  {
			warn(xctxt, XSLTErrorResources.WG_CANNOT_LOAD_REQUESTED_DOC, new object[]{diagnosticsString}); //"Can not load requested doc: "+((base == null) ? "" : base )+uri);
		  }
		  else
		  {
			warn(xctxt, XSLTErrorResources.WG_CANNOT_LOAD_REQUESTED_DOC, new object[]{string.ReferenceEquals(uri, null) ? ((string.ReferenceEquals(@base, null)) ? "" : @base) + uri : uri.ToString()}); //"Can not load requested doc: "+((base == null) ? "" : base )+uri);
		  }
		}
		else
		{
		  // %REVIEW%
		  // TBD: What to do about XLocator?
		  // xctxt.getSourceTreeManager().associateXLocatorToNode(newDoc, url, null);
		}

		return newDoc;
	  }

	  /// <summary>
	  /// Tell the user of an error, and probably throw an
	  /// exception.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state. </param>
	  /// <param name="msg"> The error message key </param>
	  /// <param name="args"> Arguments to be used in the error message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.apache.xpath.XPathContext xctxt, String msg, Object args[]) throws javax.xml.transform.TransformerException
	  public override void error(XPathContext xctxt, string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createMessage(msg, args);
		ErrorListener errHandler = xctxt.ErrorListener;
		TransformerException spe = new TransformerException(formattedMsg, (SourceLocator)xctxt.SAXLocator);

		if (null != errHandler)
		{
		  errHandler.error(spe);
		}
		else
		{
		  Console.WriteLine(formattedMsg);
		}
	  }

	  /// <summary>
	  /// Warn the user of a problem.
	  /// </summary>
	  /// <param name="xctxt"> The XPath runtime state. </param>
	  /// <param name="msg"> Warning message key </param>
	  /// <param name="args"> Arguments to be used in the warning message </param>
	  /// <exception cref="XSLProcessorException"> thrown if the active ProblemListener and XPathContext decide
	  /// the error condition is severe enough to halt processing.
	  /// </exception>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warn(org.apache.xpath.XPathContext xctxt, String msg, Object args[]) throws javax.xml.transform.TransformerException
	  public override void warn(XPathContext xctxt, string msg, object[] args)
	  {

		string formattedMsg = XSLMessages.createWarning(msg, args);
		ErrorListener errHandler = xctxt.ErrorListener;
		TransformerException spe = new TransformerException(formattedMsg, (SourceLocator)xctxt.SAXLocator);

		if (null != errHandler)
		{
		  errHandler.warning(spe);
		}
		else
		{
		  Console.WriteLine(formattedMsg);
		}
	  }

	 /// <summary>
	 /// Overide the superclass method to allow one or two arguments.
	 ///  
	 /// </summary>
	 /// <param name="argNum"> Number of arguments passed in to this function
	 /// </param>
	 /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkNumberArgs(int argNum) throws org.apache.xpath.functions.WrongNumberArgsException
	  public override void checkNumberArgs(int argNum)
	  {
		if ((argNum < 1) || (argNum > 2))
		{
		  reportWrongNumberArgs();
		}
	  }

	  /// <summary>
	  /// Constructs and throws a WrongNumberArgException with the appropriate
	  /// message for this function object.
	  /// </summary>
	  /// <exception cref="WrongNumberArgsException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void reportWrongNumberArgs() throws org.apache.xpath.functions.WrongNumberArgsException
	  protected internal override void reportWrongNumberArgs()
	  {
		  throw new WrongNumberArgsException(XSLMessages.createMessage(XSLTErrorResources.ER_ONE_OR_TWO, null)); //"1 or 2");
	  }

	  /// <summary>
	  /// Tell if the expression is a nodeset expression. </summary>
	  /// <returns> true if the expression can be represented as a nodeset. </returns>
	  public override bool NodesetExpr
	  {
		  get
		  {
			return true;
		  }
	  }

	}

}