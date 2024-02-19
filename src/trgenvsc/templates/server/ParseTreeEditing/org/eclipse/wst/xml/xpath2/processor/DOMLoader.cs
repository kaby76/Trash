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
///     Jesper Steen Moller - Documented namespace awareness
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using Document = org.w3c.dom.Document;

	/// <summary>
	/// The DOM loader loads the XML document.
	/// </summary>
	public interface DOMLoader
	{

		/// <summary>
		/// The DOM loader loads the XML docuemnt
		/// </summary>
		/// <param name="in">
		///            is the input stream. </param>
		/// <exception cref="DOMLoaderException">
		///             DOM loader exception. </exception>
		/// <returns> The loaded document. The document is always loaded as namespace-aware </returns>
		Document load(System.IO.Stream @in);

		/// <summary>
		/// Set validating boolean.
		/// </summary>
		/// <param name="val">
		///            is the validating boolean. </param>
		// XXX: default is false ?! [document it ?]
		void set_validating(bool val);

	}

}