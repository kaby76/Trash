using System;
using System.Collections;
using System.Text;

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
 * $Id: CallTemplate.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	internal sealed class CallTemplate : Instruction
	{

		/// <summary>
		/// Name of template to call.
		/// </summary>
		private QName _name;

		/// <summary>
		/// The array of effective parameters in this CallTemplate. An object in 
		/// this array can be either a WithParam or a Param if no WithParam 
		/// exists for a particular parameter.
		/// </summary>
		private object[] _parameters = null;

		/// <summary>
		/// The corresponding template which this CallTemplate calls.
		/// </summary>
		private Template _calleeTemplate = null;

		public override void display(int indent)
		{
		this.indent(indent);
		Console.Write("CallTemplate");
		Util.println(" name " + _name);
		displayContents(indent + IndentIncrement);
		}

		public bool hasWithParams()
		{
		return elementCount() > 0;
		}

		public override void parseContents(Parser parser)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getAttribute("name");
			string name = getAttribute("name");
			if (name.Length > 0)
			{
				if (!XML11Char.isXML11ValidQName(name))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, name, this);
					parser.reportError(Constants.ERROR, err);
				}
				_name = parser.getQNameIgnoreDefaultNs(name);
			}
			else
			{
				reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "name");
			}
		parseChildren(parser);
		}

		/// <summary>
		/// Verify that a template with this name exists.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = stable.lookupTemplate(_name);
		Template template = stable.lookupTemplate(_name);
		if (template != null)
		{
			typeCheckContents(stable);
		}
		else
		{
			ErrorMsg err = new ErrorMsg(ErrorMsg.TEMPLATE_UNDEF_ERR,_name,this);
			throw new TypeCheckError(err);
		}
		return Type.Void;
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stylesheet stylesheet = classGen.getStylesheet();
		Stylesheet stylesheet = classGen.Stylesheet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

			// If there are Params in the stylesheet or WithParams in this call?
		if (stylesheet.hasLocalParams() || hasContents())
		{
			_calleeTemplate = CalleeTemplate;

			// Build the parameter list if the called template is simple named
			if (_calleeTemplate != null)
			{
				buildParameterList();
			}
			// This is only needed when the called template is not
			// a simple named template.
			else
			{
				// Push parameter frame
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int push = cpg.addMethodref(TRANSLET_CLASS, PUSH_PARAM_FRAME, PUSH_PARAM_FRAME_SIG);
				int push = cpg.addMethodref(TRANSLET_CLASS, PUSH_PARAM_FRAME, PUSH_PARAM_FRAME_SIG);
				il.append(classGen.loadTranslet());
				il.append(new INVOKEVIRTUAL(push));
				translateContents(classGen, methodGen);
			}
		}

			// Generate a valid Java method name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = stylesheet.getClassName();
		string className = stylesheet.ClassName;
			string methodName = Util.escape(_name.ToString());

			// Load standard arguments
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadIterator());
		il.append(methodGen.loadHandler());
		il.append(methodGen.loadCurrentNode());

			// Initialize prefix of method signature
		StringBuilder methodSig = new StringBuilder("(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + NODE_SIG);

			// If calling a simply named template, push actual arguments
		if (_calleeTemplate != null)
		{
			ArrayList calleeParams = _calleeTemplate.Parameters;
			int numParams = _parameters.Length;

			for (int i = 0; i < numParams; i++)
			{
				SyntaxTreeNode node = (SyntaxTreeNode)_parameters[i];
					methodSig.Append(OBJECT_SIG); // append Object to signature

					// Push 'null' if Param to indicate no actual parameter specified
					if (node is Param)
					{
						il.append(ACONST_NULL);
					}
					else
					{ // translate WithParam
						node.translate(classGen, methodGen);
					}
			}
		}

			// Complete signature and generate invokevirtual call
		methodSig.Append(")V");
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(className, methodName, methodSig.ToString())));

		// Do not need to call Translet.popParamFrame() if we are
		// calling a simple named template.
		if (_calleeTemplate == null && (stylesheet.hasLocalParams() || hasContents()))
		{
			// Pop parameter frame
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pop = cpg.addMethodref(TRANSLET_CLASS, POP_PARAM_FRAME, POP_PARAM_FRAME_SIG);
			int pop = cpg.addMethodref(TRANSLET_CLASS, POP_PARAM_FRAME, POP_PARAM_FRAME_SIG);
			il.append(classGen.loadTranslet());
			il.append(new INVOKEVIRTUAL(pop));
		}
		}

		/// <summary>
		/// Return the simple named template which this CallTemplate calls.
		/// Return false if there is no matched template or the matched
		/// template is not a simple named template.
		/// </summary>
		public Template CalleeTemplate
		{
			get
			{
				Template foundTemplate = XSLTC.Parser.SymbolTable.lookupTemplate(_name);
    
				return foundTemplate.SimpleNamedTemplate ? foundTemplate : null;
			}
		}

		/// <summary>
		/// Build the list of effective parameters in this CallTemplate.
		/// The parameters of the called template are put into the array first.
		/// Then we visit the WithParam children of this CallTemplate and replace
		/// the Param with a corresponding WithParam having the same name.
		/// </summary>
		private void buildParameterList()
		{
			// Put the parameters from the called template into the array first.
			// This is to ensure the order of the parameters.
			ArrayList defaultParams = _calleeTemplate.Parameters;
			int numParams = defaultParams.Count;
			_parameters = new object[numParams];
			for (int i = 0; i < numParams; i++)
			{
				_parameters[i] = defaultParams[i];
			}

			// Replace a Param with a WithParam if they have the same name.
			int count = elementCount();
			for (int i = 0; i < count; i++)
			{
				object node = elementAt(i);

				// Ignore if not WithParam
				if (node is WithParam)
				{
					WithParam withParam = (WithParam)node;
					QName name = withParam.Name;

					// Search for a Param with the same name
					for (int k = 0; k < numParams; k++)
					{
						object @object = _parameters[k];
						if (@object is Param && ((Param)@object).Name.Equals(name))
						{
							withParam.DoParameterOptimization = true;
							_parameters[k] = withParam;
							break;
						}
						else if (@object is WithParam && ((WithParam)@object).Name.Equals(name))
						{
							withParam.DoParameterOptimization = true;
							_parameters[k] = withParam;
							break;
						}
					}
				}
			}
		}
	}


}