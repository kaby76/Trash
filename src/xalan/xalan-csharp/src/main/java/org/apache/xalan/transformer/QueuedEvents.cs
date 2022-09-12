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
 * $Id: QueuedEvents.java 468645 2006-10-28 06:57:24Z minchau $
 */
namespace org.apache.xalan.transformer
{

	using MutableAttrListImpl = org.apache.xml.utils.MutableAttrListImpl;


	/// <summary>
	/// This class acts as a base for ResultTreeHandler, and keeps
	/// queud stack events.  In truth, we don't need a stack,
	/// so I may change this down the line a bit.
	/// </summary>
	public abstract class QueuedEvents
	{

	  /// <summary>
	  /// The number of events queued </summary>
	  protected internal int m_eventCount = 0;

	  /// <summary>
	  /// Queued start document </summary>
	  // QueuedStartDocument m_startDoc = new QueuedStartDocument();

	  /// <summary>
	  /// Queued start element </summary>
	  // QueuedStartElement m_startElement = new QueuedStartElement();

	  public bool m_docPending = false;
	  protected internal bool m_docEnded = false;

	  /// <summary>
	  /// Flag indicating that an event is pending.  Public for 
	  ///  fast access by ElemForEach.         
	  /// </summary>
	  public bool m_elemIsPending = false;

	  /// <summary>
	  /// Flag indicating that an event is ended </summary>
	  public bool m_elemIsEnded = false;

	  /// <summary>
	  /// The pending attributes.  We have to delay the call to
	  /// m_flistener.startElement(name, atts) because of the
	  /// xsl:attribute and xsl:copy calls.  In other words,
	  /// the attributes have to be fully collected before you
	  /// can call startElement.
	  /// </summary>
	  protected internal MutableAttrListImpl m_attributes = new MutableAttrListImpl();

	  /// <summary>
	  /// Flag to try and get the xmlns decls to the attribute list
	  /// before other attributes are added.
	  /// </summary>
	  protected internal bool m_nsDeclsHaveBeenAdded = false;

	  /// <summary>
	  /// The pending element, namespace, and local name.
	  /// </summary>
	  protected internal string m_name;

	  /// <summary>
	  /// Namespace URL of the element </summary>
	  protected internal string m_url;

	  /// <summary>
	  /// Local part of qualified name of the element </summary>
	  protected internal string m_localName;


	  /// <summary>
	  /// Vector of namespaces for this element </summary>
	  protected internal ArrayList m_namespaces = null;

	//  /**
	//   * Get the queued element.
	//   *
	//   * @return the queued element.
	//   */
	//  QueuedStartElement getQueuedElem()
	//  {
	//    return (m_eventCount > 1) ? m_startElement : null;
	//  }

	  /// <summary>
	  /// To re-initialize the document and element events 
	  /// 
	  /// </summary>
	  protected internal virtual void reInitEvents()
	  {
	  }

	  /// <summary>
	  /// Push document event and re-initialize events  
	  /// 
	  /// </summary>
	  public virtual void reset()
	  {
		pushDocumentEvent();
		reInitEvents();
	  }

	  /// <summary>
	  /// Push the document event.  This never gets popped.
	  /// </summary>
	  internal virtual void pushDocumentEvent()
	  {

		// m_startDoc.setPending(true);
		// initQSE(m_startDoc);
		m_docPending = true;

		m_eventCount++;
	  }

	  /// <summary>
	  /// Pop element event 
	  /// 
	  /// </summary>
	  internal virtual void popEvent()
	  {
		m_elemIsPending = false;
		m_attributes.clear();

		m_nsDeclsHaveBeenAdded = false;
		m_name = null;
		m_url = null;
		m_localName = null;
		m_namespaces = null;

		m_eventCount--;
	  }

	  /// <summary>
	  /// Instance of a serializer </summary>
	  private org.apache.xml.serializer.Serializer m_serializer;

	  /// <summary>
	  /// This is only for use of object pooling, so that
	  /// it can be reset.
	  /// </summary>
	  /// <param name="s"> non-null instance of a serializer  </param>
	  internal virtual org.apache.xml.serializer.Serializer Serializer
	  {
		  set
		  {
			m_serializer = value;
		  }
		  get
		  {
			return m_serializer;
		  }
	  }

	}

}