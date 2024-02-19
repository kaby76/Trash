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
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Function to join strings together.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:string-join($arg1 as xs:string*, $arg2 as xs:string) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class returns a xs:string created by concatenating the members of the
	/// $arg1 sequence using $arg2 as a separator. If the value of $arg2 is the
	/// zero-length string, then the members of $arg1 are concatenated without a
	/// separator.
	/// </para>
	/// 
	/// <para>
	/// If the value of $arg1 is the empty sequence, the zero-length string is
	/// returned.
	/// </para>
	/// </summary>
	public class FnStringJoin : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnStringJoin
		/// </summary>
		public FnStringJoin() : base(new QName("string-join"), 2)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the joining of the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return string_join(args);
		}

		/// <summary>
		/// Join the arguments.
		/// </summary>
		/// <param name="args">
		///            are joined. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of the arguments being joined together. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence string_join(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence string_join(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			IEnumerator argi = cargs.GetEnumerator();
            argi.MoveNext();
            ResultSequence arg1 = (ResultSequence) argi.Current;
            argi.MoveNext();
            ResultSequence arg2 = (ResultSequence) argi.Current;

			string result = "";
			string separator = ((XSString) arg2.first()).value();

			StringBuilder buf = new StringBuilder();
            bool first = false;
			for (var i = arg1.iterator(); i.MoveNext();)
            {
                if (!first)
                {
                    buf.Append(separator);
                }
                first = false;
				XSString item = (XSString) i.Current;
				buf.Append(item.value());
            }

			result = buf.ToString();
			return new XSString(result);
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnStringJoin))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_STAR));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}