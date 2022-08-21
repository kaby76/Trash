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
///     Jesper Steen Moeller - bug 282096 - clean up string storage and make
///                                         translate function surrogate aware
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;
	using CodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator;
	using StringCodePointIterator = org.eclipse.wst.xml.xpath2.processor.@internal.utils.StringCodePointIterator;


	/// <summary>
	/// <para>
	/// Translation function.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:translate($arg as xs:string?, $mapString as xs:string, $transString
	/// as xs:string) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This class returns the value of $arg modified so that every character in the
	/// value of $arg that occurs at some position N in the value of $mapString has
	/// been replaced by the character that occurs at position N in the value of
	/// $transString.
	/// </para>
	/// 
	/// <para>
	/// If the value of $arg is the empty sequence, the zero-length string is
	/// returned.
	/// </para>
	/// 
	/// <para>
	/// Every character in the value of $arg that does not appear in the value of
	/// $mapString is unchanged.
	/// </para>
	/// 
	/// <para>
	/// Every character in the value of $arg that appears at some position M in the
	/// value of $mapString, where the value of $transString is less than M
	/// characters in length, is omitted from the returned value. If $mapString is
	/// the zero-length string $arg is returned.
	/// </para>
	/// 
	/// <para>
	/// If a character occurs more than once in $mapString, then the first occurrence
	/// determines the replacement character. If $transString is longer than
	/// $mapString, the excess characters are ignored.
	/// </para>
	/// </summary>
	public class FnTranslate : Function
	{
		private static ArrayList _expected_args = null;

		/// <summary>
		/// Constructor for FnTranslate.
		/// </summary>
		public FnTranslate() : base(new QName("translate"), 3)
		{
		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the arguments being translated. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec)
		{
			return translate(args);
		}

		/// <summary>
		/// Translate arguments.
		/// </summary>
		/// <param name="args">
		///            are translated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of translating the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence translate(java.util.Collection args) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public static ResultSequence translate(ICollection args)
		{
			ICollection cargs = Function.convert_arguments(args, expected_args());

			IEnumerator argi = cargs.GetEnumerator();
            argi.MoveNext();
            ResultSequence arg1 = (ResultSequence) argi.Current;
            argi.MoveNext();
			ResultSequence arg2 = (ResultSequence) argi.Current;
            argi.MoveNext();
            ResultSequence arg3 = (ResultSequence) argi.Current;

			if (arg1.empty())
			{
				return new XSString("");
			}

			string str = ((XSString) arg1.first()).value();
			string mapstr = ((XSString) arg2.first()).value();
			string transstr = ((XSString) arg3.first()).value();

			IDictionary replacements = buildReplacementMap(mapstr, transstr);

			StringBuilder sb = new StringBuilder(str.Length);
			CodePointIterator strIter = new StringCodePointIterator(str);
			for (int input = strIter.current(); input != org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator_Fields.DONE; input = strIter.next())
			{
				int? inputCodepoint = new int?(input);
				if (replacements.Contains(inputCodepoint))
				{
					int? replaceWith = (int?)replacements[inputCodepoint];
					if (replaceWith != null)
                    {
                        var c = (char) replaceWith.Value;
						sb.Append(c);
					}
				}
				else
				{
                    var c = (char)input;
                    sb.Append(c);
				}
			}

			return new XSString(sb.ToString());
		}

		/// <summary>
		/// Build a replacement map from the mapstr and the transstr for translation. The function returns a Map<Integer, Integer> mapping each codepoint
		/// mentioned in the mapstr into the corresponding codepoint in transstr, or null if there is no matching mapping in transstr.
		/// </summary>
		/// <param name="mapstr"> The "mapping from" string </param>
		/// <param name="transstr"> The "mapping into" string </param>
		/// <returns> A map which maps input codepoint to output codepoint (or null) </returns>
		private static IDictionary buildReplacementMap(string mapstr, string transstr)
		{
			// Build mapping (map from codepoint -> codepoint)		
			IDictionary replacements = new Hashtable(mapstr.Length * 4);

			CodePointIterator mapIter = new StringCodePointIterator(mapstr);
			CodePointIterator transIter = new StringCodePointIterator(transstr);
			// Iterate through both mapIter and transIter and produce the mapping
			int mapFrom = mapIter.current();
			int mapTo = transIter.current();
			while (mapFrom != org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator_Fields.DONE)
			{
				int? codepointFrom = new int?(mapFrom);
				if (!replacements.Contains(codepointFrom))
				{
					// only overwrite if it doesn't exist already
					int? replacement = mapTo != org.eclipse.wst.xml.xpath2.processor.@internal.utils.CodePointIterator_Fields.DONE ? new int?(mapTo) : null;
					replacements[codepointFrom] = replacement;
				}
				mapFrom = mapIter.next();
				mapTo = transIter.next();
			}
			return replacements;
		}

		/// <summary>
		/// Calculate the expected arguments.
		/// </summary>
		/// <returns> The expected arguments. </returns>
		public static ICollection expected_args()
		{
			lock (typeof(FnTranslate))
			{
				if (_expected_args == null)
				{
					_expected_args = new ArrayList();
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
					_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
				}
        
				return _expected_args;
			}
		}
	}

}