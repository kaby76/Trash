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
 * $Id: XSLTVisitor.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using XPathVisitor = org.apache.xpath.XPathVisitor;

	/// <summary>
	/// A derivation from this class can be passed to a class that implements 
	/// the XSLTVisitable interface, to have the appropriate method called 
	/// for each component of an XSLT stylesheet.  Aside from possible other uses,
	/// the main intention is to provide a reasonable means to perform expression 
	/// rewriting.
	/// </summary>
	public class XSLTVisitor : XPathVisitor
	{
		/// <summary>
		/// Visit an XSLT instruction.  Any element that isn't called by one 
		/// of the other visit methods, will be called by this method.
		/// </summary>
		/// <param name="elem"> The xsl instruction element object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitInstruction(ElemTemplateElement elem)
		{
			return true;
		}

		/// <summary>
		/// Visit an XSLT stylesheet instruction.
		/// </summary>
		/// <param name="elem"> The xsl instruction element object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitStylesheet(ElemTemplateElement elem)
		{
			return true;
		}


		/// <summary>
		/// Visit an XSLT top-level instruction.
		/// </summary>
		/// <param name="elem"> The xsl instruction element object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitTopLevelInstruction(ElemTemplateElement elem)
		{
			return true;
		}

		/// <summary>
		/// Visit an XSLT top-level instruction.
		/// </summary>
		/// <param name="elem"> The xsl instruction element object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitTopLevelVariableOrParamDecl(ElemTemplateElement elem)
		{
			return true;
		}


		/// <summary>
		/// Visit an XSLT variable or parameter declaration.
		/// </summary>
		/// <param name="elem"> The xsl instruction element object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitVariableOrParamDecl(ElemVariable elem)
		{
			return true;
		}

		/// <summary>
		/// Visit a LiteralResultElement.
		/// </summary>
		/// <param name="elem"> The literal result object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitLiteralResultElement(ElemLiteralResult elem)
		{
			return true;
		}

		/// <summary>
		/// Visit an Attribute Value Template (at the top level).
		/// </summary>
		/// <param name="elem"> The attribute value template object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitAVT(AVT elem)
		{
			return true;
		}


		/// <summary>
		/// Visit an extension element. </summary>
		/// <param name="elem"> The extension object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public virtual bool visitExtensionElement(ElemExtensionCall elem)
		{
			return true;
		}

	}


}