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
 * $Id: SAXSourceLocator.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	using Locator = org.xml.sax.Locator;
	using SAXParseException = org.xml.sax.SAXParseException;
	using LocatorImpl = org.xml.sax.helpers.LocatorImpl;

	/// <summary>
	/// Class SAXSourceLocator extends org.xml.sax.helpers.LocatorImpl
	/// for the purpose of implementing the SourceLocator interface, 
	/// and thus can be both a SourceLocator and a SAX Locator.
	/// </summary>
	[Serializable]
	public class SAXSourceLocator : LocatorImpl, SourceLocator
	{
		internal const long serialVersionUID = 3181680946321164112L;
	  /// <summary>
	  /// The SAX Locator object.
	  ///  @serial
	  /// </summary>
	  internal Locator m_locator;

	  /// <summary>
	  /// Constructor SAXSourceLocator
	  /// 
	  /// </summary>
	  public SAXSourceLocator()
	  {
	  }

	  /// <summary>
	  /// Constructor SAXSourceLocator
	  /// 
	  /// </summary>
	  /// <param name="locator"> Source locator </param>
	  public SAXSourceLocator(Locator locator)
	  {
		m_locator = locator;
		this.ColumnNumber = locator.ColumnNumber;
		this.LineNumber = locator.LineNumber;
		this.PublicId = locator.PublicId;
		this.SystemId = locator.SystemId;
	  }

	  /// <summary>
	  /// Constructor SAXSourceLocator
	  /// 
	  /// </summary>
	  /// <param name="locator"> Source locator </param>
	  public SAXSourceLocator(SourceLocator locator)
	  {
		m_locator = null;
		this.ColumnNumber = locator.ColumnNumber;
		this.LineNumber = locator.LineNumber;
		this.PublicId = locator.PublicId;
		this.SystemId = locator.SystemId;
	  }


	  /// <summary>
	  /// Constructor SAXSourceLocator
	  /// 
	  /// </summary>
	  /// <param name="spe"> SAXParseException exception. </param>
	  public SAXSourceLocator(SAXParseException spe)
	  {
		this.LineNumber = spe.LineNumber;
		this.ColumnNumber = spe.ColumnNumber;
		this.PublicId = spe.PublicId;
		this.SystemId = spe.SystemId;
	  }

	  /// <summary>
	  /// Return the public identifier for the current document event.
	  /// 
	  /// <para>The return value is the public identifier of the document
	  /// entity or of the external parsed entity in which the markup
	  /// triggering the event appears.</para>
	  /// </summary>
	  /// <returns> A string containing the public identifier, or
	  ///         null if none is available. </returns>
	  /// <seealso cref= #getSystemId </seealso>
	  public virtual string PublicId
	  {
		  get
		  {
			return (null == m_locator) ? base.PublicId : m_locator.PublicId;
		  }
	  }

	  /// <summary>
	  /// Return the system identifier for the current document event.
	  /// 
	  /// <para>The return value is the system identifier of the document
	  /// entity or of the external parsed entity in which the markup
	  /// triggering the event appears.</para>
	  /// 
	  /// <para>If the system identifier is a URL, the parser must resolve it
	  /// fully before passing it to the application.</para>
	  /// </summary>
	  /// <returns> A string containing the system identifier, or null
	  ///         if none is available. </returns>
	  /// <seealso cref= #getPublicId </seealso>
	  public virtual string SystemId
	  {
		  get
		  {
			return (null == m_locator) ? base.SystemId : m_locator.SystemId;
		  }
	  }

	  /// <summary>
	  /// Return the line number where the current document event ends.
	  /// 
	  /// <para><strong>Warning:</strong> The return value from the method
	  /// is intended only as an approximation for the sake of error
	  /// reporting; it is not intended to provide sufficient information
	  /// to edit the character content of the original XML document.</para>
	  /// 
	  /// <para>The return value is an approximation of the line number
	  /// in the document entity or external parsed entity where the
	  /// markup triggering the event appears.</para>
	  /// </summary>
	  /// <returns> The line number, or -1 if none is available. </returns>
	  /// <seealso cref= #getColumnNumber </seealso>
	  public virtual int LineNumber
	  {
		  get
		  {
			return (null == m_locator) ? base.LineNumber : m_locator.LineNumber;
		  }
	  }

	  /// <summary>
	  /// Return the column number where the current document event ends.
	  /// 
	  /// <para><strong>Warning:</strong> The return value from the method
	  /// is intended only as an approximation for the sake of error
	  /// reporting; it is not intended to provide sufficient information
	  /// to edit the character content of the original XML document.</para>
	  /// 
	  /// <para>The return value is an approximation of the column number
	  /// in the document entity or external parsed entity where the
	  /// markup triggering the event appears.</para>
	  /// </summary>
	  /// <returns> The column number, or -1 if none is available. </returns>
	  /// <seealso cref= #getLineNumber </seealso>
	  public virtual int ColumnNumber
	  {
		  get
		  {
			return (null == m_locator) ? base.ColumnNumber : m_locator.ColumnNumber;
		  }
	  }
	}

}