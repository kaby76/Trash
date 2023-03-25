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
///     David Carver (STAR) - bug 273763 - correct error codes 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	/// <summary>
	/// Error caused by bad types.
	/// </summary>
	public class TypeError : XPathException
	{
		/// 
		private const long serialVersionUID = 932275035706936883L;
		// errorcode specified in http://www.w3.org/2004/10/xqt-errors i fink
		private string _code;

		/// <summary>
		/// Constructor for type error.
		/// </summary>
		/// <param name="code">
		///            is the error code. </param>
		/// <param name="err">
		///            is the reason for the error. </param>
		public TypeError(string code, string err) : base(err)
		{
			_code = code;
		}

		/// <summary>
		/// Get the error code.
		/// </summary>
		/// <returns> The error code. </returns>
		public virtual string code()
		{
			return _code;
		}

		/// <summary>
		/// "Factory" for building errors
		/// </summary>
		/// <param name="err">
		///            is the reason for the error. </param>
		/// <returns> the error. </returns>
		public static TypeError ci_not_node(string err)
		{
			string error = "Context item is not a node.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new TypeError("XPTY0020", error);
		}

		/// <summary>
		/// "Factory" for building errors
		/// </summary>
		/// <param name="err">
		///            is the reason for the error. </param>
		/// <returns> the error. </returns>
		public static TypeError mixed_vals(string err)
		{
			string error = "The result of the last step in a path expression contains both nodes and atomic values.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new TypeError("XPTY0018", error);
		}

		/// <summary>
		/// "Factory" for building errors
		/// </summary>
		/// <param name="err">
		///            is the reason for the error. </param>
		/// <returns> the error. </returns>
		public static TypeError step_conatins_atoms(string err)
		{
			string error = "The result of an step (other than the last step) in a path expression contains an atomic value.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new TypeError("XPTY0019", error);
		}

		/// <summary>
		/// "Factory" for building errors
		/// </summary>
		/// <param name="err">
		///            is the reason for the error. </param>
		/// <returns> the error. </returns>
		public static TypeError invalid_type(string err)
		{
			string error = "Value does not match a required type.";

			if (!string.ReferenceEquals(err, null))
			{
				error += " " + err;
			}

			return new TypeError("XPTY0004", error);
		}
	}

}