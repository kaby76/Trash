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

	using org.eclipse.wst.xml.xpath2.processor.@internal.types;

	/// <summary>
	/// Class for Variable Expression Pairs.
	/// </summary>
	public class VarExprPair
	{
		private QName _var;
		private Expr _expr;

		/// <summary>
		/// Constructor for VarExprPair.
		/// </summary>
		/// <param name="var">
		///            QName variable. </param>
		/// <param name="expr">
		///            Expression. </param>
		public VarExprPair(QName @var, Expr expr)
		{
			_var = @var;
			_expr = expr;
		}

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		public virtual QName varname()
		{
			return _var;
		}

		/// <summary>
		/// Support for Expression interface.
		/// </summary>
		/// <returns> Result of Expr operation. </returns>
		public virtual Expr expr()
		{
			return _expr;
		}
	}

}