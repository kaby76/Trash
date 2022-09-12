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
 * $Id: ListingErrorHandler.java 468655 2006-10-28 07:12:06Z minchau $
 */

namespace org.apache.xml.utils
{



	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;

	using ErrorHandler = org.xml.sax.ErrorHandler;
	using SAXException = org.xml.sax.SAXException;
	using SAXParseException = org.xml.sax.SAXParseException;


	/// <summary>
	/// Sample implementation of similar SAX ErrorHandler and JAXP ErrorListener.  
	/// 
	/// <para>This implementation is suitable for various use cases, and 
	/// provides some basic configuration API's as well to control 
	/// when we re-throw errors, etc.</para>
	/// 
	/// @author shane_curcuru@us.ibm.com
	/// @version $Id: ListingErrorHandler.java 468655 2006-10-28 07:12:06Z minchau $
	/// @xsl.usage general
	/// </summary>
	public class ListingErrorHandler : ErrorHandler, ErrorListener
	{
		protected internal PrintWriter m_pw = null;


		/// <summary>
		/// Constructor ListingErrorHandler; user-supplied PrintWriter.  
		/// </summary>
		public ListingErrorHandler(PrintWriter pw)
		{
			if (null == pw)
			{
				throw new System.NullReferenceException(XMLMessages.createXMLMessage(XMLErrorResources.ER_ERRORHANDLER_CREATED_WITH_NULL_PRINTWRITER, null));
			}
				// "ListingErrorHandler created with null PrintWriter!");

			m_pw = pw;
		}

		/// <summary>
		/// Constructor ListingErrorHandler; uses System.err.  
		/// </summary>
		public ListingErrorHandler()
		{
			m_pw = new PrintWriter(System.err, true);
		}


