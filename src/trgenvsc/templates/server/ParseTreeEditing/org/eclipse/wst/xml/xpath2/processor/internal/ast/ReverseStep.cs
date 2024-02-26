using System.Collections;
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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using org.eclipse.wst.xml.xpath2.processor.@internal;

    /// <summary>
    /// Class for Reverse stepping support for Step operations.
    /// </summary>
    public class ReverseStep : Step, IEnumerable
    {
        public enum Type
        {

            /// <summary>
            /// Set internal value for PARENT.
            /// </summary>
            PARENT = 0,

            /// <summary>
            /// Set internal value for ANCESTOR.
            /// </summary>
            ANCESTOR = 1,

            /// <summary>
            /// Set internal value for PRECEDING_SIBLING.
            /// </summary>
            PRECEDING_SIBLING = 2,

            /// <summary>
            /// Set internal value for PRECEDING.
            /// </summary>
            PRECEDING = 3,

            /// <summary>
            /// Set internal value for ANCESTOR_OR_SELF.
            /// </summary>
            ANCESTOR_OR_SELF = 4,

            /// <summary>
            /// Set internal value for DOTDOT.
            /// </summary>
            DOTDOT = 5
        }

        private Type _axis;
		private ReverseAxis _iterator;

		private void update_iterator()
		{
			switch (_axis)
			{
			case Type.PARENT:
				_iterator = new ParentAxis();
				break;
			case Type.ANCESTOR:
				_iterator = new AncestorAxis();
				break;

			case Type.PRECEDING_SIBLING:
				_iterator = new PrecedingSiblingAxis();
				break;

			case Type.PRECEDING:
				_iterator = new PrecedingAxis();
				break;

			case Type.ANCESTOR_OR_SELF:
				_iterator = new AncestorOrSelfAxis();
				break;

			case Type.DOTDOT:
				_iterator = null;
				break;

			default:
				Debug.Assert(false);
			break;
			}
		}

		/// <summary>
		/// Constructor for ReverseStep.
		/// </summary>
		/// <param name="axis">
		///            Axis number. </param>
		/// <param name="node_test">
		///            Node test. </param>
		public ReverseStep(ReverseStep.Type axis, NodeTest node_test) : base(node_test)
		{

			_axis = axis;
			update_iterator();
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
		/// Support for Axis interface.
		/// </summary>
		/// <returns> Result of Axis operation. </returns>
		public virtual ReverseStep.Type axis()
		{
			return _axis;
		}

		/// <summary>
		/// Support for Iterator interface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual ReverseAxis iterator()
		{
			return _iterator;
		}

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public override ICollection<XPathNode> GetAllChildren()
        {
            var list = new List<XPathNode>();
            list.Add(this.node_test());
            return list;
        }

		public override string QuickInfo()
        {
            return "_axis " + _axis;
        }
    }

}