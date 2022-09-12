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
 * $Id: ExtendedType.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm.@ref
{

	/// <summary>
	/// The class ExtendedType represents an extended type object used by
	/// ExpandedNameTable.
	/// </summary>
	public sealed class ExtendedType
	{
		private int nodetype;
		private string @namespace;
		private string localName;
		private int hash;

		/// <summary>
		/// Create an ExtendedType object from node type, namespace and local name.
		/// The hash code is calculated from the node type, namespace and local name.
		/// </summary>
		/// <param name="nodetype"> Type of the node </param>
		/// <param name="namespace"> Namespace of the node </param>
		/// <param name="localName"> Local name of the node </param>
		public ExtendedType(int nodetype, string @namespace, string localName)
		{
		  this.nodetype = nodetype;
		  this.@namespace = @namespace;
		  this.localName = localName;
		  this.hash = nodetype + @namespace.GetHashCode() + localName.GetHashCode();
		}

		/// <summary>
		/// Create an ExtendedType object from node type, namespace, local name
		/// and a given hash code.
		/// </summary>
		/// <param name="nodetype"> Type of the node </param>
		/// <param name="namespace"> Namespace of the node </param>
		/// <param name="localName"> Local name of the node </param>
		/// <param name="hash"> The given hash code </param>
		public ExtendedType(int nodetype, string @namespace, string localName, int hash)
		{
		  this.nodetype = nodetype;
		  this.@namespace = @namespace;
		  this.localName = localName;
		  this.hash = hash;
		}

		/// <summary>
		/// Redefine this ExtendedType object to represent a different extended type.
		/// This is intended to be used ONLY on the hashET object. Using it elsewhere
		/// will mess up existing hashtable entries!
		/// </summary>
		protected internal void redefine(int nodetype, string @namespace, string localName)
		{
		  this.nodetype = nodetype;
		  this.@namespace = @namespace;
		  this.localName = localName;
		  this.hash = nodetype + @namespace.GetHashCode() + localName.GetHashCode();
		}

		/// <summary>
		/// Redefine this ExtendedType object to represent a different extended type.
		/// This is intended to be used ONLY on the hashET object. Using it elsewhere
		/// will mess up existing hashtable entries!
		/// </summary>
		protected internal void redefine(int nodetype, string @namespace, string localName, int hash)
		{
		  this.nodetype = nodetype;
		  this.@namespace = @namespace;
		  this.localName = localName;
		  this.hash = hash;
		}

		/// <summary>
		/// Override the hashCode() method in the Object class
		/// </summary>
		public override int GetHashCode()
		{
		  return hash;
		}

		/// <summary>
		/// Test if this ExtendedType object is equal to the given ExtendedType.
		/// </summary>
		/// <param name="other"> The other ExtendedType object to test for equality </param>
		/// <returns> true if the two ExtendedType objects are equal. </returns>
		public bool Equals(ExtendedType other)
		{
		  try
		  {
			return other.nodetype == this.nodetype && other.localName.Equals(this.localName) && other.@namespace.Equals(this.@namespace);
		  }
		  catch (System.NullReferenceException)
		  {
			return false;
		  }
		}

		/// <summary>
		/// Return the node type
		/// </summary>
		public int NodeType
		{
			get
			{
			  return nodetype;
			}
		}

		/// <summary>
		/// Return the local name
		/// </summary>
		public string LocalName
		{
			get
			{
			  return localName;
			}
		}

		/// <summary>
		/// Return the namespace
		/// </summary>
		public string Namespace
		{
			get
			{
			  return @namespace;
			}
		}

	}

}