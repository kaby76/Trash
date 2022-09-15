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
 * $Id: OutlineableChunkStart.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{
	using Instruction = org.apache.bcel.generic.Instruction;

	/// <summary>
	/// <para>This pseudo-instruction marks the beginning of a region of byte code that
	/// can be copied into a new method, termed an "outlineable" chunk.  The size of
	/// the Java stack must be the same at the start of the region as it is at the
	/// end of the region, any value on the stack at the start of the region must not
	/// be consumed by an instruction in the region of code, the region must not
	/// contain a return instruction, no branch instruction in the region is
	/// permitted to have a target that is outside the region, and no branch
	/// instruction outside the region is permitted to have a target that is inside
	/// the region.</para>
	/// <para>The end of the region is marked by an <seealso cref="OutlineableChunkEnd"/>
	/// pseudo-instruction.</para>
	/// <para>Such a region of code may contain other outlineable regions.</para>
	/// </summary>
	internal class OutlineableChunkStart : MarkerInstruction
	{
		/// <summary>
		/// A constant instance of <seealso cref="OutlineableChunkStart"/>.  As it has no fields,
		/// there should be no need to create an instance of this class. 
		/// </summary>
		public static readonly Instruction OUTLINEABLECHUNKSTART = new OutlineableChunkStart();

		/// <summary>
		/// Private default constructor.  As it has no fields,
		/// there should be no need to create an instance of this class.  See
		/// <seealso cref="OutlineableChunkStart.OUTLINEABLECHUNKSTART"/>.
		/// </summary>
		private OutlineableChunkStart()
		{
		}

		/// <summary>
		/// Get the name of this instruction.  Used for debugging. </summary>
		/// <returns> the instruction name </returns>
		public virtual string Name
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				return typeof(OutlineableChunkStart).FullName;
			}
		}

		/// <summary>
		/// Get the name of this instruction.  Used for debugging. </summary>
		/// <returns> the instruction name </returns>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Get the name of this instruction.  Used for debugging. </summary>
		/// <returns> the instruction name </returns>
		public virtual string toString(bool verbose)
		{
			return Name;
		}
	}

}