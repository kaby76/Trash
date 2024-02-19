using System.Collections;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Standards for Technology in Automotive Retail and others.
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     David Carver (STAR) - initial API and implementation
///     David Carver (STAR) - bug 296882 - fixed function that would always return false.
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDateTime;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDuration;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;

	public abstract class AbstractCollationEqualFunction : Function
	{

		public AbstractCollationEqualFunction(QName name, int arity) : base(name, arity)
		{
			// TODO Auto-generated constructor stub
		}

		public AbstractCollationEqualFunction(QName name, int min_arity, int max_arity) : base(name, min_arity, max_arity)
		{
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static boolean hasValue(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType itema, org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType itemb, org.eclipse.wst.xml.xpath2.api.DynamicContext context, String collationURI) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		protected internal static bool hasValue(AnyType itema, AnyType itemb, DynamicContext context, string collationURI)
		{
			XSString itemStr = new XSString(itema.StringValue);
			if (isBoolean(itema, itemb))
			{
				XSBoolean boolat = (XSBoolean) itema;
				if (boolat.eq(itemb, context))
				{
					return true;
				}
			}

			if (isNumeric(itema, itemb))
			{
				NumericType numericat = (NumericType) itema;
				if (numericat.eq(itemb, context))
				{
					return true;
				}
			}

			if (isDuration(itema, itemb))
			{
				XSDuration durat = (XSDuration) itema;
				if (durat.eq(itemb, context))
				{
					return true;
				}
			}

			if (needsStringComparison(itema, itemb))
			{
				XSString xstr1 = new XSString(itema.StringValue);
				if (FnCompare.compare_string(collationURI, xstr1, itemStr, context).Equals(System.Numerics.BigInteger.Zero))
				{
					return true;
				}
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static boolean hasValue(org.eclipse.wst.xml.xpath2.api.ResultBuffer rs, org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType item, org.eclipse.wst.xml.xpath2.api.DynamicContext context, String collationURI) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		protected internal static bool hasValue(ResultBuffer rs, AnyAtomicType item, DynamicContext context, string collationURI)
		{
			XSString itemStr = new XSString(item.StringValue);

			for (IEnumerator i = rs.iterator(); i.MoveNext();)
			{
				AnyType at = (AnyType) i.Current;

				if (!(at is CmpEq))
				{
					continue;
				}

				if (isBoolean(item, at))
				{
					XSBoolean boolat = (XSBoolean) at;
					if (boolat.eq(item, context))
					{
						return true;
					}
				}

				if (isNumeric(item, at))
				{
					NumericType numericat = (NumericType) at;
					if (numericat.eq(item, context))
					{
						return true;
					}
				}

				if (isDuration(item, at))
				{
					XSDuration durat = (XSDuration) at;
					if (durat.eq(item, context))
					{
						return true;
					}
				}

				if (needsStringComparison(item, at))
				{
					XSString xstr1 = new XSString(at.StringValue);
					if (FnCompare.compare_string(collationURI, xstr1, itemStr, context).Equals(System.Numerics.BigInteger.Zero))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected internal static bool isDuration(AnyAtomicType item, AnyType at)
		{
			return at is XSDuration && item is XSDuration;
		}

		protected internal static bool isBoolean(AnyAtomicType item, AnyType at)
		{
			return at is XSBoolean && item is XSBoolean;
		}

		protected internal static bool isNumeric(AnyAtomicType item, AnyType at)
		{
			return at is NumericType && item is NumericType;
		}

		protected internal static bool needsStringComparison(AnyAtomicType item, AnyType at)
		{
			AnyType anyItem = (AnyType) item;
			return needsStringComparison(anyItem, at);
		}

		protected internal static bool isDuration(AnyType item, AnyType at)
		{
			return at is XSDuration && item is XSDuration;
		}

		protected internal static bool isDate(AnyType item, AnyType at)
		{
			return at is XSDateTime && item is XSDateTime;
		}


		protected internal static bool isBoolean(AnyType cmptype, AnyType at)
		{
			return at is XSBoolean && cmptype is XSBoolean;
		}

		protected internal static bool isNumeric(AnyType item, AnyType at)
		{
			return at is NumericType && item is NumericType;
		}

		protected internal static bool needsStringComparison(AnyType item, AnyType at)
		{
			if (item is NumericType)
			{
				if (at is XSFloat)
				{
					XSFloat f = (XSFloat) at;
					if (f.nan())
					{
						return true;
					}
				}

				if (at is XSDouble)
				{
					XSDouble d = (XSDouble) at;
					if (d.nan())
					{
						return true;
					}
				}
			}

			if (at is XSString)
			{
				return true;
			}

			if (at is XSUntypedAtomic)
			{
				return true;
			}
			return false;
		}

	}

}