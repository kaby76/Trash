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
 * $Id: WhitespaceInfoPaths.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{

	using Stylesheet = org.apache.xalan.templates.Stylesheet;
	using WhiteSpaceInfo = org.apache.xalan.templates.WhiteSpaceInfo;

	[Serializable]
	public class WhitespaceInfoPaths : WhiteSpaceInfo
	{
		internal new const long serialVersionUID = 5954766719577516723L;

	  /// <summary>
	  /// Bean property to allow setPropertiesFromAttributes to
	  /// get the elements attribute.
	  /// </summary>
	  private ArrayList m_elements;

	  /// <summary>
	  /// Set from the elements attribute.  This is a list of 
	  /// whitespace delimited element qualified names that specify
	  /// preservation of whitespace.
	  /// </summary>
	  /// <param name="elems"> Should be a non-null reference to a list 
	  ///              of <seealso cref="org.apache.xpath.XPath"/> objects. </param>
	  public virtual ArrayList Elements
	  {
		  set
		  {
			m_elements = value;
		  }
		  get
		  {
			return m_elements;
		  }
	  }


	  public virtual void clearElements()
	  {
		  m_elements = null;
	  }

	 /// <summary>
	 /// Constructor WhitespaceInfoPaths
	 /// </summary>
	 /// <param name="thisSheet"> The current stylesheet </param>
	  public WhitespaceInfoPaths(Stylesheet thisSheet) : base(thisSheet)
	  {
		  Stylesheet = thisSheet;
	  }


	}


}