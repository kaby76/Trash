using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using java.xml;
using org.w3c.dom;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Moller - initial API and implementation
///     Lukasz Wycisk - bug 361804 - StaticContextAdapter returns mock function
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{
	using URI = java.net.URI;

	using XMLConstants = java.xml.XMLConstants;
	using NamespaceContext = javax.xml.@namespace.NamespaceContext;

	using CollationProvider = org.eclipse.wst.xml.xpath2.api.CollationProvider;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.api.FunctionLibrary;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using StaticVariableResolver = org.eclipse.wst.xml.xpath2.api.StaticVariableResolver;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using NodeItemTypeImpl = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeItemTypeImpl;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using SimpleAtomicItemTypeImpl = org.eclipse.wst.xml.xpath2.processor.@internal.types.SimpleAtomicItemTypeImpl;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using Node = org.w3c.dom.Node;

	public class StaticContextAdapter : StaticContext
	{
		private readonly org.eclipse.wst.xml.xpath2.processor.StaticContext sc;

		public StaticContextAdapter(org.eclipse.wst.xml.xpath2.processor.StaticContext sc)
		{
			this.sc = sc;
		}

		public virtual bool XPath1Compatible
		{
			get
			{
				return sc.xpath1_compatible();
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
			private readonly StaticContextAdapter outerInstance;

			public StaticVariableResolverAnonymousInnerClass(StaticContextAdapter outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual bool isVariablePresent(javax.xml.@namespace.QName name)
			{
				return outerInstance.sc.variable_exists(outerInstance.qn(name));
			}

			public virtual org.eclipse.wst.xml.xpath2.api.typesystem.ItemType getVariableType(javax.xml.@namespace.QName name)
			{
				return new SimpleAtomicItemTypeImpl(BuiltinTypeLibrary.XS_ANYTYPE);
			}
		}

		private QName qn(javax.xml.@namespace.QName name)
		{
			return new QName(name);
		}

		public virtual TypeDefinition InitialContextType
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPED;
			}
		}

		public virtual IDictionary<string, FunctionLibrary> FunctionLibraries
		{
			get
			{
				if (sc is DefaultStaticContext)
				{
					DefaultStaticContext dsc = (DefaultStaticContext)sc;
                    IDictionary<string, function.FunctionLibrary> x = dsc.get_function_libraries();
                    IDictionary<string, FunctionLibrary> y = new Dictionary<string, FunctionLibrary>();
					foreach (var z in x) y.Add(z.Key, z.Value);
					return y;
				}
				return new Dictionary<string, FunctionLibrary>();
			}
		}

		public virtual CollationProvider CollationProvider
		{
			get
			{
				if (sc is DynamicContext)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.eclipse.wst.xml.xpath2.processor.DynamicContext dc = (org.eclipse.wst.xml.xpath2.processor.DynamicContext)sc;
					DynamicContext dc = (DynamicContext)sc;
					return new CollationProviderAnonymousInnerClass(this, dc);
				}
    
				return new CollationProviderAnonymousInnerClass2(this);
			}
		}

		private class CollationProviderAnonymousInnerClass : CollationProvider
		{
			private readonly StaticContextAdapter outerInstance;

			private DynamicContext dc;

			public CollationProviderAnonymousInnerClass(StaticContextAdapter outerInstance, DynamicContext dc)
			{
				this.outerInstance = outerInstance;
				this.dc = dc;
			}


			public virtual string DefaultCollation
			{
				get
				{
					return dc.default_collation_name();
				}
			}

			public virtual IComparer<string> getCollation(string name)
			{
				return dc.get_collation(name);
			}
		}

		private class CollationProviderAnonymousInnerClass2 : CollationProvider
		{
			private readonly StaticContextAdapter outerInstance;

			public CollationProviderAnonymousInnerClass2(StaticContextAdapter outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual string DefaultCollation
			{
				get
				{
					return null;
				}
			}

			public virtual IComparer<string> getCollation(string name)
			{
				return null;
			}
		}

		public virtual URI BaseUri
		{
			get
			{
				// TODO Auto-generated method stub
				try
				{
					return new URI(sc.base_uri().StringValue);
				}
				catch
				{
					throw;
				}
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
			private readonly StaticContextAdapter outerInstance;

			public NamespaceContextAnonymousInnerClass(StaticContextAdapter outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual IEnumerator<string> getPrefixes(string arg0)
			{
				return new List<string>().GetEnumerator();
			}

			public virtual string getPrefix(string arg0)
			{
				return "x";
			}

			public override string getNamespaceURI(string prefix)
			{
				string ns = outerInstance.sc.resolve_prefix(prefix);
				return !string.ReferenceEquals(ns, null) ? ns : XMLConstants.NULL_NS_URI;
			}
		}

		public virtual string DefaultNamespace
		{
			get
			{
				return sc.default_namespace();
			}
		}

		public virtual string DefaultFunctionNamespace
		{
			get
			{
				return sc.default_function_namespace();
			}
		}

		public virtual TypeModel TypeModel
		{
			get
			{
				return sc.getTypeModel(null);
			}
		}

		public virtual Function resolveFunction(javax.xml.@namespace.QName name, int arity)
		{
			if (sc.function_exists(new QName(name), arity))
			{
				if (sc is DefaultStaticContext)
				{
					DefaultStaticContext dc = (DefaultStaticContext)sc;
					return dc.function(new QName(name), arity);
				}
			}
			throw new System.ArgumentException("Function not found " + name);
		}

		public virtual TypeDefinition getCollectionType(string collectionName)
		{
			return BuiltinTypeLibrary.XS_UNTYPED;
		}

		public virtual TypeDefinition DefaultCollectionType
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPED;
    
			}
		}

		public virtual org.eclipse.wst.xml.xpath2.api.typesystem.ItemType getDocumentType(URI documentUri)
		{
			return new NodeItemTypeImpl(org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_OPTIONAL, NodeConstants.DOCUMENT_NODE);
		}
	}
}