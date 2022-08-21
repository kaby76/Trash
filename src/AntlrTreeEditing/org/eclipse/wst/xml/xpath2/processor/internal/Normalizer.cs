using System;
using System.Collections;
using System.Collections.Generic;
using xpath.org.eclipse.wst.xml.xpath2.processor.@internal.ast;

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

namespace org.eclipse.wst.xml.xpath2.processor.@internal
{


	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
	using FnFunctionLibrary = org.eclipse.wst.xml.xpath2.processor.function.FnFunctionLibrary;
	using AddExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AddExpr;
	using AndExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AndExpr;
	using AnyKindTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AnyKindTest;
	using AttributeTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AttributeTest;
	using AxisStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.AxisStep;
	using BinExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.BinExpr;
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
	using NodeTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.NodeTest;
	using OrExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.OrExpr;
	using PITest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PITest;
	using ParExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ParExpr;
	using PipeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PipeExpr;
	using PlusExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PlusExpr;
	using PrimaryExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.PrimaryExpr;
	using QuantifiedExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.QuantifiedExpr;
	using RangeExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.RangeExpr;
	using ReverseStep = org.eclipse.wst.xml.xpath2.processor.@internal.ast.ReverseStep;
	using SchemaAttrTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaAttrTest;
	using SchemaElemTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SchemaElemTest;
	using SequenceType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SequenceType;
	using SingleType = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SingleType;
	using Step = org.eclipse.wst.xml.xpath2.processor.@internal.ast.Step;
	using StepExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StepExpr;
	using StringLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StringLiteral;
	using SubExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SubExpr;
	using TextTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TextTest;
	using TreatAsExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TreatAsExpr;
	using UnExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnExpr;
	using UnionExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnionExpr;
	using VarExprPair = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarExprPair;
	using VarRef = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarRef;
	using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
	using XPathNode = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathNode;
	using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;
	using OpFunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.OpFunctionLibrary;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;

	/// <summary>
	/// Normalizer that uses XPathVisitor.
	/// </summary>
	// XXX currently not supported anymore!
	public class Normalizer : XPathVisitor
	{

		private StaticContext _sc;

		/// <summary>
		/// Static Context is set to sc
		/// </summary>
		/// <param name="sc">
		///            is the StaticContext. </param>
		public Normalizer(StaticContext sc)
		{
			_sc = sc;
		}

		/// <summary>
		/// Returns the normalized tree
		/// </summary>
		/// <param name="xp">
		///            is the xpath expression. </param>
		/// <returns> the xpath expressions. </returns>
		public virtual object visit(XPath xp)
        {
            var exprs = new List<Expr>();

			for (IEnumerator i = xp.GetEnumerator(); i.MoveNext();)
			{
				Expr e = (Expr) i.Current;

				Expr n = (Expr) e.accept(this);

				exprs.Add(n);
			}

			return new XPath(exprs);
		}

		private void printVarExprPairs(IEnumerator i)
		{
			while (i.MoveNext())
			{
				VarExprPair pair = (VarExprPair) i.Current;

				QName @var = pair.varname();
				Expr e = pair.expr();

				e.accept(this);
			}
		}

		// does a for and a quantified expression
		// takes the iterator for var expr paris
		private void doForExpr(IEnumerator iter, Expr expr)
		{
			var vars = new ArrayList();

			// go through expression and cache variables
			while (iter.MoveNext())
			{
				VarExprPair pair = (VarExprPair) iter.Current;

				QName @var = pair.varname();
				Expr e = pair.expr();

				// XXX this is wrong!
				// need to define new scope, and reference "inner scope"
				// [shadow outer vars]
				/*
				 * if(_sc.variable_exists(var)) report_error(new
				 * StaticNameError("Variable " + var.string() +
				 * " already defined"));
				 */
				// ok we can cheat here cuz we only care about variable
				// "presence" not its specific instance / value so we
				// can fakely shadow without creating explicit scopes
				// if variable already exists.... then leave it there...
				// [we do not need to create a new instance and delete
				// it at the end]
				// we only need to if variable does not exist
				// XXX: i fink this is all wrong
				vars.Add(@var);

				e.accept(this);
			}

			// add variables to scope
			for (IEnumerator i = vars.GetEnumerator(); i.MoveNext();)
			{
				QName @var = (QName) i.Current;
			}

			// do the bounded expression
			expr.accept(this);

			// remove variables
		}

