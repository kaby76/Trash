using System;
using System.Collections;
using System.Text;
using System.Threading;

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
 * $Id: XSLTProcessorApplet.java 1225408 2011-12-29 01:11:41Z mrglavas $
 */
namespace org.apache.xalan.client
{



	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	/// <summary>
	/// Provides applet host for the XSLT processor. To perform transformations on an HTML client:
	/// <ol>
	/// <li>Use an &lt;applet&gt; tag to embed this applet in the HTML client.</li>
	/// <li>Use the DocumentURL and StyleURL PARAM tags or the <seealso cref="#setDocumentURL"/> and
	/// <seealso cref="#setStyleURL"/> methods to specify the XML source document and XSL stylesheet.</li>
	/// <li>Call the <seealso cref="#getHtmlText"/> method (or one of the transformToHtml() methods)
	/// to perform the transformation and return the result as a String.</li>
	/// </ol>
	/// 
	/// This class extends Applet which ultimately causes this class to implement Serializable.
	/// This is a serious restriction on this class. All fields that are not transient and not
	/// static are written-out/read-in during serialization. So even private fields essentially
	/// become part of the API. Developers need to take care when modifying fields.
	/// @xsl.usage general
	/// </summary>
	public class XSLTProcessorApplet : Applet
	{

	  /// <summary>
	  /// The stylesheet processor.
	  /// This field is now transient because a 
	  /// javax.xml.transform.TransformerFactory from JAXP 
	  /// makes no claims to be serializable.
	  /// </summary>
	  [NonSerialized]
	  internal TransformerFactory m_tfactory = null;

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private string m_styleURL;

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private string m_documentURL;

	  // Parameter names.  To change a name of a parameter, you need only make
	  // a single change.  Simply modify the value of the parameter string below.
	  //--------------------------------------------------------------------------

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private readonly string PARAM_styleURL = "styleURL";

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private readonly string PARAM_documentURL = "documentURL";


	  // We'll keep the DOM trees around, so tell which trees
	  // are cached.

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private string m_styleURLOfCached = null;

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private string m_documentURLOfCached = null;

	  /// <summary>
	  /// Save this for use on the worker thread; may not be necessary.
	  /// @serial
	  /// </summary>
	  private URL m_codeBase = null;

	  /// <summary>
	  /// @serial
	  /// </summary>
	  private string m_treeURL = null;

	  /// <summary>
	  /// DocumentBase URL
	  /// @serial       
	  /// </summary>
	  private URL m_documentBase = null;

	  /// <summary>
	  /// Thread stuff for the trusted worker thread.
	  /// </summary>
	  [NonSerialized]
	  private Thread m_callThread = null;

	  [NonSerialized]
	  private TrustedAgent m_trustedAgent = null;

	  /// <summary>
	  /// Thread for running TrustedAgent.
	  /// </summary>
	  [NonSerialized]
	  private Thread m_trustedWorker = null;

	  /// <summary>
	  /// Where the worker thread puts the HTML text.
	  /// </summary>
	  [NonSerialized]
	  private string m_htmlText = null;

	  /// <summary>
	  /// Where the worker thread puts the document/stylesheet text.
	  /// </summary>
	  [NonSerialized]
	  private string m_sourceText = null;

	  /// <summary>
	  /// Stylesheet attribute name and value that the caller can set.
	  /// </summary>
	  [NonSerialized]
	  private string m_nameOfIDAttrOfElemToModify = null;

	  [NonSerialized]
	  private string m_elemIdToModify = null;

	  [NonSerialized]
	  private string m_attrNameToSet = null;

	  [NonSerialized]
	  private string m_attrValueToSet = null;

	  /// <summary>
	  /// The XSLTProcessorApplet constructor takes no arguments.
	  /// </summary>
	  public XSLTProcessorApplet()
	  {
	  }

	  /// <summary>
	  /// Get basic information about the applet </summary>
	  /// <returns> A String with the applet name and author. </returns>
	  public virtual string AppletInfo
	  {
		  get
		  {
			return "Name: XSLTProcessorApplet\r\n" + "Author: Scott Boag";
		  }
	  }

	  /// <summary>
	  /// Get descriptions of the applet parameters. </summary>
	  /// <returns> A two-dimensional array of Strings with Name, Type, and Description
	  /// for each parameter. </returns>
	  public virtual string[][] ParameterInfo
	  {
		  get
		  {
    
			string[][] info = new string[][]
			{
				new string[] {PARAM_styleURL, "String", "URL to an XSL stylesheet"},
				new string[] {PARAM_documentURL, "String", "URL to an XML document"}
			};
    
			return info;
		  }
	  }

