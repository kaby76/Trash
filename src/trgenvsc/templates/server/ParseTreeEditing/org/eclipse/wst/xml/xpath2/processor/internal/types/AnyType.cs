using System;
using System.Collections;
using System.Collections.Generic;
using javax.xml.datatype;

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
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{



	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using SingleItemSequence = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.SingleItemSequence;

	/// <summary>
	/// Common base for every type
	/// </summary>
	public abstract class AnyType : SingleItemSequence
	{

		protected internal static DatatypeFactory _datatypeFactory;
		static AnyType()
		{
			try
			{
				_datatypeFactory = DatatypeFactory.newInstance();
			}
			catch (DatatypeConfigurationException e)
			{
				throw new Exception("Cannot initialize XML datatypes", e);
			}
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> Datatype's full pathname </returns>
		public abstract string string_type();

		/// <summary>
		/// Retrieves the datatype's text representation
		/// </summary>
		/// <returns> Value as a string </returns>
		public abstract string StringValue {get;}

		public virtual string string_value()
		{
			return StringValue;
		}

		/// <summary>
		/// Returns the "new style" of TypeDefinition for this item.
		/// </summary>
		/// <returns> Type definition (possibly backed by a schema type) </returns>
		public abstract TypeDefinition TypeDefinition {get;}

		public virtual bool empty()
		{
			return false;
		}

		public virtual IEnumerator<Item> iterator()
		{
			List<Item> list = new List<Item>();
			list.Add(this);
            return list.GetEnumerator();
		}

		public virtual ItemType ItemType
		{
			get
			{
				return new SimpleAtomicItemTypeImpl(TypeDefinition);
			}
		}

		public abstract object NativeValue {get;}

		public virtual int size()
		{
			return 1;
		}

		public virtual Item item(int index)
		{
			checkIOOB(index);
			return this;
		}

		private void checkIOOB(int index)
		{
			if (index != 0)
			{
				throw new System.IndexOutOfRangeException("Index out of bounds, index = " + index + ", length = 1");
			}
		}

		public virtual object value(int index)
		{
			checkIOOB(index);
			return NativeValue;
		}
		public virtual ItemType itemType(int index)
		{
			checkIOOB(index);
			return ItemType;
		}

		public virtual Item first()
		{
			return this;
		}

		public virtual object firstValue()
		{
			return this.NativeValue;
		}

		public virtual ItemType sequenceType()
		{
			return new SimpleAtomicItemTypeImpl(TypeDefinition, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_ONE);
		}

        public IEnumerator<Item> GetEnumerator()
        {
            return iterator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}