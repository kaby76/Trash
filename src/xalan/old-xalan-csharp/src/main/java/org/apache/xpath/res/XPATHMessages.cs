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
 * $Id: XPATHMessages.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.res
{

	using XMLMessages = org.apache.xml.res.XMLMessages;

	/// <summary>
	/// A utility class for issuing XPath error messages.
	/// @xsl.usage internal
	/// </summary>
	public class XPATHMessages : XMLMessages
	{
	  /// <summary>
	  /// The language specific resource object for XPath messages. </summary>
	  private static ListResourceBundle XPATHBundle = null;

	  /// <summary>
	  /// The class name of the XPath error message string table. </summary>
	  private const string XPATH_ERROR_RESOURCES = "org.apache.xpath.res.XPATHErrorResources";

	  /// <summary>
	  /// Creates a message from the specified key and replacement
	  /// arguments, localized to the given locale.
	  /// </summary>
	  /// <param name="msgKey">    The key for the message text. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted message string. </returns>
	  public static string createXPATHMessage(string msgKey, object[] args) //throws Exception
	  {
		if (XPATHBundle == null)
		{
		  XPATHBundle = loadResourceBundle(XPATH_ERROR_RESOURCES);
		}

		if (XPATHBundle != null)
		{
		  return createXPATHMsg(XPATHBundle, msgKey, args);
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
	  /// <param name="msgKey"> The key for the message text. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted warning string. </returns>
	  public static string createXPATHWarning(string msgKey, object[] args) //throws Exception
	  {
		if (XPATHBundle == null)
		{
		  XPATHBundle = loadResourceBundle(XPATH_ERROR_RESOURCES);
		}

		if (XPATHBundle != null)
		{
		  return createXPATHMsg(XPATHBundle, msgKey, args);
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
	  public static string createXPATHMsg(ListResourceBundle fResourceBundle, string msgKey, object[] args) //throws Exception
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
		  msg = fResourceBundle.getString(XPATHErrorResources.BAD_CODE);
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
			fmsg = fResourceBundle.getString(XPATHErrorResources.FORMAT_FAILED);
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

	}

}