	  /// <summary>
	  /// Standard applet initialization.
	  /// </summary>
	  public virtual void init()
	  {

		// PARAMETER SUPPORT
		//          The following code retrieves the value of each parameter
		// specified with the <PARAM> tag and stores it in a member
		// variable.
		//----------------------------------------------------------------------
		string param;

		// styleURL: Parameter description
		//----------------------------------------------------------------------
		param = getParameter(PARAM_styleURL);

		// stylesheet parameters
		m_parameters = new Hashtable();

		if (!string.ReferenceEquals(param, null))
		{
		  StyleURL = param;
		}

		// documentURL: Parameter description
		//----------------------------------------------------------------------
		param = getParameter(PARAM_documentURL);

		if (!string.ReferenceEquals(param, null))
		{
		  DocumentURL = param;
		}

		m_codeBase = this.CodeBase;
		m_documentBase = this.DocumentBase;

		// If you use a ResourceWizard-generated "control creator" class to
		// arrange controls in your applet, you may want to call its
		// CreateControls() method from within this method. Remove the following
		// call to resize() before adding the call to CreateControls();
		// CreateControls() does its own resizing.
		//----------------------------------------------------------------------
		resize(320, 240);
	  }

		/// <summary>
		///  Automatically called when the HTML client containing the applet loads.
		///  This method starts execution of the applet thread.
		/// </summary>
	  public virtual void start()
	  {

		m_trustedAgent = new TrustedAgent(this);
		Thread currentThread = Thread.CurrentThread;
		m_trustedWorker = new Thread(currentThread.ThreadGroup, m_trustedAgent);
		m_trustedWorker.Start();
		try
		{
		  m_tfactory = TransformerFactory.newInstance();
		  this.showStatus("Causing Transformer and Parser to Load and JIT...");

		  // Prime the pump so that subsequent transforms are faster.
		  StringReader xmlbuf = new StringReader("<?xml version='1.0'?><foo/>");
		  StringReader xslbuf = new StringReader("<?xml version='1.0'?><xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'><xsl:template match='foo'><out/></xsl:template></xsl:stylesheet>");
		  PrintWriter pw = new PrintWriter(new StringWriter());

		  lock (m_tfactory)
		  {
			Templates templates = m_tfactory.newTemplates(new StreamSource(xslbuf));
			Transformer transformer = templates.newTransformer();
			transformer.transform(new StreamSource(xmlbuf), new StreamResult(pw));
		  }
		  Console.WriteLine("Primed the pump!");
		  this.showStatus("Ready to go!");
		}
		catch (Exception e)
		{
		  this.showStatus("Could not prime the pump!");
		  Console.WriteLine("Could not prime the pump!");
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }

	  /// <summary>
	  /// Do not call; this applet contains no UI or visual components.
	  /// 
	  /// </summary>
	  public virtual void paint(Graphics g)
	  {
	  }

	  /// <summary>
	  /// Automatically called when the HTML page containing the applet is no longer
	  /// on the screen. Stops execution of the applet thread.
	  /// </summary>
	  public virtual void stop()
	  {
		if (null != m_trustedWorker)
		{
		  m_trustedWorker.Abort();

		  // m_trustedWorker.destroy();
		  m_trustedWorker = null;
		}

		m_styleURLOfCached = null;
		m_documentURLOfCached = null;
	  }

	  /// <summary>
	  /// Cleanup; called when applet is terminated and unloaded.
	  /// </summary>
	  public virtual void destroy()
	  {
		if (null != m_trustedWorker)
		{
		  m_trustedWorker.Abort();

		  // m_trustedWorker.destroy();
		  m_trustedWorker = null;
		}
		m_styleURLOfCached = null;
		m_documentURLOfCached = null;
	  }

	  /// <summary>
	  /// Set the URL to the XSL stylesheet that will be used
	  /// to transform the input XML.  No processing is done yet. </summary>
	  /// <param name="urlString"> valid URL string for XSL stylesheet. </param>
	  public virtual string StyleURL
	  {
		  set
		  {
			m_styleURL = value;
		  }
	  }

	  /// <summary>
	  /// Set the URL to the XML document that will be transformed
	  /// with the XSL stylesheet.  No processing is done yet. </summary>
	  /// <param name="urlString"> valid URL string for XML document. </param>
	  public virtual string DocumentURL
	  {
		  set
		  {
			m_documentURL = value;
		  }
	  }

	  /// <summary>
	  /// The processor keeps a cache of the source and
	  /// style trees, so call this method if they have changed
	  /// or you want to do garbage collection.
	  /// </summary>
	  public virtual void freeCache()
	  {
		m_styleURLOfCached = null;
		m_documentURLOfCached = null;
	  }

	  /// <summary>
	  /// Set an attribute in the stylesheet, which gives the ability
	  /// to have some dynamic selection control. </summary>
	  /// <param name="nameOfIDAttrOfElemToModify"> The name of an attribute to search for a unique id. </param>
	  /// <param name="elemId"> The unique ID to look for. </param>
	  /// <param name="attrName"> Once the element is found, the name of the attribute to set. </param>
	  /// <param name="value"> The value to set the attribute to. </param>
	  public virtual void setStyleSheetAttribute(string nameOfIDAttrOfElemToModify, string elemId, string attrName, string value)
	  {
		m_nameOfIDAttrOfElemToModify = nameOfIDAttrOfElemToModify;
		m_elemIdToModify = elemId;
		m_attrNameToSet = attrName;
		m_attrValueToSet = value;
	  }


	  /// <summary>
	  /// Stylesheet parameter key/value pair stored in a hashtable
	  /// </summary>
	  [NonSerialized]
	  internal Hashtable m_parameters;

	  /// <summary>
	  /// Submit a stylesheet parameter.
	  /// </summary>
	  /// <param name="key"> stylesheet parameter key </param>
	  /// <param name="expr"> the parameter expression to be submitted. </param>
	  /// <seealso cref= javax.xml.transform.Transformer#setParameter(String,Object) </seealso>
	  public virtual void setStylesheetParam(string key, string expr)
	  {
		m_parameters[key] = expr;
	  }

	  /// <summary>
	  /// Given a String containing markup, escape the markup so it
	  /// can be displayed in the browser.
	  /// </summary>
	  /// <param name="s"> String to escape
	  /// 
	  /// The escaped string. </param>
	  public virtual string escapeString(string s)
	  {
		StringBuilder sb = new StringBuilder();
		int length = s.Length;

		for (int i = 0; i < length; i++)
		{
		  char ch = s[i];

		  if ('<' == ch)
		  {
			sb.Append("&lt;");
		  }
		  else if ('>' == ch)
		  {
			sb.Append("&gt;");
		  }
		  else if ('&' == ch)
		  {
			sb.Append("&amp;");
		  }
		  else if (0xd800 <= ch && ch < 0xdc00)
		  {
			// UTF-16 surrogate
			int next;

			if (i + 1 >= length)
			{
			  throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_UTF16_SURROGATE, new object[]{ch.ToString("x")})); //"Invalid UTF-16 surrogate detected: "

			  //+Integer.toHexString(ch)+ " ?");
			}
			else
			{
			  next = s[++i];

			  if (!(0xdc00 <= next && next < 0xe000))
			  {
				throw new Exception(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_UTF16_SURROGATE, new object[]{ch.ToString("x") + " " + next.ToString("x")})); //"Invalid UTF-16 surrogate detected: "
			  }

			  //+Integer.toHexString(ch)+" "+Integer.toHexString(next));
			  next = ((ch - 0xd800) << 10) + next - 0xdc00 + 0x00010000;
			}
			sb.Append("&#x");
			sb.Append(next.ToString("x"));
			sb.Append(";");
		  }
		  else
		  {
			sb.Append(ch);
		  }
		}
		return sb.ToString();
	  }

	  /// <summary>
	  /// Assuming the stylesheet URL and the input XML URL have been set,
	  /// perform the transformation and return the result as a String.
	  /// </summary>
	  /// <returns> A string that contains the contents pointed to by the URL.
	  ///  </returns>
	  public virtual string HtmlText
	  {
		  get
		  {
			m_trustedAgent.m_getData = true;
			m_callThread = Thread.CurrentThread;
			try
			{
			  lock (m_callThread)
			  {
				Monitor.Wait(m_callThread);
			  }
			}
			catch (InterruptedException ie)
			{
			  Console.WriteLine(ie.Message);
			}
			return m_htmlText;
		  }
	  }

	  /// <summary>
	  /// Get an XML document (or stylesheet)
	  /// </summary>
	  /// <param name="treeURL"> valid URL string for the document.
	  /// </param>
	  /// <returns> document
	  /// </returns>
	  /// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getTreeAsText(String treeURL) throws java.io.IOException
	  public virtual string getTreeAsText(string treeURL)
	  {
		m_treeURL = treeURL;
		m_trustedAgent.m_getData = true;
		m_trustedAgent.m_getSource = true;
		m_callThread = Thread.CurrentThread;
		try
		{
		  lock (m_callThread)
		  {
			Monitor.Wait(m_callThread);
		  }
		}
		catch (InterruptedException ie)
		{
		  Console.WriteLine(ie.Message);
		}
		return m_sourceText;
	  }

	  /// <summary>
	  /// Use a Transformer to copy the source document
	  /// to a StreamResult.
	  /// </summary>
	  /// <returns> the document as a string </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String getSource() throws javax.xml.transform.TransformerException
	  private string Source
	  {
		  get
		  {
			StringWriter osw = new StringWriter();
			PrintWriter pw = new PrintWriter(osw, false);
			string text = "";
			try
			{
			  URL docURL = new URL(m_documentBase, m_treeURL);
			  lock (m_tfactory)
			  {
				Transformer transformer = m_tfactory.newTransformer();
				StreamSource source = new StreamSource(docURL.ToString());
				StreamResult result = new StreamResult(pw);
				transformer.transform(source, result);
				text = osw.ToString();
			  }
			}
			catch (MalformedURLException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			  throw new Exception(e.Message);
			}
			catch (Exception any_error)
			{
			  Console.WriteLine(any_error.ToString());
			  Console.Write(any_error.StackTrace);
			}
			return text;
		  }
	  }

	  /// <summary>
	  /// Get the XML source Tree as a text string suitable
	  /// for display in a browser.  Note that this is for display of the
	  /// XML itself, not for rendering of HTML by the browser.
	  /// </summary>
	  /// <returns> XML source document as a string. </returns>
	  /// <exception cref="Exception"> thrown if tree can not be converted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getSourceTreeAsText() throws Exception
	  public virtual string SourceTreeAsText
	  {
		  get
		  {
			return getTreeAsText(m_documentURL);
		  }
	  }

	  /// <summary>
	  /// Get the XSL style Tree as a text string suitable
	  /// for display in a browser.  Note that this is for display of the
	  /// XML itself, not for rendering of HTML by the browser.
	  /// </summary>
	  /// <returns> The XSL stylesheet as a string. </returns>
	  /// <exception cref="Exception"> thrown if tree can not be converted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getStyleTreeAsText() throws Exception
	  public virtual string StyleTreeAsText
	  {
		  get
		  {
			return getTreeAsText(m_styleURL);
		  }
	  }

	  /// <summary>
	  /// Get the HTML result Tree as a text string suitable
	  /// for display in a browser.  Note that this is for display of the
	  /// XML itself, not for rendering of HTML by the browser.
	  /// </summary>
	  /// <returns> Transformation result as unmarked text. </returns>
	  /// <exception cref="Exception"> thrown if tree can not be converted. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getResultTreeAsText() throws Exception
	  public virtual string ResultTreeAsText
	  {
		  get
		  {
			return escapeString(HtmlText);
		  }
	  }

	  /// <summary>
	  /// Process a document and a stylesheet and return
	  /// the transformation result.  If one of these is null, the
	  /// existing value (of a previous transformation) is not affected.
	  /// </summary>
	  /// <param name="doc"> URL string to XML document </param>
	  /// <param name="style"> URL string to XSL stylesheet
	  /// </param>
	  /// <returns> HTML transformation result </returns>
	  public virtual string transformToHtml(string doc, string style)
	  {

		if (null != doc)
		{
		  m_documentURL = doc;
		}

		if (null != style)
		{
		  m_styleURL = style;
		}

		return HtmlText;
	  }

	  /// <summary>
	  /// Process a document and a stylesheet and return
	  /// the transformation result. Use the xsl:stylesheet PI to find the
	  /// document, if one exists.
	  /// </summary>
	  /// <param name="doc">  URL string to XML document containing an xsl:stylesheet PI.
	  /// </param>
	  /// <returns> HTML transformation result </returns>
	  public virtual string transformToHtml(string doc)
	  {

		if (null != doc)
		{
		  m_documentURL = doc;
		}

		m_styleURL = null;

		return HtmlText;
	  }


	  /// <summary>
	  /// Process the transformation.
	  /// </summary>
	  /// <returns> The transformation result as a string.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String processTransformation() throws javax.xml.transform.TransformerException
	  private string processTransformation()
	  {
		string htmlData = null;
		this.showStatus("Waiting for Transformer and Parser to finish loading and JITing...");

		lock (m_tfactory)
		{
		 URL documentURL = null;
		  URL styleURL = null;
		  StringWriter osw = new StringWriter();
		  PrintWriter pw = new PrintWriter(osw, false);
		  StreamResult result = new StreamResult(pw);

		  this.showStatus("Begin Transformation...");
		  try
		  {
			documentURL = new URL(m_codeBase, m_documentURL);
			StreamSource xmlSource = new StreamSource(documentURL.ToString());

			styleURL = new URL(m_codeBase, m_styleURL);
			StreamSource xslSource = new StreamSource(styleURL.ToString());

			Transformer transformer = m_tfactory.newTransformer(xslSource);

			IEnumerator m_entries = m_parameters.SetOfKeyValuePairs().GetEnumerator();
			while (m_entries.MoveNext())
			{
				DictionaryEntry entry = (DictionaryEntry) m_entries.Current;
				object key = entry.Key;
				object expression = entry.Value;
				transformer.setParameter((string) key, expression);
			}
			transformer.transform(xmlSource, result);
		  }
		  catch (TransformerConfigurationException tfe)
		  {
			Console.WriteLine(tfe.ToString());
			Console.Write(tfe.StackTrace);
			throw new Exception(tfe.Message);
		  }
		  catch (MalformedURLException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
			throw new Exception(e.Message);
		  }

		  this.showStatus("Transformation Done!");
		  htmlData = osw.ToString();
		}
		return htmlData;
	  }

	  /// <summary>
	  /// This class maintains a worker thread that that is
	  /// trusted and can do things like access data.  You need
	  /// this because the thread that is called by the browser
	  /// is not trusted and can't access data from the URLs.
	  /// </summary>
	  internal class TrustedAgent : System.Threading.ThreadStart
	  {
		  private readonly XSLTProcessorApplet outerInstance;

		  public TrustedAgent(XSLTProcessorApplet outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		/// <summary>
		/// Specifies whether the worker thread should perform a transformation.
		/// </summary>
		public bool m_getData = false;

		/// <summary>
		/// Specifies whether the worker thread should get an XML document or XSL stylesheet.
		/// </summary>
		public bool m_getSource = false;

		/// <summary>
		/// The real work is done from here.
		/// 
		/// </summary>
		public virtual void run()
		{
		  while (true)
		  {
			Thread.yield();

			if (m_getData) // Perform a transformation or get a document.
			{
			  try
			  {
				m_getData = false;
				outerInstance.m_htmlText = null;
				outerInstance.m_sourceText = null;
				if (m_getSource) // Get a document.
				{
				  m_getSource = false;
				  outerInstance.m_sourceText = outerInstance.Source;
				}
				else // Perform a transformation.
				{
				  outerInstance.m_htmlText = outerInstance.processTransformation();
				}
			  }
			  catch (Exception e)
			  {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			  }
			  finally
			  {
				lock (outerInstance.m_callThread)
				{
				  Monitor.Pulse(outerInstance.m_callThread);
				}
			  }
			}
			else
			{
			  try
			  {
				Thread.Sleep(50);
			  }
			  catch (InterruptedException ie)
			  {
				Console.WriteLine(ie.ToString());
				Console.Write(ie.StackTrace);
			  }
			}
		  }
		}
	  }

	  // For compatiblity with old serialized objects
	  // We will change non-serialized fields and change methods
	  // and not have this break us.
	  private const long serialVersionUID = 4618876841979251422L;

	  // For compatibility when de-serializing old objects
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void readObject(java.io.ObjectInputStream inStream) throws java.io.IOException, ClassNotFoundException
	  private void readObject(java.io.ObjectInputStream inStream)
	  {
		  inStream.defaultReadObject();

		  // Needed assignment of non-serialized fields

		  // A TransformerFactory is not guaranteed to be serializable, 
		  // so we create one here
		  m_tfactory = TransformerFactory.newInstance();
	  }
	}

}