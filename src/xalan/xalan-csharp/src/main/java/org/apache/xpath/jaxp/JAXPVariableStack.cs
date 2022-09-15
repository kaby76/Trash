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
// $Id: JAXPVariableStack.java 524815 2007-04-02 15:52:15Z zongaro $

namespace org.apache.xpath.jaxp
{

	using QName = org.apache.xml.utils.QName;
	using VariableStack = org.apache.xpath.VariableStack;
	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;

	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;
	using XSLMessages = org.apache.xalan.res.XSLMessages;


	/// <summary>
	/// Overrides <seealso cref="VariableStack"/> and delegates the call to
	/// <seealso cref="javax.xml.xpath.XPathVariableResolver"/>.
	/// 
	/// @author Ramesh Mandava ( ramesh.mandava@sun.com )
	/// </summary>
	public class JAXPVariableStack : VariableStack
	{

		private readonly XPathVariableResolver resolver;

		public JAXPVariableStack(XPathVariableResolver resolver) : base(2)
		{
			this.resolver = resolver;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject getVariableOrParam(org.apache.xpath.XPathContext xctxt, org.apache.xml.utils.QName qname) throws TransformerException,IllegalArgumentException
		public override XObject getVariableOrParam(XPathContext xctxt, QName qname)
		{
			if (qname == null)
			{
				//JAXP 1.3 spec says that if variable name is null then 
				// we need to through IllegalArgumentException
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_ARG_CANNOT_BE_NULL, new object[] {"Variable qname"});
				throw new System.ArgumentException(fmsg);
			}
		javax.xml.@namespace.QName name = new javax.xml.@namespace.QName(qname.Namespace, qname.LocalPart);
			object varValue = resolver.resolveVariable(name);
			if (varValue == null)
			{
				string fmsg = XSLMessages.createXPATHMessage(XPATHErrorResources.ER_RESOLVE_VARIABLE_RETURNS_NULL, new object[] {name.ToString()});
				throw new TransformerException(fmsg);
			}
			return XObject.create(varValue, xctxt);
		}

	}

}