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
///     David Carver (STAR) - bug 262765 - Added FnDefaultCollation.
///     David Carver (STAR) - bug 285321 - implemented fn:encode-for-uri()
///     Jesper Moller       - bug 287369 - Support fn:codepoint-equal()
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.function
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
	using FnNot = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNot;
	using FnNodeName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNodeName;
	using FnNilled = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNilled;
	using FnString = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnString;
	using FnBaseUri = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnBaseUri;
	using FnStaticBaseUri = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnStaticBaseUri;
	using FnDocumentUri = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDocumentUri;
	using FnError = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnError;
	using FnTrace = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTrace;
	using FnAbs = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnAbs;
	using FnCeiling = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCeiling;
	using FnFloor = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnFloor;
	using FnRound = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnRound;
	using FnRoundHalfToEven = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnRoundHalfToEven;
    using FnCodepointsToString = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCodepointsToString;
    using FnStringToCodepoints = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnStringToCodepoints;
    using FnCompare = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCompare;
    using FnCodepointEqual = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCodepointEqual;
    using FnConcat = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnConcat;
    using FnStringJoin = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnStringJoin;
    using FnSubstring = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSubstring;
    using FnStringLength = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnStringLength;
    using FnNormalizeSpace = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNormalizeSpace;
    using FnNormalizeUnicode = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNormalizeUnicode;
    using FnUpperCase = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnUpperCase;
    using FnLowerCase = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnLowerCase;
    using FnTranslate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTranslate;
    using FnEscapeHTMLUri = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnEscapeHTMLUri;
    using FnIriToURI = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnIriToURI;
    using FnContains = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnContains;
    using FnStartsWith = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnStartsWith;
    using FnEndsWith = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnEndsWith;
    using FnSubstringBefore = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSubstringBefore;
    using FnSubstringAfter = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSubstringAfter;
    using FnMatches = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMatches;
    using FnReplace = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnReplace;
    using FnTokenize = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTokenize;
    using FnEncodeForURI = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnEncodeForURI;
    using FnResolveURI = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnResolveURI;
    using FnTrue = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTrue;
    using FnFalse = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnFalse;
    using FnYearsFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnYearsFromDuration;
    using FnMonthsFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMonthsFromDuration;
    using FnDaysFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDaysFromDuration;
    using FnHoursFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnHoursFromDuration;
    using FnMinutesFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMinutesFromDuration;
    using FnSecondsFromDuration = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSecondsFromDuration;
    using FnYearFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnYearFromDateTime;
    using FnMonthFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMonthFromDateTime;
    using FnDayFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDayFromDateTime;
    using FnHoursFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnHoursFromDateTime;
    using FnMinutesFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMinutesFromDateTime;
    using FnSecondsFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSecondsFromDateTime;
    using FnTimezoneFromDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTimezoneFromDateTime;
    using FnYearFromDate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnYearFromDate;
    using FnMonthFromDate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMonthFromDate;
    using FnDayFromDate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDayFromDate;
    using FnTimezoneFromDate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTimezoneFromDate;
    using FnHoursFromTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnHoursFromTime;
    using FnMinutesFromTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMinutesFromTime;
    using FnSecondsFromTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSecondsFromTime;
    using FnTimezoneFromTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnTimezoneFromTime;
    using FnDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDateTime;
    using FnImplicitTimezone = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnImplicitTimezone;
    using FnAdjustDateTimeToTimeZone = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnAdjustDateTimeToTimeZone;
    using FnAdjustTimeToTimeZone = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnAdjustTimeToTimeZone;
    using FnAdjustDateToTimeZone = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnAdjustDateToTimeZone;
    using FnResolveQName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnResolveQName;
    using FnQName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnQName;
    using FnLocalNameFromQName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnLocalNameFromQName;
    using FnNamespaceUriFromQName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNamespaceUriFromQName;
    using FnPrefixFromQName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnPrefixFromQName;
    using FnName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnName;
    using FnLocalName = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnLocalName;
    using FnNamespaceUri = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNamespaceUri;
    using FnNumber = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnNumber;
    using FnInScopePrefixes = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnInScopePrefixes;
    using FnLang = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnLang;
    using FnIndexOf = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnIndexOf;
    using FnEmpty = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnEmpty;
    using FnExists = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnExists;
    using FnDistinctValues = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDistinctValues;
    using FnInsertBefore = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnInsertBefore;
    using FnRemove = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnRemove;
    using FnReverse = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnReverse;
    using FnSubsequence = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSubsequence;
    using FnUnordered = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnUnordered;
    using FnZeroOrOne = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnZeroOrOne;
    using FnOneOrMore = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnOneOrMore;
    using FnExactlyOne = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnExactlyOne;
    using FnDeepEqual = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDeepEqual;
    using FnCount = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCount;
    using FnAvg = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnAvg;
    using FnMax = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnMax;
    using FnSum = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnSum;
    using FnDoc = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDoc;
    using FnCollection = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCollection;
    using FnPosition = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnPosition;
    using FnLast = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnLast;
    using FnCurrentDateTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCurrentDateTime;
    using FnCurrentDate = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCurrentDate;
    using FnCurrentTime = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCurrentTime;
    using FnDefaultCollation = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnDefaultCollation;
    using FnID = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnID;
    using FnIDREF = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnIDREF;


	// this is the equivalent of libc =D
	/// <summary>
	/// Maintains a library of core functions and user defined functions.
	/// </summary>
	public class FnFunctionLibrary : FunctionLibrary
	{
		/// <summary>
		/// Path to xpath functions specification.
		/// </summary>
		public const string XPATH_FUNCTIONS_NS = "http://www.w3.org/2005/xpath-functions";

		/// <summary>
		/// Constructor for FnFunctionLibrary.
		/// </summary>
		public FnFunctionLibrary() : base(XPATH_FUNCTIONS_NS)
		{

			// add functions here
			add_function(new FnBoolean());
			add_function(new FnRoot());
			add_function(new FnNot());

			// accessors
			add_function(new FnNodeName());
			add_function(new FnNilled());
			add_function(new FnData());
			add_function(new FnString());
			add_function(new FnBaseUri());
			add_function(new FnStaticBaseUri());
			add_function(new FnDocumentUri());

			// error
			add_function(new FnError());

			// trace
			add_function(new FnTrace());

			// numeric functions
			add_function(new FnAbs());
			add_function(new FnCeiling());
			add_function(new FnFloor());
			add_function(new FnRound());
			add_function(new FnRoundHalfToEven());

			// string functions
			add_function(new FnCodepointsToString());
			add_function(new FnStringToCodepoints());
			add_function(new FnCompare());
			add_function(new FnCodepointEqual());
			add_function(new FnConcat());
			add_function(new FnStringJoin());
			add_function(new FnSubstring());
			add_function(new FnStringLength());
			add_function(new FnNormalizeSpace());
			add_function(new FnNormalizeUnicode());
			add_function(new FnUpperCase());
			add_function(new FnLowerCase());
			add_function(new FnTranslate());
			add_function(new FnEscapeHTMLUri());
			add_function(new FnIriToURI());
			add_function(new FnContains());
			add_function(new FnStartsWith());
			add_function(new FnEndsWith());
			add_function(new FnSubstringBefore());
			add_function(new FnSubstringAfter());
			add_function(new FnMatches());
			add_function(new FnReplace());
			add_function(new FnTokenize());
			add_function(new FnEncodeForURI());
			add_function(new FnResolveURI());

			// boolean functions
			add_function(new FnTrue());
			add_function(new FnFalse());

			// date extraction functions
			add_function(new FnYearsFromDuration());
			add_function(new FnMonthsFromDuration());
			add_function(new FnDaysFromDuration());
			add_function(new FnHoursFromDuration());
			add_function(new FnMinutesFromDuration());
			add_function(new FnSecondsFromDuration());
			add_function(new FnYearFromDateTime());
			add_function(new FnMonthFromDateTime());
			add_function(new FnDayFromDateTime());
			add_function(new FnHoursFromDateTime());
			add_function(new FnMinutesFromDateTime());
			add_function(new FnSecondsFromDateTime());
			add_function(new FnTimezoneFromDateTime());
			add_function(new FnYearFromDate());
			add_function(new FnMonthFromDate());
			add_function(new FnDayFromDate());
			add_function(new FnTimezoneFromDate());
			add_function(new FnHoursFromTime());
			add_function(new FnMinutesFromTime());
			add_function(new FnSecondsFromTime());
			add_function(new FnTimezoneFromTime());
			add_function(new FnDateTime());

			// timezone functs
			add_function(new FnImplicitTimezone());
			add_function(new FnAdjustDateTimeToTimeZone());
			add_function(new FnAdjustTimeToTimeZone());
			add_function(new FnAdjustDateToTimeZone());

			// QName functs
			add_function(new FnResolveQName());
			add_function(new FnQName());
			add_function(new FnLocalNameFromQName());
			add_function(new FnNamespaceUriFromQName());
			add_function(new FnPrefixFromQName());

			// XXX implement hex & binary & notations

			// node functions
			add_function(new FnName());
			add_function(new FnLocalName());
			add_function(new FnNamespaceUri());
			add_function(new FnNumber());
			add_function(new FnInScopePrefixes());

			// node functs
			add_function(new FnLang());

			// sequence functions
			add_function(new FnIndexOf());
			add_function(new FnEmpty());
			add_function(new FnExists());
			add_function(new FnDistinctValues());
			add_function(new FnInsertBefore());
			add_function(new FnRemove());
			add_function(new FnReverse());
			add_function(new FnSubsequence());
			add_function(new FnUnordered());

			// sequence caridnality
			add_function(new FnZeroOrOne());
			add_function(new FnOneOrMore());
			add_function(new FnExactlyOne());

			add_function(new FnDeepEqual());

			// aggregate functions
			add_function(new FnCount());
			add_function(new FnAvg());
			add_function(new FnMax());
			add_function(new FnMax());
			add_function(new FnSum());

			// XXX implement functions that generate sequences
			add_function(new FnDoc());
			add_function(new FnCollection());

			// context functions
			add_function(new FnPosition());
			add_function(new FnLast());
			add_function(new FnCurrentDateTime());
			add_function(new FnCurrentDate());
			add_function(new FnCurrentTime());

			// XXX collation
			add_function(new FnDefaultCollation());

			// ID and IDRef
			add_function(new FnID());
			add_function(new FnIDREF());

		}
	}

}