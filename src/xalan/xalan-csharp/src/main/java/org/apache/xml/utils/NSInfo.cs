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
 * $Id: NSInfo.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{

	/// <summary>
	/// This class holds information about the namespace info
	/// of a node.  It is used to optimize namespace lookup in
	/// a generic DOM.
	/// @xsl.usage internal
	/// </summary>
	public class NSInfo
	{

	  /// <summary>
	  /// Constructor NSInfo
	  /// 
	  /// </summary>
	  /// <param name="hasProcessedNS"> Flag indicating whether namespaces
	  /// have been processed for this node </param>
	  /// <param name="hasXMLNSAttrs"> Flag indicating whether this node
	  /// has XMLNS attributes.  </param>
	  public NSInfo(bool hasProcessedNS, bool hasXMLNSAttrs)
	  {

		m_hasProcessedNS = hasProcessedNS;
		m_hasXMLNSAttrs = hasXMLNSAttrs;
		m_namespace = null;
		m_ancestorHasXMLNSAttrs = ANCESTORXMLNSUNPROCESSED;
	  }

	  // Unused at the moment

	  /// <summary>
	  /// Constructor NSInfo
	  /// 
	  /// </summary>
	  /// <param name="hasProcessedNS"> Flag indicating whether namespaces
	  /// have been processed for this node </param>
	  /// <param name="hasXMLNSAttrs"> Flag indicating whether this node
	  /// has XMLNS attributes. </param>
	  /// <param name="ancestorHasXMLNSAttrs"> Flag indicating whether one of this node's
	  /// ancestor has XMLNS attributes. </param>
	  public NSInfo(bool hasProcessedNS, bool hasXMLNSAttrs, int ancestorHasXMLNSAttrs)
	  {

		m_hasProcessedNS = hasProcessedNS;
		m_hasXMLNSAttrs = hasXMLNSAttrs;
		m_ancestorHasXMLNSAttrs = ancestorHasXMLNSAttrs;
		m_namespace = null;
	  }

	  /// <summary>
	  /// Constructor NSInfo
	  /// 
	  /// </summary>
	  /// <param name="namespace"> The namespace URI </param>
	  /// <param name="hasXMLNSAttrs"> Flag indicating whether this node
	  /// has XMLNS attributes. </param>
	  public NSInfo(string @namespace, bool hasXMLNSAttrs)
	  {

		m_hasProcessedNS = true;
		m_hasXMLNSAttrs = hasXMLNSAttrs;
		m_namespace = @namespace;
		m_ancestorHasXMLNSAttrs = ANCESTORXMLNSUNPROCESSED;
	  }

	  /// <summary>
	  /// The namespace URI </summary>
	  public string m_namespace;

	  /// <summary>
	  /// Flag indicating whether this node has an XMLNS attribute </summary>
	  public bool m_hasXMLNSAttrs;

	  /// <summary>
	  /// Flag indicating whether namespaces have been processed for this node </summary>
	  public bool m_hasProcessedNS;

	  /// <summary>
	  /// Flag indicating whether one of this node's ancestor has an XMLNS attribute </summary>
	  public int m_ancestorHasXMLNSAttrs;

	  /// <summary>
	  /// Constant for ancestors XMLNS atributes not processed </summary>
	  public const int ANCESTORXMLNSUNPROCESSED = 0;

	  /// <summary>
	  /// Constant indicating an ancestor has an XMLNS attribute </summary>
	  public const int ANCESTORHASXMLNS = 1;

	  /// <summary>
	  /// Constant indicating ancestors don't have an XMLNS attribute </summary>
	  public const int ANCESTORNOXMLNS = 2;
	}

}