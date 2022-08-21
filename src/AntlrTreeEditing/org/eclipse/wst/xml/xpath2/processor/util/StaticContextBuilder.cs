using System.Collections;
using System.Collections.Generic;
using java.net;
using java.xml;
using javax.xml.@namespace;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
///     Jesper Steen Moller - bug 343804 - Updated API information
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.util
{
	using URI = java.net.URI;
	using XMLConstants = java.xml.XMLConstants;
	using NamespaceContext = javax.xml.@namespace.NamespaceContext;
	using QName = javax.xml.@namespace.QName;
	using CollationProvider = org.eclipse.wst.xml.xpath2.api.CollationProvider;
	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.api.FunctionLibrary;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using StaticVariableResolver = org.eclipse.wst.xml.xpath2.api.StaticVariableResolver;
	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using FnFunctionLibrary = org.eclipse.wst.xml.xpath2.processor.function.FnFunctionLibrary;
	using XSCtrLibrary = org.eclipse.wst.xml.xpath2.processor.function.XSCtrLibrary;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public class StaticContextBuilder : StaticContext
	{
		private bool InstanceFieldsInitialized = false;

		public StaticContextBuilder()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
            _collationProvider = new CollationProviderAnonymousInnerClass(this);
		}

		private void InitializeInstanceFields()
		{
			_functionLibraries[XPATH_FUNCTIONS_NS] = new FnFunctionLibrary();
			_functionLibraries[XSCtrLibrary.XML_SCHEMA_NS] = new XSCtrLibrary();
		}


		public const string XPATH_FUNCTIONS_NS = "http://www.w3.org/2005/xpath-functions";

		private bool _xpath1_compatible = false;
		private string _default_namespace = "";
		private string _default_function_namespace = XPATH_FUNCTIONS_NS;
		private TypeDefinition _initialContextType = null;
		private string _defaultCollation = org.eclipse.wst.xml.xpath2.api.CollationProvider_Fields.CODEPOINT_COLLATION;

		// key: String prefix, contents: String namespace
		private Dictionary<string, string> _namespaces = new Dictionary<string, string>();
		private IDictionary<string, FunctionLibrary> _functionLibraries = new Dictionary<string, FunctionLibrary>();

		private URI _base_uri;
		private IDictionary _variableTypes = new Hashtable();
		private IDictionary _variableCardinality = new Hashtable();
		private IDictionary _collectionTypes = new Hashtable();

		private HashSet<QName> _hiddenFunctions = new HashSet<QName>();

		private TypeModel _typeModel;

		public virtual bool XPath1Compatible
		{
			get
			{
				return _xpath1_compatible;
			}
		}

		public virtual NamespaceContext NamespaceContext
		{
			get
			{
				return new NamespaceContextAnonymousInnerClass(this);
			}
		}

		private class NamespaceContextAnonymousInnerClass : NamespaceContext
		{
			private readonly StaticContextBuilder outerInstance;

			public NamespaceContextAnonymousInnerClass(StaticContextBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual IEnumerator<string> getPrefixes(string ns)
			{
				var prefixes = new List<string>();
				for (var it = outerInstance._namespaces.GetEnumerator(); it.MoveNext();)
				{
					var entry = it.Current;

					if (entry.Value.Equals(ns))
					{
						prefixes.Add(entry.Key);
					}
				}
				return prefixes.GetEnumerator();
			}

			public virtual string getPrefix(string ns)
			{
				for (var it = outerInstance._namespaces.GetEnumerator(); it.MoveNext();)
				{
					var entry = it.Current;
					if (entry.Value.Equals(ns))
					{
						return entry.Key;
					}
				}
				return null;
			}

			public override string getNamespaceURI(string prefix)
			{
				string ns = outerInstance._namespaces[prefix];
				if (string.ReferenceEquals(ns, null))
				{
					ns = XMLConstants.NULL_NS_URI;
				}
				return ns;
			}
		}

		public virtual string DefaultNamespace
		{
			get
			{
				return _default_namespace;
			}
		}

		public virtual string DefaultFunctionNamespace
		{
			get
			{
				return _default_function_namespace;
			}
		}

		public virtual TypeModel TypeModel
		{
			get
			{
				if (_typeModel != null)
				{
					return _typeModel;
				}
    
				return new TypeModelAnonymousInnerClass(this);
			}
		}

		private class TypeModelAnonymousInnerClass : TypeModel
		{
			private readonly StaticContextBuilder outerInstance;

			public TypeModelAnonymousInnerClass(StaticContextBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual TypeDefinition getType(Node node)
			{
				return null;
			}

			public virtual TypeDefinition lookupType(string @namespace, string typeName)
			{
				return null;
			}

			public virtual TypeDefinition lookupElementDeclaration(string @namespace, string elementName)
			{
				return null;
			}

			public virtual TypeDefinition lookupAttributeDeclaration(string @namespace, string attributeName)
			{
				return null;
			}
		}

		public virtual Function resolveFunction(QName name, int arity)
		{
			if (_hiddenFunctions.Contains(name))
			{
				return null;
			}
			FunctionLibrary flib = _functionLibraries[name.NamespaceURI];
			if (flib != null)
			{
				return flib.resolveFunction(name.LocalPart, arity);
			}
			return null;
		}

		public virtual URI BaseUri
		{
			get
			{
				return _base_uri;
			}
		}

		public virtual IDictionary<string, FunctionLibrary> FunctionLibraries
		{
			get
			{
				return _functionLibraries;
			}
		}

		public virtual TypeDefinition getCollectionType(string collectionName)
		{
			return (TypeDefinition) _collectionTypes[collectionName];
		}

		public virtual TypeDefinition InitialContextType
		{
			get
			{
				return _initialContextType;
			}
		}

		public virtual StaticContextBuilder withNamespace(string prefix, string uri)
		{
			_namespaces[prefix] = uri;
			return this;
		}

		public virtual StaticContextBuilder withDefaultNamespace(string uri)
		{
			_default_namespace = uri;
			return this;
		}

		public virtual StaticContextBuilder withXPath1Compatibility(bool compatible)
		{
			_xpath1_compatible = compatible;
			return this;
		}

		public virtual StaticContextBuilder withTypeModel(TypeModel tm)
		{
			_typeModel = tm;
			return this;
		}

		public virtual StaticContextBuilder withoutFunction(QName functionToSuppress)
		{
			_hiddenFunctions.Add(functionToSuppress);
			return this;
		}
		public virtual StaticContextBuilder withoutFunction(params QName[] functionsToSuppress)
		{
			foreach (QName name in functionsToSuppress)
			{
				_hiddenFunctions.Add(name);
			}
			return this;
		}

		public virtual TypeDefinition DefaultCollectionType
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPED;
			}
		}

		public virtual StaticVariableResolver InScopeVariables
		{
			get
			{
				return new StaticVariableResolverAnonymousInnerClass(this);
			}
		}

		private class StaticVariableResolverAnonymousInnerClass : StaticVariableResolver
		{
			private readonly StaticContextBuilder outerInstance;

			public StaticVariableResolverAnonymousInnerClass(StaticContextBuilder outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual bool isVariablePresent(QName name)
			{
				return outerInstance._variableTypes.Contains(name);
			}

			public virtual ItemType getVariableType(QName name)
			{
				return (ItemType) outerInstance._variableTypes[name];
			}
		}

		// We are explicitly NOT using generics here, in anticipation of JDK1.4 compatibility
		private static IComparer CODEPOINT_COMPARATOR = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer
		{
			public ComparatorAnonymousInnerClass()
			{
			}


			public virtual int compare(object o1, object o2)
			{
				return ((string)o1).CompareTo((string)o2);
			}

            public int Compare(object o1, object o2)
            {
                return ((string)o1).CompareTo((string)o2);
            }
		}

        private CollationProvider _collationProvider = null;

		private class CollationProviderAnonymousInnerClass : CollationProvider
		{
			public CollationProviderAnonymousInnerClass(StaticContextBuilder builder)
            {
                DefaultCollation = builder._defaultCollation;
            }


			public virtual string DefaultCollation
			{
				get; private set;
			}

			public virtual IComparer<string> getCollation(string uri)
			{
				//if (org.eclipse.wst.xml.xpath2.api.CollationProvider_Fields.CODEPOINT_COLLATION.Equals(uri))
				//{
				//	return CODEPOINT_COMPARATOR;
				//}
				return null;
			}
		}

		public virtual CollationProvider CollationProvider
		{
			get
			{
				return _collationProvider;
			}
		}

		public virtual StaticContextBuilder withCollationProvider(CollationProvider cp)
		{
			_collationProvider = cp;
			return this;
		}

		public virtual StaticContextBuilder withVariable(QName qName, ItemType type)
		{
			_variableTypes[qName] = type;
			return this;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StaticContextBuilder withBaseUri(String string) throws java.net.URISyntaxException
		public virtual StaticContextBuilder withBaseUri(string @string)
		{
			_base_uri = new URI(@string);
			return this;
		}

		public virtual StaticContextBuilder withFunctionLibrary(string @namespace, FunctionLibrary fl)
		{
			_functionLibraries[@namespace] = fl;
			return this;
		}

		public virtual StaticContextBuilder withDefaultCollation(string uri)
		{
			this._defaultCollation = uri;
			return this;
		}

		public virtual ItemType getDocumentType(URI documentUri)
		{
			return null;
		}
	}

}