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
 * $Id: ElemOtherwise.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	/// <summary>
	/// Implement xsl:otherwise.
	/// <pre>
	/// <!ELEMENT xsl:otherwise %template;>
	/// <!ATTLIST xsl:otherwise %space-att;>
	/// </pre> </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.section-Conditional-Processing-with-xsl:choose">XXX in XSLT Specification</a>"
	/// @xsl.usage advanced/>
	[Serializable]
	public class ElemOtherwise : ElemTemplateElement
	{
		internal new const long serialVersionUID = 1863944560970181395L;

	  /// <summary>
	  /// Get an int constant identifying the type of element. </summary>
	  /// <seealso cref="org.apache.xalan.templates.Constants"
	  ////>
	  /// <returns> The token ID for this element </returns>
	  public override int XSLToken
	  {
		  get
		  {
			return Constants.ELEMNAME_OTHERWISE;
		  }
	  }

	  /// <summary>
	  /// Return the node name.
	  /// </summary>
	  /// <returns> The element's name </returns>
	  public override string NodeName
	  {
		  get
		  {
			return Constants.ELEMNAME_OTHERWISE_STRING;
		  }
	  }
	}

}