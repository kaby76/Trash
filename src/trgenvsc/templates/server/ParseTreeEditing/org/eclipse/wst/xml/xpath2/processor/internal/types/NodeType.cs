using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using org.eclipse.wst.xml.xpath2.api;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2012 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     Mukul Gandhi - bug 276134 - improvements to schema aware primitive type support
///                                 for attribute/element nodes
///     David Carver (STAR)- bug 277774 - XSDecimal returning wrong values.
///     Jesper Moller - bug 275610 - Avoid big time and memory overhead for externals
///     David Carver (STAR) - bug 281186 - implementation of fn:id and fn:idref
///     David Carver (STAR) - bug 289304 - fixed schema awareness on elements
///     Mukul Gandhi - bug 318313 - improvements to computation of typed values of nodes,
///                                 when validated by XML Schema primitive types
///     Mukul Gandhi - bug 323900 - improving computing the typed value of element &
///                                 attribute nodes, where the schema type of nodes
///                                 are simple, with varieties 'list' and 'union'.                                 
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
///     Lukasz Wycisk - bug 361803 - NodeType:dom_to_xpath and null value
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{


	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using ComplexTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.ComplexTypeDefinition;
	using PrimitiveType = org.eclipse.wst.xml.xpath2.api.typesystem.PrimitiveType;
	using SimpleTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using Attr = org.w3c.dom.Attr;
	using Comment = org.w3c.dom.Comment;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeConstants = org.w3c.dom.NodeConstants;
	using ProcessingInstruction = org.w3c.dom.ProcessingInstruction;
	using Text = org.w3c.dom.Text;
	using TypeInfo = org.w3c.dom.TypeInfo;

	/// <summary>
	/// A representation of a Node datatype
	/// </summary>
	public abstract class NodeType : AnyType
	{

		protected internal const string SCHEMA_TYPE_IDREF = "IDREF";
		protected internal const string SCHEMA_TYPE_ID = "ID";
		private Node _node;
		protected internal TypeModel _typeModel;

		public static readonly IComparer NODE_COMPARATOR = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer
		{
			public ComparatorAnonymousInnerClass()
			{
			}

			public virtual int compare(object o1, object o2)
			{
				return compare_node((NodeType)o1, (NodeType)o2);
			}

            public int Compare(object x, object y)
            {
                return compare_node((NodeType)x, (NodeType)y);
            }
		}

		/// <summary>
		/// Initialises according to the supplied parameters
		/// </summary>
		/// <param name="node">
		///            The Node being represented </param>
		/// <param name="document_order">
		///            The document order </param>
		public NodeType(Node node, TypeModel tm)
		{
			_node = node;
			_typeModel = tm;
		}

		/// <summary>
		/// Retrieves the actual node being represented
		/// </summary>
		/// <returns> Actual node being represented </returns>
		public virtual Node node_value()
		{
			return _node;
		}

		// Accessors defined in XPath Data model
		// http://www.w3.org/TR/xpath-datamodel/
		/// <summary>
		/// Retrieves the actual node being represented
		/// </summary>
		/// <returns> Actual node being represented </returns>
		public abstract ResultSequence typed_value();

		/// <summary>
		/// Retrieves the name of the node
		/// </summary>
		/// <returns> QName representation of the name of the node </returns>
		public abstract QName node_name(); // may return null ["empty sequence"]

		// XXX element should override
		public virtual ResultSequence nilled()
		{
			return ResultSequenceFactory.create_new();
		}

		// a little factory for converting from DOM to our representation
		public static NodeType dom_to_xpath(Node node, TypeModel tm)
		{
			Debug.Assert(node != null);

			switch (node.NodeType)
			{
			case NodeConstants.ELEMENT_NODE:
				return new ElementType((Element) node, tm);

			case NodeConstants.COMMENT_NODE:
				return new CommentType((Comment) node, tm);

			case NodeConstants.ATTRIBUTE_NODE:
				return new AttrType((Attr) node, tm);

			case NodeConstants.TEXT_NODE:
			case NodeConstants.CDATA_SECTION_NODE:
				return new TextType((Text) node, tm);

			case NodeConstants.DOCUMENT_NODE:
				return new DocType((Document) node, tm);

			case NodeConstants.PROCESSING_INSTRUCTION_NODE:
				return new PIType((ProcessingInstruction) node, tm);

			}

			// for unknown unsupported implementations
            throw new Exception();  // KED you should not have this.
            //return null;
		}

//		public static org.eclipse.wst.xml.xpath2.processor.ResultSequence eliminate_dups(org.eclipse.wst.xml.xpath2.processor.ResultSequence rs)
//		{
//			Hashtable added = new Hashtable(rs.size());


//			for (IEnumerator i = rs.GetEnumerator(); i.MoveNext();)
//			{
//				NodeType node = (NodeType) i.Current;
//				Node n = node.node_value();

//				if (added.ContainsKey(n))
//				{
////JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
//					i.remove();
//				}
//				else
//				{
//					added[n] = true;
//				}
//			}
//			return rs;
//		}

		public static org.eclipse.wst.xml.xpath2.processor.ResultSequence sort_document_order(org.eclipse.wst.xml.xpath2.processor.ResultSequence rs)
		{
			ArrayList res = new ArrayList(rs.size());

			for (IEnumerator i = rs.GetEnumerator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;
				bool added = false;

				for (int j = 0; j < res.Count; j++)
				{
					NodeType x = (NodeType) res[j];

					if (before(node, x))
					{
						res.Insert(j, node);
						added = true;
						break;
					}
				}
				if (!added)
				{
					res.Add(node);
				}
			}

			rs = ResultSequenceFactory.create_new();
			for (IEnumerator i = res.GetEnumerator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;

				rs.add(node);
			}

			return rs;
		}

		public static bool same(NodeType a, NodeType b)
		{
			return (a.node_value().isSameNode(b.node_value()));
			// While compare_node(a, b) == 0 is tempting, it is also expensive
		}

		public virtual bool before(NodeType two)
		{
			return before(this, two);
		}

		public static bool before(NodeType a, NodeType b)
		{
			return compare_node(a, b) < 0;
		}

		public virtual bool after(NodeType two)
		{
			return after(this, two);
		}

		public static bool after(NodeType a, NodeType b)
		{
			return compare_node(a, b) > 0;
		}

		private static int compare_node(NodeType a, NodeType b)
		{
			Node nodeA = a.node_value();
			Node nodeB = b.node_value();

			if (nodeA == nodeB || nodeA.isSameNode(nodeB))
			{
				return 0;
			}

			Document docA = getDocument(nodeA);
			Document docB = getDocument(nodeB);

			if (docA != docB && !docA.isSameNode(docB))
			{
				return compareDocuments(docA, docB);
			}
			short relation = nodeA.compareDocumentPosition(nodeB);
			if ((relation & NodeConstants.DOCUMENT_POSITION_PRECEDING) != 0)
			{
				  return 1;
			}
			if ((relation & NodeConstants.DOCUMENT_POSITION_FOLLOWING) != 0)
			{
				  return -1;
			}
			throw new Exception("Unexpected result from node comparison: " + relation);
		}

		private static int compareDocuments(Document docA, Document docB)
		{
			// Arbitrary but fulfills the spec (provided documenURI is always set)
			//if (docB.DocumentURI == null && docA.DocumentURI == null)
			//{
			//	return System.identityHashCode(docA) - System.identityHashCode(docB);
			//}
			return docB.DocumentURI.CompareTo(docA.DocumentURI);
		}

		private static Document getDocument(Node nodeA)
		{
			return nodeA is Document ? (Document)nodeA : nodeA.OwnerDocument;
		}

		protected internal virtual object getTypedValueForPrimitiveType(TypeDefinition typeDef)
		{
			string strValue = StringValue;

			if (typeDef == null)
			{
			   return new XSUntypedAtomic(strValue);
			}

			return SchemaTypeValueFactory.newSchemaTypeValue(PsychoPathTypeHelper.getXSDTypeShortCode(typeDef), strValue);
		} // getTypedValueForPrimitiveType


		/*
		 * Construct the "typed value" from a "string value", given the simpleType of the node.
		 */
		protected internal virtual ResultSequence getXDMTypedValue(TypeDefinition typeDef, IList itemValTypes)
		{

			if ("anySimpleType".Equals(typeDef.Name) || "anyAtomicType".Equals(typeDef.Name))
			{
				return new XSUntypedAtomic(StringValue);
			}
			else
			{
				SimpleTypeDefinition simpType = null;

				if (typeDef is ComplexTypeDefinition)
				{
					ComplexTypeDefinition complexTypeDefinition = (ComplexTypeDefinition) typeDef;
					simpType = complexTypeDefinition.SimpleType;
					if (simpType != null)
					{
						// element has a complexType with a simple content
						return getTypedValueForSimpleContent(simpType, itemValTypes);
					}
					else
					{
						// element has a complexType with complex content
						return new XSUntypedAtomic(StringValue);
					}
				}
				else
				{
					// element has a simpleType
					simpType = (SimpleTypeDefinition) typeDef;
					return getTypedValueForSimpleContent(simpType, itemValTypes);
				}
			}

		} // getXDMTypedValue

		/*
		 * Get the XDM typed value for schema "simple content model". 
		 */
		private ResultSequence getTypedValueForSimpleContent(SimpleTypeDefinition simpType, IList itemValueTypes)
		{

			ResultBuffer rs = new ResultBuffer();

			if (simpType.Variety == org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition_Fields.VARIETY_ATOMIC)
			{
			   AnyType schemaTypeValue = SchemaTypeValueFactory.newSchemaTypeValue(PsychoPathTypeHelper.getXSDTypeShortCode(simpType), StringValue);
			   if (schemaTypeValue != null)
			   {
					return schemaTypeValue;
			   }
			   else
			   {
				   return new XSUntypedAtomic(StringValue);
			   }
			}
			else if (simpType.Variety == org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition_Fields.VARIETY_LIST)
			{
				addAtomicListItemsToResultSet(simpType, itemValueTypes, rs);
			}
			else if (simpType.Variety == org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition_Fields.VARIETY_UNION)
			{
				getTypedValueForVarietyUnion(simpType, rs);
			}

			return rs.Sequence;

		} // getTypedValueForSimpleContent


		/*
		 * If the variety of simpleType was 'list', add the typed "list item" values to the parent result set. 
		 */
		private void addAtomicListItemsToResultSet(SimpleTypeDefinition simpType, IList itemValueTypes, ResultBuffer rs)
		{

			// tokenize the string value by a 'longest sequence' of white-spaces. this gives us the list items as string values.
			string[] listItemsStrValues = StringValue.Split("\\s+", true);

			SimpleTypeDefinition itemType = (SimpleTypeDefinition) simpType.ItemType;
			if (itemType.Variety == org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition_Fields.VARIETY_ATOMIC)
			{
				for (int listItemIdx = 0; listItemIdx < listItemsStrValues.Length; listItemIdx++)
				{
				   // add an atomic typed value (whose type is the "item  type" of the list, and "string value" is the "string 
				   // value of the list item") to the "result sequence".
				   rs.add(SchemaTypeValueFactory.newSchemaTypeValue(PsychoPathTypeHelper.getXSDTypeShortCode(itemType), listItemsStrValues[listItemIdx]));
				}
			}
			else if (itemType.Variety == org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition_Fields.VARIETY_UNION)
			{
				// here the list items may have different atomic types
				for (int listItemIdx = 0; listItemIdx < listItemsStrValues.Length; listItemIdx++)
				{
					string listItem = listItemsStrValues[listItemIdx];
					rs.add(SchemaTypeValueFactory.newSchemaTypeValue(((short?)itemValueTypes[listItemIdx]).Value, listItem));
				}
			}

		} // addAtomicListItemsToResultSet


		/*
		 * If the variety of simpleType was 'union', find the typed value (and added to the parent 'result set') 
		 * to be returned as the typed value of the parent node, by considering the member types of the union (i.e
		 * whichever member type first in order, can successfully validate the string value of the parent node).
		 */
		private void getTypedValueForVarietyUnion(SimpleTypeDefinition simpType, ResultBuffer rs)
		{

			IList memberTypes = simpType.MemberTypes;
			// check member types in order, to find that which one can successfully validate the string value.
			for (int memTypeIdx = 0; memTypeIdx < memberTypes.Count; memTypeIdx++)
			{
				PrimitiveType memSimpleType = (PrimitiveType) memberTypes[memTypeIdx];
			   if (isValueValidForSimpleType(StringValue, memSimpleType))
			   {

				   rs.add(SchemaTypeValueFactory.newSchemaTypeValue(PsychoPathTypeHelper.getXSDTypeShortCode(memSimpleType), StringValue));
				   // no more memberTypes need to be checked
				   break;
			   }
			}

		} // getTypedValueForVarietyUnion


		/*
		 * Determine if a "string value" is valid for a given simpleType definition. This is a helped method for other methods.
		 */
		private bool isValueValidForSimpleType(string value, PrimitiveType simplType)
		{

			// attempt to validate the value with the simpleType
			return simplType.validate(value);

		} // isValueValidForASimpleType


		public abstract bool ID {get;}

		public abstract bool IDREF {get;}

		/// <summary>
		/// Utility method to check to see if a particular TypeInfo matches. </summary>
		/// <param name="typeInfo"> </param>
		/// <param name="typeName">
		/// @return </param>
		protected internal virtual bool isType(TypeInfo typeInfo, string typeName)
		{
			if (typeInfo != null)
			{
				string typeInfoName = typeInfo.TypeName;
				if (!string.ReferenceEquals(typeInfoName, null))
				{
					if (typeInfo.TypeName.ToLower() == typeName.ToLower())
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Looks up the available type for the node, if available
		/// </summary>
		/// <returns> TypeDefinition, or null </returns>
		protected internal virtual TypeDefinition Type
		{
			get
			{
				if (_typeModel != null)
				{
					return _typeModel.getType(node_value());
				}
				else
				{
					 return null;
				}
			}
		}

		public override object NativeValue
		{
			get
			{
				return node_value();
			}
		}

		public virtual TypeModel TypeModel
		{
			get
			{
				return _typeModel;
			}
		}

		public static ResultBuffer linarize(ResultBuffer rs)
		{
			HashSet<Item> all = new HashSet<Item>();
			foreach (var x in rs.Collection) all.Add(x);
			return (new ResultBuffer()).concat(all);
		}
	}

}