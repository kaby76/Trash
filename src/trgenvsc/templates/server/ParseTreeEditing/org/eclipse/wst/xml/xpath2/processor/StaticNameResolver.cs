using System;
using System.Collections;
using System.Collections.Generic;
using java.xml;
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
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
///     Jesper Steen Moller - bug 343804 - Updated API information
///     Lukasz Wycisk - bug 361802 - Default variable namespace � no namespace
/// 
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor
{

	using Function = org.eclipse.wst.xml.xpath2.api.Function;
	using XPath = org.eclipse.wst.xml.xpath2.processor.ast.XPath;
	using ReverseAxis = org.eclipse.wst.xml.xpath2.processor.@internal.ReverseAxis;
	using StaticAttrNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticAttrNameError;
	using StaticContextAdapter = org.eclipse.wst.xml.xpath2.processor.@internal.StaticContextAdapter;
	using StaticElemNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticElemNameError;
	using StaticFunctNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticFunctNameError;
	using StaticNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticNameError;
	using StaticNsNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticNsNameError;
	using StaticTypeNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticTypeNameError;
	using StaticVarNameError = org.eclipse.wst.xml.xpath2.processor.@internal.StaticVarNameError;
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
	using UnExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnExpr;
	using UnionExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.UnionExpr;
	using VarExprPair = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarExprPair;
	using VarRef = org.eclipse.wst.xml.xpath2.processor.@internal.ast.VarRef;
	using XPathExpr = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathExpr;
	using XPathNode = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathNode;
	using XPathVisitor = org.eclipse.wst.xml.xpath2.processor.@internal.ast.XPathVisitor;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using SimpleAtomicItemTypeImpl = org.eclipse.wst.xml.xpath2.processor.@internal.types.SimpleAtomicItemTypeImpl;
	using BuiltinTypeDefinition = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeDefinition;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// This class resolves static names.
	/// </summary>
	public class StaticNameResolver : XPathVisitor, StaticChecker
	{
		internal class DummyError : Exception
		{

			/// 
			internal const long serialVersionUID = 3898564402981741950L;
		}

		private org.eclipse.wst.xml.xpath2.api.StaticContext _sc;
		private StaticNameError _err;

		private ISet<javax.xml.@namespace.QName> _resolvedFunctions = new HashSet<javax.xml.@namespace.QName>();
		private ISet<string> _axes = new HashSet<string>();
		private ISet<javax.xml.@namespace.QName> _freeVariables = new HashSet<javax.xml.@namespace.QName>();
		/// <summary>
		/// Constructor for static name resolver
		/// </summary>
		/// <param name="sc">
		///            is the static context.
		/// @since 2.0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public StaticNameResolver(final org.eclipse.wst.xml.xpath2.processor.StaticContext sc)
		public StaticNameResolver(org.eclipse.wst.xml.xpath2.processor.StaticContext sc)
		{
			_sc = new StaticContextAdapter(sc);
			_err = null;
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public StaticNameResolver(org.eclipse.wst.xml.xpath2.api.StaticContext context)
		{
			_sc = context;
			_err = null;
		}

		internal class VariableScope
		{
			private readonly StaticNameResolver outerInstance;

			public VariableScope(StaticNameResolver outerInstance, QName name, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType typeDef, VariableScope nextScope)
			{
				this.outerInstance = outerInstance;
				this.name = name;
				this.typeDef = typeDef;
				this.nextScope = nextScope;
			}
			public readonly QName name;
			public readonly org.eclipse.wst.xml.xpath2.api.typesystem.ItemType typeDef;
			public readonly VariableScope nextScope;
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual ISet<string> Axes
		{
			get
			{
				return _axes;
			}
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual ISet<javax.xml.@namespace.QName> FreeVariables
		{
			get
			{
				return _freeVariables;
			}
		}

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual ISet<javax.xml.@namespace.QName> ResolvedFunctions
		{
			get
			{
				return _resolvedFunctions;
			}
		}

		private VariableScope _innerScope = null;
		private bool _rootUsed;

		private org.eclipse.wst.xml.xpath2.api.typesystem.ItemType getVariableType(QName name)
		{
			VariableScope scope = _innerScope;
			while (scope != null)
			{
				if (name.Equals(scope.name))
				{
					return scope.typeDef;
				}
				scope = scope.nextScope;
			}
			return _sc.InScopeVariables.getVariableType(name.asQName());
		}

		private bool isVariableCaptured(QName name)
		{
			VariableScope scope = _innerScope;
			while (scope != null)
			{
				if (name.Equals(scope.name))
				{
					return true;
				}
				scope = scope.nextScope;
			}
			return false;
		}

		private bool isVariableInScope(QName name)
		{
			return isVariableCaptured(name) || _sc.InScopeVariables.isVariablePresent(name.asQName());
		}

		private void popScope()
		{
			if (_innerScope == null)
			{
				throw new System.InvalidOperationException("Unmatched scope pop");
			}
			_innerScope = _innerScope.nextScope;
		}

		private void pushScope(QName @var, BuiltinTypeDefinition xsAnytype)
		{
			_innerScope = new VariableScope(this, @var, new SimpleAtomicItemTypeImpl(xsAnytype), _innerScope);
		}

		private bool expandQName(QName name, string def, bool allowWildcards)
		{
			string prefix = name.prefix();

			if ("*".Equals(prefix))
			{
				if (!allowWildcards)
				{
					return false;
				}
				name.set_namespace("*");
				return true;
			}

			if (string.ReferenceEquals(prefix, null) || XMLConstants.DEFAULT_NS_PREFIX.Equals(prefix))
			{
				name.set_namespace(def);
				return true;
			}

			// At this point, we know we have a non-null prefix, so look it up
			string namespaceURI = _sc.NamespaceContext.getNamespaceURI(prefix);
			if (XMLConstants.NULL_NS_URI.Equals(namespaceURI))
			{
				return false;
			}

			name.set_namespace(namespaceURI);
			return true;
		}

		private bool expandItemQName(QName name)
		{
			return expandQName(name, _sc.DefaultNamespace, true);
		}

		private bool expandVarQName(QName name)
		{
			return expandQName(name, null, false);
		}

		/// <summary>
		/// Expands a qname and uses the default function namespace if unprefixed.
		/// </summary>
		/// <param name="name">
		///            qname to expand. </param>
		/// <returns> true on success. </returns>
		private bool expandFunctionQName(QName name)
		{
			return expandQName(name, _sc.DefaultFunctionNamespace, false);
		}

		/// <summary>
		/// Expands a qname and uses the default type/element namespace if
		/// unprefixed.
		/// </summary>
		/// <param name="name">
		///            qname to expand. </param>
		/// <returns> true on success. </returns>
		private bool expandItemTypeQName(QName name)
		{
			return expandQName(name, _sc.DefaultNamespace, false);
		}

		// the problem is that visitor interface does not throw exceptions...
		// so we get around it ;D
		private void reportError(StaticNameError err)
		{
			_err = err;
			throw new DummyError();
		}

		private void reportBadPrefix(string prefix)
		{
			reportError(StaticNsNameError.unknown_prefix(prefix));
		}

		/// <summary>
		/// Check the XPath node.
		/// </summary>
		/// <param name="node">
		///            is the XPath node to check. </param>
		/// <exception cref="StaticError">
		///             static error. </exception>
		public virtual void check(XPathNode node)
		{
			try
			{
				node.accept(this);
			}
			catch (DummyError)
			{
				throw _err;
			}
		}

		/// <summary>
		/// Validate an XPath by visiting all the nodes.
		/// </summary>
		/// <param name="xp">
		///            is the XPath. </param>
		/// <returns> null. </returns>
		public virtual object visit(XPath xp)
		{
			for (IEnumerator<Expr> i = xp.iterator(); i.MoveNext();)
			{
				Expr e = (Expr) i.Current;

				e.accept(this);
			}

			return null;
		}

		// does a for and a quantified expression
		// takes the iterator for var expr paris
		private void doForExpr(IEnumerator iter, Expr expr)
		{
			int scopes = 0;

			// add variables to scope and check the binding sequence
			while (iter.MoveNext())
			{
				VarExprPair pair = (VarExprPair) iter.Current;

				QName @var = pair.varname();
				if (!expandVarQName(@var))
				{
					reportBadPrefix(@var.prefix());
				}

				Expr e = pair.expr();

				e.accept(this);

				pushScope(@var, BuiltinTypeLibrary.XS_ANYTYPE);
				scopes++;
			}

			expr.accept(this);

			// kill the scopes
			for (int i = 0; i < scopes; i++)
			{
				popScope();
			}
		}

		/// <summary>
		/// Validate a for expression.
		/// </summary>
		/// <param name="fex">
		///            is the for expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ForExpr fex)
		{

			doForExpr(fex.iterator(), fex.expr());

			return null;
		}

		/// <summary>
		/// Validate a quantified expression.
		/// </summary>
		/// <param name="qex">
		///            is the quantified expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(QuantifiedExpr qex)
		{
			// lets cheat
			doForExpr(qex.iterator(), qex.expr());

			return null;
		}

		private void visitExprs(IEnumerator i)
		{
			while (i.MoveNext())
			{
				Expr e = (Expr) i.Current;

				e.accept(this);
			}
		}

		/// <summary>
		/// Validate an if expression.
		/// </summary>
		/// <param name="ifex">
		///            is the if expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(IfExpr ifex)
		{

			visitExprs(ifex.iterator());

			ifex.then_clause().accept(this);

			ifex.else_clause().accept(this);

			return null;
		}

		/// <summary>
		/// Validate a binary expression by checking its left and right children.
		/// </summary>
		/// <param name="name">
		///            is the name of the binary expression. </param>
		/// <param name="e">
		///            is the expression itself. </param>
		public virtual void printBinExpr(string name, BinExpr e)
		{
			e.left().accept(this);
			e.right().accept(this);
		}

		/// <summary>
		/// Validate an OR expression.
		/// </summary>
		/// <param name="orex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(OrExpr orex)
		{
			printBinExpr("OR", orex);
			return null;
		}

		/// <summary>
		/// Validate an AND expression.
		/// </summary>
		/// <param name="andex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(AndExpr andex)
		{
			printBinExpr("AND", andex);
			return null;
		}

		/// <summary>
		/// Validate a comparison expression.
		/// </summary>
		/// <param name="cmpex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(CmpExpr cmpex)
		{
			printBinExpr("CMP" + cmpex.type(), cmpex);
			return null;
		}

		/// <summary>
		/// Validate a range expression.
		/// </summary>
		/// <param name="rex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(RangeExpr rex)
		{
			printBinExpr("RANGE", rex);
			return null;
		}

		/// <summary>
		/// Validate an additon expression.
		/// </summary>
		/// <param name="addex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(AddExpr addex)
		{
			printBinExpr("ADD", addex);
			return null;
		}

		/// <summary>
		/// Validate a subtraction expression.
		/// </summary>
		/// <param name="subex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(SubExpr subex)
		{
			printBinExpr("SUB", subex);
			return null;
		}

		/// <summary>
		/// Validate a multiplication expression.
		/// </summary>
		/// <param name="mulex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(MulExpr mulex)
		{
			printBinExpr("MUL", mulex);
			return null;
		}

		/// <summary>
		/// Validate a division expression.
		/// </summary>
		/// <param name="mulex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(DivExpr mulex)
		{
			printBinExpr("DIV", mulex);
			return null;
		}

		/// <summary>
		/// Validate an integer divison expression.
		/// </summary>
		/// <param name="mulex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(IDivExpr mulex)
		{
			printBinExpr("IDIV", mulex);
			return null;
		}

		/// <summary>
		/// Validate a mod expression.
		/// </summary>
		/// <param name="mulex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ModExpr mulex)
		{
			printBinExpr("MOD", mulex);
			return null;
		}

		/// <summary>
		/// Validate a union expression.
		/// </summary>
		/// <param name="unex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(UnionExpr unex)
		{
			printBinExpr("UNION", unex);
			return null;
		}

		/// <summary>
		/// Validate a piped expression.
		/// </summary>
		/// <param name="pipex">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(PipeExpr pipex)
		{
			printBinExpr("PIPE", pipex);
			return null;
		}

		/// <summary>
		/// Validate an intersection expression.
		/// </summary>
		/// <param name="iexpr">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(IntersectExpr iexpr)
		{
			printBinExpr("INTERSECT", iexpr);
			return null;
		}

		/// <summary>
		/// Validate an except expression.
		/// </summary>
		/// <param name="eexpr">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ExceptExpr eexpr)
		{
			printBinExpr("INT_EXCEPT", eexpr);
			return null;
		}

		/// <summary>
		/// Validate an 'instance of' expression.
		/// </summary>
		/// <param name="ioexp">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(InstOfExpr ioexp)
		{
			printBinExpr("INSTANCEOF", ioexp);
			return null;
		}

		/// <summary>
		/// Validate a 'treat as' expression.
		/// </summary>
		/// <param name="taexp">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(TreatAsExpr taexp)
		{
			printBinExpr("TREATAS", taexp);
			return null;
		}

		/// <summary>
		/// Validate a castable expression.
		/// </summary>
		/// <param name="cexp">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(CastableExpr cexp)
		{
			printBinExpr("CASTABLE", cexp);
			return null;
		}

		/// <summary>
		/// Validate a cast expression.
		/// </summary>
		/// <param name="cexp">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(CastExpr cexp)
		{
			printBinExpr("CAST", cexp);

			SingleType st = (SingleType) cexp.right();
			QName type = st.type();

			javax.xml.@namespace.QName qName = type.asQName();
			Function f = _sc.resolveFunction(qName, 1);
			if (f == null)
			{
				reportError(new StaticFunctNameError("Type does not exist: " + type.ToString()));
			}
			cexp.set_function(f);
			_resolvedFunctions.Add(qName);

			return null;
		}

		/// <summary>
		/// Validate a unary expression by checking its one child.
		/// </summary>
		/// <param name="name">
		///            is the name of the expression. </param>
		/// <param name="e">
		///            is the expression itself. </param>
		public virtual void printUnExpr(string name, UnExpr e)
		{
			e.arg().accept(this);

		}

		/// <summary>
		/// Validate a minus expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(MinusExpr e)
		{
			printUnExpr("MINUS", e);
			return null;
		}

		/// <summary>
		/// Validate a plus expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(PlusExpr e)
		{
			printUnExpr("PLUS", e);
			return null;
		}

		/// <summary>
		/// Validate an xpath expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(XPathExpr e)
		{
			XPathExpr xp = e;
			bool firstStep = true;

			while (xp != null)
			{
				if (firstStep && xp.slashes() != 0)
				{
					_rootUsed = true;
				}

				firstStep = false;
				StepExpr se = xp.expr();

				if (se != null)
				{
					se.accept(this);
				}

				xp = xp.next();
			}
			return null;
		}

		/// <summary>
		/// Validate a forward step.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ForwardStep e)
		{
			e.node_test().accept(this);

			_axes.Add(e.iterator().name());

			return null;
		}

		/// <summary>
		/// Validate a reverse step.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ReverseStep e)
		{

			NodeTest nt = e.node_test();
			if (nt != null)
			{
				nt.accept(this);
			}

			var iterator = e.iterator();
			if (iterator != null)
			{
				_axes.Add(iterator.name());
			}

			return null;
		}

		/// <summary>
		/// Validate a name test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(NameTest e)
		{
			QName name = e.name();

			if (!expandItemQName(name))
			{
				reportBadPrefix(name.prefix());
			}

			return null;
		}

		/// <summary>
		/// Validate a variable reference.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(VarRef e)
		{
			QName @var = e.name();

			if (!expandVarQName(@var))
			{
				reportBadPrefix(@var.prefix());
			}

			if (!isVariableInScope(@var))
			{
				reportError(new StaticNameError(StaticNameError.NAME_NOT_FOUND));
			}

			if (getVariableType(@var) == null)
			{
				reportError(new StaticNameError(StaticNameError.NAME_NOT_FOUND));
			}

			// The variable is good. If it was not captured, it must be referring to an external var
			if (!isVariableCaptured(@var))
			{
				_freeVariables.Add(@var.asQName());
			}

			return null;
		}

		/// <summary>
		/// Validate a string literal.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(StringLiteral e)
		{
			return null;
		}

		/// <summary>
		/// Validate an integer literal.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(IntegerLiteral e)
		{
			return null;
		}

		/// <summary>
		/// Validate a double literal.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(DoubleLiteral e)
		{
			return null;
		}

		/// <summary>
		/// Validate a decimal literal.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(DecimalLiteral e)
		{
			return null;
		}

		/// <summary>
		/// Validate a parenthesized expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ParExpr e)
		{
			visitExprs(e.GetEnumerator());
			return null;
		}

		/// <summary>
		/// Validate a context item expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(CntxItemExpr e)
		{
			return null;
		}

		/// <summary>
		/// Validate a function call.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(FunctionCall e)
		{
			QName name = e.name();

			if (!expandFunctionQName(name))
			{
				reportBadPrefix(name.prefix());
			}

			javax.xml.@namespace.QName qName = name.asQName();
			Function f = _sc.resolveFunction(qName, e.arity());
			if (f == null)
			{
				reportError(new StaticFunctNameError("Function does not exist: " + name.@string() + " arity: " + e.arity()));
			}
			e.set_function(f);
			_resolvedFunctions.Add(qName);

			visitExprs(e.GetEnumerator());
			return null;
		}

		/// <summary>
		/// Validate a single type.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(SingleType e)
		{
			QName type = e.type();
			if (!expandItemTypeQName(type))
			{
				reportBadPrefix(type.prefix());
			}

			return null;
		}

		/// <summary>
		/// Validate a sequence type.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
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
		/// Validate an item type.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(ItemType e)
		{

			switch (e.type())
			{
			case ItemType.ITEM:
				break;
			case ItemType.QNAME:
				QName type = e.qname();
				if (!expandItemTypeQName(type))
				{
					reportBadPrefix(type.prefix());
				}

				if (BuiltinTypeLibrary.BUILTIN_TYPES.lookupType(e.qname().@namespace(), e.qname().local()) == null)
				{
					if (_sc.TypeModel == null || _sc.TypeModel.lookupType(e.qname().@namespace(), e.qname().local()) == null)
					{
						reportError(new StaticTypeNameError("Type not defined: " + e.qname().@string()));
					}
				}
				break;

			case ItemType.KINDTEST:
				e.kind_test().accept(this);
				break;
			}

			return null;
		}

		/// <summary>
		/// Validate an any kind test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(AnyKindTest e)
		{
			return null;
		}

		/// <summary>
		/// Validate a document test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
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

		/// <summary>
		/// Validate a text test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(TextTest e)
		{
			return null;
		}

		/// <summary>
		/// Validate a comment test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(CommentTest e)
		{
			return null;
		}

		/// <summary>
		/// Validate a processing instructing test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(PITest e)
		{
			string arg = e.arg();
			if (string.ReferenceEquals(arg, null))
			{
				arg = "";
			}

			return null;
		}

		/// <summary>
		/// Validate an attribute test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		// XXX NO CHECK ?
		public virtual object visit(AttributeTest e)
		{
			QName name = e.name();
			if (name != null)
			{
				if (!expandItemQName(name))
				{
					reportBadPrefix(name.prefix());
				}
			}

			name = e.type();
			if (name != null)
			{
				if (!expandItemTypeQName(name))
				{
					reportBadPrefix(name.prefix());
				}
			}
			return null;
		}

		/// <summary>
		/// Validate a schema attribute test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(SchemaAttrTest e)
		{
			QName name = e.arg();

			if (!expandItemQName(name))
			{
				reportBadPrefix(name.prefix());
			}

			if (_sc.TypeModel.lookupAttributeDeclaration(name.@namespace(), name.local()) == null)
			{
				reportError(new StaticAttrNameError("Attribute not decleared: " + name.@string()));
			}

			return null;
		}

		/// <summary>
		/// Validate an element test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		// XXX NO SEMANTIC CHECK?!
		public virtual object visit(ElementTest e)
		{
			if (e.name() != null)
			{
				if (!expandItemTypeQName(e.name()))
				{
					reportBadPrefix(e.name().prefix());
				}
			}

			if (e.type() != null)
			{
				if (!expandItemTypeQName(e.type()))
				{
					reportBadPrefix(e.type().prefix());
				}
			}
			return null;
		}

		/// <summary>
		/// Validate a schema element test.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(SchemaElemTest e)
		{
			QName elem = e.name();

			if (!expandItemQName(elem))
			{
				reportBadPrefix(elem.prefix());
			}

			if (_sc.TypeModel.lookupElementDeclaration(elem.@namespace(), elem.local()) == null)
			{
				reportError(new StaticElemNameError("Element not declared: " + elem.@string()));
			}
			return null;
		}

		private void visitCollExprs(IEnumerator i)
		{
			while (i.MoveNext())
			{
				ICollection exprs = (ICollection) i.Current;

				visitExprs(exprs.GetEnumerator());
			}
		}

		/// <summary>
		/// Validate an axis step.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(AxisStep e)
		{

			e.step().accept(this);

			visitCollExprs(e.GetEnumerator());
			return null;
		}

		/// <summary>
		/// Validate a filter expression.
		/// </summary>
		/// <param name="e">
		///            is the expression. </param>
		/// <returns> null. </returns>
		public virtual object visit(FilterExpr e)
		{
			e.primary().accept(this);
            visitCollExprs(e.GetEnumerator());
			return null;
		}

        public object visit(PostfixExpr e)
        {
            e.primary().accept(this);
            visitCollExprs(e.GetEnumerator());
            return null;
        }

		/// <summary>
		/// @since 2.0
		/// </summary>
		public virtual bool RootUsed
		{
			get
			{
				return _rootUsed;
			}
		}
	}

}