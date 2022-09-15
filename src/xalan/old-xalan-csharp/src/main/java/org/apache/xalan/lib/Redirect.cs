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
 * $Id: Redirect.java 468639 2006-10-28 06:52:33Z minchau $
 */
namespace org.apache.xalan.lib
{



	using XSLProcessorContext = org.apache.xalan.extensions.XSLProcessorContext;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using ElemExtensionCall = org.apache.xalan.templates.ElemExtensionCall;
	using OutputProperties = org.apache.xalan.templates.OutputProperties;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;
	using XObject = org.apache.xpath.objects.XObject;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;
	using ContentHandler = org.xml.sax.ContentHandler;

	/// <summary>
	/// Implements three extension elements to allow an XSLT transformation to
	/// redirect its output to multiple output files.
	/// 
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:redirect="http://xml.apache.org/xalan/redirect"
	/// </pre>
	/// 
	/// <para>You can either just use redirect:write, in which case the file will be
	/// opened and immediately closed after the write, or you can bracket the
	/// write calls by redirect:open and redirect:close, in which case the
	/// file will be kept open for multiple writes until the close call is
	/// encountered.  Calls can be nested.  
	/// 
	/// </para>
	/// <para>Calls can take a 'file' attribute
	/// and/or a 'select' attribute in order to get the filename.  If a select
	/// attribute is encountered, it will evaluate that expression for a string
	/// that indicates the filename.  If the string evaluates to empty, it will
	/// attempt to use the 'file' attribute as a default.  Filenames can be relative
	/// or absolute.  If they are relative, the base directory will be the same as
	/// the base directory for the output document.  This is obtained by calling
	/// getOutputTarget() on the TransformerImpl.  You can set this base directory
	/// by calling TransformerImpl.setOutputTarget() or it is automatically set
	/// when using the two argument form of transform() or transformNode().
	/// 
	/// </para>
	/// <para>Calls to redirect:write and redirect:open also take an optional 
	/// attribute append="true|yes", which will attempt to simply append 
	/// to an existing file instead of always opening a new file.  The 
	/// default behavior of always overwriting the file still happens 
	/// if you do not specify append.
	/// </para>
	/// <para><b>Note:</b> this may give unexpected results when using xml 
	/// or html output methods, since this is <b>not</b> coordinated 
	/// with the serializers - hence, you may get extra xml decls in 
	/// the middle of your file after appending to it.
	/// 
	/// </para>
	/// <para>Example:</para>
	/// <PRE>
	/// &lt;?xml version="1.0"?>
	/// &lt;xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	///                 version="1.0"
	///                 xmlns:redirect="http://xml.apache.org/xalan/redirect"
	///                 extension-element-prefixes="redirect">
	/// 
	///   &lt;xsl:template match="/">
	///     &lt;out>
	///       default output.
	///     &lt;/out>
	///     &lt;redirect:open file="doc3.out"/>
	///     &lt;redirect:write file="doc3.out">
	///       &lt;out>
	///         &lt;redirect:write file="doc1.out">
	///           &lt;out>
	///             doc1 output.
	///             &lt;redirect:write file="doc3.out">
	///               Some text to doc3
	///             &lt;/redirect:write>
	///           &lt;/out>
	///         &lt;/redirect:write>
	///         &lt;redirect:write file="doc2.out">
	///           &lt;out>
	///             doc2 output.
	///             &lt;redirect:write file="doc3.out">
	///               Some more text to doc3
	///               &lt;redirect:write select="doc/foo">
	///                 text for doc4
	///               &lt;/redirect:write>
	///             &lt;/redirect:write>
	///           &lt;/out>
	///         &lt;/redirect:write>
	///       &lt;/out>
	///     &lt;/redirect:write>
	///     &lt;redirect:close file="doc3.out"/>
	///   &lt;/xsl:template>
	/// 
	/// &lt;/xsl:stylesheet>
	/// </PRE>
	/// 
	/// @author Scott Boag
	/// @version 1.0 </summary>
	/// <seealso cref= <a href="../../../../../../extensions.html#ex-redirect" target="_top">Example with Redirect extension</a> </seealso>
	public class Redirect
	{
	  /// <summary>
	  /// List of formatter listeners indexed by filename.
	  /// </summary>
	  protected internal Hashtable m_formatterListeners = new Hashtable();

