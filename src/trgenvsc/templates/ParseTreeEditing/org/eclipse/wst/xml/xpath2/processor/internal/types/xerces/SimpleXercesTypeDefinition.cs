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


//	using XSObjectList = org.apache.xerces.xs.XSObjectList;
//	using XSSimpleTypeDefinition = org.apache.xerces.xs.XSSimpleTypeDefinition;
//	using SimpleTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition;
//	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
//	using Node = org.w3c.dom.Node;

//	public class SimpleXercesTypeDefinition : XercesTypeDefinition, SimpleTypeDefinition
//	{

//		private readonly XSSimpleTypeDefinition simpleTypeDefinition;

//		public SimpleXercesTypeDefinition(XSSimpleTypeDefinition ad) : base(ad)
//		{
//			this.simpleTypeDefinition = ad;
//		}

//		public virtual short Variety
//		{
//			get
//			{
//				return simpleTypeDefinition.Variety;
//			}
//		}

//		public virtual SimpleTypeDefinition PrimitiveType
//		{
//			get
//			{
//				return createTypeDefinition(simpleTypeDefinition.PrimitiveType);
//			}
//		}

//		public virtual short BuiltInKind
//		{
//			get
//			{
//				return simpleTypeDefinition.BuiltInKind;
//			}
//		}

//		public virtual TypeDefinition ItemType
//		{
//			get
//			{
//				return createTypeDefinition(simpleTypeDefinition.ItemType);
//			}
//		}

//		public virtual IList MemberTypes
//		{
//			get
//			{
//				XSObjectList xsMemberTypes = simpleTypeDefinition.MemberTypes;
//				IList memberTypes = new LinkedList();
//				for (int i = 0; i < xsMemberTypes.Length; i++)
//				{
//					memberTypes.Add(createTypeDefinition((XSSimpleTypeDefinition) xsMemberTypes.item(i)));
//				}
//				return memberTypes;
//			}
//		}

//		public virtual short Ordered
//		{
//			get
//			{
//				return simpleTypeDefinition.Ordered;
//			}
//		}

//		public virtual bool Finite
//		{
//			get
//			{
//				return simpleTypeDefinition.Finite;
//			}
//		}

//		public virtual bool Bounded
//		{
//			get
//			{
//				return simpleTypeDefinition.Bounded;
//			}
//		}

//		public virtual bool Numeric
//		{
//			get
//			{
//				return simpleTypeDefinition.Numeric;
//			}
//		}

//		public override Type NativeType
//		{
//			get
//			{
//				return typeof(Node);
//			}
//		}

//	}

//}