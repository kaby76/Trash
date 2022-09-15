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
 * $Id: ExtendedLexicalHandler.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{

	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This interface has extensions to the standard SAX LexicalHandler interface.
	/// This interface is intended to be used by a serializer.
	/// @xsl.usage internal
	/// </summary>
	public interface ExtendedLexicalHandler : org.xml.sax.ext.LexicalHandler
	{
		/// <summary>
		/// This method is used to notify of a comment </summary>
		/// <param name="comment"> the comment, but unlike the SAX comment() method this
		/// method takes a String rather than a character array. </param>
		/// <exception cref="SAXException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void comment(String comment) throws org.xml.sax.SAXException;
		void comment(string comment);
	}

}