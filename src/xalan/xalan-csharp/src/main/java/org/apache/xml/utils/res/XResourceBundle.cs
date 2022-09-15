/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: XResourceBundle.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils.res
{

	/// <summary>
	/// The default (english) resource bundle.
	/// @xsl.usage internal
	/// </summary>
	public class XResourceBundle : ListResourceBundle
	{

	  /// <summary>
	  /// Error resource constants </summary>
	  public const string ERROR_RESOURCES = "org.apache.xalan.res.XSLTErrorResources", XSLT_RESOURCE = "org.apache.xml.utils.res.XResourceBundle", LANG_BUNDLE_NAME = "org.apache.xml.utils.res.XResources", MULT_ORDER = "multiplierOrder", MULT_PRECEDES = "precedes", MULT_FOLLOWS = "follows", LANG_ORIENTATION = "orientation", LANG_RIGHTTOLEFT = "rightToLeft", LANG_LEFTTORIGHT = "leftToRight", LANG_NUMBERING = "numbering", LANG_ADDITIVE = "additive", LANG_MULT_ADD = "multiplicative-additive", LANG_MULTIPLIER = "multiplier", LANG_MULTIPLIER_CHAR = "multiplierChar", LANG_NUMBERGROUPS = "numberGroups", LANG_NUM_TABLES = "tables", LANG_ALPHABET = "alphabet", LANG_TRAD_ALPHABET = "tradAlphabet";

	  /// <summary>
	  /// Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  /// of ResourceBundle.getBundle().
	  /// </summary>
	  /// <param name="className"> Name of local-specific subclass. </param>
	  /// <param name="locale"> the locale to prefer when searching for the bundle </param>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static final XResourceBundle loadResourceBundle(String className, java.util.Locale locale) throws java.util.MissingResourceException
	  public static XResourceBundle loadResourceBundle(string className, Locale locale)
	  {

		string suffix = getResourceSuffix(locale);

		//System.out.println("resource " + className + suffix);
		try
		{

		  // first try with the given locale
		  string resourceName = className + suffix;
		  return (XResourceBundle) ResourceBundle.getBundle(resourceName, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (XResourceBundle) ResourceBundle.getBundle(XSLT_RESOURCE, new Locale("en", "US"));
		  }
		  catch (MissingResourceException)
		  {

			// Now we are really in trouble.
			// very bad, definitely very bad...not going to get very far
			throw new MissingResourceException("Could not load any resource bundles.", className, "");
		  }
		}
	  }

	  /// <summary>
	  /// Return the resource file suffic for the indicated locale
	  /// For most locales, this will be based the language code.  However
	  /// for Chinese, we do distinguish between Taiwan and PRC
	  /// </summary>
	  /// <param name="locale"> the locale </param>
	  /// <returns> an String suffix which canbe appended to a resource name </returns>
	  private static string getResourceSuffix(Locale locale)
	  {

		string lang = locale.getLanguage();
		string country = locale.getCountry();
		string variant = locale.getVariant();
		string suffix = "_" + locale.getLanguage();

		if (lang.Equals("zh"))
		{
		  suffix += "_" + country;
		}

		if (country.Equals("JP"))
		{
		  suffix += "_" + country + "_" + variant;
		}

		return suffix;
	  }

	  /// <summary>
	  /// Get the association list.
	  /// </summary>
	  /// <returns> The association list. </returns>
	  public virtual object[][] Contents
	  {
		  get
		  {
			return new object[][]
			{
				new object[] {"ui_language", "en"},
				new object[] {"help_language", "en"},
				new object[] {"language", "en"},
				new object[] {"alphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"tradAlphabet", new CharArrayWrapper(new char[]{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'})},
				new object[] {"orientation", "LeftToRight"},
				new object[] {"numbering", "additive"}
			};
		  }
	  }
	}

}