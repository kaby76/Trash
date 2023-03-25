using System.Diagnostics;
using System.Collections;
using System.Threading;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2005, 2011 Jesper Steen Moeller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Steen Moeller - bug 285152 - implement fn:normalize-unicocde
///     Jesper Steen Moller  - bug 290337 - Revisit use of ICU
///     Mukul Gandhi - bug 280798 - PsychoPath support for JDK 1.4
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.processor.@internal.function
{


	using DynamicContext = org.eclipse.wst.xml.xpath2.api.DynamicContext;
	using EvaluationContext = org.eclipse.wst.xml.xpath2.api.EvaluationContext;
	using ResultSequence = org.eclipse.wst.xml.xpath2.api.ResultSequence;
	using QName = org.eclipse.wst.xml.xpath2.processor.@internal.types.QName;
	using XSString = org.eclipse.wst.xml.xpath2.processor.@internal.types.XSString;

	/// <summary>
	/// <para>
	/// Function to normalize unicode.
	/// </para>
	/// 
	/// <para>
	/// Usage: fn:normalize-unicode($arg as xs:string?) as xs:string
	/// or fn:normalize-unicode($arg as xs:string?, $normalization-form as xs:string) as xs:string
	/// </para>
	/// 
	/// <para>
	/// This function returns the normalized value of the first argument in the normalization form specified by the second argument.
	/// </para>
	/// 
	/// </summary>
	public class FnNormalizeUnicode : Function
	{
		//private static ArrayList _expected_args = null;
		//private static W3CNormalizer normalizer = null;

		/// <summary>
		/// Constructor for FnNormalizeUnicode
		/// </summary>
		public FnNormalizeUnicode() : base(new QName("normalize-unicode"), 1, 2)
		{
		}

		/// <summary>
		/// The common interface which normalizers must adhere to
		/// </summary>
		public interface W3CNormalizer
		{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String normalize(String argument, String normalizationForm) throws org.eclipse.wst.xml.xpath2.processor.DynamicError;
			string normalize(string argument, string normalizationForm);
		}

		/// <summary>
		/// W3C normalizer implemented via IBM's ICU
		/// </summary>
//		internal class ICUNormalizer : W3CNormalizer
//		{
//			internal bool InstanceFieldsInitialized = false;

//			public ICUNormalizer()
//			{
//				if (!InstanceFieldsInitialized)
//				{
//					InitializeInstanceFields();
//					InstanceFieldsInitialized = true;
//				}
//			}

//			internal virtual void InitializeInstanceFields()
//			{
//				// Can't handle "FULLY-NORMALIZED" yet
                
//				modeMap["NFC"] = Normalizer.NFC;
//				modeMap["NFD"] = Normalizer.NFD;
//				modeMap["NFKC"] = Normalizer.NFKC;
//				modeMap["NFKD"] = Normalizer.NFKD;
//			}


//			internal IDictionary modeMap = new Hashtable();

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public String normalize(String argument, String normalizationForm) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
//			public virtual string normalize(string argument, string normalizationForm)
//			{
//				Normalizer.Mode mode = (Normalizer.Mode)modeMap[normalizationForm];
//				if (mode != null)
//				{
//					return Normalizer.normalize(argument, mode);
//				}
//				else
//				{
//					throw DynamicError.unsupported_normalization_form(normalizationForm);
//				}
//			}
//		}

		/*
		static class JDK6Normalizer implements W3CNormalizer {
			private Method normalizeMethod;
			private Map formMap = new HashMap();
	
			public JDK6Normalizer(Class normalizerCls, Class formCls) throws SecurityException, NoSuchMethodException {
				this.normalizeMethod = normalizerCls.getMethod("normalize", CharSequence.class, formCls);
				Enum[] formConstants = formCls.getEnumConstants();
				for (Enum form : formConstants) {
					formMap.put(form.name(), form);
				}
				// Can't handle "FULLY-NORMALIZED" yet
			}
			
			public String normalize(String argument, String normalizationForm)
					throws DynamicError {
	
				//if (normalizationForm.equals("FULLY-NORMALIZED")) {
				//   We can't handle this one yet
				// }
				Enum form = formMap.get(normalizationForm);
				if (form != null) {
					try {
						return (String)normalizeMethod.invoke(null, argument, form);
					} catch (RuntimeException e) {
						throw DynamicError.runtime_error("java.text.Normalizer.normalize(..., \"" + normalizationForm + "\")", e);
					} catch (IllegalAccessException e) {
						throw DynamicError.runtime_error("java.text.Normalizer.normalize(..., \"" + normalizationForm + "\")", e);
					} catch (InvocationTargetException e) {
						throw DynamicError.runtime_error("java.text.Normalizer.normalize(..., \"" + normalizationForm + "\")", e);
					}
				} else {
					throw DynamicError.unsupported_normalization_form(normalizationForm);
				}			
			}
		}
		*/

//		internal class FailingNormalizer : W3CNormalizer
//		{

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public String normalize(String argument, String normalizationForm) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
//			public virtual string normalize(string argument, string normalizationForm)
//			{
//				throw DynamicError.unsupported_normalization_form("Can't normalize to form " + normalizationForm + ": No ICU Library or Java 6 found. 'normalize-unicode' requires either 'com.ibm.icu.text.Normalizer' or 'java.text.Normalizer' on the classpath");
//			}
//		}

		/// <summary>
		/// Evaluate the arguments.
		/// </summary>
		/// <param name="args">
		///            are evaluated. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The evaluation of the space in the arguments being normalized. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.eclipse.wst.xml.xpath2.api.ResultSequence evaluate(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.EvaluationContext ec) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
		public override ResultSequence evaluate(ICollection args, EvaluationContext ec)
        {
            return null;
			//return normalize_unicode(args, ec.DynamicContext);
		}

		/// <summary>
		/// Normalize unicode codepoints in the argument.
		/// </summary>
		/// <param name="args">
		///            are used to obtain the input string and optionally the normalization type from. </param>
		/// <exception cref="DynamicError">
		///             Dynamic error. </exception>
		/// <returns> The result of normalizing the space in the arguments. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.eclipse.wst.xml.xpath2.api.ResultSequence normalize_unicode(java.util.Collection args, org.eclipse.wst.xml.xpath2.api.DynamicContext d_context) throws org.eclipse.wst.xml.xpath2.processor.DynamicError
//		public static ResultSequence normalize_unicode(ICollection args, DynamicContext d_context)
//		{
//			Debug.Assert(args.Count >= 1 && args.Count <= 2);

//			ICollection cargs = Function.convert_arguments(args, expected_args());

//			IEnumerator cargsIterator = cargs.GetEnumerator();
////JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//			ResultSequence arg1 = (ResultSequence) cargsIterator.next();


//			string normalizationType = "NFC";
////JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//			if (cargsIterator.hasNext())
//			{
////JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
//				ResultSequence arg2 = (ResultSequence)cargsIterator.next();
//				// Trim and convert to upper as per the spec
//				if (arg2.empty())
//				{
//					normalizationType = "";
//				}
//				else
//				{
//					normalizationType = ((XSString) arg2.first()).value().Trim().ToUpper();
//				}
//			}

//			string argument = "";
//			if (!arg1.empty())
//			{
//				argument = ((XSString) arg1.first()).value();
//			}

//			string normalized = normalizationType.Equals("") ? argument : Normalizer.normalize(argument, normalizationType);
//			return new XSString(normalized);
//		}

//		private static W3CNormalizer Normalizer
//		{
//			get
//			{
//				if (normalizer == null)
//				{
//					ClassLoader classLoader = Thread.CurrentThread.ContextClassLoader;
    
//					normalizer = createICUNormalizer(classLoader);
//					/*
//					if (normalizer == null) {
//						normalizer = createJDKNormalizer(classLoader);
//					}
//					*/
//					if (normalizer == null)
//					{
//						normalizer = new FailingNormalizer();
//					}
//				}
//				return normalizer;
//			}
//		}

		/*
		private static W3CNormalizer createJDKNormalizer(ClassLoader classLoader) {
			// If that fails, we'll check for the Java 6 Normalizer class
			try {
				Class normalizerClass = classLoader.loadClass("java.text.Normalizer");
				Class formClass = classLoader.loadClass("java.text.Normalizer$Form");
				
				return new JDK6Normalizer(normalizerClass, formClass);
			} catch (ClassNotFoundException e) {
			} catch (SecurityException e) {
			} catch (NoSuchMethodException e) {
			}
			return null;
		}
		*/

		//private static W3CNormalizer createICUNormalizer(ClassLoader classLoader)
		//{
		//	// First attempt is to try the IBM ICU library
		//	try
		//	{
		//		if (classLoader.loadClass("com.ibm.icu.text.Normalizer") != null)
		//		{
		//			return new ICUNormalizer();
		//		}
		//	}
		//	catch (ClassNotFoundException)
		//	{
		//	}
		//	return null;
		//}

		///// <summary>
		///// Calculate the expected arguments.
		///// </summary>
		///// <returns> The expected arguments. </returns>
		//public static ICollection expected_args()
		//{
		//	lock (typeof(FnNormalizeUnicode))
		//	{
		//		if (_expected_args == null)
		//		{
		//			_expected_args = new ArrayList();
		//			_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_QMARK));
		//			_expected_args.Add(new SeqType(new XSString(), SeqType.OCC_NONE));
		//		}
        
		//		return _expected_args;
		//	}
		//}
	}

}