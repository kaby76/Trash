using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using org.w3c.dom;
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
///     Mukul Gandhi - bug 274805 - improvements to xs:integer data type 
///     Jesper Steen Moeller - bug 285145 - check arguments to op:to
///     Jesper Steen Moeller - bug 262765 - fixed node state iteration
///     Jesper Steen Moller  - bug 275610 - Avoid big time and memory overhead for externals
///     Jesper Steen Moller  - bug 280555 - Add pluggable collation support
///     Jesper Steen Moller  - bug 281938 - undefined context should raise error
///     Jesper Steen Moller  - bug 262765 - use correct 'effective boolean value'
///     Jesper Steen Moller  - bug 312191 - instance of test fails with partial matches
///    Mukul Gandhi         - bug 325262 - providing ability to store an XPath2 sequence into
///                                         an user-defined variable.
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
///     Jesper Steen Moller - bug 343804 - Updated API information
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using StaticContext = org.eclipse.wst.xml.xpath2.api.StaticContext;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using TypeModel = org.eclipse.wst.xml.xpath2.api.typesystem.TypeModel;
	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
	using Axis = org.eclipse.wst.xml.xpath2.processor.@internal.Axis;
	using DescendantOrSelfAxis = org.eclipse.wst.xml.xpath2.processor.@internal.DescendantOrSelfAxis;
	using DynamicContextAdapter = org.eclipse.wst.xml.xpath2.processor.@internal.DynamicContextAdapter;
	using Focus = org.eclipse.wst.xml.xpath2.processor.@internal.Focus;
	using ForwardAxis = org.eclipse.wst.xml.xpath2.processor.@internal.ForwardAxis;
	using ParentAxis = org.eclipse.wst.xml.xpath2.processor.@internal.ParentAxis;
	using ReverseAxis = org.eclipse.wst.xml.xpath2.processor.@internal.ReverseAxis;
	using SelfAxis = org.eclipse.wst.xml.xpath2.processor.@internal.SelfAxis;
	using SeqType = org.eclipse.wst.xml.xpath2.processor.@internal.SeqType;
	using StaticContextAdapter = org.eclipse.wst.xml.xpath2.processor.@internal.StaticContextAdapter;
	using StaticNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticNameError;
	using StaticTypeNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticTypeNameError;
	using TypeError = org.eclipse.wst.xml.xpath2.processor.@internal.TypeError;
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
	using StepExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StepExpr;
	using StringLiteral = org.eclipse.wst.xml.xpath2.processor.@internal.ast.StringLiteral;
	using SubExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.SubExpr;
	using TextTest = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TextTest;
	using TreatAsExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.TreatAsExpr;
	using UnionExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnionExpr;
	using VarExprPair = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarExprPair;
	using VarRef = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarRef;
	using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
	using XPathNode = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathNode;
	using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;
	using ConstructorFL = org.eclipse.wst.xml.xpath2.processor.@internal.function.ConstructorFL;
	using FnBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnBoolean;
	using FnData = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnData;
	using FnRoot = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnRoot;
	using FsDiv = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsDiv;
	using FsEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsEq;
	using FsGe = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsGe;
	using FsGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsGt;
	using FsIDiv = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsIDiv;
	using FsLe = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsLe;
	using FsLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsLt;
	using FsMinus = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsMinus;
	using FsMod = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsMod;
	using FsNe = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsNe;
	using FsPlus = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsPlus;
	using FsTimes = org.eclipse.wst.xml.xpath2.processor.@internal.function.FsTimes;
	using FunctionLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.function.FunctionLibrary;
	using OpExcept = org.eclipse.wst.xml.xpath2.processor.@internal.function.OpExcept;
	using OpIntersect = org.eclipse.wst.xml.xpath2.processor.@internal.function.OpIntersect;
	using OpTo = org.eclipse.wst.xml.xpath2.processor.@internal.function.OpTo;
	using OpUnion = org.eclipse.wst.xml.xpath2.processor.@internal.function.OpUnion;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using AttrType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AttrType;
	using CommentType = org.eclipse.wst.xml.xpath2.processor.@internal.types.CommentType;
	using DocType = org.eclipse.wst.xml.xpath2.processor.@internal.types.DocType;
	using ElementType = org.eclipse.wst.xml.xpath2.processor.@internal.types.ElementType;
	using NodeType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NodeType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using PIType = org.eclipse.wst.xml.xpath2.processor.@internal.types.PIType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using TextType = org.eclipse.wst.xml.xpath2.processor.@internal.types.TextType;
	using XSBoolean = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSBoolean;
	using XSInteger = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSInteger;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using ResultSequenceUtil = org.eclipse.wst.xml.xpath2.processor.util.ResultSequenceUtil;
	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// Default evaluator interface
	/// </summary>
	public class DefaultEvaluator : XPathVisitor, Evaluator
	{

		private const string XML_SCHEMA_NS = "http://www.w3.org/2001/XMLSchema";

		private static readonly QName ANY_ATOMIC_TYPE = new QName("xs", "anyAtomicType", XML_SCHEMA_NS);

		private org.eclipse.wst.xml.xpath2.api.DynamicContext _dc;

		// this is a parameter that may be set on a call...
		// the parameter may become invalid on the next call... i.e. the
		// previous parameter is not saved... so use with care! [remember...
		// this thing is highly recursive]
		private object _param;
		private EvaluationContext _ec;

		private api.StaticContext _sc;

		private Focus _focus = new Focus(ResultBuffer.EMPTY);

		internal virtual Focus focus()
		{
			return _focus;
		}

		internal virtual void set_focus(Focus f)
		{
			_focus = f;
		}

		internal class Pair
		{
			public object _one;
			public object _two;

			public Pair(object o, object t)
			{
				_one = o;
				_two = t;
			}
		}

		private void popScope()
		{
			if (_innerScope == null)
			{
				throw new System.InvalidOperationException("Unmatched scope pop");
			}
			_innerScope = _innerScope.nextScope;
		}

		private void pushScope(QName @var, api.ResultSequence value)
		{
			_innerScope = new VariableScope(this, @var, value, _innerScope);
		}

		private bool derivesFrom(NodeType at, QName et)
		{
			TypeDefinition td = _sc.TypeModel.getType(at.node_value());

			short method = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_EXTENSION | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_RESTRICTION;
			return td != null && td.derivedFrom(et.@namespace(), et.local(), method);
		}

		private bool derivesFrom(NodeType at, TypeDefinition td)
		{
			TypeDefinition nodeTd = _sc.TypeModel.getType(at.node_value());
			short method = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_EXTENSION | org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition_Fields.DERIVATION_RESTRICTION;
			return nodeTd != null && nodeTd.derivedFromType(td, method);
		}

		public DefaultEvaluator(org.eclipse.wst.xml.xpath2.processor.DynamicContext dynamicContext, Document doc) : this(new StaticContextAdapter(dynamicContext), new DynamicContextAdapter(dynamicContext))
		{

			api.ResultSequence focusSequence = (doc != null) ? new DocType(doc, _sc.TypeModel) : ResultBuffer.EMPTY;
			set_focus(new Focus(focusSequence));
			dynamicContext.set_focus(focus());
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public DefaultEvaluator(api.StaticContext staticContext, api.DynamicContext dynamicContext, object[] contextItems) : this(staticContext, dynamicContext)
		{

			// initialize context item with root of document
			ResultBuffer rs = new ResultBuffer();
			foreach (object obj in contextItems)
			{
				if (obj is Node)
				{
					rs.add(NodeType.dom_to_xpath((Node)obj, _sc.TypeModel));
				}
				else throw new Exception("I have no clue what context you are passing here.");
			}
			set_focus(new Focus(rs.Sequence));
			_param = null;
		}

		private DefaultEvaluator(api.StaticContext staticContext, api.DynamicContext dynamicContext)
		{
			_sc = staticContext;
			_dc = dynamicContext;
			_ec = new EvaluationContextAnonymousInnerClass(this);
		}

		private class EvaluationContextAnonymousInnerClass : EvaluationContext
		{
			private readonly DefaultEvaluator outerInstance;

			public EvaluationContextAnonymousInnerClass(DefaultEvaluator outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			public virtual org.eclipse.wst.xml.xpath2.api.DynamicContext DynamicContext
			{
				get
				{
					return outerInstance._dc;
				}
			}

			public virtual Item ContextItem
			{
				get
				{
					return outerInstance._focus.context_item();
				}
			}

			public virtual int ContextPosition
			{
				get
				{
					return outerInstance._focus.position();
				}
			}

			public virtual int LastPosition
			{
				get
				{
					return outerInstance._focus.last();
				}
			}

			public virtual api.StaticContext StaticContext
			{
				get
				{
					return outerInstance._sc;
				}
			}
		}

		internal class VariableScope
		{
			private readonly DefaultEvaluator outerInstance;

			public VariableScope(DefaultEvaluator outerInstance, QName name, api.ResultSequence value, VariableScope nextScope)
			{
				this.outerInstance = outerInstance;
				this.name = name;
				this.value = value;
				this.nextScope = nextScope;
			}
			public readonly QName name;
			public readonly api.ResultSequence value;
			public readonly VariableScope nextScope;
		}

		private VariableScope _innerScope = null;

		private api.ResultSequence getVariable(QName name)
		{
			// First, try local scopes
			VariableScope scope = _innerScope;
			while (scope != null)
			{
				if (name.Equals(scope.name))
				{
					return scope.value;
				}
				scope = scope.nextScope;
			}
			return (api.ResultSequence) _dc.getVariable(name.asQName());
		}

		// XXX this kinda sux
		// the problem is that visistor interface does not throw exceptions...
		// so we get around it ;D
		private void report_error(DynamicError err)
		{
			throw err;
		}

		private void report_error(TypeError err)
		{
			throw new DynamicError(err);
		}

		private void report_error(StaticNameError err)
		{
			throw err;
		}

		private AnyAtomicType makeAtomic(QName name)
		{
			FunctionLibrary fl = (FunctionLibrary) _sc.FunctionLibraries[name.@namespace()];
			if (fl is ConstructorFL)
			{
				ConstructorFL cfl = (ConstructorFL)fl;
				return cfl.atomic_type(name);
			}
			return null;
		}

		/// <summary>
		/// evaluate the xpath node
		/// </summary>
		/// <param name="node">
		///            is the xpath node. </param>
		/// <exception cref="dynamic">
		///             error. </exception>
		/// <returns> result sequence. </returns>
		public virtual org.eclipse.wst.xml.xpath2.processor.ResultSequence evaluate(XPathNode node)
		{
			return ResultSequenceUtil.newToOld(evaluate2(node));
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual api.ResultSequence evaluate2(XPathNode node)
		{
			return (api.ResultSequence) node.accept(this);
		}

		// basically the comma operator...
		private api.ResultSequence do_expr(IEnumerator i)
        {
            api.ResultSequence rs = null;
			ResultBuffer buffer = null;

			while (i.MoveNext())
			{
				Expr e = (Expr) i.Current;

                api.ResultSequence result = (api.ResultSequence) e.accept(this);

				if (rs == null && buffer == null)
				{
					rs = result;
				}
				else
				{
					if (buffer == null)
					{
						buffer = new ResultBuffer();
						buffer.concat(rs);
						rs = null;
					}
					buffer.concat(result);
				}
			}

			if (buffer != null)
			{
				return buffer.Sequence;
			}
			else if (rs != null)
			{
				return rs;
			}
			else
			{
				return ResultBuffer.EMPTY;
			}
		}

		/// <summary>
		/// iterate through xpath expression
		/// </summary>
		/// <param name="xp">
		///            is the xpath. </param>
		/// <returns> result sequence. </returns>
		public virtual object visit(XPath xp)
		{
			ResultBuffer rs = new ResultBuffer();
            ArrayList results = new ArrayList();
			int type = 0; // 0: don't know yet
            // 1: atomic
            // 2: node
            Focus xfocus = focus();
            int original_pos = xfocus.position();

            // execute step for all items in focus
            while (true)
            {
				int current_pos = xfocus.position();
                api.ResultSequence one_rs = do_expr(xp.GetEnumerator());
				foreach (var c in one_rs)
                    results.Add(c);

                // go to next
                if (!xfocus.advance_cp())
                {
                    break;
                }
            }

            // make sure we didn't change focus
            xfocus.set_position(original_pos);
            bool node_types = false;

            // check the results
            for (IEnumerator i = results.GetEnumerator(); i.MoveNext();)
            {
                api.ResultSequence result = (api.ResultSequence)i.Current;

                // make sure results are of same type, and add them in
                for (var j = result.iterator(); j.MoveNext();)
                {
                    AnyType item = (AnyType)j.Current;

                    // first item
                    if (type == 0)
                    {
                        if (item is AnyAtomicType)
                        {
                            type = 1;
                        }
                        else if (item is NodeType)
                        {
                            type = 2;
                        }
                        else
                        {
                            Debug.Assert(false);
                        }

                    }

                    // make sure we got coherent types
                    switch (type)
                    {
                        // atomic... just concat
                        case 1:
                            if (!(item is AnyAtomicType))
                            {
                                report_error(TypeError.mixed_vals(null));
                            }
                            rs.add(item);
                            break;

                        case 2:
                            node_types = true;
                            if (!(item is NodeType))
                            {
                                report_error(TypeError.mixed_vals(null));
                            }
                            rs.add(item);
                            break;

                        default:
                            Debug.Assert(false);
                            break;
                    }
                }
            }
            // XXX lame
            if (node_types)
            {
                rs = NodeType.linarize(rs);
            }
            return rs.Sequence;
		}

		private void do_for_each(IEnumerator iter, Expr finalexpr, ResultBuffer destination)
		{
			// we have more vars to bind...
			if (iter.MoveNext())
			{
				VarExprPair ve = (VarExprPair) iter.Current;

				// evaluate binding sequence
                api.ResultSequence rs = (api.ResultSequence) ve.expr().accept(this);

				// XXX
				if (rs.empty())
				{
					//iter.previous();
					return;
				}

				QName varname = ve.varname();

				// for each item of binding sequence, bind the range
				// variable and do the expression, concatenating the
				// result

				for (IEnumerator i = rs.iterator(); i.MoveNext();)
				{
					AnyType item = (AnyType) i.Current;

					pushScope(varname, item);
					do_for_each(iter, finalexpr, destination);
					popScope();
				}
				//iter.previous();
			}
			// we finally got to do the "last expression"
			else
			{
				destination.concat((api.ResultSequence) finalexpr.accept(this));
			}
		}


		// XXX ugly
		// type: 0 = for [return == "correct"]
		// 1 = for all [return false, return empty on true]
		// 2 = there exists [return true, return empty on false]
		private XSBoolean do_for_all(IEnumerator iter, Expr finalexpr)
		{

			// we have more vars to bind...
			if (iter.MoveNext())
			{
				VarExprPair ve = (VarExprPair) iter.Current;

				// evaluate binding sequence
                api.ResultSequence rs = (api.ResultSequence) ve.expr().accept(this);

				QName varname = ve.varname();

				// for each item of binding sequence, bind the range
				// variable and check the predicate

				try
				{
					for (IEnumerator i = rs.iterator(); i.MoveNext();)
					{
						AnyType item = (AnyType) i.Current;

						pushScope(varname, item);
						XSBoolean effbool = do_for_all(iter, finalexpr);
						popScope();

						// ok here we got a "real" result, now figure
						// out what to do with it
						if (!effbool.value())
						{
							return XSBoolean.FALSE;
						}
					}
				}
				finally
				{
					//iter.previous();
				}
				return XSBoolean.TRUE;
			}
			// we finally got to do the "last expression"
			else
			{
				return effective_boolean_value((api.ResultSequence) finalexpr.accept(this));
			}

		}

		private XSBoolean do_exists(IEnumerator iter, Expr finalexpr)
		{

			// we have more vars to bind...
			if (iter.MoveNext())
			{
				VarExprPair ve = (VarExprPair) iter.Current;

				// evaluate binding sequence
                api.ResultSequence rs = (api.ResultSequence) ve.expr().accept(this);

				QName varname = ve.varname();

				// for each item of binding sequence, bind the range
				// variable and check the expression

				try
				{
					for (IEnumerator i = rs.iterator(); i.MoveNext();)
					{
						AnyType item = (AnyType) i.Current;

						pushScope(varname, item);
						XSBoolean effbool = do_exists(iter, finalexpr);
						popScope();

						// ok here we got a "real" result, now figure
						// out what to do with it
						if (effbool.value())
						{
							return XSBoolean.TRUE;
						}
					}
				}
				finally
				{
					//iter.previous();
				}

				// since none in this sequence evaluated to true, return false
				return XSBoolean.FALSE;
			}
			// we finally got to do the "last expression"
			else
			{
				return effective_boolean_value((api.ResultSequence) finalexpr.accept(this));
			}

		}

		/// <summary>
		/// visit for expression
		/// </summary>
		/// <param name="fex">
		///            is the for expression. </param>
		/// <returns> a new function. </returns>
		public virtual object visit(ForExpr fex)
		{
			// XXX
			IList pairs = new ArrayList(fex.ve_pairs().ToList());
			ResultBuffer rb = new ResultBuffer();
			do_for_each(pairs.GetEnumerator(), fex.expr(), rb);
			return rb.Sequence;
		}

		/// <summary>
		/// visit quantified expression
		/// </summary>
		/// <param name="qex">
		///            is the quantified expression. </param>
		/// <returns> a new function or null. </returns>
		public virtual object visit(QuantifiedExpr qex)
		{
			IList pairs = new ArrayList(qex.ve_pairs().ToList());

			switch (qex.type())
			{
			case QuantifiedExpr.SOME:
				return do_exists(pairs.GetEnumerator(), qex.expr());
			case QuantifiedExpr.ALL:
				return do_for_all(pairs.GetEnumerator(), qex.expr());

			default:
				Debug.Assert(false);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit if expression
		/// </summary>
		/// <param name="ifex">
		///            is the if expression. </param>
		/// <returns> a ifex.then_clause().accept(this). </returns>
		public virtual object visit(IfExpr ifex)
		{
            api.ResultSequence test_res = do_expr(ifex.iterator());

			XSBoolean res = effective_boolean_value(test_res);

			if (res.value())
			{
				return ifex.then_clause().accept(this);
			}
			else
			{
				return ifex.else_clause().accept(this);
			}
		}

		private bool[] do_logic_exp(BinExpr e)
		{
			ICollection args = do_bin_args(e);

			IEnumerator argiter = args.GetEnumerator();
            argiter.MoveNext();
            api.ResultSequence one = (api.ResultSequence) argiter.Current;
            argiter.MoveNext();
            api.ResultSequence two = (api.ResultSequence) argiter.Current;

			bool oneb = effective_boolean_value(one).value();
			bool twob = effective_boolean_value(two).value();

			bool[] res = new bool[] {oneb, twob};
			return res;
		}

		/// <summary>
		/// visit or expression
		/// </summary>
		/// <param name="orex">
		///            is the or expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(OrExpr orex)
		{
			bool[] res = do_logic_exp(orex);

			return ResultSequenceFactory.create_new(new XSBoolean(res[0] || res[1]));
		}

		/// <summary>
		/// visit and expression
		/// </summary>
		/// <param name="andex">
		///            is the and expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(AndExpr andex)
		{
			bool[] res = do_logic_exp(andex);

			return ResultSequenceFactory.create_new(new XSBoolean(res[0] && res[1]));
		}

		private api.ResultSequence node_cmp(CmpExpr.Type type, ICollection args)
		{
			Debug.Assert(args.Count == 2);

			IEnumerator argsiter = args.GetEnumerator();

            argsiter.MoveNext();
            api.ResultSequence one = (api.ResultSequence) argsiter.Current;
            argsiter.MoveNext();
            api.ResultSequence two = (api.ResultSequence) argsiter.Current;

			int size_one = one.size();
			int size_two = two.size();

			if (size_one > 1 || size_two > 1)
			{
				report_error(TypeError.invalid_type(null));
			}

			if (size_one == 0 || size_two == 0)
			{
				return ResultBuffer.EMPTY;
			}

			Item at_one = one.item(0);
			Item at_two = two.item(0);

			if (!(at_one is NodeType) || !(at_two is NodeType))
			{
				report_error(TypeError.invalid_type(null));
			}

			// ok we got the args finally
			NodeType nt_one = (NodeType) at_one;
			NodeType nt_two = (NodeType) at_two;

			bool answer = false; // we are pessimistic as usual

			// do comparison
			switch (type)
			{
			case CmpExpr.Type.IS:
				answer = nt_one.node_value() == nt_two.node_value();
				break;

			case CmpExpr.Type.LESS_LESS:
				answer = nt_one.before(nt_two);
				break;

			case CmpExpr.Type.GREATER_GREATER:
				answer = nt_one.after(nt_two);
				break;

			default:
				Debug.Assert(false);
			break;
			}

			return XSBoolean.valueOf(answer);
		}

		/// <summary>
		/// visit compare expression
		/// </summary>
		/// <param name="cmpex">
		///            is the compare expression. </param>
		/// <returns> a new function or null </returns>
		public virtual object visit(CmpExpr cmpex)
		{
			try
			{
				ICollection args = do_bin_args(cmpex);

				switch (cmpex.type())
				{
				case CmpExpr.Type.EQ:
					return FsEq.fs_eq_value(args, _dc);

				case CmpExpr.Type.NE:
					return FsNe.fs_ne_value(args, _dc);

				case CmpExpr.Type.GT:
					return FsGt.fs_gt_value(args, _dc);

				case CmpExpr.Type.LT:
					return FsLt.fs_lt_value(args, _dc);

				case CmpExpr.Type.GE:
					return FsGe.fs_ge_value(args, _dc);

				case CmpExpr.Type.LE:
					return FsLe.fs_le_value(args, _dc);

				case CmpExpr.Type.EQUALS:
					return FsEq.fs_eq_general(args, _dc);

				case CmpExpr.Type.NOTEQUALS:
					return FsNe.fs_ne_general(args, _dc);

				case CmpExpr.Type.GREATER:
					return FsGt.fs_gt_general(args, _dc);

				case CmpExpr.Type.LESSTHAN:
					return FsLt.fs_lt_general(args, _dc);

				case CmpExpr.Type.GREATEREQUAL:
					return FsGe.fs_ge_general(args, _dc);

				case CmpExpr.Type.LESSEQUAL:
					return FsLe.fs_le_general(args, _dc);

				case CmpExpr.Type.IS:
				case CmpExpr.Type.LESS_LESS:
				case CmpExpr.Type.GREATER_GREATER:
					return node_cmp(cmpex.type(), args);

				default:
					Debug.Assert(false);
				break;
				}
			}
			catch (DynamicError err)
			{
				report_error(err);
			}
			return null; // unreach
		}

		/// <summary>
		/// visit range expression
		/// </summary>
		/// <param name="rex">
		///            is the range expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(RangeExpr rex)
		{
            api.ResultSequence one = (api.ResultSequence) rex.left().accept(this);
            api.ResultSequence two = (api.ResultSequence) rex.right().accept(this);
			if (one.empty() || two.empty())
			{
				return ResultSequenceFactory.create_new();
			}
			var args = new ArrayList();
			args.Add(one);
			args.Add(two);

			try
			{
				return OpTo.op_to(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		private XSBoolean effective_boolean_value(api.ResultSequence rs)
		{
			try
			{
				return FnBoolean.fn_boolean(rs);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}


		/// <summary>
		/// visit and expression
		/// </summary>
		/// <param name="addex">
		///            is the and expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(AddExpr addex)
		{
			try
			{
				ICollection args = do_bin_args(addex);
				return FsPlus.fs_plus(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit sub expression
		/// </summary>
		/// <param name="subex">
		///            is the sub expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(SubExpr subex)
		{
			try
			{
				ICollection args = do_bin_args(subex);
				return FsMinus.fs_minus(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit multiply expression
		/// </summary>
		/// <param name="mulex">
		///            is the mul expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(MulExpr mulex)
		{
			try
			{
				ICollection args = do_bin_args(mulex);
				return FsTimes.fs_times(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit division expression
		/// </summary>
		/// <param name="mulex">
		///            is the division expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(DivExpr mulex)
		{
			try
			{
				ICollection args = do_bin_args(mulex);
				return FsDiv.fs_div(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit integer division expression
		/// </summary>
		/// <param name="mulex">
		///            is the integer division expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(IDivExpr mulex)
		{
			try
			{
				ICollection args = do_bin_args(mulex);
				return FsIDiv.fs_idiv(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit modular expression
		/// </summary>
		/// <param name="mulex">
		///            is the modular expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(ModExpr mulex)
		{
			try
			{
				ICollection args = do_bin_args(mulex);
				return FsMod.fs_mod(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		private ICollection do_bin_args(BinExpr e)
		{
            api.ResultSequence one = (api.ResultSequence) e.left().accept(this);
            api.ResultSequence two = (api.ResultSequence) e.right().accept(this);

			var args = new ArrayList();
			args.Add(one);
			args.Add(two);

			return args;
		}

		/// <summary>
		/// visit union expression
		/// </summary>
		/// <param name="unex">
		///            is the union expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(UnionExpr unex)
		{
			try
			{
				ICollection args = do_bin_args(unex);
				return OpUnion.op_union(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit pipe expression
		/// </summary>
		/// <param name="pipex">
		///            is the pipe expression. </param>
		/// <returns> a new function </returns>
		// XXX same as above
		public virtual object visit(PipeExpr pipex)
		{
			try
			{
				ICollection args = do_bin_args(pipex);
				return OpUnion.op_union(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit intersect expression
		/// </summary>
		/// <param name="iexpr">
		///            is the intersect expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(IntersectExpr iexpr)
		{
			try
			{
				ICollection args = do_bin_args(iexpr);
				return OpIntersect.op_intersect(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit except expression
		/// </summary>
		/// <param name="eexpr">
		///            is the except expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(ExceptExpr eexpr)
		{
			try
			{
				ICollection args = do_bin_args(eexpr);
				return OpExcept.op_except(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit instance of expression
		/// </summary>
		/// <param name="ioexp">
		///            is the instance of expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(InstOfExpr ioexp)
		{
			// get the value
            api.ResultSequence rs = (api.ResultSequence) ioexp.left().accept(this);

			// get the sequence type
			SequenceType seqt = (SequenceType) ioexp.right();
			return ResultSequenceFactory.create_new(new XSBoolean(isInstanceOf(rs, seqt)));
		}

		private bool isInstanceOf(api.ResultSequence rs, SequenceType seqt)
		{
			object oldParam = this._param;
			try
			{
				this._param = new Pair(null, rs);
				int sequenceLength = rs.size();
				// Run the matcher
				seqt.accept(this);
				rs = (api.ResultSequence)((Pair)_param)._two;
				int lengthAfter = rs.size();

				if (sequenceLength != lengthAfter)
				{
					return false; // Something didn't match, so it's not an instance of it
				}

				return seqt.isLengthValid(sequenceLength);
			}
			finally
			{
				this._param = oldParam;
			}
		}

		/// <summary>
		/// visit treat-as expression
		/// </summary>
		/// <param name="taexp">
		///            is the treat-as expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(TreatAsExpr taexp)
		{

            api.ResultSequence rs = (api.ResultSequence) taexp.left().accept(this);

			SequenceType seqt = (SequenceType) taexp.right();
			SeqType st = new SeqType(seqt, _sc, rs);

			try
			{
				st.match(rs);
			}
			catch (DynamicError err)
			{
				report_error(err);
			}

			return rs;
		}

		/// <summary>
		/// visit castable expression
		/// </summary>
		/// <param name="cexp">
		///            is the castable expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(CastableExpr cexp)
		{
			bool castable = false;
			try
			{
				CastExpr ce = new CastExpr((Expr) cexp.left(), (SingleType) cexp.right());

				visit(ce);
				castable = true;
			}
			catch (Exception)
			{
				castable = false;
			}

			return ResultSequenceFactory.create_new(new XSBoolean(castable));
		}

		/// <summary>
		/// visit cast expression
		/// </summary>
		/// <param name="cexp">
		///            is the cast expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(CastExpr cexp)
		{

            api.ResultSequence rs = (api.ResultSequence) cexp.left().accept(this);
			SingleType st = (SingleType) cexp.right();

			rs = FnData.atomize(rs);

			if (rs.size() > 1)
			{
				report_error(TypeError.invalid_type(null));
			}

			if (rs.empty())
			{
				if (st.qmark())
				{
					return rs;
				}
				else
				{
					report_error(TypeError.invalid_type(null));
				}
			}

			AnyType at = (AnyType) rs.item(0);

			if (!(at is AnyAtomicType))
			{
				report_error(TypeError.invalid_type(null));
			}

			AnyAtomicType aat = (AnyAtomicType) at;
			QName type = st.type();

			// prepare args from function
			var args = new List<api.ResultSequence>();
			args.Add(ResultSequenceFactory.create_new(aat));

			try
			{
				Function function = cexp.function();
				if (function == null)
				{
					function = _sc.resolveFunction(type.asQName(), args.Count);
					cexp.set_function(function);
				}
				if (function == null)
				{
					report_error(TypeError.invalid_type(null));
				}
				return function.evaluate(args, _ec);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit minus expression
		/// </summary>
		/// <param name="e">
		///            is the minus expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(MinusExpr e)
		{
            api.ResultSequence rs = (api.ResultSequence) e.arg().accept(this);

			var args = new ArrayList();
			args.Add(rs);

			try
			{
				return FsMinus.fs_minus_unary(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit plus expression
		/// </summary>
		/// <param name="e">
		///            is the plus expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(PlusExpr e)
		{
            api.ResultSequence rs = (api.ResultSequence) e.arg().accept(this);

			var args = new ArrayList();
			args.Add(rs);

			try
			{
				return FsPlus.fs_plus_unary(args);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		// this will evaluate the step expression for the whole focus and return
		// the result.
		//
		// i.e. It will execute the step expression for each item in the focus
		// [each time changing the context item].
		private api.ResultSequence do_step(StepExpr se)
		{

			ResultBuffer rs = new ResultBuffer();
			ArrayList results = new ArrayList();
			int type = 0; // 0: don't know yet
			// 1: atomic
			// 2: node

			Focus xfocus = focus();
			int original_pos = xfocus.position();

			// execute step for all items in focus
			while (true)
			{
				results.Add(se.accept(this));

				// go to next
				if (!xfocus.advance_cp())
				{
					break;
				}
			}

			// make sure we didn't change focus
			xfocus.set_position(original_pos);

			bool node_types = false;

			// check the results
			for (IEnumerator i = results.GetEnumerator(); i.MoveNext();)
			{
                api.ResultSequence result = (api.ResultSequence) i.Current;

				// make sure results are of same type, and add them in
				for (var j = result.iterator(); j.MoveNext();)
				{
					AnyType item = (AnyType) j.Current;

					// first item
					if (type == 0)
					{
						if (item is AnyAtomicType)
						{
							type = 1;
						}
						else if (item is NodeType)
						{
							type = 2;
						}
						else
						{
							Debug.Assert(false);
						}

					}

					// make sure we got coherent types
					switch (type)
					{
					// atomic... just concat
					case 1:
						if (!(item is AnyAtomicType))
						{
							report_error(TypeError.mixed_vals(null));
						}
						rs.add(item);
						break;

					case 2:
						node_types = true;
						if (!(item is NodeType))
						{
							report_error(TypeError.mixed_vals(null));
						}
						rs.add(item);
						break;

					default:
						Debug.Assert(false);
					break;
					}
				}
			}
			// XXX lame
			if (node_types)
			{
				rs = NodeType.linarize(rs);
			}
			return rs.Sequence;
		}

		private api.ResultSequence root_self_node()
		{
			Axis axis = new SelfAxis();
			ResultBuffer buffer = new ResultBuffer();

			// XXX the cast!!!
			axis.iterate((NodeType) focus().context_item(), buffer, _dc.LimitNode);

            api.ResultSequence rs = kind_test(buffer.Sequence, typeof(NodeType));

			IList records = new ArrayList();
			records.Add(rs);
			rs = FnRoot.fn_root(records, _ec);
			return rs;
		}

		private api.ResultSequence descendant_or_self_node(api.ResultSequence rs)
		{
			ResultBuffer res = new ResultBuffer();
			Axis axis = new DescendantOrSelfAxis();

			// for all nodes, get descendant or self nodes
			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType item = (NodeType) i.Current;

				axis.iterate(item, res, _dc.LimitNode);
			}

			return res.Sequence;
		}

		/// <summary>
		/// visit XPath expression
		/// </summary>
		/// <param name="e">
		///            is the XPath expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(XPathExpr e)
		{
			XPathExpr xp = e;

            api.ResultSequence rs = null;
			Focus original_focus = focus();

			// do all the steps
			while (xp != null)
			{
				StepExpr se = xp.expr();

				if (se != null)
				{
					// this is not the first step
					if (rs != null)
					{
						// XXX ?
						// the expression didn't return any
						// results...
						if (rs.size() == 0)
						{
							break;
						}

						// make sure result of previous step are
						// nodes!
						for (var i = rs.iterator(); i.MoveNext();)
						{
							AnyType item = (AnyType) i.Current;

							if (!(item is NodeType))
							{
								report_error(TypeError.step_conatins_atoms(null));
								return null; // unreach
							}
						}

						// check if we got a //
						if (xp.slashes() == 2)
						{
							rs = descendant_or_self_node(rs);

							if (rs.size() == 0)
							{
								break;
							}
						}

						// make result of previous step the new
						// focus
						set_focus(new Focus(rs));

						// do the step for all item in context
						rs = do_step(se);
					}
					// this is first step...
					// note... we may be called from upstream...
					// like in the expression sorbo/*[2] ... we may
					// be called to evaluate the 2... the caller
					// will iterate through the whole outer focus
					// for us
					else
					{
						// XXX ???
						if (xp.slashes() == 1)
						{
							rs = root_self_node();
							set_focus(new Focus(rs));

							rs = do_step(se);
						}
						else if (xp.slashes() == 2)
						{
							rs = root_self_node();

							rs = descendant_or_self_node(rs);

							set_focus(new Focus(rs));

							rs = do_step(se);
						}
						else
						{
                            rs = (api.ResultSequence) se.accept(this);
						}
					}
				}
				// the expression is "/"
				else
				{
					Debug.Assert(xp.slashes() == 1);

					rs = root_self_node();
				}

				xp = xp.next();
			}

			// restore focus
			set_focus(original_focus);

			return rs;
		}

		/// <summary>
		/// visit a forward step expression
		/// </summary>
		/// <param name="e">
		///            is the forward step. </param>
		/// <returns> a new function </returns>
		public virtual object visit(ForwardStep e)
		{

			// get context node
			AnyType ci = focus().context_item();

			if (ci == null)
			{
				report_error(DynamicError.contextUndefined());
			}

			if (!(ci is NodeType))
			{
				report_error(TypeError.ci_not_node(ci.string_type()));
			}

			NodeType cn = (NodeType) ci;

			// get the nodes on the axis
			ForwardAxis axis = e.iterator();
			ResultBuffer rb = new ResultBuffer();
			axis.iterate(cn, rb, _dc.LimitNode);
			// get all nodes in the axis, and principal node
			Pair arg = new Pair(axis.principal_node_kind().string_type(), rb.Sequence);

			// do the name test
			_param = arg;
            api.ResultSequence rs = (api.ResultSequence) e.node_test().accept(this);

			return rs;
		}

		/// <summary>
		/// visit a reverse step expression
		/// </summary>
		/// <param name="e">
		///            is the reverse step. </param>
		/// <returns> a new function </returns>
		// XXX unify with top
		public virtual object visit(ReverseStep e)
		{
            ResultBuffer result = new ResultBuffer();

			// get context node
			AnyType ci = focus().context_item();

            if (ci == null) return result.Sequence;

            if (!(ci is NodeType))
			{
				report_error(TypeError.ci_not_node(ci.string_type()));
			}

			NodeType cn = (NodeType) ci;

			// get the nodes on the axis
			ReverseAxis axis = e.iterator();

			// short for "gimme da parent"
			if (e.axis() == ReverseStep.Type.DOTDOT)
			{
				(new ParentAxis()).iterate(cn, result, _dc.LimitNode);
				return result.Sequence;
			}

			Debug.Assert(axis != null);

			axis.iterate(cn, result, null);
			// get all nodes in the axis, and principal node
			Pair arg = new Pair(axis.principal_node_kind().string_type(), result.Sequence);

			// do the name test
			_param = arg;
            api.ResultSequence rs = (api.ResultSequence) e.node_test().accept(this);

			return rs;
		}

		// XXX this routine sux
		private bool name_test(NodeType node, QName name, string type)
		{
			// make sure principal node kind is the same
			if (node == null)
			{
				return false;
			}
			if (!type.Equals(node.string_type()))
			{
				return false;
			}

			string test_prefix = name.prefix();

			// if unprefixed and principal node kind is element, set default
			// element namespace
			if (string.ReferenceEquals(test_prefix, null) && type.Equals("element"))
			{
				// XXX make a new copy
				name = new QName(null, name.local());
				name.set_namespace(_sc.DefaultNamespace);

				// if we actually have a namespace, pretend we do =D
				if (!string.ReferenceEquals(name.@namespace(), null) && name.@namespace().Length > 0)
				{
					test_prefix = "";
				}
			}

			QName node_name = node.node_name();

			Debug.Assert(node_name != null);

			// make sure namespace matches
			string node_namespace = node_name.@namespace();

			string test_namespace = null;
			if (name.expanded())
			{
				test_namespace = name.@namespace();
			}

			// name test has no prefix
			if (string.ReferenceEquals(test_prefix, null))
			{
				// ok no namespace... match
				if (string.ReferenceEquals(node_namespace, null))
				{
				}
				else
				{
					return false;
				}
			}
			// name test has a prefix and is not wildcard
			// XXX AT THIS POINT ALL PREFIXES NEED TO BE RESOLVED!
			else if (!test_namespace.Equals("*"))
			{
				// the node doesn't have a namespace... no match
				if (string.ReferenceEquals(node_namespace, null))
				{
					return false;
				}
				// check namespaces
				else
				{
					if (node_namespace.Equals(test_namespace))
					{
						// namespace matches
					}
					else
					{
						return false;
					}
				}
			}

			// make sure local part matches
			// check for wildcard in localpart
			if (name.local().Equals("*"))
			{
				return true;
			}

			// check if local part matches
			if (!name.local().Equals(node_name.local()))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// visit a name test expression
		/// </summary>
		/// <param name="e">
		///            is thename test. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(NameTest e)
		{
			QName name = e.name();

			// get the arguments
			Pair arg = (Pair) _param;
			string type = (string) arg._one;
            api.ResultSequence rs = (api.ResultSequence) arg._two;

			ResultBuffer rb = new ResultBuffer();

			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType nt = (NodeType) i.Current;

				// check if node passes name test
				if (name_test(nt, name, type))
				{
					rb.add(nt);
				}
			}
			rs = rb.Sequence;
			arg._two = rs;

			return rs;
		}

		/// <summary>
		/// visit variable reference
		/// </summary>
		/// <param name="e">
		///            is the variable reference. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(VarRef e)
		{
			ResultBuffer rs = new ResultBuffer();

			object @var = getVariable(e.name());

			Debug.Assert(@var != null);

			if (@var is AnyType)
			{
			   rs.add((AnyType) @var);
			}
			else if (@var is api.ResultSequence)
			{
			   rs.concat((api.ResultSequence) @var);
			}

			return rs.Sequence;
		}

		/// <summary>
		/// visit string literal.
		/// </summary>
		/// <param name="e">
		///            is the string literal. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(StringLiteral e)
		{
			return e.value();
		}

		/// <summary>
		/// visit integer literal.
		/// </summary>
		/// <param name="e">
		///            is the integer literal. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(IntegerLiteral e)
		{
			return e.value();
		}

		/// <summary>
		/// visit double literal.
		/// </summary>
		/// <param name="e">
		///            is the double literal. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(DoubleLiteral e)
		{
			return e.value();
		}

		/// <summary>
		/// visit decimal literal.
		/// </summary>
		/// <param name="e">
		///            is the decimal literal. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(DecimalLiteral e)
		{
			ResultBuffer rs = new ResultBuffer();

			rs.add(e.value());
			return rs.Sequence;
		}

		/// <summary>
		/// visit parent expression.
		/// </summary>
		/// <param name="e">
		///            is the parent expression. </param>
		/// <returns> a new function </returns>
		public virtual object visit(ParExpr e)
		{
			return do_expr(e.GetEnumerator());
		}

		/// <summary>
		/// visit context item expression.
		/// </summary>
		/// <param name="e">
		///            is the context item expression. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(CntxItemExpr e)
		{
			ResultBuffer rs = new ResultBuffer();

			AnyType contextItem = focus().context_item();
			if (contextItem == null)
			{
				report_error(DynamicError.contextUndefined());
			}
			rs.add(contextItem);
			return rs.Sequence;
		}

		/// <summary>
		/// visit function call.
		/// </summary>
		/// <param name="e">
		///            is the function call. </param>
		/// <returns> a new function or null </returns>
		public virtual object visit(FunctionCall e)
		{
			var args = new List<api.ResultSequence>();

			for (IEnumerator i = e.GetEnumerator(); i.MoveNext();)
			{
				Expr arg = (Expr) i.Current;
				// each argument will produce a result sequence
				args.Add((api.ResultSequence)arg.accept(this));
			}

			try
			{
				Function function = e.function();
				if (function == null)
				{
					function = _sc.resolveFunction(e.name().asQName(), args.Count);
					e.set_function(function);
				}
				return function.evaluate(args, _ec);
			}
			catch (DynamicError err)
			{
				report_error(err);
				return null; // unreach
			}
		}

		/// <summary>
		/// visit single type.
		/// </summary>
		/// <param name="e">
		///            is the single type. </param>
		/// <returns> null </returns>
		public virtual object visit(SingleType e)
		{
			return null;
		}

		/// <summary>
		/// visit sequence type.
		/// </summary>
		/// <param name="e">
		///            is the sequence type. </param>
		/// <returns> null </returns>
		public virtual object visit(SequenceType e)
		{
			ItemType it = e.item_type();

			if (it != null)
			{
				it.accept(this);
			}

			return null;
		}

		/// <summary>
		/// visit item type.
		/// </summary>
		/// <param name="e">
		///            is the item type. </param>
		/// <returns> null </returns>
		public virtual object visit(ItemType e)
		{

			switch (e.type())
			{
			case ItemType.ITEM:
				break;
			case ItemType.QNAME:

				bool ok = false;
				TypeModel model = _sc.TypeModel;
				if (model != null)
				{
					ok = _sc.TypeModel.lookupType(e.qname().@namespace(), e.qname().local()) != null;
				}
				if (!ok)
				{
					ok = BuiltinTypeLibrary.BUILTIN_TYPES.lookupType(e.qname().@namespace(), e.qname().local()) != null;
				}
				if (!ok)
				{
					report_error(new StaticTypeNameError("Type not defined: " + e.qname().@string()));
				}

				api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;
				((Pair) _param)._two = item_test(arg, e.qname());
				break;

			case ItemType.KINDTEST:
				((Pair) _param)._two = e.kind_test().accept(this);
				break;
			}

			return null;
		}

		private api.ResultSequence item_test(api.ResultSequence rs, QName qname)
		{
			ResultBuffer rb = new ResultBuffer();
			for (var i = rs.iterator(); i.MoveNext();)
			{
				AnyType item = (AnyType) i.Current;

				if (item is NodeType)
				{
					NodeType node = ((NodeType)item);
					if (derivesFrom(node, qname))
					{
						rb.add(node);
					}
				}
				else
				{
					// atomic of some sort
					if (qname.Equals(ANY_ATOMIC_TYPE))
					{
						rb.add(item);
						continue; // match !
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.eclipse.wst.xml.xpath2.processor.internal.types.AnyAtomicType aat = makeAtomic(qname);
					AnyAtomicType aat = makeAtomic(qname);
					if (aat.GetType().IsInstanceOfType(item))
					{
						rb.add(item);
					}

					// fall through => non-match
				}
			}
			return rb.Sequence;
		}

		private api.ResultSequence kind_test(api.ResultSequence rs, Type kind)
		{
			ResultBuffer rb = new ResultBuffer();
			for (var i = rs.iterator(); i.MoveNext();)
			{
				Item item = (Item) i.Current;
				if (kind.IsInstanceOfType(item))
				{
					rb.add(item);
				}
			}
			return rb.Sequence;
		}

		/// <summary>
		/// visit any kind test.
		/// </summary>
		/// <param name="e">
		///            is the any kind test. </param>
		/// <returns> a new function </returns>
		public virtual object visit(AnyKindTest e)
		{
            api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;

			return kind_test(arg, typeof(NodeType));
		}

		/// <summary>
		/// visit document test.
		/// </summary>
		/// <param name="e">
		///            is the document test. </param>
		/// <returns> result sequence </returns>
		public virtual object visit(DocumentTest e)
		{
            api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;
			int type = e.type();

			// filter doc nodes
            api.ResultSequence rs = kind_test(arg, typeof(DocType));

			if (type == DocumentTest.NONE)
			{
				return rs;
			}

			// for all docs, find the ones with exactly one element, and do
			// the element test
			for (var i = rs.iterator(); i.MoveNext();)
			{
				DocType doc = (DocType) i.Current;
				int elem_count = 0;
				ElementType elem = null;

				// make sure doc has only 1 element
				NodeList children = doc.node_value().ChildNodes;
				for (int j = 0; j < children.Length; j++)
				{
					Node child = children.item(j);

					// bingo
					if (child.NodeType == NodeConstants.ELEMENT_NODE)
					{
						elem_count++;

						if (elem_count > 1)
						{
							break;
						}

						elem = new ElementType((Element) child, _sc.TypeModel);
					}
				}

				// this doc is no good... send him to hell
				if (elem_count != 1)
				{
				//	i.remove();
					continue;
				}

				Debug.Assert(elem != null);

				// setup parameter for element test
                api.ResultSequence res = new ResultBuffer.SingleResultSequence(elem);
				_param = new Pair("element", res);

				// do name test
				res = null;
				if (type == DocumentTest.ELEMENT)
				{
					res = (api.ResultSequence) e.elem_test().accept(this);
				}
				else if (type == DocumentTest.SCHEMA_ELEMENT)
				{
					res = (api.ResultSequence) e.schema_elem_test().accept(this);
				}
				else
				{
					Debug.Assert(false);
				}

				// check if element survived nametest
				if (res.size() != 1)
				{
			//		i.remove();
				}
			}

			return rs;
		}

		/// <summary>
		/// visit text test.
		/// </summary>
		/// <param name="e">
		///            is the text test. </param>
		/// <returns> a new function </returns>
		public virtual object visit(TextTest e)
		{
            api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;

			((Pair) _param)._two = kind_test(arg, typeof(TextType));
			return ((Pair) _param)._two;
		}

		/// <summary>
		/// visit comment test.
		/// </summary>
		/// <param name="e">
		///            is the text test. </param>
		/// <returns> a new function </returns>
		public virtual object visit(CommentTest e)
		{
            api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;

			return kind_test(arg, typeof(CommentType));
		}

		/// <summary>
		/// visit PI test.
		/// </summary>
		/// <param name="e">
		///            is the PI test. </param>
		/// <returns> a argument </returns>
		public virtual object visit(PITest e)
		{
            api.ResultSequence arg = (api.ResultSequence)((Pair) _param)._two;

			string pit_arg = e.arg();

			// match any pi
			if (string.ReferenceEquals(pit_arg, null))
			{
				return kind_test(arg, typeof(PIType));
			}

			ResultBuffer rb = new ResultBuffer();
			for (var i = arg.iterator(); i.MoveNext();)
			{
				AnyType item = (AnyType) i.Current;

				// match PI
				if (item is PIType)
				{
					PIType pi = (PIType) item;

					// match target
					if (pit_arg.Equals(pi.value().Target))
					{
						rb.add(pi);
					}
				}
			}
			arg = rb.Sequence;
			((Pair) _param)._two = arg;
			return arg;
		}

		/// <summary>
		/// visit attribute test.
		/// </summary>
		/// <param name="e">
		///            is the attribute test. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(AttributeTest e)
		{
			// filter out all attrs
            api.ResultSequence rs = kind_test((api.ResultSequence)((Pair) _param)._two, typeof(AttrType));

			ResultBuffer rb = new ResultBuffer();

			QName name = e.name();
			QName type = e.type();

			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;
				// match the name if it's not a wild card
				if (name != null && !e.wild())
				{
					if (!name_test(node, name, "attribute"))
					{
						continue;
					}
				}
				// match the type
				if (type != null)
				{
					// check if element derives from
					if (!derivesFrom(node, type))
					{
						continue;
					}
				}
				rb.add(node);
			}
			((Pair) _param)._two = rb.Sequence;
			return ((Pair) _param)._two;
		}

		/// <summary>
		/// visit schema attribute test.
		/// </summary>
		/// <param name="e">
		///            is the schema attribute test. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(SchemaAttrTest e)
		{
			// filter out all attrs
            api.ResultSequence rs = kind_test((api.ResultSequence)((Pair) _param)._two, typeof(AttrType));

			// match the name
			QName name = e.arg();
			for (var i = rs.iterator(); i.MoveNext();)
			{
				if (!name_test((NodeType) i.Current, name, "attribute"))
				{

					//i.remove();
				}
			}

			// check the type
			TypeDefinition et = _sc.TypeModel.lookupAttributeDeclaration(name.@namespace(), name.local());
			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;

				if (!derivesFrom(node, et))
				{
				//	i.remove();
				}

			}

			return rs;
		}

		/// <summary>
		/// visit element test.
		/// </summary>
		/// <param name="e">
		///            is the element test. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(ElementTest e)
		{
			// filter out all elements
            api.ResultSequence rs = kind_test((api.ResultSequence)((Pair) _param)._two, typeof(ElementType));

			// match the name if it's not a wild card
			ResultBuffer rb = new ResultBuffer();
			QName nameTest = e.name();
			QName typeTest = e.type();
			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;

				if (nameTest != null && !e.wild())
				{
					// skip if there's a name test and the name does not match
					if (!name_test((ElementType) node, nameTest, "element"))
					{
						continue;
					}
				}
				if (typeTest != null)
				{
					// check if element derives from
					if (!derivesFrom(node, typeTest))
					{
						continue;
					}

					// nilled may be true or false
					if (!e.qmark())
					{
						XSBoolean nilled = (XSBoolean) node.nilled().first();
						if (nilled.value())
						{
							continue;
						}
					}
				}
				rb.add(node);
			}
			((Pair) _param)._two = rb.Sequence;
			return ((Pair) _param)._two;
		}

		/// <summary>
		/// visit schema element test.
		/// </summary>
		/// <param name="e">
		///            is the schema element test. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(SchemaElemTest e)
		{
			// filter out all elements
            api.ResultSequence rs = kind_test((api.ResultSequence)((Pair) _param)._two, typeof(ElementType));

			// match the name
			// XXX substitution groups
			QName name = e.name();
			for (var i = rs.iterator(); i.MoveNext();)
			{
				if (!name_test((ElementType) i.Current, name, "element"))
				{

			//		i.remove();
				}
			}

			// check the type
			TypeDefinition et = _sc.TypeModel.lookupElementDeclaration(name.@namespace(), name.local());
			for (var i = rs.iterator(); i.MoveNext();)
			{
				NodeType node = (NodeType) i.Current;

				if (!derivesFrom(node, et))
				{
	//				i.remove();
					continue;
				}

				XSBoolean nilled = (XSBoolean) node.nilled().first();
				// XXX or, in schema it is nillable
				if (nilled.value())
				{
		//			i.remove();
				}
			}

			return rs;
		}

		private bool predicate_truth(api.ResultSequence rs)
		{
			// rule 1 of spec... if numeric type:
			// if num eq position then true else false
			if (rs.size() == 1)
			{
				AnyType at = (AnyType) rs.item(0);

				if (at is NumericType)
				{
					try
					{
						return FsEq.fs_eq_fast(at, new XSInteger(new System.Numerics.BigInteger(focus().position())), _dc);
					}
					catch (DynamicError err)
					{
						report_error(err);

						// unreach
						Debug.Assert(false);
						return false;
					}
				}
			}

			// rule 2
			XSBoolean ret = effective_boolean_value(rs);

			return ret.value();
		}

		// do the predicate for all items in focus
		private api.ResultSequence do_predicate(ICollection exprs)
		{
			ResultBuffer rs = new ResultBuffer();

			Focus xfocus = focus();
			int original_cp = xfocus.position();

			// optimization
			// check if predicate is single numeric constant
			//if (exprs.Count == 1)
   //         {
   //             var x = exprs.GetEnumerator();
   //             x.MoveNext();
			//	Expr expr = (Expr) x.Current;

			//	if (expr is XPathExpr)
			//	{
			//		XPathExpr xpe = (XPathExpr) expr;
			//		if (xpe.next() == null && xpe.slashes() == 0 && xpe.expr() is FilterExpr)
			//		{
			//			FilterExpr fex = (FilterExpr) xpe.expr();
			//			if (fex.primary() is IntegerLiteral)
			//			{
			//				int pos = (int) (((IntegerLiteral) fex.primary()).value().int_value());

			//				if (pos <= xfocus.last() && pos > 0)
			//				{
			//					xfocus.set_position(pos);
			//					rs.add(xfocus.context_item());
			//				}
			//				xfocus.set_position(original_cp);
			//				return rs.Sequence;
			//			}
			//		}
			//	}
			//}

			// go through all items in focus.
            while (true)
            {
				// do_expr only takes one item, but could result in a set of more than one thing.
				api.ResultSequence res = do_expr(exprs.GetEnumerator());

                if (res.size() > 1)
                {}

                // if predicate is true, the context item is definitely
				// in the sequence
				if (predicate_truth(res))
				{
					rs.add(focus().context_item());
				}

				if (!xfocus.advance_cp())
				{
					break;
				}

			}

			// restore
			xfocus.set_position(original_cp);

			return rs.Sequence;
		}

		/// <summary>
		/// visit axis step.
		/// </summary>
		/// <param name="e">
		///            is the axis step. </param>
		/// <returns> a result sequence </returns>
		public virtual object visit(AxisStep e)
		{
			api.ResultSequence rs = (api.ResultSequence) e.step().accept(this);

			if (e.predicate_count() == 0)
			{
				return rs;
			}

			// I take it predicates are logical ANDS...
			Focus original_focus = focus();

			// go through all predicates
			for (IEnumerator i = e.GetEnumerator(); i.MoveNext();)
			{
				// empty results... get out of here ? XXX
				if (rs.size() == 0)
				{
					break;
				}

				set_focus(new Focus(rs));
				rs = do_predicate((ICollection) i.Current);

			}

			// restore focus [context switching ;D ]
			set_focus(original_focus);
			return rs;
		}

		/// <summary>
		/// visit filter expression
		/// </summary>
		/// <param name="e">
		///            is the filter expression. </param>
		/// <returns> a result sequence </returns>
		// XXX unify with top ?
		public virtual object visit(FilterExpr e)
		{
            api.ResultSequence rs = (api.ResultSequence) e.primary().accept(this);

			// if no predicates are present, then the result is the same as
			// the primary expression
			if (e.predicate_count() == 0)
			{
				return rs;
			}

			Focus original_focus = focus();

			// go through all predicates
			for (IEnumerator i = e.GetEnumerator(); i.MoveNext();)
			{
				if (rs.size() == 0)
				{
					break;
				}

				set_focus(new Focus(rs));
				rs = do_predicate((ICollection) i.Current);
			}

			// restore focus [context switching ;D ]
			set_focus(original_focus);
			return rs;
		}

        public object visit(PostfixExpr e)
        {
            api.ResultSequence rs = (api.ResultSequence)e.primary().accept(this);

            // if no predicates are present, then the result is the same as
            // the primary expression
            if (e.predicate_count() == 0)
            {
                return rs;
            }

            Focus original_focus = focus();

            // go through all predicates
            for (IEnumerator i = e.GetEnumerator(); i.MoveNext();)
            {
                if (rs.size() == 0)
                {
                    break;
                }

                set_focus(new Focus(rs));
                rs = do_predicate((ICollection)i.Current);
            }

            // restore focus [context switching ;D ]
            set_focus(original_focus);
			return rs;
        }
	}

}