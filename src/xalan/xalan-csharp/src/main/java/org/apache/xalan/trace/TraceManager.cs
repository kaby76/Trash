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
 * $Id: TraceManager.java 468644 2006-10-28 06:56:42Z minchau $
 */
namespace org.apache.xalan.trace
{


	using ElemTemplateElement = org.apache.xalan.templates.ElemTemplateElement;
	using TransformerImpl = org.apache.xalan.transformer.TransformerImpl;
	using XPath = org.apache.xpath.XPath;
	using XObject = org.apache.xpath.objects.XObject;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// This class manages trace listeners, and acts as an
	/// interface for the tracing functionality in Xalan.
	/// </summary>
	public class TraceManager
	{

	  /// <summary>
	  /// A transformer instance </summary>
	  private TransformerImpl m_transformer;

	  /// <summary>
	  /// Constructor for the trace manager.
	  /// </summary>
	  /// <param name="transformer"> a non-null instance of a transformer </param>
	  public TraceManager(TransformerImpl transformer)
	  {
		m_transformer = transformer;
	  }

	  /// <summary>
	  /// List of listeners who are interested in tracing what's
	  /// being generated.
	  /// </summary>
	  private ArrayList m_traceListeners = null;

	  /// <summary>
	  /// Add a trace listener for the purposes of debugging and diagnosis. </summary>
	  /// <param name="tl"> Trace listener to be added.
	  /// </param>
	  /// <exception cref="TooManyListenersException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addTraceListener(TraceListener tl) throws java.util.TooManyListenersException
	  public virtual void addTraceListener(TraceListener tl)
	  {

		m_transformer.Debug = true;

		if (null == m_traceListeners)
		{
		  m_traceListeners = new ArrayList();
		}

		m_traceListeners.Add(tl);
	  }

	  /// <summary>
	  /// Remove a trace listener. </summary>
	  /// <param name="tl"> Trace listener to be removed. </param>
	  public virtual void removeTraceListener(TraceListener tl)
	  {

		if (null != m_traceListeners)
		{
		  m_traceListeners.Remove(tl);

		  // The following line added to fix the bug#5140: hasTraceListeners() returns true
		  // after adding and removing a listener.
		  // Check: if m_traceListeners is empty, then set it to NULL.
		  if (0 == m_traceListeners.Count)
		  {
			  m_traceListeners = null;
		  }
		}
	  }

	  /// <summary>
	  /// Fire a generate event.
	  /// </summary>
	  /// <param name="te"> Generate Event to fire </param>
	  public virtual void fireGenerateEvent(GenerateEvent te)
	  {

		if (null != m_traceListeners)
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];

