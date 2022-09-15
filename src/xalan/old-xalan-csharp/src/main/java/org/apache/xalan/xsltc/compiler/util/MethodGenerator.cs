using System;
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
 * $Id: MethodGenerator.java 1225436 2011-12-29 05:09:31Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{


	using Constants = org.apache.bcel.Constants;
	using Field = org.apache.bcel.classfile.Field;
	using Method = org.apache.bcel.classfile.Method;
	using ALOAD = org.apache.bcel.generic.ALOAD;
	using ASTORE = org.apache.bcel.generic.ASTORE;
	using BranchHandle = org.apache.bcel.generic.BranchHandle;
	using BranchInstruction = org.apache.bcel.generic.BranchInstruction;
	using ConstantPoolGen = org.apache.bcel.generic.ConstantPoolGen;
	using DLOAD = org.apache.bcel.generic.DLOAD;
	using DSTORE = org.apache.bcel.generic.DSTORE;
	using FLOAD = org.apache.bcel.generic.FLOAD;
	using FSTORE = org.apache.bcel.generic.FSTORE;
	using GETFIELD = org.apache.bcel.generic.GETFIELD;
	using GOTO = org.apache.bcel.generic.GOTO;
	using ICONST = org.apache.bcel.generic.ICONST;
	using IfInstruction = org.apache.bcel.generic.IfInstruction;
	using ILOAD = org.apache.bcel.generic.ILOAD;
	using IndexedInstruction = org.apache.bcel.generic.IndexedInstruction;
	using INVOKEINTERFACE = org.apache.bcel.generic.INVOKEINTERFACE;
	using INVOKESPECIAL = org.apache.bcel.generic.INVOKESPECIAL;
	using INVOKESTATIC = org.apache.bcel.generic.INVOKESTATIC;
	using INVOKEVIRTUAL = org.apache.bcel.generic.INVOKEVIRTUAL;
	using ISTORE = org.apache.bcel.generic.ISTORE;
	using Instruction = org.apache.bcel.generic.Instruction;
	using InstructionConstants = org.apache.bcel.generic.InstructionConstants;
	using InstructionHandle = org.apache.bcel.generic.InstructionHandle;
	using InstructionList = org.apache.bcel.generic.InstructionList;
	using InstructionTargeter = org.apache.bcel.generic.InstructionTargeter;
	using LocalVariableGen = org.apache.bcel.generic.LocalVariableGen;
	using LocalVariableInstruction = org.apache.bcel.generic.LocalVariableInstruction;
	using LLOAD = org.apache.bcel.generic.LLOAD;
	using LSTORE = org.apache.bcel.generic.LSTORE;
	using MethodGen = org.apache.bcel.generic.MethodGen;
	using NEW = org.apache.bcel.generic.NEW;
	using PUTFIELD = org.apache.bcel.generic.PUTFIELD;
	using RET = org.apache.bcel.generic.RET;
	using Select = org.apache.bcel.generic.Select;
	using TargetLostException = org.apache.bcel.generic.TargetLostException;
	using Type = org.apache.bcel.generic.Type;


	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public class MethodGenerator : MethodGen, org.apache.xalan.xsltc.compiler.Constants
	{
		protected internal const int INVALID_INDEX = -1;

		private static readonly string START_ELEMENT_SIG = "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + ")V";
		private static readonly string END_ELEMENT_SIG = START_ELEMENT_SIG;

		private InstructionList _mapTypeSub;

		private const int DOM_INDEX = 1;
		private const int ITERATOR_INDEX = 2;
		private const int HANDLER_INDEX = 3;

		private const int MAX_METHOD_SIZE = 65535;
		private const int MAX_BRANCH_TARGET_OFFSET = 32767;
		private const int MIN_BRANCH_TARGET_OFFSET = -32768;

		private const int TARGET_METHOD_SIZE = 60000;
		private const int MINIMUM_OUTLINEABLE_CHUNK_SIZE = 1000;

		private Instruction _iloadCurrent;
		private Instruction _istoreCurrent;
		private readonly Instruction _astoreHandler;
		private readonly Instruction _aloadHandler;
		private readonly Instruction _astoreIterator;
		private readonly Instruction _aloadIterator;
		private readonly Instruction _aloadDom;
		private readonly Instruction _astoreDom;

		private readonly Instruction _startElement;
		private readonly Instruction _endElement;
		private readonly Instruction _startDocument;
		private readonly Instruction _endDocument;
		private readonly Instruction _attribute;
		private readonly Instruction _uniqueAttribute;
		private readonly Instruction _namespace;

		private readonly Instruction _setStartNode;
		private readonly Instruction _reset;
		private readonly Instruction _nextNode;

		private SlotAllocator _slotAllocator;
		private bool _allocatorInit = false;
		private LocalVariableRegistry _localVariableRegistry;

		/// <summary>
		/// A mapping between patterns and instruction lists used by 
		/// test sequences to avoid compiling the same pattern multiple 
		/// times. Note that patterns whose kernels are "*", "node()" 
		/// and "@*" can between shared by test sequences.
		/// </summary>
		private Hashtable _preCompiled = new Hashtable();

		public MethodGenerator(int access_flags, Type return_type, Type[] arg_types, string[] arg_names, string method_name, string class_name, InstructionList il, ConstantPoolGen cpg) : base(access_flags, return_type, arg_types, arg_names, method_name, class_name, il, cpg)
		{

		_astoreHandler = new ASTORE(HANDLER_INDEX);
		_aloadHandler = new ALOAD(HANDLER_INDEX);
		_astoreIterator = new ASTORE(ITERATOR_INDEX);
		_aloadIterator = new ALOAD(ITERATOR_INDEX);
		_aloadDom = new ALOAD(DOM_INDEX);
		_astoreDom = new ASTORE(DOM_INDEX);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startElement = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "startElement", START_ELEMENT_SIG);
		int startElement = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "startElement", START_ELEMENT_SIG);
		_startElement = new INVOKEINTERFACE(startElement, 2);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endElement = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "endElement", END_ELEMENT_SIG);
		int endElement = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "endElement", END_ELEMENT_SIG);
		_endElement = new INVOKEINTERFACE(endElement, 2);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int attribute = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "addAttribute", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + ")V");
		int attribute = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "addAttribute", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + ")V");
		_attribute = new INVOKEINTERFACE(attribute, 3);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int uniqueAttribute = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "addUniqueAttribute", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + "I)V");
		int uniqueAttribute = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "addUniqueAttribute", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + "I)V");
		_uniqueAttribute = new INVOKEINTERFACE(uniqueAttribute, 4);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int namespace = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "namespaceAfterStartElement", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + ")V");
		int @namespace = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "namespaceAfterStartElement", "(" + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + org.apache.xalan.xsltc.compiler.Constants_Fields.STRING_SIG + ")V");
		_namespace = new INVOKEINTERFACE(@namespace, 3);

		int index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "startDocument", "()V");
		_startDocument = new INVOKEINTERFACE(index, 1);

		index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.TRANSLET_OUTPUT_INTERFACE, "endDocument", "()V");
		_endDocument = new INVOKEINTERFACE(index, 1);


		index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, org.apache.xalan.xsltc.compiler.Constants_Fields.SET_START_NODE, org.apache.xalan.xsltc.compiler.Constants_Fields.SET_START_NODE_SIG);
		_setStartNode = new INVOKEINTERFACE(index, 2);

		index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, "reset", "()" + org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR_SIG);
		_reset = new INVOKEINTERFACE(index, 1);

		index = cpg.addInterfaceMethodref(org.apache.xalan.xsltc.compiler.Constants_Fields.NODE_ITERATOR, org.apache.xalan.xsltc.compiler.Constants_Fields.NEXT, org.apache.xalan.xsltc.compiler.Constants_Fields.NEXT_SIG);
		_nextNode = new INVOKEINTERFACE(index, 1);

		_slotAllocator = new SlotAllocator();
		_slotAllocator.initialize(getLocalVariableRegistry().getLocals(false));
		_allocatorInit = true;
		}

		/// <summary>
		/// Allocates a local variable. If the slot allocator has already been
		/// initialized, then call addLocalVariable2() so that the new variable
		/// is known to the allocator. Failing to do this may cause the allocator 
		/// to return a slot that is already in use.
		/// </summary>
		public virtual LocalVariableGen addLocalVariable(string name, Type type, InstructionHandle start, InstructionHandle end)
		{
			LocalVariableGen lvg;

			if (_allocatorInit)
			{
				lvg = addLocalVariable2(name, type, start);
			}
			else
			{
			lvg = base.addLocalVariable(name, type, start, end);
				getLocalVariableRegistry().registerLocalVariable(lvg);
			}

			return lvg;
		}

		public virtual LocalVariableGen addLocalVariable2(string name, Type type, InstructionHandle start)
		{
		LocalVariableGen lvg = base.addLocalVariable(name, type, _slotAllocator.allocateSlot(type), start, null);
			getLocalVariableRegistry().registerLocalVariable(lvg);
			return lvg;
		}

		private LocalVariableRegistry getLocalVariableRegistry()
		{
			if (_localVariableRegistry == null)
			{
				_localVariableRegistry = new LocalVariableRegistry(this);
			}

			return _localVariableRegistry;
		}

		/// <summary>
		/// Keeps track of all local variables used in the method.
		/// <para>The
		/// <seealso cref="MethodGen#addLocalVariable(String,Type,InstructionHandle,InstructionHandle)"/></code>
		/// and
		/// <seealso cref="MethodGen#addLocalVariable(String,Type,int,InstructionHandle,InstructionHandle)"/></code>
		/// methods of <seealso cref="MethodGen"/> will only keep track of
		/// <seealso cref="LocalVariableGen"/> object until it'ss removed by a call to
		/// <seealso cref="MethodGen#removeLocalVariable(LocalVariableGen)"/>.</para>
		/// <para>In order to support efficient copying of local variables to outlined
		/// methods by
		/// <seealso cref="#outline(InstructionHandle,InstructionHandle,String,ClassGenerator)"/>,
		/// this class keeps track of all local variables defined by the method.</para>
		/// </summary>
		protected internal class LocalVariableRegistry
		{
			private readonly MethodGenerator outerInstance;

			public LocalVariableRegistry(MethodGenerator outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			/// <summary>
			/// <para>A <code>java.lang.ArrayList</code> of all
			/// <seealso cref="LocalVariableGen"/>s created for this method, indexed by the
			/// slot number of the local variable.  The JVM stack frame of local
			/// variables is divided into "slots".  A single slot can be used to
			/// store more than one variable in a method, without regard to type, so
			/// long as the byte code keeps the ranges of the two disjoint.</para>
			/// <para>If only one registration of use of a particular slot occurs, the
			/// corresponding entry of <code>_variables</code> contains the
			/// <code>LocalVariableGen</code>; if more than one occurs, the
			/// corresponding entry contains all such <code>LocalVariableGen</code>s
			/// registered for the same slot; and if none occurs, the entry will be
			/// <code>null</code>. 
			/// </para>
			/// </summary>
			protected internal ArrayList _variables = new ArrayList();

			/// <summary>
			/// Maps a name to a <seealso cref="LocalVariableGen"/>
			/// </summary>
			protected internal Hashtable _nameToLVGMap = new Hashtable();

			/// <summary>
			/// Registers a <seealso cref="org.apache.bcel.generic.LocalVariableGen"/>
			/// for this method.
			/// <para><b>Preconditions:</b>
			/// <ul>
			/// <li>The range of instructions for <code>lvg</code> does not
			/// overlap with the range of instructions for any
			/// <code>LocalVariableGen</code> with the same slot index previously
			/// registered for this method.  <b><em>(Unchecked.)</em></b></li>
			/// </ul></para> </summary>
			/// <param name="lvg"> The variable to be registered </param>
			protected internal virtual void registerLocalVariable(LocalVariableGen lvg)
			{
				int slot = lvg.Index;

				int registrySize = _variables.Count;

				// If the LocalVariableGen uses a slot index beyond any previously
				// encountered, expand the _variables, padding with intervening null
				// entries as required.
				if (slot >= registrySize)
				{
					for (int i = registrySize; i < slot; i++)
					{
						_variables.Add(null);
					}
					_variables.Add(lvg);
				}
				else
				{
					// If the LocalVariableGen reuses a slot, make sure the entry
					// in _variables contains an ArrayList and add the newly
					// registered LocalVariableGen to the list.  If the entry in
					// _variables just contains null padding, store the
					// LocalVariableGen directly.
					object localsInSlot = _variables[slot];
					if (localsInSlot != null)
					{
						if (localsInSlot is LocalVariableGen)
						{
							ArrayList listOfLocalsInSlot = new ArrayList();
							listOfLocalsInSlot.Add(localsInSlot);
							listOfLocalsInSlot.Add(lvg);
							_variables[slot] = listOfLocalsInSlot;
						}
						else
						{
							((ArrayList) localsInSlot).Add(lvg);
						}
					}
					else
					{
						_variables[slot] = lvg;
					}
				}

				registerByName(lvg);
			}

			/// <summary>
			/// <para>Find which <seealso cref="LocalVariableGen"/>, if any, is registered for a
			/// particular JVM local stack frame slot at a particular position in the
			/// byte code for the method.</para>
			/// <para><b>Preconditions:</b>
			/// <ul>
			/// <li>The <seealso cref="InstructionList#setPositions()"/> has been called for
			/// the <seealso cref="InstructionList"/> associated with this
			/// <seealso cref="MethodGenerator"/>.</li>
			/// </ul></para> </summary>
			/// <param name="slot"> the JVM local stack frame slot number </param>
			/// <param name="offset"> the position in the byte code </param>
			/// <returns> the <code>LocalVariableGen</code> for the local variable
			/// stored in the relevant slot at the relevant offset; <code>null</code>
			/// if there is none. </returns>
			protected internal virtual LocalVariableGen lookupRegisteredLocalVariable(int slot, int offset)
			{
				object localsInSlot = (_variables != null) ? _variables[slot] : null;

				// If this slot index was never used, _variables.get will return
				// null; if it was used once, it will return the LocalVariableGen;
				// more than once it will return an ArrayList of all the
				// LocalVariableGens for variables stored in that slot.  For each
				// LocalVariableGen, check whether its range includes the
				// specified offset, and return the first such encountered.
				if (localsInSlot != null)
				{
					if (localsInSlot is LocalVariableGen)
					{
						LocalVariableGen lvg = (LocalVariableGen)localsInSlot;
						if (outerInstance.offsetInLocalVariableGenRange(lvg, offset))
						{
							return lvg;
						}
					}
					else
					{
						ArrayList listOfLocalsInSlot = (ArrayList) localsInSlot;
						int size = listOfLocalsInSlot.Count;

						for (int i = 0; i < size; i++)
						{
							LocalVariableGen lvg = (LocalVariableGen)listOfLocalsInSlot[i];
							if (outerInstance.offsetInLocalVariableGenRange(lvg, offset))
							{
								return lvg;
							}
						}
					}
				}

				// No local variable stored in the specified slot at the specified
				return null;
			}

			/// <summary>
			/// <para>Set up a mapping of the name of the specified
			/// <seealso cref="LocalVariableGen"/> object to the <code>LocalVariableGen</code>
			/// itself.</para>
			/// <para>This is a bit of a hack.  XSLTC is relying on the fact that the
			/// name that is being looked up won't be duplicated, which isn't
			/// guaranteed.  It replaces code which used to call
			/// <seealso cref="MethodGen#getLocalVariables()"/> and looped through the
			/// <code>LocalVariableGen</code> objects it contained to find the one
			/// with the specified name.  However, <code>getLocalVariables()</code>
			/// has the side effect of setting the start and end for any
			/// <code>LocalVariableGen</code> which did not already have them
			/// set, which causes problems for outlining..</para>
			/// <para>See also <seealso cref="#lookUpByName(String)"/> and
			/// <seealso cref="#removeByNameTracking(LocalVariableGen)"/></P
			/// </para>
			/// </summary>
			/// <param name="lvg"> a <code>LocalVariableGen</code> </param>
			protected internal virtual void registerByName(LocalVariableGen lvg)
			{
				object duplicateNameEntry = _nameToLVGMap[lvg.Name];

				if (duplicateNameEntry == null)
				{
					_nameToLVGMap[lvg.Name] = lvg;
				}
				else
				{
					ArrayList sameNameList;

					if (duplicateNameEntry is ArrayList)
					{
						sameNameList = (ArrayList) duplicateNameEntry;
						sameNameList.Add(lvg);
					}
					else
					{
						sameNameList = new ArrayList();
						sameNameList.Add(duplicateNameEntry);
						sameNameList.Add(lvg);
					}

					_nameToLVGMap[lvg.Name] = sameNameList;
				}
			}

			/// <summary>
			/// Remove the mapping from the name of the specified
			/// <seealso cref="LocalVariableGen"/> to itself. 
			/// See also <seealso cref="#registerByName(LocalVariableGen)"/> and
			/// <seealso cref="#lookUpByName(String)"/> </summary>
			/// <param name="lvg"> a <code>LocalVariableGen</code> </param>
			protected internal virtual void removeByNameTracking(LocalVariableGen lvg)
			{
				object duplicateNameEntry = _nameToLVGMap[lvg.Name];

				if (duplicateNameEntry is ArrayList)
				{
					ArrayList sameNameList = (ArrayList) duplicateNameEntry;
					for (int i = 0; i < sameNameList.Count; i++)
					{
						if (sameNameList[i] == lvg)
						{
							sameNameList.RemoveAt(i);
							break;
						}
					}
				}
				else
				{
					_nameToLVGMap.Remove(lvg);
				}
			}

			/// <summary>
			/// <para>Given the name of a variable, finds a <seealso cref="LocalVariableGen"/>
			/// corresponding to it.</para>
			/// <para>See also <seealso cref="#registerByName(LocalVariableGen)"/> and
			/// <seealso cref="#removeByNameTracking(LocalVariableGen)"/></para> </summary>
			/// <param name="name">
			/// @return </param>
			protected internal virtual LocalVariableGen lookUpByName(string name)
			{
				LocalVariableGen lvg = null;
				object duplicateNameEntry = _nameToLVGMap[name];

				if (duplicateNameEntry is ArrayList)
				{
					ArrayList sameNameList = (ArrayList) duplicateNameEntry;

					for (int i = 0; i < sameNameList.Count; i++)
					{
						lvg = (LocalVariableGen)sameNameList[i];
						if (lvg.Name == name)
						{
							break;
						}
					}
				}
				else
				{
					lvg = (LocalVariableGen) duplicateNameEntry;
				}

				return lvg;
			}

			/// <summary>
			/// <para>Gets all <seealso cref="LocalVariableGen"/> objects for this method.</para>
			/// <para>When the <code>includeRemoved</code> argument has the value
			/// <code>false</code>, this method replaces uses of
			/// <seealso cref="MethodGen#getLocalVariables()"/> which has
			/// a side-effect of setting the start and end range for any
			/// <code>LocalVariableGen</code> if either was <code>null</code>.  That
			/// side-effect causes problems for outlining of code in XSLTC. 
			/// </para>
			/// </summary>
			/// <param name="includeRemoved"> Specifies whether all local variables ever
			/// declared should be returned (<code>true</code>) or only those not
			/// removed (<code>false</code>) </param>
			/// <returns> an array of <code>LocalVariableGen</code> containing all the
			/// local variables </returns>
			protected internal virtual LocalVariableGen[] getLocals(bool includeRemoved)
			{
				LocalVariableGen[] locals = null;
				ArrayList allVarsEverDeclared = new ArrayList();

				if (includeRemoved)
				{
					int slotCount = allVarsEverDeclared.Count;

					for (int i = 0; i < slotCount; i++)
					{
						object slotEntries = _variables[i];
						if (slotEntries != null)
						{
							if (slotEntries is ArrayList)
							{
								ArrayList slotList = (ArrayList) slotEntries;

								for (int j = 0; j < slotList.Count; j++)
								{
									allVarsEverDeclared.Add(slotList[i]);
								}
							}
							else
							{
								allVarsEverDeclared.Add(slotEntries);
							}
						}
					}
				}
				else
				{
					IEnumerator nameVarsPairsIter = _nameToLVGMap.SetOfKeyValuePairs().GetEnumerator();

					while (nameVarsPairsIter.MoveNext())
					{
						DictionaryEntry nameVarsPair = (DictionaryEntry) nameVarsPairsIter.Current;
						object vars = nameVarsPair.Value;
						if (vars != null)
						{
							if (vars is ArrayList)
							{
								ArrayList varsList = (ArrayList) vars;
								for (int i = 0; i < varsList.Count; i++)
								{
									allVarsEverDeclared.Add(varsList[i]);
								}
							}
							else
							{
								allVarsEverDeclared.Add(vars);
							}
						}
					}
				}

				locals = new LocalVariableGen[allVarsEverDeclared.Count];
				allVarsEverDeclared.toArray(locals);

				return locals;
			}
		}

		/// <summary>
		/// Determines whether a particular variable is in use at a particular offset
		/// in the byte code for this method.
		/// <para><b>Preconditions:</b>
		/// <ul>
		/// <li>The <seealso cref="InstructionList#setPositions()"/> has been called for the
		/// <seealso cref="InstructionList"/> associated with this <seealso cref="MethodGenerator"/>.
		/// </li></ul></para> </summary>
		/// <param name="lvg"> the <seealso cref="LocalVariableGen"/> for the variable </param>
		/// <param name="offset"> the position in the byte code </param>
		/// <returns> <code>true</code> if and only if the specified variable is in
		/// use at the particular byte code offset. </returns>
		internal virtual bool offsetInLocalVariableGenRange(LocalVariableGen lvg, int offset)
		{
			InstructionHandle lvgStart = lvg.Start;
			InstructionHandle lvgEnd = lvg.End;

			// If no start handle is recorded for the LocalVariableGen, it is
			// assumed to be in use from the beginning of the method.
			if (lvgStart == null)
			{
				lvgStart = InstructionList.Start;
			}

			// If no end handle is recorded for the LocalVariableGen, it is assumed
			// to be in use to the end of the method.
			if (lvgEnd == null)
			{
				lvgEnd = InstructionList.End;
			}

			// Does the range of the instruction include the specified offset?
			// Note that the InstructionHandle.getPosition method returns the
			// offset of the beginning of an instruction.  A LocalVariableGen's
			// range includes the end instruction itself, so that instruction's
			// length must be taken into consideration in computing whether the
			// varible is in range at a particular offset.
			return ((lvgStart.Position <= offset) && (lvgEnd.Position + lvgEnd.Instruction.Length >= offset));
		}

		public virtual void removeLocalVariable(LocalVariableGen lvg)
		{
		_slotAllocator.releaseSlot(lvg);
			getLocalVariableRegistry().removeByNameTracking(lvg);
		base.removeLocalVariable(lvg);
		}

		public virtual Instruction loadDOM()
		{
		return _aloadDom;
		}

		public virtual Instruction storeDOM()
		{
		return _astoreDom;
		}

		public virtual Instruction storeHandler()
		{
		return _astoreHandler;
		}

		public virtual Instruction loadHandler()
		{
		return _aloadHandler;
		}

		public virtual Instruction storeIterator()
		{
		return _astoreIterator;
		}

		public virtual Instruction loadIterator()
		{
		return _aloadIterator;
		}

		public Instruction setStartNode()
		{
		return _setStartNode;
		}

		public Instruction reset()
		{
		return _reset;
		}

		public Instruction nextNode()
		{
		return _nextNode;
		}

		public Instruction startElement()
		{
		return _startElement;
		}

		public Instruction endElement()
		{
		return _endElement;
		}

		public Instruction startDocument()
		{
		return _startDocument;
		}

		public Instruction endDocument()
		{
		return _endDocument;
		}

		public Instruction attribute()
		{
		return _attribute;
		}

		public Instruction uniqueAttribute()
		{
			return _uniqueAttribute;
		}

		public Instruction @namespace()
		{
		return _namespace;
		}

		public virtual Instruction loadCurrentNode()
		{
		if (_iloadCurrent == null)
		{
			int idx = getLocalIndex("current");
			if (idx > 0)
			{
			_iloadCurrent = new ILOAD(idx);
			}
			else
			{
			_iloadCurrent = new ICONST(0);
			}
		}
		return _iloadCurrent;
		}

		public virtual Instruction storeCurrentNode()
		{
		return _istoreCurrent != null ? _istoreCurrent : (_istoreCurrent = new ISTORE(getLocalIndex("current")));
		}

		/// <summary>
		/// by default context node is the same as current node. MK437 </summary>
		public virtual Instruction loadContextNode()
		{
		return loadCurrentNode();
		}

		public virtual Instruction storeContextNode()
		{
		return storeCurrentNode();
		}

		public virtual int getLocalIndex(string name)
		{
		return getLocalVariable(name).Index;
		}

		public virtual LocalVariableGen getLocalVariable(string name)
		{
			return getLocalVariableRegistry().lookUpByName(name);
		}

		public virtual void setMaxLocals()
		{

		// Get the current number of local variable slots
		int maxLocals = base.MaxLocals;

		// Get numer of actual variables
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.LocalVariableGen[] localVars = super.getLocalVariables();
		LocalVariableGen[] localVars = base.LocalVariables;
		if (localVars != null)
		{
			if (localVars.Length > maxLocals)
			{
			maxLocals = localVars.Length;
			}
		}

		// We want at least 5 local variable slots (for parameters)
		if (maxLocals < 5)
		{
			maxLocals = 5;
		}

		base.MaxLocals = maxLocals;
		}

		/// <summary>
		/// Add a pre-compiled pattern to this mode. 
		/// </summary>
		public virtual void addInstructionList(Pattern pattern, InstructionList ilist)
		{
			_preCompiled[pattern] = ilist;
		}

		/// <summary>
		/// Get the instruction list for a pre-compiled pattern. Used by 
		/// test sequences to avoid compiling patterns more than once.
		/// </summary>
		public virtual InstructionList getInstructionList(Pattern pattern)
		{
			return (InstructionList) _preCompiled[pattern];
		}

		/// <summary>
		/// Used to keep track of an outlineable chunk of instructions in the
		/// current method.  See <seealso cref="OutlineableChunkStart"/> and
		/// <seealso cref="OutlineableChunkEnd"/> for more information.
		/// </summary>
		private class Chunk : IComparable
		{
			/// <summary>
			/// <seealso cref="InstructionHandle"/> of the first instruction in the outlineable
			/// chunk.
			/// </summary>
			internal InstructionHandle m_start;

			/// <summary>
			/// <seealso cref="org.apache.bcel.generic.InstructionHandle"/> of the first
			/// instruction in the outlineable chunk.
			/// </summary>
			internal InstructionHandle m_end;

			/// <summary>
			/// Number of bytes in the instructions contained in this outlineable
			/// chunk.
			/// </summary>
			internal int m_size;

			/// <summary>
			/// <para>Constructor for an outlineable <seealso cref="MethodGenerator.Chunk"/>.</para>
			/// <para><b>Preconditions:</b>
			/// <ul>
			/// <li>The <seealso cref="InstructionList#setPositions()"/> has been called for
			/// the <seealso cref="InstructionList"/> associated with this
			/// <seealso cref="MethodGenerator"/>.</li>
			/// </ul></para> </summary>
			/// <param name="start"> The <seealso cref="InstructionHandle"/> of the first
			///              instruction in the outlineable chunk. </param>
			/// <param name="end"> The <seealso cref="InstructionHandle"/> of the last
			///            instruction in the outlineable chunk. </param>
			internal Chunk(InstructionHandle start, InstructionHandle end)
			{
				m_start = start;
				m_end = end;
				m_size = end.Position - start.Position;
			}

			/// <summary>
			/// Determines whether this outlineable <seealso cref="MethodGenerator.Chunk"/> is
			/// followed immediately by the argument
			/// <code>MethodGenerator.Chunk</code>, with no other intervening
			/// instructions, including <seealso cref="OutlineableChunkStart"/> or
			/// <seealso cref="OutlineableChunkEnd"/> instructions. </summary>
			/// <param name="neighbour"> an outlineable <seealso cref="MethodGenerator.Chunk"/> </param>
			/// <returns> <code>true</code> if and only if the argument chunk
			/// immediately follows <code>this</code> chunk </returns>
			internal virtual bool isAdjacentTo(Chunk neighbour)
			{
				return ChunkEnd.Next == neighbour.ChunkStart;
			}

			/// <summary>
			/// Getter method for the start of this {@linke MethodGenerator.Chunk} </summary>
			/// <returns> the <seealso cref="org.apache.bcel.generic.InstructionHandle"/> of the
			/// start of this chunk </returns>
			internal virtual InstructionHandle ChunkStart
			{
				get
				{
					return m_start;
				}
			}

			/// <summary>
			/// Getter method for the end of this <seealso cref="MethodGenerator.Chunk"/> </summary>
			/// <returns> the <seealso cref="InstructionHandle"/> of the start of this chunk </returns>
			internal virtual InstructionHandle ChunkEnd
			{
				get
				{
					return m_end;
				}
			}

			/// <summary>
			/// The size of this <seealso cref="MethodGenerator.Chunk"/> </summary>
			/// <returns> the number of bytes in the byte code represented by this
			///         chunk. </returns>
			internal virtual int ChunkSize
			{
				get
				{
					return m_size;
				}
			}

			/// <summary>
			/// Implements the <code>java.util.Comparable.compareTo(Object)</code>
			/// method.
			/// @return
			/// <ul>
			/// <li>A positive <code>int</code> if the length of <code>this</code>
			/// chunk in bytes is greater than that of <code>comparand</code></li>
			/// <li>A negative <code>int</code> if the length of <code>this</code>
			/// chunk in bytes is less than that of <code>comparand</code></li>
			/// <li>Zero, otherwise.</li>
			/// </ul> 
			/// </summary>
			public virtual int CompareTo(object comparand)
			{
				return ChunkSize - ((Chunk)comparand).ChunkSize;
			}
		}

		/// <summary>
		/// Find the outlineable chunks in this method that would be the best choices
		/// to outline, based on size and position in the method. </summary>
		/// <param name="classGen"> The <seealso cref="ClassGen"/> with which the generated methods
		///                 will be associated </param>
		/// <param name="totalMethodSize"> the size of the bytecode in the original method </param>
		/// <returns> a <code>java.util.ArrayList</code> containing the
		///  <seealso cref="MethodGenerator.Chunk"/>s that may be outlined from this method </returns>
		private ArrayList getCandidateChunks(ClassGenerator classGen, int totalMethodSize)
		{
			IEnumerator instructions = InstructionList.GetEnumerator();
			ArrayList candidateChunks = new ArrayList();
			ArrayList currLevelChunks = new ArrayList();
			Stack subChunkStack = new Stack();
			bool openChunkAtCurrLevel = false;
			bool firstInstruction = true;

			InstructionHandle currentHandle;

			if (m_openChunks != 0)
			{
				string msg = (new ErrorMsg(ErrorMsg.OUTLINE_ERR_UNBALANCED_MARKERS)).ToString();
				throw new InternalError(msg);
			}

			// Scan instructions in the method, keeping track of the nesting level
			// of outlineable chunks.
			//
			// currLevelChunks 
			//     keeps track of the child chunks of a chunk.  For each chunk,
			//     there will be a pair of entries:  the InstructionHandles for the
			//     start and for the end of the chunk
			// subChunkStack
			//     a stack containing the partially accumulated currLevelChunks for
			//     each chunk that's still open at the current position in the
			//     InstructionList.
			// candidateChunks
			//     the list of chunks which have been accepted as candidates chunks
			//     for outlining
			do
			{
				// Get the next instruction.  The loop will perform one extra
				// iteration after it reaches the end of the InstructionList, with
				// currentHandle set to null.
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				currentHandle = instructions.hasNext() ? (InstructionHandle) instructions.next() : null;
				Instruction inst = (currentHandle != null) ? currentHandle.Instruction : null;

				// At the first iteration, create a chunk representing all the
				// code in the method.  This is done just to simplify the logic -
				// this chunk can never be outlined because it will be too big.
				if (firstInstruction)
				{
					openChunkAtCurrLevel = true;
					currLevelChunks.Add(currentHandle);
					firstInstruction = false;
				}

				// Found a new chunk
				if (inst is OutlineableChunkStart)
				{
					// If last MarkerInstruction encountered was an
					// OutlineableChunkStart, this represents the first chunk
					// nested within that previous chunk - push the list of chunks
					// from the outer level onto the stack
					if (openChunkAtCurrLevel)
					{
						subChunkStack.Push(currLevelChunks);
						currLevelChunks = new ArrayList();
					}

					openChunkAtCurrLevel = true;
					currLevelChunks.Add(currentHandle);
				// Close off an open chunk
				}
				else if (currentHandle == null || inst is OutlineableChunkEnd)
				{
					ArrayList nestedSubChunks = null;

					// If the last MarkerInstruction encountered was an
					// OutlineableChunkEnd, it means that the current instruction
					// marks the end of a chunk that contained child chunks.
					// Those children might need to be examined below in case they
					// are better candidates for outlining than the current chunk.
					if (!openChunkAtCurrLevel)
					{
						nestedSubChunks = currLevelChunks;
						currLevelChunks = (ArrayList)subChunkStack.Pop();
					}

					// Get the handle for the start of this chunk (the last entry
					// in currLevelChunks)
					InstructionHandle chunkStart = (InstructionHandle) currLevelChunks[currLevelChunks.Count - 1];

					int chunkEndPosition = (currentHandle != null) ? currentHandle.Position : totalMethodSize;
					int chunkSize = chunkEndPosition - chunkStart.Position;

					// Two ranges of chunk size to consider:
					//
					// 1. [0,TARGET_METHOD_SIZE]
					//      Keep this chunk in consideration as a candidate,
					//      and ignore its subchunks, if any - there's nothing to be
					//      gained by outlining both the current chunk and its
					//      children!
					//
					// 2. (TARGET_METHOD_SIZE,+infinity)
					//      Ignore this chunk - it's too big.  Add its subchunks
					//      as candidates, after merging adjacent chunks to produce
					//      chunks that are as large as possible
					if (chunkSize <= TARGET_METHOD_SIZE)
					{
						currLevelChunks.Add(currentHandle);
					}
					else
					{
						if (!openChunkAtCurrLevel)
						{
							int childChunkCount = nestedSubChunks.Count / 2;
							if (childChunkCount > 0)
							{
								Chunk[] childChunks = new Chunk[childChunkCount];

								// Gather all the child chunks of the current chunk
								for (int i = 0; i < childChunkCount; i++)
								{
									InstructionHandle start = (InstructionHandle) nestedSubChunks[i * 2];
									InstructionHandle end = (InstructionHandle) nestedSubChunks[i * 2 + 1];

									childChunks[i] = new Chunk(start, end);
								}

								// Merge adjacent siblings
								ArrayList mergedChildChunks = mergeAdjacentChunks(childChunks);

								// Add chunks that mean minimum size requirements
								// to the list of candidate chunks for outlining
								for (int i = 0; i < mergedChildChunks.Count; i++)
								{
									Chunk mergedChunk = (Chunk)mergedChildChunks[i];
									int mergedSize = mergedChunk.ChunkSize;

									if (mergedSize >= MINIMUM_OUTLINEABLE_CHUNK_SIZE && mergedSize <= TARGET_METHOD_SIZE)
									{
										candidateChunks.Add(mergedChunk);
									}
								}
							}
						}

						// Drop the chunk which was too big
						currLevelChunks.Remove(currLevelChunks.Count - 1);
					}

					// currLevelChunks contains pairs of InstructionHandles.  If
					// its size is an odd number, the loop has encountered the
					// start of a chunk at this level, but not its end.
					openChunkAtCurrLevel = ((currLevelChunks.Count & 0x1) == 1);
				}

			} while (currentHandle != null);

			return candidateChunks;
		}

		/// <summary>
		/// Merge adjacent sibling chunks to produce larger candidate chunks for
		/// outlining </summary>
		/// <param name="chunks"> array of sibling <seealso cref="MethodGenerator.Chunk"/>s that are
		///               under consideration for outlining.  Chunks must be in
		///               the order encountered in the <seealso cref="InstructionList"/> </param>
		/// <returns> a <code>java.util.ArrayList</code> of
		///         <code>MethodGenerator.Chunk</code>s maximally merged  </returns>
		private ArrayList mergeAdjacentChunks(Chunk[] chunks)
		{
			int[] adjacencyRunStart = new int[chunks.Length];
			int[] adjacencyRunLength = new int[chunks.Length];
			bool[] chunkWasMerged = new bool[chunks.Length];

			int maximumRunOfChunks = 0;
			int startOfCurrentRun;
			int numAdjacentRuns = 0;

			ArrayList mergedChunks = new ArrayList();

			startOfCurrentRun = 0;

			// Loop through chunks, and record in adjacencyRunStart where each
			// run of adjacent chunks begins and how many are in that run.  For
			// example, given chunks A B C D E F, if A is adjacent to B, but not
			// to C, and C, D, E and F are all adjacent,
			//   adjacencyRunStart[0] == 0; adjacencyRunLength[0] == 2
			//   adjacencyRunStart[1] == 2; adjacencyRunLength[1] == 4
			for (int i = 1; i < chunks.Length; i++)
			{
				if (!chunks[i - 1].isAdjacentTo(chunks[i]))
				{
					int lengthOfRun = i - startOfCurrentRun;

					// Track the longest run of chunks found
					if (maximumRunOfChunks < lengthOfRun)
					{
						maximumRunOfChunks = lengthOfRun;
					}

					if (lengthOfRun > 1)
					{
						adjacencyRunLength[numAdjacentRuns] = lengthOfRun;
						adjacencyRunStart[numAdjacentRuns] = startOfCurrentRun;
						numAdjacentRuns++;
					}

					startOfCurrentRun = i;
				}
			}

			if (chunks.Length - startOfCurrentRun > 1)
			{
				int lengthOfRun = chunks.Length - startOfCurrentRun;

				// Track the longest run of chunks found
				if (maximumRunOfChunks < lengthOfRun)
				{
					maximumRunOfChunks = lengthOfRun;
				}

				adjacencyRunLength[numAdjacentRuns] = chunks.Length - startOfCurrentRun;
				adjacencyRunStart[numAdjacentRuns] = startOfCurrentRun;
				numAdjacentRuns++;
			}

			// Try merging adjacent chunks to come up with better sized chunks for
			// outlining.  This algorithm is not optimal, but it should be
			// reasonably fast.  Consider an example like this, where four chunks
			// of the sizes specified in brackets are adjacent.  The best way of
			// combining these chunks would be to merge the first pair and merge
			// the last three to form two chunks, but the algorithm will merge the
			// three in the middle instead, leaving three chunks in all.
			//    [25000] [25000] [20000] [1000] [20000]

			// Start by trying to merge the maximum number of adjacent chunks, and
			// work down from there.
			for (int numToMerge = maximumRunOfChunks; numToMerge > 1; numToMerge--)
			{
				// Look at each run of adjacent chunks
				for (int run = 0; run < numAdjacentRuns; run++)
				{
					int runStart = adjacencyRunStart[run];
					int runEnd = runStart + adjacencyRunLength[run] - 1;

					bool foundChunksToMerge = false;

					// Within the current run of adjacent chunks, look at all
					// "subruns" of length numToMerge, until we run out or find
					// a subrun that can be merged.
					for (int mergeStart = runStart; mergeStart + numToMerge-1 <= runEnd && !foundChunksToMerge; mergeStart++)
					{
						int mergeEnd = mergeStart + numToMerge - 1;
						int mergeSize = 0;

						// Find out how big the subrun is
						for (int j = mergeStart; j <= mergeEnd; j++)
						{
							mergeSize = mergeSize + chunks[j].ChunkSize;
						}

						// If the current subrun is small enough to outline,
						// merge it, and split the remaining chunks in the run
						if (mergeSize <= TARGET_METHOD_SIZE)
						{
							foundChunksToMerge = true;

							for (int j = mergeStart; j <= mergeEnd; j++)
							{
								chunkWasMerged[j] = true;
							}

							mergedChunks.Add(new Chunk(chunks[mergeStart].ChunkStart, chunks[mergeEnd].ChunkEnd));

							// Adjust the length of the current run of adjacent
							// chunks to end at the newly merged chunk...
							adjacencyRunLength[run] = adjacencyRunStart[run] - mergeStart;

							int trailingRunLength = runEnd - mergeEnd;

							// and any chunks that follow the newly merged chunk
							// in the current run of adjacent chunks form another
							// new run of adjacent chunks
							if (trailingRunLength >= 2)
							{
								adjacencyRunStart[numAdjacentRuns] = mergeEnd + 1;
								adjacencyRunLength[numAdjacentRuns] = trailingRunLength;
								numAdjacentRuns++;
							}
						}
					}
				}
			}

			// Make a final pass for any chunk that wasn't merged with a sibling
			// and include it in the list of chunks after merging.
			for (int i = 0; i < chunks.Length; i++)
			{
				if (!chunkWasMerged[i])
				{
					mergedChunks.Add(chunks[i]);
				}
			}

			return mergedChunks;
		}

		/// <summary>
		/// Breaks up the IL for this <seealso cref="MethodGenerator"/> into separate
		/// outlined methods so that no method exceeds the 64KB limit on the length
		/// of the byte code associated with a method. </summary>
		/// <param name="classGen"> The <seealso cref="ClassGen"/> with which the generated methods
		///                 will be associated </param>
		/// <param name="originalMethodSize"> The number of bytes of bytecode represented by
		///                 the <seealso cref="InstructionList"/> of this method </param>
		/// <returns> an array of the outlined <code>Method</code>s and the original
		///         method itself </returns>
		public virtual Method[] outlineChunks(ClassGenerator classGen, int originalMethodSize)
		{
			ArrayList methodsOutlined = new ArrayList();
			int currentMethodSize = originalMethodSize;

			int outlinedCount = 0;
			bool moreMethodsOutlined;
			string originalMethodName = Name;

			// Special handling for initialization methods.  No other methods can
			// include the less than and greater than characters in their names,
			// so we munge the names here.
			if (originalMethodName.Equals("<init>"))
			{
				originalMethodName = "$lt$init$gt$";
			}
			else if (originalMethodName.Equals("<clinit>"))
			{
				originalMethodName = "$lt$clinit$gt$";
			}

			// Loop until the original method comes in under the JVM limit or
			// the loop was unable to outline any more methods
			do
			{
				// Get all the best candidates for outlining, and sort them in
				// ascending order of size
				ArrayList candidateChunks = getCandidateChunks(classGen, currentMethodSize);
				candidateChunks.Sort();

				moreMethodsOutlined = false;

				// Loop over the candidates for outlining, from the largest to the
				// smallest and outline them one at a time, until the loop has
				// outlined all or the original method comes in under the JVM
				// limit on the size of a method.
				for (int i = candidateChunks.Count - 1; i >= 0 && currentMethodSize > TARGET_METHOD_SIZE; i--)
				{
					Chunk chunkToOutline = (Chunk)candidateChunks[i];

					methodsOutlined.Add(outline(chunkToOutline.ChunkStart, chunkToOutline.ChunkEnd, originalMethodName + "$outline$" + outlinedCount, classGen));
					outlinedCount++;
					moreMethodsOutlined = true;

					InstructionList il = InstructionList;
					InstructionHandle lastInst = il.End;
					il.setPositions();

					// Check the size of the method now
					currentMethodSize = lastInst.Position + lastInst.Instruction.Length;
				}
			} while (moreMethodsOutlined && currentMethodSize > TARGET_METHOD_SIZE);

			// Outlining failed to reduce the size of the current method
			// sufficiently.  Throw an internal error.
			if (currentMethodSize > MAX_METHOD_SIZE)
			{
				string msg = (new ErrorMsg(ErrorMsg.OUTLINE_ERR_METHOD_TOO_BIG)).ToString();
				throw new InternalError(msg);
			}

			Method[] methodsArr = new Method[methodsOutlined.Count + 1];
			methodsOutlined.toArray(methodsArr);

			methodsArr[methodsOutlined.Count] = ThisMethod;

			return methodsArr;
		}

		/// <summary>
		/// Given an outlineable chunk of code in the current <seealso cref="MethodGenerator"/>
		/// move ("outline") the chunk to a new method, and replace the chunk in the
		/// old method with a reference to that new method.  No
		/// <seealso cref="OutlineableChunkStart"/> or <seealso cref="OutlineableChunkEnd"/> instructions
		/// are copied. </summary>
		/// <param name="first"> The <seealso cref="InstructionHandle"/> of the first instruction in
		///              the chunk to outline </param>
		/// <param name="last"> The <code>InstructionHandle</code> of the last instruction in
		///             the chunk to outline </param>
		/// <param name="outlinedMethodName"> The name of the new method </param>
		/// <param name="classGen"> The <seealso cref="ClassGenerator"/> of which the original
		///              and new methods will be members </param>
		/// <returns> The new <seealso cref="Method"/> containing the outlined code. </returns>
		private Method outline(InstructionHandle first, InstructionHandle last, string outlinedMethodName, ClassGenerator classGen)
		{
			// We're not equipped to deal with exception handlers yet.  Bail out!
			if (ExceptionHandlers.length != 0)
			{
				string msg = (new ErrorMsg(ErrorMsg.OUTLINE_ERR_TRY_CATCH)).ToString();
				throw new InternalError(msg);
			}

			int outlineChunkStartOffset = first.Position;
			int outlineChunkEndOffset = last.Position + last.Instruction.Length;

			ConstantPoolGen cpg = ConstantPool;

			// Create new outlined method with signature:
			//
			//   private final outlinedMethodName(CopyLocals copyLocals);
			//
			// CopyLocals is an object that is used to copy-in/copy-out local
			// variables that are used by the outlined method.   Only locals whose
			// value is potentially set or referenced outside the range of the
			// chunk that is being outlined will be represented in CopyLocals.  The
			// type of the variable for copying local variables is actually
			// generated to be unique - it is not named CopyLocals.
			//
			// The outlined method never needs to be referenced outside of this
			// class, and will never be overridden, so we mark it private final.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.InstructionList newIL = new org.apache.bcel.generic.InstructionList();
			InstructionList newIL = new InstructionList();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.XSLTC xsltc = classGen.getParser().getXSLTC();
			XSLTC xsltc = classGen.Parser.XSLTC;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String argTypeName = xsltc.getHelperClassName();
			string argTypeName = xsltc.HelperClassName;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.bcel.generic.Type[] argTypes = new org.apache.bcel.generic.Type[] {(new ObjectType(argTypeName)).toJCType()};
			Type[] argTypes = new Type[] {(new ObjectType(argTypeName)).toJCType()};
			const string argName = "copyLocals";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] argNames = new String[] {argName};
			string[] argNames = new string[] {argName};

			int methodAttributes = org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_PRIVATE | org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_FINAL;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isStaticMethod = (getAccessFlags() & org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_STATIC) != 0;
			bool isStaticMethod = (AccessFlags & org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_STATIC) != 0;

			if (isStaticMethod)
			{
				methodAttributes = methodAttributes | org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_STATIC;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MethodGenerator outlinedMethodGen = new MethodGenerator(methodAttributes, org.apache.bcel.generic.Type.VOID, argTypes, argNames, outlinedMethodName, getClassName(), newIL, cpg);
			MethodGenerator outlinedMethodGen = new MethodGenerator(methodAttributes, Type.VOID, argTypes, argNames, outlinedMethodName, ClassName, newIL, cpg);

			// Create class for copying local variables to the outlined method.
			// The fields the class will need to contain will be determined as the
			// code in the outlineable chunk is examined.
			ClassGenerator copyAreaCG = new ClassGeneratorAnonymousInnerClass(this, argTypeName, argTypeName + ".java", org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_FINAL | org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_PUBLIC | org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_SUPER, classGen.Stylesheet);
			ConstantPoolGen copyAreaCPG = copyAreaCG.ConstantPool;
			copyAreaCG.addEmptyConstructor(org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_PUBLIC);

			// Number of fields in the copy class
			int copyAreaFieldCount = 0;

			// The handle for the instruction after the last one to be outlined.
			// Note that this should never end up being null.  An outlineable chunk
			// won't contain a RETURN instruction or other branch out of the chunk,
			// and the JVM specification prohibits code in a method from just
			// "falling off the end" so this should always point to a valid handle.
			InstructionHandle limit = last.Next;

			// InstructionLists for copying values into and out of an instance of
			// CopyLocals:
			//      oldMethCoypInIL  - from locals in old method into an instance
			//                         of the CopyLocals class (oldMethCopyInIL)
			//      oldMethCopyOutIL - from CopyLocals back into locals in the old
			//                         method
			//      newMethCopyInIL  - from CopyLocals into locals in the new
			//                         method
			//      newMethCopyOutIL - from locals in new method into the instance
			//                         of the CopyLocals class
			InstructionList oldMethCopyInIL = new InstructionList();
			InstructionList oldMethCopyOutIL = new InstructionList();
			InstructionList newMethCopyInIL = new InstructionList();
			InstructionList newMethCopyOutIL = new InstructionList();

			// Allocate instance of class in which we'll copy in or copy out locals
			// and make two copies:  last copy is used to invoke constructor;
			// other two are used for references to fields in the CopyLocals object
			InstructionHandle outlinedMethodCallSetup = oldMethCopyInIL.append(new NEW(cpg.addClass(argTypeName)));
			oldMethCopyInIL.append(InstructionConstants.DUP);
			oldMethCopyInIL.append(InstructionConstants.DUP);
			oldMethCopyInIL.append(new INVOKESPECIAL(cpg.addMethodref(argTypeName, "<init>", "()V")));

			// Generate code to invoke the new outlined method, and place the code
			// on oldMethCopyOutIL
			InstructionHandle outlinedMethodRef;

			if (isStaticMethod)
			{
				outlinedMethodRef = oldMethCopyOutIL.append(new INVOKESTATIC(cpg.addMethodref(classGen.ClassName, outlinedMethodName, outlinedMethodGen.Signature)));
			}
			else
			{
				oldMethCopyOutIL.append(InstructionConstants.THIS);
				oldMethCopyOutIL.append(InstructionConstants.SWAP);
				outlinedMethodRef = oldMethCopyOutIL.append(new INVOKEVIRTUAL(cpg.addMethodref(classGen.ClassName, outlinedMethodName, outlinedMethodGen.Signature)));
			}

			// Used to keep track of the first in a sequence of
			// OutlineableChunkStart instructions
			bool chunkStartTargetMappingsPending = false;
			InstructionHandle pendingTargetMappingHandle = null;

			// Used to keep track of the last instruction that was copied
			InstructionHandle lastCopyHandle = null;

			// Keeps track of the mapping from instruction handles in the old
			// method to instruction handles in the outlined method.  Only need
			// to track instructions that are targeted by something else in the
			// generated BCEL 
			Hashtable targetMap = new Hashtable();

			// Keeps track of the mapping from local variables in the old method
			// to local variables in the outlined method.
			Hashtable localVarMap = new Hashtable();

			Hashtable revisedLocalVarStart = new Hashtable();
			Hashtable revisedLocalVarEnd = new Hashtable();

			// Pass 1: Make copies of all instructions, append them to the new list
			// and associate old instruction references with the new ones, i.e.,
			// a 1:1 mapping.  The special marker instructions are not copied.
			// Also, identify local variables whose values need to be copied into or
			// out of the new outlined method, and builds up targetMap and
			// localVarMap as described above.  The code identifies those local
			// variables first so that they can have fixed slots in the stack
			// frame for the outlined method assigned them ahead of all those
			// variables that don't need to exist for the entirety of the outlined
			// method invocation.
			for (InstructionHandle ih = first; ih != limit; ih = ih.Next)
			{
				Instruction inst = ih.Instruction;

				// MarkerInstructions are not copied, so if something else targets
				// one, the targetMap will point to the nearest copied sibling
				// InstructionHandle:  for an OutlineableChunkEnd, the nearest
				// preceding sibling; for an OutlineableChunkStart, the nearest
				// following sibling.
				if (inst is MarkerInstruction)
				{
					if (ih.hasTargeters())
					{
						if (inst is OutlineableChunkEnd)
						{
							targetMap[ih] = lastCopyHandle;
						}
						else
						{
							if (!chunkStartTargetMappingsPending)
							{
								chunkStartTargetMappingsPending = true;
								pendingTargetMappingHandle = ih;
							}
						}
					}
				}
				else
				{
					// Copy the instruction and append it to the outlined method's
					// InstructionList.
					Instruction c = inst.copy(); // Use clone for shallow copy

					if (c is BranchInstruction)
					{
						lastCopyHandle = newIL.append((BranchInstruction)c);
					}
					else
					{
						lastCopyHandle = newIL.append(c);
					}

					if (c is LocalVariableInstruction || c is RET)
					{
						// For any instruction that touches a local variable,
						// check whether the local variable's value needs to be
						// copied into or out of the outlined method.  If so,
						// generate the code to perform the necessary copying, and 
						// use localVarMap to map the variable in the original
						// method to the variable in the new method.
						IndexedInstruction lvi = (IndexedInstruction)c;
						int oldLocalVarIndex = lvi.Index;
						LocalVariableGen oldLVG = getLocalVariableRegistry().lookupRegisteredLocalVariable(oldLocalVarIndex, ih.Position);
						LocalVariableGen newLVG = (LocalVariableGen)localVarMap[oldLVG];

						// Has the code already mapped this local variable to a
						// local in the new method?
						if (localVarMap[oldLVG] == null)
						{
							// Determine whether the local variable needs to be
							// copied into or out of the outlined by checking
							// whether the range of instructions in which the
							// variable is accessible is outside the range of
							// instructions in the outlineable chunk.
							// Special case a chunk start offset of zero:  a local
							// variable live at that position must be a method
							// parameter, so the code doesn't need to check whether
							// the variable is live before that point; being live
							// at offset zero is sufficient to know that the value
							// must be copied in to the outlined method.
							bool copyInLocalValue = offsetInLocalVariableGenRange(oldLVG, (outlineChunkStartOffset != 0) ? outlineChunkStartOffset - 1 : 0);
							bool copyOutLocalValue = offsetInLocalVariableGenRange(oldLVG, outlineChunkEndOffset + 1);

							// For any variable that needs to be copied into or out
							// of the outlined method, create a field in the
							// CopyLocals class, and generate the necessary code for
							// copying the value.
							if (copyInLocalValue || copyOutLocalValue)
							{
								string varName = oldLVG.Name;
								Type varType = oldLVG.Type;
								newLVG = outlinedMethodGen.addLocalVariable(varName, varType, null, null);
								int newLocalVarIndex = newLVG.Index;
								string varSignature = varType.Signature;

								// Record the mapping from the old local to the new
								localVarMap[oldLVG] = newLVG;

								copyAreaFieldCount++;
								string copyAreaFieldName = "field" + copyAreaFieldCount;
								copyAreaCG.addField(new Field(org.apache.xalan.xsltc.compiler.Constants_Fields.ACC_PUBLIC, copyAreaCPG.addUtf8(copyAreaFieldName), copyAreaCPG.addUtf8(varSignature), null, copyAreaCPG.ConstantPool));

								int fieldRef = cpg.addFieldref(argTypeName, copyAreaFieldName, varSignature);

								if (copyInLocalValue)
								{
									// Generate code for the old method to store the
									// value of the local into the correct field in
									// CopyLocals prior to invocation of the
									// outlined method.
									oldMethCopyInIL.append(InstructionConstants.DUP);
									InstructionHandle copyInLoad = oldMethCopyInIL.append(loadLocal(oldLocalVarIndex, varType));
									oldMethCopyInIL.append(new PUTFIELD(fieldRef));

									// If the end of the live range of the old
									// variable was in the middle of the outlined
									// chunk.  Make the load of its value the new
									// end of its range.
									if (!copyOutLocalValue)
									{
										revisedLocalVarEnd[oldLVG] = copyInLoad;
									}

									// Generate code for start of the outlined
									// method to copy the value from a field in
									// CopyLocals to the new local in the outlined
									// method
									newMethCopyInIL.append(InstructionConstants.ALOAD_1);
									newMethCopyInIL.append(new GETFIELD(fieldRef));
									newMethCopyInIL.append(storeLocal(newLocalVarIndex, varType));
								}

								if (copyOutLocalValue)
								{
									// Generate code for the end of the outlined
									// method to copy the value from the new local
									// variable into a field in CopyLocals
									// method
									newMethCopyOutIL.append(InstructionConstants.ALOAD_1);
									newMethCopyOutIL.append(loadLocal(newLocalVarIndex, varType));
									newMethCopyOutIL.append(new PUTFIELD(fieldRef));

									// Generate code to copy the value from a field
									// in CopyLocals into a local in the original
									// method following invocation of the outlined
									// method.
									oldMethCopyOutIL.append(InstructionConstants.DUP);
									oldMethCopyOutIL.append(new GETFIELD(fieldRef));
									InstructionHandle copyOutStore = oldMethCopyOutIL.append(storeLocal(oldLocalVarIndex, varType));

									// If the start of the live range of the old
									// variable was in the middle of the outlined
									// chunk.  Make this store into it the new start
									// of its range.
									if (!copyInLocalValue)
									{
										revisedLocalVarStart[oldLVG] = copyOutStore;
									}
								}
							}
						}
					}

					if (ih.hasTargeters())
					{
						targetMap[ih] = lastCopyHandle;
					}

					// If this is the first instruction copied following a sequence
					// of OutlineableChunkStart instructions, indicate that the
					// sequence of old instruction all map to this newly created
					// instruction
					if (chunkStartTargetMappingsPending)
					{
						do
						{
							 targetMap[pendingTargetMappingHandle] = lastCopyHandle;
							 pendingTargetMappingHandle = pendingTargetMappingHandle.Next;
						} while (pendingTargetMappingHandle != ih);

						chunkStartTargetMappingsPending = false;
					}
				}
			}

			// Pass 2: Walk old and new instruction lists, updating branch targets
			// and local variable references in the new list
			InstructionHandle ih = first;
			InstructionHandle ch = newIL.Start;

			while (ch != null)
			{
				// i == old instruction; c == copied instruction
				Instruction i = ih.Instruction;
				Instruction c = ch.Instruction;

				if (i is BranchInstruction)
				{
					BranchInstruction bc = (BranchInstruction)c;
					BranchInstruction bi = (BranchInstruction)i;
					InstructionHandle itarget = bi.Target; // old target

					// New target must be in targetMap
					InstructionHandle newTarget = (InstructionHandle)targetMap[itarget];

					bc.Target = newTarget;

					// Handle LOOKUPSWITCH or TABLESWITCH which may have many
					// target instructions
					if (bi is Select)
					{
						InstructionHandle[] itargets = ((Select)bi).Targets;
						InstructionHandle[] ctargets = ((Select)bc).Targets;

						// Update all targets
						for (int j = 0; j < itargets.Length; j++)
						{
							ctargets[j] = (InstructionHandle)targetMap[itargets[j]];
						}
					}
				}
				else if (i is LocalVariableInstruction || i is RET)
				{
					// For any instruction that touches a local variable,
					// map the location of the variable in the original
					// method to its location in the new method.
					IndexedInstruction lvi = (IndexedInstruction)c;
					int oldLocalVarIndex = lvi.Index;
					LocalVariableGen oldLVG = getLocalVariableRegistry().lookupRegisteredLocalVariable(oldLocalVarIndex, ih.Position);
					LocalVariableGen newLVG = (LocalVariableGen)localVarMap[oldLVG];
					int newLocalVarIndex;

					if (newLVG == null)
					{
						// Create new variable based on old variable - use same
						// name and type, but we will let the variable be active
						// for the entire outlined method.
						// LocalVariableGen oldLocal = oldLocals[oldLocalVarIndex];
						string varName = oldLVG.Name;
						Type varType = oldLVG.Type;
						newLVG = outlinedMethodGen.addLocalVariable(varName, varType, null, null);
						newLocalVarIndex = newLVG.Index;
						localVarMap[oldLVG] = newLVG;

						// The old variable's live range was wholly contained in
						// the outlined chunk.  There should no longer be stores
						// of values into it or loads of its value, so we can just
						// mark its live range as the reference to the outlined
						// method.
						revisedLocalVarStart[oldLVG] = outlinedMethodRef;
						revisedLocalVarEnd[oldLVG] = outlinedMethodRef;
					}
					else
					{
						newLocalVarIndex = newLVG.Index;
					}
					lvi.Index = newLocalVarIndex;
				}

				// If the old instruction marks the end of the range of a local
				// variable, make sure that any slots on the stack reserved for
				// local variables are made available for reuse by calling
				// MethodGenerator.removeLocalVariable
				if (ih.hasTargeters())
				{
					InstructionTargeter[] targeters = ih.Targeters;

					for (int idx = 0; idx < targeters.Length; idx++)
					{
						InstructionTargeter targeter = targeters[idx];

						if (targeter is LocalVariableGen && ((LocalVariableGen)targeter).End == ih)
						{
							object newLVG = localVarMap[targeter];
							if (newLVG != null)
							{
								outlinedMethodGen.removeLocalVariable((LocalVariableGen)newLVG);
							}
						}
					}
				}

				// If the current instruction in the original list was a marker,
				// it wasn't copied, so don't advance through the list of copied
				// instructions yet.
				if (!(i is MarkerInstruction))
				{
					ch = ch.Next;
				}
				ih = ih.Next;

			}

			// POP the reference to the CopyLocals object from the stack
			oldMethCopyOutIL.append(InstructionConstants.POP);

			// Now that the generation of the outlined code is complete, update
			// the old local variables with new start and end ranges, as required.
			IEnumerator revisedLocalVarStartPairIter = revisedLocalVarStart.SetOfKeyValuePairs().GetEnumerator();
			while (revisedLocalVarStartPairIter.MoveNext())
			{
				DictionaryEntry lvgRangeStartPair = (DictionaryEntry)revisedLocalVarStartPairIter.Current;
				LocalVariableGen lvg = (LocalVariableGen)lvgRangeStartPair.Key;
				InstructionHandle startInst = (InstructionHandle)lvgRangeStartPair.Value;

				lvg.Start = startInst;

			}

			IEnumerator revisedLocalVarEndPairIter = revisedLocalVarEnd.SetOfKeyValuePairs().GetEnumerator();
			while (revisedLocalVarEndPairIter.MoveNext())
			{
				DictionaryEntry lvgRangeEndPair = (DictionaryEntry)revisedLocalVarEndPairIter.Current;
				LocalVariableGen lvg = (LocalVariableGen)lvgRangeEndPair.Key;
				InstructionHandle endInst = (InstructionHandle)lvgRangeEndPair.Value;

				lvg.End = endInst;
			}

			xsltc.dumpClass(copyAreaCG.JavaClass);

			// Assemble the instruction lists so that the old method invokes the
			// new outlined method
			InstructionList oldMethodIL = InstructionList;

			oldMethodIL.insert(first, oldMethCopyInIL);
			oldMethodIL.insert(first, oldMethCopyOutIL);

			// Insert the copying code into the outlined method
			newIL.insert(newMethCopyInIL);
			newIL.append(newMethCopyOutIL);
			newIL.append(InstructionConstants.RETURN);

			// Discard instructions in outlineable chunk from old method
			try
			{
				oldMethodIL.delete(first, last);
			}
			catch (TargetLostException e)
			{
				InstructionHandle[] targets = e.Targets;
				// If there were still references to old instructions lingering,
				// clean those up.  The only instructions targetting the deleted
				// instructions should have been part of the chunk that was just
				// deleted, except that instructions might branch to the start of
				// the outlined chunk; similarly, all the live ranges of local
				// variables should have been adjusted, except for unreferenced
				// variables.
				for (int i = 0; i < targets.Length; i++)
				{
					InstructionHandle lostTarget = targets[i];
					InstructionTargeter[] targeters = lostTarget.Targeters;
					for (int j = 0; j < targeters.Length; j++)
					{
						if (targeters[j] is LocalVariableGen)
						{
							LocalVariableGen lvgTargeter = (LocalVariableGen) targeters[j];
							// In the case of any lingering variable references,
							// just make the live range point to the outlined
							// function reference.  Such variables should be unused
							// anyway.
							if (lvgTargeter.Start == lostTarget)
							{
								lvgTargeter.Start = outlinedMethodRef;
							}
							if (lvgTargeter.End == lostTarget)
							{
								lvgTargeter.End = outlinedMethodRef;
							}
						}
						else
						{
							targeters[j].updateTarget(lostTarget, outlinedMethodCallSetup);
						}
					}
				}
			}

			// Make a copy for the new method of all exceptions that might be thrown
			string[] exceptions = Exceptions;
			for (int i = 0; i < exceptions.Length; i++)
			{
				outlinedMethodGen.addException(exceptions[i]);
			}

			return outlinedMethodGen.ThisMethod;
		}

		private class ClassGeneratorAnonymousInnerClass : ClassGenerator
		{
			private readonly MethodGenerator outerInstance;

			public ClassGeneratorAnonymousInnerClass(MethodGenerator outerInstance, string argTypeName, string java", int ACC_SUPER, org.apache.xalan.xsltc.compiler.Stylesheet getStylesheet) //Tangible note: chained constructor call
			{
				base(argTypeName, org.apache.xalan.xsltc.compiler.Constants_Fields.OBJECT_CLASS, java", ACC_SUPER, null, getStylesheet);
				this.outerInstance = outerInstance;
			}

			public override bool External
			{
				get
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Helper method to generate an instance of a subclass of
		/// <seealso cref="LoadInstruction"/> based on the specified <seealso cref="Type"/> that will
		/// load the specified local variable </summary>
		/// <param name="index"> the JVM stack frame index of the variable that is to be
		/// loaded </param>
		/// <param name="type"> the <seealso cref="Type"/> of the variable </param>
		/// <returns> the generated <seealso cref="LoadInstruction"/> </returns>
		private static Instruction loadLocal(int index, Type type)
		{
			if (type == Type.BOOLEAN)
			{
			   return new ILOAD(index);
			}
			else if (type == Type.INT)
			{
			   return new ILOAD(index);
			}
			else if (type == Type.SHORT)
			{
			   return new ILOAD(index);
			}
			else if (type == Type.LONG)
			{
			   return new LLOAD(index);
			}
			else if (type == Type.BYTE)
			{
			   return new ILOAD(index);
			}
			else if (type == Type.CHAR)
			{
			   return new ILOAD(index);
			}
			else if (type == Type.FLOAT)
			{
			   return new FLOAD(index);
			}
			else if (type == Type.DOUBLE)
			{
			   return new DLOAD(index);
			}
			else
			{
			   return new ALOAD(index);
			}
		}

		/// <summary>
		/// Helper method to generate an instance of a subclass of
		/// <seealso cref="StoreInstruction"/> based on the specified <seealso cref="Type"/> that will
		/// store a value in the specified local variable </summary>
		/// <param name="index"> the JVM stack frame index of the variable that is to be
		/// stored </param>
		/// <param name="type"> the <seealso cref="Type"/> of the variable </param>
		/// <returns> the generated <seealso cref="StoredInstruction"/> </returns>
		private static Instruction storeLocal(int index, Type type)
		{
			if (type == Type.BOOLEAN)
			{
			   return new ISTORE(index);
			}
			else if (type == Type.INT)
			{
			   return new ISTORE(index);
			}
			else if (type == Type.SHORT)
			{
			   return new ISTORE(index);
			}
			else if (type == Type.LONG)
			{
			   return new LSTORE(index);
			}
			else if (type == Type.BYTE)
			{
			   return new ISTORE(index);
			}
			else if (type == Type.CHAR)
			{
			   return new ISTORE(index);
			}
			else if (type == Type.FLOAT)
			{
			   return new FSTORE(index);
			}
			else if (type == Type.DOUBLE)
			{
			   return new DSTORE(index);
			}
			else
			{
			   return new ASTORE(index);
			}
		}

		/// <summary>
		/// Track the number of outlineable chunks seen.
		/// </summary>
		private int m_totalChunks = 0;

		/// <summary>
		/// Track the number of outlineable chunks started but not yet ended.  Used
		/// to detect imbalances in byte code generation.
		/// </summary>
		private int m_openChunks = 0;

		/// <summary>
		/// Mark the end of the method's
		/// <seealso cref="InstructionList"/> as the start of an outlineable chunk of code.
		/// The outlineable chunk begins after the <seealso cref="InstructionHandle"/> that is
		/// at the end of the method's <seealso cref="InstructionList"/>, or at the start of
		/// the method if the <code>InstructionList</code> is empty.
		/// See <seealso cref="OutlineableChunkStart"/> for more information.
		/// </summary>
		public virtual void markChunkStart()
		{
			// m_chunkTree.markChunkStart();
			InstructionList.append(OutlineableChunkStart.OUTLINEABLECHUNKSTART);
			m_totalChunks++;
			m_openChunks++;
		}

		/// <summary>
		/// Mark the end of an outlineable chunk of code.  See
		/// <seealso cref="OutlineableChunkStart"/> for more information.
		/// </summary>
		public virtual void markChunkEnd()
		{
			// m_chunkTree.markChunkEnd();
			InstructionList.append(OutlineableChunkEnd.OUTLINEABLECHUNKEND);
			m_openChunks--;
			if (m_openChunks < 0)
			{
				string msg = (new ErrorMsg(ErrorMsg.OUTLINE_ERR_UNBALANCED_MARKERS)).ToString();
				throw new InternalError(msg);
			}
		}

		/// <summary>
		/// <para>Get all <seealso cref="Method"/>s generated by this <seealso cref="MethodGenerator"/>.
		/// The <seealso cref="MethodGen#getMethod()"/> only returns a single
		/// <code>Method</code> object.  This method takes into account the Java
		/// Virtual Machine Specification limit of 64KB on the size of a method, and
		/// may return more than one <code>Method</code>.</para>
		/// <para>If the code associated with the <code>MethodGenerator</code> would
		/// exceed the 64KB limit, this method will attempt to split the code in
		/// the <seealso cref="InstructionList"/> associated with this
		/// <code>MethodGenerator</code> into several methods.</para> </summary>
		/// <param name="classGen"> the <seealso cref="ClassGenerator"/> of which these methods are
		///                 members </param>
		/// <returns> an array of all the <code>Method</code>s generated </returns>
		internal virtual Method[] getGeneratedMethods(ClassGenerator classGen)
		{
			Method[] generatedMethods;
			InstructionList il = InstructionList;
			InstructionHandle last = il.End;

			il.setPositions();

			int instructionListSize = last.Position + last.Instruction.Length;

			// Need to look for any branch target offsets that exceed the range
			// [-32768,32767]
			if (instructionListSize > MAX_BRANCH_TARGET_OFFSET)
			{
				bool ilChanged = widenConditionalBranchTargetOffsets();

				// If any branch instructions needed widening, recompute the size
				// of the byte code for the method
				if (ilChanged)
				{
					il.setPositions();
					last = il.End;
					instructionListSize = last.Position + last.Instruction.Length;
				}
			}

			if (instructionListSize > MAX_METHOD_SIZE)
			{
				generatedMethods = outlineChunks(classGen, instructionListSize);
			}
			else
			{
				generatedMethods = new Method[] {ThisMethod};
			}
			return generatedMethods;
		}

		protected internal virtual Method ThisMethod
		{
			get
			{
				stripAttributes(true);
				setMaxLocals();
				setMaxStack();
				removeNOPs();
    
				return Method;
			}
		}
		/// <summary>
		/// <para>Rewrites branches to avoid the JVM limits of relative branch
		/// offsets.  There is no need to invoke this method if the bytecode for the
		/// <seealso cref="MethodGenerator"/> does not exceed 32KB.</para>
		/// <para>The Java Virtual Machine Specification permits the code portion of a
		/// method to be up to 64KB in length.  However, some control transfer
		/// instructions specify relative offsets as a signed 16-bit quantity,
		/// limiting the range to a subset of the instructions that might be in a
		/// method.</para>
		/// <para>The <code>TABLESWITCH</code> and <code>LOOKUPSWITCH</code>
		/// instructions always use 32-bit signed relative offsets, so they are
		/// immune to this problem.</para>
		/// <para>The <code>GOTO</code> and <code>JSR</code>
		/// instructions come in two forms, one of which uses 16-bit relative
		/// offsets, and the other of which uses 32-bit relative offsets.  The BCEL
		/// library decides whether to use the wide form of <code>GOTO</code> or
		/// <code>JSR</code>instructions based on the relative offset of the target
		/// of the instruction without any intervention by the user of the
		/// library.</para>
		/// <para>This leaves the various conditional branch instructions,
		/// <code>IFEQ</code>, <code>IFNULL</code>, <code>IF_ICMPEQ</code>,
		/// <em>et al.</em>, all of which use 16-bit signed relative offsets, with no
		/// 32-bit wide form available.</para>
		/// <para>This method scans the <seealso cref="InstructionList"/> associated with this
		/// <seealso cref="MethodGenerator"/> and finds all conditional branch instructions
		/// that might exceed the 16-bit limitation for relative branch offsets.
		/// The logic of each such instruction is inverted, and made to target the
		/// instruction which follows it.  An unconditional branch to the original
		/// target of the instruction is then inserted between the conditional
		/// branch and the instruction which previously followed it.  The
		/// unconditional branch is permitted to have a 16-bit or a 32-bit relative
		/// offset, as described above.  For example,
		/// <code>
		/// 1234:   NOP
		///          ...
		/// 55278:  IFEQ -54044
		/// 55280:  NOP
		/// </code>
		/// is rewritten as
		/// <code>
		/// 1234:   NOP
		///          ...
		/// 55278:  IFNE 7
		/// 55280:  GOTO_W -54046
		/// 55285:  NOP
		/// </code></para>
		/// <para><b>Preconditions:</b>
		/// <ul><li>The <seealso cref="InstructionList#setPositions()"/> has been called for
		/// the <code>InstructionList</code> associated with this
		/// <code>MethodGenerator</code>.
		/// </li></ul></para>
		/// <para><b>Postconditions:</b>
		/// <ul><li>Any further changes to the <code>InstructionList</code> for this
		/// <code>MethodGenerator</code> will invalidate the changes made by this
		/// method.</li></ul>
		/// </para> </summary>
		/// <returns> <code>true</code> if the <code>InstructionList</code> was
		/// modified; <code>false</code> otherwise </returns>
		/// <seealso cref= The Java Virtual Machine Specification, Second Edition </seealso>
		internal virtual bool widenConditionalBranchTargetOffsets()
		{
			bool ilChanged = false;
			int maxOffsetChange = 0;
			InstructionList il = InstructionList;

			// Loop through all the instructions, finding those that would be
			// affected by inserting new instructions in the InstructionList, and
			// calculating the maximum amount by which the relative offset between
			// two instructions could possibly change.
			// In part this loop duplicates code in
			// org.apache.bcel.generic.InstructionList.setPosition(), which does
			// this to determine whether to use 16-bit or 32-bit offsets for GOTO
			// and JSR instructions.  Ideally, that method would do the same for
			// conditional branch instructions, but it doesn't, so we duplicate the
			// processing here.
			for (InstructionHandle ih = il.Start; ih != null; ih = ih.Next)
			{
				Instruction inst = ih.Instruction;

				switch (inst.Opcode)
				{
					// Instructions that may have 16-bit or 32-bit branch targets.
					// The size of the branch offset might increase by two bytes.
					case Constants.GOTO:
					case Constants.JSR:
						maxOffsetChange = maxOffsetChange + 2;
						break;
					// Instructions that contain padding for alignment purposes
					// Up to three bytes of padding might be needed.  For greater
					// accuracy, we should be able to discount any padding already
					// added to these instructions by InstructionList.setPosition(),
					// their APIs do not expose that information.
					case Constants.TABLESWITCH:
					case Constants.LOOKUPSWITCH:
						maxOffsetChange = maxOffsetChange + 3;
						break;
					// Instructions that might be rewritten by this method as a
					// conditional branch followed by an unconditional branch.
					// The unconditional branch would require five bytes. 
					case Constants.IF_ACMPEQ:
					case Constants.IF_ACMPNE:
					case Constants.IF_ICMPEQ:
					case Constants.IF_ICMPGE:
					case Constants.IF_ICMPGT:
					case Constants.IF_ICMPLE:
					case Constants.IF_ICMPLT:
					case Constants.IF_ICMPNE:
					case Constants.IFEQ:
					case Constants.IFGE:
					case Constants.IFGT:
					case Constants.IFLE:
					case Constants.IFLT:
					case Constants.IFNE:
					case Constants.IFNONNULL:
					case Constants.IFNULL:
						maxOffsetChange = maxOffsetChange + 5;
						break;
				}
			}

			// Now that the maximum number of bytes by which the method might grow
			// has been determined, look for conditional branches to see which
			// might possibly exceed the 16-bit relative offset.
			for (InstructionHandle ih = il.Start; ih != null; ih = ih.Next)
			{
				Instruction inst = ih.Instruction;

				if (inst is IfInstruction)
				{
					IfInstruction oldIfInst = (IfInstruction)inst;
					BranchHandle oldIfHandle = (BranchHandle)ih;
					InstructionHandle target = oldIfInst.Target;
					int relativeTargetOffset = target.Position - oldIfHandle.Position;

					// Consider the worst case scenario in which the conditional
					// branch and its target are separated by all the instructions
					// in the method that might increase in size.  If that results
					// in a relative offset that cannot be represented as a 32-bit
					// signed quantity, rewrite the instruction as described above.
					if ((relativeTargetOffset - maxOffsetChange < MIN_BRANCH_TARGET_OFFSET) || (relativeTargetOffset + maxOffsetChange > MAX_BRANCH_TARGET_OFFSET))
					{
						// Invert the logic of the IF instruction, and append
						// that to the InstructionList following the original IF
						// instruction
						InstructionHandle nextHandle = oldIfHandle.Next;
						IfInstruction invertedIfInst = oldIfInst.negate();
						BranchHandle invertedIfHandle = il.append(oldIfHandle, invertedIfInst);

						// Append an unconditional branch to the target of the
						// original IF instruction after the new IF instruction
						BranchHandle gotoHandle = il.append(invertedIfHandle, new GOTO(target));

						// If the original IF was the last instruction in
						// InstructionList, add a new no-op to act as the target
						// of the new IF
						if (nextHandle == null)
						{
							nextHandle = il.append(gotoHandle, NOP);
						}

						// Make the new IF instruction branch around the GOTO
						invertedIfHandle.updateTarget(target, nextHandle);

						// If anything still "points" to the old IF instruction,
						// make adjustments to refer to either the new IF or GOTO
						// instruction
						if (oldIfHandle.hasTargeters())
						{
							InstructionTargeter[] targeters = oldIfHandle.Targeters;

							for (int i = 0; i < targeters.Length; i++)
							{
								InstructionTargeter targeter = targeters[i];
								// Ideally, one should simply be able to use
								// InstructionTargeter.updateTarget to change
								// references to the old IF instruction to the new
								// IF instruction.  However, if a LocalVariableGen
								// indicated the old IF marked the end of the range
								// in which the IF variable is in use, the live
								// range of the variable must extend to include the
								// newly created GOTO instruction.  The need for
								// this sort of specific knowledge of an
								// implementor of the InstructionTargeter interface
								// makes the code more fragile.  Future implementors
								// of the interface might have similar requirements
								// which wouldn't be accommodated seemlessly.
								if (targeter is LocalVariableGen)
								{
									LocalVariableGen lvg = (LocalVariableGen) targeter;
									if (lvg.Start == oldIfHandle)
									{
										lvg.Start = invertedIfHandle;
									}
									else if (lvg.End == oldIfHandle)
									{
										lvg.End = gotoHandle;
									}
								}
								else
								{
									targeter.updateTarget(oldIfHandle, invertedIfHandle);
								}
							}
						}

						try
						{
							il.delete(oldIfHandle);
						}
						catch (TargetLostException tle)
						{
							// This can never happen - we updated the list of
							// instructions that target the deleted instruction
							// prior to deleting it.
							string msg = (new ErrorMsg(ErrorMsg.OUTLINE_ERR_DELETED_TARGET, tle.Message)).ToString();
							throw new InternalError(msg);
						}

						// Adjust the pointer in the InstructionList to point after
						// the newly inserted IF instruction
						ih = gotoHandle;

						// Indicate that this method rewrote at least one IF
						ilChanged = true;
					}
				}
			}

			// Did this method rewrite any IF instructions?
			return ilChanged;
		}
	}

}