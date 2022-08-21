using System;
using System.Collections;
using java.util;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, 2018 IBM Corporation and others.
/// This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     IBM Corporation - initial API and implementation
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin
{
    using Duration = javax.xml.datatype.Duration;


	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using Node = org.w3c.dom.Node;


	/// <summary>
	/// This type captures all of the built-in XPath 2.0 types, as defined in F&O specification.
	/// http://www.w3.org/TR/xquery-operators/#datatypes
	/// </summary>
	public class BuiltinTypeLibrary
	{
		// This type captures all of the built-in XPath 2.0 types, as defined in F&O specification.
		// http://www.w3.org/TR/xquery-operators/#datatypes

		public static readonly BuiltinTypeDefinition XS_ANYTYPE = new BuiltinTypeDefinition("anyType", null);
			public static readonly BuiltinTypeDefinition XS_UNTYPED = new BuiltinTypeDefinition("untyped", XS_ANYTYPE);
			public static readonly BuiltinTypeDefinition XS_ANYSIMPLETYPE = new BuiltinTypeDefinition("anySimpleType", XS_ANYTYPE);
				//	Subtypes of user-defined list and union types
				//	xs:IDREFS
				//	xs:NMTOKENS
				//	xs:ENTITIES
				//	xs:anyAtomicType
				public static readonly BuiltinAtomicTypeDefinition XS_ANYATOMICTYPE = new BuiltinAtomicTypeDefinition("anyAtomicType", typeof(AnyAtomicType), typeof(object), XS_ANYSIMPLETYPE);

		public static readonly BuiltinAtomicTypeDefinition XS_UNTYPEDATOMIC = new BuiltinAtomicTypeDefinition("untypedAtomic", null, typeof(string), XS_ANYATOMICTYPE);

		public static readonly BuiltinAtomicTypeDefinition XS_DATETIME = new BuiltinAtomicTypeDefinition("dateTime", typeof(XSDateTime), typeof(Calendar),XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_DATE = new BuiltinAtomicTypeDefinition("date", typeof(XSDate), typeof(Calendar), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_TIME = new BuiltinAtomicTypeDefinition("time", typeof(XSTime), typeof(Calendar), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_DURATION = new BuiltinAtomicTypeDefinition("duration", typeof(XSDuration), typeof(Duration), XS_ANYATOMICTYPE);
			public static readonly BuiltinAtomicTypeDefinition XS_YEARMONTHDURATION = new BuiltinAtomicTypeDefinition("yearMonthDuration", typeof(XSYearMonthDuration), typeof(Duration), XS_DURATION);
			public static readonly BuiltinAtomicTypeDefinition XS_DAYTIMEDURATION = new BuiltinAtomicTypeDefinition("dayTimeDuration", typeof(XSDayTimeDuration), typeof(Duration), XS_DURATION);
		public static readonly BuiltinAtomicTypeDefinition XS_FLOAT = new BuiltinAtomicTypeDefinition("float", typeof(XSFloat), typeof(float), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_DOUBLE = new BuiltinAtomicTypeDefinition("double", typeof(XSDouble), typeof(Double), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_DECIMAL = new BuiltinAtomicTypeDefinition("decimal", typeof(XSDecimal), typeof(decimal), XS_ANYATOMICTYPE);
			public static readonly BuiltinAtomicTypeDefinition XS_INTEGER = new BuiltinAtomicTypeDefinition("integer", typeof(XSInteger), typeof(System.Numerics.BigInteger), XS_DECIMAL);
				public static readonly BuiltinAtomicTypeDefinition XS_NONPOSITIVEINTEGER = new BuiltinAtomicTypeDefinition("nonPositiveInteger", typeof(XSNonPositiveInteger), typeof(System.Numerics.BigInteger), XS_INTEGER);
					public static readonly BuiltinAtomicTypeDefinition XS_NEGATIVEINTEGER = new BuiltinAtomicTypeDefinition("negativeInteger", typeof(XSNegativeInteger), typeof(System.Numerics.BigInteger), XS_NONPOSITIVEINTEGER);
				public static readonly BuiltinAtomicTypeDefinition XS_LONG = new BuiltinAtomicTypeDefinition("long", typeof(XSLong), typeof(long), XS_INTEGER);
					public static readonly BuiltinAtomicTypeDefinition XS_INT = new BuiltinAtomicTypeDefinition("int", typeof(XSInt), typeof(int), XS_LONG);
						public static readonly BuiltinAtomicTypeDefinition XS_SHORT = new BuiltinAtomicTypeDefinition("short", typeof(XSShort), typeof(short), XS_INT);
							public static readonly BuiltinAtomicTypeDefinition XS_BYTE = new BuiltinAtomicTypeDefinition("byte", typeof(XSByte), typeof(Byte), XS_SHORT);
				public static readonly BuiltinAtomicTypeDefinition XS_NONNEGATIVEINTEGER = new BuiltinAtomicTypeDefinition("nonNegativeInteger", typeof(XSNonNegativeInteger), typeof(System.Numerics.BigInteger), XS_INTEGER);
					public static readonly BuiltinAtomicTypeDefinition XS_UNSIGNEDLONG = new BuiltinAtomicTypeDefinition("unsignedLong", typeof(XSUnsignedLong), typeof(System.Numerics.BigInteger), XS_NONNEGATIVEINTEGER);
						public static readonly BuiltinAtomicTypeDefinition XS_UNSIGNEDINT = new BuiltinAtomicTypeDefinition("unsignedInt", typeof(XSUnsignedInt), typeof(long), XS_UNSIGNEDLONG);
							public static readonly BuiltinAtomicTypeDefinition XS_UNSIGNEDSHORT = new BuiltinAtomicTypeDefinition("unsignedShort", typeof(XSUnsignedShort), typeof(int), XS_UNSIGNEDINT);
								public static readonly BuiltinAtomicTypeDefinition XS_UNSIGNEDBYTE = new BuiltinAtomicTypeDefinition("unsignedByte", typeof(XSUnsignedByte), typeof(short), XS_UNSIGNEDSHORT);
					public static readonly BuiltinAtomicTypeDefinition XS_POSITIVEINTEGER = new BuiltinAtomicTypeDefinition("positiveInteger", typeof(XSPositiveInteger), typeof(System.Numerics.BigInteger), XS_NONNEGATIVEINTEGER);
		public static readonly BuiltinAtomicTypeDefinition XS_GYEARMONTH = new BuiltinAtomicTypeDefinition("gYearMonth", typeof(XSGYearMonth), typeof(DateTime), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_GYEAR = new BuiltinAtomicTypeDefinition("gYear", typeof(XSGYear), typeof(DateTime), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_GMONTHDAY = new BuiltinAtomicTypeDefinition("gMonthDay", typeof(XSGMonthDay), typeof(DateTime), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_GDAY = new BuiltinAtomicTypeDefinition("gDay", typeof(XSGDay), typeof(DateTime), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_GMONTH = new BuiltinAtomicTypeDefinition("gMonth", typeof(XSGMonth), typeof(DateTime), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_STRING = new BuiltinAtomicTypeDefinition("string", typeof(XSString), typeof(string), XS_ANYATOMICTYPE);
			public static readonly BuiltinAtomicTypeDefinition XS_NORMALIZEDSTRING = new BuiltinAtomicTypeDefinition("normalizedString", typeof(XSString), typeof(string), XS_ANYATOMICTYPE);
				public static readonly BuiltinAtomicTypeDefinition XS_TOKEN = new BuiltinAtomicTypeDefinition("token", typeof(XSToken), typeof(string), XS_NORMALIZEDSTRING);
					public static readonly BuiltinAtomicTypeDefinition XS_LANGUAGE = new BuiltinAtomicTypeDefinition("language", typeof(XSToken), typeof(string), XS_TOKEN);
					public static readonly BuiltinAtomicTypeDefinition XS_NMTOKEN = new BuiltinAtomicTypeDefinition("NMTOKEN", typeof(XSNMTOKEN), typeof(string), XS_TOKEN);
					public static readonly BuiltinAtomicTypeDefinition XS_NAME = new BuiltinAtomicTypeDefinition("Name", typeof(XSName), typeof(string), XS_TOKEN);
						public static readonly BuiltinAtomicTypeDefinition XS_NCNAME = new BuiltinAtomicTypeDefinition("NCName", typeof(XSNCName), typeof(string), XS_NAME);
							public static readonly BuiltinAtomicTypeDefinition XS_ID = new BuiltinAtomicTypeDefinition("ID", typeof(XSID), typeof(string), XS_NCNAME);
							public static readonly BuiltinAtomicTypeDefinition XS_IDREF = new BuiltinAtomicTypeDefinition("IDREF", typeof(XSIDREF), typeof(string), XS_NCNAME);
							public static readonly BuiltinAtomicTypeDefinition XS_ENTITY = new BuiltinAtomicTypeDefinition("ENTITY", typeof(XSEntity), typeof(string), XS_NCNAME);
		public static readonly BuiltinAtomicTypeDefinition XS_BOOLEAN = new BuiltinAtomicTypeDefinition("boolean", typeof(XSBoolean), typeof(Boolean), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_BASE64BINARY = new BuiltinAtomicTypeDefinition("base64Binary", typeof(XSBase64Binary), typeof(sbyte[]), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_HEXBINARY = new BuiltinAtomicTypeDefinition("hexBinary", typeof(XSHexBinary), typeof(sbyte[]), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_ANYURI = new BuiltinAtomicTypeDefinition("anyURI", typeof(XSAnyURI), typeof(string), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_QNAME = new BuiltinAtomicTypeDefinition("QName", typeof(QName), typeof(javax.xml.@namespace.QName), XS_ANYATOMICTYPE);
		public static readonly BuiltinAtomicTypeDefinition XS_NOTATION = new BuiltinAtomicTypeDefinition("NOTATION", typeof(XSNotation), typeof(string), XS_ANYATOMICTYPE);

		// These are also subtypes of any types 
		public static readonly BuiltinListTypeDefinition XS_IDREFS = new BuiltinListTypeDefinition("IDREFS", XS_ANYTYPE, XS_IDREF);
		public static readonly BuiltinListTypeDefinition XS_NMTOKENS = new BuiltinListTypeDefinition("NMTOKENS", XS_ANYTYPE, XS_NMTOKEN);
		public static readonly BuiltinListTypeDefinition XS_ENTITIES = new BuiltinListTypeDefinition("ENTITIES", XS_ANYTYPE, XS_ENTITY);

		public static readonly TypeModel BUILTIN_TYPES = new TypeModelAnonymousInnerClass();

		private class TypeModelAnonymousInnerClass : TypeModel
		{
			public TypeModelAnonymousInnerClass()
			{
			}


			public virtual TypeDefinition lookupType(string @namespace, string typeName)
			{
				if (!BuiltinTypeDefinition.XS_NS.Equals(@namespace))
				{
					return null;
				}
				return (TypeDefinition) builtinTypes[typeName];
			}

			public virtual TypeDefinition lookupElementDeclaration(string @namespace, string elementName)
			{
				return null;
			}

			public virtual TypeDefinition lookupAttributeDeclaration(string @namespace, string attributeName)
			{
				return null;
			}

			private IDictionary builtinTypes = new Hashtable();
	//		{
	//			addType(XS_ANYTYPE);
	//			addType(XS_UNTYPED);
	//			addType(XS_ANYSIMPLETYPE);
	//			addType(XS_ANYATOMICTYPE);
	//
	//			addType(XS_UNTYPEDATOMIC);
	//
	//			addType(XS_DATETIME);
	//			addType(XS_DATE);
	//			addType(XS_TIME);
	//			addType(XS_DURATION);
	//			addType(XS_YEARMONTHDURATION);
	//			addType(XS_DAYTIMEDURATION);
	//			addType(XS_FLOAT);
	//			addType(XS_DOUBLE);
	//			addType(XS_DECIMAL);
	//			addType(XS_INTEGER);
	//			addType(XS_NONPOSITIVEINTEGER);
	//			addType(XS_NEGATIVEINTEGER);
	//			addType(XS_LONG);
	//			addType(XS_INT);
	//			addType(XS_SHORT);
	//			addType(XS_BYTE);
	//			addType(XS_NONNEGATIVEINTEGER);
	//			addType(XS_UNSIGNEDLONG);
	//			addType(XS_UNSIGNEDINT);
	//			addType(XS_UNSIGNEDSHORT);
	//			addType(XS_UNSIGNEDBYTE);
	//			addType(XS_POSITIVEINTEGER);
	//			addType(XS_GYEARMONTH);
	//			addType(XS_GYEAR);
	//			addType(XS_GMONTHDAY);
	//			addType(XS_GDAY);
	//			addType(XS_GMONTH);
	//			addType(XS_STRING);
	//			addType(XS_NORMALIZEDSTRING);
	//			addType(XS_TOKEN);
	//			addType(XS_LANGUAGE);
	//			addType(XS_NMTOKEN);
	//			addType(XS_NAME);
	//			addType(XS_NCNAME);
	//			addType(XS_ID);
	//			addType(XS_IDREF);
	//			addType(XS_ENTITY);
	//			addType(XS_BOOLEAN);
	//			addType(XS_BASE64BINARY);
	//			addType(XS_HEXBINARY);
	//			addType(XS_ANYURI);
	//			addType(XS_QNAME);
	//			addType(XS_NOTATION);
	//			addType(XS_IDREFS);
	//			addType(XS_NMTOKENS);
	//			addType(XS_ENTITIES);
	//		}

			private void addType(BuiltinTypeDefinition btd)
			{
				builtinTypes[btd.Name] = btd;
			}

			public virtual TypeDefinition getType(Node node)
			{
				return null;
			}
		}

		// Not types under XML Schema namespace
	//	item
	//	xs:anyAtomicType
	//	node
	//	attribute
	//	user-defined attribute types
	//	comment
	//	document
	//	user-defined document types
	//	element
	//	user-defined element types
	//	processing-instruction
	//	text



	}

}