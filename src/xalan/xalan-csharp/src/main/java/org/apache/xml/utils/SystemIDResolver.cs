using System;
using System.Text;

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
 * $Id: SystemIDResolver.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{


	using MalformedURIException = org.apache.xml.utils.URI.MalformedURIException;

	/// <summary>
	/// This class is used to resolve relative URIs and SystemID 
	/// strings into absolute URIs.
	/// 
	/// <para>This is a generic utility for resolving URIs, other than the 
	/// fact that it's declared to throw TransformerException.  Please 
	/// see code comments for details on how resolution is performed.</para>
	/// @xsl.usage internal
	/// </summary>
	public class SystemIDResolver
	{

	  /// <summary>
	  /// Get an absolute URI from a given relative URI (local path). 
	  /// 
	  /// <para>The relative URI is a local filesystem path. The path can be
	  /// absolute or relative. If it is a relative path, it is resolved relative 
	  /// to the system property "user.dir" if it is available; if not (i.e. in an 
	  /// Applet perhaps which throws SecurityException) then we just return the
	  /// relative path. The space and backslash characters are also replaced to
	  /// generate a good absolute URI.</para>
	  /// </summary>
	  /// <param name="localPath"> The relative URI to resolve
	  /// </param>
	  /// <returns> Resolved absolute URI </returns>
	  public static string getAbsoluteURIFromRelative(string localPath)
	  {
		if (string.ReferenceEquals(localPath, null) || localPath.Length == 0)
		{
		  return "";
		}

		// If the local path is a relative path, then it is resolved against
		// the "user.dir" system property.
		string absolutePath = localPath;
		if (!isAbsolutePath(localPath))
		{
		  try
		  {
			absolutePath = getAbsolutePathFromRelativePath(localPath);
		  }
		  // user.dir not accessible from applet
		  catch (SecurityException)
		  {
			return "file:" + localPath;
		  }
		}

		string urlString;
		if (null != absolutePath)
		{
		  if (absolutePath.StartsWith(File.separator, StringComparison.Ordinal))
		  {
			urlString = "file://" + absolutePath;
		  }
		  else
		  {
			urlString = "file:///" + absolutePath;
		  }
		}
		else
		{
		  urlString = "file:" + localPath;
		}

		return replaceChars(urlString);
	  }

	  /// <summary>
	  /// Return an absolute path from a relative path.
	  /// </summary>
	  /// <param name="relativePath"> A relative path </param>
	  /// <returns> The absolute path </returns>
	  private static string getAbsolutePathFromRelativePath(string relativePath)
	  {
		return System.IO.Path.GetFullPath(relativePath);
	  }

	  /// <summary>
	  /// Return true if the systemId denotes an absolute URI .
	  /// </summary>
	  /// <param name="systemId"> The systemId string </param>
	  /// <returns> true if the systemId is an an absolute URI </returns>
	  public static bool isAbsoluteURI(string systemId)
	  {
		 /// <summary>
		 /// http://www.ietf.org/rfc/rfc2396.txt
		 ///   Authors should be aware that a path segment which contains a colon
		 /// character cannot be used as the first segment of a relative URI path
		 /// (e.g., "this:that"), because it would be mistaken for a scheme name.
		 /// 
		 /// </summary>
		 /// <summary>
		 /// %REVIEW% Can we assume here that systemId is a valid URI?
		 /// It looks like we cannot ( See discussion of this common problem in 
		 /// Bugzilla Bug 22777 ). 
		 /// 
		 /// </summary>
		 //"fix" for Bugzilla Bug 22777
		if (isWindowsAbsolutePath(systemId))
		{
			return false;
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int fragmentIndex = systemId.indexOf('#');
		int fragmentIndex = systemId.IndexOf('#');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int queryIndex = systemId.indexOf('?');
		int queryIndex = systemId.IndexOf('?');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int slashIndex = systemId.indexOf('/');
		int slashIndex = systemId.IndexOf('/');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int colonIndex = systemId.indexOf(':');
		int colonIndex = systemId.IndexOf(':');

		//finding substring  before '#', '?', and '/' 
		int index = systemId.Length - 1;
		if (fragmentIndex > 0)
		{
			index = fragmentIndex;
		}
		if ((queryIndex > 0) && (queryIndex < index))
		{
			index = queryIndex;
		}
		if ((slashIndex > 0) && (slashIndex < index))
		{
			index = slashIndex;
		}
		// return true if there is ':' before '#', '?', and '/'
		return ((colonIndex > 0) && (colonIndex < index));

	  }

	  /// <summary>
	  /// Return true if the local path is an absolute path.
	  /// </summary>
	  /// <param name="systemId"> The path string </param>
	  /// <returns> true if the path is absolute </returns>
	  public static bool isAbsolutePath(string systemId)
	  {
		if (string.ReferenceEquals(systemId, null))
		{
			return false;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File file = new java.io.File(systemId);
		File file = new File(systemId);
		return file.Absolute;

	  }

	   /// <summary>
	   /// Return true if the local path is a Windows absolute path.
	   /// </summary>
	   /// <param name="systemId"> The path string </param>
	   /// <returns> true if the path is a Windows absolute path </returns>
		private static bool isWindowsAbsolutePath(string systemId)
		{
		if (!isAbsolutePath(systemId))
		{
		  return false;
		}
		// On Windows, an absolute path starts with "[drive_letter]:\".
		if (systemId.Length > 2 && systemId[1] == ':' && char.IsLetter(systemId[0]) && (systemId[2] == '\\' || systemId[2] == '/'))
		{
		  return true;
		}
		else
		{
		  return false;
		}
		}

	  /// <summary>
	  /// Replace spaces with "%20" and backslashes with forward slashes in 
	  /// the input string to generate a well-formed URI string.
	  /// </summary>
	  /// <param name="str"> The input string </param>
	  /// <returns> The string after conversion </returns>
	  private static string replaceChars(string str)
	  {
		StringBuilder buf = new StringBuilder(str);
		int length = buf.Length;
		for (int i = 0; i < length; i++)
		{
		  char currentChar = buf[i];
		  // Replace space with "%20"
		  if (currentChar == ' ')
		  {
			buf[i] = '%';
			buf.Insert(i + 1, "20");
			length = length + 2;
			i = i + 2;
		  }
		  // Replace backslash with forward slash
		  else if (currentChar == '\\')
		  {
			buf[i] = '/';
		  }
		}

		return buf.ToString();
	  }

	  /// <summary>
	  /// Take a SystemID string and try to turn it into a good absolute URI.
	  /// </summary>
	  /// <param name="systemId"> A URI string, which may be absolute or relative.
	  /// </param>
	  /// <returns> The resolved absolute URI </returns>
	  public static string getAbsoluteURI(string systemId)
	  {
		string absoluteURI = systemId;
		if (isAbsoluteURI(systemId))
		{
		  // Only process the systemId if it starts with "file:".
		  if (systemId.StartsWith("file:", StringComparison.Ordinal))
		  {
			string str = systemId.Substring(5);

			// Resolve the absolute path if the systemId starts with "file:///"
			// or "file:/". Don't do anything if it only starts with "file://".
			if (!string.ReferenceEquals(str, null) && str.StartsWith("/", StringComparison.Ordinal))
			{
			  if (str.StartsWith("///", StringComparison.Ordinal) || !str.StartsWith("//", StringComparison.Ordinal))
			  {
				// A Windows path containing a drive letter can be relative.
				// A Unix path starting with "file:/" is always absolute.
				int secondColonIndex = systemId.IndexOf(':', 5);
				if (secondColonIndex > 0)
				{
				  string localPath = systemId.Substring(secondColonIndex - 1);
				  try
				  {
					if (!isAbsolutePath(localPath))
					{
					  absoluteURI = systemId.Substring(0, secondColonIndex - 1) + getAbsolutePathFromRelativePath(localPath);
					}
				  }
				  catch (SecurityException)
				  {
					return systemId;
				  }
				}
			  }
			}
			else
			{
			  return getAbsoluteURIFromRelative(systemId.Substring(5));
			}

			return replaceChars(absoluteURI);
		  }
		  else
		  {
			return systemId;
		  }
		}
		else
		{
		  return getAbsoluteURIFromRelative(systemId);
		}

	  }


	  /// <summary>
	  /// Take a SystemID string and try to turn it into a good absolute URI.
	  /// </summary>
	  /// <param name="urlString"> SystemID string </param>
	  /// <param name="base"> The URI string used as the base for resolving the systemID
	  /// </param>
	  /// <returns> The resolved absolute URI </returns>
	  /// <exception cref="TransformerException"> thrown if the string can't be turned into a URI. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String getAbsoluteURI(String urlString, String super) throws javax.xml.transform.TransformerException
	  public static string getAbsoluteURI(string urlString, string @base)
	  {
		if (string.ReferenceEquals(@base, null))
		{
		  return getAbsoluteURI(urlString);
		}

		string absoluteBase = getAbsoluteURI(@base);
		URI uri = null;
		try
		{
		  URI baseURI = new URI(absoluteBase);
		  uri = new URI(baseURI, urlString);
		}
		catch (MalformedURIException mue)
		{
		  throw new TransformerException(mue);
		}

		return replaceChars(uri.ToString());
	  }

	}

}