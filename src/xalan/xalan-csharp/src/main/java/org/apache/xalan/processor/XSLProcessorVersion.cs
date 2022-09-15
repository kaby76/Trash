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
 * $Id: XSLProcessorVersion.src 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	/// <summary>
	/// Administrative class to keep track of the version number of
	/// the Xalan release.
	/// <P>See also: org/apache/xalan/res/XSLTInfo.properties</P> </summary>
	/// @deprecated To be replaced by org.apache.xalan.Version.getVersion()
	/// @xsl.usage general 
	public class XSLProcessorVersion
	{

	  /// <summary>
	  /// Print the processor version to the command line.
	  /// </summary>
	  /// <param name="argv"> command line arguments, unused. </param>
	  public static void Main(string[] argv)
	  {
		Console.WriteLine(S_VERSION);
	  }

	  /// <summary>
	  /// Constant name of product.
	  /// </summary>
	  public const string PRODUCT = "Xalan";

	  /// <summary>
	  /// Implementation Language.
	  /// </summary>
	  public const string LANGUAGE = "Java";

	  /// <summary>
	  /// Major version number.
	  /// Version number. This changes only when there is a
	  ///          significant, externally apparent enhancement from
	  ///          the previous release. 'n' represents the n'th
	  ///          version.
	  /// 
	  ///          Clients should carefully consider the implications
	  ///          of new versions as external interfaces and behaviour
	  ///          may have changed.
	  /// </summary>
	  public const int VERSION = 2;

	  /// <summary>
	  /// Release Number.
	  /// Release number. This changes when:
	  ///            -  a new set of functionality is to be added, eg,
	  ///               implementation of a new W3C specification.
	  ///            -  API or behaviour change.
	  ///            -  its designated as a reference release.
	  /// </summary>
	  public const int RELEASE = 7;

	  /// <summary>
	  /// Maintenance Drop Number.
	  /// Optional identifier used to designate maintenance
	  ///          drop applied to a specific release and contains
	  ///          fixes for defects reported. It maintains compatibility
	  ///          with the release and contains no API changes.
	  ///          When missing, it designates the final and complete
	  ///          development drop for a release.
	  /// </summary>
	  public const int MAINTENANCE = 2;

	  /// <summary>
	  /// Development Drop Number.
	  /// Optional identifier designates development drop of
	  ///          a specific release. D01 is the first development drop
	  ///          of a new release.
	  /// 
	  ///          Development drops are works in progress towards a
	  ///          compeleted, final release. A specific development drop
	  ///          may not completely implement all aspects of a new
	  ///          feature, which may take several development drops to
	  ///          complete. At the point of the final drop for the
	  ///          release, the D suffix will be omitted.
	  /// 
	  ///          Each 'D' drops can contain functional enhancements as
	  ///          well as defect fixes. 'D' drops may not be as stable as
	  ///          the final releases.
	  /// </summary>
	  public const int DEVELOPMENT = 0;

	  /// <summary>
	  /// Version String like <CODE>"<B>Xalan</B> <B>Language</B>
	  /// v.r[.dd| <B>D</B>nn]"</CODE>.
	  /// <P>Semantics of the version string are identical to the Xerces project.</P>
	  /// </summary>
	  public static readonly string S_VERSION = PRODUCT + " " + LANGUAGE + " " + VERSION + "." + RELEASE + "." + (DEVELOPMENT > 0 ? ("D" + DEVELOPMENT) : ("" + MAINTENANCE));

	}

}