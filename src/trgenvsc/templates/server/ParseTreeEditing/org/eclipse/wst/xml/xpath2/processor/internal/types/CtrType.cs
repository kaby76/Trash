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
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;

	/// <summary>
	/// A representation of the CtrType datatype
	/// </summary>
	public abstract class CtrType : AnyAtomicType
	{
		// used for constructor functions
		// arg is either empty sequence, or 1 anyatomictype
		/// <summary>
		/// Used for constructor function.
		/// </summary>
		/// <param name="arg">
		///            Either an empty sequence or 1 atomic type </param>
		/// <returns> The resulting ResultSequence </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError;
		public abstract ResultSequence constructor(ResultSequence arg);

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> String representation of the datatype's name </returns>
		public abstract string type_name();

		public override object NativeValue
		{
			get
			{
				return string_value();
			}
		}
	}

}