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
 * $Id: ExtensionsProvider.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath
{

	using FuncExtFunction = org.apache.xpath.functions.FuncExtFunction;

	/// <summary>
	/// Interface that XPath objects can call to obtain access to an 
	/// ExtensionsTable.
	/// 
	/// </summary>
	public interface ExtensionsProvider
	{
	  /// <summary>
	  /// Is the extension function available?
	  /// </summary>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean functionAvailable(String ns, String funcName) throws javax.xml.transform.TransformerException;
	  bool functionAvailable(string ns, string funcName);

	  /// <summary>
	  /// Is the extension element available?
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean elementAvailable(String ns, String elemName) throws javax.xml.transform.TransformerException;
	  bool elementAvailable(string ns, string elemName);

	  /// <summary>
	  /// Execute the extension function.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object extFunction(String ns, String funcName, java.util.Vector argVec, Object methodKey) throws javax.xml.transform.TransformerException;
	  object extFunction(string ns, string funcName, ArrayList argVec, object methodKey);

	  /// <summary>
	  /// Execute the extension function.
	  /// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object extFunction(org.apache.xpath.functions.FuncExtFunction extFunction, java.util.Vector argVec) throws javax.xml.transform.TransformerException;
	  object extFunction(FuncExtFunction extFunction, ArrayList argVec);
	}

}