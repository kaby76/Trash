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

	using XSDayTimeDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDayTimeDuration;
	using XSYearMonthDuration = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSYearMonthDuration;

	public class ScalarTypePromoter : NumericTypePromoter
	{

		protected internal override bool checkCombination(Type newType)
		{

			Type targetType = TargetType;
			if (targetType == typeof(XSDayTimeDuration) || targetType == typeof(XSYearMonthDuration))
			{
				return targetType == newType;
			}
			return base.checkCombination(newType);
		}

		protected internal override Type substitute(Type typeToConsider)
		{
			if (typeToConsider == typeof(XSDayTimeDuration) || typeToConsider == typeof(XSYearMonthDuration))
			{
				return typeToConsider;
			}

			return base.substitute(typeToConsider);
		}

	}

}