		/// 
		/// <param name="fex">
		///            is the For expression. </param>
		/// <returns> fex expression. </returns>
		public virtual object visit(ForExpr fex)
		{
			ForExpr last = fex;
			Expr ret = fex.expr();
			int depth = 0;

			for (IEnumerator i = fex.iterator(); i.MoveNext();)
			{
				VarExprPair ve = (VarExprPair) i.Current;

				// ok we got nested fors...
				if (depth > 0)
				{
					var pairs = new List<VarExprPair>();
					pairs.Add(ve);

					ForExpr fe = new ForExpr(pairs, ret);
					last.set_expr(fe);

					last = fe;
				}

				depth++;
			}

			// normalize return value, and set it to the last for expr
			ret.accept(this);

			// get rid of the pairs in the parent (original) for
			if (depth > 1)
			{
				fex.truncate_pairs();
			}

			return fex;
		}

		/// 
		/// <param name="qex">
		///            is the Quantified expression. </param>
		/// <returns> qex expression. </returns>
		// XXX: code duplication
		public virtual object visit(QuantifiedExpr qex)
		{
			QuantifiedExpr last = qex;
			Expr ret = qex.expr();
			int depth = 0;

			for (IEnumerator i = qex.iterator(); i.MoveNext();)
			{
				VarExprPair ve = (VarExprPair) i.Current;

				// ok we got nested fors...
				if (depth > 0)
				{
					var pairs = new List<VarExprPair>();
					pairs.Add(ve);

					QuantifiedExpr qe = new QuantifiedExpr(qex.type(), pairs, ret);
					last.set_expr(qe);

					last = qe;
				}

				depth++;
			}

			// normalize return value, and set it to the last for expr
			ret.accept(this);

			// get rid of the pairs in the parent (original) for
			if (depth > 1)
			{
				qex.truncate_pairs();
			}

			return qex;

		}

		private void printExprs(IEnumerator i)
		{
			while (i.MoveNext())
			{
				Expr e = (Expr) i.Current;

				e.accept(this);
			}
		}

		/// 
		/// <param name="ifex">
		///            is the 'if' expression. </param>
		/// <returns> ifex expression. </returns>
		public virtual object visit(IfExpr ifex)
		{

			printExprs(ifex.iterator());

			ifex.then_clause().accept(this);

			ifex.else_clause().accept(this);

			return ifex;
		}

		/// <param name="name">
		///            of binary expression. </param>
		/// <param name="e">
		///            is the binary expression. </param>
		public virtual void printBinExpr(string name, BinExpr e)
		{
			e.left().accept(this);
			e.right().accept(this);
		}

		private BinExpr make_logic_expr(BinExpr e)
        {
            throw new Exception();
            //ICollection normalized = normalize_bin_args(e);

            //XPathNode[] nor_arr = new XPathNode[2];
            //int j = 0;

            //for (IEnumerator i = normalized.GetEnumerator(); i.MoveNext();)
            //{
            //	nor_arr[j] = (XPathNode) i.Current;
            //	j++;
            //}

            //var args = new List<XPathNode>();
            //args.Add(nor_arr[0]);
            //e.set_left(make_function(new QName("fn", "boolean", FnFunctionLibrary.XPATH_FUNCTIONS_NS), args));

            //args.Clear();
            //args.Add(nor_arr[1]);
            //e.set_right(make_function(new QName("fn", "boolean", FnFunctionLibrary.XPATH_FUNCTIONS_NS), args));

            //return e;
        }

		/// <param name="orex">
		///            is the 'or' expression. </param>
		/// <returns> make logic expr(orex). </returns>
		public virtual object visit(OrExpr orex)
		{
			return make_logic_expr(orex);
		}

