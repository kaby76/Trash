///// <summary>
/////*****************************************************************************
///// Copyright (c) 2011, 2018 IBM Corporation and others.
///// This program and the accompanying materials
///// are made available under the terms of the Eclipse Public License 2.0
///// which accompanies this distribution, and is available at
///// https://www.eclipse.org/legal/epl-2.0/
///// 
///// SPDX-License-Identifier: EPL-2.0
///// 
///// Contributors:
/////     IBM Corporation - initial API and implementation
///// ******************************************************************************
///// </summary>
//namespace org.eclipse.wst.xml.xpath2.processor.@internal.types.xerces
//{

//	using ElementPSVI = org.apache.xerces.xs.ElementPSVI;
//	using ItemPSVI = org.apache.xerces.xs.ItemPSVI;
//	using XSAttributeDeclaration = org.apache.xerces.xs.XSAttributeDeclaration;
//	using XSElementDeclaration = org.apache.xerces.xs.XSElementDeclaration;
//	using XSModel = org.apache.xerces.xs.XSModel;
//	using XSTypeDefinition = org.apache.xerces.xs.XSTypeDefinition;
//	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
//	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
//	using Document = org.w3c.dom.Document;
//	using Node = org.w3c.dom.Node;

//	public class XercesTypeModel : TypeModel
//	{
//		/// 
//		private XSModel _schema;

//		public XercesTypeModel(Document doc)
//		{
//			_schema = ((ElementPSVI) doc.DocumentElement).SchemaInformation;
//		}

//		public XercesTypeModel(XSModel model)
//		{
//			_schema = model;
//		}


//		public virtual TypeDefinition lookupType(string @namespace, string typeName)
//		{
//			XSTypeDefinition ad = _schema.getTypeDefinition(typeName, @namespace);

//			return XercesTypeDefinition.createTypeDefinition(ad);
//		}

//		public virtual TypeDefinition lookupElementDeclaration(string @namespace, string elementName)
//		{
//			XSElementDeclaration ad = _schema.getElementDeclaration(elementName, @namespace);

//			return XercesTypeDefinition.createTypeDefinition(ad.TypeDefinition);
//		}

//		public virtual TypeDefinition lookupAttributeDeclaration(string @namespace, string attributeName)
//		{
//			XSAttributeDeclaration ad = _schema.getAttributeDeclaration(attributeName, @namespace);

//			return XercesTypeDefinition.createTypeDefinition(ad.TypeDefinition);
//		}

//		public virtual TypeDefinition getType(Node node)
//		{
//			if (node is ItemPSVI)
//			{
//				XSTypeDefinition typeDefinition = ((ItemPSVI)node).TypeDefinition;
//				if (typeDefinition != null)
//				{
//					return XercesTypeDefinition.createTypeDefinition(typeDefinition);
//				}
//			}
//			return null;
//		}
//	}
//}