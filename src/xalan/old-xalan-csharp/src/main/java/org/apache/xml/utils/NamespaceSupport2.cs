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
 * $Id: NamespaceSupport2.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	/// <summary>
	/// Encapsulate Namespace tracking logic for use by SAX drivers.
	/// 
	/// <para>This class is an attempt to rewrite the SAX NamespaceSupport
	/// "helper" class for improved efficiency. It can be used to track the
	/// namespace declarations currently in scope, providing lookup
	/// routines to map prefixes to URIs and vice versa.</para>
	/// 
	/// <para>ISSUE: For testing purposes, I've extended NamespaceSupport even
	/// though I'm completely reasserting all behaviors and fields.
	/// Wasteful.... But SAX did not put an interface under that object and
	/// we seem to have written that SAX class into our APIs... and I don't
	/// want to argue with it right now. </para>
	/// </summary>
	/// <seealso cref= org.xml.sax.helpers.NamespaceSupport
	///  </seealso>
	public class NamespaceSupport2 : org.xml.sax.helpers.NamespaceSupport
	{
		////////////////////////////////////////////////////////////////////
		// Internal state.
		////////////////////////////////////////////////////////////////////

		private Context2 currentContext; // Current point on the double-linked stack


		////////////////////////////////////////////////////////////////////
		// Constants.
		////////////////////////////////////////////////////////////////////


		/// <summary>
		/// The XML Namespace as a constant.
		/// 
		/// <para>This is the Namespace URI that is automatically mapped
		/// to the "xml" prefix.</para>
		/// </summary>
		public const string XMLNS = "http://www.w3.org/XML/1998/namespace";


		////////////////////////////////////////////////////////////////////
		// Constructor.
		////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Create a new Namespace support object.
		/// </summary>
		public NamespaceSupport2()
		{
			reset();
		}


		////////////////////////////////////////////////////////////////////
		// Context management.
		////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Reset this Namespace support object for reuse.
		/// 
		/// <para>It is necessary to invoke this method before reusing the
		/// Namespace support object for a new session.</para>
		/// </summary>
		public virtual void reset()
		{
			// Discarding the whole stack doesn't save us a lot versus
			// creating a new NamespaceSupport. Do we care, or should we
			// change this to just reset the root context?
			currentContext = new Context2(null);
			currentContext.declarePrefix("xml", XMLNS);
		}


		/// <summary>
		/// Start a new Namespace context.
		/// 
		/// <para>Normally, you should push a new context at the beginning
		/// of each XML element: the new context will automatically inherit
		/// the declarations of its parent context, but it will also keep
		/// track of which declarations were made within this context.</para>
		/// 
		/// <para>The Namespace support object always starts with a base context
		/// already in force: in this context, only the "xml" prefix is
		/// declared.</para>
		/// </summary>
		/// <seealso cref= #popContext </seealso>
		public virtual void pushContext()
		{
			// JJK: Context has a parent pointer.
			// That means we don't need a stack to pop.
			// We may want to retain for reuse, but that can be done via
			// a child pointer.

			Context2 parentContext = currentContext;
			currentContext = parentContext.Child;
			if (currentContext == null)
			{
					currentContext = new Context2(parentContext);
			}
			else
			{
				// JJK: This will wipe out any leftover data
				// if we're reusing a previously allocated Context.
				currentContext.Parent = parentContext;
			}
		}


		/// <summary>
		/// Revert to the previous Namespace context.
		/// 
		/// <para>Normally, you should pop the context at the end of each
		/// XML element.  After popping the context, all Namespace prefix
		/// mappings that were previously in force are restored.</para>
		/// 
		/// <para>You must not attempt to declare additional Namespace
		/// prefixes after popping a context, unless you push another
		/// context first.</para>
		/// </summary>
		/// <seealso cref= #pushContext </seealso>
		public virtual void popContext()
		{
			Context2 parentContext = currentContext.Parent;
			if (parentContext == null)
			{
				throw new EmptyStackException();
			}
			else
			{
				currentContext = parentContext;
			}
		}



		////////////////////////////////////////////////////////////////////
		// Operations within a context.
		////////////////////////////////////////////////////////////////////


		/// <summary>
		/// Declare a Namespace prefix.
		/// 
		/// <para>This method declares a prefix in the current Namespace
		/// context; the prefix will remain in force until this context
		/// is popped, unless it is shadowed in a descendant context.</para>
		/// 
		/// <para>To declare a default Namespace, use the empty string.  The
		/// prefix must not be "xml" or "xmlns".</para>
		/// 
		/// <para>Note that you must <em>not</em> declare a prefix after
		/// you've pushed and popped another Namespace.</para>
		/// 
		/// <para>Note that there is an asymmetry in this library: while {@link
		/// #getPrefix getPrefix} will not return the default "" prefix,
		/// even if you have declared one; to check for a default prefix,
		/// you have to look it up explicitly using <seealso cref="#getURI getURI"/>.
		/// This asymmetry exists to make it easier to look up prefixes
		/// for attribute names, where the default prefix is not allowed.</para>
		/// </summary>
		/// <param name="prefix"> The prefix to declare, or null for the empty
		///        string. </param>
		/// <param name="uri"> The Namespace URI to associate with the prefix. </param>
		/// <returns> true if the prefix was legal, false otherwise </returns>
		/// <seealso cref= #processName </seealso>
		/// <seealso cref= #getURI </seealso>
		/// <seealso cref= #getPrefix </seealso>
		public virtual bool declarePrefix(string prefix, string uri)
		{
			if (prefix.Equals("xml") || prefix.Equals("xmlns"))
			{
				return false;
			}
			else
			{
				currentContext.declarePrefix(prefix, uri);
				return true;
			}
		}


		/// <summary>
		/// Process a raw XML 1.0 name.
		/// 
		/// <para>This method processes a raw XML 1.0 name in the current
		/// context by removing the prefix and looking it up among the
		/// prefixes currently declared.  The return value will be the
		/// array supplied by the caller, filled in as follows:</para>
		/// 
		/// <dl>
		/// <dt>parts[0]</dt>
		/// <dd>The Namespace URI, or an empty string if none is
		///  in use.</dd>
		/// <dt>parts[1]</dt>
		/// <dd>The local name (without prefix).</dd>
		/// <dt>parts[2]</dt>
		/// <dd>The original raw name.</dd>
		/// </dl>
		/// 
		/// <para>All of the strings in the array will be internalized.  If
		/// the raw name has a prefix that has not been declared, then
		/// the return value will be null.</para>
		/// 
		/// <para>Note that attribute names are processed differently than
		/// element names: an unprefixed element name will received the
		/// default Namespace (if any), while an unprefixed element name
		/// will not.</para>
		/// </summary>
		/// <param name="qName"> The raw XML 1.0 name to be processed. </param>
		/// <param name="parts"> A string array supplied by the caller, capable of
		///        holding at least three members. </param>
		/// <param name="isAttribute"> A flag indicating whether this is an
		///        attribute name (true) or an element name (false). </param>
		/// <returns> The supplied array holding three internalized strings 
		///        representing the Namespace URI (or empty string), the
		///        local name, and the raw XML 1.0 name; or null if there
		///        is an undeclared prefix. </returns>
		/// <seealso cref= #declarePrefix </seealso>
		/// <seealso cref= java.lang.String#intern  </seealso>
		public virtual string [] processName(string qName, string[] parts, bool isAttribute)
		{
			string[] name = currentContext.processName(qName, isAttribute);
			if (name == null)
			{
				return null;
			}

			// JJK: This recopying is required because processName may return
			// a cached result. I Don't Like It. *****
			Array.Copy(name,0,parts,0,3);
			return parts;
		}


		/// <summary>
		/// Look up a prefix and get the currently-mapped Namespace URI.
		/// 
		/// <para>This method looks up the prefix in the current context.
		/// Use the empty string ("") for the default Namespace.</para>
		/// </summary>
		/// <param name="prefix"> The prefix to look up. </param>
		/// <returns> The associated Namespace URI, or null if the prefix
		///         is undeclared in this context. </returns>
		/// <seealso cref= #getPrefix </seealso>
		/// <seealso cref= #getPrefixes </seealso>
		public virtual string getURI(string prefix)
		{
			return currentContext.getURI(prefix);
		}


		/// <summary>
		/// Return an enumeration of all prefixes currently declared.
		/// 
		/// <para><strong>Note:</strong> if there is a default prefix, it will not be
		/// returned in this enumeration; check for the default prefix
		/// using the <seealso cref="#getURI getURI"/> with an argument of "".</para>
		/// </summary>
		/// <returns> An enumeration of all prefixes declared in the
		///         current context except for the empty (default)
		///         prefix. </returns>
		/// <seealso cref= #getDeclaredPrefixes </seealso>
		/// <seealso cref= #getURI </seealso>
		public virtual System.Collections.IEnumerator Prefixes
		{
			get
			{
				return currentContext.Prefixes;
			}
		}


		/// <summary>
		/// Return one of the prefixes mapped to a Namespace URI.
		/// 
		/// <para>If more than one prefix is currently mapped to the same
		/// URI, this method will make an arbitrary selection; if you
		/// want all of the prefixes, use the <seealso cref="#getPrefixes"/>
		/// method instead.</para>
		/// 
		/// <para><strong>Note:</strong> this will never return the empty
		/// (default) prefix; to check for a default prefix, use the {@link
		/// #getURI getURI} method with an argument of "".</para>
		/// </summary>
		/// <param name="uri"> The Namespace URI. </param>
		/// <returns> One of the prefixes currently mapped to the URI supplied,
		///         or null if none is mapped or if the URI is assigned to
		///         the default Namespace. </returns>
		/// <seealso cref= #getPrefixes(java.lang.String) </seealso>
		/// <seealso cref= #getURI  </seealso>
		public virtual string getPrefix(string uri)
		{
			return currentContext.getPrefix(uri);
		}


		/// <summary>
		/// Return an enumeration of all prefixes currently declared for a URI.
		/// 
		/// <para>This method returns prefixes mapped to a specific Namespace
		/// URI.  The xml: prefix will be included.  If you want only one
		/// prefix that's mapped to the Namespace URI, and you don't care 
		/// which one you get, use the <seealso cref="#getPrefix getPrefix"/>
		///  method instead.</para>
		/// 
		/// <para><strong>Note:</strong> the empty (default) prefix is
		/// <em>never</em> included in this enumeration; to check for the
		/// presence of a default Namespace, use the <seealso cref="#getURI getURI"/>
		/// method with an argument of "".</para>
		/// </summary>
		/// <param name="uri"> The Namespace URI. </param>
		/// <returns> An enumeration of all prefixes declared in the
		///         current context. </returns>
		/// <seealso cref= #getPrefix </seealso>
		/// <seealso cref= #getDeclaredPrefixes </seealso>
		/// <seealso cref= #getURI  </seealso>
		public virtual System.Collections.IEnumerator getPrefixes(string uri)
		{
			// JJK: The old code involved creating a vector, filling it
			// with all the matching prefixes, and then getting its
			// elements enumerator. Wastes storage, wastes cycles if we
			// don't actually need them all. Better to either implement
			// a specific enumerator for these prefixes... or a filter
			// around the all-prefixes enumerator, which comes out to
			// roughly the same thing.
			//
			// **** Currently a filter. That may not be most efficient
			// when I'm done restructuring storage!
			return new PrefixForUriEnumerator(this,uri,Prefixes);
		}


		/// <summary>
		/// Return an enumeration of all prefixes declared in this context.
		/// 
		/// <para>The empty (default) prefix will be included in this 
		/// enumeration; note that this behaviour differs from that of
		/// <seealso cref="#getPrefix"/> and <seealso cref="#getPrefixes"/>.</para>
		/// </summary>
		/// <returns> An enumeration of all prefixes declared in this
		///         context. </returns>
		/// <seealso cref= #getPrefixes </seealso>
		/// <seealso cref= #getURI </seealso>
		public virtual System.Collections.IEnumerator DeclaredPrefixes
		{
			get
			{
				return currentContext.DeclaredPrefixes;
			}
		}



	}

	////////////////////////////////////////////////////////////////////
	// Local classes.
	// These were _internal_ classes... but in fact they don't have to be,
	// and may be more efficient if they aren't. 
	////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Implementation of Enumeration filter, wrapped
	/// aroung the get-all-prefixes version of the operation. This is NOT
	/// necessarily the most efficient approach; finding the URI and then asking
	/// what prefixes apply to it might make much more sense.
	/// </summary>
	internal class PrefixForUriEnumerator : System.Collections.IEnumerator
	{
		private System.Collections.IEnumerator allPrefixes;
		private string uri;
		private string lookahead = null;
		private NamespaceSupport2 nsup;

		// Kluge: Since one can't do a constructor on an
		// anonymous class (as far as I know)...
		internal PrefixForUriEnumerator(NamespaceSupport2 nsup, string uri, System.Collections.IEnumerator allPrefixes)
		{
		this.nsup = nsup;
			this.uri = uri;
			this.allPrefixes = allPrefixes;
		}

		public virtual bool hasMoreElements()
		{
			if (!string.ReferenceEquals(lookahead, null))
			{
				return true;
			}

			while (allPrefixes.MoveNext())
			{
					string prefix = (string)allPrefixes.Current;
					if (uri.Equals(nsup.getURI(prefix)))
					{
							lookahead = prefix;
							return true;
					}
			}
			return false;
		}

		public virtual object nextElement()
		{
			if (hasMoreElements())
			{
					string tmp = lookahead;
					lookahead = null;
					return tmp;
			}
			else
			{
				throw new java.util.NoSuchElementException();
			}
		}
	}

	/// <summary>
	/// Internal class for a single Namespace context.
	/// 
	/// <para>This module caches and reuses Namespace contexts, so the number allocated
	/// will be equal to the element depth of the document, not to the total
	/// number of elements (i.e. 5-10 rather than tens of thousands).</para>
	/// </summary>
	internal sealed class Context2
	{

		////////////////////////////////////////////////////////////////
		// Manefest Constants
		////////////////////////////////////////////////////////////////

		/// <summary>
		/// An empty enumeration.
		/// </summary>
		private static readonly System.Collections.IEnumerator EMPTY_ENUMERATION = new ArrayList().elements();

		////////////////////////////////////////////////////////////////
		// Protected state.
		////////////////////////////////////////////////////////////////

		internal Hashtable prefixTable;
		internal Hashtable uriTable;
		internal Hashtable elementNameTable;
		internal Hashtable attributeNameTable;
		internal string defaultNS = null;

		////////////////////////////////////////////////////////////////
		// Internal state.
		////////////////////////////////////////////////////////////////

		private ArrayList declarations = null;
		private bool tablesDirty = false;
		private Context2 parent = null;
		private Context2 child = null;

		/// <summary>
		/// Create a new Namespace context.
		/// </summary>
		internal Context2(Context2 parent)
		{
			if (parent == null)
			{
					prefixTable = new Hashtable();
					uriTable = new Hashtable();
					elementNameTable = null;
					attributeNameTable = null;
			}
			else
			{
				Parent = parent;
			}
		}


		/// <summary>
		/// @returns The child Namespace context object, or null if this
		/// is the last currently on the chain.
		/// </summary>
		internal Context2 Child
		{
			get
			{
				return child;
			}
		}

		/// <summary>
		/// @returns The parent Namespace context object, or null if this
		/// is the root.
		/// </summary>
		internal Context2 Parent
		{
			get
			{
				return parent;
			}
			set
			{
				this.parent = value;
				value.child = this; // JJK: Doubly-linked
				declarations = null;
				prefixTable = value.prefixTable;
				uriTable = value.uriTable;
				elementNameTable = value.elementNameTable;
				attributeNameTable = value.attributeNameTable;
				defaultNS = value.defaultNS;
				tablesDirty = false;
			}
		}



		/// <summary>
		/// Declare a Namespace prefix for this context.
		/// </summary>
		/// <param name="prefix"> The prefix to declare. </param>
		/// <param name="uri"> The associated Namespace URI. </param>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#declarePrefix </seealso>
		internal void declarePrefix(string prefix, string uri)
		{
									// Lazy processing...
			if (!tablesDirty)
			{
				copyTables();
			}
			if (declarations == null)
			{
				declarations = new ArrayList();
			}

			prefix = prefix.intern();
			uri = uri.intern();
			if ("".Equals(prefix))
			{
				if ("".Equals(uri))
				{
					defaultNS = null;
				}
				else
				{
					defaultNS = uri;
				}
			}
			else
			{
				prefixTable[prefix] = uri;
				uriTable[uri] = prefix; // may wipe out another prefix
			}
			declarations.Add(prefix);
		}


		/// <summary>
		/// Process a raw XML 1.0 name in this context.
		/// </summary>
		/// <param name="qName"> The raw XML 1.0 name. </param>
		/// <param name="isAttribute"> true if this is an attribute name. </param>
		/// <returns> An array of three strings containing the
		///         URI part (or empty string), the local part,
		///         and the raw name, all internalized, or null
		///         if there is an undeclared prefix. </returns>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#processName </seealso>
		internal string [] processName(string qName, bool isAttribute)
		{
			string[] name;
			Hashtable table;

									// Select the appropriate table.
			if (isAttribute)
			{
				if (elementNameTable == null)
				{
					elementNameTable = new Hashtable();
				}
				table = elementNameTable;
			}
			else
			{
				if (attributeNameTable == null)
				{
					attributeNameTable = new Hashtable();
				}
				table = attributeNameTable;
			}

									// Start by looking in the cache, and
									// return immediately if the name
									// is already known in this content
			name = (string[])table[qName];
			if (name != null)
			{
				return name;
			}

									// We haven't seen this name in this
									// context before.
			name = new string[3];
			int index = qName.IndexOf(':');


									// No prefix.
			if (index == -1)
			{
				if (isAttribute || string.ReferenceEquals(defaultNS, null))
				{
					name[0] = "";
				}
				else
				{
					name[0] = defaultNS;
				}
				name[1] = qName.intern();
				name[2] = name[1];
			}

									// Prefix
			else
			{
				string prefix = qName.Substring(0, index);
				string local = qName.Substring(index + 1);
				string uri;
				if ("".Equals(prefix))
				{
					uri = defaultNS;
				}
				else
				{
					uri = (string)prefixTable[prefix];
				}
				if (string.ReferenceEquals(uri, null))
				{
					return null;
				}
				name[0] = uri;
				name[1] = local.intern();
				name[2] = qName.intern();
			}

									// Save in the cache for future use.
			table[name[2]] = name;
			tablesDirty = true;
			return name;
		}


		/// <summary>
		/// Look up the URI associated with a prefix in this context.
		/// </summary>
		/// <param name="prefix"> The prefix to look up. </param>
		/// <returns> The associated Namespace URI, or null if none is
		///         declared. </returns>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#getURI </seealso>
		internal string getURI(string prefix)
		{
			if ("".Equals(prefix))
			{
				return defaultNS;
			}
			else if (prefixTable == null)
			{
				return null;
			}
			else
			{
				return (string)prefixTable[prefix];
			}
		}


		/// <summary>
		/// Look up one of the prefixes associated with a URI in this context.
		/// 
		/// <para>Since many prefixes may be mapped to the same URI,
		/// the return value may be unreliable.</para>
		/// </summary>
		/// <param name="uri"> The URI to look up. </param>
		/// <returns> The associated prefix, or null if none is declared. </returns>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#getPrefix </seealso>
		internal string getPrefix(string uri)
		{
			if (uriTable == null)
			{
				return null;
			}
			else
			{
				return (string)uriTable[uri];
			}
		}


		/// <summary>
		/// Return an enumeration of prefixes declared in this context.
		/// </summary>
		/// <returns> An enumeration of prefixes (possibly empty). </returns>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#getDeclaredPrefixes </seealso>
		internal System.Collections.IEnumerator DeclaredPrefixes
		{
			get
			{
				if (declarations == null)
				{
					return EMPTY_ENUMERATION;
				}
				else
				{
					return declarations.elements();
				}
			}
		}


		/// <summary>
		/// Return an enumeration of all prefixes currently in force.
		/// 
		/// <para>The default prefix, if in force, is <em>not</em>
		/// returned, and will have to be checked for separately.</para>
		/// </summary>
		/// <returns> An enumeration of prefixes (never empty). </returns>
		/// <seealso cref= org.xml.sax.helpers.NamespaceSupport2#getPrefixes </seealso>
		internal System.Collections.IEnumerator Prefixes
		{
			get
			{
				if (prefixTable == null)
				{
					return EMPTY_ENUMERATION;
				}
				else
				{
					return prefixTable.Keys.GetEnumerator();
				}
			}
		}

		////////////////////////////////////////////////////////////////
		// Internal methods.
		////////////////////////////////////////////////////////////////

		/// <summary>
		/// Copy on write for the internal tables in this context.
		/// 
		/// <para>This class is optimized for the normal case where most
		/// elements do not contain Namespace declarations. In that case,
		/// the Context2 will share data structures with its parent.
		/// New tables are obtained only when new declarations are issued,
		/// so they can be popped off the stack.</para>
		/// 
		/// <para> JJK: **** Alternative: each Context2 might declare
		///  _only_ its local bindings, and delegate upward if not found.</para>
		/// </summary>
		private void copyTables()
		{
			// Start by copying our parent's bindings
			prefixTable = (Hashtable)prefixTable.clone();
			uriTable = (Hashtable)uriTable.clone();

			// Replace the caches with empty ones, rather than
			// trying to determine which bindings should be flushed.
			// As far as I can tell, these caches are never actually
			// used in Xalan... More efficient to remove the whole
			// cache system? ****
			if (elementNameTable != null)
			{
				elementNameTable = new Hashtable();
			}
			if (attributeNameTable != null)
			{
				attributeNameTable = new Hashtable();
			}
			tablesDirty = true;
		}

	}


	// end of NamespaceSupport2.java

}