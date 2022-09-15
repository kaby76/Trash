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
 * $Id: DefaultErrorHandler.java 524806 2007-04-02 15:51:39Z zongaro $
 */
namespace org.apache.xml.utils
{



	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;


	/// <summary>
	/// Implement SAX error handler for default reporting.
	/// @xsl.usage general
	/// </summary>
	public class DefaultErrorHandler : ErrorHandler, ErrorListener
	{
	  internal PrintWriter m_pw;

	  /// <summary>
	  /// if this flag is set to true, we will rethrow the exception on
	  /// the error() and fatalError() methods. If it is false, the errors 
	  /// are reported to System.err. 
	  /// </summary>
	  internal bool m_throwExceptionOnError = true;

	  /// <summary>
	  /// Constructor DefaultErrorHandler
	  /// </summary>
	  public DefaultErrorHandler(PrintWriter pw)
	  {
		m_pw = pw;
	  }

	  /// <summary>
	  /// Constructor DefaultErrorHandler
	  /// </summary>
	  public DefaultErrorHandler(PrintStream pw)
	  {
		m_pw = new PrintWriter(pw, true);
	  }

	  /// <summary>
	  /// Constructor DefaultErrorHandler
	  /// </summary>
	  public DefaultErrorHandler() : this(true)
	  {
	  }

	  /// <summary>
	  /// Constructor DefaultErrorHandler
	  /// </summary>
	  public DefaultErrorHandler(bool throwExceptionOnError)
	  {
		// Defer creation of a PrintWriter until it's actually needed
		m_throwExceptionOnError = throwExceptionOnError;
	  }

	  /// <summary>
	  /// Retrieve <code>java.io.PrintWriter</code> to which errors are being
	  /// directed. </summary>
	  /// <returns> The <code>PrintWriter</code> installed via the constructor
	  ///         or the default <code>PrintWriter</code> </returns>
	  public virtual PrintWriter ErrorWriter
	  {
		  get
		  {
			// Defer creating the java.io.PrintWriter until an error needs to be
			// reported.
			if (m_pw == null)
			{
			  m_pw = new PrintWriter(System.err, true);
			}
			return m_pw;
		  }
	  }

	  /// <summary>
	  /// Receive notification of a warning.
	  /// 
	  /// <para>SAX parsers will use this method to report conditions that
	  /// are not errors or fatal errors as defined by the XML 1.0
	  /// recommendation.  The default behaviour is to take no action.</para>
	  /// 
	  /// <para>The SAX parser must continue to provide normal parsing events
	  /// after invoking this method: it should still be possible for the
	  /// application to process the document through to the end.</para>
	  /// </summary>
	  /// <param name="exception"> The warning information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void warning(SAXParseException exception)
	  {
		PrintWriter pw = ErrorWriter;

		printLocation(pw, exception);
		pw.println("Parser warning: " + exception.Message);
	  }

	  /// <summary>
	  /// Receive notification of a recoverable error.
	  /// 
	  /// <para>This corresponds to the definition of "error" in section 1.2
	  /// of the W3C XML 1.0 Recommendation.  For example, a validating
	  /// parser would use this callback to report the violation of a
	  /// validity constraint.  The default behaviour is to take no
	  /// action.</para>
	  /// 
	  /// <para>The SAX parser must continue to provide normal parsing events
	  /// after invoking this method: it should still be possible for the
	  /// application to process the document through to the end.  If the
	  /// application cannot do so, then the parser should report a fatal
	  /// error even if the XML 1.0 recommendation does not require it to
	  /// do so.</para>
	  /// </summary>
	  /// <param name="exception"> The error information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void error(SAXParseException exception)
	  {
		//printLocation(exception);
		// getErrorWriter().println(exception.getMessage());

		throw exception;
	  }

	  /// <summary>
	  /// Receive notification of a non-recoverable error.
	  /// 
	  /// <para>This corresponds to the definition of "fatal error" in
	  /// section 1.2 of the W3C XML 1.0 Recommendation.  For example, a
	  /// parser would use this callback to report the violation of a
	  /// well-formedness constraint.</para>
	  /// 
	  /// <para>The application must assume that the document is unusable
	  /// after the parser has invoked this method, and should continue
	  /// (if at all) only for the sake of collecting addition error
	  /// messages: in fact, SAX parsers are free to stop reporting any
	  /// other events once this method has been invoked.</para>
	  /// </summary>
	  /// <param name="exception"> The error information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="SAXException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
	  public virtual void fatalError(SAXParseException exception)
	  {
		// printLocation(exception);
		// getErrorWriter().println(exception.getMessage());

		throw exception;
	  }

	  /// <summary>
	  /// Receive notification of a warning.
	  /// 
	  /// <para>SAX parsers will use this method to report conditions that
	  /// are not errors or fatal errors as defined by the XML 1.0
	  /// recommendation.  The default behaviour is to take no action.</para>
	  /// 
	  /// <para>The SAX parser must continue to provide normal parsing events
	  /// after invoking this method: it should still be possible for the
	  /// application to process the document through to the end.</para>
	  /// </summary>
	  /// <param name="exception"> The warning information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
	  public virtual void warning(TransformerException exception)
	  {
		PrintWriter pw = ErrorWriter;

		printLocation(pw, exception);
		pw.println(exception.Message);
	  }

