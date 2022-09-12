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
 * $Id: XPathProcessorException.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{

	/// <summary>
	/// Derived from XPathException in order that XPath processor
	/// exceptions may be specifically caught.
	/// @xsl.usage general
	/// </summary>
	public class XPathProcessorException : XPathException
	{
		internal new const long serialVersionUID = 1215509418326642603L;

	  /// <summary>
	  /// Create an XPathProcessorException object that holds
	  /// an error message. </summary>
	  /// <param name="message"> The error message. </param>
	  public XPathProcessorException(string message) : base(message)
	  {
	  }


	  /// <summary>
	  /// Create an XPathProcessorException object that holds
	  /// an error message, and another exception
	  /// that caused this exception. </summary>
	  /// <param name="message"> The error message. </param>
	  /// <param name="e"> The exception that caused this exception. </param>
	  public XPathProcessorException(string message, Exception e) : base(message, e)
	  {
	  }
	}

}