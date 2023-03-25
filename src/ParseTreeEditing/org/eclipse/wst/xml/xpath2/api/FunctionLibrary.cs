/// <summary>
///*****************************************************************************
/// Copyright (c) 2009, 2011 Jesper Moller, and others
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

namespace org.eclipse.wst.xml.xpath2.api
{


	/// <summary>
	/// Interface for function libraries support.
	/// @since 2.0
	/// </summary>
	public interface FunctionLibrary
	{

		/// <summary>
		/// Checks whether the function exists or not.
		/// </summary>
		/// <param name="name">
		///            Name of function. </param>
		/// <param name="arity">
		///            arity of the function, </param>
		/// <returns> Result of the test. </returns>
		bool functionExists(string name, int arity);

		/// <summary>
		/// Function support.
		/// </summary>
		/// <param name="name">
		///            local name . </param>
		/// <param name="arity">
		///            arity of the function. </param>
		/// <returns> The function from the library. </returns>
		Function resolveFunction(string localName, int arity);

		/// <summary>
		/// Returns the namespace of the function library.
		/// </summary>
		/// <returns> Namespace. </returns>
		string Namespace {get;}
	}

	public static class FunctionLibrary_Fields
	{
		public static readonly int VARIABLE_ARITY = int.MaxValue;
	}

}