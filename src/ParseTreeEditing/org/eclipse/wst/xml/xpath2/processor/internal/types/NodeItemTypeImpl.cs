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

using org.eclipse.wst.xml.xpath2.api.typesystem;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using NodeItemType = org.eclipse.wst.xml.xpath2.api.typesystem.NodeItemType;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	public class NodeItemTypeImpl : NodeItemType
	{

		public NodeItemTypeImpl(short occurrence, short nodeType) : this(occurrence, nodeType, null, null, true)
		{
		}

		public NodeItemTypeImpl(short nodeType) : this(ItemType_Fields.OCCURRENCE_ONE, nodeType, null, null, true)
		{
		}

		public NodeItemTypeImpl(short occurrence, short nodeType, TypeDefinition typeDefinition, QName name, bool wildcard)
		{
			this.occurrence = occurrence;
			this.typeDefinition = typeDefinition;
			this.nodeType = nodeType;
			this.name = name;
			this.wildcard = wildcard;
		}

		private readonly short occurrence;
		private readonly TypeDefinition typeDefinition;
		private readonly short nodeType;
		private readonly QName name;
		private readonly bool wildcard;

		public virtual short Occurrence
		{
			get
			{
				return occurrence;
			}
		}

		public virtual TypeDefinition TypeDefinition
		{
			get
			{
				return typeDefinition;
			}
		}

		public virtual bool Wildcard
		{
			get
			{
				return wildcard;
			}
		}

		public virtual QName Name
		{
			get
			{
				return name;
			}
		}

		public virtual short NodeType
		{
			get
			{
				return nodeType;
			}
		}

	}

}