	  /// <summary>
	  /// List of output streams indexed by filename.
	  /// </summary>
	  protected internal Hashtable m_outputStreams = new Hashtable();

	  /// <summary>
	  /// Default append mode for bare open calls.  
	  /// False for backwards compatibility (I think). 
	  /// </summary>
	  public const bool DEFAULT_APPEND_OPEN = false;

	  /// <summary>
	  /// Default append mode for bare write calls.  
	  /// False for backwards compatibility. 
	  /// </summary>
	  public const bool DEFAULT_APPEND_WRITE = false;

	  /// <summary>
	  /// Open the given file and put it in the XML, HTML, or Text formatter listener's table.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem) throws java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException, javax.xml.transform.TransformerException
	  public virtual void open(XSLProcessorContext context, ElemExtensionCall elem)
	  {
		string fileName = getFilename(context, elem);
		object flistener = m_formatterListeners[fileName];
		if (null == flistener)
		{
		  string mkdirsExpr = elem.getAttribute("mkdirs", context.ContextNode, context.Transformer);
		  bool mkdirs = (!string.ReferenceEquals(mkdirsExpr, null)) ? (mkdirsExpr.Equals("true") || mkdirsExpr.Equals("yes")) : true;

		  // Whether to append to existing files or not, <jpvdm@iafrica.com>
		  string appendExpr = elem.getAttribute("append", context.ContextNode, context.Transformer);
		  bool append = (!string.ReferenceEquals(appendExpr, null)) ? (appendExpr.Equals("true") || appendExpr.Equals("yes")) : DEFAULT_APPEND_OPEN;

		  object ignored = makeFormatterListener(context, elem, fileName, true, mkdirs, append);
		}
	  }

	  /// <summary>
	  /// Write the evalutation of the element children to the given file. Then close the file
	  /// unless it was opened with the open extension element and is in the formatter listener's table.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem) throws java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException, javax.xml.transform.TransformerException
	  public virtual void write(XSLProcessorContext context, ElemExtensionCall elem)
	  {
		string fileName = getFilename(context, elem);
		object flObject = m_formatterListeners[fileName];
		ContentHandler formatter;
		bool inTable = false;
		if (null == flObject)
		{
		  string mkdirsExpr = ((ElemExtensionCall)elem).getAttribute("mkdirs", context.ContextNode, context.Transformer);
		  bool mkdirs = (!string.ReferenceEquals(mkdirsExpr, null)) ? (mkdirsExpr.Equals("true") || mkdirsExpr.Equals("yes")) : true;

		  // Whether to append to existing files or not, <jpvdm@iafrica.com>
		  string appendExpr = elem.getAttribute("append", context.ContextNode, context.Transformer);
		  bool append = (!string.ReferenceEquals(appendExpr, null)) ? (appendExpr.Equals("true") || appendExpr.Equals("yes")) : DEFAULT_APPEND_WRITE;

		  formatter = makeFormatterListener(context, elem, fileName, true, mkdirs, append);
		}
		else
		{
		  inTable = true;
		  formatter = (ContentHandler)flObject;
		}

		TransformerImpl transf = context.Transformer;

		startRedirection(transf, formatter); // for tracing only

		transf.executeChildTemplates(elem, context.ContextNode, context.Mode, formatter);

		endRedirection(transf); // for tracing only

		if (!inTable)
		{
		  System.IO.Stream ostream = (System.IO.Stream)m_outputStreams[fileName];
		  if (null != ostream)
		  {
			try
			{
			  formatter.endDocument();
			}
			catch (org.xml.sax.SAXException se)
			{
			  throw new TransformerException(se);
			}
			ostream.Close();
			m_outputStreams.Remove(fileName);
			m_formatterListeners.Remove(fileName);
		  }
		}
	  }


