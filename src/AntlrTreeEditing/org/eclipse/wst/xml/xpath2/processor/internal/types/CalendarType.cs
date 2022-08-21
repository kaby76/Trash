
/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     David Carver - bug 280547 - fix dates for comparison 
///     Jesper Steen Moller  - bug 262765 - fix type tests
/// ******************************************************************************
/// </summary>

using java.util;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{
	using Calendar = java.util.Calendar;
	using GregorianCalendar = java.util.GregorianCalendar;


	// common base for anything that uses a calendar... basically stuff doing with
	// time... hopefully in the future this may be factored out here
	/// <summary>
	/// Common base for all Calendar based classes
	/// </summary>
	public abstract class CalendarType : CtrType
	{

		public abstract Calendar calendar();

		public virtual Calendar normalizeCalendar(Calendar cal, XSDuration timezone)
		{
            Calendar adjusted = (Calendar) cal.clone();

			if (timezone != null)
			{
				int hours = timezone.hours();
				int minutes = timezone.minutes();
				if (!timezone.negative())
				{
					hours *= -1;
					minutes *= -1;
				}
				adjusted.AddHours(hours);
				adjusted.AddMinutes(minutes);
			}

			return adjusted;

		}

		protected internal virtual bool isGDataType(AnyType aat)
		{
			if (!(aat is AnyAtomicType))
			{
				return false;
			}

			string type = aat.string_type();
			if (type.Equals("xs:gMonthDay") || type.Equals("xs:gDay") || type.Equals("xs:gMonth") || type.Equals("xs:gYear") || type.Equals("xs:gYearMonth"))
			{
				return true;
			}
			return false;
		}

		public override object NativeValue
		{
			get
			{
				return _datatypeFactory.newXMLGregorianCalendar((GregorianCalendar)calendar());
			}
		}
	}

}