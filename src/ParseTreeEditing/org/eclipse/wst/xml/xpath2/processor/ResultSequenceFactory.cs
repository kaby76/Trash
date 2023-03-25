/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2009 Andrea Bittau, University College London, and others
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

namespace org.eclipse.wst.xml.xpath2.processor
{

	using DefaultRSFactory = org.eclipse.wst.xml.xpath2.processor.@internal.DefaultRSFactory;
	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Result sequence factory
	/// </summary>
	public abstract class ResultSequenceFactory
	{
		private static readonly ResultSequenceFactory _factory = new DefaultRSFactory();

		protected internal abstract ResultSequence fact_create_new();

		protected internal abstract void fact_release(ResultSequence rs);

		protected internal virtual ResultSequence fact_create_new(AnyType item)
		{
			ResultSequence rs = fact_create_new();
			rs.add(item);
			return rs;
		}

		protected internal virtual void fact_print_debug()
		{
		}

		/// <returns> the creation of a new result sequence </returns>
		public static ResultSequence create_new()
		{
			return _factory.fact_create_new();
		}

		/// <param name="item">
		///            is an item of any type. </param>
		/// <returns> factory creating new item </returns>
		public static ResultSequence create_new(AnyType item)
		{
			return _factory.fact_create_new(item);
		}

		/// <param name="rs">
		///            is the result sequence factory release rs </param>
		public static void release(ResultSequence rs)
		{
			_factory.fact_release(rs);
		}

		/// <summary>
		/// factory debug
		/// </summary>
		public static void print_debug()
		{
			_factory.fact_print_debug();
		}
	}

}