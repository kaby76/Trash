using System.Collections;

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
///     Mukul Gandhi - bug 274471 - improvements to fn:string function (support for arity 0)
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Class for Function Library support.
	/// </summary>
	public class FunctionLibrary : org.eclipse.wst.xml.xpath2.api.FunctionLibrary
	{
		private string _namespace;
		private IDictionary _functions;
		private StaticContext _sc;
		private DynamicContext _dc;

		/// <summary>
		/// Constructor for FunctionLibrary.
		/// </summary>
		/// <param name="ns">
		///            namespace. </param>
		public FunctionLibrary(string ns)
		{
			_namespace = ns;
			_functions = new Hashtable();
			_sc = null;
			_dc = null;
		}

		/// <summary>
		/// Add a function.
		/// </summary>
		/// <param name="x">
		///            function to add. </param>
		public virtual void add_function(Function x)
		{
			x.name().set_namespace(_namespace);
			_functions[x.signature()] = x;
		}

		/// <summary>
		/// Checks whether the function exists or not.
		/// </summary>
		/// <param name="name">
		///            QName of function. </param>
		/// <param name="arity">
		///            arity of the function. </param>
		/// <returns> Result of the test. </returns>
		public virtual bool function_exists(QName name, int arity)
		{
			return function(name, arity) != null;
		}

		/// <summary>
		/// Function support.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="arity">
		///            arity of the function. </param>
		/// <returns> The new function. </returns>
		public virtual Function function(QName name, int arity)
		{
			Function f = (Function) _functions[Function.signature(name, arity)];

			if (f != null || arity == -1)
			{
				return f;
			}

			// see if we got a varg one
			f = function(name, -1);

			// nope
			if (f == null)
			{
				return null;
			}

			if (f.matches_arity(arity))
			{
				return f;
			}

			return null;
		}

		/// <summary>
		/// Support for namespace.
		/// </summary>
		/// <returns> Namespace. </returns>
		public virtual string @namespace()
		{
			return _namespace;
		}

		/// <summary>
		/// Set static context on function.
		/// </summary>
		public virtual void set_static_context(StaticContext sc)
		{
			_sc = sc;
		}

		/// <summary>
		/// Set dynamic context on function.
		/// </summary>
		public virtual void set_dynamic_context(DynamicContext dc)
		{
			_dc = dc;
		}

		/// <summary>
		/// Support for Static context.
		/// </summary>
		/// <returns> Result of static context. </returns>
		public virtual StaticContext static_context()
		{
			return _sc;
		}

		/// <summary>
		/// Support for Dynamic context.
		/// </summary>
		/// <returns> Result of dynamic context. </returns>
		public virtual DynamicContext dynamic_context()
		{
			return _dc;
		}

		public virtual bool functionExists(string name, int arity)
		{
			return function_exists(new QName(null,name, @namespace()), arity);
		}

		public virtual org.eclipse.wst.xml.xpath2.api.Function resolveFunction(string localName, int arity)
		{

			return function(new QName("f", localName, @namespace()), arity);
		}

		public virtual string Namespace
		{
			get
			{
				return @namespace();
			}
		}
	}

}