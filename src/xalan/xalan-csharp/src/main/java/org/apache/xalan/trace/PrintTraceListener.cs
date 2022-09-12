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
 * $Id: PrintTraceListener.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{


	using Constants = org.apache.xalan.templates.Constants;
	using ElemTemplate = org.apache.xalan.templates.ElemTemplate;
	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using ElemTextLiteral = org.apache.xalan.templates.ElemTextLiteral;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;
	using SerializerTrace = org.apache.xml.serializer.SerializerTrace;

	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Implementation of the TraceListener interface that
	/// prints each event to standard out as it occurs.
	/// </summary>
	/// <seealso cref= org.apache.xalan.trace.TracerEvent
	/// @xsl.usage advanced </seealso>
	public class PrintTraceListener : TraceListenerEx3
	{

	  /// <summary>
	  /// Construct a trace listener.
	  /// </summary>
	  /// <param name="pw"> PrintWriter to use for tracing events </param>
	  public PrintTraceListener(java.io.PrintWriter pw)
	  {
		m_pw = pw;
	  }

	  /// <summary>
	  /// The print writer where the events should be written.
	  /// </summary>
	  internal java.io.PrintWriter m_pw;

	  /// <summary>
	  /// This needs to be set to true if the listener is to print an event whenever a template is invoked.
	  /// </summary>
	  public bool m_traceTemplates = false;

	  /// <summary>
	  /// Set to true if the listener is to print events that occur as each node is 'executed' in the stylesheet.
	  /// </summary>
	  public bool m_traceElements = false;

	  /// <summary>
	  /// Set to true if the listener is to print information after each result-tree generation event.
	  /// </summary>
	  public bool m_traceGeneration = false;

	  /// <summary>
	  /// Set to true if the listener is to print information after each selection event.
	  /// </summary>
	  public bool m_traceSelection = false;

	  /// <summary>
	  /// Set to true if the listener is to print information after each extension event.
	  /// </summary>
	  public bool m_traceExtension = false;

	  /// <summary>
	  /// Print information about a TracerEvent.
	  /// </summary>
	  /// <param name="ev"> the trace event. </param>
	  public virtual void _trace(TracerEvent ev)
	  {

		switch (ev.m_styleNode.XSLToken)
		{
		case Constants.ELEMNAME_TEXTLITERALRESULT :
		  if (m_traceElements)
		  {
			m_pw.print(ev.m_styleNode.SystemId + " Line #" + ev.m_styleNode.LineNumber + ", " + "Column #" + ev.m_styleNode.ColumnNumber + " -- " + ev.m_styleNode.NodeName + ": ");

			ElemTextLiteral etl = (ElemTextLiteral) ev.m_styleNode;
			string chars = new string(etl.Chars, 0, etl.Chars.Length);

			m_pw.println("    " + chars.Trim());
		  }
		  break;
		case Constants.ELEMNAME_TEMPLATE :
		  if (m_traceTemplates || m_traceElements)
		  {
			ElemTemplate et = (ElemTemplate) ev.m_styleNode;

			m_pw.print(et.SystemId + " Line #" + et.LineNumber + ", " + "Column #" + et.ColumnNumber + ": " + et.NodeName + " ");

			if (null != et.Match)
			{
			  m_pw.print("match='" + et.Match.PatternString + "' ");
			}

			if (null != et.Name)
			{
			  m_pw.print("name='" + et.Name + "' ");
			}

			m_pw.println();
		  }
		  break;
		default :
		  if (m_traceElements)
		  {
			m_pw.println(ev.m_styleNode.SystemId + " Line #" + ev.m_styleNode.LineNumber + ", " + "Column #" + ev.m_styleNode.ColumnNumber + ": " + ev.m_styleNode.NodeName);
		  }
	  break;
		}
	  }

	  internal int m_indent = 0;

	  /// <summary>
	  /// Print information about a TracerEvent.
	  /// </summary>
	  /// <param name="ev"> the trace event. </param>
	  public virtual void trace(TracerEvent ev)
	  {
	//  	m_traceElements = true;
	//  	m_traceTemplates = true;
	//  	
	//  	for(int i = 0; i < m_indent; i++)
	//  		m_pw.print(" ");
	//    m_indent = m_indent+2;
	//  	m_pw.print("trace: ");
		_trace(ev);
	  }

	  /// <summary>
	  /// Method that is called when the end of a trace event occurs.
	  /// The method is blocking.  It must return before processing continues.
	  /// </summary>
	  /// <param name="ev"> the trace event. </param>
	  public virtual void traceEnd(TracerEvent ev)
	  {
	//  	m_traceElements = true;
	//  	m_traceTemplates = true;
	//  	
	//  	m_indent = m_indent-2;
	//  	for(int i = 0; i < m_indent; i++)
	//  		m_pw.print(" ");
	//  	m_pw.print("etrac: ");
	//	_trace(ev);
	  }


	  /// <summary>
	  /// Method that is called just after a select attribute has been evaluated.
	  /// </summary>
	  /// <param name="ev"> the generate event.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void selected(SelectionEvent ev) throws javax.xml.transform.TransformerException
	public virtual void selected(SelectionEvent ev)
	{

		if (m_traceSelection)
		{
			ElemTemplateElement ete = (ElemTemplateElement) ev.m_styleNode;
			Node sourceNode = ev.m_sourceNode;

			SourceLocator locator = null;
			if (sourceNode is DTMNodeProxy)
			{
				int nodeHandler = ((DTMNodeProxy) sourceNode).DTMNodeNumber;
				locator = ((DTMNodeProxy) sourceNode).DTM.getSourceLocatorFor(nodeHandler);
			}

			if (locator != null)
			{
				m_pw.println("Selected source node '" + sourceNode.NodeName + "', at " + locator);
			}
			else
			{
				m_pw.println("Selected source node '" + sourceNode.NodeName + "'");
			}

			if (ev.m_styleNode.LineNumber == 0)
			{

				// You may not have line numbers if the selection is occuring from a
				// default template.
				ElemTemplateElement parent = (ElemTemplateElement) ete.ParentElem;

				if (parent == ete.StylesheetRoot.DefaultRootRule)
				{
					m_pw.print("(default root rule) ");
				}
				else if (parent == ete.StylesheetRoot.DefaultTextRule)
				{
					m_pw.print("(default text rule) ");
				}
				else if (parent == ete.StylesheetRoot.DefaultRule)
				{
					m_pw.print("(default rule) ");
				}

				m_pw.print(ete.NodeName + ", " + ev.m_attributeName + "='" + ev.m_xpath.PatternString + "': ");
			}
			else
			{
				m_pw.print(ev.m_styleNode.SystemId + " Line #" + ev.m_styleNode.LineNumber + ", " + "Column #" + ev.m_styleNode.ColumnNumber + ": " + ete.NodeName + ", " + ev.m_attributeName + "='" + ev.m_xpath.PatternString + "': ");
			}

			if (ev.m_selection.Type == org.apache.xpath.objects.XObject.CLASS_NODESET)
			{
				m_pw.println();

				org.apache.xml.dtm.DTMIterator nl = ev.m_selection.iter();

				// The following lines are added to fix bug#16222.
				// The main cause is that the following loop change the state of iterator, which is shared
				// with the transformer. The fix is that we record the initial state before looping, then 
				// restore the state when we finish it, which is done in the following lines added.
				int currentPos = org.apache.xml.dtm.DTM_Fields.NULL;
				currentPos = nl.CurrentPos;
				nl.ShouldCacheNodes = true; // This MUST be done before we clone the iterator!
				org.apache.xml.dtm.DTMIterator clone = null;
				// End of block

				try
				{
					clone = nl.cloneWithReset();
				}
				catch (CloneNotSupportedException)
				{
					m_pw.println("     [Can't trace nodelist because it it threw a CloneNotSupportedException]");
					return;
				}
				int pos = clone.nextNode();

				if (org.apache.xml.dtm.DTM_Fields.NULL == pos)
				{
					m_pw.println("     [empty node list]");
				}
				else
				{
					while (org.apache.xml.dtm.DTM_Fields.NULL != pos)
					{
						// m_pw.println("     " + ev.m_processor.getXPathContext().getDTM(pos).getNode(pos));
						DTM dtm = ev.m_processor.XPathContext.getDTM(pos);
						m_pw.print("     ");
						m_pw.print(pos.ToString("x"));
						m_pw.print(": ");
						m_pw.println(dtm.getNodeName(pos));
						pos = clone.nextNode();
					}
				}

				// Restore the initial state of the iterator, part of fix for bug#16222.
				nl.runTo(-1);
				nl.CurrentPos = currentPos;
				// End of fix for bug#16222

			}
			else
			{
				m_pw.println(ev.m_selection.str());
			}
		}
	}
	  /// <summary>
	  /// Method that is called after an xsl:apply-templates or xsl:for-each 
	  /// selection occurs.
	  /// </summary>
	  /// <param name="ev"> the generate event.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void selectEnd(EndSelectionEvent ev) throws javax.xml.transform.TransformerException
	  public virtual void selectEnd(EndSelectionEvent ev)
	  {
		  // Nothing for right now.
	  }


	  /// <summary>
	  /// Print information about a Generate event.
	  /// </summary>
	  /// <param name="ev"> the trace event. </param>
	  public virtual void generated(GenerateEvent ev)
	  {

		if (m_traceGeneration)
		{
		  switch (ev.m_eventtype)
		  {
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_STARTDOCUMENT :
			m_pw.println("STARTDOCUMENT");
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_ENDDOCUMENT :
			m_pw.println("ENDDOCUMENT");
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_STARTELEMENT :
			m_pw.println("STARTELEMENT: " + ev.m_name);
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_ENDELEMENT :
			m_pw.println("ENDELEMENT: " + ev.m_name);
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_CHARACTERS :
		  {
			string chars = new string(ev.m_characters, ev.m_start, ev.m_length);

			m_pw.println("CHARACTERS: " + chars);
		  }
		  break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_CDATA :
		  {
			string chars = new string(ev.m_characters, ev.m_start, ev.m_length);

			m_pw.println("CDATA: " + chars);
		  }
		  break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_COMMENT :
			m_pw.println("COMMENT: " + ev.m_data);
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_PI :
			m_pw.println("PI: " + ev.m_name + ", " + ev.m_data);
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_ENTITYREF :
			m_pw.println("ENTITYREF: " + ev.m_name);
			break;
		  case org.apache.xml.serializer.SerializerTrace_Fields.EVENTTYPE_IGNORABLEWHITESPACE :
			m_pw.println("IGNORABLEWHITESPACE");
			break;
		  }
		}
	  }

	  /// <summary>
	  /// Print information about an extension event.
	  /// </summary>
	  /// <param name="ev"> the extension event to print information about </param>
	  public virtual void extension(ExtensionEvent ev)
	  {
		if (m_traceExtension)
		{
		  switch (ev.m_callType)
		  {
			case ExtensionEvent.DEFAULT_CONSTRUCTOR:
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  m_pw.println("EXTENSION: " + ((Type)ev.m_method).FullName + "#<init>");
			  break;
			case ExtensionEvent.METHOD:
			  m_pw.println("EXTENSION: " + ((Method)ev.m_method).DeclaringClass.Name + "#" + ((Method)ev.m_method).Name);
			  break;
			case ExtensionEvent.CONSTRUCTOR:
			  m_pw.println("EXTENSION: " + ((Constructor)ev.m_method).DeclaringClass.Name + "#<init>");
			  break;
		  }
		}
	  }


	  /// <summary>
	  /// Print information about an extension event.
	  /// </summary>
	  /// <param name="ev"> the extension event to print information about </param>
	  public virtual void extensionEnd(ExtensionEvent ev)
	  {
		// do nothing
	  }

	}

}