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
 * $Id: VarNameCollector.java 468643 2006-10-28 06:56:03Z minchau $
 */
namespace org.apache.xalan.templates
{

	using QName = org.apache.xml.utils.QName;
	using ExpressionOwner = org.apache.xpath.ExpressionOwner;
	using XPathVisitor = org.apache.xpath.XPathVisitor;
	using Variable = org.apache.xpath.operations.Variable;

	/// <summary>
	/// This class visits variable refs in an XPath and collects their QNames.
	/// </summary>
	public class VarNameCollector : XPathVisitor
	{
		internal ArrayList m_refs = new ArrayList();

		/// <summary>
		/// Reset the list for a fresh visitation and collection.
		/// </summary>
		public virtual void reset()
		{
			m_refs.Clear(); //.clear();
		}

		/// <summary>
		/// Get the number of variable references that were collected. </summary>
		/// <returns> the size of the list. </returns>
		public virtual int VarCount
		{
			get
			{
				return m_refs.Count;
			}
		}

		/// <summary>
		/// Tell if the given qualified name occurs in 
		/// the list of qualified names collected.
		/// </summary>
		/// <param name="refName"> Must be a valid qualified name. </param>
		/// <returns> true if the list contains the qualified name. </returns>
		internal virtual bool doesOccur(QName refName)
		{
			return m_refs.Contains(refName);
		}

		/// <summary>
		/// Visit a variable reference. </summary>
		/// <param name="owner"> The owner of the expression, to which the expression can 
		///              be reset if rewriting takes place. </param>
		/// <param name="var"> The variable reference object. </param>
		/// <returns> true if the sub expressions should be traversed. </returns>
		public override bool visitVariableRef(ExpressionOwner owner, Variable var)
		{
			m_refs.Add(var.QName);
			return true;
		}

	}


}