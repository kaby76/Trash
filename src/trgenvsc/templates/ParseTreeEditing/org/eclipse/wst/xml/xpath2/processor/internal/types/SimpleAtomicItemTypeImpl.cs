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
namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using AtomicItemType = org.eclipse.wst.xml.xpath2.api.AtomicItemType;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	public class SimpleAtomicItemTypeImpl : AtomicItemType
	{

		private readonly short occurrence;
		private readonly TypeDefinition typeDefinition;

		public SimpleAtomicItemTypeImpl(TypeDefinition typeDefinition) : base()
		{
			this.typeDefinition = typeDefinition;
			this.occurrence = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_ONE;
		}

		public SimpleAtomicItemTypeImpl(TypeDefinition typeDefinition, short occurrence) : base()
		{
			this.typeDefinition = typeDefinition;
			this.occurrence = occurrence;
		}

		public virtual short Occurrence
		{
			get
			{
				return this.occurrence;
			}
		}

		public virtual TypeDefinition TypeDefinition
		{
			get
			{
				return typeDefinition;
			}
		}

	}

}