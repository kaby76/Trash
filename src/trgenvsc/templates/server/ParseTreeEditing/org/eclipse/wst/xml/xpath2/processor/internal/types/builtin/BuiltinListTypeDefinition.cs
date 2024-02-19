using System.Collections;

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
	using QName = javax.xml.@namespace.QName;

	using SimpleTypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.SimpleTypeDefinition;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	public class BuiltinListTypeDefinition : BuiltinTypeDefinition, SimpleTypeDefinition
	{

		private readonly BuiltinAtomicTypeDefinition itemType;

		public BuiltinListTypeDefinition(QName name, BuiltinTypeDefinition baseType, BuiltinAtomicTypeDefinition itemType) : base(name, null, typeof(ICollection), baseType)
		{
			this.itemType = itemType;
		}

		public BuiltinListTypeDefinition(string name, BuiltinTypeDefinition baseType, BuiltinAtomicTypeDefinition itemType) : base(name, null, typeof(ICollection), baseType)
		{
			this.itemType = itemType;
		}

		public override bool Abstract
		{
			get
			{
				return false;
			}
		}

		public virtual short Variety
		{
			get
			{
				// TODO Auto-generated method stub
				return 0;
			}
		}

		public virtual SimpleTypeDefinition PrimitiveType
		{
			get
			{
				// TODO Auto-generated method stub
				return null;
			}
		}

		public virtual short BuiltInKind
		{
			get
			{
				// TODO Auto-generated method stub
				return 0;
			}
		}

		public virtual TypeDefinition ItemType
		{
			get
			{
				return itemType;
			}
		}

		public virtual IList MemberTypes
		{
			get
			{
				// TODO Auto-generated method stub
				return null;
			}
		}

		public virtual short Ordered
		{
			get
			{
				// TODO Auto-generated method stub
				return 0;
			}
		}

		public virtual bool Finite
		{
			get
			{
				// TODO Auto-generated method stub
				return false;
			}
		}

		public virtual bool Bounded
		{
			get
			{
				// TODO Auto-generated method stub
				return false;
			}
		}

		public virtual bool Numeric
		{
			get
			{
				// TODO Auto-generated method stub
				return false;
			}
		}

	}

}