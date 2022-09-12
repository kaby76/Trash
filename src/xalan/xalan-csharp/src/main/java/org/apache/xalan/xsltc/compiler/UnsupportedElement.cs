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
 * $Id: UnsupportedElement.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using PUSH = org.apache.bcel.generic.PUSH;

	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;
	using Type = org.apache.xalan.xsltc.compiler.util.Type;
	using TypeCheckError = org.apache.xalan.xsltc.compiler.util.TypeCheckError;
	using Util = org.apache.xalan.xsltc.compiler.util.Util;

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class UnsupportedElement : SyntaxTreeNode
	{

		private ArrayList _fallbacks = null;
		private ErrorMsg _message = null;
		private bool _isExtension = false;

		/// <summary>
		/// Basic consutrcor - stores element uri/prefix/localname
		/// </summary>
		public UnsupportedElement(string uri, string prefix, string local, bool isExtension) : base(uri, prefix, local)
		{
		_isExtension = isExtension;
		}

		/// <summary>
		/// There are different categories of unsupported elements (believe it
		/// or not): there are elements within the XSLT namespace (these would
		/// be elements that are not yet implemented), there are extensions of
		/// other XSLT processors and there are unrecognised extension elements
		/// of this XSLT processor. The error message passed to this method
		/// should describe the unsupported element itself and what category
		/// the element belongs in.
		/// </summary>
		public ErrorMsg ErrorMessage
		{
			set
			{
			_message = value;
			}
		}

		/// <summary>
		/// Displays the contents of this element
		/// </summary>
		public override void display(int indent)
		{
		indent(indent);
		Util.println("Unsupported element = " + _qname.Namespace + ":" + _qname.LocalPart);
		displayContents(indent + IndentIncrement);
		}


		/// <summary>
		/// Scan and process all fallback children of the unsupported element.
		/// </summary>
		private void processFallbacks(Parser parser)
		{

		ArrayList children = Contents;
		if (children != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = children.size();
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
			SyntaxTreeNode child = (SyntaxTreeNode)children[i];
			if (child is Fallback)
			{
				Fallback fallback = (Fallback)child;
				fallback.activate();
				fallback.parseContents(parser);
				if (_fallbacks == null)
				{
					_fallbacks = new ArrayList();
				}
				_fallbacks.Add(child);
			}
			}
		}
		}

		/// <summary>
		/// Find any fallback in the descendant nodes; then activate & parse it
		/// </summary>
		public override void parseContents(Parser parser)
		{
			processFallbacks(parser);
		}

		/// <summary>
		/// Run type check on the fallback element (if any).
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.xalan.xsltc.compiler.util.Type typeCheck(SymbolTable stable) throws org.apache.xalan.xsltc.compiler.util.TypeCheckError
		public override Type typeCheck(SymbolTable stable)
		{
		if (_fallbacks != null)
		{
			int count = _fallbacks.Count;
			for (int i = 0; i < count; i++)
			{
				Fallback fallback = (Fallback)_fallbacks[i];
				fallback.typeCheck(stable);
			}
		}
		return Type.Void;
		}

		/// <summary>
		/// Translate the fallback element (if any).
		/// </summary>
		public override void translate(ClassGenerator classGen, MethodGenerator methodGen)
		{
		if (_fallbacks != null)
		{
			int count = _fallbacks.Count;
			for (int i = 0; i < count; i++)
			{
				Fallback fallback = (Fallback)_fallbacks[i];
				fallback.translate(classGen, methodGen);
			}
		}
		// We only go into the else block in forward-compatibility mode, when
		// the unsupported element has no fallback.
		else
		{
			// If the unsupported element does not have any fallback child, then
			// at runtime, a runtime error should be raised when the unsupported
			// element is instantiated. Otherwise, no error is thrown.
			ConstantPoolGen cpg = classGen.ConstantPool;
			InstructionList il = methodGen.InstructionList;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int unsupportedElem = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "unsupported_ElementF", "(" + Constants_Fields.STRING_SIG + "Z)V");
			int unsupportedElem = cpg.addMethodref(Constants_Fields.BASIS_LIBRARY_CLASS, "unsupported_ElementF", "(" + Constants_Fields.STRING_SIG + "Z)V");
			il.append(new PUSH(cpg, QName.ToString()));
			il.append(new PUSH(cpg, _isExtension));
			il.append(new INVOKESTATIC(unsupportedElem));
		}
		}
	}

}