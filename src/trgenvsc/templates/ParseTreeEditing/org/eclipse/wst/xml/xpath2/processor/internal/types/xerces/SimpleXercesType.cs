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

//	using InvalidDatatypeValueException = org.apache.xerces.impl.dv.InvalidDatatypeValueException;
//	using ValidatedInfo = org.apache.xerces.impl.dv.ValidatedInfo;
//	using ValidationContext = org.apache.xerces.impl.dv.ValidationContext;
//	using XSSimpleType = org.apache.xerces.impl.dv.XSSimpleType;
//	using ValidationState = org.apache.xerces.impl.validation.ValidationState;
//	using Item = org.eclipse.wst.xml.xpath2.api.Item;
//	using PrimitiveType = org.eclipse.wst.xml.xpath2.api.typesystem.PrimitiveType;

//	public class SimpleXercesType : SimpleXercesTypeDefinition, PrimitiveType
//	{

//		private readonly XSSimpleType simpleType;

//		public SimpleXercesType(XSSimpleType ad) : base(ad)
//		{
//			this.simpleType = ad;
//		}

//		public virtual short PrimitiveKind
//		{
//			get
//			{
//				return simpleType.PrimitiveKind;
//			}
//		}

//		public virtual bool validate(string content)
//		{
//			bool isValueValid = true;
//			try
//			{
//				ValidatedInfo validatedInfo = new ValidatedInfo();
//				ValidationContext validationState = new ValidationState();

//				// attempt to validate the value with the simpleType
//				simpleType.validate(content, validationState, validatedInfo);
//			}
//			catch (InvalidDatatypeValueException)
//			{
//				isValueValid = false;
//			}
//			return isValueValid;
//		}

//		public virtual bool isEqual(object value1, object value2)
//		{
//			return simpleType.isEqual(value1, value2);
//		}

//		public virtual bool IDType
//		{
//			get
//			{
//				return simpleType.IDType;
//			}
//		}

//		public virtual bool validateNative(object content)
//		{

//			return false;
//		}

//		public virtual Item construct(object content)
//		{
//			throw new Exception("construct not supported for Xerces types");
//		}

//		public virtual Type InterfaceClass
//		{
//			get
//			{
//				throw new Exception("getInterfaceClass not supported for Xerces types");
//			}
//		}

//		public override Type NativeType
//		{
//			get
//			{
//				throw new Exception("getNativeType not supported for Xerces types");
//			}
//		}


//	}

//}