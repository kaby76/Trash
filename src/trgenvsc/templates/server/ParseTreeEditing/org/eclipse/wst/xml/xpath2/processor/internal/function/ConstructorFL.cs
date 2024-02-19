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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using CtrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.CtrType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Constructor class for the functions library.
	/// </summary>
	public class ConstructorFL : FunctionLibrary
	{

		private Hashtable _types;

		/// <summary>
		/// Constructor for ConstructorFL.
		/// </summary>
		/// <param name="ns">
		///            input string. </param>
		public ConstructorFL(string ns) : base(ns)
		{

			_types = new Hashtable();
		}

		/// <summary>
		/// Adds a type into the functions library.
		/// </summary>
		/// <param name="at">
		///            input of any atomic type. </param>
		public virtual void add_type(CtrType at)
		{
			QName name = new QName(at.type_name());
			name.set_namespace(@namespace());

			_types[name] = at;

			add_function(new Constructor(at));
		}

		/// <summary>
		/// Adds a type into the functions library as an abstract type.
		/// </summary>
		/// <param name="at">
		///            input of any atomic type. </param>
		public virtual void add_abstract_type(string localName, AnyAtomicType at)
		{
			QName name = new QName(localName);
			name.set_namespace(@namespace());

			_types[name] = at;
		}

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <param name="name">
		///            variable name. </param>
		/// <returns> type of input variable. </returns>
		public virtual AnyAtomicType atomic_type(QName name)
		{
			return (AnyAtomicType) _types[name];
		}
	}

}