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
 * $Id: NamespaceMappings.java 1225444 2011-12-29 05:52:39Z mrglavas $
 */
namespace org.apache.xml.serializer
{


	using ContentHandler = org.xml.sax.ContentHandler;
	using SAXException = org.xml.sax.SAXException;

	/// <summary>
	/// This class keeps track of the currently defined namespaces. Conceptually the
	/// prefix/uri/depth triplets are pushed on a stack pushed on a stack. The depth
	/// indicates the nesting depth of the element for which the mapping was made.
	/// 
	/// <para>For example:
	/// <pre>
	/// <chapter xmlns:p1="def">
	///   <paragraph xmlns:p2="ghi">
	///      <sentance xmlns:p3="jkl">
	///      </sentance>
	///    </paragraph>
	///    <paragraph xlmns:p4="mno">
	///    </paragraph>
	/// </chapter>
	/// </pre>
	/// 
	/// When the <chapter> element is encounted the prefix "p1" associated with uri
	/// "def" is pushed on the stack with depth 1.
	/// When the first <paragraph> is encountered "p2" and "ghi" are pushed with
	/// depth 2.
	/// When the <sentance> is encountered "p3" and "jkl" are pushed with depth 3.
	/// When </sentance> occurs the popNamespaces(3) will pop "p3"/"jkl" off the
	/// stack.  Of course popNamespaces(2) would pop anything with depth 2 or
	/// greater.
	/// 
	/// So prefix/uri pairs are pushed and poped off the stack as elements are
	/// processed.  At any given moment of processing the currently visible prefixes
	/// are on the stack and a prefix can be found given a uri, or a uri can be found
	/// given a prefix.
	/// 
	/// This class is intended for internal use only.  However, it is made public because
	/// other packages require it. 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	public class NamespaceMappings
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			m_nodeStack = new Stack(this);
		}

		/// <summary>
		/// This member is continually incremented when new prefixes need to be
		/// generated. ("ns0"  "ns1" ...)
		/// </summary>
		private int count = 0;

		/// <summary>
		/// Each entry (prefix) in this hashtable points to a Stack of URIs
		/// This table maps a prefix (String) to a Stack of NamespaceNodes.
		/// All Namespace nodes in that retrieved stack have the same prefix,
		/// though possibly different URI's or depths. Such a stack must have
		/// mappings at deeper depths push later on such a stack.  Mappings pushed
		/// earlier on the stack will have smaller values for MappingRecord.m_declarationDepth.
		/// </summary>
		private Hashtable m_namespaces = new Hashtable();

		/// <summary>
		/// This stack is used as a convenience.
		/// It contains the pushed NamespaceNodes (shallowest
		/// to deepest) and is used to delete NamespaceNodes 
		/// when leaving the current element depth 
		/// to returning to the parent. The mappings of the deepest
		/// depth can be popped of the top and the same node
		/// can be removed from the appropriate prefix stack.
		/// 
		/// All prefixes pushed at the current depth can be 
		/// removed at the same time by using this stack to
		/// ensure prefix/uri map scopes are closed correctly.
		/// </summary>
		private Stack m_nodeStack;

		private const string EMPTYSTRING = "";
		private const string XML_PREFIX = "xml"; // was "xmlns"

		/// <summary>
		/// Default constructor </summary>
		/// <seealso cref= java.lang.Object#Object() </seealso>
		public NamespaceMappings()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			initNamespaces();
		}

		/// <summary>
		/// This method initializes the namespace object with appropriate stacks
		/// and predefines a few prefix/uri pairs which always exist.
		/// </summary>
		private void initNamespaces()
		{
			// The initial prefix mappings will never be deleted because they are at element depth -1 
			// (a kludge)

			// Define the default namespace (initially maps to "" uri)
			Stack stack;
			MappingRecord nn;
			nn = new MappingRecord(EMPTYSTRING, EMPTYSTRING, -1);
			stack = createPrefixStack(EMPTYSTRING);
			stack.push(nn);

			// define "xml" namespace
			nn = new MappingRecord(XML_PREFIX, "http://www.w3.org/XML/1998/namespace", -1);
			stack = createPrefixStack(XML_PREFIX);
			stack.push(nn);
		}

		/// <summary>
		/// Use a namespace prefix to lookup a namespace URI.
		/// </summary>
		/// <param name="prefix"> String the prefix of the namespace </param>
		/// <returns> the URI corresponding to the prefix, returns ""
		/// if there is no visible mapping. </returns>
		public virtual string lookupNamespace(string prefix)
		{
			string uri = null;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stack stack = getPrefixStack(prefix);
			Stack stack = getPrefixStack(prefix);
			if (stack != null && !stack.Empty)
			{
				uri = ((MappingRecord) stack.peek()).m_uri;
			}
			if (string.ReferenceEquals(uri, null))
			{
				uri = EMPTYSTRING;
			}
			return uri;
		}


		internal virtual MappingRecord getMappingFromPrefix(string prefix)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Stack stack = (Stack) m_namespaces.get(prefix);
			Stack stack = (Stack) m_namespaces[prefix];
			return stack != null && !stack.Empty ? ((MappingRecord) stack.peek()) : null;
		}

		/// <summary>
		/// Given a namespace uri, and the namespaces mappings for the 
		/// current element, return the current prefix for that uri.
		/// </summary>
		/// <param name="uri"> the namespace URI to be search for </param>
		/// <returns> an existing prefix that maps to the given URI, null if no prefix
		/// maps to the given namespace URI. </returns>
		public virtual string lookupPrefix(string uri)
		{
			string foundPrefix = null;
			System.Collections.IEnumerator prefixes = m_namespaces.Keys.GetEnumerator();
			while (prefixes.MoveNext())
			{
				string prefix = (string) prefixes.Current;
				string uri2 = lookupNamespace(prefix);
				if (!string.ReferenceEquals(uri2, null) && uri2.Equals(uri))
				{
					foundPrefix = prefix;
					break;
				}
			}
			return foundPrefix;
		}

		internal virtual MappingRecord getMappingFromURI(string uri)
		{
			MappingRecord foundMap = null;
			System.Collections.IEnumerator prefixes = m_namespaces.Keys.GetEnumerator();
			while (prefixes.MoveNext())
			{
				string prefix = (string) prefixes.Current;
				MappingRecord map2 = getMappingFromPrefix(prefix);
				if (map2 != null && (map2.m_uri).Equals(uri))
				{
					foundMap = map2;
					break;
				}
			}
			return foundMap;
		}

		/// <summary>
		/// Undeclare the namespace that is currently pointed to by a given prefix
		/// </summary>
		internal virtual bool popNamespace(string prefix)
		{
			// Prefixes "xml" and "xmlns" cannot be redefined
			if (prefix.StartsWith(XML_PREFIX, StringComparison.Ordinal))
			{
				return false;
			}

			Stack stack;
			if ((stack = getPrefixStack(prefix)) != null)
			{
				stack.pop();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Declare a mapping of a prefix to namespace URI at the given element depth. </summary>
		/// <param name="prefix"> a String with the prefix for a qualified name </param>
		/// <param name="uri"> a String with the uri to which the prefix is to map </param>
		/// <param name="elemDepth"> the depth of current declaration </param>
		public virtual bool pushNamespace(string prefix, string uri, int elemDepth)
		{
			// Prefixes "xml" and "xmlns" cannot be redefined
			if (prefix.StartsWith(XML_PREFIX, StringComparison.Ordinal))
			{
				return false;
			}

			Stack stack;
			// Get the stack that contains URIs for the specified prefix
			if ((stack = (Stack) m_namespaces[prefix]) == null)
			{
				m_namespaces[prefix] = stack = new Stack(this);
			}

			if (!stack.empty())
			{
				MappingRecord mr = (MappingRecord)stack.peek();
				if (uri.Equals(mr.m_uri) || elemDepth == mr.m_declarationDepth)
				{
					// If the same prefix/uri mapping is already on the stack
					// don't push this one.
					// Or if we have a mapping at the same depth
					// don't replace by pushing this one. 
					return false;
				}
			}
			MappingRecord map = new MappingRecord(prefix,uri,elemDepth);
			stack.push(map);
			m_nodeStack.push(map);
			return true;
		}

		/// <summary>
		/// Pop, or undeclare all namespace definitions that are currently
		/// declared at the given element depth, or deepter. </summary>
		/// <param name="elemDepth"> the element depth for which mappings declared at this
		/// depth or deeper will no longer be valid </param>
		/// <param name="saxHandler"> The ContentHandler to notify of any endPrefixMapping()
		/// calls.  This parameter can be null. </param>
		internal virtual void popNamespaces(int elemDepth, ContentHandler saxHandler)
		{
			while (true)
			{
				if (m_nodeStack.Empty)
				{
					return;
				}
				MappingRecord map = (MappingRecord)(m_nodeStack.peek());
				int depth = map.m_declarationDepth;
				if (elemDepth < 1 || map.m_declarationDepth < elemDepth)
				{
					break;
				}
				/* the depth of the declared mapping is elemDepth or deeper
				 * so get rid of it
				 */

				MappingRecord nm1 = (MappingRecord) m_nodeStack.pop();
				// pop the node from the stack
				string prefix = map.m_prefix;

				Stack prefixStack = getPrefixStack(prefix);
				MappingRecord nm2 = (MappingRecord) prefixStack.peek();
				if (nm1 == nm2)
				{
					// It would be nice to always pop() but we
					// need to check that the prefix stack still has
					// the node we want to get rid of. This is because
					// the optimization of essentially this situation:
					// <a xmlns:x="abc"><b xmlns:x="" xmlns:x="abc" /></a>
					// will remove both mappings in <b> because the
					// new mapping is the same as the masked one and we get
					// <a xmlns:x="abc"><b/></a>
					// So we are only removing xmlns:x="" or
					// xmlns:x="abc" from the depth of element <b>
					// when going back to <a> if in fact they have
					// not been optimized away.
					// 
					prefixStack.pop();
					if (saxHandler != null)
					{
						try
						{
							saxHandler.endPrefixMapping(prefix);
						}
						catch (SAXException)
						{
							// not much we can do if they aren't willing to listen
						}
					}
				}

			}
		}

		/// <summary>
		/// Generate a new namespace prefix ( ns0, ns1 ...) not used before </summary>
		/// <returns> String a new namespace prefix ( ns0, ns1, ns2 ...) </returns>
		public virtual string generateNextPrefix()
		{
			return "ns" + (count++);
		}


		/// <summary>
		/// This method makes a clone of this object.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
		public virtual object clone()
		{
			NamespaceMappings clone = new NamespaceMappings();
			clone.m_nodeStack = (NamespaceMappings.Stack) m_nodeStack.clone();
			clone.count = this.count;
			clone.m_namespaces = (Hashtable) m_namespaces.clone();

			clone.count = count;
			return clone;

		}

		internal void reset()
		{
			this.count = 0;
			this.m_namespaces.Clear();
			this.m_nodeStack.clear();

			initNamespaces();
		}

		/// <summary>
		/// Just a little class that ties the 3 fields together
		/// into one object, and this simplifies the pushing
		/// and popping of namespaces to one push or one pop on
		/// one stack rather than on 3 separate stacks.
		/// </summary>
		internal class MappingRecord
		{
			internal readonly string m_prefix; // the prefix
			internal readonly string m_uri; // the uri, possibly "" but never null
			// the depth of the element where declartion was made
			internal readonly int m_declarationDepth;
			internal MappingRecord(string prefix, string uri, int depth)
			{
				m_prefix = prefix;
				m_uri = (string.ReferenceEquals(uri, null))? EMPTYSTRING : uri;
				m_declarationDepth = depth;
			}
		}

		/// <summary>
		/// Rather than using java.util.Stack, this private class
		/// provides a minimal subset of methods and is faster
		/// because it is not thread-safe.
		/// </summary>
		private class Stack
		{
			internal bool InstanceFieldsInitialized = false;

			internal virtual void InitializeInstanceFields()
			{
				m_stack = new object[max];
			}

			private readonly NamespaceMappings outerInstance;

			internal int top = -1;
			internal int max = 20;
			internal object[] m_stack;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object clone() throws CloneNotSupportedException
			public virtual object clone()
			{
				NamespaceMappings.Stack clone = new NamespaceMappings.Stack(outerInstance);
				clone.max = this.max;
				clone.top = this.top;
				clone.m_stack = new object[clone.max];
				for (int i = 0; i <= top; i++)
				{
					// We are just copying references to immutable MappingRecord objects here
					// so it is OK if the clone has references to these.
					clone.m_stack[i] = this.m_stack[i];
				}
				return clone;
			}

			public Stack(NamespaceMappings outerInstance)
			{
				this.outerInstance = outerInstance;

				if (!InstanceFieldsInitialized)
				{
					InitializeInstanceFields();
					InstanceFieldsInitialized = true;
				}
			}

			public virtual object push(object o)
			{
				top++;
				if (max <= top)
				{
					int newMax = 2 * max + 1;
					object[] newArray = new object[newMax];
					Array.Copy(m_stack,0, newArray, 0, max);
					max = newMax;
					m_stack = newArray;
				}
				m_stack[top] = o;
				return o;
			}

			public virtual object pop()
			{
				object o;
				if (0 <= top)
				{
					o = m_stack[top];
					// m_stack[top] = null;  do we really care?
					top--;
				}
				else
				{
					o = null;
				}
				return o;
			}

			public virtual object peek()
			{
				object o;
				if (0 <= top)
				{
					o = m_stack[top];
				}
				else
				{
					o = null;
				}
				return o;
			}

			public virtual object peek(int idx)
			{
				return m_stack[idx];
			}

			public virtual bool Empty
			{
				get
				{
					return (top < 0);
				}
			}
			public virtual bool empty()
			{
				return (top < 0);
			}

			public virtual void clear()
			{
				for (int i = 0; i <= top; i++)
				{
					m_stack[i] = null;
				}
				top = -1;
			}

			public virtual object getElement(int index)
			{
				return m_stack[index];
			}
		}
		/// <summary>
		/// A more type-safe way to get a stack of prefix mappings
		/// from the Hashtable m_namespaces
		/// (this is the only method that does the type cast).
		/// </summary>

		private Stack getPrefixStack(string prefix)
		{
			Stack fs = (Stack) m_namespaces[prefix];
			return fs;
		}

		/// <summary>
		/// A more type-safe way of saving stacks under the
		/// m_namespaces Hashtable.
		/// </summary>
		private Stack createPrefixStack(string prefix)
		{
			Stack fs = new Stack(this);
			m_namespaces[prefix] = fs;
			return fs;
		}

		/// <summary>
		/// Given a namespace uri, get all prefixes bound to the Namespace URI in the current scope. 
		/// </summary>
		/// <param name="uri"> the namespace URI to be search for </param>
		/// <returns> An array of Strings which are
		/// all prefixes bound to the namespace URI in the current scope.
		/// An array of zero elements is returned if no prefixes map to the given
		/// namespace URI. </returns>
		public virtual string[] lookupAllPrefixes(string uri)
		{
			ArrayList foundPrefixes = new ArrayList();
			System.Collections.IEnumerator prefixes = m_namespaces.Keys.GetEnumerator();
			while (prefixes.MoveNext())
			{
				string prefix = (string) prefixes.Current;
				string uri2 = lookupNamespace(prefix);
				if (!string.ReferenceEquals(uri2, null) && uri2.Equals(uri))
				{
					foundPrefixes.Add(prefix);
				}
			}
			string[] prefixArray = new string[foundPrefixes.Count];
			foundPrefixes.toArray(prefixArray);
			return prefixArray;
		}
	}

}