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
 * $Id: OutlineableChunkEnd.java 1225426 2011-12-29 04:13:08Z mrglavas $
 */

namespace org.apache.xalan.xsltc.compiler.util
{

	using Instruction = org.apache.bcel.generic.Instruction;

	/// <summary>
	/// <para>Marks the end of a region of byte code that can be copied into a new
	/// method.  See the <seealso cref="OutlineableChunkStart"/> pseudo-instruction for
	/// details.</para>
	/// </summary>
	internal class OutlineableChunkEnd : MarkerInstruction
	{
		/// <summary>
		/// A constant instance of <seealso cref="OutlineableChunkEnd"/>.  As it has no fields,
		/// there should be no need to create an instance of this class. 
		/// </summary>
		public static readonly Instruction OUTLINEABLECHUNKEND = new OutlineableChunkEnd();

		/// <summary>
		/// Private default constructor.  As it has no fields,
		/// there should be no need to create an instance of this class.  See
		/// <seealso cref="OutlineableChunkEnd#OUTLINEABLECHUNKEND"/>.
		/// </summary>
		private OutlineableChunkEnd()
		{
		}

		/// <summary>
		/// Get the name of this instruction.  Used for debugging. </summary>
		/// <returns> the instruction name </returns>
		public virtual string Name
		{
			get
			{
				return typeof(OutlineableChunkEnd).Name;
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
		public virtual string ToString(bool verbose)
		{
			return Name;
		}
	}

}