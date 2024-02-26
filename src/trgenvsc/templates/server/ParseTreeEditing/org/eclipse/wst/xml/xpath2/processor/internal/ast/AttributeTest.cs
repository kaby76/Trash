using System;
using System.Collections.Generic;
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
///     David Carver - bug 298535 - Attribute instance of improvements 
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using AttrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AttrType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using Attr = org.w3c.dom.Attr;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Class used to match an attribute node by its name and/or type.
	/// </summary>
	public class AttributeTest : AttrElemTest
	{

		private AnyType anyType = null;

		/// <summary>
		/// Constructor for AttributeTest. This one takes in 3 inputs, Name, wildcard
		/// test(true/false) and type.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="wild">
		///            Wildcard test, True/False. </param>
		/// <param name="type">
		///            QName type. </param>
		public AttributeTest(QName name, bool wild, QName type) : base(name, wild, type)
		{
		}

		/// <summary>
		/// Constructor for AttributeTest. This one takes in 2 inputs, Name and
		/// wildcard test(true/false).
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="wild">
		///            Wildcard test, True/False. </param>
		public AttributeTest(QName name, bool wild) : base(name, wild)
		{
		}

		/// <summary>
		/// Default Constructor for AttributeTest.
		/// </summary>
		public AttributeTest() : base()
		{
		}

		/// <summary>
		/// Support for Visitor interface.
		/// </summary>
		/// <returns> Result of Visitor operation. </returns>
		public override object accept(XPathVisitor v)
		{
			return v.visit(this);
		}

		public override AnyType createTestType(ResultSequence rs, StaticContext sc)
		{
			if (name() == null && !wild())
			{
				return new AttrType();
			}

			Item at = rs.first();

			if (!(at is NodeType))
			{
				return new AttrType();
			}

			return createAttrType(at, sc);
		}

		private AnyType createAttrType(Item at, StaticContext sc)
		{
			anyType = new AttrType();
			NodeType nodeType = (NodeType) at;
			Node node = nodeType.node_value();
			if (node == null)
			{
				return anyType;
			}

			string nodeName = node.LocalName;

			if (wild())
			{
				if (type() != null)
				{
					anyType = createAttrForXSDType(node, sc);
				}
			}
			else if (nodeName.Equals(name().local()))
			{
				if (type() != null)
				{
					anyType = createAttrForXSDType(node, sc);
				}
				else
				{
					anyType = new AttrType((Attr) node, sc.TypeModel);
				}
			}
			return anyType;
		}

		private AnyType createAttrForXSDType(Node node, StaticContext sc)
		{
			Attr attr = (Attr) node;

			TypeModel typeModel = sc.TypeModel;
			TypeDefinition typedef = typeModel.getType(attr);

			if (typedef != null)
			{
				if (typedef.derivedFrom(type().@namespace(), type().local(), DerviationTypes))
				{
					anyType = new AttrType(attr, sc.TypeModel);
				}
			}
			else
			{
				anyType = new AttrType(attr, sc.TypeModel);
			}
			return anyType;
		}

		public override bool Wild
		{
			get
			{
				return wild();
			}
		}

		public override Type XDMClassType
		{
			get
			{
				return typeof(AttrType);
			}
		}

        public override ICollection<XPathNode> GetAllChildren()
        {
            throw new System.NotImplementedException();
        }

        public override string QuickInfo()
        {
            throw new NotImplementedException();
        }
    }

}