//using System;
//using System.Collections;

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


//	using PSVIAttrNSImpl = org.apache.xerces.dom.PSVIAttrNSImpl;
//	using PSVIElementNSImpl = org.apache.xerces.dom.PSVIElementNSImpl;
//	using XSSimpleType = org.apache.xerces.impl.dv.XSSimpleType;
//	using ShortList = org.apache.xerces.xs.ShortList;
//	using XSComplexTypeDefinition = org.apache.xerces.xs.XSComplexTypeDefinition;
//	using XSConstants = org.apache.xerces.xs.XSConstants;
//	using XSSimpleTypeDefinition = org.apache.xerces.xs.XSSimpleTypeDefinition;
//	using XSTypeDefinition = org.apache.xerces.xs.XSTypeDefinition;
//	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
//	using Attr = org.w3c.dom.Attr;
//	using Element = org.w3c.dom.Element;

//	public abstract class XercesTypeDefinition : TypeDefinition
//	{
//		public abstract Type NativeType {get;}

//		private readonly XSTypeDefinition xsTypeDefinition;
//		private XercesTypeDefinition baseType = null;

//		public XercesTypeDefinition(XSTypeDefinition typeDef)
//		{
//			if (typeDef == null)
//			{
//				throw new System.ArgumentException("typeDef");
//			}
//			xsTypeDefinition = typeDef;
//		}

//		public virtual string Namespace
//		{
//			get
//			{
//				return xsTypeDefinition.Namespace;
//			}
//		}

//		public virtual string Name
//		{
//			get
//			{
//				return xsTypeDefinition.Name;
//			}
//		}

//		public virtual bool ComplexType
//		{
//			get
//			{
//				return (xsTypeDefinition.TypeCategory & XSConstants.PARTICLE) != 0;
//			}
//		}

//		public virtual TypeDefinition BaseType
//		{
//			get
//			{
//				// TODO: Cache per-model??
//				if (baseType == null && xsTypeDefinition.BaseType != null)
//				{
//					baseType = createTypeDefinition(xsTypeDefinition.BaseType);
//				}
//				return baseType;
//			}
//		}

//		public virtual bool derivedFromType(TypeDefinition ancestorType, short derivationMethod)
//		{

//			if (ancestorType is XercesTypeDefinition)
//			{
//				XercesTypeDefinition xercesType = (XercesTypeDefinition)ancestorType;
//				return xsTypeDefinition.derivedFromType(xercesType.xsTypeDefinition, mapFlags(derivationMethod));
//			}
//			else
//			{
//				return xsTypeDefinition.derivedFrom(ancestorType.Namespace, ancestorType.Name, mapFlags(derivationMethod));
//			}
//		}

//		public virtual bool derivedFrom(string @namespace, string name, short derivationMethod)
//		{

//			return xsTypeDefinition.derivedFrom(@namespace, name, mapFlags(derivationMethod));
//		}

//		private static short mapFlags(short modelFlags)
//		{
//			short xercesFlags = 0;
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_NONE) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_NONE;
//			}
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_EXTENSION) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_EXTENSION;
//			}
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_RESTRICTION) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_RESTRICTION;
//			}
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_SUBSTITUTION) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_SUBSTITUTION;
//			}
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_UNION) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_UNION;
//			}
//			if ((modelFlags & org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_LIST) != 0)
//			{
//				xercesFlags |= XSConstants.DERIVATION_LIST;
//			}

//			return xercesFlags;
//		}

//		public virtual IList getSimpleTypes(Attr attr)
//		{
//			PSVIAttrNSImpl psviAttr = (PSVIAttrNSImpl)attr;
//			return mapList(psviAttr.ItemValueTypes);
//		}

//		public virtual IList getSimpleTypes(Element element)
//		{
//			PSVIElementNSImpl psviElement = (PSVIElementNSImpl)element;
//			return mapList(psviElement.ItemValueTypes);
//		}

//		private IList mapList(ShortList valueTypes)
//		{
//			if (valueTypes == null)
//			{
//				return null;
//			}
//			IList types = new LinkedList();
//			int limit = valueTypes.Length;
//			for (int i = 0; i < limit; ++i)
//			{
//				types.Add(Convert.ToInt16(valueTypes.item(i)));
//			}
//			return types;
//		}

//		public static XercesTypeDefinition createTypeDefinition(XSTypeDefinition ad)
//		{
//			if (ad is XSSimpleType)
//			{
//				return new SimpleXercesType((XSSimpleType)ad);
//			}
//			else if (ad is XSSimpleTypeDefinition)
//			{
//				return new SimpleXercesTypeDefinition((XSSimpleTypeDefinition)ad);
//			}
//			else
//			{
//				return new ComplexXercesTypeDefinition((XSComplexTypeDefinition)ad);
//			}
//		}

//		public static SimpleXercesTypeDefinition createTypeDefinition(XSSimpleTypeDefinition ad)
//		{
//			if (ad is XSSimpleType)
//			{
//				return new SimpleXercesType((XSSimpleType)ad);
//			}
//			return new SimpleXercesTypeDefinition((XSSimpleTypeDefinition)ad);
//		}

//		public static ComplexXercesTypeDefinition createTypeDefinition(XSComplexTypeDefinition ad)
//		{
//			return new ComplexXercesTypeDefinition((XSComplexTypeDefinition)ad);
//		}

//	}

//}