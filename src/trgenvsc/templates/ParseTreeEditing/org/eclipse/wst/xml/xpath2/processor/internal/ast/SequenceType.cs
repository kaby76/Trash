using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
///     Jesper Steen Moller - bug 312191 - instance of test fails with partial matches
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	/// <summary>
	/// Support for Sequence type.
	/// </summary>
	public class SequenceType : XPathNode
	{
		/// <summary>
		/// Set internal value for EMPTY.
		/// </summary>
		public const int EMPTY = 0;
		/// <summary>
		/// Set internal value for NONE.
		/// </summary>
		public const int NONE = 1;
		/// <summary>
		/// Set internal value for QUESTION.
		/// </summary>
		public const int QUESTION = 2;
		/// <summary>
		/// Set internal value for STAR.
		/// </summary>
		public const int STAR = 3;
		/// <summary>
		/// Set internal value for PLUS.
		/// </summary>
		public const int PLUS = 4;

		private int _occ;
		private ItemType _it;

		/// <summary>
		/// Constructor for SequenceType.
		/// </summary>
		/// <param name="occ">
		///            Occurence. </param>
		/// <param name="it">
		///            Item type. </param>
		public SequenceType(int occ, ItemType it)
		{
			_occ = occ;
			_it = it;
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		/// <summary>
		/// Get occurence of item.
		/// </summary>
		/// <returns> Result from Int operation. </returns>
		public virtual int occurrence()
		{
			return _occ;
		}

		/// <summary>
		/// Support for ItemType interface.
		/// </summary>
		/// <returns> Result of ItemType operation. </returns>
		public virtual ItemType item_type()
		{
			return _it;
		}

		public virtual bool isLengthValid(int length)
		{
			switch (occurrence())
			{
			case EMPTY:
				return length == 0;
			case NONE:
				return length == 1;
			case QUESTION:
				return length <= 1;
			case STAR:
				return true;
			case PLUS:
				return length >= 1;
			default:
				Debug.Assert(false);
				return false;
			}
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            throw new System.NotImplementedException();
        }

        public override string QuickInfo()
        {
            throw new System.NotImplementedException();
        }
    }

}