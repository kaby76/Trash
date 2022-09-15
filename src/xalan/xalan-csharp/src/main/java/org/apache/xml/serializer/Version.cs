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
 * $Id: Version.java 1581504 2014-03-25 20:45:47Z ggregory $
 */
namespace org.apache.xml.serializer
{
	/// <summary>
	/// Administrative class to keep track of the version number of
	/// the Serializer release.
	/// <P>This class implements the upcoming standard of having
	/// org.apache.project-name.Version.getVersion() be a standard way 
	/// to get version information.</P> 
	/// @xsl.usage general
	/// </summary>
	public sealed class Version
	{

	  /// <summary>
	  /// Get the basic version string for the current Serializer.
	  /// Version String formatted like 
	  /// <CODE>"<B>Serializer</B> <B>Java</B> v.r[.dd| <B>D</B>nn]"</CODE>.
	  /// 
	  /// Futurework: have this read version info from jar manifest.
	  /// </summary>
	  /// <returns> String denoting our current version </returns>
	  public static string Version
	  {
		  get
		  {
			 return Product + " " + ImplementationLanguage + " " + MajorVersionNum + "." + ReleaseVersionNum + "." + ((DevelopmentVersionNum > 0) ? ("D" + DevelopmentVersionNum) : ("" + MaintenanceVersionNum));
		  }
	  }

	  /// <summary>
	  /// Print the processor version to the command line.
	  /// </summary>
	  /// <param name="argv"> command line arguments, unused. </param>
	  public static void Main(string[] argv)
	  {
		Console.WriteLine(Version);
	  }

	  /// <summary>
	  /// Name of product: Serializer.
	  /// </summary>
	  public static string Product
	  {
		  get
		  {
			return "Serializer";
		  }
	  }

	  /// <summary>
	  /// Implementation Language: Java.
	  /// </summary>
	  public static string ImplementationLanguage
	  {
		  get
		  {
			return "Java";
		  }
	  }


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
	  public static int MajorVersionNum
	  {
		  get
		  {
			return 2;
    
		  }
	  }

	  /// <summary>
	  /// Release Number.
	  /// Release number. This changes when:
	  ///            -  a new set of functionality is to be added, eg,
	  ///               implementation of a new W3C specification.
	  ///            -  API or behaviour change.
	  ///            -  its designated as a reference release.
	  /// </summary>
	  public static int ReleaseVersionNum
	  {
		  get
		  {
			return 7;
		  }
	  }

	  /// <summary>
	  /// Maintenance Drop Number.
	  /// Optional identifier used to designate maintenance
	  ///          drop applied to a specific release and contains
	  ///          fixes for defects reported. It maintains compatibility
	  ///          with the release and contains no API changes.
	  ///          When missing, it designates the final and complete
	  ///          development drop for a release.
	  /// </summary>
	  public static int MaintenanceVersionNum
	  {
		  get
		  {
			return 2;
		  }
	  }

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
	  public static int DevelopmentVersionNum
	  {
		  get
		  {
			try
			{
				if (("").Length == 0)
				{
				  return 0;
				}
				else
				{
				  return int.Parse("");
				}
			}
			catch (System.FormatException)
			{
				   return 0;
			}
		  }
	  }
	}

}