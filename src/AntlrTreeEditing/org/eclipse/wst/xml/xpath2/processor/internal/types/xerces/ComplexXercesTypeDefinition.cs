//using System;

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

//	using XSComplexTypeDefinition = org.apache.xerces.xs.XSComplexTypeDefinition;
//	using XSSimpleTypeDefinition = org.apache.xerces.xs.XSSimpleTypeDefinition;
//	using ComplexTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.ComplexTypeDefinition;
//	using SimpleTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition;
//	using NodeList = org.w3c.dom.NodeList;

//	public class ComplexXercesTypeDefinition : XercesTypeDefinition, ComplexTypeDefinition
//	{

//		private readonly XSComplexTypeDefinition complexTypeDefinition;

//		public ComplexXercesTypeDefinition(XSComplexTypeDefinition ad) : base(ad)
//		{
//			this.complexTypeDefinition = ad;
//		}

//		public virtual SimpleTypeDefinition SimpleType
//		{
//			get
//			{
//				XSSimpleTypeDefinition simpleType = complexTypeDefinition.SimpleType;
//				if (simpleType != null)
//				{
//					return createTypeDefinition(simpleType);
//				}
//				else
//				{
//					return null;
//				}
//			}
//		}

//		public virtual short DerivationMethod
//		{
//			get
//			{
//				// TODO: Map it
//				return complexTypeDefinition.DerivationMethod;
//			}
//		}

//		public virtual bool Abstract
//		{
//			get
//			{
//				return complexTypeDefinition.Abstract;
//			}
//		}

//		public virtual short ContentType
//		{
//			get
//			{
//				return complexTypeDefinition.ContentType;
//			}
//		}

//		public virtual bool isProhibitedSubstitution(short restriction)
//		{
//			return complexTypeDefinition.isProhibitedSubstitution(restriction);
//		}

//		public virtual short ProhibitedSubstitutions
//		{
//			get
//			{
//				return complexTypeDefinition.ProhibitedSubstitutions;
//			}
//		}

//		public override Type NativeType
//		{
//			get
//			{
//				return typeof(NodeList);
//			}
//		}

//	}

//}