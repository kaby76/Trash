using System.Collections.Generic;

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

	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;

	/// <summary>
	/// Support for functions.
	/// @since 2.0
	/// </summary>
	public interface Function
	{

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		string Name {get;}

		/// <summary>
		/// Minimal number of allowed arguments.
		/// </summary>
		/// <returns> The smallest number of arguments possible </returns>
		int MinArity {get;}

		/// <summary>
		/// Maximum number of allowed arguments.
		/// </summary>
		/// <returns> The highest number of arguments possible </returns>
		int MaxArity {get;}

		/// <summary>
		/// Maximum number of allowed arguments.
		/// </summary>
		/// <returns> The highest number of arguments possible </returns>
		bool VariableArgument {get;}

		/// <summary>
		/// Checks if this function has an to the
		/// </summary>
		/// <param name="actual_arity">
		/// @return </param>
		bool canMatchArity(int actualArity);

		/// <summary>
		/// Gets the return type for the function.
		/// </summary>
		/// <returns> TypeDefinition for the argument </returns>
		TypeDefinition ResultType {get;}

		/// <summary>
		/// Returns the type of the argument at position index,
		/// starting at 0.  
		/// </summary>
		/// <returns> TypeDefinition for the argument </returns>
		TypeDefinition getArgumentType(int index);

		/// <summary>
		/// Name hint for the index'th argument,  
		/// starting at 0.
		/// </summary>
		/// <returns> TypeDefinition for the argument </returns>
		string getArgumentNameHint(int index);

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <returns> Result of evaluation. </returns>
		ResultSequence evaluate(ICollection<ResultSequence> args, EvaluationContext evaluationContext);

		/// <summary>
		/// Evaluate the exact result type.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <returns> Result of evaluation. </returns>
		TypeDefinition computeReturnType(ICollection<TypeDefinition> args, StaticContext sc);
	}

}