		/* ======== Implement org.xml.sax.ErrorHandler ======== */
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
		/// 
		/// <para>Filters may use this method to report other, non-XML warnings
		/// as well.</para>
		/// </summary>
		/// <param name="exception"> The warning information encapsulated in a
		///                  SAX parse exception. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		/// wrapping another exception; only if setThrowOnWarning is true. </exception>
		/// <seealso cref= org.xml.sax.SAXParseException  </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
		public virtual void warning(SAXParseException exception)
		{
			logExceptionLocation(m_pw, exception);
			// Note: should we really call .toString() below, since 
			//  sometimes the message is not properly set?
			m_pw.println("warning: " + exception.Message);
			m_pw.flush();

			if (ThrowOnWarning)
			{
				throw exception;
			}
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
		/// 
		/// <para>Filters may use this method to report other, non-XML errors
		/// as well.</para>
		/// </summary>
		/// <param name="exception"> The error information encapsulated in a
		///                  SAX parse exception. </param>
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		/// wrapping another exception; only if setThrowOnErroris true. </exception>
		/// <seealso cref= org.xml.sax.SAXParseException  </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
		public virtual void error(SAXParseException exception)
		{
			logExceptionLocation(m_pw, exception);
			m_pw.println("error: " + exception.Message);
			m_pw.flush();

			if (ThrowOnError)
			{
				throw exception;
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
		/// <exception cref="org.xml.sax.SAXException"> Any SAX exception, possibly
		/// wrapping another exception; only if setThrowOnFatalError is true. </exception>
		/// <seealso cref= org.xml.sax.SAXParseException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException exception) throws org.xml.sax.SAXException
		public virtual void fatalError(SAXParseException exception)
		{
			logExceptionLocation(m_pw, exception);
			m_pw.println("fatalError: " + exception.Message);
			m_pw.flush();

			if (ThrowOnFatalError)
			{
				throw exception;
			}
		}


		/* ======== Implement javax.xml.transform.ErrorListener ======== */

		/// <summary>
		/// Receive notification of a warning.
		/// 
		/// <para><seealso cref="javax.xml.transform.Transformer"/> can use this method to report
		/// conditions that are not errors or fatal errors.  The default behaviour
		/// is to take no action.</para>
		/// 
		/// <para>After invoking this method, the Transformer must continue with
		/// the transformation. It should still be possible for the
		/// application to process the document through to the end.</para>
		/// </summary>
		/// <param name="exception"> The warning information encapsulated in a
		///                  transformer exception.
		/// </param>
		/// <exception cref="javax.xml.transform.TransformerException">  only if 
		/// setThrowOnWarning is true.
		/// </exception>
		/// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void warning(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
		public virtual void warning(TransformerException exception)
		{
			logExceptionLocation(m_pw, exception);
			m_pw.println("warning: " + exception.Message);
			m_pw.flush();

			if (ThrowOnWarning)
			{
				throw exception;
			}
		}

		/// <summary>
		/// Receive notification of a recoverable error.
		/// 
		/// <para>The transformer must continue to try and provide normal transformation
		/// after invoking this method.  It should still be possible for the
		/// application to process the document through to the end if no other errors
		/// are encountered.</para>
		/// </summary>
		/// <param name="exception"> The error information encapsulated in a
		///                  transformer exception.
		/// </param>
		/// <exception cref="javax.xml.transform.TransformerException">  only if 
		/// setThrowOnError is true.
		/// </exception>
		/// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void error(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
		public virtual void error(TransformerException exception)
		{
			logExceptionLocation(m_pw, exception);
			m_pw.println("error: " + exception.Message);
			m_pw.flush();

			if (ThrowOnError)
			{
				throw exception;
			}
		}

		/// <summary>
		/// Receive notification of a non-recoverable error.
		/// 
		/// <para>The transformer must continue to try and provide normal transformation
		/// after invoking this method.  It should still be possible for the
		/// application to process the document through to the end if no other errors
		/// are encountered, but there is no guarantee that the output will be
		/// useable.</para>
		/// </summary>
		/// <param name="exception"> The error information encapsulated in a
		///                  transformer exception.
		/// </param>
		/// <exception cref="javax.xml.transform.TransformerException">  only if 
		/// setThrowOnError is true.
		/// </exception>
		/// <seealso cref= javax.xml.transform.TransformerException </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fatalError(javax.xml.transform.TransformerException exception) throws javax.xml.transform.TransformerException
		public virtual void fatalError(TransformerException exception)
		{
			logExceptionLocation(m_pw, exception);
			m_pw.println("error: " + exception.Message);
			m_pw.flush();

			if (ThrowOnError)
			{
				throw exception;
			}
		}



		/* ======== Implement worker methods ======== */


		/// <summary>
		/// Print out location information about the exception.  
		/// 
		/// Cribbed from DefaultErrorHandler.printLocation() </summary>
		/// <param name="pw"> PrintWriter to send output to </param>
		/// <param name="exception"> TransformerException or SAXParseException
		/// to log information about </param>
		public static void logExceptionLocation(PrintWriter pw, Exception exception)
		{
			if (null == pw)
			{
				pw = new PrintWriter(System.err, true);
			}

			SourceLocator locator = null;
			Exception cause = exception;

			// Try to find the locator closest to the cause.
			do
			{
				// Find the current locator, if one present
				if (cause is SAXParseException)
				{
					// A SAXSourceLocator is a Xalan helper class 
					//  that implements both a SourceLocator and a SAX Locator
					//@todo check that the new locator actually has 
					//  as much or more information as the 
					//  current one already does
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

				// Then walk back down the chain of exceptions
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

			// Formatting note: mimic javac-like errors:
			//  path\filename:123: message-here
			//  systemId:L=1;C=2: message-here
			if (null != locator)
			{
				string id = (locator.PublicId != locator.PublicId) ? locator.PublicId : (null != locator.SystemId) ? locator.SystemId : "SystemId-Unknown";

				pw.print(id + ":Line=" + locator.LineNumber + ";Column=" + locator.ColumnNumber + ": ");
				pw.println("exception:" + exception.Message);
				pw.println("root-cause:" + ((null != cause) ? cause.Message : "null"));
				logSourceLine(pw, locator);
			}
			else
			{
				pw.print("SystemId-Unknown:locator-unavailable: ");
				pw.println("exception:" + exception.Message);
				pw.println("root-cause:" + ((null != cause) ? cause.Message : "null"));
			}
		}


		/// <summary>
		/// Print out the specific source line that caused the exception, 
		/// if possible to load it.  
		/// </summary>
		/// <param name="pw"> PrintWriter to send output to </param>
		/// <param name="locator"> Xalan wrapper for either a JAXP or a SAX 
		/// source location object </param>
		public static void logSourceLine(PrintWriter pw, SourceLocator locator)
		{
			if (null == locator)
			{
				return;
			}

			if (null == pw)
			{
				pw = new PrintWriter(System.err, true);
			}

			string url = locator.SystemId;
			// Bail immediately if we get SystemId-Unknown
			//@todo future improvement: attempt to get resource 
			//  from a publicId if possible
			if (null == url)
			{
				pw.println("line: (No systemId; cannot read file)");
				pw.println();
				return;
			}

			//@todo attempt to get DOM backpointer or other ids

			try
			{
				int line = locator.LineNumber;
				int column = locator.ColumnNumber;
				pw.println("line: " + getSourceLine(url, line));
				StringBuilder buf = new StringBuilder("line: ");
				for (int i = 1; i < column; i++)
				{
					buf.Append(' ');
				}
				buf.Append('^');
				pw.println(buf.ToString());
			}
			catch (Exception e)
			{
				pw.println("line: logSourceLine unavailable due to: " + e.Message);
				pw.println();
			}
		}


		/// <summary>
		/// Return the specific source line that caused the exception, 
		/// if possible to load it; allow exceptions to be thrown.  
		/// 
		/// @author shane_curcuru@us.ibm.com
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static String getSourceLine(String sourceUrl, int lineNum) throws Exception
		protected internal static string getSourceLine(string sourceUrl, int lineNum)
		{
			URL url = null;
			// Get a URL from the sourceUrl
			try
			{
				// Try to get a URL from it as-is
				url = new URL(sourceUrl);
			}
			catch (java.net.MalformedURLException mue)
			{
				int indexOfColon = sourceUrl.IndexOf(':');
				int indexOfSlash = sourceUrl.IndexOf('/');

				if ((indexOfColon != -1) && (indexOfSlash != -1) && (indexOfColon < indexOfSlash))
				{
					// The url is already absolute, but we could not get 
					//  the system to form it, so bail
					throw mue;
				}
				else
				{
					// The url is relative, so attempt to get absolute
					url = new URL(SystemIDResolver.getAbsoluteURI(sourceUrl));
					// If this fails, allow the exception to propagate
				}
			}

			string line = null;
			System.IO.Stream @is = null;
			System.IO.StreamReader br = null;
			try
			{
				// Open the URL and read to our specified line
				URLConnection uc = url.openConnection();
				@is = uc.InputStream;
				br = new System.IO.StreamReader(@is);

				// Not the most efficient way, but it works
				// (Feel free to patch to seek to the appropriate line)
				for (int i = 1; i <= lineNum; i++)
				{
					line = br.ReadLine();
				}

			}
			// Allow exceptions to propagate from here, but ensure 
			//  streams are closed!
			finally
			{
				br.Close();
				@is.Close();
			}

			// Return whatever we found
			return line;
		}


		/* ======== Implement settable properties ======== */

		/// <summary>
		/// User-settable behavior: when to re-throw exceptions.  
		/// 
		/// <para>This allows per-instance configuration of 
		/// ListingErrorHandlers.  You can ask us to either throw 
		/// an exception when we're called for various warning / 
		/// error / fatalErrors, or simply log them and continue.</para>
		/// </summary>
		/// <param name="b"> if we should throw an exception on warnings </param>
		public virtual bool ThrowOnWarning
		{
			set
			{
				throwOnWarning = value;
			}
			get
			{
				return throwOnWarning;
			}
		}


		/// <summary>
		/// If we should throw exception on warnings; default:false. </summary>
		protected internal bool throwOnWarning = false;


		/// <summary>
		/// User-settable behavior: when to re-throw exceptions.  
		/// 
		/// <para>This allows per-instance configuration of 
		/// ListingErrorHandlers.  You can ask us to either throw 
		/// an exception when we're called for various warning / 
		/// error / fatalErrors, or simply log them and continue.</para>
		/// 
		/// <para>Note that the behavior of many parsers/transformers 
		/// after an error is not necessarily defined!</para>
		/// </summary>
		/// <param name="b"> if we should throw an exception on errors </param>
		public virtual bool ThrowOnError
		{
			set
			{
				throwOnError = value;
			}
			get
			{
				return throwOnError;
			}
		}


		/// <summary>
		/// If we should throw exception on errors; default:true. </summary>
		protected internal bool throwOnError = true;


		/// <summary>
		/// User-settable behavior: when to re-throw exceptions.  
		/// 
		/// <para>This allows per-instance configuration of 
		/// ListingErrorHandlers.  You can ask us to either throw 
		/// an exception when we're called for various warning / 
		/// error / fatalErrors, or simply log them and continue.</para>
		/// 
		/// <para>Note that the behavior of many parsers/transformers 
		/// after a fatalError is not necessarily defined, most 
		/// products will probably barf if you continue.</para>
		/// </summary>
		/// <param name="b"> if we should throw an exception on fatalErrors </param>
		public virtual bool ThrowOnFatalError
		{
			set
			{
				throwOnFatalError = value;
			}
			get
			{
				return throwOnFatalError;
			}
		}


		/// <summary>
		/// If we should throw exception on fatalErrors; default:true. </summary>
		protected internal bool throwOnFatalError = true;

	}

}