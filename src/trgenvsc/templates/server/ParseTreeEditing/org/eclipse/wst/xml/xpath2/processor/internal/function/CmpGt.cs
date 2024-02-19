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
///     Jesper Moller - bug 280555 - Add pluggable collation support
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;

	/// <summary>
	/// Class for compare for greater than operation.
	/// </summary>
	public interface CmpGt
	{
		/// <summary>
		/// Constructor for CmpGt
		/// </summary>
		/// <param name="arg">
		///            argument of any type. </param>
		/// <param name="context"> TODO </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of operation, true/false. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(org.eclipse.wst.xml.xpath2.processor.internal.types.AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError;
		bool gt(AnyType arg, DynamicContext context);
	}

}