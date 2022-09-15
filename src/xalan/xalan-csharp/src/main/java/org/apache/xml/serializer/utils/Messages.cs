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
 * $Id: Messages.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer.utils
{

	/// <summary>
	/// A utility class for issuing error messages.
	/// 
	/// A user of this class normally would create a singleton 
	/// instance of this class, passing the name
	/// of the message class on the constructor. For example:
	/// <CODE>
	/// static Messages x = new Messages("org.package.MyMessages");
	/// </CODE>
	/// Later the message is typically generated this way if there are no 
	/// substitution arguments:
	/// <CODE>
	/// String msg = x.createMessage(org.package.MyMessages.KEY_ONE, null); 
	/// </CODE>
	/// If there are arguments substitutions then something like this:
	/// <CODE>
	/// String filename = ...;
	/// String directory = ...;
	/// String msg = x.createMessage(org.package.MyMessages.KEY_TWO, 
	///   new Object[] {filename, directory) ); 
	/// </CODE>
	/// 
	/// The constructor of an instance of this class must be given
	/// the class name of a class that extends java.util.ListResourceBundle 
	/// ("org.package.MyMessages" in the example above).  
	/// The name should not have any language suffix 
	/// which will be added automatically by this utility class.
	/// 
	/// The message class ("org.package.MyMessages")
	/// must define the abstract method getContents() that is
	/// declared in its base class, for example:
	/// <CODE>
	/// public Object[][] getContents() {return contents;}
	/// </CODE>
	/// 
	/// It is suggested that the message class expose its
	/// message keys like this:
	/// <CODE>
	///   public static final String KEY_ONE = "KEY1";
	///   public static final String KEY_TWO = "KEY2";
	///   . . . 
	/// </CODE>
	/// and used through their names (KEY_ONE ...) rather than
	/// their values ("KEY1" ...).
	/// 
	/// The field contents (returned by getContents()
	/// should be initialized something like this:
	/// <CODE>
	/// public static final Object[][] contents = {
	/// { KEY_ONE, "Something has gone wrong!" },
	/// { KEY_TWO, "The file ''{0}'' does not exist in directory ''{1}''." },
	/// . . .
	/// { KEY_N, "Message N" }  }
	/// </CODE>
	/// 
	/// Where that section of code with the KEY to Message mappings
	/// (where the message classes 'contents' field is initialized)
	/// can have the Message strings translated in an alternate language
	/// in a errorResourceClass with a language suffix.
	/// 
	/// More sophisticated use of this class would be to pass null
	/// when contructing it, but then call loadResourceBundle()
	/// before creating any messages.
	/// 
	/// This class is not a public API, it is only public because it is 
	/// used in org.apache.xml.serializer.
	/// 
	///  @xsl.usage internal
	/// </summary>
	public sealed class Messages
	{
		/// <summary>
		/// The local object to use. </summary>
		private readonly Locale m_locale = Locale.getDefault();

		/// <summary>
		/// The language specific resource object for messages. </summary>
		private ListResourceBundle m_resourceBundle;

		/// <summary>
		/// The class name of the error message string table with no language suffix. </summary>
		private string m_resourceBundleName;



		/// <summary>
		/// Constructor. </summary>
		/// <param name="resourceBundle"> the class name of the ListResourceBundle
		/// that the instance of this class is associated with and will use when
		/// creating messages.
		/// The class name is without a language suffix. If the value passed
		/// is null then loadResourceBundle(errorResourceClass) needs to be called
		/// explicitly before any messages are created.
		/// 
		/// @xsl.usage internal </param>
		internal Messages(string resourceBundle)
		{

			m_resourceBundleName = resourceBundle;
		}

		/*
		 * Set the Locale object to use. If this method is not called the
		 * default locale is used. This method needs to be called before
		 * loadResourceBundle().
		 * 
		 * @param locale non-null reference to Locale object.
		 * @xsl.usage internal
		 */
	//    public void setLocale(Locale locale)
	//    {
	//        m_locale = locale;
	//    }

		/// <summary>
		/// Get the Locale object that is being used.
		/// </summary>
		/// <returns> non-null reference to Locale object.
		/// @xsl.usage internal </returns>
		private Locale Locale
		{
			get
			{
				return m_locale;
			}
		}

		/// <summary>
		/// Get the ListResourceBundle being used by this Messages instance which was
		/// previously set by a call to loadResourceBundle(className)
		/// @xsl.usage internal
		/// </summary>
		private ListResourceBundle ResourceBundle
		{
			get
			{
				return m_resourceBundle;
			}
		}

		/// <summary>
		/// Creates a message from the specified key and replacement
		/// arguments, localized to the given locale.
		/// </summary>
		/// <param name="msgKey">  The key for the message text. </param>
		/// <param name="args">    The arguments to be used as replacement text
		/// in the message created.
		/// </param>
		/// <returns> The formatted message string.
		/// @xsl.usage internal </returns>
		public string createMessage(string msgKey, object[] args)
		{
			if (m_resourceBundle == null)
			{
				m_resourceBundle = loadResourceBundle(m_resourceBundleName);
			}

			if (m_resourceBundle != null)
			{
				return createMsg(m_resourceBundle, msgKey, args);
			}
			else
			{
				return "Could not load the resource bundles: " + m_resourceBundleName;
			}
		}

		/// <summary>
		/// Creates a message from the specified key and replacement
		/// arguments, localized to the given locale.
		/// </summary>
		/// <param name="errorCode"> The key for the message text.
		/// </param>
		/// <param name="fResourceBundle"> The resource bundle to use. </param>
		/// <param name="msgKey">  The message key to use. </param>
		/// <param name="args">      The arguments to be used as replacement text
		///                  in the message created.
		/// </param>
		/// <returns> The formatted message string.
		/// @xsl.usage internal </returns>
		private string createMsg(ListResourceBundle fResourceBundle, string msgKey, object[] args) //throws Exception
		{

			string fmsg = null;
			bool throwex = false;
			string msg = null;

			if (!string.ReferenceEquals(msgKey, null))
			{
				msg = fResourceBundle.getString(msgKey);
			}
			else
			{
				msgKey = "";
			}

			if (string.ReferenceEquals(msg, null))
			{
				throwex = true;
				/* The message is not in the bundle . . . this is bad,
				 * so try to get the message that the message is not in the bundle
				 */
				try
				{

					msg = java.text.MessageFormat.format(MsgKey.BAD_MSGKEY, new object[] {msgKey, m_resourceBundleName});
				}
				catch (Exception)
				{
					/* even the message that the message is not in the bundle is
					 * not there ... this is really bad
					 */
					msg = "The message key '" + msgKey + "' is not in the message class '" + m_resourceBundleName + "'";
				}
			}
			else if (args != null)
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
					// if we get past the line above we have create the message ... hurray!
				}
				catch (Exception)
				{
					throwex = true;
					try
					{
						// Get the message that the format failed.
						fmsg = java.text.MessageFormat.format(MsgKey.BAD_MSGFORMAT, new object[] {msgKey, m_resourceBundleName});
						fmsg += " " + msg;
					}
					catch (Exception)
					{
						// We couldn't even get the message that the format of
						// the message failed ... so fall back to English.
						fmsg = "The format of message '" + msgKey + "' in message class '" + m_resourceBundleName + "' failed.";
					}
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
		/// <param name="className"> the name of the class that implements ListResourceBundle,
		/// without language suffix. </param>
		/// <returns> the ResourceBundle </returns>
		/// <exception cref="MissingResourceException">
		/// @xsl.usage internal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.ListResourceBundle loadResourceBundle(String resourceBundle) throws java.util.MissingResourceException
		private ListResourceBundle loadResourceBundle(string resourceBundle)
		{
			m_resourceBundleName = resourceBundle;
			Locale locale = Locale;

			ListResourceBundle lrb;

			try
			{

				ResourceBundle rb = ResourceBundle.getBundle(m_resourceBundleName, locale);
				lrb = (ListResourceBundle) rb;
			}
			catch (MissingResourceException)
			{
				try // try to fall back to en_US if we can't load
				{

					// Since we can't find the localized property file,
					// fall back to en_US.
					lrb = (ListResourceBundle) ResourceBundle.getBundle(m_resourceBundleName, new Locale("en", "US"));
				}
				catch (MissingResourceException)
				{

					// Now we are really in trouble.
					// very bad, definitely very bad...not going to get very far
					throw new MissingResourceException("Could not load any resource bundles." + m_resourceBundleName, m_resourceBundleName, "");
				}
			}
			m_resourceBundle = lrb;
			return lrb;
		}

		/// <summary>
		/// Return the resource file suffic for the indicated locale
		/// For most locales, this will be based the language code.  However
		/// for Chinese, we do distinguish between Taiwan and PRC
		/// </summary>
		/// <param name="locale"> the locale </param>
		/// <returns> an String suffix which can be appended to a resource name
		/// @xsl.usage internal </returns>
		private static string getResourceSuffix(Locale locale)
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