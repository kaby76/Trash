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


	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.api.FunctionLibrary;

	/// <summary>
	/// Class for Function Library support.
	/// </summary>
	public class FunctionLibraryImpl : api.FunctionLibrary
	{
		private string _namespace;
		private IDictionary _functions;

		/// <summary>
		/// Constructor for FunctionLibrary.
		/// </summary>
		/// <param name="ns">
		///            namespace. </param>
		public FunctionLibraryImpl(string ns)
		{
			_namespace = ns;
			_functions = new Hashtable();
		}

		/// <summary>
		/// Add a function.
		/// </summary>
		/// <param name="x">
		///            function to add. </param>
		public virtual void addFunction(api.Function x)
		{
			_functions[signature(x)] = x;
		}

		/// <summary>
		/// Obtain the function name and arity from signature.
		/// </summary>
		/// <param name="f">
		///            current function. </param>
		/// <returns> Signature. </returns>
		private static string signature(api.Function f)
		{
			return signature(f.Name, f.VariableArgument ? -1 : f.MinArity);
		}

		/// <summary>
		/// Apply the name and arity to signature.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="arity">
		///            arity of the function. </param>
		/// <returns> Signature. </returns>
		private static string signature(string name, int arity)
		{
			return name + "_" + ((arity < 0) ? "x" : arity.ToString());
		}

		public virtual bool functionExists(string name, int arity)
		{
			return resolveFunction(name, arity) != null;
		}

		public virtual api.Function resolveFunction(string localName, int arity)
		{

            api.Function f = (api.Function) _functions[signature(localName, arity)];

			if (f != null || arity == -1)
			{
				return f;
			}

			// see if we got a varg one
			f = (api.Function) _functions[signature(localName, -1)];

			// nope
			if (f == null)
			{
				return null;
			}

			if (f.canMatchArity(arity))
			{
				return f;
			}

			return null;
		}

		public virtual string Namespace
		{
			get
			{
				return _namespace;
			}
		}
	}

}