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
 * $Id: XSLMessages.java 468641 2006-10-28 06:54:42Z minchau $
 */
namespace org.apache.xalan.res
{

	using XPATHMessages = org.apache.xpath.res.XPATHMessages;

	/// <summary>
	/// Sets things up for issuing error messages.  This class is misnamed, and
	/// should be called XalanMessages, or some such.
	/// @xsl.usage internal
	/// </summary>
	public class XSLMessages : XPATHMessages
	{

	  /// <summary>
	  /// The language specific resource object for Xalan messages. </summary>
	  private static ListResourceBundle XSLTBundle = null;

	  /// <summary>
	  /// The class name of the Xalan error message string table. </summary>
	  private const string XSLT_ERROR_RESOURCES = "org.apache.xalan.res.XSLTErrorResources";

	  /// <summary>
	  /// Creates a message from the specified key and replacement
	  /// arguments, localized to the given locale.
	  /// </summary>
	  /// <param name="msgKey">    The key for the message text. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted message string. </returns>
	  public static string createMessage(string msgKey, object[] args) //throws Exception
	  {
		if (XSLTBundle == null)
		{
		  XSLTBundle = loadResourceBundle(XSLT_ERROR_RESOURCES);
		}

		if (XSLTBundle != null)
		{
		  return createMsg(XSLTBundle, msgKey, args);
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
	  /// <param name="msgKey">    The key for the message text. </param>
	  /// <param name="args">      The arguments to be used as replacement text
	  ///                  in the message created.
	  /// </param>
	  /// <returns> The formatted warning string. </returns>
	  public static string createWarning(string msgKey, object[] args) //throws Exception
	  {
		if (XSLTBundle == null)
		{
		  XSLTBundle = loadResourceBundle(XSLT_ERROR_RESOURCES);
		}

		if (XSLTBundle != null)
		{
		  return createMsg(XSLTBundle, msgKey, args);
		}
		else
		{
		  return "Could not load any resource bundles.";
		}
	  }
	}

}