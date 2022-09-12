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
 * $Id: SymbolTable.java 669373 2008-06-19 03:40:20Z zongaro $
 */

namespace org.apache.xalan.xsltc.compiler
{


	using MethodType = org.apache.xalan.xsltc.compiler.util.MethodType;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author Morten Jorgensen
	/// </summary>
	internal sealed class SymbolTable
	{

		// These hashtables are used for all stylesheets
		private readonly Hashtable _stylesheets = new Hashtable();
		private readonly Hashtable _primops = new Hashtable();

		// These hashtables are used for some stylesheets
		private Hashtable _variables = null;
		private Hashtable _templates = null;
		private Hashtable _attributeSets = null;
		private Hashtable _aliases = null;
		private Hashtable _excludedURI = null;
		private Stack _excludedURIStack = null;
		private Hashtable _decimalFormats = null;
		private Hashtable _keys = null;

		public DecimalFormatting getDecimalFormatting(QName name)
		{
		if (_decimalFormats == null)
		{
			return null;
		}
		return ((DecimalFormatting)_decimalFormats[name]);
		}

		public void addDecimalFormatting(QName name, DecimalFormatting symbols)
		{
		if (_decimalFormats == null)
		{
			_decimalFormats = new Hashtable();
		}
		_decimalFormats[name] = symbols;
		}

		public Key getKey(QName name)
		{
		if (_keys == null)
		{
			return null;
		}
		return (Key) _keys[name];
		}

		public void addKey(QName name, Key key)
		{
		if (_keys == null)
		{
			_keys = new Hashtable();
		}
		_keys[name] = key;
		}

		public Stylesheet addStylesheet(QName name, Stylesheet node)
		{
		return (Stylesheet)_stylesheets[name] = node;
		}

		public Stylesheet lookupStylesheet(QName name)
		{
		return (Stylesheet)_stylesheets[name];
		}

		public Template addTemplate(Template template)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final QName name = template.getName();
		QName name = template.Name;
		if (_templates == null)
		{
			_templates = new Hashtable();
		}
		return (Template)_templates[name] = template;
		}

		public Template lookupTemplate(QName name)
		{
		if (_templates == null)
		{
			return null;
		}
		return (Template)_templates[name];
		}

