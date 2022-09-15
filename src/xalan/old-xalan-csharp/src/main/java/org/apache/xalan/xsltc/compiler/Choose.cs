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
 * $Id: Choose.java 1225842 2011-12-30 15:14:35Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using GOTO = org.apache.bcel.generic.GOTO;
	using IFEQ = org.apache.bcel.generic.IFEQ;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class Choose : Instruction
	{

		/// <summary>
		/// Display the element contents (a lot of when's and an otherwise)
		/// </summary>
		public override void display(int indent)
		{
		indent(indent);
		Util.println("Choose");
		indent(indent + IndentIncrement);
		displayContents(indent + IndentIncrement);
		}

		/// <summary>
		/// Translate this Choose element. Generate a test-chain for the various
		/// <xsl:when> elements and default to the <xsl:otherwise> if present.
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List whenElements = new java.util.ArrayList();
		IList whenElements = new ArrayList();
		Otherwise otherwise = null;
		System.Collections.IEnumerator elements = elements();

		// These two are for reporting errors only
		ErrorMsg error = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int line = getLineNumber();
		int line = LineNumber;

		// Traverse all child nodes - must be either When or Otherwise
		while (elements.MoveNext())
		{
			object element = elements.Current;
			// Add a When child element
			if (element is When)
			{
			whenElements.Add(element);
			}
			// Add an Otherwise child element
			else if (element is Otherwise)
			{
			if (otherwise == null)
			{
				otherwise = (Otherwise)element;
			}
			else
			{
				error = new ErrorMsg(ErrorMsg.MULTIPLE_OTHERWISE_ERR, this);
				Parser.reportError(Constants_Fields.ERROR, error);
			}
			}
			else if (element is Text)
			{
			((Text)element).ignore();
			}
			// It is an error if we find some other element here
			else
			{
			error = new ErrorMsg(ErrorMsg.WHEN_ELEMENT_ERR, this);
			Parser.reportError(Constants_Fields.ERROR, error);
			}
		}

		// Make sure that there is at least one <xsl:when> element
		if (whenElements.Count == 0)
		{
			error = new ErrorMsg(ErrorMsg.MISSING_WHEN_ERR, this);
			Parser.reportError(Constants_Fields.ERROR, error);
			return;
		}

		InstructionList il = methodGen.InstructionList;

		// next element will hold a handle to the beginning of next
		// When/Otherwise if test on current When fails
		BranchHandle nextElement = null;
		IList exitHandles = new ArrayList();
		InstructionHandle exit = null;

		IEnumerator whens = whenElements.GetEnumerator();
		while (whens.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final When when = (When)whens.Current;
			When when = (When)whens.Current;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Expression test = when.getTest();
			Expression test = when.Test;

			InstructionHandle truec = il.End;

			if (nextElement != null)
			{
			nextElement.Target = il.append(NOP);
			}
			test.translateDesynthesized(classGen, methodGen);

			if (test is FunctionCall)
			{
			FunctionCall call = (FunctionCall)test;
			try
			{
				Type type = call.typeCheck(Parser.SymbolTable);
				if (type != Type.Boolean)
				{
				test._falseList.add(il.append(new IFEQ(null)));
				}
			}
			catch (TypeCheckError)
			{
				// handled later!
			}
			}
			// remember end of condition
			truec = il.End;

			// The When object should be ignored completely in case it tests
			// for the support of a non-available element
			if (!when.ignore())
			{
				when.translateContents(classGen, methodGen);
			}

			// goto exit after executing the body of when
			exitHandles.Add(il.append(new GOTO(null)));
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (whens.hasNext() || otherwise != null)
			{
			nextElement = il.append(new GOTO(null));
			test.backPatchFalseList(nextElement);
			}
			else
			{
			test.backPatchFalseList(exit = il.append(NOP));
			}
			test.backPatchTrueList(truec.Next);
		}

		// Translate any <xsl:otherwise> element
		if (otherwise != null)
		{
			nextElement.Target = il.append(NOP);
			otherwise.translateContents(classGen, methodGen);
			exit = il.append(NOP);
		}

		// now that end is known set targets of exit gotos
		IEnumerator exitGotos = exitHandles.GetEnumerator();
		while (exitGotos.MoveNext())
		{
			BranchHandle gotoExit = (BranchHandle)exitGotos.Current;
			gotoExit.Target = exit;
		}
		}
	}

}