		/// <param name="andex">
		///            is the 'and' expression. </param>
		/// <returns> make logic expr(andex). </returns>
		public virtual object visit(AndExpr andex)
		{
			return make_logic_expr(andex);
		}

		/// <param name="cmpex">
		///            is the compare expression. </param>
		/// <returns> cmpex. </returns>
		public virtual object visit(CmpExpr cmpex)
		{
			throw new Exception();
			//switch (cmpex.type())
			//{
			//case CmpExpr.EQ:
			//	return make_CmpOp(cmpex, new QName("fs", "eq", OpFunctionLibrary.XPATH_OP_NS));

			//case CmpExpr.NE:
			//	return make_CmpOp(cmpex, new QName("fs", "ne", OpFunctionLibrary.XPATH_OP_NS));

			//case CmpExpr.LT:
			//	return make_CmpOp(cmpex, new QName("fs", "lt", OpFunctionLibrary.XPATH_OP_NS));

			//case CmpExpr.GT:
			//	return make_CmpOp(cmpex, new QName("fs", "gt", OpFunctionLibrary.XPATH_OP_NS));

			//case CmpExpr.LE:
			//	return make_CmpOp(cmpex, new QName("fs", "le", OpFunctionLibrary.XPATH_OP_NS));

			//case CmpExpr.GE:
			//	return make_CmpOp(cmpex, new QName("fs", "ge", OpFunctionLibrary.XPATH_OP_NS));

			//	// XXX don't have functs!
			//case CmpExpr.IS:
			//	return make_function(new QName("op", "node-equal"), normalize_bin_args(cmpex));

			//case CmpExpr.LESS_LESS:
			//	return make_function(new QName("op", "node-before"), normalize_bin_args(cmpex));

			//case CmpExpr.GREATER_GREATER:
			//	return make_function(new QName("op", "node-after"), normalize_bin_args(cmpex));
			//}

			//printBinExpr("CMP" + cmpex.type(), cmpex);
			//return cmpex;
		}

		private ICollection normalize_bin_args(BinExpr e)
		{
			var args = new ArrayList();

			XPathNode left = (XPathNode) e.left().accept(this);
			XPathNode right = (XPathNode) e.right().accept(this);

			args.Add(left);
			args.Add(right);

			return args;
		}

