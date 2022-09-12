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
 * $Id: URI.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer.utils
{



	/// <summary>
	/// A class to represent a Uniform Resource Identifier (URI). This class
	/// is designed to handle the parsing of URIs and provide access to
	/// the various components (scheme, host, port, userinfo, path, query
	/// string and fragment) that may constitute a URI.
	/// <para>
	/// Parsing of a URI specification is done according to the URI
	/// syntax described in RFC 2396
	/// <http://www.ietf.org/rfc/rfc2396.txt?number=2396>. Every URI consists
	/// of a scheme, followed by a colon (':'), followed by a scheme-specific
	/// part. For URIs that follow the "generic URI" syntax, the scheme-
	/// specific part begins with two slashes ("//") and may be followed
	/// by an authority segment (comprised of user information, host, and
	/// port), path segment, query segment and fragment. Note that RFC 2396
	/// no longer specifies the use of the parameters segment and excludes
	/// the "user:password" syntax as part of the authority segment. If
	/// "user:password" appears in a URI, the entire user/password string
	/// is stored as userinfo.
	/// </para>
	/// <para>
	/// For URIs that do not follow the "generic URI" syntax (e.g. mailto),
	/// the entire scheme-specific part is treated as the "path" portion
	/// of the URI.
	/// </para>
	/// <para>
	/// Note that, unlike the java.net.URL class, this class does not provide
	/// any built-in network access functionality nor does it provide any
	/// scheme-specific functionality (for example, it does not know a
	/// default port for a specific scheme). Rather, it only knows the
	/// grammar and basic set of operations that can be applied to a URI.
	/// 
	/// This class is a copy of the one in org.apache.xml.utils. 
	/// It exists to cut the serializers dependancy on that package.
	/// 
	/// A minor change from the original is that this class no longer implements
	/// Serializable, and the serialVersionUID magic field is dropped, and
	/// the class is no longer "public".
	/// 
	/// @xsl.usage internal
	/// </para>
	/// </summary>
	internal sealed class URI
	{
	  /// <summary>
	  /// MalformedURIExceptions are thrown in the process of building a URI
	  /// or setting fields on a URI when an operation would result in an
	  /// invalid URI specification.
	  /// 
	  /// </summary>
	  public class MalformedURIException : IOException
	  {

		/// <summary>
		/// Constructs a <code>MalformedURIException</code> with no specified
		/// detail message.
		/// </summary>
		public MalformedURIException() : base()
		{
		}

		/// <summary>
		/// Constructs a <code>MalformedURIException</code> with the
		/// specified detail message.
		/// </summary>
		/// <param name="p_msg"> the detail message. </param>
		public MalformedURIException(string p_msg) : base(p_msg)
		{
		}
	  }

	  /// <summary>
	  /// reserved characters </summary>
	  private const string RESERVED_CHARACTERS = ";/?:@&=+$,";

	  /// <summary>
	  /// URI punctuation mark characters - these, combined with
	  ///   alphanumerics, constitute the "unreserved" characters 
	  /// </summary>
	  private const string MARK_CHARACTERS = "-_.!~*'() ";

	  /// <summary>
	  /// scheme can be composed of alphanumerics and these characters </summary>
	  private const string SCHEME_CHARACTERS = "+-.";

	  /// <summary>
	  /// userinfo can be composed of unreserved, escaped and these
	  ///   characters 
	  /// </summary>
	  private const string USERINFO_CHARACTERS = ";:&=+$,";

	  /// <summary>
	  /// Stores the scheme (usually the protocol) for this URI.
	  ///  @serial 
	  /// </summary>
	  private string m_scheme = null;

	  /// <summary>
	  /// If specified, stores the userinfo for this URI; otherwise null.
	  ///  @serial 
	  /// </summary>
	  private string m_userinfo = null;

	  /// <summary>
	  /// If specified, stores the host for this URI; otherwise null.
	  ///  @serial 
	  /// </summary>
	  private string m_host = null;

	  /// <summary>
	  /// If specified, stores the port for this URI; otherwise -1.
	  ///  @serial 
	  /// </summary>
	  private int m_port = -1;

	  /// <summary>
	  /// If specified, stores the path for this URI; otherwise null.
	  ///  @serial 
	  /// </summary>
	  private string m_path = null;

	  /// <summary>
	  /// If specified, stores the query string for this URI; otherwise
	  ///   null. 
	  /// @serial 
	  /// </summary>
	  private string m_queryString = null;

	  /// <summary>
	  /// If specified, stores the fragment for this URI; otherwise null.
	  ///  @serial 
	  /// </summary>
	  private string m_fragment = null;

	  /// <summary>
	  /// Indicate whether in DEBUG mode </summary>
	  private static bool DEBUG = false;

	  /// <summary>
	  /// Construct a new and uninitialized URI.
	  /// </summary>
	  public URI()
	  {
	  }

	  /// <summary>
	  /// Construct a new URI from another URI. All fields for this URI are
	  /// set equal to the fields of the URI passed in.
	  /// </summary>
	  /// <param name="p_other"> the URI to copy (cannot be null) </param>
	  public URI(URI p_other)
	  {
		initialize(p_other);
	  }

	  /// <summary>
	  /// Construct a new URI from a URI specification string. If the
	  /// specification follows the "generic URI" syntax, (two slashes
	  /// following the first colon), the specification will be parsed
	  /// accordingly - setting the scheme, userinfo, host,port, path, query
	  /// string and fragment fields as necessary. If the specification does
	  /// not follow the "generic URI" syntax, the specification is parsed
	  /// into a scheme and scheme-specific part (stored as the path) only.
	  /// </summary>
	  /// <param name="p_uriSpec"> the URI specification string (cannot be null or
	  ///                  empty)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_uriSpec violates any syntax
	  ///                                   rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String p_uriSpec) throws MalformedURIException
	  public URI(string p_uriSpec) : this((URI) null, p_uriSpec)
	  {
	  }

	  /// <summary>
	  /// Construct a new URI from a base URI and a URI specification string.
	  /// The URI specification string may be a relative URI.
	  /// </summary>
	  /// <param name="p_base"> the base URI (cannot be null if p_uriSpec is null or
	  ///               empty) </param>
	  /// <param name="p_uriSpec"> the URI specification string (cannot be null or
	  ///                  empty if p_base is null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_uriSpec violates any syntax
	  ///                                  rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(URI p_base, String p_uriSpec) throws MalformedURIException
	  public URI(URI p_base, string p_uriSpec)
	  {
		initialize(p_base, p_uriSpec);
	  }

	  /// <summary>
	  /// Construct a new URI that does not follow the generic URI syntax.
	  /// Only the scheme and scheme-specific part (stored as the path) are
	  /// initialized.
	  /// </summary>
	  /// <param name="p_scheme"> the URI scheme (cannot be null or empty) </param>
	  /// <param name="p_schemeSpecificPart"> the scheme-specific part (cannot be
	  ///                             null or empty)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_scheme violates any
	  ///                                  syntax rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String p_scheme, String p_schemeSpecificPart) throws MalformedURIException
	  public URI(string p_scheme, string p_schemeSpecificPart)
	  {

		if (string.ReferenceEquals(p_scheme, null) || p_scheme.Trim().Length == 0)
		{
		  throw new MalformedURIException("Cannot construct URI with null/empty scheme!");
		}

		if (string.ReferenceEquals(p_schemeSpecificPart, null) || p_schemeSpecificPart.Trim().Length == 0)
		{
		  throw new MalformedURIException("Cannot construct URI with null/empty scheme-specific part!");
		}

		Scheme = p_scheme;
		Path = p_schemeSpecificPart;
	  }

	  /// <summary>
	  /// Construct a new URI that follows the generic URI syntax from its
	  /// component parts. Each component is validated for syntax and some
	  /// basic semantic checks are performed as well.  See the individual
	  /// setter methods for specifics.
	  /// </summary>
	  /// <param name="p_scheme"> the URI scheme (cannot be null or empty) </param>
	  /// <param name="p_host"> the hostname or IPv4 address for the URI </param>
	  /// <param name="p_path"> the URI path - if the path contains '?' or '#',
	  ///               then the query string and/or fragment will be
	  ///               set from the path; however, if the query and
	  ///               fragment are specified both in the path and as
	  ///               separate parameters, an exception is thrown </param>
	  /// <param name="p_queryString"> the URI query string (cannot be specified
	  ///                      if path is null) </param>
	  /// <param name="p_fragment"> the URI fragment (cannot be specified if path
	  ///                   is null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if any of the parameters violates
	  ///                                  syntax rules or semantic rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String p_scheme, String p_host, String p_path, String p_queryString, String p_fragment) throws MalformedURIException
	  public URI(string p_scheme, string p_host, string p_path, string p_queryString, string p_fragment) : this(p_scheme, null, p_host, -1, p_path, p_queryString, p_fragment)
	  {
	  }

	  /// <summary>
	  /// Construct a new URI that follows the generic URI syntax from its
	  /// component parts. Each component is validated for syntax and some
	  /// basic semantic checks are performed as well.  See the individual
	  /// setter methods for specifics.
	  /// </summary>
	  /// <param name="p_scheme"> the URI scheme (cannot be null or empty) </param>
	  /// <param name="p_userinfo"> the URI userinfo (cannot be specified if host
	  ///                   is null) </param>
	  /// <param name="p_host"> the hostname or IPv4 address for the URI </param>
	  /// <param name="p_port"> the URI port (may be -1 for "unspecified"; cannot
	  ///               be specified if host is null) </param>
	  /// <param name="p_path"> the URI path - if the path contains '?' or '#',
	  ///               then the query string and/or fragment will be
	  ///               set from the path; however, if the query and
	  ///               fragment are specified both in the path and as
	  ///               separate parameters, an exception is thrown </param>
	  /// <param name="p_queryString"> the URI query string (cannot be specified
	  ///                      if path is null) </param>
	  /// <param name="p_fragment"> the URI fragment (cannot be specified if path
	  ///                   is null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if any of the parameters violates
	  ///                                  syntax rules or semantic rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public URI(String p_scheme, String p_userinfo, String p_host, int p_port, String p_path, String p_queryString, String p_fragment) throws MalformedURIException
	  public URI(string p_scheme, string p_userinfo, string p_host, int p_port, string p_path, string p_queryString, string p_fragment)
	  {

		if (string.ReferenceEquals(p_scheme, null) || p_scheme.Trim().Length == 0)
		{
		  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_SCHEME_REQUIRED, null)); //"Scheme is required!");
		}

		if (string.ReferenceEquals(p_host, null))
		{
		  if (!string.ReferenceEquals(p_userinfo, null))
		  {
			throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_USERINFO_IF_NO_HOST, null)); //"Userinfo may not be specified if host is not specified!");
		  }

		  if (p_port != -1)
		  {
			throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_PORT_IF_NO_HOST, null)); //"Port may not be specified if host is not specified!");
		  }
		}

		if (!string.ReferenceEquals(p_path, null))
		{
		  if (p_path.IndexOf('?') != -1 && !string.ReferenceEquals(p_queryString, null))
		  {
			throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_QUERY_STRING_IN_PATH, null)); //"Query string cannot be specified in path and query string!");
		  }

		  if (p_path.IndexOf('#') != -1 && !string.ReferenceEquals(p_fragment, null))
		  {
			throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, null)); //"Fragment cannot be specified in both the path and fragment!");
		  }
		}

		Scheme = p_scheme;
		Host = p_host;
		Port = p_port;
		Userinfo = p_userinfo;
		Path = p_path;
		QueryString = p_queryString;
		Fragment = p_fragment;
	  }

	  /// <summary>
	  /// Initialize all fields of this URI from another URI.
	  /// </summary>
	  /// <param name="p_other"> the URI to copy (cannot be null) </param>
	  private void initialize(URI p_other)
	  {

		m_scheme = p_other.Scheme;
		m_userinfo = p_other.Userinfo;
		m_host = p_other.Host;
		m_port = p_other.Port;
		m_path = p_other.Path;
		m_queryString = p_other.QueryString;
		m_fragment = p_other.Fragment;
	  }

	  /// <summary>
	  /// Initializes this URI from a base URI and a URI specification string.
	  /// See RFC 2396 Section 4 and Appendix B for specifications on parsing
	  /// the URI and Section 5 for specifications on resolving relative URIs
	  /// and relative paths.
	  /// </summary>
	  /// <param name="p_base"> the base URI (may be null if p_uriSpec is an absolute
	  ///               URI) </param>
	  /// <param name="p_uriSpec"> the URI spec string which may be an absolute or
	  ///                  relative URI (can only be null/empty if p_base
	  ///                  is not null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_base is null and p_uriSpec
	  ///                                  is not an absolute URI or if
	  ///                                  p_uriSpec violates syntax rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initialize(URI p_base, String p_uriSpec) throws MalformedURIException
	  private void initialize(URI p_base, string p_uriSpec)
	  {

		if (p_base == null && (string.ReferenceEquals(p_uriSpec, null) || p_uriSpec.Trim().Length == 0))
		{
		  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, null)); //"Cannot initialize URI with empty parameters.");
		}

		// just make a copy of the base if spec is empty
		if (string.ReferenceEquals(p_uriSpec, null) || p_uriSpec.Trim().Length == 0)
		{
		  initialize(p_base);

		  return;
		}

		string uriSpec = p_uriSpec.Trim();
		int uriSpecLen = uriSpec.Length;
		int index = 0;

		// check for scheme
		int colonIndex = uriSpec.IndexOf(':');
		if (colonIndex < 0)
		{
		  if (p_base == null)
		  {
			throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_SCHEME_IN_URI, new object[]{uriSpec})); //"No scheme found in URI: "+uriSpec);
		  }
		}
		else
		{
		  initializeScheme(uriSpec);
		  uriSpec = uriSpec.Substring(colonIndex + 1);
		  uriSpecLen = uriSpec.Length;
		}

		// two slashes means generic URI syntax, so we get the authority
		if (uriSpec.StartsWith("//", StringComparison.Ordinal))
		{
		  index += 2;

		  int startPos = index;

		  // get authority - everything up to path, query or fragment
		  char testChar = '\0';

		  while (index < uriSpecLen)
		  {
			testChar = uriSpec[index];

			if (testChar == '/' || testChar == '?' || testChar == '#')
			{
			  break;
			}

			index++;
		  }

		  // if we found authority, parse it out, otherwise we set the
		  // host to empty string
		  if (index > startPos)
		  {
			initializeAuthority(uriSpec.Substring(startPos, index - startPos));
		  }
		  else
		  {
			m_host = "";
		  }
		}

		initializePath(uriSpec.Substring(index));

		// Resolve relative URI to base URI - see RFC 2396 Section 5.2
		// In some cases, it might make more sense to throw an exception
		// (when scheme is specified is the string spec and the base URI
		// is also specified, for example), but we're just following the
		// RFC specifications 
		if (p_base != null)
		{

		  // check to see if this is the current doc - RFC 2396 5.2 #2
		  // note that this is slightly different from the RFC spec in that
		  // we don't include the check for query string being null
		  // - this handles cases where the urispec is just a query
		  // string or a fragment (e.g. "?y" or "#s") - 
		  // see <http://www.ics.uci.edu/~fielding/url/test1.html> which
		  // identified this as a bug in the RFC
		  if (m_path.Length == 0 && string.ReferenceEquals(m_scheme, null) && string.ReferenceEquals(m_host, null))
		  {
			m_scheme = p_base.Scheme;
			m_userinfo = p_base.Userinfo;
			m_host = p_base.Host;
			m_port = p_base.Port;
			m_path = p_base.Path;

			if (string.ReferenceEquals(m_queryString, null))
			{
			  m_queryString = p_base.QueryString;
			}

			return;
		  }

		  // check for scheme - RFC 2396 5.2 #3
		  // if we found a scheme, it means absolute URI, so we're done
		  if (string.ReferenceEquals(m_scheme, null))
		  {
			m_scheme = p_base.Scheme;
		  }

		  // check for authority - RFC 2396 5.2 #4
		  // if we found a host, then we've got a network path, so we're done
		  if (string.ReferenceEquals(m_host, null))
		  {
			m_userinfo = p_base.Userinfo;
			m_host = p_base.Host;
			m_port = p_base.Port;
		  }
		  else
		  {
			return;
		  }

		  // check for absolute path - RFC 2396 5.2 #5
		  if (m_path.Length > 0 && m_path.StartsWith("/", StringComparison.Ordinal))
		  {
			return;
		  }

		  // if we get to this point, we need to resolve relative path
		  // RFC 2396 5.2 #6
		  string path = "";
		  string basePath = p_base.Path;

		  // 6a - get all but the last segment of the base URI path
		  if (!string.ReferenceEquals(basePath, null))
		  {
			int lastSlash = basePath.LastIndexOf('/');

			if (lastSlash != -1)
			{
			  path = basePath.Substring(0, lastSlash + 1);
			}
		  }

		  // 6b - append the relative URI path
		  path = path + m_path;

		  // 6c - remove all "./" where "." is a complete path segment
		  index = -1;

		  while ((index = path.IndexOf("/./", StringComparison.Ordinal)) != -1)
		  {
			path = path.Substring(0, index + 1) + path.Substring(index + 3);
		  }

		  // 6d - remove "." if path ends with "." as a complete path segment
		  if (path.EndsWith("/.", StringComparison.Ordinal))
		  {
			path = path.Substring(0, path.Length - 1);
		  }

		  // 6e - remove all "<segment>/../" where "<segment>" is a complete 
		  // path segment not equal to ".."
		  index = -1;

		  int segIndex = -1;
		  string tempString = null;

		  while ((index = path.IndexOf("/../", StringComparison.Ordinal)) > 0)
		  {
			tempString = path.Substring(0, path.IndexOf("/../", StringComparison.Ordinal));
			segIndex = tempString.LastIndexOf('/');

			if (segIndex != -1)
			{
			  if (!tempString.Substring(segIndex++).Equals(".."))
			  {
				path = path.Substring(0, segIndex) + path.Substring(index + 4);
			  }
			}
		  }

		  // 6f - remove ending "<segment>/.." where "<segment>" is a 
		  // complete path segment
		  if (path.EndsWith("/..", StringComparison.Ordinal))
		  {
			tempString = path.Substring(0, path.Length - 3);
			segIndex = tempString.LastIndexOf('/');

			if (segIndex != -1)
			{
			  path = path.Substring(0, segIndex + 1);
			}
		  }

		  m_path = path;
		}
	  }

	  /// <summary>
	  /// Initialize the scheme for this URI from a URI string spec.
	  /// </summary>
	  /// <param name="p_uriSpec"> the URI specification (cannot be null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if URI does not have a conformant
	  ///                                  scheme </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initializeScheme(String p_uriSpec) throws MalformedURIException
	  private void initializeScheme(string p_uriSpec)
	  {

		int uriSpecLen = p_uriSpec.Length;
		int index = 0;
		string scheme = null;
		char testChar = '\0';

		while (index < uriSpecLen)
		{
		  testChar = p_uriSpec[index];

		  if (testChar == ':' || testChar == '/' || testChar == '?' || testChar == '#')
		  {
			break;
		  }

		  index++;
		}

		scheme = p_uriSpec.Substring(0, index);

		if (scheme.Length == 0)
		{
		  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_NO_SCHEME_INURI, null)); //"No scheme found in URI.");
		}
		else
		{
		  Scheme = scheme;
		}
	  }

	  /// <summary>
	  /// Initialize the authority (userinfo, host and port) for this
	  /// URI from a URI string spec.
	  /// </summary>
	  /// <param name="p_uriSpec"> the URI specification (cannot be null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_uriSpec violates syntax rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initializeAuthority(String p_uriSpec) throws MalformedURIException
	  private void initializeAuthority(string p_uriSpec)
	  {

		int index = 0;
		int start = 0;
		int end = p_uriSpec.Length;
		char testChar = '\0';
		string userinfo = null;

		// userinfo is everything up @
		if (p_uriSpec.IndexOf('@', start) != -1)
		{
		  while (index < end)
		  {
			testChar = p_uriSpec[index];

			if (testChar == '@')
			{
			  break;
			}

			index++;
		  }

		  userinfo = p_uriSpec.Substring(start, index - start);

		  index++;
		}

		// host is everything up to ':'
		string host = null;

		start = index;

		while (index < end)
		{
		  testChar = p_uriSpec[index];

		  if (testChar == ':')
		  {
			break;
		  }

		  index++;
		}

		host = p_uriSpec.Substring(start, index - start);

		int port = -1;

		if (host.Length > 0)
		{

		  // port
		  if (testChar == ':')
		  {
			index++;

			start = index;

			while (index < end)
			{
			  index++;
			}

			string portStr = p_uriSpec.Substring(start, index - start);

			if (portStr.Length > 0)
			{
			  for (int i = 0; i < portStr.Length; i++)
			  {
				if (!isDigit(portStr[i]))
				{
				  throw new MalformedURIException(portStr + " is invalid. Port should only contain digits!");
				}
			  }

			  try
			  {
				port = int.Parse(portStr);
			  }
			  catch (System.FormatException)
			  {

				// can't happen
			  }
			}
		  }
		}

		Host = host;
		Port = port;
		Userinfo = userinfo;
	  }

	  /// <summary>
	  /// Initialize the path for this URI from a URI string spec.
	  /// </summary>
	  /// <param name="p_uriSpec"> the URI specification (cannot be null)
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_uriSpec violates syntax rules </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initializePath(String p_uriSpec) throws MalformedURIException
	  private void initializePath(string p_uriSpec)
	  {

		if (string.ReferenceEquals(p_uriSpec, null))
		{
		  throw new MalformedURIException("Cannot initialize path from null string!");
		}

		int index = 0;
		int start = 0;
		int end = p_uriSpec.Length;
		char testChar = '\0';

		// path - everything up to query string or fragment
		while (index < end)
		{
		  testChar = p_uriSpec[index];

		  if (testChar == '?' || testChar == '#')
		  {
			break;
		  }

		  // check for valid escape sequence
		  if (testChar == '%')
		  {
			if (index + 2 >= end || !isHex(p_uriSpec[index + 1]) || !isHex(p_uriSpec[index + 2]))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, null)); //"Path contains invalid escape sequence!");
			}
		  }
		  else if (!isReservedCharacter(testChar) && !isUnreservedCharacter(testChar))
		  {
			if ('\\' != testChar)
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_PATH_INVALID_CHAR, new object[]{testChar.ToString()})); //"Path contains invalid character: "
			}
											  //+ testChar);
		  }

		  index++;
		}

		m_path = p_uriSpec.Substring(start, index - start);

		// query - starts with ? and up to fragment or end
		if (testChar == '?')
		{
		  index++;

		  start = index;

		  while (index < end)
		  {
			testChar = p_uriSpec[index];

			if (testChar == '#')
			{
			  break;
			}

			if (testChar == '%')
			{
			  if (index + 2 >= end || !isHex(p_uriSpec[index + 1]) || !isHex(p_uriSpec[index + 2]))
			  {
				throw new MalformedURIException("Query string contains invalid escape sequence!");
			  }
			}
			else if (!isReservedCharacter(testChar) && !isUnreservedCharacter(testChar))
			{
			  throw new MalformedURIException("Query string contains invalid character:" + testChar);
			}

			index++;
		  }

		  m_queryString = p_uriSpec.Substring(start, index - start);
		}

		// fragment - starts with #
		if (testChar == '#')
		{
		  index++;

		  start = index;

		  while (index < end)
		  {
			testChar = p_uriSpec[index];

			if (testChar == '%')
			{
			  if (index + 2 >= end || !isHex(p_uriSpec[index + 1]) || !isHex(p_uriSpec[index + 2]))
			  {
				throw new MalformedURIException("Fragment contains invalid escape sequence!");
			  }
			}
			else if (!isReservedCharacter(testChar) && !isUnreservedCharacter(testChar))
			{
			  throw new MalformedURIException("Fragment contains invalid character:" + testChar);
			}

			index++;
		  }

		  m_fragment = p_uriSpec.Substring(start, index - start);
		}
	  }

	  /// <summary>
	  /// Get the scheme for this URI.
	  /// </summary>
	  /// <returns> the scheme for this URI </returns>
	  public string Scheme
	  {
		  get
		  {
			return m_scheme;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_SCHEME_FROM_NULL_STRING, null)); //"Cannot set scheme from null string!");
			}
    
			if (!isConformantSchemeName(value))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_SCHEME_NOT_CONFORMANT, null)); //"The scheme is not conformant.");
			}
    
			m_scheme = value.ToLower();
		  }
	  }

	  /// <summary>
	  /// Get the scheme-specific part for this URI (everything following the
	  /// scheme and the first colon). See RFC 2396 Section 5.2 for spec.
	  /// </summary>
	  /// <returns> the scheme-specific part for this URI </returns>
	  public string SchemeSpecificPart
	  {
		  get
		  {
    
			StringBuilder schemespec = new StringBuilder();
    
			if (!string.ReferenceEquals(m_userinfo, null) || !string.ReferenceEquals(m_host, null) || m_port != -1)
			{
			  schemespec.Append("//");
			}
    
			if (!string.ReferenceEquals(m_userinfo, null))
			{
			  schemespec.Append(m_userinfo);
			  schemespec.Append('@');
			}
    
			if (!string.ReferenceEquals(m_host, null))
			{
			  schemespec.Append(m_host);
			}
    
			if (m_port != -1)
			{
			  schemespec.Append(':');
			  schemespec.Append(m_port);
			}
    
			if (!string.ReferenceEquals(m_path, null))
			{
			  schemespec.Append((m_path));
			}
    
			if (!string.ReferenceEquals(m_queryString, null))
			{
			  schemespec.Append('?');
			  schemespec.Append(m_queryString);
			}
    
			if (!string.ReferenceEquals(m_fragment, null))
			{
			  schemespec.Append('#');
			  schemespec.Append(m_fragment);
			}
    
			return schemespec.ToString();
		  }
	  }

	  /// <summary>
	  /// Get the userinfo for this URI.
	  /// </summary>
	  /// <returns> the userinfo for this URI (null if not specified). </returns>
	  public string Userinfo
	  {
		  get
		  {
			return m_userinfo;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null))
			{
			  m_userinfo = null;
			}
			else
			{
			  if (string.ReferenceEquals(m_host, null))
			  {
				throw new MalformedURIException("Userinfo cannot be set when host is null!");
			  }
    
			  // userinfo can contain alphanumerics, mark characters, escaped
			  // and ';',':','&','=','+','$',','
			  int index = 0;
			  int end = value.Length;
			  char testChar = '\0';
    
			  while (index < end)
			  {
				testChar = value[index];
    
				if (testChar == '%')
				{
				  if (index + 2 >= end || !isHex(value[index + 1]) || !isHex(value[index + 2]))
				  {
					throw new MalformedURIException("Userinfo contains invalid escape sequence!");
				  }
				}
				else if (!isUnreservedCharacter(testChar) && USERINFO_CHARACTERS.IndexOf(testChar) == -1)
				{
				  throw new MalformedURIException("Userinfo contains invalid character:" + testChar);
				}
    
				index++;
			  }
			}
    
			m_userinfo = value;
		  }
	  }

	  /// <summary>
	  /// Get the host for this URI.
	  /// </summary>
	  /// <returns> the host for this URI (null if not specified). </returns>
	  public string Host
	  {
		  get
		  {
			return m_host;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null) || value.Trim().Length == 0)
			{
			  m_host = value;
			  m_userinfo = null;
			  m_port = -1;
			}
			else if (!isWellFormedAddress(value))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, null)); //"Host is not a well formed address!");
			}
    
			m_host = value;
		  }
	  }

	  /// <summary>
	  /// Get the port for this URI.
	  /// </summary>
	  /// <returns> the port for this URI (-1 if not specified). </returns>
	  public int Port
	  {
		  get
		  {
			return m_port;
		  }
		  set
		  {
    
			if (value >= 0 && value <= 65535)
			{
			  if (string.ReferenceEquals(m_host, null))
			  {
				throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_PORT_WHEN_HOST_NULL, null)); //"Port cannot be set when host is null!");
			  }
			}
			else if (value != -1)
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_INVALID_PORT, null)); //"Invalid port number!");
			}
    
			m_port = value;
		  }
	  }

	  /// <summary>
	  /// Get the path for this URI (optionally with the query string and
	  /// fragment).
	  /// </summary>
	  /// <param name="p_includeQueryString"> if true (and query string is not null),
	  ///                             then a "?" followed by the query string
	  ///                             will be appended </param>
	  /// <param name="p_includeFragment"> if true (and fragment is not null),
	  ///                             then a "#" followed by the fragment
	  ///                             will be appended
	  /// </param>
	  /// <returns> the path for this URI possibly including the query string
	  ///         and fragment </returns>
	  public string getPath(bool p_includeQueryString, bool p_includeFragment)
	  {

		StringBuilder pathString = new StringBuilder(m_path);

		if (p_includeQueryString && !string.ReferenceEquals(m_queryString, null))
		{
		  pathString.Append('?');
		  pathString.Append(m_queryString);
		}

		if (p_includeFragment && !string.ReferenceEquals(m_fragment, null))
		{
		  pathString.Append('#');
		  pathString.Append(m_fragment);
		}

		return pathString.ToString();
	  }

	  /// <summary>
	  /// Get the path for this URI. Note that the value returned is the path
	  /// only and does not include the query string or fragment.
	  /// </summary>
	  /// <returns> the path for this URI. </returns>
	  public string Path
	  {
		  get
		  {
			return m_path;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null))
			{
			  m_path = null;
			  m_queryString = null;
			  m_fragment = null;
			}
			else
			{
			  initializePath(value);
			}
		  }
	  }

	  /// <summary>
	  /// Get the query string for this URI.
	  /// </summary>
	  /// <returns> the query string for this URI. Null is returned if there
	  ///         was no "?" in the URI spec, empty string if there was a
	  ///         "?" but no query string following it. </returns>
	  public string QueryString
	  {
		  get
		  {
			return m_queryString;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null))
			{
			  m_queryString = null;
			}
			else if (!GenericURI)
			{
			  throw new MalformedURIException("Query string can only be set for a generic URI!");
			}
			else if (string.ReferenceEquals(Path, null))
			{
			  throw new MalformedURIException("Query string cannot be set when path is null!");
			}
			else if (!isURIString(value))
			{
			  throw new MalformedURIException("Query string contains invalid character!");
			}
			else
			{
			  m_queryString = value;
			}
		  }
	  }

	  /// <summary>
	  /// Get the fragment for this URI.
	  /// </summary>
	  /// <returns> the fragment for this URI. Null is returned if there
	  ///         was no "#" in the URI spec, empty string if there was a
	  ///         "#" but no fragment following it. </returns>
	  public string Fragment
	  {
		  get
		  {
			return m_fragment;
		  }
		  set
		  {
    
			if (string.ReferenceEquals(value, null))
			{
			  m_fragment = null;
			}
			else if (!GenericURI)
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_FRAG_FOR_GENERIC_URI, null)); //"Fragment can only be set for a generic URI!");
			}
			else if (string.ReferenceEquals(Path, null))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_FRAG_WHEN_PATH_NULL, null)); //"Fragment cannot be set when path is null!");
			}
			else if (!isURIString(value))
			{
			  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_FRAG_INVALID_CHAR, null)); //"Fragment contains invalid character!");
			}
			else
			{
			  m_fragment = value;
			}
		  }
	  }






	  /// <summary>
	  /// Append to the end of the path of this URI. If the current path does
	  /// not end in a slash and the path to be appended does not begin with
	  /// a slash, a slash will be appended to the current path before the
	  /// new segment is added. Also, if the current path ends in a slash
	  /// and the new segment begins with a slash, the extra slash will be
	  /// removed before the new segment is appended.
	  /// </summary>
	  /// <param name="p_addToPath"> the new segment to be added to the current path
	  /// </param>
	  /// <exception cref="MalformedURIException"> if p_addToPath contains syntax
	  ///                                  errors </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void appendPath(String p_addToPath) throws MalformedURIException
	  public void appendPath(string p_addToPath)
	  {

		if (string.ReferenceEquals(p_addToPath, null) || p_addToPath.Trim().Length == 0)
		{
		  return;
		}

		if (!isURIString(p_addToPath))
		{
		  throw new MalformedURIException(Utils.messages.createMessage(MsgKey.ER_PATH_INVALID_CHAR, new object[]{p_addToPath})); //"Path contains invalid character!");
		}

		if (string.ReferenceEquals(m_path, null) || m_path.Trim().Length == 0)
		{
		  if (p_addToPath.StartsWith("/", StringComparison.Ordinal))
		  {
			m_path = p_addToPath;
		  }
		  else
		  {
			m_path = "/" + p_addToPath;
		  }
		}
		else if (m_path.EndsWith("/", StringComparison.Ordinal))
		{
		  if (p_addToPath.StartsWith("/", StringComparison.Ordinal))
		  {
			m_path = m_path + p_addToPath.Substring(1);
		  }
		  else
		  {
			m_path = m_path + p_addToPath;
		  }
		}
		else
		{
		  if (p_addToPath.StartsWith("/", StringComparison.Ordinal))
		  {
			m_path = m_path + p_addToPath;
		  }
		  else
		  {
			m_path = m_path + "/" + p_addToPath;
		  }
		}
	  }



	  /// <summary>
	  /// Determines if the passed-in Object is equivalent to this URI.
	  /// </summary>
	  /// <param name="p_test"> the Object to test for equality.
	  /// </param>
	  /// <returns> true if p_test is a URI with all values equal to this
	  ///         URI, false otherwise </returns>
	  public override bool Equals(object p_test)
	  {

		if (p_test is URI)
		{
		  URI testURI = (URI) p_test;

		  if (((string.ReferenceEquals(m_scheme, null) && string.ReferenceEquals(testURI.m_scheme, null)) || (!string.ReferenceEquals(m_scheme, null) && !string.ReferenceEquals(testURI.m_scheme, null) && m_scheme.Equals(testURI.m_scheme))) && ((string.ReferenceEquals(m_userinfo, null) && string.ReferenceEquals(testURI.m_userinfo, null)) || (!string.ReferenceEquals(m_userinfo, null) && !string.ReferenceEquals(testURI.m_userinfo, null) && m_userinfo.Equals(testURI.m_userinfo))) && ((string.ReferenceEquals(m_host, null) && string.ReferenceEquals(testURI.m_host, null)) || (!string.ReferenceEquals(m_host, null) && !string.ReferenceEquals(testURI.m_host, null) && m_host.Equals(testURI.m_host))) && m_port == testURI.m_port && ((string.ReferenceEquals(m_path, null) && string.ReferenceEquals(testURI.m_path, null)) || (!string.ReferenceEquals(m_path, null) && !string.ReferenceEquals(testURI.m_path, null) && m_path.Equals(testURI.m_path))) && ((string.ReferenceEquals(m_queryString, null) && string.ReferenceEquals(testURI.m_queryString, null)) || (!string.ReferenceEquals(m_queryString, null) && !string.ReferenceEquals(testURI.m_queryString, null) && m_queryString.Equals(testURI.m_queryString))) && ((string.ReferenceEquals(m_fragment, null) && string.ReferenceEquals(testURI.m_fragment, null)) || (!string.ReferenceEquals(m_fragment, null) && !string.ReferenceEquals(testURI.m_fragment, null) && m_fragment.Equals(testURI.m_fragment))))
		  {
			return true;
		  }
		}

		return false;
	  }

	  /// <summary>
	  /// Get the URI as a string specification. See RFC 2396 Section 5.2.
	  /// </summary>
	  /// <returns> the URI string specification </returns>
	  public override string ToString()
	  {

		StringBuilder uriSpecString = new StringBuilder();

		if (!string.ReferenceEquals(m_scheme, null))
		{
		  uriSpecString.Append(m_scheme);
		  uriSpecString.Append(':');
		}

		uriSpecString.Append(SchemeSpecificPart);

		return uriSpecString.ToString();
	  }

	  /// <summary>
	  /// Get the indicator as to whether this URI uses the "generic URI"
	  /// syntax.
	  /// </summary>
	  /// <returns> true if this URI uses the "generic URI" syntax, false
	  ///         otherwise </returns>
	  public bool GenericURI
	  {
		  get
		  {
    
			// presence of the host (whether valid or empty) means 
			// double-slashes which means generic uri
			return (!string.ReferenceEquals(m_host, null));
		  }
	  }

	  /// <summary>
	  /// Determine whether a scheme conforms to the rules for a scheme name.
	  /// A scheme is conformant if it starts with an alphanumeric, and
	  /// contains only alphanumerics, '+','-' and '.'.
	  /// 
	  /// </summary>
	  /// <param name="p_scheme"> The sheme name to check </param>
	  /// <returns> true if the scheme is conformant, false otherwise </returns>
	  public static bool isConformantSchemeName(string p_scheme)
	  {

		if (string.ReferenceEquals(p_scheme, null) || p_scheme.Trim().Length == 0)
		{
		  return false;
		}

		if (!isAlpha(p_scheme[0]))
		{
		  return false;
		}

		char testChar;

		for (int i = 1; i < p_scheme.Length; i++)
		{
		  testChar = p_scheme[i];

		  if (!isAlphanum(testChar) && SCHEME_CHARACTERS.IndexOf(testChar) == -1)
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// Determine whether a string is syntactically capable of representing
	  /// a valid IPv4 address or the domain name of a network host. A valid
	  /// IPv4 address consists of four decimal digit groups separated by a
	  /// '.'. A hostname consists of domain labels (each of which must
	  /// begin and end with an alphanumeric but may contain '-') separated
	  /// & by a '.'. See RFC 2396 Section 3.2.2.
	  /// 
	  /// </summary>
	  /// <param name="p_address"> The address string to check </param>
	  /// <returns> true if the string is a syntactically valid IPv4 address
	  ///              or hostname </returns>
	  public static bool isWellFormedAddress(string p_address)
	  {

		if (string.ReferenceEquals(p_address, null))
		{
		  return false;
		}

		string address = p_address.Trim();
		int addrLength = address.Length;

		if (addrLength == 0 || addrLength > 255)
		{
		  return false;
		}

		if (address.StartsWith(".", StringComparison.Ordinal) || address.StartsWith("-", StringComparison.Ordinal))
		{
		  return false;
		}

		// rightmost domain label starting with digit indicates IP address
		// since top level domain label can only start with an alpha
		// see RFC 2396 Section 3.2.2
		int index = address.LastIndexOf('.');

		if (address.EndsWith(".", StringComparison.Ordinal))
		{
		  index = address.Substring(0, index).LastIndexOf('.');
		}

		if (index + 1 < addrLength && isDigit(p_address[index + 1]))
		{
		  char testChar;
		  int numDots = 0;

		  // make sure that 1) we see only digits and dot separators, 2) that
		  // any dot separator is preceded and followed by a digit and 
		  // 3) that we find 3 dots
		  for (int i = 0; i < addrLength; i++)
		  {
			testChar = address[i];

			if (testChar == '.')
			{
			  if (!isDigit(address[i - 1]) || (i + 1 < addrLength && !isDigit(address[i + 1])))
			  {
				return false;
			  }

			  numDots++;
			}
			else if (!isDigit(testChar))
			{
			  return false;
			}
		  }

		  if (numDots != 3)
		  {
			return false;
		  }
		}
		else
		{

		  // domain labels can contain alphanumerics and '-"
		  // but must start and end with an alphanumeric
		  char testChar;

		  for (int i = 0; i < addrLength; i++)
		  {
			testChar = address[i];

			if (testChar == '.')
			{
			  if (!isAlphanum(address[i - 1]))
			  {
				return false;
			  }

			  if (i + 1 < addrLength && !isAlphanum(address[i + 1]))
			  {
				return false;
			  }
			}
			else if (!isAlphanum(testChar) && testChar != '-')
			{
			  return false;
			}
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// Determine whether a char is a digit.
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the char is betweeen '0' and '9', false otherwise </returns>
	  private static bool isDigit(char p_char)
	  {
		return p_char >= '0' && p_char <= '9';
	  }

	  /// <summary>
	  /// Determine whether a character is a hexadecimal character.
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the char is betweeen '0' and '9', 'a' and 'f'
	  ///         or 'A' and 'F', false otherwise </returns>
	  private static bool isHex(char p_char)
	  {
		return (isDigit(p_char) || (p_char >= 'a' && p_char <= 'f') || (p_char >= 'A' && p_char <= 'F'));
	  }

	  /// <summary>
	  /// Determine whether a char is an alphabetic character: a-z or A-Z
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the char is alphabetic, false otherwise </returns>
	  private static bool isAlpha(char p_char)
	  {
		return ((p_char >= 'a' && p_char <= 'z') || (p_char >= 'A' && p_char <= 'Z'));
	  }

	  /// <summary>
	  /// Determine whether a char is an alphanumeric: 0-9, a-z or A-Z
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the char is alphanumeric, false otherwise </returns>
	  private static bool isAlphanum(char p_char)
	  {
		return (isAlpha(p_char) || isDigit(p_char));
	  }

	  /// <summary>
	  /// Determine whether a character is a reserved character:
	  /// ';', '/', '?', ':', '@', '&', '=', '+', '$' or ','
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the string contains any reserved characters </returns>
	  private static bool isReservedCharacter(char p_char)
	  {
		return RESERVED_CHARACTERS.IndexOf(p_char) != -1;
	  }

	  /// <summary>
	  /// Determine whether a char is an unreserved character.
	  /// 
	  /// </summary>
	  /// <param name="p_char"> the character to check </param>
	  /// <returns> true if the char is unreserved, false otherwise </returns>
	  private static bool isUnreservedCharacter(char p_char)
	  {
		return (isAlphanum(p_char) || MARK_CHARACTERS.IndexOf(p_char) != -1);
	  }

	  /// <summary>
	  /// Determine whether a given string contains only URI characters (also
	  /// called "uric" in RFC 2396). uric consist of all reserved
	  /// characters, unreserved characters and escaped characters.
	  /// 
	  /// </summary>
	  /// <param name="p_uric"> URI string </param>
	  /// <returns> true if the string is comprised of uric, false otherwise </returns>
	  private static bool isURIString(string p_uric)
	  {

		if (string.ReferenceEquals(p_uric, null))
		{
		  return false;
		}

		int end = p_uric.Length;
		char testChar = '\0';

		for (int i = 0; i < end; i++)
		{
		  testChar = p_uric[i];

		  if (testChar == '%')
		  {
			if (i + 2 >= end || !isHex(p_uric[i + 1]) || !isHex(p_uric[i + 2]))
			{
			  return false;
			}
			else
			{
			  i += 2;

			  continue;
			}
		  }

		  if (isReservedCharacter(testChar) || isUnreservedCharacter(testChar))
		  {
			continue;
		  }
		  else
		  {
			return false;
		  }
		}

		return true;
	  }
	}

}