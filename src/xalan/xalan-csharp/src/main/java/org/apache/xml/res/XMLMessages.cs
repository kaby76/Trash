using System;

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
 * $Id: XMLMessages.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.res
{

	/// <summary>
	/// A utility class for issuing XML error messages.
	/// @xsl.usage internal
	/// </summary>
	public class XMLMessages
	{

	  /// <summary>
	  /// The local object to use. </summary>
	  protected internal Locale fLocale = Locale.getDefault();

	  /// <summary>
	  /// The language specific resource object for XML messages. </summary>
	  private static ListResourceBundle XMLBundle = null;

	  /// <summary>
	  /// The class name of the XML error message string table. </summary>
	  private const string XML_ERROR_RESOURCES = "org.apache.xml.res.XMLErrorResources";

	  /// <summary>
	  /// String to use if a bad message code is used. </summary>
	  protected internal const string BAD_CODE = "BAD_CODE";

	  /// <summary>
	  /// String to use if the message format operation failed. </summary>
	  protected internal const string FORMAT_FAILED = "FORMAT_FAILED";

	  /// <summary>
	  /// Set the Locale object to use.
	  /// </summary>
	  /// <param name="locale"> non-null reference to Locale object. </param>
	   public virtual Locale Locale
	   {
		   set
		   {
			fLocale = value;
		   }
		   get
		   {
			return fLocale;
		   }
	   }


	  /// <summary>
	  /// Creates a message from the specified key and replacement
	  /// arguments, localized to the given locale.
	  /// </summary>
	  /// <param name="msgKey">    The key for the message text. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted message string. </returns>
	  public static string createXMLMessage(string msgKey, object[] args)
	  {
		if (XMLBundle == null)
		{
		  XMLBundle = loadResourceBundle(XML_ERROR_RESOURCES);
		}

		if (XMLBundle != null)
		{
		  return createMsg(XMLBundle, msgKey, args);
		}
		else
		{
		  return "Could not load any resource bundles.";
		}
	  }

	  /// <summary>
	  /// Creates a message from the specified key and replacement
	  /// arguments, localized to the given locale.
	  /// </summary>
	  /// <param name="fResourceBundle"> The resource bundle to use. </param>
	  /// <param name="msgKey">  The message key to use. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted message string. </returns>
	  public static string createMsg(ListResourceBundle fResourceBundle, string msgKey, object[] args) //throws Exception
	  {

		string fmsg = null;
		bool throwex = false;
		string msg = null;

		if (!string.ReferenceEquals(msgKey, null))
		{
		  msg = fResourceBundle.getString(msgKey);
		}

		if (string.ReferenceEquals(msg, null))
		{
		  msg = fResourceBundle.getString(BAD_CODE);
		  throwex = true;
		}

		if (args != null)
		{
		  try
		  {

			// Do this to keep format from crying.
			// This is better than making a bunch of conditional
			// code all over the place.
			int n = args.Length;

			for (int i = 0; i < n; i++)
			{
			  if (null == args[i])
			  {
				args[i] = "";
			  }
			}

			fmsg = java.text.MessageFormat.format(msg, args);
		  }
		  catch (Exception)
		  {
			fmsg = fResourceBundle.getString(FORMAT_FAILED);
			fmsg += " " + msg;
		  }
		}
		else
		{
		  fmsg = msg;
		}

		if (throwex)
		{
		  throw new Exception(fmsg);
		}

		return fmsg;
	  }

	  /// <summary>
	  /// Return a named ResourceBundle for a particular locale.  This method mimics the behavior
	  /// of ResourceBundle.getBundle().
	  /// </summary>
	  /// <param name="className"> The class name of the resource bundle. </param>
	  /// <returns> the ResourceBundle </returns>
	  /// <exception cref="MissingResourceException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static java.util.ListResourceBundle loadResourceBundle(String className) throws java.util.MissingResourceException
	  public static ListResourceBundle loadResourceBundle(string className)
	  {
		Locale locale = Locale.getDefault();

		try
		{
		  return (ListResourceBundle)ResourceBundle.getBundle(className, locale);
		}
		catch (MissingResourceException)
		{
		  try // try to fall back to en_US if we can't load
		  {

			// Since we can't find the localized property file,
			// fall back to en_US.
			return (ListResourceBundle)ResourceBundle.getBundle(className, new Locale("en", "US"));
		  }
		  catch (MissingResourceException)
		  {

			// Now we are really in trouble.
			// very bad, definitely very bad...not going to get very far
			throw new MissingResourceException("Could not load any resource bundles." + className, className, "");
		  }
		}
	  }

	  /// <summary>
	  /// Return the resource file suffic for the indicated locale
	  /// For most locales, this will be based the language code.  However
	  /// for Chinese, we do distinguish between Taiwan and PRC
	  /// </summary>
	  /// <param name="locale"> the locale </param>
	  /// <returns> an String suffix which can be appended to a resource name </returns>
	  protected internal static string getResourceSuffix(Locale locale)
	  {

		string suffix = "_" + locale.getLanguage();
		string country = locale.getCountry();

		if (country.Equals("TW"))
		{
		  suffix += "_" + country;
		}

		return suffix;
	  }
	}

}