		/// <param name="rex">
		///            is the range expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(RangeExpr rex)
		{
			throw new Exception();
			//ICollection args = normalize_bin_args(rex);
			//return make_function(new QName("op", "to", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		private XPathExpr make_xpathexpr(PrimaryExpr pex)
		{
			FilterExpr fe = new FilterExpr(pex, new List<Expr>());
			return new XPathExpr(0, false, fe);
		}

		private XPathExpr make_int_lit(int i)
		{
			IntegerLiteral il = new IntegerLiteral(new System.Numerics.BigInteger(i));
			return make_xpathexpr(il);
		}

		private XPathExpr make_string_lit(string s)
		{
			StringLiteral sl = new StringLiteral(s);
			return make_xpathexpr(sl);
		}

		private XPathExpr make_convert_operand(XPathExpr arg1, XPathExpr arg2)
		{
            throw new Exception();
			//var args = new ArrayList();
			//args.Add(arg1);
			//args.Add(arg2);

			//return make_function(new QName("fs", "convert-operand", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		private XPathExpr make_double_lit(double d)
		{
			DoubleLiteral dl = new DoubleLiteral(d);
			return make_xpathexpr(dl);
		}

		// fs:fname( fs:convert-operand( fn:data(ARG1), 1.0E0 ),
		// fs:convert-operand( fn:data(ARG2), 1.0E0 )
		// )
		private XPathExpr make_convert_binop(BinExpr e, XPathExpr convarg, QName name)
		{
			throw new Exception();
			//ICollection args = normalize_bin_args(e);
			//XPathExpr[] args_arr = new XPathExpr[2];
			//int j = 0;

			//for (IEnumerator i = args.GetEnumerator(); i.MoveNext();)
			//{
			//	args_arr[j] = (XPathExpr) i.Current;
			//	j++;
			//}

			//var argsfname = new ArrayList();
			//for (j = 0; j < 2; j++)
			//{
			//	XPathExpr arg = make_convert_operand(args_arr[j], convarg);
			//	argsfname.Add(arg);
			//}
			//return make_function(name, argsfname);
		}

		private XPathExpr make_ArithOp(BinExpr e, QName name)
		{
			return make_convert_binop(e, make_double_lit(1.0), name);
		}

		// fs:fname( fs:convert_operand( fn:data(ARG1), "string"),
		// fs:convert_operand( fn:data(ARG2), "string")
		// )
		private XPathExpr make_CmpOp(BinExpr e, QName name)
		{
			return make_convert_binop(e, make_string_lit("string"), name);
		}

		/// <param name="addex">
		///            is the add expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(AddExpr addex)
		{
			return make_ArithOp(addex, new QName("fs", "plus", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="subex">
		///            is the sub expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(SubExpr subex)
		{
			return make_ArithOp(subex, new QName("fs", "minus", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="mulex">
		///            is the multiply expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(MulExpr mulex)
		{
			return make_ArithOp(mulex, new QName("fs", "times", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="mulex">
		///            is the division expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(DivExpr mulex)
		{
			return make_ArithOp(mulex, new QName("fs", "div", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="mulex">
		///            is the integer division expression that always returns an
		///            integer. </param>
		/// <returns> a new function. </returns>
		// XXX: integer cast!
		public virtual object visit(IDivExpr mulex)
		{
			return make_ArithOp(mulex, new QName("fs", "idiv", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="mulex">
		///            is the mod expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(ModExpr mulex)
		{
			return make_ArithOp(mulex, new QName("fs", "mod", OpFunctionLibrary.XPATH_OP_NS));
		}

		/// <param name="unex">
		///            is the union expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(UnionExpr unex)
		{
            throw new Exception();
			//ICollection args = normalize_bin_args(unex);
			//return make_function(new QName("op", "union", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		/// <param name="pipex">
		///            is the pipe expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(PipeExpr pipex)
		{
            throw new Exception();
			//ICollection args = normalize_bin_args(pipex);
			//return make_function(new QName("op", "union", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		/// <param name="iexpr">
		///            is the intersect expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(IntersectExpr iexpr)
		{
            throw new Exception();
			//ICollection args = normalize_bin_args(iexpr);
			//return make_function(new QName("op", "intersect", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		/// <param name="eexpr">
		///            is the except expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(ExceptExpr eexpr)
		{
            throw new Exception();
			//ICollection args = normalize_bin_args(eexpr);
			//return make_function(new QName("op", "except", OpFunctionLibrary.XPATH_OP_NS), args);
		}

		/// <param name="ioexp">
		///            is the instance of expression. </param>
		/// <returns> a ioexp. </returns>
		public virtual object visit(InstOfExpr ioexp)
		{
			printBinExpr("INSTANCEOF", ioexp);
			return ioexp;
		}

		/// <param name="taexp">
		///            is the treat as expression. </param>
		/// <returns> a taexp. </returns>
		public virtual object visit(TreatAsExpr taexp)
		{
			printBinExpr("TREATAS", taexp);
			return taexp;
		}

		/// <param name="cexp">
		///            is the castable expression. </param>
		/// <returns> cexp. </returns>
		public virtual object visit(CastableExpr cexp)
		{
			printBinExpr("CASTABLE", cexp);
			return cexp;
		}

		/// <param name="cexp">
		///            is the cast expression. </param>
		/// <returns> cexp. </returns>
		public virtual object visit(CastExpr cexp)
		{
			printBinExpr("CAST", cexp);
			return cexp;
		}

		/// <param name="name">
		///            is the name. </param>
		/// <param name="e">
		///            is the Un Expression. </param>
		public virtual void printUnExpr(string name, UnExpr e)
		{
			e.arg().accept(this);

		}

		/// <param name="e">
		///            is the minus expression. </param>
		/// <returns> new sub expression </returns>
		public virtual object visit(MinusExpr e)
		{
			SubExpr se = new SubExpr(make_int_lit(0), e.arg());
			return se.accept(this);
		}

		/// <param name="e">
		///            is the plus expression. </param>
		/// <returns> new add expression </returns>
		public virtual object visit(PlusExpr e)
		{
			AddExpr ae = new AddExpr(make_int_lit(0), e.arg());

			return ae.accept(this);
		}

		private XPathExpr make_function(QName name, ICollection<Expr> args)
		{

			FunctionCall fc = new FunctionCall(name, args);
			FilterExpr fe = new FilterExpr(fc, new List<Expr>());
			return new XPathExpr(0, false, fe);

		}

		private XPathExpr make_root_self_node()
		{
            throw new Exception();

			// self::node()
			//Step self_node = new ForwardStep(ForwardStep.SELF, new AnyKindTest());
			//StepExpr self_node_expr = new AxisStep(self_node, new ArrayList());
			//XPathExpr self_node_xpath = new XPathExpr(0, self_node_expr);

			//// fn:root(self::node())
			//var args = new ArrayList();
			//args.Add(self_node_xpath);
			//XPathExpr xpe = make_function(new QName("fn", "root", FnFunctionLibrary.XPATH_FUNCTIONS_NS), args);

			//return xpe;
		}

		private XPathExpr make_descendant_or_self()
		{
            throw new Exception();
			//Step desc_self_node = new ForwardStep(ForwardStep.DESCENDANT_OR_SELF, new AnyKindTest());
			//StepExpr se = new AxisStep(desc_self_node, new ArrayList());

			//return new XPathExpr(0, se);
		}

		/// <param name="e">
		///            is the xpath expression. </param>
		/// <returns> result. </returns>
		public virtual object visit(XPathExpr e)
		{
			XPathExpr xp = e;
			int depth = 0; // indicates how many / we traversed
			XPathExpr result = e;

			while (xp != null)
			{
				int slashes = xp.slashes();
				StepExpr se = xp.expr();

				if (slashes == 1)
				{
					// this is a single slash and nothing else...
					if (se == null)
					{
						return make_root_self_node();
					}

					// /RelativePathExpr
					if (depth == 0)
					{
						XPathExpr xpe = make_root_self_node();
						xpe.set_next(e);

						result = xpe;
					}
				}

				if (slashes == 2)
				{
					// //RelativePathExpr
					if (depth == 0)
					{
						XPathExpr desc = make_descendant_or_self();
						desc.set_slashes(1);
						e.set_slashes(1);
						desc.set_next(e);

						XPathExpr root_self = make_root_self_node();
						root_self.set_next(desc);
						return root_self;
					}
				}

				if (se != null)
				{
					se.accept(this);
				}

				XPathExpr next = xp.next();

				// peek if the next guy will have 2 slashes...
				if (next != null)
				{
					// StepExpr//StepExpr
					if (next.slashes() == 2)
					{
						// create the node to stick between the
						// slashes
						XPathExpr desc = make_descendant_or_self();
						desc.set_slashes(1);

						// current node / desc / next
						xp.set_next(desc);
						desc.set_next(next);
						next.set_slashes(1);
					}
				}
				xp = next;
				depth++;
			}
			return result;
		}

		/// <param name="e">
		///            is the forward step. </param>
		/// <returns> e </returns>
		// XXX: normalzie!
		public virtual object visit(ForwardStep e)
		{
			var axis = e.axis();

			switch (axis)
			{
			case ForwardStep.Type.AT_SYM:
				e.set_axis(ForwardStep.Type.ATTRIBUTE);
				break;

			case ForwardStep.Type.NONE:
				e.set_axis(ForwardStep.Type.CHILD);
				break;

			}

			e.node_test().accept(this);

			return e;
		}

		/// <param name="e">
		///            is the reverse step. </param>
		/// <returns> e </returns>
		public virtual object visit(ReverseStep e)
		{
            throw new Exception();

			//if (e.axis() == ReverseStep.DOTDOT)
			//{
			//	NodeTest nt = new AnyKindTest();
			//	Step s = new ReverseStep(ReverseStep.PARENT, nt);

			//	return s;
			//}

			//NodeTest nt = e.node_test();
			//if (nt != null)
			//{
			//	nt.accept(this);
			//}

			//return e;
		}

		/// <param name="e">
		///            is the Name test. </param>
		/// <returns> e </returns>
		public virtual object visit(NameTest e)
		{

			string prefix = e.name().prefix();

			// XXX: is this correct ?
			// i.e. if there is no prefix... its ok.. else it must exist
			if (string.ReferenceEquals(prefix, null))
			{
				return null;
			}

			return e;
		}

		/// <param name="e">
		///            is the veriable reference. </param>
		/// <returns> e </returns>
		public virtual object visit(VarRef e)
		{
			return e;
		}

		/// <param name="e">
		///            is the string literal. </param>
		/// <returns> e </returns>
		public virtual object visit(StringLiteral e)
		{
			return e;
		}

		/// <param name="e">
		///            is the integer literal. </param>
		/// <returns> e </returns>
		public virtual object visit(IntegerLiteral e)
		{
			return e;
		}

		/// <param name="e">
		///            is the double literal. </param>
		/// <returns> e </returns>
		public virtual object visit(DoubleLiteral e)
		{
			return e;
		}

		/// <param name="e">
		///            is the decimal literal. </param>
		/// <returns> e </returns>
		public virtual object visit(DecimalLiteral e)
		{
			return e;
		}

		/// <param name="e">
		///            is the par expression. </param>
		/// <returns> e </returns>
		public virtual object visit(ParExpr e)
		{
			printExprs(e.GetEnumerator());
			return e;
		}

		/// <param name="e">
		///            is the Cntx Item Expression. </param>
		/// <returns> new function </returns>
		public virtual object visit(CntxItemExpr e)
		{
			return new VarRef(new QName("fs", "dot"));
		}

		/// <param name="e">
		///            is the fucntion call. </param>
		/// <returns> e </returns>
		// XXX: how do we normalize ?
		public virtual object visit(FunctionCall e)
		{

			printExprs(e.GetEnumerator());
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

			return e;
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

			return e;
		}

		/// <param name="e">
		///            is the any kind test. </param>
		/// <returns> e </returns>
		public virtual object visit(AnyKindTest e)
		{
			return e;
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
			return e;
		}

		/// <param name="e">
		///            is the text test. </param>
		/// <returns> e </returns>
		public virtual object visit(TextTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the common test. </param>
		/// <returns> e </returns>
		public virtual object visit(CommentTest e)
		{
			return e;
		}

		/// <param name="e">
		///            is the PI test. </param>
		/// <returns> e </returns>
		public virtual object visit(PITest e)
		{
			string arg = e.arg();
			if (string.ReferenceEquals(arg, null))
			{
				arg = "";
			}

			return e;
		}

		/// <param name="e">
		///            is the attribute test. </param>
		/// <returns> e </returns>
		// XXX NO CHECK ?
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
		// XXX NO SEMANTIC CHECK?!
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

		private void printCollExprs(IEnumerator i)
		{
			while (i.MoveNext())
			{
				ICollection exprs = (ICollection) i.Current;

				printExprs(exprs.GetEnumerator());
			}
		}

		/// <param name="e">
		///            is the axis step. </param>
		/// <returns> e </returns>
		public virtual object visit(AxisStep e)
		{

			Step s = (Step) e.step().accept(this);
			e.set_step(s);

			printCollExprs(e.GetEnumerator());
			return e;
		}

		/// <param name="e">
		///            is the filter expression. </param>
		/// <returns> e </returns>
		public virtual object visit(FilterExpr e)
		{
			PrimaryExpr pe = (PrimaryExpr) e.primary().accept(this);
			e.set_primary(pe);

			printCollExprs(e.GetEnumerator());
			return e;
		}

        public object visit(PostfixExpr fex)
        {
            throw new NotImplementedException();
        }
    }

}