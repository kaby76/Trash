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
 * $Id: ElemMessage.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;

	/// <summary>
	/// Implement xsl:message.
	/// <pre>
	/// <!ELEMENT xsl:message %template;>
	/// <!ATTLIST xsl:message
	///   %space-att;
	///   terminate (yes|no) "no"
	/// >
	/// </pre> </summary>
	/// <seealso cref= <a href="http://www.w3.org/TR/xslt#message">message in XSLT Specification</a>
	/// @xsl.usage advanced </seealso>
	[Serializable]
	public class ElemMessage : ElemTemplateElement
	{
		internal new const long serialVersionUID = 1530472462155060023L;

	  /// <summary>
	  /// If the terminate attribute has the value yes, then the
	  /// XSLT transformer should terminate processing after sending
	  /// the message. The default value is no.
	  /// @serial
	  /// </summary>
	  private bool m_terminate = Constants.ATTRVAL_NO; // default value

	  /// <summary>
	  /// Set the "terminate" attribute.
	  /// If the terminate attribute has the value yes, then the
	  /// XSLT transformer should terminate processing after sending
	  /// the message. The default value is no.
	  /// </summary>
	  /// <param name="v"> Value to set for "terminate" attribute.  </param>
	  public virtual bool Terminate
	  {
		  set
		  {
			m_terminate = value;
		  }
		  get
		  {
			return m_terminate;
		  }
	  }


	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref= org.apache.xalan.templates.Constants
	  /// </seealso>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_MESSAGE;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> name of the element  </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_MESSAGE_STRING;
		  }
	  }

	  /// <summary>
	  /// Send a message to diagnostics.
	  /// The xsl:message instruction sends a message in a way that
	  /// is dependent on the XSLT transformer. The content of the xsl:message
	  /// instruction is a template. The xsl:message is instantiated by
	  /// instantiating the content to create an XML fragment. This XML
	  /// fragment is the content of the message.
	  /// </summary>
	  /// <param name="transformer"> non-null reference to the the current transform-time state.
	  /// </param>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.apache.xalan.transformer.TransformerImpl transformer) throws javax.xml.transform.TransformerException
	  public override void execute(TransformerImpl transformer)
	  {

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEvent(this);
		}

		string data = transformer.transformToString(this);

		transformer.MsgMgr.message(this, data, m_terminate);

		if (m_terminate)
		{
		  transformer.ErrorListener.fatalError(new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_STYLESHEET_DIRECTED_TERMINATION, null))); //"Stylesheet directed termination"));
		}

		if (transformer.Debug)
		{
		  transformer.TraceManager.fireTraceEndEvent(this);
		}
	  }
	}

}