using System;

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
	public interface PrimitiveType : TypeDefinition
	{

		/// <summary>
		/// validate a given string against this simple type.
		/// </summary>
		/// <param name="content">       the string value that needs to be validated </param>
		/// <param name="context">       the validation context </param>
		/// <param name="validatedInfo"> used to store validation result
		/// </param>
		/// <returns>              the actual value (QName, Boolean) of the string value </returns>
		bool validate(string content);

		/// <summary>
		/// validate a given string against this simple type.
		/// </summary>
		/// <param name="content">       the string value that needs to be validated </param>
		/// <param name="context">       the validation context </param>
		/// <param name="validatedInfo"> used to store validation result
		/// </param>
		/// <returns>              the actual value (QName, Boolean) of the string value </returns>
		bool validateNative(object content);

		/// <summary>
		/// validate a given string against this simple type.
		/// </summary>
		/// <param name="content">       the string value that needs to be validated </param>
		/// <param name="context">       the validation context </param>
		/// <param name="validatedInfo"> used to store validation result
		/// </param>
		/// <returns>              the actual value (QName, Boolean) of the string value </returns>
		Item construct(object content);

		/// <returns>              the actual value (QName, Boolean) of the string value </returns>
		Type InterfaceClass {get;}

		/// <summary>
		/// Check whether two actual values are equal.
		/// </summary>
		/// <param name="value1">  the first value </param>
		/// <param name="value2">  the second value </param>
		/// <returns>        true if the two value are equal </returns>
		bool isEqual(object value1, object value2);

		/// <summary>
		/// Check whether this type is or is derived from ID.
		/// REVISIT: this method makes ID special, which is not a good design.
		///          but since ID is not a primitive, there doesn't seem to be a
		///          clean way of doing it except to define special method like this.
		/// </summary>
		/// <returns>  whether this simple type is or is derived from ID. </returns>
		bool IDType {get;}

		/// <summary>
		/// Return the whitespace corresponding to this datatype.
		/// </summary>
		/// <returns> valid values are WS_PRESERVE, WS_REPLACE, WS_COLLAPSE. </returns>
		/// <exception cref="DatatypeException">
		///                   union datatypes don't have whitespace facet associated with them </exception>
	//    public short getWhitespace() throws DatatypeException;
	}

}