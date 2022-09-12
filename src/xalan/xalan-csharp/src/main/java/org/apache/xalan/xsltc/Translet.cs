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
 * $Id: Translet.java 468648 2006-10-28 07:00:06Z minchau $
 */

namespace org.apache.xalan.xsltc
{

	using DTMAxisIterator = org.apache.xml.dtm.DTMAxisIterator;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface Translet
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(DOM document, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void transform(DOM document, SerializationHandler handler);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(DOM document, org.apache.xml.serializer.SerializationHandler[] handlers) throws TransletException;
		void transform(DOM document, SerializationHandler[] handlers);
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void transform(DOM document, org.apache.xml.dtm.DTMAxisIterator iterator, org.apache.xml.serializer.SerializationHandler handler) throws TransletException;
		void transform(DOM document, DTMAxisIterator iterator, SerializationHandler handler);

		object addParameter(string name, object value);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void buildKeys(DOM document, org.apache.xml.dtm.DTMAxisIterator iterator, org.apache.xml.serializer.SerializationHandler handler, int root) throws TransletException;
		void buildKeys(DOM document, DTMAxisIterator iterator, SerializationHandler handler, int root);
		void addAuxiliaryClass(Type auxClass);
		Type getAuxiliaryClass(string className);
		string[] NamesArray {get;}
		string[] UrisArray {get;}
		int[] TypesArray {get;}
		string[] NamespaceArray {get;}
	}

}