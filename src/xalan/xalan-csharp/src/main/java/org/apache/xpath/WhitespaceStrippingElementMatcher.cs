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
 * $Id: WhitespaceStrippingElementMatcher.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{

	using Element = org.w3c.dom.Element;

	/// <summary>
	/// A class that implements this interface can tell if a given element should 
	/// strip whitespace nodes from it's children.
	/// </summary>
	public interface WhitespaceStrippingElementMatcher
	{
	  /// <summary>
	  /// Get information about whether or not an element should strip whitespace. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <param name="support"> The XPath runtime state. </param>
	  /// <param name="targetElement"> Element to check
	  /// </param>
	  /// <returns> true if the whitespace should be stripped.
	  /// </returns>
	  /// <exception cref="TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean shouldStripWhiteSpace(XPathContext support, org.w3c.dom.Element targetElement) throws javax.xml.transform.TransformerException;
	  bool shouldStripWhiteSpace(XPathContext support, Element targetElement);

	  /// <summary>
	  /// Get information about whether or not whitespace can be stripped. </summary>
	  /// <seealso cref="<a href="http://www.w3.org/TR/xslt.strip">strip in XSLT Specification</a>"
	  ////>
	  /// <returns> true if the whitespace can be stripped. </returns>
	  bool canStripWhiteSpace();
	}

}