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
 * $Id: Template.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using NamedMethodGenerator = org.apache.xalan.xsltc.compiler.util.NamedMethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;
	using XML11Char = org.apache.xml.utils.XML11Char;


	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// </summary>
	public sealed class Template : TopLevelElement
	{

		private QName _name; // The name of the template (if any)
		private QName _mode; // Mode in which this template is instantiated.
		private Pattern _pattern; // Matching pattern defined for this template.
		private double _priority; // Matching priority of this template.
		private int _position; // Position within stylesheet (prio. resolution)
		private bool _disabled = false;
		private bool _compiled = false; //make sure it is compiled only once
		private bool _simplified = false;

		// True if this is a simple named template. A simple named 
		// template is a template which only has a name but no match pattern.
		private bool _isSimpleNamedTemplate = false;

		// The list of parameters in this template. This is only used
		// for simple named templates.
		private ArrayList _parameters = new ArrayList();

		public bool hasParams()
		{
		return _parameters.Count > 0;
		}

		public bool Simplified
		{
			get
			{
			return (_simplified);
			}
		}

		public void setSimplified()
		{
		_simplified = true;
		}

		public bool SimpleNamedTemplate
		{
			get
			{
				return _isSimpleNamedTemplate;
			}
		}

		public void addParameter(Param param)
		{
			_parameters.Add(param);
		}

		public ArrayList Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public void disable()
		{
		_disabled = true;
		}

		public bool disabled()
		{
		return (_disabled);
		}

		public double Priority
		{
			get
			{
			return _priority;
			}
		}

		public int Position
		{
			get
			{
			return (_position);
			}
		}

		public bool Named
		{
			get
			{
			return _name != null;
			}
		}

		public Pattern Pattern
		{
			get
			{
			return _pattern;
			}
		}

		public QName Name
		{
			get
			{
			return _name;
			}
			set
			{
			if (_name == null)
			{
				_name = value;
			}
			}
		}


		public QName ModeName
		{
			get
			{
			return _mode;
			}
		}

		/// <summary>
		/// Compare this template to another. First checks priority, then position.
		/// </summary>
		public int compareTo(object template)
		{
		Template other = (Template)template;
		if (_priority > other._priority)
		{
			return 1;
		}
		else if (_priority < other._priority)
		{
			return -1;
		}
		else if (_position > other._position)
		{
			return 1;
		}
		else if (_position < other._position)
		{
			return -1;
		}
		else
		{
			return 0;
		}
		}

		public override void display(int indent)
		{
		Util.println('\n');
		this.indent(indent);
		if (_name != null)
		{
			this.indent(indent);
			Util.println("name = " + _name);
		}
		else if (_pattern != null)
		{
			this.indent(indent);
			Util.println("match = " + _pattern.ToString());
		}
		if (_mode != null)
		{
			this.indent(indent);
			Util.println("mode = " + _mode);
		}
		displayContents(indent + IndentIncrement);
		}

		private bool resolveNamedTemplates(Template other, Parser parser)
		{

		if (other == null)
		{
			return true;
		}

		SymbolTable stable = parser.SymbolTable;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int us = this.getImportPrecedence();
		int us = this.ImportPrecedence;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int them = other.getImportPrecedence();
		int them = other.ImportPrecedence;

		if (us > them)
		{
			other.disable();
			return true;
		}
		else if (us < them)
		{
			stable.addTemplate(other);
			this.disable();
			return true;
		}
		else
		{
			return false;
		}
		}

		private Stylesheet _stylesheet = null;

		public override Stylesheet Stylesheet
		{
			get
			{
			return _stylesheet;
			}
		}

		public override void parseContents(Parser parser)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = getAttribute("name");
		string name = getAttribute("name");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String mode = getAttribute("mode");
		string mode = getAttribute("mode");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String match = getAttribute("match");
		string match = getAttribute("match");
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String priority = getAttribute("priority");
		string priority = getAttribute("priority");

		_stylesheet = base.Stylesheet;

		if (name.Length > 0)
		{
				if (!XML11Char.isXML11ValidQName(name))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, name, this);
					parser.reportError(Constants.ERROR, err);
				}
			_name = parser.getQNameIgnoreDefaultNs(name);
		}

		if (mode.Length > 0)
		{
				if (!XML11Char.isXML11ValidQName(mode))
				{
					ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_QNAME_ERR, mode, this);
					parser.reportError(Constants.ERROR, err);
				}
			_mode = parser.getQNameIgnoreDefaultNs(mode);
		}

		if (match.Length > 0)
		{
			_pattern = parser.parsePattern(this, "match", null);
		}

		if (priority.Length > 0)
		{
			_priority = double.Parse(priority);
		}
		else
		{
			if (_pattern != null)
			{
			_priority = _pattern.Priority;
			}
			else
			{
			_priority = Double.NaN;
			}
		}

		_position = parser.TemplateIndex;

		// Add the (named) template to the symbol table
		if (_name != null)
		{
			Template other = parser.SymbolTable.addTemplate(this);
			if (!resolveNamedTemplates(other, parser))
			{
			ErrorMsg err = new ErrorMsg(ErrorMsg.TEMPLATE_REDEF_ERR, _name, this);
			parser.reportError(Constants.ERROR, err);
			}
			// Is this a simple named template?
			if (_pattern == null && _mode == null)
			{
				_isSimpleNamedTemplate = true;
			}
		}

		if (_parent is Stylesheet)
		{
			((Stylesheet)_parent).addTemplate(this);
		}

		parser.Template = this; // set current template
		parseChildren(parser);
		parser.Template = null; // clear template
		}

		/// <summary>
		/// When the parser realises that it is dealign with a simplified stylesheet
		/// it will create an empty Stylesheet object with the root element of the
		/// stylesheet (a LiteralElement object) as its only child. The Stylesheet
		/// object will then create this Template object and invoke this method to
		/// force some specific behaviour. What we need to do is:
		///  o) create a pattern matching on the root node
		///  o) add the LRE root node (the only child of the Stylesheet) as our
		///     only child node
		///  o) set the empty Stylesheet as our parent
		///  o) set this template as the Stylesheet's only child
		/// </summary>
		public void parseSimplified(Stylesheet stylesheet, Parser parser)
		{

		_stylesheet = stylesheet;
		Parent = stylesheet;

		_name = null;
		_mode = null;
		_priority = Double.NaN;
		_pattern = parser.parsePattern(this, "/");

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector contents = _stylesheet.getContents();
		ArrayList contents = _stylesheet.Contents;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SyntaxTreeNode root = (SyntaxTreeNode)contents.elementAt(0);
		SyntaxTreeNode root = (SyntaxTreeNode)contents[0];

		if (root is LiteralElement)
		{
			addElement(root);
			root.Parent = this;
			contents[0] = this;
			parser.Template = this;
			root.parseContents(parser);
			parser.Template = null;
		}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_pattern != null)
		{
			_pattern.typeCheck(stable);
		}

		return typeCheckContents(stable);
		}

		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.getConstantPool();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.getInstructionList();

		if (_disabled)
		{
			return;
		}
		// bug fix #4433133, add a call to named template from applyTemplates 
		string className = classGen.ClassName;

		if (_compiled && Named)
		{
			string methodName = Util.escape(_name.ToString());
			il.append(classGen.loadTranslet());
			il.append(methodGen.loadDOM());
			il.append(methodGen.loadIterator());
			il.append(methodGen.loadHandler());
			il.append(methodGen.loadCurrentNode());
			il.append(new INVOKEVIRTUAL(cpg.addMethodref(className, methodName, "(" + DOM_INTF_SIG + NODE_ITERATOR_SIG + TRANSLET_OUTPUT_SIG + "I)V")));
			return;
		}

		if (_compiled)
		{
			return;
		}
		_compiled = true;

		// %OPT% Special handling for simple named templates.
		if (_isSimpleNamedTemplate && methodGen is NamedMethodGenerator)
		{
			int numParams = _parameters.Count;
			NamedMethodGenerator namedMethodGen = (NamedMethodGenerator)methodGen;

				// Update load/store instructions to access Params from the stack
			for (int i = 0; i < numParams; i++)
			{
				Param param = (Param)_parameters[i];
				param.LoadInstruction = namedMethodGen.loadParameter(i);
				param.StoreInstruction = namedMethodGen.storeParameter(i);
			}
		}

			translateContents(classGen, methodGen);
		il.setPositions(true);
		}

	}

}