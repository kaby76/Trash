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
///     Jesper Steen Moller - bug 281938 - handle missing focus
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;

	/// <summary>
	/// set the focus from a result sequence
	/// </summary>
	public class Focus
	{
		private int _cp; // context position
		private ResultSequence _rs; // all items in context

		/// <summary>
		/// Sets the _rs to rs and context position to 1.
		/// </summary>
		/// <param name="rs">
		///            is a ResultSequence and is set to _rs. </param>
		public Focus(ResultSequence rs)
		{
			_rs = rs;
			_cp = 1;
		}

		/// <summary>
		/// Retrieves previous item from current context position.
		/// </summary>
		/// <returns> the item from _rs, or null if there is no context item. </returns>
		public virtual AnyType context_item()
		{
			// idexes start at 0
			if (_cp > _rs.size())
			{
				return null;
			}
			return (AnyType)_rs.item(_cp - 1);
		}

		/// <summary>
		/// Checks to see if possible to advance rs.
		/// </summary>
		/// <returns> the boolean. </returns>
		public virtual bool advance_cp()
		{
			int size;

			// check if we can advance
			size = _rs.size();
			if (_cp >= size)
			{
				return false;
			}

			_cp++;
			return true;
		}

		/// <summary>
		/// returns an integer of the current position.
		/// </summary>
		/// <returns> the current position of rs. </returns>
		public virtual int position()
		{
			return _cp;
		}

		/// <summary>
		/// returns the position of the last item in rs.
		/// </summary>
		/// <returns> the size of rs. </returns>
		public virtual int last()
		{
			return _rs.size();
		}

		/// <summary>
		/// sets the position.
		/// </summary>
		/// <param name="p">
		///            is the position that is set. </param>
		public virtual void set_position(int p)
		{
			_cp = p; // XXX no checks
		}
	}

}