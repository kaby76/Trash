/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2013 Andrea Bittau, University College London, and others
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

using xpath.org.eclipse.wst.xml.xpath2.processor.@internal.ast;

namespace org.eclipse.wst.xml.xpath2.processor.@internal.ast
{

	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;

	/// <summary>
	/// Visitor class for XPath expressions.
	/// </summary>
	public interface XPathVisitor
	{
		/// <summary>
		/// Visit XPath.
		/// </summary>
		object visit(XPath xp);

		/// <summary>
		/// Visit ForExpr.
		/// </summary>
		object visit(ForExpr fex);

		/// <summary>
		/// Visit QuantifiedExpr.
		/// </summary>
		object visit(QuantifiedExpr qex);

		/// <summary>
		/// Visit IfExpr.
		/// </summary>
		object visit(IfExpr ifex);

		/// <summary>
		/// Visit OrExpr.
		/// </summary>
		object visit(OrExpr orex);

		/// <summary>
		/// Visit AndExpr.
		/// </summary>
		object visit(AndExpr andex);

		/// <summary>
		/// Visit CmpExpr.
		/// </summary>
		object visit(CmpExpr cmpex);

		/// <summary>
		/// Visit RangeExpr.
		/// </summary>
		object visit(RangeExpr rex);

		/// <summary>
		/// Visit AddExpr.
		/// </summary>
		object visit(AddExpr addex);

		/// <summary>
		/// Visit SubExpr.
		/// </summary>
		object visit(SubExpr subex);

		/// <summary>
		/// Visit MulExpr.
		/// </summary>
		object visit(MulExpr mulex);

		/// <summary>
		/// Visit DivExpr.
		/// </summary>
		object visit(DivExpr mulex);

		/// <summary>
		/// Visit IDivExpr.
		/// </summary>
		object visit(IDivExpr mulex);

		/// <summary>
		/// Visit ModExpr.
		/// </summary>
		object visit(ModExpr mulex);

		/// <summary>
		/// Visit UnionExpr.
		/// </summary>
		object visit(UnionExpr unex);

		/// <summary>
		/// Visit PipeExpr.
		/// </summary>
		object visit(PipeExpr pipex);

		/// <summary>
		/// Visit IntersectExpr.
		/// </summary>
		object visit(IntersectExpr iexpr);

		/// <summary>
		/// Visit ExceptExpr.
		/// </summary>
		object visit(ExceptExpr eexpr);

		/// <summary>
		/// Visit InstOfExpr.
		/// </summary>
		object visit(InstOfExpr ioexp);

		/// <summary>
		/// Visit TreatAsExpr.
		/// </summary>
		object visit(TreatAsExpr taexp);

		/// <summary>
		/// Visit CastableExpr.
		/// </summary>
		object visit(CastableExpr cexp);

		/// <summary>
		/// Visit CastExpr.
		/// </summary>
		object visit(CastExpr cexp);

		/// <summary>
		/// Visit MinusExpr.
		/// </summary>
		object visit(MinusExpr e);

		/// <summary>
		/// Visit PlusExpr.
		/// </summary>
		object visit(PlusExpr e);

		/// <summary>
		/// Visit XPathExpr.
		/// </summary>
		object visit(XPathExpr e);

		/// <summary>
		/// Visit ForwardStep.
		/// </summary>
		object visit(ForwardStep e);

		/// <summary>
		/// Visit ReverseStep.
		/// </summary>
		object visit(ReverseStep e);

		/// <summary>
		/// Visit NameTest.
		/// </summary>
		object visit(NameTest e);

		/// <summary>
		/// Visit VarRef.
		/// </summary>
		object visit(VarRef e);

		/// <summary>
		/// Visit StringLiteral.
		/// </summary>
		object visit(StringLiteral e);

		/// <summary>
		/// Visit IntegerLiteral.
		/// </summary>
		object visit(IntegerLiteral e);

		/// <summary>
		/// Visit DoubleLiteral.
		/// </summary>
		object visit(DoubleLiteral e);

		/// <summary>
		/// Visit DecimalLiteral.
		/// </summary>
		object visit(DecimalLiteral e);

		/// <summary>
		/// Visit ParExpr.
		/// </summary>
		object visit(ParExpr e);

		/// <summary>
		/// Visit CntxItemExpr.
		/// </summary>
		object visit(CntxItemExpr e);

		/// <summary>
		/// Visit FunctionCall.
		/// </summary>
		object visit(FunctionCall e);

		/// <summary>
		/// Visit SingleType.
		/// </summary>
		object visit(SingleType e);

		/// <summary>
		/// Visit SequenceType.
		/// </summary>
		object visit(SequenceType e);

		/// <summary>
		/// Visit ItemType.
		/// </summary>
		object visit(ItemType e);

		/// <summary>
		/// Visit AnyKindTest.
		/// </summary>
		object visit(AnyKindTest e);

		/// <summary>
		/// Visit DocumentTest.
		/// </summary>
		object visit(DocumentTest e);

		/// <summary>
		/// Visit TextTest.
		/// </summary>
		object visit(TextTest e);

		/// <summary>
		/// Visit CommentTest.
		/// </summary>
		object visit(CommentTest e);

		/// <summary>
		/// Visit PITest.
		/// </summary>
		object visit(PITest e);

		/// <summary>
		/// Visit AttributeTest.
		/// </summary>
		object visit(AttributeTest e);

		/// <summary>
		/// Visit SchemaAttrTest.
		/// </summary>
		object visit(SchemaAttrTest e);

		/// <summary>
		/// Visit ElementTest.
		/// </summary>
		object visit(ElementTest e);

		/// <summary>
		/// Visit SchemElemTest.
		/// </summary>
		object visit(SchemaElemTest e);

		/// <summary>
		/// Visit AxisStep.
		/// </summary>
		object visit(AxisStep e);

		/// <summary>
		/// Visit FilterExpr.
		/// </summary>
		object visit(FilterExpr e);


        object visit(PostfixExpr fex);

	}

}