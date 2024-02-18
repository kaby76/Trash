using System.Diagnostics;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Andrea Bittau, University College London, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Andrea Bittau - initial API and implementation from the PsychoPath XPath 2.0
///     Mukul Gandhi - improved comparison of xs:string with other XDM types
///  Jesper S Moller - bug 286061   correct handling of quoted string 
///  Jesper S Moller - bug 280555 - Add pluggable collation support
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.types
{

	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using CmpEq = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpEq;
	using CmpGt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpGt;
	using CmpLt = org.eclipse.wst.xml.xpath2.processor.@internal.function.CmpLt;
	using FnCompare = org.eclipse.wst.xml.xpath2.processor.@internal.function.FnCompare;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// A representation of the String datatype
	/// </summary>
	public class XSString : CtrType, CmpEq, CmpGt, CmpLt
	{

		private const string XS_STRING = "xs:string";
		private string _value;

		/// <summary>
		/// Initialises using the supplied String
		/// </summary>
		/// <param name="x">
		///            The String to initialise to </param>
		public XSString(string x)
		{
			_value = x;
		}

		/// <summary>
		/// Initialises to null
		/// </summary>
		public XSString() : this(null)
		{
		}

		/// <summary>
		/// Retrieves the datatype's full pathname
		/// </summary>
		/// <returns> "xs:string" which is the datatype's full pathname </returns>
		public override string string_type()
		{
			return XS_STRING;
		}

		/// <summary>
		/// Retrieves the datatype's name
		/// </summary>
		/// <returns> "string" which is the datatype's name </returns>
		public override string type_name()
		{
			return "string";
		}

		/// <summary>
		/// Retrieves a String representation of the string stored. This method is
		/// functionally identical to value()
		/// </summary>
		/// <returns> The String stored </returns>
		public override string StringValue
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Retrieves a String representation of the string stored. This method is
		/// functionally identical to string_value()
		/// </summary>
		/// <returns> The String stored </returns>
		public virtual string value()
		{
			return StringValue;
		}

		/// <summary>
		/// Creates a new ResultSequence consisting of the extractable String in the
		/// supplied ResultSequence
		/// </summary>
		/// <param name="arg">
		///            The ResultSequence from which to extract the String </param>
		/// <returns> New ResultSequence consisting of the supplied String </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence constructor(org.eclipse.wst.xml.xpath2.api.ResultSequence arg) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence constructor(ResultSequence arg)
		{
			if (arg.empty())
			{
				return ResultBuffer.EMPTY;
			}

			//AnyAtomicType aat = (AnyAtomicType) arg.first();
			Item aat = arg.first();

			return new XSString(aat.StringValue);
		}

		// comparisons

		// 666 indicates death [compare returned empty seq]
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int do_compare(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dc) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		private int do_compare(AnyType arg, DynamicContext dc)
		{

			// XXX: This can't happen, I guess
			if (arg == null)
			{
				return 666;
			}

			XSString comparand = arg is XSString ? (XSString)arg : new XSString(arg.StringValue);

			System.Numerics.BigInteger result = FnCompare.compare_string(dc.CollationProvider.DefaultCollation, this, comparand, dc);

			return (int)result;
		}

		/// <summary>
		/// Equality comparison between this and the supplied representation which
		/// must be of type String
		/// </summary>
		/// <param name="arg">
		///            The representation to compare with </param>
		/// <returns> True if the two representation are of the same String. False
		///         otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean eq(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext dynamicContext) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool eq(AnyType arg, DynamicContext dynamicContext)
		{
			int cmp = do_compare(arg, dynamicContext);

			// XXX im not sure what to do here!!! because eq has to return
			// something i fink....
			if (cmp == 666)
			{
				Debug.Assert(false);
			}

			return cmp == 0;
		}

		/// <summary>
		/// Comparison between this and the supplied representation which must be of
		/// type String
		/// </summary>
		/// <param name="arg">
		///            The representation to compare with </param>
		/// <returns> True if this String is lexographically greater than that
		///         supplied. False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean gt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool gt(AnyType arg, DynamicContext context)
		{
			int cmp = do_compare(arg, context);

			Debug.Assert(cmp != 666);

			return cmp > 0;
		}

		/// <summary>
		/// Comparison between this and the supplied representation which must be of
		/// type String
		/// </summary>
		/// <param name="arg">
		///            The representation to compare with </param>
		/// <returns> True if this String is lexographically less than that supplied.
		///         False otherwise </returns>
		/// <exception cref="DynamicError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean lt(AnyType arg, org.eclipse.wst.xml.xpath2.api.DynamicContext context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public virtual bool lt(AnyType arg, DynamicContext context)
		{
			int cmp = do_compare(arg, context);

			Debug.Assert(cmp != 666);

			return cmp < 0;
		}
		public override TypeDefinition TypeDefinition
		{
			get
			{
				return BuiltinTypeLibrary.XS_STRING;
			}
		}

		public override object NativeValue
		{
			get
			{
				return _value;
			}
		}

	}

}