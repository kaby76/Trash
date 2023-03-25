
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
///     Mukul Gandhi - bug 273760 - wrong namespace for functions and data types
///     David Carver - bug 282223 - implementation of xs:duration data type.
///                  - bug 262765 - fix handling of range expression op:to and empty sequence 
///     Jesper Moller- bug 281159 - fix document loading and resolving URIs 
///     Jesper Moller- bug 286452 - always return the stable date/time from dynamic context
///     Jesper Moller- bug 275610 - Avoid big time and memory overhead for externals
///     Jesper Moller- bug 280555 - Add pluggable collation support
///    Mukul Gandhi - bug 325262 - providing ability to store an XPath2 sequence into
///                                 an user-defined variable.
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using java.net;

namespace org.eclipse.wst.xml.xpath2.processor
{
	using MalformedURLException = java.net.MalformedURLException;
	using GregorianCalendar = java.util.GregorianCalendar;
	using URI = java.net.URI;
	using XSModel = org.apache.xerces.xs.XSModel;
	using TimeZone = java.util.TimeZone;
	using DefaultStaticContext = org.eclipse.wst.xml.xpath2.processor.@internal.DefaultStaticContext;
	using Focus = org.eclipse.wst.xml.xpath2.processor.@internal.Focus;
	using Function = org.eclipse.wst.xml.xpath2.processor.@internal.function.Function;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.FunctionLibrary;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDayTimeDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDayTimeDuration;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using ResultSequenceUtil = org.eclipse.wst.xml.xpath2.processor.util.ResultSequenceUtil;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// The default implementation of a Dynamic Context.
	/// 
	/// Initializes and provides functionality of a dynamic context according to the
	/// XPath 2.0 specification.
	/// </summary>
	public class DefaultDynamicContext : DefaultStaticContext, DynamicContext
	{

		private Focus _focus;
		private XSDuration _tz;
		private IDictionary _loaded_documents;
		private GregorianCalendar _current_date_time;
		private string _default_collation_name = DynamicContext_Fields.CODEPOINT_COLLATION;
		private CollationProvider _collation_provider;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="schema">
		///            Schema information of document. May be null </param>
		/// <param name="doc">
		///            Document [root] node of XML source. </param>
		public DefaultDynamicContext(XSModel schema, Document doc)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="schema">
		///            Schema information of document. May be null </param>
		/// <param name="doc">
		///            Document [root] node of XML source.
		/// @since 2.0 </param>
		public DefaultDynamicContext(TypeModel schema) : base(schema)
		{

			_focus = null;
			_tz = new XSDayTimeDuration(0, 5, 0, 0, true);
			_loaded_documents = new Hashtable();
		}
		/// <summary>
		/// Reads the day from a TimeDuration type
		/// </summary>
		/// <returns> an xs:integer _tz
		/// @since 1.1 </returns>
		public virtual XSDuration tz()
		{
			return _tz;
		}

		/// <summary>
		/// Gets the Current stable date time from the dynamic context.
		/// @since 1.1 </summary>
		/// <seealso cref= org.eclipse.wst.xml.xpath2.processor.DynamicContext#get_current_time() </seealso>
		public virtual GregorianCalendar current_date_time()
		{
			if (_current_date_time == null)
			{
				_current_date_time = new GregorianCalendar(TimeZone.getTimeZone("GMT"));
			}
			return _current_date_time;
		}

		/// <summary>
		/// Changes the current focus.
		/// </summary>
		/// <param name="f">
		///            focus to set </param>
		public virtual void set_focus(Focus f)
		{
			_focus = f;
		}

		/// <summary>
		/// Return the focus
		/// </summary>
		/// <returns> _focus </returns>
		public virtual Focus focus()
		{
			return _focus;
		}

		/// <summary>
		/// Retrieve context item that is in focus
		/// </summary>
		/// <returns> an AnyType result from _focus.context_item() </returns>
		public virtual AnyType context_item()
		{
			return _focus.context_item();
		}

		/// <summary>
		/// Retrieve the position of the focus
		/// </summary>
		/// <returns> an integer result from _focus.position() </returns>
		public virtual int context_position()
		{
			return _focus.position();
		}

		/// <summary>
		/// Retrieve the position of the last focus
		/// </summary>
		/// <returns> an integer result from _focus.last() </returns>
		public virtual int last()
		{
			return _focus.last();
		}

