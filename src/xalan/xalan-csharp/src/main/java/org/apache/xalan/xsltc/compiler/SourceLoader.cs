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
 * $Id: SourceLoader.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using InputSource = org.xml.sax.InputSource;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public interface SourceLoader
	{

		/// <summary>
		/// This interface is used to plug external document loaders into XSLTC
		/// (used with the <xsl:include> and <xsl:import> elements.
		/// </summary>
		/// <param name="href"> The URI of the document to load </param>
		/// <param name="context"> The URI of the currently loaded document </param>
		/// <param name="xsltc"> The compiler that resuests the document </param>
		/// <returns> An InputSource with the loaded document </returns>
		InputSource loadSource(string href, string context, XSLTC xsltc);

	}

}