	  /// <summary>
	  /// Close the given file and remove it from the formatter listener's table.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem) throws java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException, javax.xml.transform.TransformerException
	  public virtual void close(XSLProcessorContext context, ElemExtensionCall elem)
	  {
		string fileName = getFilename(context, elem);
		object formatterObj = m_formatterListeners[fileName];
		if (null != formatterObj)
		{
		  ContentHandler fl = (ContentHandler)formatterObj;
		  try
		  {
			fl.endDocument();
		  }
		  catch (org.xml.sax.SAXException se)
		  {
			throw new TransformerException(se);
		  }
		  System.IO.Stream ostream = (System.IO.Stream)m_outputStreams[fileName];
		  if (null != ostream)
		  {
			ostream.Close();
			m_outputStreams.Remove(fileName);
		  }
		  m_formatterListeners.Remove(fileName);
		}
	  }

	  /// <summary>
	  /// Get the filename from the 'select' or the 'file' attribute.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getFilename(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem) throws java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException, javax.xml.transform.TransformerException
	  private string getFilename(XSLProcessorContext context, ElemExtensionCall elem)
	  {
		string fileName;
		string fileNameExpr = ((ElemExtensionCall)elem).getAttribute("select", context.ContextNode, context.Transformer);
		if (null != fileNameExpr)
		{
		  org.apache.xpath.XPathContext xctxt = context.Transformer.XPathContext;
		  XPath myxpath = new XPath(fileNameExpr, elem, xctxt.NamespaceContext, XPath.SELECT);
		  XObject xobj = myxpath.execute(xctxt, context.ContextNode, elem);
		  fileName = xobj.str();
		  if ((null == fileName) || (fileName.Length == 0))
		  {
			fileName = elem.getAttribute("file", context.ContextNode, context.Transformer);
		  }
		}
		else
		{
		  fileName = elem.getAttribute("file", context.ContextNode, context.Transformer);
		}
		if (null == fileName)
		{
		  context.Transformer.MsgMgr.error(elem, elem, context.ContextNode, XSLTErrorResources.ER_REDIRECT_COULDNT_GET_FILENAME);
								  //"Redirect extension: Could not get filename - file or select attribute must return vald string.");
		}
		return fileName;
	  }

	  // yuck.
	  // Note: this is not the best way to do this, and may not even 
	  //    be fully correct! Patches (with test cases) welcomed. -sc
	  private string urlToFileName(string @base)
	  {
		if (null != @base)
		{
		  if (@base.StartsWith("file:////", StringComparison.Ordinal))
		  {
			@base = @base.Substring(7);
		  }
		  else if (@base.StartsWith("file:///", StringComparison.Ordinal))
		  {
			@base = @base.Substring(6);
		  }
		  else if (@base.StartsWith("file://", StringComparison.Ordinal))
		  {
			@base = @base.Substring(5); // absolute?
		  }
		  else if (@base.StartsWith("file:/", StringComparison.Ordinal))
		  {
			@base = @base.Substring(5);
		  }
		  else if (@base.StartsWith("file:", StringComparison.Ordinal))
		  {
			@base = @base.Substring(4);
		  }
		}
		return @base;
	  }