	  /// <summary>
	  /// Receive notification of a recoverable error.
	  /// 
	  /// <para>This corresponds to the definition of "error" in section 1.2
	  /// of the W3C XML 1.0 Recommendation.  For example, a validating
	  /// parser would use this callback to report the violation of a
	  /// validity constraint.  The default behaviour is to take no
	  /// action.</para>
	  /// 
	  /// <para>The SAX parser must continue to provide normal parsing events
	  /// after invoking this method: it should still be possible for the
	  /// application to process the document through to the end.  If the
	  /// application cannot do so, then the parser should report a fatal
	  /// error even if the XML 1.0 recommendation does not require it to
	  /// do so.</para>
	  /// </summary>
	  /// <param name="exception"> The error information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
	  public virtual void error(TransformerException exception)
	  {
		// If the m_throwExceptionOnError flag is true, rethrow the exception.
		// Otherwise report the error to System.err.
		if (m_throwExceptionOnError)
		{
		  throw exception;
		}
		else
		{
		  PrintWriter pw = ErrorWriter;

		  printLocation(pw, exception);
		  pw.println(exception.Message);
		}
	  }

	  /// <summary>
	  /// Receive notification of a non-recoverable error.
	  /// 
	  /// <para>This corresponds to the definition of "fatal error" in
	  /// section 1.2 of the W3C XML 1.0 Recommendation.  For example, a
	  /// parser would use this callback to report the violation of a
	  /// well-formedness constraint.</para>
	  /// 
	  /// <para>The application must assume that the document is unusable
	  /// after the parser has invoked this method, and should continue
	  /// (if at all) only for the sake of collecting addition error
	  /// messages: in fact, SAX parsers are free to stop reporting any
	  /// other events once this method has been invoked.</para>
	  /// </summary>
	  /// <param name="exception"> The error information encapsulated in a
	  ///                  SAX parse exception. </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> Any SAX exception, possibly
	  ///            wrapping another exception. </exception>
	  /// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
	  public virtual void fatalError(TransformerException exception)
	  {
		// If the m_throwExceptionOnError flag is true, rethrow the exception.
		// Otherwise report the error to System.err.
		if (m_throwExceptionOnError)
		{
		  throw exception;
		}
		else
		{
		  PrintWriter pw = ErrorWriter;

		  printLocation(pw, exception);
		  pw.println(exception.Message);
		}
	  }

	  public static void ensureLocationSet(TransformerException exception)
	  {
		// SourceLocator locator = exception.getLocator();
		SourceLocator locator = null;
		Exception cause = exception;

		// Try to find the locator closest to the cause.
		do
		{
		  if (cause is SAXParseException)
		  {
			locator = new SAXSourceLocator((SAXParseException)cause);
		  }
		  else if (cause is TransformerException)
		  {
			SourceLocator causeLocator = ((TransformerException)cause).Locator;
			if (null != causeLocator)
			{
			  locator = causeLocator;
			}
		  }

		  if (cause is TransformerException)
		  {
			cause = ((TransformerException)cause).InnerException;
		  }
		  else if (cause is SAXException)
		  {
			cause = ((SAXException)cause).Exception;
		  }
		  else
		  {
			cause = null;
		  }
		} while (null != cause);

		exception.Locator = locator;
	  }

	  public static void printLocation(PrintStream pw, TransformerException exception)
	  {
		printLocation(new PrintWriter(pw), exception);
	  }

	  public static void printLocation(PrintStream pw, SAXParseException exception)
	  {
		printLocation(new PrintWriter(pw), exception);
	  }

	  public static void printLocation(PrintWriter pw, Exception exception)
	  {
		SourceLocator locator = null;
		Exception cause = exception;

		// Try to find the locator closest to the cause.
		do
		{
		  if (cause is SAXParseException)
		  {
			locator = new SAXSourceLocator((SAXParseException)cause);
		  }
		  else if (cause is TransformerException)
		  {
			SourceLocator causeLocator = ((TransformerException)cause).Locator;
			if (null != causeLocator)
			{
			  locator = causeLocator;
			}
		  }
		  if (cause is TransformerException)
		  {
			cause = ((TransformerException)cause).InnerException;
		  }
		  else if (cause is WrappedRuntimeException)
		  {
			cause = ((WrappedRuntimeException)cause).Exception;
		  }
		  else if (cause is SAXException)
		  {
			cause = ((SAXException)cause).Exception;
		  }
		  else
		  {
			cause = null;
		  }
		} while (null != cause);

		if (null != locator)
		{
		  // getErrorWriter().println("Parser fatal error: "+exception.getMessage());
		  string id = (null != locator.PublicId) ? locator.PublicId : (null != locator.SystemId) ? locator.SystemId : XMLMessages.createXMLMessage(XMLErrorResources.ER_SYSTEMID_UNKNOWN, null); //"SystemId Unknown";

		  pw.print(id + "; " + XMLMessages.createXMLMessage("line", null) + locator.LineNumber + "; " + XMLMessages.createXMLMessage("column", null) + locator.ColumnNumber + "; ");
		}
		else
		{
		  pw.print("(" + XMLMessages.createXMLMessage(XMLErrorResources.ER_LOCATION_UNKNOWN, null) + ")");
		}
	  }
	}

}