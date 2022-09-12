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
 * $Id: ApplyImports.java 469276 2006-10-30 21:09:47Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	internal sealed class ApplyImports : Instruction
	{

		private QName _modeName;
		private int _precedence;

		public override void display(int indent)
		{
		indent(indent);
		Util.println("ApplyTemplates");
		indent(indent + IndentIncrement);
		if (_modeName != null)
		{
			indent(indent + IndentIncrement);
			Util.println("mode " + _modeName);
		}
		}

		/// <summary>
		/// Returns true if this <xsl:apply-imports/> element has parameters
		/// </summary>
		public bool hasWithParams()
		{
		return hasContents();
		}

		/// <summary>
		/// Determine the lowest import precedence for any stylesheet imported
		/// or included by the stylesheet in which this <xsl:apply-imports/>
		/// element occured. The templates that are imported by the stylesheet in
		/// which this element occured will all have higher import precedence than
		/// the integer returned by this method.
		/// </summary>
		private int getMinPrecedence(int max)
		{
			// Move to root of include tree
			Stylesheet includeRoot = Stylesheet;
			while (includeRoot._includedFrom != null)
			{
				includeRoot = includeRoot._includedFrom;
			}

			return includeRoot.MinimumDescendantPrecedence;
		}

		/// <summary>
		/// Parse the attributes and contents of an <xsl:apply-imports/> element.
		/// </summary>
		public override void parseContents(Parser parser)
		{
		// Indicate to the top-level stylesheet that all templates must be
		// compiled into separate methods.
		Stylesheet stylesheet = Stylesheet;
		stylesheet.TemplateInlining = false;

		// Get the mode we are currently in (might not be any)
		Template template = Template;
		_modeName = template.ModeName;
		_precedence = template.ImportPrecedence;

		parseChildren(parser); // with-params
		}

		/// <summary>
		/// Type-check the attributes/contents of an <xsl:apply-imports/> element.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		typeCheckContents(stable); // with-params
		return Type.Void;
		}

		/// <summary>
		/// Translate call-template. A parameter frame is pushed only if
		/// some template in the stylesheet uses parameters. 
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stylesheet stylesheet = classGen.getStylesheet();
		Stylesheet stylesheet = classGen.Stylesheet;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int current = methodGen.getLocalIndex("current");
		int current = methodGen.getLocalIndex("current");

		// Push the arguments that are passed to applyTemplates()
		il.append(classGen.loadTranslet());
		il.append(methodGen.loadDOM());
		il.append(methodGen.loadIterator());
		il.append(methodGen.loadHandler());
		il.append(methodGen.loadCurrentNode());

			// Push a new parameter frame in case imported template might expect
			// parameters.  The apply-imports has nothing that it can pass.
			if (stylesheet.hasLocalParams())
			{
				il.append(classGen.loadTranslet());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pushFrame = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, Constants_Fields.PUSH_PARAM_FRAME, Constants_Fields.PUSH_PARAM_FRAME_SIG);
				int pushFrame = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, Constants_Fields.PUSH_PARAM_FRAME, Constants_Fields.PUSH_PARAM_FRAME_SIG);
				il.append(new INVOKEVIRTUAL(pushFrame));
			}

		// Get the [min,max> precedence of all templates imported under the
		// current stylesheet
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxPrecedence = _precedence;
		int maxPrecedence = _precedence;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int minPrecedence = getMinPrecedence(maxPrecedence);
		int minPrecedence = getMinPrecedence(maxPrecedence);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Mode mode = stylesheet.getMode(_modeName);
		Mode mode = stylesheet.getMode(_modeName);

			// Get name of appropriate apply-templates function for this
			// xsl:apply-imports instruction
		string functionName = mode.functionName(minPrecedence, maxPrecedence);

		// Construct the translet class-name and the signature of the method
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String className = classGen.getStylesheet().getClassName();
		string className = classGen.Stylesheet.ClassName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String signature = classGen.getApplyTemplatesSigForImport();
		string signature = classGen.ApplyTemplatesSigForImport;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int applyTemplates = cpg.addMethodref(className, functionName, signature);
		int applyTemplates = cpg.addMethodref(className, functionName, signature);
		il.append(new INVOKEVIRTUAL(applyTemplates));

			// Pop any parameter frame that was pushed above.
			if (stylesheet.hasLocalParams())
			{
				il.append(classGen.loadTranslet());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pushFrame = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, Constants_Fields.POP_PARAM_FRAME, Constants_Fields.POP_PARAM_FRAME_SIG);
				int pushFrame = cpg.addMethodref(Constants_Fields.TRANSLET_CLASS, Constants_Fields.POP_PARAM_FRAME, Constants_Fields.POP_PARAM_FRAME_SIG);
				il.append(new INVOKEVIRTUAL(pushFrame));
			}
		}

	}

}