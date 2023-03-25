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

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	/// <summary>
	/// Support for Unary expressions.
	/// </summary>
	public abstract class UnExpr : Expr
	{
		private Expr _arg;

		/// <summary>
		/// Constructor for UnExpr.
		/// </summary>
		/// <param name="arg">
		///            expression. </param>
		public UnExpr(Expr arg)
		{
			_arg = arg;
		}

		/// <summary>
		/// Support for Expression interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr arg()
		{
			return _arg;
		}
	}

}