		public Variable addVariable(Variable variable)
		{
		if (_variables == null)
		{
			_variables = new Hashtable();
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = variable.getName().getStringRep();
		string name = variable.Name.StringRep;
		return (Variable)_variables[name] = variable;
		}

		public Param addParam(Param parameter)
		{
		if (_variables == null)
		{
			_variables = new Hashtable();
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = parameter.getName().getStringRep();
		string name = parameter.Name.StringRep;
		return (Param)_variables[name] = parameter;
		}

		public Variable lookupVariable(QName qname)
		{
		if (_variables == null)
		{
			return null;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = qname.getStringRep();
		string name = qname.StringRep;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object obj = _variables.get(name);
		object obj = _variables[name];
		return obj is Variable ? (Variable)obj : null;
		}

		public Param lookupParam(QName qname)
		{
		if (_variables == null)
		{
			return null;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = qname.getStringRep();
		string name = qname.StringRep;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Object obj = _variables.get(name);
		object obj = _variables[name];
		return obj is Param ? (Param)obj : null;
		}

		public SyntaxTreeNode lookupName(QName qname)
		{
		if (_variables == null)
		{
			return null;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = qname.getStringRep();
		string name = qname.StringRep;
		return (SyntaxTreeNode)_variables[name];
		}

		public AttributeSet addAttributeSet(AttributeSet atts)
		{
		if (_attributeSets == null)
		{
			_attributeSets = new Hashtable();
		}
		return (AttributeSet)_attributeSets[atts.Name] = atts;
		}

		public AttributeSet lookupAttributeSet(QName name)
		{
		if (_attributeSets == null)
		{
			return null;
		}
		return (AttributeSet)_attributeSets[name];
		}

		/// <summary>
		/// Add a primitive operator or function to the symbol table. To avoid
		/// name clashes with user-defined names, the prefix <tt>PrimopPrefix</tt>
		/// is prepended.
		/// </summary>
		public void addPrimop(string name, MethodType mtype)
		{
		ArrayList methods = (ArrayList)_primops[name];
		if (methods == null)
		{
			_primops[name] = methods = new ArrayList();
		}
		methods.Add(mtype);
		}

		/// <summary>
		/// Lookup a primitive operator or function in the symbol table by
		/// prepending the prefix <tt>PrimopPrefix</tt>.
		/// </summary>
		public ArrayList lookupPrimop(string name)
		{
		return (ArrayList)_primops[name];
		}

		/// <summary>
		/// This is used for xsl:attribute elements that have a "namespace"
		/// attribute that is currently not defined using xmlns:
		/// </summary>
		private int _nsCounter = 0;

		public string generateNamespacePrefix()
		{
		return ("ns" + (_nsCounter++));
		}

		/// <summary>
		/// Use a namespace prefix to lookup a namespace URI
		/// </summary>
		private SyntaxTreeNode _current = null;

		public SyntaxTreeNode CurrentNode
		{
			set
			{
			_current = value;
			}
		}

		public string lookupNamespace(string prefix)
		{
		if (_current == null)
		{
			return (Constants_Fields.EMPTYSTRING);
		}
		return (_current.lookupNamespace(prefix));
		}

		/// <summary>
		/// Adds an alias for a namespace prefix
		/// </summary>
		public void addPrefixAlias(string prefix, string alias)
		{
		if (_aliases == null)
		{
			_aliases = new Hashtable();
		}
		_aliases[prefix] = alias;
		}

		/// <summary>
		/// Retrieves any alias for a given namespace prefix
		/// </summary>
		public string lookupPrefixAlias(string prefix)
		{
		if (_aliases == null)
		{
			return null;
		}
		return (string)_aliases[prefix];
		}

		/// <summary>
		/// Register a namespace URI so that it will not be declared in the output
		/// unless it is actually referenced in the output.
		/// </summary>
		public void excludeURI(string uri)
		{
		// The null-namespace cannot be excluded
		if (string.ReferenceEquals(uri, null))
		{
			return;
		}

		// Create new hashtable of exlcuded URIs if none exists
		if (_excludedURI == null)
		{
			_excludedURI = new Hashtable();
		}

		// Register the namespace URI
		int? refcnt = (int?)_excludedURI[uri];
		if (refcnt == null)
		{
			refcnt = new int?(1);
		}
		else
		{
			refcnt = new int?(refcnt.Value + 1);
		}
		_excludedURI[uri] = refcnt;
		}

		/// <summary>
		/// Exclude a series of namespaces given by a list of whitespace
		/// separated namespace prefixes.
		/// </summary>
		public void excludeNamespaces(string prefixes)
		{
		if (!string.ReferenceEquals(prefixes, null))
		{
			StringTokenizer tokens = new StringTokenizer(prefixes);
			while (tokens.hasMoreTokens())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = tokens.nextToken();
			string prefix = tokens.nextToken();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri;
			string uri;
			if (prefix.Equals("#default"))
			{
				uri = lookupNamespace(Constants_Fields.EMPTYSTRING);
			}
			else
			{
				uri = lookupNamespace(prefix);
			}
			if (!string.ReferenceEquals(uri, null))
			{
				excludeURI(uri);
			}
			}
		}
		}

		/// <summary>
		/// Check if a namespace should not be declared in the output (unless used)
		/// </summary>
		public bool isExcludedNamespace(string uri)
		{
		if (!string.ReferenceEquals(uri, null) && _excludedURI != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Nullable<int> refcnt = (Nullable<int>)_excludedURI.get(uri);
			int? refcnt = (int?)_excludedURI[uri];
			return (refcnt != null && refcnt.Value > 0);
		}
		return false;
		}

		/// <summary>
		/// Turn of namespace declaration exclusion
		/// </summary>
		public void unExcludeNamespaces(string prefixes)
		{
		if (_excludedURI == null)
		{
			return;
		}
		if (!string.ReferenceEquals(prefixes, null))
		{
			StringTokenizer tokens = new StringTokenizer(prefixes);
			while (tokens.hasMoreTokens())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String prefix = tokens.nextToken();
			string prefix = tokens.nextToken();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String uri;
			string uri;
			if (prefix.Equals("#default"))
			{
				uri = lookupNamespace(Constants_Fields.EMPTYSTRING);
			}
			else
			{
				uri = lookupNamespace(prefix);
			}
			int? refcnt = (int?)_excludedURI[uri];
			if (refcnt != null)
			{
				_excludedURI[uri] = new int?(refcnt.Value - 1);
			}
			}
		}
		}

		/// <summary>
		/// Exclusion of namespaces by a stylesheet does not extend to any stylesheet
		/// imported or included by the stylesheet.  Upon entering the context of a
		/// new stylesheet, a call to this method is needed to clear the current set
		/// of excluded namespaces temporarily.  Every call to this method requires
		/// a corresponding call to <seealso cref="#popExcludedNamespacesContext()"/>.
		/// </summary>
		public void pushExcludedNamespacesContext()
		{
			if (_excludedURIStack == null)
			{
				_excludedURIStack = new Stack();
			}
			_excludedURIStack.Push(_excludedURI);
			_excludedURI = null;
		}

		/// <summary>
		/// Exclusion of namespaces by a stylesheet does not extend to any stylesheet
		/// imported or included by the stylesheet.  Upon exiting the context of a
		/// stylesheet, a call to this method is needed to restore the set of
		/// excluded namespaces that was in effect prior to entering the context of
		/// the current stylesheet.
		/// </summary>
		public void popExcludedNamespacesContext()
		{
			_excludedURI = (Hashtable) _excludedURIStack.Pop();
			if (_excludedURIStack.Count == 0)
			{
				_excludedURIStack = null;
			}
		}

	}


}