			tl.generated(te);
		  }
		}
	  }

	  /// <summary>
	  /// Tell if trace listeners are present.
	  /// </summary>
	  /// <returns> True if there are trace listeners </returns>
	  public virtual bool hasTraceListeners()
	  {
		return (null != m_traceListeners);
	  }

	  /// <summary>
	  /// Fire a trace event.
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet template node </param>
	  public virtual void fireTraceEvent(ElemTemplateElement styleNode)
	  {

		if (hasTraceListeners())
		{
		  int sourceNode = m_transformer.XPathContext.CurrentNode;
		  Node source = getDOMNodeFromDTM(sourceNode);

		  fireTraceEvent(new TracerEvent(m_transformer, source, m_transformer.Mode, styleNode)); //sourceNode, mode,
		}
	  }

	  /// <summary>
	  /// Fire a end trace event, after all children of an element have been
	  /// executed.
	  /// </summary>
	  /// <param name="styleNode"> Stylesheet template node </param>
	  public virtual void fireTraceEndEvent(ElemTemplateElement styleNode)
	  {

		if (hasTraceListeners())
		{
		  int sourceNode = m_transformer.XPathContext.CurrentNode;
		  Node source = getDOMNodeFromDTM(sourceNode);

		  fireTraceEndEvent(new TracerEvent(m_transformer, source, m_transformer.Mode, styleNode)); //sourceNode, mode,
		}
	  }

	  /// <summary>
	  /// Fire a trace event.
	  /// </summary>
	  /// <param name="te"> Trace event to fire </param>
	  public virtual void fireTraceEndEvent(TracerEvent te)
	  {

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];
			if (tl is TraceListenerEx2)
			{
			  ((TraceListenerEx2)tl).traceEnd(te);
			}
		  }
		}
	  }



	  /// <summary>
	  /// Fire a trace event.
	  /// </summary>
	  /// <param name="te"> Trace event to fire </param>
	  public virtual void fireTraceEvent(TracerEvent te)
	  {

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];

			tl.trace(te);
		  }
		}
	  }

	  /// <summary>
	  /// Fire a selection event.
	  /// </summary>
	  /// <param name="sourceNode"> Current source node </param>
	  /// <param name="styleNode"> node in the style tree reference for the event. </param>
	  /// <param name="attributeName"> The attribute name from which the selection is made. </param>
	  /// <param name="xpath"> The XPath that executed the selection. </param>
	  /// <param name="selection"> The result of the selection.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireSelectedEvent(int sourceNode, org.apache.xalan.templates.ElemTemplateElement styleNode, String attributeName, org.apache.xpath.XPath xpath, org.apache.xpath.objects.XObject selection) throws javax.xml.transform.TransformerException
	  public virtual void fireSelectedEvent(int sourceNode, ElemTemplateElement styleNode, string attributeName, XPath xpath, XObject selection)
	  {

		if (hasTraceListeners())
		{
		  Node source = getDOMNodeFromDTM(sourceNode);

		  fireSelectedEvent(new SelectionEvent(m_transformer, source, styleNode, attributeName, xpath, selection));
		}
	  }

	  /// <summary>
	  /// Fire a selection event.
	  /// </summary>
	  /// <param name="sourceNode"> Current source node </param>
	  /// <param name="styleNode"> node in the style tree reference for the event. </param>
	  /// <param name="attributeName"> The attribute name from which the selection is made. </param>
	  /// <param name="xpath"> The XPath that executed the selection. </param>
	  /// <param name="selection"> The result of the selection.
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireSelectedEndEvent(int sourceNode, org.apache.xalan.templates.ElemTemplateElement styleNode, String attributeName, org.apache.xpath.XPath xpath, org.apache.xpath.objects.XObject selection) throws javax.xml.transform.TransformerException
	  public virtual void fireSelectedEndEvent(int sourceNode, ElemTemplateElement styleNode, string attributeName, XPath xpath, XObject selection)
	  {

		if (hasTraceListeners())
		{
		  Node source = getDOMNodeFromDTM(sourceNode);

		  fireSelectedEndEvent(new EndSelectionEvent(m_transformer, source, styleNode, attributeName, xpath, selection));
		}
	  }

	  /// <summary>
	  /// Fire a selection event.
	  /// </summary>
	  /// <param name="se"> Selection event to fire
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireSelectedEndEvent(EndSelectionEvent se) throws javax.xml.transform.TransformerException
	  public virtual void fireSelectedEndEvent(EndSelectionEvent se)
	  {

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];

			if (tl is TraceListenerEx)
			{
			  ((TraceListenerEx)tl).selectEnd(se);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Fire a selection event.
	  /// </summary>
	  /// <param name="se"> Selection event to fire
	  /// </param>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fireSelectedEvent(SelectionEvent se) throws javax.xml.transform.TransformerException
	  public virtual void fireSelectedEvent(SelectionEvent se)
	  {

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];

			tl.selected(se);
		  }
		}
	  }


	  /// <summary>
	  /// Fire an end extension event.
	  /// </summary>
	  /// <seealso cref= java.lang.reflect.Method#invoke
	  /// </seealso>
	  /// <param name="method"> The java method about to be executed </param>
	  /// <param name="instance"> The instance the method will be executed on </param>
	  /// <param name="arguments"> Parameters passed to the method. </param>
	  public virtual void fireExtensionEndEvent(Method method, object instance, object[] arguments)
	  {
		  ExtensionEvent ee = new ExtensionEvent(m_transformer, method, instance, arguments);

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];
			if (tl is TraceListenerEx3)
			{
			  ((TraceListenerEx3)tl).extensionEnd(ee);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Fire an end extension event.
	  /// </summary>
	  /// <seealso cref= java.lang.reflect.Method#invoke
	  /// </seealso>
	  /// <param name="method"> The java method about to be executed </param>
	  /// <param name="instance"> The instance the method will be executed on </param>
	  /// <param name="arguments"> Parameters passed to the method. </param>
	  public virtual void fireExtensionEvent(Method method, object instance, object[] arguments)
	  {
		ExtensionEvent ee = new ExtensionEvent(m_transformer, method, instance, arguments);

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];
			if (tl is TraceListenerEx3)
			{
			  ((TraceListenerEx3)tl).extension(ee);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Fire an end extension event.
	  /// </summary>
	  /// <seealso cref= java.lang.reflect.Method#invoke
	  /// </seealso>
	  /// <param name="ee"> the ExtensionEvent to fire </param>
	  public virtual void fireExtensionEndEvent(ExtensionEvent ee)
	  {
		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];
			if (tl is TraceListenerEx3)
			{
			  ((TraceListenerEx3)tl).extensionEnd(ee);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Fire an end extension event.
	  /// </summary>
	  /// <seealso cref= java.lang.reflect.Method#invoke
	  /// </seealso>
	  /// <param name="ee"> the ExtensionEvent to fire </param>
	  public virtual void fireExtensionEvent(ExtensionEvent ee)
	  {

		if (hasTraceListeners())
		{
		  int nListeners = m_traceListeners.Count;

		  for (int i = 0; i < nListeners; i++)
		  {
			TraceListener tl = (TraceListener) m_traceListeners[i];
			if (tl is TraceListenerEx3)
			{
			  ((TraceListenerEx3)tl).extension(ee);
			}
		  }
		}
	  }

	  /// <summary>
	  /// Get the DOM Node of the current XPath context, which is possibly null. </summary>
	  /// <param name="sourceNode"> the handle on the node used by a DTM. </param>
	  private Node getDOMNodeFromDTM(int sourceNode)
	  {
		org.apache.xml.dtm.DTM dtm = m_transformer.XPathContext.getDTM(sourceNode);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.w3c.dom.Node source = (dtm == null) ? null : dtm.getNode(sourceNode);
		Node source = (dtm == null) ? null : dtm.getNode(sourceNode);
		return source;
	  }
	}

}