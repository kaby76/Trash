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
 * $Id: ClassGenerator.java 468649 2006-10-28 07:00:55Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using Method = org.apache.bcel.classfile.Method;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ClassGen = org.apache.bcel.generic.ClassGen;
	using Instruction = org.apache.bcel.generic.Instruction;

	/// <summary>
	/// The class that implements any class that inherits from 
	/// <tt>AbstractTranslet</tt>, i.e. any translet. Methods in this 
	/// class may be of the following kinds: 
	/// 
	/// 1. Main method: applyTemplates, implemented by intances of 
	/// <tt>MethodGenerator</tt>.
	/// 
	/// 2. Named methods: for named templates, implemented by instances 
	/// of <tt>NamedMethodGenerator</tt>.
	/// 
	/// 3. Rt methods: for result tree fragments, implemented by 
	/// instances of <tt>RtMethodGenerator</tt>.
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class ClassGenerator : ClassGen
	{
		protected internal const int TRANSLET_INDEX = 0;

		private Stylesheet _stylesheet;
		private readonly Parser _parser; // --> can be moved to XSLT
		// a  single instance cached here
		private readonly Instruction _aloadTranslet;
		private readonly string _domClass;
		private readonly string _domClassSig;
		private readonly string _applyTemplatesSig;
		private readonly string _applyTemplatesSigForImport;

		public ClassGenerator(string class_name, string super_class_name, string file_name, int access_flags, string[] interfaces, Stylesheet stylesheet) : base(class_name, super_class_name, file_name, access_flags, interfaces)
		{
		_stylesheet = stylesheet;
		_parser = stylesheet.Parser;
		_aloadTranslet = new ALOAD(TRANSLET_INDEX);

		if (stylesheet.MultiDocument)
		{
			_domClass = "org.apache.xalan.xsltc.dom.MultiDOM";
			_domClassSig = "Lorg/apache/xalan/xsltc/dom/MultiDOM;";
		}
		else
		{
			_domClass = "org.apache.xalan.xsltc.dom.DOMAdapter";
			_domClassSig = "Lorg/apache/xalan/xsltc/dom/DOMAdapter;";
		}
		_applyTemplatesSig = "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_SIG + ")V";

		_applyTemplatesSigForImport = "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.DOM_INTF_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_FIELD_SIG + ")V";
		}

		public Parser Parser
		{
			get
			{
			return _parser;
			}
		}

		public Stylesheet Stylesheet
		{
			get
			{
			return _stylesheet;
			}
		}

		/// <summary>
		/// Pretend this is the stylesheet class. Useful when compiling 
		/// references to global variables inside a predicate.
		/// </summary>
		public string ClassName
		{
			get
			{
			return _stylesheet.ClassName;
			}
		}

		public virtual Instruction loadTranslet()
		{
		return _aloadTranslet;
		}

		public string DOMClass
		{
			get
			{
			return _domClass;
			}
		}

		public string DOMClassSig
		{
			get
			{
			return _domClassSig;
			}
		}

		public string ApplyTemplatesSig
		{
			get
			{
			return _applyTemplatesSig;
			}
		}

		public string ApplyTemplatesSigForImport
		{
			get
			{
			return _applyTemplatesSigForImport;
			}
		}

		/// <summary>
		/// Returns <tt>true</tt> or <tt>false</tt> depending on whether
		/// this class inherits from <tt>AbstractTranslet</tt> or not.
		/// </summary>
		public virtual bool External
		{
			get
			{
			return false;
			}
		}

		public virtual void addMethod(MethodGenerator methodGen)
		{
			Method[] methodsToAdd = methodGen.getGeneratedMethods(this);
			for (int i = 0; i < methodsToAdd.Length; i++)
			{
				addMethod(methodsToAdd[i]);
			}
		}
	}

}