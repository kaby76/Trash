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
 * $Id: NodeCounterGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using Instruction = org.apache.bcel.generic.Instruction;
	using Stylesheet = org.apache.xalan.xsltc.compiler.Stylesheet;

	/// <summary>
	/// This class implements auxiliary classes needed to compile 
	/// patterns in <tt>xsl:number</tt>. These classes inherit from
	/// {Any,Single,Multiple}NodeCounter and override the 
	/// <tt>matchFrom</tt> and <tt>matchCount</tt> methods.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class NodeCounterGenerator : ClassGenerator
	{
		private Instruction _aloadTranslet;

		public NodeCounterGenerator(string className, string superClassName, string fileName, int accessFlags, string[] interfaces, Stylesheet stylesheet) : base(className, superClassName, fileName, accessFlags, interfaces, stylesheet)
		{
		}

		/// <summary>
		/// Set the index of the register where "this" (the pointer to
		/// the translet) is stored.
		/// </summary>
		public int TransletIndex
		{
			set
			{
			_aloadTranslet = new ALOAD(value);
			}
		}

		/// <summary>
		/// The index of the translet pointer within the execution of
		/// matchFrom or matchCount.
		/// Overridden from ClassGenerator.
		/// </summary>
		public override Instruction loadTranslet()
		{
		return _aloadTranslet;
		}

		/// <summary>
		/// Returns <tt>true</tt> since this class is external to the
		/// translet.
		/// </summary>
		public override bool External
		{
			get
			{
			return true;
			}
		}
	}

}