		/// <summary>
		/// Retrieve the variable name
		/// </summary>
		/// <returns> an AnyType result from get_var(name) or return NULL
		/// @since 2.0 </returns>
		public virtual object get_variable(QName name)
		{
			// XXX: built-in variables
			if ("fs".Equals(name.prefix()))
			{
				if (name.local().Equals("dot"))
				{
					return context_item();
				}

				return null;
			}
			return get_var(name);
		}

		/// 
		/// <returns> a ResultSequence from funct.evaluate(args) </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ResultSequence evaluate_function(org.eclipse.wst.xml.xpath2.processor.internal.types.QName name, java.util.Collection args) throws DynamicError
		public virtual ResultSequence evaluate_function(QName name, ICollection args)
		{
			Function funct = function(name, args.Count);

			Debug.Assert(funct != null);

			return ResultSequenceUtil.newToOld(funct.evaluate(args));
		}

		/// <summary>
		/// Adds function definitions.
		/// </summary>
		/// <param name="fl">
		///            Function library to add.
		///  </param>
		public override void add_function_library(FunctionLibrary fl)
		{
			base.add_function_library(fl);
			fl.set_dynamic_context(this);
		}

		/// <summary>
		/// get document
		/// </summary>
		/// <returns> a ResultSequence from ResultSequenceFactory.create_new()
		/// @since 1.1 </returns>
		public virtual ResultSequence get_doc(URI resolved)
		{
			Document doc = null;
			if (_loaded_documents.Contains(resolved))
			{
				 //tried before
				doc = (Document)_loaded_documents[resolved];
			}
			else
			{
				doc = retrieve_doc(resolved);
				_loaded_documents[resolved] = doc;
			}

			if (doc == null)
			{
				return null;
			}

			return ResultSequenceFactory.create_new(new DocType(doc, getTypeModel(doc)));
		}
		/// <summary>
		/// @since 1.1
		/// </summary>
		public virtual URI resolve_uri(string uri)
		{
			try
			{
                URI realURI = URI.create(uri);
				if (realURI.Absolute)
				{
					return realURI;
				}
				else
				{
                    URI baseURI = URI.create(base_uri().StringValue);
					return baseURI.resolve(uri);
				}
			}
			catch (System.ArgumentException)
			{
				return null;
			}
		}

		// XXX make it nice, and move it out as a utility function
		private Document retrieve_doc(URI uri)
		{
			//try
			//{
			//	DOMLoader loader = new XercesLoader();
			//	loader.set_validating(false);

			//	Document doc = loader.load((new URL(uri.ToString())).openStream());
			//	doc.DocumentURI = uri.ToString();
			//	return doc;
			//}
			//catch (DOMLoaderException)
			//{
			//	return null;
			//}
			//catch (FileNotFoundException)
			//{
			//	return null;
			//}
			//catch (MalformedURLException)
			//{
			//	return null;
			//}
			//catch (IOException)
			//{
			//	return null;
			//}
            return null;
        }

		/// <summary>
		/// Sets the value of a variable.
		/// </summary>
		/// <param name="var">
		///            Variable name. </param>
		/// <param name="val">
		///            Variable value. </param>
		public override void set_variable(QName @var, AnyType val)
		{
			base.set_variable(@var, val);
		}


		/*
		 * Set a XPath2 sequence into a variable.
		 */
		/// <summary>
		/// @since 2.0
		/// </summary>
		public override void set_variable(QName @var, ResultSequence val)
		{
			base.set_variable(@var, val);
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public virtual void set_default_collation(string _default_collation)
		{
			this._default_collation_name = _default_collation;
		}

		/// <summary>
		/// @since 1.1
		/// </summary>
		public virtual string default_collation_name()
		{
			return _default_collation_name;
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

            public int Compare(object x, object y)
            {
                throw new NotImplementedException();
            }
        }

		/// <summary>
		/// @since 1.1
		/// 
		/// </summary>
		public virtual IComparer<string> get_collation(string uri)
		{
			//if (DynamicContext_Fields.CODEPOINT_COLLATION.Equals(uri))
			//{
			//	return CODEPOINT_COMPARATOR;
			//}

			return _collation_provider != null ? _collation_provider.get_collation(uri) : null;
		}

		/// 
		/// 
		/// <param name="provider">
		/// @since 1.1 </param>
		public virtual void set_collation_provider(CollationProvider provider)
		{
			this._collation_provider = provider;
		}

		/// <summary>
		/// Use focus().position() to retrieve the value. </summary>
		/// @deprecated  This will be removed in a future version use focus().position(). 
		public virtual int node_position(Node node)
		{
		  // unused parameter!
		  return _focus.position();
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public override TypeModel getTypeModel(Node node)
		{
			return base.getTypeModel(node);
		}
	}

}