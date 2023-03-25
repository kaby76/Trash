/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2009 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using org.eclipse.wst.xml.xpath2.processor.@internal.ast;

	/// <summary>
	/// Interface to static checker.
	/// </summary>
	public interface StaticChecker
	{

		/// <summary>
		/// checks XPathNode
		/// </summary>
		/// <exception cref="static"> error. </exception>
		/// <param name="root">
		///            is an XPath node. </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void check(XPathNode root) throws StaticError;
		void check(XPathNode root);
	}

}