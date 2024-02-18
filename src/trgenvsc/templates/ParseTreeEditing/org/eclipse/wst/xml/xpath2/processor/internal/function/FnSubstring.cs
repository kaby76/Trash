using System;
using System.Collections;
using System.Text;

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
///     Mukul Gandhi - bug 273795 - improvements to function, substring
///     Jesper Steen Moeller - bug 285145 - implement full arity checking
///     David Carver - bug 282096 - improvements for surrogate handling 
///     Jesper Steen Moeller - bug 282096 - reimplemented to be surrogate sensitive
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSDouble = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSDouble;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using CodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator;
	using StringCodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.StringCodePointIterator;

	/// <summary>
	/// <para>
	/// Function to obtain a substring from a string.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:substring($sourceString as xs:string?, $startingLoc as xs:double)
	/// as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class returns the portion of the value of $sourceString beginning at the
	/// position indicated by the value of $startingLoc. The characters returned do
	/// not extend beyond $sourceString. If $startingLoc is zero or negative, only
	/// those characters in positions greater than zero are returned.
	/// </para>
	/// 
	/// <para>
	/// If the value of $sourceString is the empty sequence, the zero-length string
	/// is returned.
	/// </para>
	/// 
	/// <para>
	/// The first character of a string is located at position 1, not position 0.
	/// </para>
	/// </summary>
	public class FnSubstring : Function
	{

		/// <summary>
		/// Constructor for FnSubstring
		/// </summary>
		public FnSubstring() : base(new QName("substring"), 2, 3)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the substring obtained from the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return substring(args);
		}

        /// <summary>
        /// Obtain a substring from the arguments.
        /// </summary>
        /// <param name="args">
        ///            are used to obtain a substring. </param>
        /// <exception cref="DynamicError">
        ///             Dynamic error. </exception>
        /// <returns> The result of obtaining a substring from the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence substring(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
        public static ResultSequence substring(ICollection args)
        {
            ICollection cargs = Function.convert_arguments(args, expected_args(args));

            IEnumerator argi = cargs.GetEnumerator();
            argi.MoveNext();
            ResultSequence stringArg = (ResultSequence) argi.Current;
            argi.MoveNext();
            ResultSequence startPosArg = (ResultSequence) argi.Current;
            ResultSequence lengthArg = null;

            if (argi.MoveNext())
            {
                lengthArg = (ResultSequence) argi.Current;
            }

            if (stringArg.empty())
            {
                return emptyString();
            }

            string str = ((XSString) stringArg.first()).value();
            double dstart = ((XSDouble) startPosArg.first()).double_value();

            // is start is NaN, no chars are returned
            if (double.IsNaN(dstart) || double.NegativeInfinity == dstart)
            {
                return emptyString();
            }

            double x = Math.Round(dstart);
            long istart = (long) x;

            long ilength = long.MaxValue;
            if (lengthArg != null)
            {
                double dlength = ((XSDouble) lengthArg.first()).double_value();
                if (double.IsNaN(dlength))
                {
                    return emptyString();
                }

                // Switch to the rounded kind
                double y = Math.Round(dlength);
                ilength = (long) y;
                if (ilength <= 0)
                {
                    return emptyString();
                }
            }


            // could guess too short in cases supplementary chars 
            StringBuilder sb = new StringBuilder((int) Math.Min(str.Length, ilength));

            // This looks like an inefficient way to iterate, but due to surrogate handling,
            // string indexes are no good here. Welcome to UTF-16!

            CodePointIterator strIter = new StringCodePointIterator(str);
            for (long p = 1;
                strIter.current() != org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator_Fields.DONE;
                ++p, strIter.next())
            {
                if (istart <= p && p - istart < ilength)
                {
                    sb.Append((char)strIter.current());
                }
            }

            return new XSString(sb.ToString());
        }

        private static ResultSequence emptyString()
		{
			return new XSString("");
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args(ICollection actualArgs)
		{
			var _expected_args = new ArrayList();

			_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
			_expected_args.Add(new SeqType(new XSDouble(), SeqType.OCC_NONE));

			// for arity 3
			if (actualArgs.Count == 3)
			{
			  _expected_args.Add(new SeqType(new XSDouble(), SeqType.OCC_NONE));
			}

			return _expected_args;
		}
	}

}