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
 * $Id: WithParam.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using ReferenceType = org.apache.xalan.xsltc.compiler.util.ReferenceType;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using XML11Char = org.apache.xml.utils.XML11Char;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author John Howard <JohnH@schemasoft.com>
	/// </summary>
	internal sealed class WithParam : Instruction
	{

		/// <summary>
		/// Parameter's name.
		/// </summary>
		private QName _name;

		/// <summary>
		/// The escaped qname of the with-param.
		/// </summary>
		protected internal string _escapedName;

		/// <summary>
		/// Parameter's default value.
		/// </summary>
		private Expression _select;

		/// <summary>
		/// %OPT% This is set to true when the WithParam is used in a CallTemplate
		/// for a simple named template. If this is true, the parameters are 
		/// passed to the named template through method arguments rather than
		/// using the expensive Translet.addParameter() call.
		/// </summary>
		private bool _doParameterOptimization = false;

		/// <summary>
		/// Displays the contents of this element
		/// </summary>
		public override void display(int indent)
		{
		this.indent(indent);
		Util.println("with-param " + _name);
		if (_select != null)
		{
			this.indent(indent + IndentIncrement);
			Util.println("select " + _select.ToString());
		}
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Returns the escaped qname of the parameter
		/// </summary>
		public string EscapedName
		{
			get
			{
			return _escapedName;
			}
		}

		/// <summary>
		/// Return the name of this WithParam.
		/// </summary>
		public QName Name
		{
			get
			{
				return _name;
			}
			set
			{
			_name = value;
			_escapedName = Util.escape(value.StringRep);
			}
		}


		/// <summary>
		/// Set the do parameter optimization flag
		/// </summary>
		public bool DoParameterOptimization
		{
			set
			{
				_doParameterOptimization = value;
			}
		}

		/// <summary>
		/// The contents of a <xsl:with-param> elements are either in the element's
		/// 'select' attribute (this has precedence) or in the element body.
		/// </summary>
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
			Name = parser.getQNameIgnoreDefaultNs(name);
		}
			else
			{
			reportError(this, parser, ErrorMsg.REQUIRED_ATTR_ERR, "name");
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String select = getAttribute("select");
		string select = getAttribute("select");
		if (select.Length > 0)
		{
			_select = parser.parseExpression(this, "select", null);
		}

		parseChildren(parser);
		}

		/// <summary>
		/// Type-check either the select attribute or the element body, depending
		/// on which is in use.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_select != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.util.Type tselect = _select.typeCheck(stable);
			Type tselect = _select.typeCheck(stable);
			if (tselect is ReferenceType == false)
			{
			_select = new CastExpr(_select, Type.Reference);
			}
		}
		else
		{
			typeCheckContents(stable);
		}
		return Type.Void;
		}

		/// <summary>
		/// Compile the value of the parameter, which is either in an expression in
		/// a 'select' attribute, or in the with-param element's body
		/// </summary>
		public void translateValue(ClassGenerator classGen, MethodGenerator methodGen)
		{
		// Compile expression is 'select' attribute if present
		if (_select != null)
		{
			_select.translate(classGen, methodGen);
			_select.startIterator(classGen, methodGen);
		}
		// If not, compile result tree from parameter body if present.
		else if (hasContents())
		{
			compileResultTree(classGen, methodGen);
		}
		// If neither are present then store empty string in parameter slot
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
			ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
			InstructionList il = methodGen.getInstructionList();
			il.append(new PUSH(cpg, Constants.EMPTYSTRING));
		}
		}

		/// <summary>
		/// This code generates a sequence of bytecodes that call the
		/// addParameter() method in AbstractTranslet. The method call will add
		/// (or update) the parameter frame with the new parameter value.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		// Translate the value and put it on the stack
		if (_doParameterOptimization)
		{
			translateValue(classGen, methodGen);
			return;
		}

		// Make name acceptable for use as field name in class
		string name = Util.escape(EscapedName);

		// Load reference to the translet (method is in AbstractTranslet)
		il.append(classGen.loadTranslet());

		// Load the name of the parameter
		il.append(new PUSH(cpg, name)); // TODO: namespace ?
		// Generete the value of the parameter (use value in 'select' by def.)
		translateValue(classGen, methodGen);
		// Mark this parameter value is not being the default value
		il.append(new PUSH(cpg, false));
		// Pass the parameter to the template
		il.append(new INVOKEVIRTUAL(cpg.addMethodref(TRANSLET_CLASS, ADD_PARAMETER, ADD_PARAMETER_SIG)));
		il.append(POP); // cleanup stack
		}
	}

}