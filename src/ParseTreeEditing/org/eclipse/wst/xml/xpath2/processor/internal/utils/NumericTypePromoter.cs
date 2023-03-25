using System;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Steen Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller - initial API and implementation
///     Jesper Steen Moller - bug 281028 - avg/min/max/sum work
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{

	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using XSDecimal = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDecimal;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSFloat = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSFloat;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;

	public class NumericTypePromoter : TypePromoter
	{

		protected internal override bool checkCombination(Type newType)
		{
			// Note: Double or float will override everything
			if (newType == typeof(XSDouble) || TargetType == typeof(XSDouble))
			{
				TargetType = typeof(XSDouble);
			}
			else if (newType == typeof(XSFloat) || TargetType == typeof(XSFloat))
			{
				TargetType = typeof(XSFloat);
			// If we're still with integers, stick with it
			}
			else if (newType == typeof(XSInteger) && TargetType == typeof(XSInteger))
			{
				TargetType = typeof(XSInteger);
			}
			else
			{
				// Otherwise, switch to decimals
				TargetType = typeof(XSDecimal);
			}
			return true;
		}

		//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType doPromote(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType value) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override AnyAtomicType doPromote(AnyAtomicType value)
		{
			if (TargetType == typeof(XSFloat))
			{
				return new XSFloat(value.StringValue);
			}
			else if (TargetType == typeof(XSDouble))
			{
				return new XSDouble(value.StringValue);
			}
			else if (TargetType == typeof(XSInteger))
			{
				return new XSInteger(value.StringValue);
			}
			else if (TargetType == typeof(XSDecimal))
			{
				return new XSDecimal(value.StringValue);
			}
			return null;
		}

		protected internal override Type substitute(Type typeToConsider)
		{
			if (typeToConsider == typeof(XSUntypedAtomic))
			{
				return typeof(XSDouble);
			}
			if (isDerivedFrom(typeToConsider, typeof(XSFloat)))
			{
				return typeof(XSFloat);
			}
			if (isDerivedFrom(typeToConsider, typeof(XSDouble)))
			{
				return typeof(XSDouble);
			}
			if (isDerivedFrom(typeToConsider, typeof(XSInteger)))
			{
				return typeof(XSInteger);
			}
			if (isDerivedFrom(typeToConsider, typeof(XSDecimal)))
			{
				return typeof(XSDecimal);
			}
			return null;
		}

		private bool isDerivedFrom(Type typeToConsider, Type superType)
		{
			return superType.IsAssignableFrom(typeToConsider);
		}
    }

}