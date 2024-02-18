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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.utils
{

	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;
	using XSDate = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDate;
	using XSDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDateTime;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using XSTime = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSTime;

	public class ComparableTypePromoter : ScalarTypePromoter
	{

		protected internal override bool checkCombination(Type newType)
		{

			Type targetType = TargetType;
			if (newType == typeof(XSString) || newType == typeof(XSTime) || targetType == typeof(XSString) || targetType == typeof(XSTime))
			{
				return targetType == newType;
			}
			if (newType == typeof(XSDate) && targetType == typeof(XSDateTime))
			{
				return true; // leave alone
			}
			if (newType == typeof(XSDateTime) && targetType != typeof(XSDateTime))
			{
				if (targetType == typeof(XSDate))
				{
					TargetType = typeof(XSDateTime);
				}
				else
				{
					return false;
				}
			}

			return base.checkCombination(newType);
		}

		protected internal override Type substitute(Type typeToConsider)
		{
			if (typeToConsider == typeof(XSAnyURI) || typeToConsider == typeof(XSString))
			{
				return typeof(XSString);
			}
			if (typeToConsider == typeof(XSDateTime) || typeToConsider == typeof(XSDate) || typeToConsider == typeof(XSTime))
			{
				return typeToConsider;
			}

			return base.substitute(typeToConsider);
		}

	}

}