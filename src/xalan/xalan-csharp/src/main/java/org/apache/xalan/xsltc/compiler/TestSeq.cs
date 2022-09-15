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
 * $Id: TestSeq.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{

	using GOTO_W = org.apache.bcel.generic.GOTO_W;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using ClassGenerator = org.apache.xalan.xsltc.compiler.util.ClassGenerator;
	using MethodGenerator = org.apache.xalan.xsltc.compiler.util.MethodGenerator;

	/// <summary>
	/// A test sequence is a sequence of patterns that
	/// 
	///  (1) occured in templates in the same mode
	///  (2) share the same kernel node type (e.g. A/B and C/C/B)
	///  (3) may also contain patterns matching "*" and "node()"
	///      (element sequence only) or matching "@*" (attribute
	///      sequence only).
	/// 
	/// A test sequence may have a default template, which will be 
	/// instantiated if none of the other patterns match. 
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Erwin Bolwidt <ejb@klomp.org>
	/// @author Morten Jorgensen <morten.jorgensen@sun.com>
	/// </summary>
	internal sealed class TestSeq
	{

		/// <summary>
		/// Integer code for the kernel type of this test sequence
		/// </summary>
		private int _kernelType;

		/// <summary>
		/// Vector of all patterns in the test sequence. May include
		/// patterns with "*", "@*" or "node()" kernel.
		/// </summary>
		private ArrayList _patterns = null;

		/// <summary>
		/// A reference to the Mode object.
		/// </summary>
		private Mode _mode = null;

		/// <summary>
		/// Default template for this test sequence
		/// </summary>
		private Template _default = null;

		/// <summary>
		/// Instruction list representing this test sequence.
		/// </summary>
		private InstructionList _instructionList;

		/// <summary>
		/// Cached handle to avoid compiling more than once.
		/// </summary>
		private InstructionHandle _start = null;

		/// <summary>
		/// Creates a new test sequence given a set of patterns and a mode.
		/// </summary>
		public TestSeq(ArrayList patterns, Mode mode) : this(patterns, -2, mode)
		{
		}

		public TestSeq(ArrayList patterns, int kernelType, Mode mode)
		{
		_patterns = patterns;
		_kernelType = kernelType;
		_mode = mode;
		}

		/// <summary>
		/// Returns a string representation of this test sequence. Notice
		/// that test sequences are mutable, so the value returned by this
		/// method is different before and after calling reduce().
		/// </summary>
		public override string ToString()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _patterns.size();
		int count = _patterns.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuffer result = new StringBuffer();
		StringBuilder result = new StringBuilder();

		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern pattern = (LocationPathPattern) _patterns.elementAt(i);
			LocationPathPattern pattern = (LocationPathPattern) _patterns[i];

			if (i == 0)
			{
			result.Append("Testseq for kernel " + _kernelType).Append('\n');
			}
			result.Append("   pattern " + i + ": ").Append(pattern.ToString()).Append('\n');
		}
		return result.ToString();
		}

		/// <summary>
		/// Returns the instruction list for this test sequence
		/// </summary>
		public InstructionList InstructionList
		{
			get
			{
			return _instructionList;
			}
		}

		/// <summary>
		/// Return the highest priority for a pattern in this test
		/// sequence. This is either the priority of the first or
		/// of the default pattern.
		/// </summary>
		public double Priority
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Template template = (_patterns.size() == 0) ? _default : ((Pattern) _patterns.elementAt(0)).getTemplate();
			Template template = (_patterns.Count == 0) ? _default : ((Pattern) _patterns[0]).Template;
			return template.Priority;
			}
		}

		/// <summary>
		/// Returns the position of the highest priority pattern in 
		/// this test sequence.
		/// </summary>
		public int Position
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Template template = (_patterns.size() == 0) ? _default : ((Pattern) _patterns.elementAt(0)).getTemplate();
			Template template = (_patterns.Count == 0) ? _default : ((Pattern) _patterns[0]).Template;
			return template.Position;
			}
		}

		/// <summary>
		/// Reduce the patterns in this test sequence. Creates a new
		/// vector of patterns and sets the default pattern if it
		/// finds a patterns that is fully reduced.
		/// </summary>
		public void reduce()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector newPatterns = new java.util.Vector();
		ArrayList newPatterns = new ArrayList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _patterns.size();
		int count = _patterns.Count;
		for (int i = 0; i < count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern pattern = (LocationPathPattern)_patterns.elementAt(i);
			LocationPathPattern pattern = (LocationPathPattern)_patterns[i];

			// Reduce this pattern
			pattern.reduceKernelPattern();

			// Is this pattern fully reduced?
			if (pattern.Wildcard)
			{
			_default = pattern.Template;
			break; // Ignore following patterns
			}
			else
			{
			newPatterns.Add(pattern);
			}
		}
		_patterns = newPatterns;
		}

		/// <summary>
		/// Returns, by reference, the templates that are included in 
		/// this test sequence. Note that a single template can occur 
		/// in several test sequences if its pattern is a union.
		/// </summary>
		public void findTemplates(Dictionary templates)
		{
		if (_default != null)
		{
			templates.put(_default, this);
		}
		for (int i = 0; i < _patterns.Count; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern pattern = (LocationPathPattern)_patterns.elementAt(i);
			LocationPathPattern pattern = (LocationPathPattern)_patterns[i];
			templates.put(pattern.Template, this);
		}
		}

		/// <summary>
		/// Get the instruction handle to a template's code. This is 
		/// used when a single template occurs in several test 
		/// sequences; that is, if its pattern is a union of patterns 
		/// (e.g. match="A/B | A/C").
		/// </summary>
		private InstructionHandle getTemplateHandle(Template template)
		{
		return (InstructionHandle)_mode.getTemplateInstructionHandle(template);
		}

		/// <summary>
		/// Returns pattern n in this test sequence
		/// </summary>
		private LocationPathPattern getPattern(int n)
		{
		return (LocationPathPattern)_patterns[n];
		}

		/// <summary>
		/// Compile the code for this test sequence. Compile patterns 
		/// from highest to lowest priority. Note that since patterns 
		/// can be share by multiple test sequences, instruction lists 
		/// must be copied before backpatching.
		/// </summary>
		public InstructionHandle compile(ClassGenerator classGen, MethodGenerator methodGen, InstructionHandle continuation)
		{
		// Returned cached value if already compiled
		if (_start != null)
		{
			return _start;
		}

		// If not patterns, then return handle for default template
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = _patterns.size();
		int count = _patterns.Count;
		if (count == 0)
		{
			return (_start = getTemplateHandle(_default));
		}

		// Init handle to jump when all patterns failed
		InstructionHandle fail = (_default == null) ? continuation : getTemplateHandle(_default);

		// Compile all patterns in reverse order
		for (int n = count - 1; n >= 0; n--)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LocationPathPattern pattern = getPattern(n);
			LocationPathPattern pattern = getPattern(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Template template = pattern.getTemplate();
			Template template = pattern.Template;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList il = new org.apache.bcel.generic.InstructionList();
			InstructionList il = new InstructionList();

			// Patterns expect current node on top of stack
			il.append(methodGen.loadCurrentNode());

			// Apply the test-code compiled for the pattern
			InstructionList ilist = methodGen.getInstructionList(pattern);
			if (ilist == null)
			{
			ilist = pattern.compile(classGen, methodGen);
			methodGen.addInstructionList(pattern, ilist);
			}

			// Make a copy of the instruction list for backpatching
			InstructionList copyOfilist = ilist.copy();

			FlowList trueList = pattern.TrueList;
			if (trueList != null)
			{
			trueList = trueList.copyAndRedirect(ilist, copyOfilist);
			}
			FlowList falseList = pattern.FalseList;
			if (falseList != null)
			{
			falseList = falseList.copyAndRedirect(ilist, copyOfilist);
			}

			il.append(copyOfilist);

			// On success branch to the template code
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle gtmpl = getTemplateHandle(template);
			InstructionHandle gtmpl = getTemplateHandle(template);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionHandle success = il.append(new org.apache.bcel.generic.GOTO_W(gtmpl));
			InstructionHandle success = il.append(new GOTO_W(gtmpl));

			if (trueList != null)
			{
			trueList.backPatch(success);
			}
			if (falseList != null)
			{
			falseList.backPatch(fail);
			}

			// Next pattern's 'fail' target is this pattern's first instruction
			fail = il.getStart();

			// Append existing instruction list to the end of this one
			if (_instructionList != null)
			{
			il.append(_instructionList);
			}

			// Set current instruction list to be this one
			_instructionList = il;
		}
		return (_start = fail);
		}
	}

}