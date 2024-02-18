using System.Collections.Generic;

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

namespace org.eclipse.wst.xml.xpath2.api
{
    using QName = javax.xml.@namespace.QName;

	/// <summary>
	/// This interface represents a parsed and statically bound XPath2 expression.
	/// 
	/// @noimplement This interface is not intended to be implemented by clients.
	/// @since 2.0
	/// </summary>
	public interface XPath2Expression
	{
        /// <summary>
        /// The actual string representation of the expression that was parsed.
        /// </summary>
		string Expression { get; }

        /// <summary>
        /// Return a collections of QNames of the names of free variables referenced in the XPath expression.
        /// These variables may be requested during evaluation.
        /// </summary>
        /// <returns> A Collection containing javax.xml.namespacing.QName of free variables </returns>
        ICollection<QName> FreeVariables { get; }

        /// <summary>
        /// Return a collections of the functions used in the XPath2 expression.
        /// </summary>
        /// <returns> A Collection containing javax.xml.namespacing.QName of functions in use. </returns>
        ICollection<QName> ResolvedFunctions { get; }

        /// <summary>
        /// Return a collections of the axis used in the XPath2 expression.
        /// </summary>
        /// <returns> A Collection containing Strings with the axis names in use. </returns>
        ICollection<string> Axes { get; }

        /// <summary>
        /// Whether or not the root path is in use in the XPath2 expression.
        /// </summary>
        /// <returns> true if the expression uses / or //, false otherwise. </returns>
        bool RootPathUsed { get; }

        /// <summary>
		/// Evaluate the XPath2 expression, using the supplied DynamicContext.
		/// </summary>
		/// <param name="dynamicContext"> Dynamic context for the expression. </param>
		/// <param name="contextItems"> Context item (typically nodes, often one) to evaluate under. </param>
		/// <returns> A ResultSequence  </returns>
		ResultSequence evaluate(DynamicContext dynamicContext, object[] contextItems);
	}

}