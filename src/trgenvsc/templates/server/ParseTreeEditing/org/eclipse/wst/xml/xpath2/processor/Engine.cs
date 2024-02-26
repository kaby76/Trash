/// <summary>
///*****************************************************************************
/// Copyright (c) 2011, 2018 IBM Corporation and others.
/// This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     IBM Corporation - initial API and implementation
/// ******************************************************************************
/// </summary>
namespace org.eclipse.wst.xml.xpath2.processor
{

	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using XPath2Engine = org.eclipse.wst.xml.xpath2.api.XPath2Engine;
	using XPath2Expression = org.eclipse.wst.xml.xpath2.api.XPath2Expression;
	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
	using FnBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnBoolean;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public class Engine : XPath2Engine
	{

		public virtual XPath2Expression parseExpression(string expression, api.StaticContext context)
		{

			XPath xPath = (new XPathParserInAntlr()).parse(expression);
			xPath.StaticContext = context;
			StaticNameResolver name_check = new StaticNameResolver(context);
			name_check.check(xPath);

			xPath.Axes = name_check.Axes;
			xPath.FreeVariables = name_check.FreeVariables;
			xPath.ResolvedFunctions = name_check.ResolvedFunctions;
			xPath.RootUsed = name_check.RootUsed;

			return xPath;
		}

		internal virtual bool effectiveBooleanValue(api.ResultSequence rs)
		{
			return FnBoolean.fn_boolean(rs).value();
		}
	}

}