	  /// <summary>
	  /// Create a new ContentHandler, based on attributes of the current ContentHandler.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.xml.sax.ContentHandler makeFormatterListener(org.apache.xalan.extensions.XSLProcessorContext context, org.apache.xalan.templates.ElemExtensionCall elem, String fileName, boolean shouldPutInTable, boolean mkdirs, boolean append) throws java.net.MalformedURLException, java.io.FileNotFoundException, java.io.IOException, javax.xml.transform.TransformerException
	  private ContentHandler makeFormatterListener(XSLProcessorContext context, ElemExtensionCall elem, string fileName, bool shouldPutInTable, bool mkdirs, bool append)
	  {
		File file = new File(fileName);
		TransformerImpl transformer = context.Transformer;
		string @base; // Base URI to use for relative paths

		if (!file.Absolute)
		{
		  // This code is attributed to Jon Grov <jon@linpro.no>.  A relative file name
		  // is relative to the Result used to kick off the transform.  If no such
		  // Result was supplied, the filename is relative to the source document.
		  // When transforming with a SAXResult or DOMResult, call
		  // TransformerImpl.setOutputTarget() to set the desired Result base.
	  //      String base = urlToFileName(elem.getStylesheet().getSystemId());

		  Result outputTarget = transformer.OutputTarget;
		  if ((null != outputTarget) && (!string.ReferenceEquals((@base = outputTarget.SystemId), null)))
		  {
			@base = urlToFileName(@base);
		  }
		  else
		  {
			@base = urlToFileName(transformer.BaseURLOfSource);
		  }

		  if (null != @base)
		  {
			File baseFile = new File(@base);
			file = new File(baseFile.Parent, fileName);
		  }
		  // System.out.println("file is: "+file.toString());
		}

		if (mkdirs)
		{
		  string dirStr = file.Parent;
		  if ((null != dirStr) && (dirStr.Length > 0))
		  {
			File dir = new File(dirStr);
			dir.mkdirs();
		  }
		}

		// This should be worked on so that the output format can be 
		// defined by a first child of the redirect element.
		OutputProperties format = transformer.OutputFormat;

		// FileOutputStream ostream = new FileOutputStream(file);
		// Patch from above line to below by <jpvdm@iafrica.com>
		//  Note that in JDK 1.2.2 at least, FileOutputStream(File)
		//  is implemented as a call to 
		//  FileOutputStream(File.getPath, append), thus this should be 
		//  the equivalent instead of getAbsolutePath()
		System.IO.FileStream ostream = new System.IO.FileStream(file.Path, append);

		try
		{
		  SerializationHandler flistener = createSerializationHandler(transformer, ostream, file, format);

		  try
		  {
			flistener.startDocument();
		  }
		  catch (org.xml.sax.SAXException se)
		  {
			throw new TransformerException(se);
		  }
		  if (shouldPutInTable)
		  {
			m_outputStreams[fileName] = ostream;
			m_formatterListeners[fileName] = flistener;
		  }
		  return flistener;
		}
		catch (TransformerException te)
		{
		  throw new TransformerException(te);
		}

	  }

	  /// <summary>
	  /// A class that extends this class can over-ride this public method and recieve
	  /// a callback that redirection is about to start </summary>
	  /// <param name="transf"> The transformer. </param>
	  /// <param name="formatter"> The handler that receives the redirected output </param>
	  public virtual void startRedirection(TransformerImpl transf, ContentHandler formatter)
	  {
		  // A class that extends this class could provide a method body        
	  }

	  /// <summary>
	  /// A class that extends this class can over-ride this public method and receive
	  /// a callback that redirection to the ContentHandler specified in the startRedirection()
	  /// call has ended </summary>
	  /// <param name="transf"> The transformer. </param>
	  public virtual void endRedirection(TransformerImpl transf)
	  {
		  // A class that extends this class could provide a method body        
	  }

	  /// <summary>
	  /// A class that extends this one could over-ride this public method and receive
	  /// a callback for the creation of the serializer used in the redirection. </summary>
	  /// <param name="transformer"> The transformer </param>
	  /// <param name="ostream"> The output stream that the serializer wraps </param>
	  /// <param name="file"> The file associated with the ostream </param>
	  /// <param name="format"> The format parameter used to create the serializer </param>
	  /// <returns> the serializer that the redirection will go to.
	  /// </returns>
	  /// <exception cref="java.io.IOException"> </exception>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xml.serializer.SerializationHandler createSerializationHandler(org.apache.xalan.transformer.TransformerImpl transformer, java.io.FileOutputStream ostream, java.io.File file, org.apache.xalan.templates.OutputProperties format) throws java.io.IOException, javax.xml.transform.TransformerException
	  public virtual SerializationHandler createSerializationHandler(TransformerImpl transformer, System.IO.FileStream ostream, File file, OutputProperties format)
	  {

		  SerializationHandler serializer = transformer.createSerializationHandler(new StreamResult(ostream), format);
		  return serializer;
	  }
	}

}