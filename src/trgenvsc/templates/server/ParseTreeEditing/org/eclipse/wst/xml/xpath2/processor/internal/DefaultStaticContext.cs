using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     David Carver (STAR) - bug 277792 - add built in types to static context. 
///     Jesper Steen Moller - bug 297707 - Missing the empty-sequence() type
///     Mukul Gandhi        - bug 325262 - providing ability to store an XPath2 sequence
///                                        into an user-defined variable.
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{


	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using FnFunctionLibrary = org.eclipse.wst.xml.xpath2.processor.function.FnFunctionLibrary;
	using XSCtrLibrary = org.eclipse.wst.xml.xpath2.processor.function.XSCtrLibrary;
	using ConstructorFL = org.eclipse.wst.xml.xpath2.processor.@internal.function.ConstructorFL;
	using Function = org.eclipse.wst.xml.xpath2.processor.@internal.function.Function;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.FunctionLibrary;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;
	using Node = org.w3c.dom.Node;
    using Document = org.w3c.dom.Document;

	/// <summary>
	/// Default implementation of a static context as described by the XPath 2.0
	/// specification.
	/// </summary>
	public class DefaultStaticContext : StaticContext
	{

		private bool _xpath1_compatible;
		private string _default_namespace;
		private string _default_function_namespace;
		private TypeModel _model;
		private XSCtrLibrary builtinTypes;

		// key: String prefix, contents: String namespace
		private IDictionary _namespaces;

		private string _cntxt_item_type;
		private IDictionary<string, FunctionLibrary> _functions;

		// XXX collations

		private XSAnyURI _base_uri;
		private IDictionary _documents;
		private IDictionary<string, IList<Document>> _collections;

		public virtual string get_cntxt_item_type()
		{
			return _cntxt_item_type;
		}

		public virtual void set_cntxt_item_type(string cntxtItemType)
		{
			_cntxt_item_type = cntxtItemType;
		}

		public virtual IDictionary<string, IList<Document>> get_collections()
		{
			return _collections;
		}

		public virtual void set_collections(IDictionary<string, IList<Document>> collections)
		{
			_collections = collections;
		}

		public virtual string get_default_collection_type()
		{
			return _default_collection_type;
		}

		public virtual void set_default_collection_type(string defaultCollectionType)
		{
			_default_collection_type = defaultCollectionType;
		}

		private string _default_collection_type;

		// Variables are held like this:
		// A stack of maps of variables....
		// or in more human terms:
		// a stack of scopes each containing a symbol table
		// XXX vars contain AnyType... should they be ResultSequence ?
		private Stack _scopes;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="schema">
		///            Schema information from document. May be null. </param>
		public DefaultStaticContext(TypeModel model)
		{
			_xpath1_compatible = false;

			_default_namespace = null;
			_default_function_namespace = FnFunctionLibrary.XPATH_FUNCTIONS_NS;
			_model = model;
			builtinTypes = new XSCtrLibrary();

			_functions = new Dictionary<string, FunctionLibrary>(20); // allow null keys: null namespace
			_namespaces = new Hashtable(20); // ditto

			_cntxt_item_type = null;

			_scopes = new Stack();
			new_scope();

			if (_model != null)
			{
				init_schema(model);
			}

			_base_uri = new XSAnyURI();

			// XXX wildcard prefix
			add_namespace("*", "*");

		}

		/// <summary>
		/// Constructor for schema-less documents.
		/// 
		/// </summary>
		public DefaultStaticContext() : this(null)
		{
		}

		private void init_schema(TypeModel schema)
		{
			_model = schema;
		}

		/// <summary>
		/// return the base URI
		/// </summary>
		/// <returns> XSAnyURI </returns>
		public virtual XSAnyURI base_uri()
		{
			return _base_uri;
		}

		/// <summary>
		/// is it xpath1 compatible?
		/// </summary>
		/// <returns> boolean </returns>
		public virtual bool xpath1_compatible()
		{
			return _xpath1_compatible;
		}

		/// <summary>
		/// adds namespace
		/// </summary>
		/// <param name="prefix">
		///            namespace prefix </param>
		/// <param name="namespace">
		///            namespace URI
		///  </param>
		public virtual void add_namespace(string prefix, string @namespace)
		{
			// XXX are these reserved ?
			// refer to formal semantics section 2.5.1
			if ((!string.ReferenceEquals(prefix, null)) && (prefix.Equals("fs") || prefix.Equals("op") || prefix.Equals("dm")))
			{
				return;
			}
			 if (string.ReferenceEquals(prefix, null))
			 {
			   _default_namespace = @namespace;
			   _namespaces[""] = @namespace;
			 }
			 else
			 {
			   _namespaces[prefix] = @namespace;
			 }
		}

		/// <summary>
		/// Retrieves the default namespace, when one is not allocated
		/// </summary>
		/// <returns> string </returns>
		public virtual string default_namespace()
		{
			return _default_namespace;
		}

		/// <summary>
		/// Retrieves the defaul function namespace
		/// </summary>
		/// <returns> string </returns>
		public virtual string default_function_namespace()
		{
			return _default_function_namespace;
		}

		/// <summary>
		/// Adds a function to the library.
		/// </summary>
		/// <param name="fl">
		///            Function library to add. </param>
		public virtual void add_function_library(FunctionLibrary fl)
		{
			fl.set_static_context(this);
			_functions[fl.@namespace()] = fl;
		}

		/// <summary>
		/// Check for existance of function.
		/// </summary>
		/// <param name="name">
		///            function name. </param>
		/// <param name="arity">
		///            arity of function. </param>
		/// <returns> true if function exists. False otherwise. </returns>
		public virtual bool function_exists(QName name, int arity)
		{
			string ns = name.@namespace();
			if (!_functions.ContainsKey(ns))
			{
				return false;
			}

			FunctionLibrary fl = (FunctionLibrary) _functions[ns];

			return fl.function_exists(name, arity);
		}

		public virtual Function function(QName name, int arity)
		{
			string ns = name.@namespace();
			if (!_functions.ContainsKey(ns))
			{
				return null;
			}

			FunctionLibrary fl = (FunctionLibrary) _functions[ns];

			return fl.function(name, arity);
		}

		/// 
		/// <summary>
		/// Creates an atomic from a specific type name initialized with a default
		/// value.
		/// </summary>
		/// <param name="name">
		///            name of type to create </param>
		/// <returns> Atomic type of desired type. </returns>
		public virtual AnyAtomicType make_atomic(QName name)
		{
			string ns = name.@namespace();

			if (!_functions.ContainsKey(ns))
			{
				return null;
			}

			FunctionLibrary fl = (FunctionLibrary) _functions[ns];

			if (!(fl is ConstructorFL))
			{
				return null;
			}

			ConstructorFL cfl = (ConstructorFL) fl;

			return cfl.atomic_type(name);
		}

		private bool expand_qname(QName name, string def)
		{
			string prefix = name.prefix();

			if (string.ReferenceEquals(prefix, null))
			{
				name.set_namespace(def);
				return true;
			}

			if (!prefix_exists(prefix))
			{
				return false;
			}

			name.set_namespace(resolve_prefix(prefix));
			return true;

		}

		/// <summary>
		/// Expands the qname's prefix into a namespace.
		/// </summary>
		/// <param name="name">
		///            qname to expand. </param>
		/// <returns> true on success. </returns>
		public virtual bool expand_qname(QName name)
		{
			return expand_qname(name, null);
		}

		/// <summary>
		/// Expands a qname and uses the default function namespace if unprefixed.
		/// </summary>
		/// <param name="name">
		///            qname to expand. </param>
		/// <returns> true on success. </returns>
		public virtual bool expand_function_qname(QName name)
		{
			return expand_qname(name, default_function_namespace());
		}

		/// <summary>
		/// Expands a qname and uses the default type/element namespace if
		/// unprefixed.
		/// </summary>
		/// <param name="name">
		///            qname to expand. </param>
		/// <returns> true on success. </returns>
		public virtual bool expand_elem_type_qname(QName name)
		{
			return expand_qname(name, default_namespace());
		}

		/// 
		/// <summary>
		/// Checks whether the type is defined in the in scope schema definitions.
		/// </summary>
		/// <param name="qname">
		///            type name. </param>
		/// <returns> true if type is defined. </returns>
		public virtual bool type_defined(QName qname)
		{

			if (_model == null)
			{
				return builtinTypes.atomic_type(qname) != null;
			}

			TypeDefinition td = _model.lookupType(qname.@namespace(), qname.local());
			if (td == null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks whether the type is defined in the in scope schema definitions.
		/// </summary>
		/// <param name="ns">
		///            namespace of type. </param>
		/// <param name="type">
		///            name of type. </param>
		/// <returns> true if type is defined.
		///  </returns>
		public virtual bool type_defined(string ns, string type)
		{
			return type_defined(new QName(ns, type));
		}

		/// <summary>
		/// is element declared?
		/// </summary>
		/// <param name="elem">
		///            name of element. </param>
		/// <returns> true if element declared. </returns>
		public virtual bool element_declared(QName elem)
		{
			if (_model == null)
			{
				return false;
			}

			TypeDefinition td = _model.lookupElementDeclaration(elem.local(), elem.@namespace());

			if (td == null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Obtains schema definition of the type of an element.
		/// </summary>
		/// <param name="elem">
		///            name of element who's type is desired. </param>
		/// <returns> schema definition of type </returns>
		public virtual TypeDefinition element_type_definition(QName elem)
		{
			return _model.lookupElementDeclaration(elem.local(), elem.@namespace());
		}

		/// <summary>
		/// Checks if an attribute is in the in-scope schema definitions.
		/// </summary>
		/// <param name="attr">
		///            name of attribute. </param>
		/// <returns> true if attribute is declared. </returns>
		public virtual bool attribute_declared(QName attr)
		{
			if (_model == null)
			{
				return false;
			}

			TypeDefinition td = _model.lookupAttributeDeclaration(attr.local(), attr.@namespace());

			if (td == null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Retrieves type definition of the attribute in an element.
		/// </summary>
		/// <param name="elem">
		///            element name </param>
		/// <returns> schema definition of the type of the attribute </returns>
		public virtual TypeDefinition attribute_type_definition(QName elem)
		{
			return _model.lookupAttributeDeclaration(elem.local(), elem.@namespace());
		}

		/// <summary>
		/// does prefix exist?
		/// </summary>
		/// <param name="pref">
		///            prefix name. </param>
		/// <returns> true if it does. </returns>
		public virtual bool prefix_exists(string pref)
		{
			return _namespaces.Contains(pref);
		}

		/// <summary>
		/// Resolves a prefix into a namespace URI.
		/// </summary>
		/// <param name="pref">
		///            prefix name </param>
		/// <returns> uri prefix is resolved to or null. </returns>
		public virtual string resolve_prefix(string pref)
		{
			return (string) _namespaces[pref];
		}

		/// <summary>
		/// Checks if an XML node derives from a specified type.
		/// </summary>
		/// <param name="at">
		///            node actual type </param>
		/// <param name="et">
		///            name of expected type </param>
		/// <returns> true if a derivation exists </returns>
		// XXX fix this
		public virtual bool derives_from(NodeType at, QName et)
		{

			TypeDefinition td = _model.getType(at.node_value());

			short method = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_EXTENSION | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_RESTRICTION;

			// XXX
			if (!et.expanded())
			{
				string pre = et.prefix();

				if (!string.ReferenceEquals(pre, null))
				{
					if (prefix_exists(pre))
					{
						et.set_namespace(resolve_prefix(pre));
					}
					else
					{
						Debug.Assert(false);
					}
				}
				else
				{
					et.set_namespace(default_namespace());
				}
			}

			return td != null && td.derivedFrom(et.@namespace(), et.local(), method);
		}

		/// <summary>
		/// Checks if an XML node derives from a specified type definition.
		/// </summary>
		/// <param name="at">
		///            node actual type. </param>
		/// <param name="et">
		///            type definition of expected type. </param>
		/// <returns> true if a derivation exists. </returns>
		public virtual bool derives_from(NodeType at, TypeDefinition et)
		{
			TypeDefinition td = _model.getType(at.node_value());
			short method = 0;
			return td.derivedFromType(et, method);
		}

		/// <summary>
		/// Creates a new scope level.
		/// </summary>
		// variable stuff
		public virtual void new_scope()
		{
			IDictionary vars = new Hashtable();

			_scopes.Push(vars);
		}

		/// <summary>
		/// Destroys a scope.
		/// </summary>
		public virtual void destroy_scope()
		{
			_scopes.Pop();
		}

		private IDictionary current_scope()
		{
			return (IDictionary) _scopes.Peek();
		}

		/// <summary>
		/// does variable exist in current scope ?
		/// </summary>
		/// <param name="var">
		///            variable name. </param>
		/// <returns> true if it does. </returns>
		public virtual bool variable_exists(QName @var)
		{
			IDictionary scope = current_scope();

			return scope.Contains(@var);
		}

		/// <summary>
		/// checks to see if variable is in scope
		/// </summary>
		/// <param name="var">
		///            variable name. </param>
		/// <returns> true if variable is in current or above scope. </returns>
		public virtual bool variable_in_scope(QName @var)
		{
			// order doesn't matter..
			for (IEnumerator i = _scopes.GetEnumerator(); i.MoveNext();)
			{
				IDictionary scope = (IDictionary) i.Current;

				if (scope.Contains(@var))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adds a variable to current scope.
		/// 
		/// used for static checking.... i.e. presence of variables
		/// </summary>
		/// <param name="var">
		///            variable name to add. </param>
		public virtual void add_variable(QName @var)
		{
			set_variable(@var, (AnyType) null);
		}

		// overwrites, or creates
		public virtual void set_variable(QName @var, AnyType val)
		{
			IDictionary scope = current_scope();

			scope[@var] = val;
		}

		/*
		 * Set a XPath2 sequence into a variable.
		 */
        public virtual void set_variable(QName @var, ResultSequence val)
		{
			IDictionary scope = current_scope();

			scope[@var] = val;
		}


		/// <summary>
		/// Deletes a variable from current scope.
		/// </summary>
		/// <param name="var">
		///            variable name to delete. </param>
		/// <returns> false if variable doesn't exist. </returns>
		public virtual bool del_variable(QName @var)
		{
			if (!variable_exists(@var))
			{
				return false;
			}

			IDictionary scope = current_scope();
			if (!scope.Contains(@var))
			{
				return false;
			}
			scope.Remove(@var);
			return true;

		}

		// return null if "not found"
        public virtual object get_var(QName @var)
		{
			// go through the stack in reverse order... reverse iterators
			// would be nice here...
            throw new Exception();
            //int pos = _scopes.Count;
            //while (--pos >= 0)
            //{
            //	IDictionary scope = (IDictionary) _scopes.Peek(pos);

            //	// gotcha
            //	if (scope.Contains(@var))
            //	{
            //		return scope[@var];
            //	}
            //}

            //return null;
        }

		/// <summary>
		/// Debug function which will print current variable scopes and info.
		/// </summary>
		// debug functions
		public virtual void debug_print_vars()
		{
			int level = 0;

			for (IEnumerator i = _scopes.GetEnumerator(); i.MoveNext();)
			{
				IDictionary scope = (IDictionary) i.Current;

				Console.WriteLine("Scope level " + level);
	//			scope.entrySet().iterator();
				for (IEnumerator j = scope.GetEnumerator(); j.MoveNext();)
				{
					QName varname = (QName) j.Current;

					AnyType val = (AnyType) scope[varname];

					string string_val = "null";

					if (val != null)
					{
						string_val = val.StringValue;
					}

					Console.WriteLine("Varname: " + varname.@string() + " expanded=" + varname.expanded() + " Value: " + string_val);

				}

				level++;
			}
		}

		/// <summary>
		/// Set the Base URI for the static context.
		/// </summary>
		public virtual void set_base_uri(string baseuri)
		{
			_base_uri = new XSAnyURI(baseuri);
		}

		public virtual void set_documents(IDictionary _documents)
		{
			this._documents = _documents;
		}

		public virtual IDictionary get_documents()
		{
			return _documents;
		}

		public virtual TypeModel getTypeModel(Node node)
		{
			return _model;
		}

		public virtual IDictionary<string, FunctionLibrary> get_function_libraries()
		{
			return _functions;
		}
	}

}