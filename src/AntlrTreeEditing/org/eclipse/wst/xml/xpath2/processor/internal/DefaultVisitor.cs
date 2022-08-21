using System.Collections;
using System.Collections.Generic;
using xpath.org.eclipse.wst.xml.xpath2.processor.@internal.ast;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2013 Jesper Steen Moeller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moller  - bug 338494 - But without dependency on magic constants 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{

	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
	using AddExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AddExpr;
	using AndExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AndExpr;
	using AnyKindTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AnyKindTest;
	using AttributeTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AttributeTest;
	using AxisStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AxisStep;
	using CastExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CastExpr;
	using CastableExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CastableExpr;
	using CmpExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CmpExpr;
	using CntxItemExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CntxItemExpr;
	using CommentTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.CommentTest;
	using DecimalLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DecimalLiteral;
	using DivExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DivExpr;
	using DocumentTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DocumentTest;
	using DoubleLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.DoubleLiteral;
	using ElementTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ElementTest;
	using ExceptExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ExceptExpr;
	using Expr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.Expr;
	using FilterExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.FilterExpr;
	using ForExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ForExpr;
	using ForwardStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ForwardStep;
	using FunctionCall = org.eclipse.wst.xml.xpath2.processor.@internal.ast.FunctionCall;
	using IDivExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IDivExpr;
	using IfExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IfExpr;
	using InstOfExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.InstOfExpr;
	using IntegerLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IntegerLiteral;
	using IntersectExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.IntersectExpr;
	using ItemType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ItemType;
	using MinusExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.MinusExpr;
	using ModExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ModExpr;
	using MulExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.MulExpr;
	using NameTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.NameTest;
	using OrExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.OrExpr;
	using PITest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PITest;
	using ParExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ParExpr;
	using PipeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PipeExpr;
	using PlusExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PlusExpr;
	using QuantifiedExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.QuantifiedExpr;
	using RangeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.RangeExpr;
	using ReverseStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ReverseStep;
	using SchemaAttrTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaAttrTest;
	using SchemaElemTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaElemTest;
	using SequenceType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SequenceType;
	using SingleType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SingleType;
	using StringLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StringLiteral;
	using SubExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SubExpr;
	using TextTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TextTest;
	using TreatAsExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TreatAsExpr;
	using UnionExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnionExpr;
	using VarExprPair = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarExprPair;
	using VarRef = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarRef;
	using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
	using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"unchecked", "deprecation"}) public class DefaultVisitor implements org.eclipse.wst.xml.xpath2.processor.internal.ast.XPathVisitor
	public class DefaultVisitor : XPathVisitor
	{

		/// <summary>
		/// Returns the normalized tree
		/// </summary>
		/// <param name="xp">
		///            is the xpath expression. </param>
		/// <returns> the xpath expressions. </returns>
		public virtual object visit(XPath xp)
		{
			for (IEnumerator<Expr> i = xp.iterator(); i.MoveNext();)
			{
				Expr e = (Expr) i.Current;
				e.accept(this);
			}
			return null;
		}

		/// 
		/// <param name="fex">
		///            is the For expression. </param>
		/// <returns> fex expression. </returns>
		public virtual object visit(ForExpr fex)
		{
			for (IEnumerator<VarExprPair> i = fex.iterator(); i.MoveNext();)
			{
				i.Current.expr().accept(this);
			}
			fex.expr().accept(this);
			return null;
		}

		/// 
		/// <param name="qex">
		///            is the Quantified expression. </param>
		/// <returns> qex expression. </returns>
		public virtual object visit(QuantifiedExpr qex)
		{
			for (IEnumerator<VarExprPair> i = qex.iterator(); i.MoveNext();)
			{
				i.Current.expr().accept(this);
			}
			qex.expr().accept(this);
			return null;
		}

		/// 
		/// <param name="ifex">
		///            is the 'if' expression. </param>
		/// <returns> ifex expression. </returns>
		public virtual object visit(IfExpr ifex)
		{
			for (IEnumerator<Expr> i = ifex.iterator(); i.MoveNext();)
			{
				i.Current.accept(this);
			}
			ifex.then_clause().accept(this);
			ifex.else_clause().accept(this);
			return ifex;
		}

		/// <param name="ex">
		///            is the 'or' expression. </param>
		/// <returns> make logic expr(orex). </returns>
		public virtual object visit(OrExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the 'and' expression. </param>
		/// <returns> make logic expr(andex). </returns>
		public virtual object visit(AndExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="cmpex">
		///            is the compare expression. </param>
		/// <returns> cmpex. </returns>
		public virtual object visit(CmpExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="rex">
		///            is the range expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(RangeExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the add expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(AddExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the sub expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(SubExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="mulex">
		///            is the multiply expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(MulExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the division expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(DivExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the integer division expression that always returns an
		///            integer. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(IDivExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the mod expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(ModExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the union expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(UnionExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the pipe expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(PipeExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the intersect expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(IntersectExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ex">
		///            is the except expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(ExceptExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="ioexp">
		///            is the instance of expression. </param>
		/// <returns> a ioexp. </returns>
		public virtual object visit(InstOfExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="taexp">
		///            is the treat as expression. </param>
		/// <returns> a taexp. </returns>
		public virtual object visit(TreatAsExpr ex)
		{
			ex.left().accept(this);
			ex.right().accept(this);
			return null;
		}

		/// <param name="cexp">
		///            is the castable expression. </param>
		/// <returns> cexp. </returns>
		public virtual object visit(CastableExpr cexp)
		{
			cexp.left().accept(this);
			cexp.right().accept(this);
			return null;
		}

		/// <param name="cexp">
		///            is the cast expression. </param>
		/// <returns> cexp. </returns>
		public virtual object visit(CastExpr cexp)
		{
			cexp.left().accept(this);
			cexp.right().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the minus expression. </param>
		/// <returns> new sub expression </returns>
		public virtual object visit(MinusExpr e)
		{
			e.arg().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the plus expression. </param>
		/// <returns> new add expression </returns>
		public virtual object visit(PlusExpr e)
		{
			e.arg().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the xpath expression. </param>
		/// <returns> result. </returns>
		public virtual object visit(XPathExpr e)
		{
			e.expr().accept(this);
			e.next().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the forward step. </param>
		/// <returns> e </returns>
		public virtual object visit(ForwardStep e)
		{
			e.node_test().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the reverse step. </param>
		/// <returns> e </returns>
		public virtual object visit(ReverseStep e)
		{
			e.node_test().accept(this);
			return null;
		}

		/// <param name="e">
		///            is the Name test. </param>
		/// <returns> e </returns>
		public virtual object visit(NameTest e)
		{
			return null;
		}

		/// <param name="e">
		///            is the veriable reference. </param>
		/// <returns> e </returns>
		public virtual object visit(VarRef e)
		{
			return null;
		}

		/// <param name="e">
		///            is the string literal. </param>
		/// <returns> e </returns>
		public virtual object visit(StringLiteral e)
		{
			return null;
		}

		/// <param name="e">
		///            is the integer literal. </param>
		/// <returns> e </returns>
		public virtual object visit(IntegerLiteral e)
		{
			return null;
		}

		/// <param name="e">
		///            is the double literal. </param>
		/// <returns> e </returns>
		public virtual object visit(DoubleLiteral e)
		{
			return null;
		}

		/// <param name="e">
		///            is the decimal literal. </param>
		/// <returns> e </returns>
		public virtual object visit(DecimalLiteral e)
		{
			return null;
		}

		/// <param name="e">
		///            is the par expression. </param>
		/// <returns> e </returns>
		public virtual object visit(ParExpr e)
		{
			for (IEnumerator<Expr> i = e.iterator(); i.MoveNext();)
			{
				i.Current.accept(this);
			}
			return null;
		}

		/// <param name="e">
		///            is the Cntx Item Expression. </param>
		/// <returns> new function </returns>
		public virtual object visit(CntxItemExpr e)
		{
			return null;
		}

		/// <param name="e">
		///            is the fucntion call. </param>
		/// <returns> e </returns>
		public virtual object visit(FunctionCall e)
		{
			for (IEnumerator<Expr> i = e.iterator(); i.MoveNext();)
			{
				i.Current.accept(this);
			}
			return e;
		}

		/// <param name="e">
		///            is the single type. </param>
		/// <returns> e </returns>
		public virtual object visit(SingleType e)
		{
			return e;
		}

		/// <param name="e">
		///            is the sequence type. </param>
		/// <returns> e </returns>
		public virtual object visit(SequenceType e)
		{
			ItemType it = e.item_type();
			if (it != null)
			{
				it.accept(this);
			}
			return null;
		}

		/// <param name="e">
		///            is the item type. </param>
		/// <returns> e </returns>
		public virtual object visit(ItemType e)
		{

			switch (e.type())
			{
			case ItemType.ITEM:
				break;
			case ItemType.QNAME:
				break;

			case ItemType.KINDTEST:
				e.kind_test().accept(this);
				break;
			}
			return null;
		}

		/// <param name="e">
		///            is the any kind test. </param>
		/// <returns> e </returns>
		public virtual object visit(AnyKindTest e)
		{
			return null;
		}

		/// <param name="e">
		///            is the document test. </param>
		/// <returns> e </returns>
		public virtual object visit(DocumentTest e)
		{

			switch (e.type())
			{
			case DocumentTest.ELEMENT:
				e.elem_test().accept(this);
				break;

			case DocumentTest.SCHEMA_ELEMENT:
				e.schema_elem_test().accept(this);
				break;
			}
			return null;
		}

		/// <param name="e">
		///            is the text test. </param>
		/// <returns> e </returns>
		public virtual object visit(TextTest e)
		{
			return null;
		}

		/// <param name="e">
		///            is the common test. </param>
		/// <returns> e </returns>
		public virtual object visit(CommentTest e)
		{
			return null;
		}

		/// <param name="e">
		///            is the PI test. </param>
		/// <returns> e </returns>
		public virtual object visit(PITest e)
		{
			return null;
		}

		/// <param name="e">
		///            is the attribute test. </param>
		/// <returns> e </returns>
		public virtual object visit(AttributeTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the schema attribute test. </param>
		/// <returns> e </returns>
		public virtual object visit(SchemaAttrTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the element test. </param>
		/// <returns> e </returns>
		public virtual object visit(ElementTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the schema element test. </param>
		/// <returns> e </returns>
		public virtual object visit(SchemaElemTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the axis step. </param>
		/// <returns> e </returns>
		public virtual object visit(AxisStep e)
		{
			e.step().accept(this);
			for (var i = e.iterator(); i.MoveNext();)
            {
                ICollection<Expr> x = i.Current;
                foreach (Expr expr in x)
                    expr.accept(this);
			}
			return null;
		}

		/// <param name="e">
		///            is the filter expression. </param>
		/// <returns> e </returns>
		public virtual object visit(FilterExpr e)
		{
			e.primary().accept(this);
			for (IEnumerator<Expr> i = e.iterator(); i.MoveNext();)
			{
				i.Current.accept(this);
			}
			return e;
		}

        public object visit(PostfixExpr e)
        {
            e.primary().accept(this);
            for (var i = e.iterator(); i.MoveNext();)
            {
                ICollection<Expr> x = i.Current;
                foreach (Expr expr in x)
                    expr.accept(this);
            }
            return e;
        }
	}

}