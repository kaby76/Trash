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


	/// <summary>
	/// Class for Forward stepping support for Step operations.
	/// </summary>
	public class ForwardStep : Step
	{
        public enum Type
        {

            /// <summary>
            /// Set internal value for NONE.
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Set internal value for CHILD.
            /// </summary>
            CHILD = 1,

            /// <summary>
            /// Set internal value for DESCENDANT.
            /// </summary>
            DESCENDANT = 2,

            /// <summary>
            /// Set internal value for ATTRIBUTE.
            /// </summary>
            ATTRIBUTE = 3,

            /// <summary>
            /// Set internal value for SELF.
            /// </summary>
            SELF = 4,

            /// <summary>
            /// Set internal value for DESCENDANT_OR_SELF.
            /// </summary>
            DESCENDANT_OR_SELF = 5,

            /// <summary>
            /// Set internal value for FOLLOWING_SIBLING.
            /// </summary>
            FOLLOWING_SIBLING = 6,

            /// <summary>
            /// Set internal value for FOLLOWING.
            /// </summary>
            FOLLOWING = 7,

            /// <summary>
            /// Set internal value for NAMESPACE.
            /// </summary>
            NAMESPACE = 8,

            /// <summary>
            /// Set internal value for AT_SYM.
            /// </summary>
            AT_SYM = 9
        }

        private ForwardStep.Type _axis;

		// XXX: we should get rid of the int axis... and make only this the axis
		private ForwardAxis _iterator;

		// XXX: needs to be fixed
		private void update_iterator()
		{
			switch (_axis)
			{
			case Type.NONE:
				if (node_test() is AttributeTest)
				{
					_iterator = new AttributeAxis();
				}
				else
				{
					_iterator = new ChildAxis();
				}
				break;

			case Type.CHILD:
				_iterator = new ChildAxis();
				break;

			case Type.DESCENDANT:
				_iterator = new DescendantAxis();
				break;

			case Type.FOLLOWING_SIBLING:
				_iterator = new FollowingSiblingAxis();
				break;

			case Type.FOLLOWING:
				_iterator = new FollowingAxis();
				break;

			case Type.AT_SYM:
			case Type.ATTRIBUTE:
				_iterator = new AttributeAxis();
				break;

			case Type.SELF:
				_iterator = new SelfAxis();
				break;

			case Type.DESCENDANT_OR_SELF:
				_iterator = new DescendantOrSelfAxis();
				break;

			case Type.NAMESPACE:
				throw new StaticError("XPST0010", "namespace axis not implemented");

			default:
				Debug.Assert(false);
				break;
			}
		}

		/// <summary>
		/// Constructor for ForwardStep.
		/// </summary>
		/// <param name="axis">
		///            Axis number. </param>
		/// <param name="node_test">
		///            Node test. </param>
		public ForwardStep(ForwardStep.Type axis, NodeTest node_test) : base(node_test)
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
		public virtual ForwardStep.Type axis()
		{
			return _axis;
		}

		/// <summary>
		/// Set Axis to current.
		/// </summary>
		/// <param name="axis">
		///            Axis to set. </param>
		public virtual void set_axis(ForwardStep.Type axis)
		{
			_axis = axis;
			update_iterator();
		}

		/// <summary>
		/// Support for Iterator interface.
		/// </summary>
		/// <returns> Result of Iterator operation. </returns>
		public virtual ForwardAxis iterator()
		{
			return _iterator;
		}


        public override ICollection<XPathNode> GetAllChildren()
        {
            var list = new List<XPathNode>();
			list.Add(this.node_test());
            return list;
        }

        public override string QuickInfo()
        {
            return "_axis " + _axis.ToString();
        }
    }

}