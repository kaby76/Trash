using System;
using System.Collections.Generic;
using System.Text;
using org.w3c.dom;

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

	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// Class for Element testing.
	/// </summary>
	public class ElementTest : AttrElemTest
	{
		private bool _qmark = false;

		private AnyType anyType = null;

		/// <summary>
		/// Constructor for ElementTest. This takes in 4 inputs, Name, wildcard
		/// test(true/false), type and question mark test(true/false).
		/// </summary>
		/// <param name="name">
		///            Name of element to test. </param>
		/// <param name="wild">
		///            Wildcard test? (true/false). </param>
		/// <param name="type">
		///            Type of element to test. </param>
		/// <param name="qmark">
		///            Nilled property (true/false). </param>
		public ElementTest(QName name, bool wild, QName type, bool qmark) : base(name, wild, type)
		{
			_qmark = qmark;
		}

		/// <summary>
		/// Constructor for ElementTest. This takes in 3 inputs, Name, wildcard
		/// test(true/false)and type.
		/// </summary>
		/// <param name="name">
		///            Name of element to test. </param>
		/// <param name="wild">
		///            Wildcard test? (true/false). </param>
		/// <param name="type">
		///            Type of element to test. </param>
		public ElementTest(QName name, bool wild, QName type) : base(name, wild, type)
		{
		}

		/// <summary>
		/// Constructor for ElementTest. This takes in 2 inputs, Name, wildcard
		/// test(true/false).
		/// </summary>
		/// <param name="name">
		///            Name of element to test. </param>
		/// <param name="wild">
		///            Wildcard test? (true/false). </param>
		public ElementTest(QName name, bool wild) : base(name, wild)
		{
		}

		/// <summary>
		/// Default Constructor for ElementTest.
		/// </summary>
		public ElementTest() : base()
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

		/// <summary>
		/// Set nilled property.
		/// </summary>
		/// <returns> Result of operation. </returns>
		public virtual bool qmark()
		{
			return _qmark;
		}

		public override AnyType createTestType(ResultSequence rs, StaticContext sc)
		{

			if (name() == null && !wild())
			{
				return new ElementType();
			}

			Item at = rs.first();

			if (!(at is NodeType))
			{
				return new ElementType();
			}

			return createElementType(at, sc);
		}

		private AnyType createElementType(Item at, StaticContext sc)
		{
			anyType = new ElementType();
			NodeType nodeType = (NodeType) at;
			Node node = nodeType.node_value();
			Document doc = null;
			if (node.NodeType == NodeConstants.DOCUMENT_NODE)
			{
				doc = (Document) node;
			}
			else
			{
				doc = nodeType.node_value().OwnerDocument;
			}

			NodeList nodeList = null;
			if (!wild())
			{
				nodeList = doc.getElementsByTagNameNS(name().@namespace(), name().local());
			}
			else
			{
				nodeList = new SingleItemNodeListImpl(node);
			}

			if (nodeList.Length > 0)
			{
				anyType = createElementForXSDType(nodeList, sc);
			}
			return anyType;
		}

		private AnyType createElementForXSDType(NodeList nodeList, StaticContext sc)
		{
			for (int i = 0; i < nodeList.Length; i++)
			{
				Element element = (Element) nodeList.item(i);

				TypeModel typeModel = sc.TypeModel;
				TypeDefinition typedef = typeModel.getType(element);
				if (type() == null || typedef == null)
				{
					anyType = new ElementType(element, typeModel);
					break;
				}
				else
				{
					if (typedef.derivedFrom(type().@namespace(), type().local(), DerviationTypes))
					{
						anyType = new ElementType(element, typeModel);
						break;
					}
				}
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
				return typeof(ElementType);
			}
		}

		private class SingleItemNodeListImpl : NodeList
		{
			internal Node node;
			public SingleItemNodeListImpl(Node node)
			{
				this.node = node;
			}

			public virtual Node item(int index)
			{
				return node;
			}

			public virtual int Length
			{
				get
				{
					if (node != null)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
                set
                {
                    throw new Exception();
                }
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