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
 * $Id: UseAttributeSets.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class UseAttributeSets : Instruction
	{

		// Only error that can occur:
		private const string ATTR_SET_NOT_FOUND = "";

		// Contains the names of all references attribute sets
		private readonly ArrayList _sets = new ArrayList(2);

		/// <summary>
		/// Constructur - define initial attribute sets to use
		/// </summary>
		public UseAttributeSets(string setNames, Parser parser)
		{
		Parser = parser;
		addAttributeSets(setNames);
		}

		/// <summary>
		/// This method is made public to enable an AttributeSet object to merge
		/// itself with another AttributeSet (including any other AttributeSets
		/// the two may inherit from).
		/// </summary>
		public void addAttributeSets(string setNames)
		{
		if ((!string.ReferenceEquals(setNames, null)) && (!setNames.Equals(Constants_Fields.EMPTYSTRING)))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.StringTokenizer tokens = new java.util.StringTokenizer(setNames);
			StringTokenizer tokens = new StringTokenizer(setNames);
			while (tokens.hasMoreTokens())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName qname = getParser().getQNameIgnoreDefaultNs(tokens.nextToken());
			QName qname = Parser.getQNameIgnoreDefaultNs(tokens.nextToken());
			_sets.Add(qname);
			}
		}
		}

		/// <summary>
		/// Do nada.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		return Type.Void;
		}

		/// <summary>
		/// Generate a call to the method compiled for this attribute set
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.ConstantPoolGen cpg = classGen.getConstantPool();
		ConstantPoolGen cpg = classGen.ConstantPool;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = methodGen.getInstructionList();
		InstructionList il = methodGen.InstructionList;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SymbolTable symbolTable = getParser().getSymbolTable();
		SymbolTable symbolTable = Parser.SymbolTable;

		// Go through each attribute set and generate a method call
		for (int i = 0; i < _sets.Count; i++)
		{
			// Get the attribute set name
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName name = (QName)_sets.elementAt(i);
			QName name = (QName)_sets[i];
			// Get the AttributeSet reference from the symbol table
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AttributeSet attrs = symbolTable.lookupAttributeSet(name);
			AttributeSet attrs = symbolTable.lookupAttributeSet(name);
			// Compile the call to the set's method if the set exists
			if (attrs != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String methodName = attrs.getMethodName();
			string methodName = attrs.MethodName;
			il.append(classGen.loadTranslet());
			il.append(methodGen.loadDOM());
			il.append(methodGen.loadIterator());
			il.append(methodGen.loadHandler());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int method = cpg.addMethodref(classGen.getClassName(), methodName, Constants_Fields.ATTR_SET_SIG);
			int method = cpg.addMethodref(classGen.ClassName, methodName, Constants_Fields.ATTR_SET_SIG);
			il.append(new INVOKESPECIAL(method));
			}
			// Generate an error if the attribute set does not exist
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Parser parser = getParser();
			Parser parser = Parser;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String atrs = name.toString();
			string atrs = name.ToString();
			reportError(this, parser, ErrorMsg.ATTRIBSET_UNDEF_ERR, atrs);
			}
		}
		}
	}

}