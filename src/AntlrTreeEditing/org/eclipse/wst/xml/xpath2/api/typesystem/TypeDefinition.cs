using System;
using System.Collections;

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

	using Attr = org.w3c.dom.Attr;
	using Element = org.w3c.dom.Element;


	/// <summary>
	/// @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// </summary>
	public interface TypeDefinition
	{
		string Namespace {get;}
		string Name {get;}

		TypeDefinition BaseType {get;}

		bool derivedFromType(TypeDefinition ancestorType, short derivationMethod);

		bool derivedFrom(string @namespace, string name, short derivationMethod);

		Type NativeType {get;}

		IList getSimpleTypes(Attr attr);

		  IList getSimpleTypes(Element attr);

	}

	public static class TypeDefinition_Fields
	{
		public const short DERIVATION_NONE = 0;
		public const short DERIVATION_EXTENSION = 1;
		public const short DERIVATION_RESTRICTION = 2;
		public const short DERIVATION_SUBSTITUTION = 4;
		public const short DERIVATION_UNION = 8;
		public const short DERIVATION_LIST = 16;
	}

}