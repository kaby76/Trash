using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
///     Mukul Gandhi - bug 273719 - String Length does not work with Element arg.
///     Mukul Gandhi - bug 273795 - improvements to function, substring (implemented
///                                 numeric type promotion). 
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     Jesper Steen Moeller - bug 281159 - implement xs:anyUri -> xs:string promotion
///     Jesper Steen Moller  - bug 281938 - undefined context should raise error
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{

	using DatatypeFactory = javax.xml.datatype.DatatypeFactory;
    using DatatypeConfigurationException = javax.xml.datatype.DatatypeConfigurationException;

	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using Item = org.eclipse.wst.xml.xpath2.api.Item;
	using ResultBuffer = org.eclipse.wst.xml.xpath2.api.ResultBuffer;
	using TypeDefinition = org.eclipse.wst.xml.xpath2.api.typesystem.TypeDefinition;
	using AnyAtomicType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyAtomicType;
	using AnyType = org.eclipse.wst.xml.xpath2.processor.@internal.types.AnyType;
	using NumericType = org.eclipse.wst.xml.xpath2.processor.@internal.types.NumericType;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSAnyURI = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSAnyURI;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using XSUntypedAtomic = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSUntypedAtomic;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;

	/// <summary>
	/// Support for functions.
	/// </summary>
	public abstract class Function : org.eclipse.wst.xml.xpath2.api.Function
	{

		protected internal static DatatypeFactory _datatypeFactory;
		static Function()
		{
			try
			{
				_datatypeFactory = DatatypeFactory.newInstance();
			}
			catch (DatatypeConfigurationException e)
			{
				throw new Exception("Cannot initialize XML datatypes", e);
			}
		}

		protected internal QName _name;
		/// <summary>
		/// if negative, need to have "at least"
		/// </summary>
		protected internal int _min_arity;

		/// <summary>
		/// If "at least", this speci, unlimited if -1
		/// </summary>
		protected internal int _max_arity;

		/// <summary>
		/// Constructor for Function.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="arity">
		///            the arity of a specific function. </param>
		public Function(QName name, int arity)
		{
			_name = name;
			if (arity < 0)
			{
				throw new Exception("We want to avoid this!");
			}
			_min_arity = arity;
			_max_arity = arity;
		}

		/// <summary>
		/// Constructor for Function.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="arity">
		///            the arity of a specific function. </param>
		public Function(QName name, int min_arity, int max_arity)
		{
			_name = name;
			if (min_arity < 0 || max_arity < 0 || max_arity < min_arity)
			{
				throw new Exception("We want to avoid this!");
			}
			_min_arity = min_arity;
			_max_arity = max_arity;
		}

		/// <summary>
		/// Support for QName interface.
		/// </summary>
		/// <returns> Result of QName operation. </returns>
		public virtual QName name()
		{
			return _name;
		}

		/// <summary>
		/// Minimal number of allowed arguments.
		/// </summary>
		/// <returns> The smallest number of erguments possible </returns>
		public virtual int min_arity()
		{
			return _min_arity;
		}

		/// <summary>
		/// Maximum number of allowed arguments.
		/// </summary>
		/// <returns> The highest number of erguments possible </returns>
		public virtual int max_arity()
		{
			return _max_arity;
		}

		/// <summary>
		/// Checks if this function has an to the
		/// </summary>
		/// <param name="actual_arity">
		/// @return </param>
		public virtual bool matches_arity(int actual_arity)
		{
			if (actual_arity < min_arity())
			{
				return false;
			}
			if (actual_arity > max_arity())
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Default constructor for signature.
		/// </summary>
		/// <returns> Signature. </returns>
		public virtual string signature()
		{
			return signature(this);
		}

		/// <summary>
		/// Obtain the function name and arity from signature.
		/// </summary>
		/// <param name="f">
		///            current function. </param>
		/// <returns> Signature. </returns>
		public static string signature(Function f)
		{
			return signature(f.name(), f.is_vararg() ? -1 : f.min_arity());
		}

		/// <summary>
		/// Apply the name and arity to signature.
		/// </summary>
		/// <param name="name">
		///            QName. </param>
		/// <param name="arity">
		///            arity of the function. </param>
		/// <returns> Signature. </returns>
		public static string signature(QName name, int arity)
		{
			string n = name.expanded_name();
			if (string.ReferenceEquals(n, null))
			{
				return null;
			}

			n += "_";

			if (arity < 0)
			{
				n += "x";
			}
			else
			{
				n += arity;
			}

			return n;
		}

		/// <summary>
		/// Evaluate arguments.
		/// </summary>
		/// <param name="args">
		///            argument expressions. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Result of evaluation. </returns>
		public virtual ResultSequence evaluate(ICollection args)
		{
			throw new System.NotSupportedException();
		}

		// convert argument according to section 3.1.5 of xpath 2.0 spec
		/// <summary>
		/// Convert the input argument according to section 3.1.5 of specification.
		/// </summary>
		/// <param name="arg">
		///            input argument. </param>
		/// <param name="expected">
		///            Expected Sequence type. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Converted argument. </returns>
		public static org.eclipse.wst.xml.xpath2.api.ResultSequence convert_argument(org.eclipse.wst.xml.xpath2.api.ResultSequence arg, SeqType expected)
		{
			ResultBuffer result = new ResultBuffer();

			// XXX: Should use type_class instead and use item.getClass().isAssignableTo(expected.type_class())
			AnyType expected_type = expected.type();

			// expected is atomic
			if (expected_type is AnyAtomicType)
			{
				AnyAtomicType expected_aat = (AnyAtomicType) expected_type;

				// atomize
				org.eclipse.wst.xml.xpath2.api.ResultSequence rs = FnData.atomize(arg);

				// cast untyped to expected type
				for (var i = rs.iterator(); i.MoveNext(); )
				{
					AnyType item = (AnyType) i.Current;

					if (item is XSUntypedAtomic)
					{
						// create a new item of the expected
						// type initialized with from the string
						// value of the item
						ResultSequence converted = null;
						if (expected_aat is XSString)
						{
						   XSString strType = new XSString(item.StringValue);
						   converted = ResultSequenceFactory.create_new(strType);
						}
						else
						{
						   converted = ResultSequenceFactory.create_new(item);
						}

						result.concat(converted);
					}
					// xs:anyURI promotion to xs:string
					else if (item is XSAnyURI && expected_aat is XSString)
					{
						result.add(new XSString(item.StringValue));
					}
					// numeric type promotion
					else if (item is NumericType)
					{
						if (expected_aat is XSDouble)
						{
						  XSDouble doubleType = new XSDouble(item.StringValue);
						  result.add(doubleType);
						}
						else
						{
						  result.add(item);
						}
					}
					else
					{
						result.add(item);
					}
				}
				// do sequence type matching on converted arguments
				return expected.match(result.Sequence);
			}
			else
			{
				// do sequence type matching on converted arguments
				return expected.match(arg);
			}
		}

		// convert arguments
		// returns collection of arguments
		/// <summary>
		/// Convert arguments.
		/// </summary>
		/// <param name="args">
		///            input arguments. </param>
		/// <param name="expected">
		///            expected arguments. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> Converted arguments. </returns>
		public static ICollection convert_arguments(ICollection args, ICollection expected)
		{
			var result = new ArrayList();

			Debug.Assert(args.Count <= expected.Count);

			IEnumerator argi = args.GetEnumerator();
			IEnumerator expi = expected.GetEnumerator();

			// convert all arguments
			while (argi.MoveNext() && expi.MoveNext())
			{
				result.Add(convert_argument(
                    (org.eclipse.wst.xml.xpath2.api.ResultSequence) argi.Current,
                    (SeqType) expi.Current));
			}

			return result;
		}

		protected internal static ResultSequence getResultSetForArityZero(EvaluationContext ec)
		{
			ResultSequence rs = ResultSequenceFactory.create_new();

			Item contextItem = ec.ContextItem;
			if (contextItem != null)
			{
			  // if context item is defined, then that is the default argument
			  // to fn:string function
			  rs.add(new XSString(contextItem.StringValue));
			}
			else
			{
				throw DynamicError.contextUndefined();
			}
			return rs;
		}

		public virtual bool is_vararg()
		{
			return _min_arity != _max_arity;
		}

		public virtual string Name
		{
			get
			{
				return name().local();
			}
		}

		public virtual int MinArity
		{
			get
			{
				return min_arity();
			}
		}

		public virtual int MaxArity
		{
			get
			{
				return max_arity();
			}
		}

		public virtual bool VariableArgument
		{
			get
			{
				return this.is_vararg();
			}
		}

		public virtual bool canMatchArity(int actualArity)
		{
			return matches_arity(actualArity);
		}

		public virtual TypeDefinition ResultType
		{
			get
			{
				return BuiltinTypeLibrary.XS_UNTYPED;
			}
		}

		public virtual TypeDefinition getArgumentType(int index)
		{
			return BuiltinTypeLibrary.XS_UNTYPED;
		}

		public virtual string getArgumentNameHint(int index)
		{
			return "argument_" + index;
		}

        public api.ResultSequence evaluate(ICollection<api.ResultSequence> args, EvaluationContext evaluationContext)
        {
            var collection = args.ToList();
            var result = this.evaluate((ICollection)collection, evaluationContext);
            return result;
        }

        public TypeDefinition computeReturnType(ICollection<TypeDefinition> args, api.StaticContext sc)
        {
            throw new NotImplementedException();
        }

        public virtual TypeDefinition computeReturnType(ICollection args, org.eclipse.wst.xml.xpath2.api.StaticContext sc)
		{
			return BuiltinTypeLibrary.XS_UNTYPED;
		}

		public virtual org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(ICollection args, EvaluationContext evaluationContext)
		{

			ResultSequence result = evaluate(args);
			return result;
		}

	}

}