/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Moller - initial API and implementation
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api.typesystem
{

	/// <summary>
	/// @since 2.0
	/// </summary>
	public interface ItemType
	{

		short Occurrence {get;}
	}

	public static class ItemType_Fields
	{
		public const short OCCURRENCE_OPTIONAL = 0;
		public const short OCCURRENCE_ONE = 1;
		public const short OCCURRENCE_NONE_OR_MANY = 2;
		public const short OCCURRENCE_ONE_OR_MANY = 3;
		public const short ALWAYS_ONE_MASK = 0x01;
		public const short MAYBE_MANY_MASK = 0x02;
	}

}