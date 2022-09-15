using System;
using System.Collections;
using System.IO;
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
 * $Id: EnvironmentCheck.java 468646 2006-10-28 06:57:58Z minchau $
 */
namespace org.apache.xalan.xslt
{

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;
	using Node = org.w3c.dom.Node;

	/// <summary>
	/// Utility class to report simple information about the environment.
	/// Simplistic reporting about certain classes found in your JVM may 
	/// help answer some FAQs for simple problems.
	/// 
	/// <para>Usage-command line:  
	/// <code>
	/// java org.apache.xalan.xslt.EnvironmentCheck [-out outFile]
	/// </code></para>
	/// 
	/// <para>Usage-from program:  
	/// <code>
	/// boolean environmentOK = 
	/// (new EnvironmentCheck()).checkEnvironment(yourPrintWriter);
	/// </code></para>
	/// 
	/// <para>Usage-from stylesheet:  
	/// <code><pre>
	///    &lt;?xml version="1.0"?&gt;
	///    &lt;xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
	///        xmlns:xalan="http://xml.apache.org/xalan"
	///        exclude-result-prefixes="xalan"&gt;
	///    &lt;xsl:output indent="yes"/&gt;
	///    &lt;xsl:template match="/"&gt;
	///      &lt;xsl:copy-of select="xalan:checkEnvironment()"/&gt;
	///    &lt;/xsl:template&gt;
	///    &lt;/xsl:stylesheet&gt;
	/// </pre></code></para>
	/// 
	/// <para>Xalan users reporting problems are encouraged to use this class 
	/// to see if there are potential problems with their actual 
	/// Java environment <b>before</b> reporting a bug.  Note that you 
	/// should both check from the JVM/JRE's command line as well as 
	/// temporarily calling checkEnvironment() directly from your code, 
	/// since the classpath may differ (especially for servlets, etc).</para>
	/// 
	/// <para>Also see http://xml.apache.org/xalan-j/faq.html</para>
	/// 
	/// <para>Note: This class is pretty simplistic: 
	/// results are not necessarily definitive nor will it find all 
	/// problems related to environment setup.  Also, you should avoid 
	/// calling this in deployed production code, both because it is 
	/// quite slow and because it forces classes to get loaded.</para>
	/// 
	/// <para>Note: This class explicitly has very limited compile-time 
	/// dependencies to enable easy compilation and usage even when 
	/// Xalan, DOM/SAX/JAXP, etc. are not present.</para>
	/// 
	/// <para>Note: for an improved version of this utility, please see 
	/// the xml-commons' project Which utility which does the same kind 
	/// of thing but in a much simpler manner.</para>
	/// 
	/// @author Shane_Curcuru@us.ibm.com
	/// @version $Id: EnvironmentCheck.java 468646 2006-10-28 06:57:58Z minchau $
	/// </summary>
	public class EnvironmentCheck
	{

	  /// <summary>
	  /// Command line runnability: checks for [-out outFilename] arg.
	  /// <para>Command line entrypoint; Sets output and calls 
	  /// <seealso cref="checkEnvironment(PrintWriter)"/>.</para> </summary>
	  /// <param name="args"> command line args </param>
	  public static void Main(string[] args)
	  {
		// Default to System.out, autoflushing
		PrintWriter sendOutputTo = new PrintWriter(System.out, true);

		// Read our simplistic input args, if supplied
		for (int i = 0; i < args.Length; i++)
		{
		  if ("-out".Equals(args[i], StringComparison.OrdinalIgnoreCase))
		  {
			i++;

			if (i < args.Length)
			{
			  try
			  {
				sendOutputTo = new PrintWriter(new StreamWriter(args[i], true));
			  }
			  catch (Exception e)
			  {
				Console.Error.WriteLine("# WARNING: -out " + args[i] + " threw " + e.ToString());
			  }
			}
			else
			{
			  Console.Error.WriteLine("# WARNING: -out argument should have a filename, output sent to console");
			}
		  }
		}

		EnvironmentCheck app = new EnvironmentCheck();
		app.checkEnvironment(sendOutputTo);
	  }

	  /// <summary>
	  /// Programmatic entrypoint: Report on basic Java environment 
	  /// and CLASSPATH settings that affect Xalan.
	  /// 
	  /// <para>Note that this class is not advanced enough to tell you 
	  /// everything about the environment that affects Xalan, and 
	  /// sometimes reports errors that will not actually affect 
	  /// Xalan's behavior.  Currently, it very simplistically 
	  /// checks the JVM's environment for some basic properties and 
	  /// logs them out; it will report a problem if it finds a setting 
	  /// or .jar file that is <i>likely</i> to cause problems.</para>
	  /// 
	  /// <para>Advanced users can peruse the code herein to help them 
	  /// investigate potential environment problems found; other users 
	  /// may simply send the output from this tool along with any bugs 
	  /// they submit to help us in the debugging process.</para>
	  /// </summary>
	  /// <param name="pw"> PrintWriter to send output to; can be sent to a 
	  /// file that will look similar to a Properties file; defaults 
	  /// to System.out if null </param>
	  /// <returns> true if your environment appears to have no major 
	  /// problems; false if potential environment problems found </returns>
	  /// <seealso cref=".getEnvironmentHash()"/>
	  public virtual bool checkEnvironment(PrintWriter pw)
	  {

		// Use user-specified output writer if non-null
		if (null != pw)
		{
		  outWriter = pw;
		}

		// Setup a hash to store various environment information in
		Hashtable hash = EnvironmentHash;

		// Check for ERROR keys in the hashtable, and print report
		bool environmentHasErrors = writeEnvironmentReport(hash);

		if (environmentHasErrors)
		{
		  // Note: many logMsg calls have # at the start to 
		  //  fake a property-file like output
		  logMsg("# WARNING: Potential problems found in your environment!");
		  logMsg("#    Check any 'ERROR' items above against the Xalan FAQs");
		  logMsg("#    to correct potential problems with your classes/jars");
		  logMsg("#    http://xml.apache.org/xalan-j/faq.html");
		  if (null != outWriter)
		  {
			outWriter.flush();
		  }
		  return false;
		}
		else
		{
		  logMsg("# YAHOO! Your environment seems to be OK.");
		  if (null != outWriter)
		  {
			outWriter.flush();
		  }
		  return true;
		}
	  }

	  /// <summary>
	  /// Fill a hash with basic environment settings that affect Xalan.
	  /// 
	  /// <para>Worker method called from various places.</para>
	  /// <para>Various system and CLASSPATH, etc. properties are put into 
	  /// the hash as keys with a brief description of the current state 
	  /// of that item as the value.  Any serious problems will be put in 
	  /// with a key that is prefixed with {@link #ERROR 'ERROR.'} so it
	  /// stands out in any resulting report; also a key with just that 
	  /// constant will be set as well for any error.</para>
	  /// <para>Note that some legitimate cases are flaged as potential 
	  /// errors - namely when a developer recompiles xalan.jar on their 
	  /// own - and even a non-error state doesn't guaruntee that 
	  /// everything in the environment is correct.  But this will help 
	  /// point out the most common classpath and system property
	  /// problems that we've seen.</para>   
	  /// </summary>
	  /// <returns> Hashtable full of useful environment info about Xalan 
	  /// and related system properties, etc. </returns>
	  public virtual Hashtable EnvironmentHash
	  {
		  get
		  {
			// Setup a hash to store various environment information in
			Hashtable hash = new Hashtable();
    
			// Call various worker methods to fill in the hash
			//  These are explicitly separate for maintenance and so 
			//  advanced users could call them standalone
			checkJAXPVersion(hash);
			checkProcessorVersion(hash);
			checkParserVersion(hash);
			checkAntVersion(hash);
			checkDOMVersion(hash);
			checkSAXVersion(hash);
			checkSystemProperties(hash);
    
			return hash;
		  }
	  }

	  /// <summary>
	  /// Dump a basic Xalan environment report to outWriter.  
	  /// 
	  /// <para>This dumps a simple header and then each of the entries in 
	  /// the Hashtable to our PrintWriter; it does special processing 
	  /// for entries that are .jars found in the classpath.</para>
	  /// </summary>
	  /// <param name="h"> Hashtable of items to report on; presumably
	  /// filled in by our various check*() methods </param>
	  /// <returns> true if your environment appears to have no major 
	  /// problems; false if potential environment problems found </returns>
	  /// <seealso cref=".appendEnvironmentReport(Node, Document, Hashtable)"
	  /// for an equivalent that appends to a Node instead/>
	  protected internal virtual bool writeEnvironmentReport(Hashtable h)
	  {

		if (null == h)
		{
		  logMsg("# ERROR: writeEnvironmentReport called with null Hashtable");
		  return false;
		}

		bool errors = false;

		logMsg("#---- BEGIN writeEnvironmentReport($Revision: 468646 $): Useful stuff found: ----");

		// Fake the Properties-like output
		for (System.Collections.IEnumerator keys = h.Keys.GetEnumerator(); keys.MoveNext();)
		{
		  object key = keys.Current;
		  string keyStr = (string) key;
		  try
		  {
			// Special processing for classes found..
			if (keyStr.StartsWith(FOUNDCLASSES, StringComparison.Ordinal))
			{
			  ArrayList v = (ArrayList) h[keyStr];
			  errors |= logFoundJars(v, keyStr);
			}
			// ..normal processing for all other entries
			else
			{
			  // Note: we could just check for the ERROR key by itself, 
			  //    since we now set that, but since we have to go 
			  //    through the whole hash anyway, do it this way,
			  //    which is safer for maintenance
			  if (keyStr.StartsWith(ERROR, StringComparison.Ordinal))
			  {
				errors = true;
			  }
			  logMsg(keyStr + "=" + h[keyStr]);
			}
		  }
		  catch (Exception e)
		  {
			logMsg("Reading-" + key + "= threw: " + e.ToString());
		  }
		}

		logMsg("#----- END writeEnvironmentReport: Useful properties found: -----");

		return errors;
	  }

	  /// <summary>
	  /// Prefixed to hash keys that signify serious problems. </summary>
	  public const string ERROR = "ERROR.";

	  /// <summary>
	  /// Added to descriptions that signify potential problems. </summary>
	  public const string WARNING = "WARNING.";

	  /// <summary>
	  /// Value for any error found. </summary>
	  public const string ERROR_FOUND = "At least one error was found!";

	  /// <summary>
	  /// Prefixed to hash keys that signify version numbers. </summary>
	  public const string VERSION = "version.";

	  /// <summary>
	  /// Prefixed to hash keys that signify .jars found in classpath. </summary>
	  public const string FOUNDCLASSES = "foundclasses.";

	  /// <summary>
	  /// Marker that a class or .jar was found. </summary>
	  public const string CLASS_PRESENT = "present-unknown-version";

	  /// <summary>
	  /// Marker that a class or .jar was not found. </summary>
	  public const string CLASS_NOTPRESENT = "not-present";

	  /// <summary>
	  /// Listing of common .jar files that include Xalan-related classes. </summary>
	  public string[] jarNames = new string[] {"xalan.jar", "xalansamples.jar", "xalanj1compat.jar", "xalanservlet.jar", "serializer.jar", "xerces.jar", "xercesImpl.jar", "testxsl.jar", "crimson.jar", "lotusxsl.jar", "jaxp.jar", "parser.jar", "dom.jar", "sax.jar", "xml.jar", "xml-apis.jar", "xsltc.jar"};

	  /// <summary>
	  /// Print out report of .jars found in a classpath. 
	  /// 
	  /// Takes the information encoded from a checkPathForJars() 
	  /// call and dumps it out to our PrintWriter.
	  /// </summary>
	  /// <param name="v"> Vector of Hashtables of .jar file info </param>
	  /// <param name="desc"> description to print out in header
	  /// </param>
	  /// <returns> false if OK, true if any .jars were reported 
	  /// as having errors </returns>
	  /// <seealso cref=".checkPathForJars(String, String[])"/>
	  protected internal virtual bool logFoundJars(ArrayList v, string desc)
	  {

		if ((null == v) || (v.Count < 1))
		{
		  return false;
		}

		bool errors = false;

		logMsg("#---- BEGIN Listing XML-related jars in: " + desc + " ----");

		for (int i = 0; i < v.Count; i++)
		{
		  Hashtable subhash = (Hashtable) v[i];

		  for (System.Collections.IEnumerator keys = subhash.Keys.GetEnumerator(); keys.MoveNext();)
		  {
			object key = keys.Current;
			string keyStr = (string) key;
			try
			{
			  if (keyStr.StartsWith(ERROR, StringComparison.Ordinal))
			  {
				errors = true;
			  }
			  logMsg(keyStr + "=" + subhash[keyStr]);

			}
			catch (Exception e)
			{
			  errors = true;
			  logMsg("Reading-" + key + "= threw: " + e.ToString());
			}
		  }
		}

		logMsg("#----- END Listing XML-related jars in: " + desc + " -----");

		return errors;
	  }

	  /// <summary>
	  /// Stylesheet extension entrypoint: Dump a basic Xalan 
	  /// environment report from getEnvironmentHash() to a Node.  
	  /// 
	  /// <para>Copy of writeEnvironmentReport that creates a Node suitable 
	  /// for other processing instead of a properties-like text output.
	  /// </para> </summary>
	  /// <param name="container"> Node to append our report to </param>
	  /// <param name="factory"> Document providing createElement, etc. services </param>
	  /// <param name="h"> Hash presumably from <seealso cref="getEnvironmentHash()"/> </param>
	  /// <seealso cref=".writeEnvironmentReport(Hashtable)"
	  /// for an equivalent that writes to a PrintWriter instead/>
	  public virtual void appendEnvironmentReport(Node container, Document factory, Hashtable h)
	  {
		if ((null == container) || (null == factory))
		{
		  return;
		}

		try
		{
		  Element envCheckNode = factory.createElement("EnvironmentCheck");
		  envCheckNode.setAttribute("version", "$Revision: 468646 $");
		  container.appendChild(envCheckNode);

		  if (null == h)
		  {
			Element statusNode = factory.createElement("status");
			statusNode.setAttribute("result", "ERROR");
			statusNode.appendChild(factory.createTextNode("appendEnvironmentReport called with null Hashtable!"));
			envCheckNode.appendChild(statusNode);
			return;
		  }

		  bool errors = false;

		  Element hashNode = factory.createElement("environment");
		  envCheckNode.appendChild(hashNode);

		  for (System.Collections.IEnumerator keys = h.Keys.GetEnumerator(); keys.MoveNext();)
		  {
			object key = keys.Current;
			string keyStr = (string) key;
			try
			{
			  // Special processing for classes found..
			  if (keyStr.StartsWith(FOUNDCLASSES, StringComparison.Ordinal))
			  {
				ArrayList v = (ArrayList) h[keyStr];
				// errors |= logFoundJars(v, keyStr);
				errors |= appendFoundJars(hashNode, factory, v, keyStr);
			  }
			  // ..normal processing for all other entries
			  else
			  {
				// Note: we could just check for the ERROR key by itself, 
				//    since we now set that, but since we have to go 
				//    through the whole hash anyway, do it this way,
				//    which is safer for maintenance
				if (keyStr.StartsWith(ERROR, StringComparison.Ordinal))
				{
				  errors = true;
				}
				Element node = factory.createElement("item");
				node.setAttribute("key", keyStr);
				node.appendChild(factory.createTextNode((string)h[keyStr]));
				hashNode.appendChild(node);
			  }
			}
			catch (Exception e)
			{
			  errors = true;
			  Element node = factory.createElement("item");
			  node.setAttribute("key", keyStr);
			  node.appendChild(factory.createTextNode(ERROR + " Reading " + key + " threw: " + e.ToString()));
			  hashNode.appendChild(node);
			}
		  } // end of for...

		  Element statusNode = factory.createElement("status");
		  statusNode.setAttribute("result", (errors ? "ERROR" : "OK"));
		  envCheckNode.appendChild(statusNode);
		}
		catch (Exception e2)
		{
		  Console.Error.WriteLine("appendEnvironmentReport threw: " + e2.ToString());
		  Console.WriteLine(e2.ToString());
		  Console.Write(e2.StackTrace);
		}
	  }

	  /// <summary>
	  /// Print out report of .jars found in a classpath. 
	  /// 
	  /// Takes the information encoded from a checkPathForJars() 
	  /// call and dumps it out to our PrintWriter.
	  /// </summary>
	  /// <param name="container"> Node to append our report to </param>
	  /// <param name="factory"> Document providing createElement, etc. services </param>
	  /// <param name="v"> Vector of Hashtables of .jar file info </param>
	  /// <param name="desc"> description to print out in header
	  /// </param>
	  /// <returns> false if OK, true if any .jars were reported 
	  /// as having errors </returns>
	  /// <seealso cref=".checkPathForJars(String, String[])"/>
	  protected internal virtual bool appendFoundJars(Node container, Document factory, ArrayList v, string desc)
	  {

		if ((null == v) || (v.Count < 1))
		{
		  return false;
		}

		bool errors = false;

		for (int i = 0; i < v.Count; i++)
		{
		  Hashtable subhash = (Hashtable) v[i];

		  for (System.Collections.IEnumerator keys = subhash.Keys.GetEnumerator(); keys.MoveNext();)
		  {
			object key = keys.Current;
			try
			{
			  string keyStr = (string) key;
			  if (keyStr.StartsWith(ERROR, StringComparison.Ordinal))
			  {
				errors = true;
			  }
			  Element node = factory.createElement("foundJar");
			  node.setAttribute("name", keyStr.Substring(0, keyStr.IndexOf("-", StringComparison.Ordinal)));
			  node.setAttribute("desc", keyStr.Substring(keyStr.IndexOf("-", StringComparison.Ordinal) + 1));
			  node.appendChild(factory.createTextNode((string)subhash[keyStr]));
			  container.appendChild(node);
			}
			catch (Exception e)
			{
			  errors = true;
			  Element node = factory.createElement("foundJar");
			  node.appendChild(factory.createTextNode(ERROR + " Reading " + key + " threw: " + e.ToString()));
			  container.appendChild(node);
			}
		  }
		}
		return errors;
	  }

	  /// <summary>
	  /// Fillin hash with info about SystemProperties.  
	  /// 
	  /// Logs java.class.path and other likely paths; then attempts 
	  /// to search those paths for .jar files with Xalan-related classes.
	  /// 
	  /// //@todo NOTE: We don't actually search java.ext.dirs for 
	  /// //  *.jar files therein! This should be updated
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  /// <seealso cref=".jarNames"/>
	  /// <seealso cref=".checkPathForJars(String, String[])"/>
	  protected internal virtual void checkSystemProperties(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		// Grab java version for later use
		try
		{
		  string javaVersion = System.getProperty("java.version");

		  h["java.version"] = javaVersion;
		}
		catch (SecurityException)
		{

		  // For applet context, etc.
		  h["java.version"] = "WARNING: SecurityException thrown accessing system version properties";
		}

		// Printout jar files on classpath(s) that may affect operation
		//  Do this in order
		try
		{

		  // This is present in all JVM's
		  string cp = System.getProperty("java.class.path");

		  h["java.class.path"] = cp;

		  ArrayList classpathJars = checkPathForJars(cp, jarNames);

		  if (null != classpathJars)
		  {
			h[FOUNDCLASSES + "java.class.path"] = classpathJars;
		  }

		  // Also check for JDK 1.2+ type classpaths
		  string othercp = System.getProperty("sun.boot.class.path");

		  if (null != othercp)
		  {
			h["sun.boot.class.path"] = othercp;

			classpathJars = checkPathForJars(othercp, jarNames);

			if (null != classpathJars)
			{
			  h[FOUNDCLASSES + "sun.boot.class.path"] = classpathJars;
			}
		  }

		  //@todo NOTE: We don't actually search java.ext.dirs for 
		  //  *.jar files therein! This should be updated
		  othercp = System.getProperty("java.ext.dirs");

		  if (null != othercp)
		  {
			h["java.ext.dirs"] = othercp;

			classpathJars = checkPathForJars(othercp, jarNames);

			if (null != classpathJars)
			{
			  h[FOUNDCLASSES + "java.ext.dirs"] = classpathJars;
			}
		  }

		  //@todo also check other System properties' paths?
		  //  v2 = checkPathForJars(System.getProperty("sun.boot.library.path"), jarNames);   // ?? may not be needed
		  //  v3 = checkPathForJars(System.getProperty("java.library.path"), jarNames);   // ?? may not be needed
		}
		catch (SecurityException)
		{
		  // For applet context, etc.
		  h["java.class.path"] = "WARNING: SecurityException thrown accessing system classpath properties";
		}
	  }

	  /// <summary>
	  /// Cheap-o listing of specified .jars found in the classpath. 
	  /// 
	  /// cp should be separated by the usual File.pathSeparator.  We 
	  /// then do a simplistic search of the path for any requested 
	  /// .jar filenames, and return a listing of their names and 
	  /// where (apparently) they came from.
	  /// </summary>
	  /// <param name="cp"> classpath to search </param>
	  /// <param name="jars"> array of .jar base filenames to look for
	  /// </param>
	  /// <returns> Vector of Hashtables filled with info about found .jars </returns>
	  /// <seealso cref=".jarNames"/>
	  /// <seealso cref=".logFoundJars(Vector, String)"/>
	  /// <seealso cref=".appendFoundJars(Node, Document, Vector, String )"/>
	  /// <seealso cref=".getApparentVersion(String, long)"/>
	  protected internal virtual ArrayList checkPathForJars(string cp, string[] jars)
	  {

		if ((null == cp) || (null == jars) || (0 == cp.Length) || (0 == jars.Length))
		{
		  return null;
		}

		ArrayList v = new ArrayList();
		StringTokenizer st = new StringTokenizer(cp, File.pathSeparator);

		while (st.hasMoreTokens())
		{

		  // Look at each classpath entry for each of our requested jarNames
		  string filename = st.nextToken();

		  for (int i = 0; i < jars.Length; i++)
		  {
			if (filename.IndexOf(jars[i], StringComparison.Ordinal) > -1)
			{
			  File f = new File(filename);

			  if (f.exists())
			  {

				// If any requested jarName exists, report on 
				//  the details of that .jar file
				try
				{
				  Hashtable h = new Hashtable(2);
				  // Note "-" char is looked for in appendFoundJars
				  h[jars[i] + "-path"] = f.getAbsolutePath();

				  // We won't bother reporting on the xalan.jar apparent version
				  // since this requires knowing the jar size of the xalan.jar
				  // before we build it. 
				  // For other jars, eg. xml-apis.jar and xercesImpl.jar, we 
				  // report the apparent version of the file we've found
				  if (!("xalan.jar".Equals(jars[i], StringComparison.OrdinalIgnoreCase)))
				  {
					h[jars[i] + "-apparent.version"] = getApparentVersion(jars[i], f.length());
				  }
				  v.Add(h);
				}
				catch (Exception)
				{

				  /* no-op, don't add it  */
				}
			  }
			  else
			  {
				Hashtable h = new Hashtable(2);
				// Note "-" char is looked for in appendFoundJars
				h[jars[i] + "-path"] = WARNING + " Classpath entry: " + filename + " does not exist";
				h[jars[i] + "-apparent.version"] = CLASS_NOTPRESENT;
				v.Add(h);
			  }
			}
		  }
		}

		return v;
	  }

	  /// <summary>
	  /// Cheap-o method to determine the product version of a .jar.   
	  /// 
	  /// Currently does a lookup into a local table of some recent 
	  /// shipped Xalan builds to determine where the .jar probably 
	  /// came from.  Note that if you recompile Xalan or Xerces 
	  /// yourself this will likely report a potential error, since 
	  /// we can't certify builds other than the ones we ship.
	  /// Only reports against selected posted Xalan-J builds.
	  /// 
	  /// //@todo actually look up version info in manifests
	  /// </summary>
	  /// <param name="jarName"> base filename of the .jarfile </param>
	  /// <param name="jarSize"> size of the .jarfile
	  /// </param>
	  /// <returns> String describing where the .jar file probably 
	  /// came from </returns>
	  protected internal virtual string getApparentVersion(string jarName, long jarSize)
	  {
		// If we found a matching size and it's for our 
		//  jar, then return it's description
		// Lookup in static jarVersions Hashtable
		string foundSize = (string) jarVersions[new long?(jarSize)];

		if ((null != foundSize) && (foundSize.StartsWith(jarName, StringComparison.Ordinal)))
		{
		  return foundSize;
		}
		else
		{
		  if ("xerces.jar".Equals(jarName, StringComparison.OrdinalIgnoreCase) || "xercesImpl.jar".Equals(jarName, StringComparison.OrdinalIgnoreCase))
		  {
	//              || "xalan.jar".equalsIgnoreCase(jarName))

			// For xalan.jar and xerces.jar/xercesImpl.jar, which we ship together:
			// The jar is not from a shipped copy of xalan-j, so 
			//  it's up to the user to ensure that it's compatible
			return jarName + " " + WARNING + CLASS_PRESENT;
		  }
		  else
		  {

			// Otherwise, it's just a jar we don't have the version info calculated for
			return jarName + " " + CLASS_PRESENT;
		  }
		}
	  }

	  /// <summary>
	  /// Report version information about JAXP interfaces.
	  /// 
	  /// Currently distinguishes between JAXP 1.0.1 and JAXP 1.1, 
	  /// and not found; only tests the interfaces, and does not 
	  /// check for reference implementation versions.
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkJAXPVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class noArgs[] = new Class[0];
		Type[] noArgs = new Type[0];
		Type clazz = null;

		try
		{
		  const string JAXP1_CLASS = "javax.xml.parsers.DocumentBuilder";
		  const string JAXP11_METHOD = "getDOMImplementation";

		  clazz = ObjectFactory.findProviderClass(JAXP1_CLASS, ObjectFactory.findClassLoader(), true);

		  System.Reflection.MethodInfo method = clazz.GetMethod(JAXP11_METHOD, noArgs);

		  // If we succeeded, we at least have JAXP 1.1 available
		  h[VERSION + "JAXP"] = "1.1 or higher";
		}
		catch (Exception)
		{
		  if (null != clazz)
		  {

			// We must have found the class itself, just not the 
			//  method, so we (probably) have JAXP 1.0.1
			h[ERROR + VERSION + "JAXP"] = "1.0.1";
			h[ERROR] = ERROR_FOUND;
		  }
		  else
		  {
			// We couldn't even find the class, and don't have 
			//  any JAXP support at all, or only have the 
			//  transform half of it
			h[ERROR + VERSION + "JAXP"] = CLASS_NOTPRESENT;
			h[ERROR] = ERROR_FOUND;
		  }
		}
	  }

	  /// <summary>
	  /// Report product version information from Xalan-J.
	  /// 
	  /// Looks for version info in xalan.jar from Xalan-J products.
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkProcessorVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		try
		{
		  const string XALAN1_VERSION_CLASS = "org.apache.xalan.xslt.XSLProcessorVersion";

		  Type clazz = ObjectFactory.findProviderClass(XALAN1_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  // Found Xalan-J 1.x, grab it's version fields
		  StringBuilder buf = new StringBuilder();
		  System.Reflection.FieldInfo f = clazz.GetField("PRODUCT");

		  buf.Append(f.get(null));
		  buf.Append(';');

		  f = clazz.GetField("LANGUAGE");

		  buf.Append(f.get(null));
		  buf.Append(';');

		  f = clazz.GetField("S_VERSION");

		  buf.Append(f.get(null));
		  buf.Append(';');
		  h[VERSION + "xalan1"] = buf.ToString();
		}
		catch (Exception)
		{
		  h[VERSION + "xalan1"] = CLASS_NOTPRESENT;
		}

		try
		{
		  // NOTE: This is the old Xalan 2.0, 2.1, 2.2 version class, 
		  //    is being replaced by class below
		  const string XALAN2_VERSION_CLASS = "org.apache.xalan.processor.XSLProcessorVersion";

		  Type clazz = ObjectFactory.findProviderClass(XALAN2_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  // Found Xalan-J 2.x, grab it's version fields
		  StringBuilder buf = new StringBuilder();
		  System.Reflection.FieldInfo f = clazz.GetField("S_VERSION");
		  buf.Append(f.get(null));

		  h[VERSION + "xalan2x"] = buf.ToString();
		}
		catch (Exception)
		{
		  h[VERSION + "xalan2x"] = CLASS_NOTPRESENT;
		}
		try
		{
		  // NOTE: This is the new Xalan 2.2+ version class
		  const string XALAN2_2_VERSION_CLASS = "org.apache.xalan.Version";
		  const string XALAN2_2_VERSION_METHOD = "getVersion";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class noArgs[] = new Class[0];
		  Type[] noArgs = new Type[0];

		  Type clazz = ObjectFactory.findProviderClass(XALAN2_2_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  System.Reflection.MethodInfo method = clazz.GetMethod(XALAN2_2_VERSION_METHOD, noArgs);
		  object returnValue = method.invoke(null, new object[0]);

		  h[VERSION + "xalan2_2"] = (string)returnValue;
		}
		catch (Exception)
		{
		  h[VERSION + "xalan2_2"] = CLASS_NOTPRESENT;
		}
	  }

	  /// <summary>
	  /// Report product version information from common parsers.
	  /// 
	  /// Looks for version info in xerces.jar/xercesImpl.jar/crimson.jar.
	  /// 
	  /// //@todo actually look up version info in crimson manifest
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkParserVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		try
		{
		  const string XERCES1_VERSION_CLASS = "org.apache.xerces.framework.Version";

		  Type clazz = ObjectFactory.findProviderClass(XERCES1_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  // Found Xerces-J 1.x, grab it's version fields
		  System.Reflection.FieldInfo f = clazz.GetField("fVersion");
		  string parserVersion = (string) f.get(null);

		  h[VERSION + "xerces1"] = parserVersion;
		}
		catch (Exception)
		{
		  h[VERSION + "xerces1"] = CLASS_NOTPRESENT;
		}

		// Look for xerces1 and xerces2 parsers separately
		try
		{
		  const string XERCES2_VERSION_CLASS = "org.apache.xerces.impl.Version";

		  Type clazz = ObjectFactory.findProviderClass(XERCES2_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  // Found Xerces-J 2.x, grab it's version fields
		  System.Reflection.FieldInfo f = clazz.GetField("fVersion");
		  string parserVersion = (string) f.get(null);

		  h[VERSION + "xerces2"] = parserVersion;
		}
		catch (Exception)
		{
		  h[VERSION + "xerces2"] = CLASS_NOTPRESENT;
		}

		try
		{
		  const string CRIMSON_CLASS = "org.apache.crimson.parser.Parser2";

		  Type clazz = ObjectFactory.findProviderClass(CRIMSON_CLASS, ObjectFactory.findClassLoader(), true);

		  //@todo determine specific crimson version
		  h[VERSION + "crimson"] = CLASS_PRESENT;
		}
		catch (Exception)
		{
		  h[VERSION + "crimson"] = CLASS_NOTPRESENT;
		}
	  }

	  /// <summary>
	  /// Report product version information from Ant.
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkAntVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		try
		{
		  const string ANT_VERSION_CLASS = "org.apache.tools.ant.Main";
		  const string ANT_VERSION_METHOD = "getAntVersion"; // noArgs
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class noArgs[] = new Class[0];
		  Type[] noArgs = new Type[0];

		  Type clazz = ObjectFactory.findProviderClass(ANT_VERSION_CLASS, ObjectFactory.findClassLoader(), true);

		  System.Reflection.MethodInfo method = clazz.GetMethod(ANT_VERSION_METHOD, noArgs);
		  object returnValue = method.invoke(null, new object[0]);

		  h[VERSION + "ant"] = (string)returnValue;
		}
		catch (Exception)
		{
		  h[VERSION + "ant"] = CLASS_NOTPRESENT;
		}
	  }

	  /// <summary>
	  /// Report version info from DOM interfaces. 
	  /// 
	  /// Currently distinguishes between pre-DOM level 2, the DOM 
	  /// level 2 working draft, the DOM level 2 final draft, 
	  /// and not found.
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkDOMVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		const string DOM_LEVEL2_CLASS = "org.w3c.dom.Document";
		const string DOM_LEVEL2_METHOD = "createElementNS"; // String, String
		const string DOM_LEVEL2WD_CLASS = "org.w3c.dom.Node";
		const string DOM_LEVEL2WD_METHOD = "supported"; // String, String
		const string DOM_LEVEL2FD_CLASS = "org.w3c.dom.Node";
		const string DOM_LEVEL2FD_METHOD = "isSupported"; // String, String
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class twoStringArgs[] = { java.lang.String.class, java.lang.String.class };
		Type[] twoStringArgs = new Type[] {typeof(string), typeof(string)};

		try
		{
		  Type clazz = ObjectFactory.findProviderClass(DOM_LEVEL2_CLASS, ObjectFactory.findClassLoader(), true);

		  System.Reflection.MethodInfo method = clazz.GetMethod(DOM_LEVEL2_METHOD, twoStringArgs);

		  // If we succeeded, we have loaded interfaces from a 
		  //  level 2 DOM somewhere
		  h[VERSION + "DOM"] = "2.0";

		  try
		  {
			// Check for the working draft version, which is 
			//  commonly found, but won't work anymore
			clazz = ObjectFactory.findProviderClass(DOM_LEVEL2WD_CLASS, ObjectFactory.findClassLoader(), true);

			method = clazz.GetMethod(DOM_LEVEL2WD_METHOD, twoStringArgs);

			h[ERROR + VERSION + "DOM.draftlevel"] = "2.0wd";
			h[ERROR] = ERROR_FOUND;
		  }
		  catch (Exception)
		  {
			try
			{
			  // Check for the final draft version as well
			  clazz = ObjectFactory.findProviderClass(DOM_LEVEL2FD_CLASS, ObjectFactory.findClassLoader(), true);

			  method = clazz.GetMethod(DOM_LEVEL2FD_METHOD, twoStringArgs);

			  h[VERSION + "DOM.draftlevel"] = "2.0fd";
			}
			catch (Exception)
			{
			  h[ERROR + VERSION + "DOM.draftlevel"] = "2.0unknown";
			  h[ERROR] = ERROR_FOUND;
			}
		  }
		}
		catch (Exception e)
		{
		  h[ERROR + VERSION + "DOM"] = "ERROR attempting to load DOM level 2 class: " + e.ToString();
		  h[ERROR] = ERROR_FOUND;
		}

		//@todo load an actual DOM implmementation and query it as well
		//@todo load an actual DOM implmementation and check if 
		//  isNamespaceAware() == true, which is needed to parse 
		//  xsl stylesheet files into a DOM
	  }

	  /// <summary>
	  /// Report version info from SAX interfaces. 
	  /// 
	  /// Currently distinguishes between SAX 2, SAX 2.0beta2, 
	  /// SAX1, and not found.
	  /// </summary>
	  /// <param name="h"> Hashtable to put information in </param>
	  protected internal virtual void checkSAXVersion(Hashtable h)
	  {

		if (null == h)
		{
		  h = new Hashtable();
		}

		const string SAX_VERSION1_CLASS = "org.xml.sax.Parser";
		const string SAX_VERSION1_METHOD = "parse"; // String
		const string SAX_VERSION2_CLASS = "org.xml.sax.XMLReader";
		const string SAX_VERSION2_METHOD = "parse"; // String
		const string SAX_VERSION2BETA_CLASSNF = "org.xml.sax.helpers.AttributesImpl";
		const string SAX_VERSION2BETA_METHODNF = "setAttributes"; // Attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class oneStringArg[] = { java.lang.String.class };
		Type[] oneStringArg = new Type[] {typeof(string)};
		// Note this introduces a minor compile dependency on SAX...
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class attributesArg[] = { org.xml.sax.Attributes.class };
		Type[] attributesArg = new Type[] {typeof(org.xml.sax.Attributes)};

		try
		{
		  // This method was only added in the final SAX 2.0 release; 
		  //  see changes.html "Changes from SAX 2.0beta2 to SAX 2.0prerelease"
		  Type clazz = ObjectFactory.findProviderClass(SAX_VERSION2BETA_CLASSNF, ObjectFactory.findClassLoader(), true);

		  System.Reflection.MethodInfo method = clazz.GetMethod(SAX_VERSION2BETA_METHODNF, attributesArg);

		  // If we succeeded, we have loaded interfaces from a 
		  //  real, final SAX version 2.0 somewhere
		  h[VERSION + "SAX"] = "2.0";
		}
		catch (Exception e)
		{
		  // If we didn't find the SAX 2.0 class, look for a 2.0beta2
		  h[ERROR + VERSION + "SAX"] = "ERROR attempting to load SAX version 2 class: " + e.ToString();
		  h[ERROR] = ERROR_FOUND;

		  try
		  {
			Type clazz = ObjectFactory.findProviderClass(SAX_VERSION2_CLASS, ObjectFactory.findClassLoader(), true);

			System.Reflection.MethodInfo method = clazz.GetMethod(SAX_VERSION2_METHOD, oneStringArg);

			// If we succeeded, we have loaded interfaces from a 
			//  SAX version 2.0beta2 or earlier; these might work but 
			//  you should really have the final SAX 2.0 
			h[VERSION + "SAX-backlevel"] = "2.0beta2-or-earlier";
		  }
		  catch (Exception)
		  {
			// If we didn't find the SAX 2.0beta2 class, look for a 1.0 one
			h[ERROR + VERSION + "SAX"] = "ERROR attempting to load SAX version 2 class: " + e.ToString();
			h[ERROR] = ERROR_FOUND;

			try
			{
			  Type clazz = ObjectFactory.findProviderClass(SAX_VERSION1_CLASS, ObjectFactory.findClassLoader(), true);

			  System.Reflection.MethodInfo method = clazz.GetMethod(SAX_VERSION1_METHOD, oneStringArg);

			  // If we succeeded, we have loaded interfaces from a 
			  //  SAX version 1.0 somewhere; which won't work very 
			  //  well for JAXP 1.1 or beyond!
			  h[VERSION + "SAX-backlevel"] = "1.0";
			}
			catch (Exception e3)
			{
			  // If we didn't find the SAX 2.0 class, look for a 1.0 one
			  // Note that either 1.0 or no SAX are both errors
			  h[ERROR + VERSION + "SAX-backlevel"] = "ERROR attempting to load SAX version 1 class: " + e3.ToString();

			}
		  }
		}
	  }

	  /// <summary>
	  /// Manual table of known .jar sizes.  
	  /// Only includes shipped versions of certain projects.
	  /// key=jarsize, value=jarname ' from ' distro name
	  /// Note assumption: two jars cannot have the same size!
	  /// </summary>
	  /// <seealso cref=".getApparentVersion(String, long)"/>
	  private static Hashtable jarVersions = new Hashtable();

	  /// <summary>
	  /// Static initializer for jarVersions table.  
	  /// Doing this just once saves time and space.
	  /// </summary>
	  /// <seealso cref=".getApparentVersion(String, long)"/>
	  static EnvironmentCheck()
	  {
		// Note: hackish Hashtable, this could use improvement
		jarVersions[new long?(857192)] = "xalan.jar from xalan-j_1_1";
		jarVersions[new long?(440237)] = "xalan.jar from xalan-j_1_2";
		jarVersions[new long?(436094)] = "xalan.jar from xalan-j_1_2_1";
		jarVersions[new long?(426249)] = "xalan.jar from xalan-j_1_2_2";
		jarVersions[new long?(702536)] = "xalan.jar from xalan-j_2_0_0";
		jarVersions[new long?(720930)] = "xalan.jar from xalan-j_2_0_1";
		jarVersions[new long?(732330)] = "xalan.jar from xalan-j_2_1_0";
		jarVersions[new long?(872241)] = "xalan.jar from xalan-j_2_2_D10";
		jarVersions[new long?(882739)] = "xalan.jar from xalan-j_2_2_D11";
		jarVersions[new long?(923866)] = "xalan.jar from xalan-j_2_2_0";
		jarVersions[new long?(905872)] = "xalan.jar from xalan-j_2_3_D1";
		jarVersions[new long?(906122)] = "xalan.jar from xalan-j_2_3_0";
		jarVersions[new long?(906248)] = "xalan.jar from xalan-j_2_3_1";
		jarVersions[new long?(983377)] = "xalan.jar from xalan-j_2_4_D1";
		jarVersions[new long?(997276)] = "xalan.jar from xalan-j_2_4_0";
		jarVersions[new long?(1031036)] = "xalan.jar from xalan-j_2_4_1";
		// Stop recording xalan.jar sizes as of Xalan Java 2.5.0    

		jarVersions[new long?(596540)] = "xsltc.jar from xalan-j_2_2_0";
		jarVersions[new long?(590247)] = "xsltc.jar from xalan-j_2_3_D1";
		jarVersions[new long?(589914)] = "xsltc.jar from xalan-j_2_3_0";
		jarVersions[new long?(589915)] = "xsltc.jar from xalan-j_2_3_1";
		jarVersions[new long?(1306667)] = "xsltc.jar from xalan-j_2_4_D1";
		jarVersions[new long?(1328227)] = "xsltc.jar from xalan-j_2_4_0";
		jarVersions[new long?(1344009)] = "xsltc.jar from xalan-j_2_4_1";
		jarVersions[new long?(1348361)] = "xsltc.jar from xalan-j_2_5_D1";
		// Stop recording xsltc.jar sizes as of Xalan Java 2.5.0

		jarVersions[new long?(1268634)] = "xsltc.jar-bundled from xalan-j_2_3_0";

		jarVersions[new long?(100196)] = "xml-apis.jar from xalan-j_2_2_0 or xalan-j_2_3_D1";
		jarVersions[new long?(108484)] = "xml-apis.jar from xalan-j_2_3_0, or xalan-j_2_3_1 from xml-commons-1.0.b2";
		jarVersions[new long?(109049)] = "xml-apis.jar from xalan-j_2_4_0 from xml-commons RIVERCOURT1 branch";
		jarVersions[new long?(113749)] = "xml-apis.jar from xalan-j_2_4_1 from factoryfinder-build of xml-commons RIVERCOURT1";
		jarVersions[new long?(124704)] = "xml-apis.jar from tck-jaxp-1_2_0 branch of xml-commons";
		jarVersions[new long?(124724)] = "xml-apis.jar from tck-jaxp-1_2_0 branch of xml-commons, tag: xml-commons-external_1_2_01";
		jarVersions[new long?(194205)] = "xml-apis.jar from head branch of xml-commons, tag: xml-commons-external_1_3_02";

		// If the below were more common I would update it to report 
		//  errors better; but this is so old hardly anyone has it
		jarVersions[new long?(424490)] = "xalan.jar from Xerces Tools releases - ERROR:DO NOT USE!";

		jarVersions[new long?(1591855)] = "xerces.jar from xalan-j_1_1 from xerces-1...";
		jarVersions[new long?(1498679)] = "xerces.jar from xalan-j_1_2 from xerces-1_2_0.bin";
		jarVersions[new long?(1484896)] = "xerces.jar from xalan-j_1_2_1 from xerces-1_2_1.bin";
		jarVersions[new long?(804460)] = "xerces.jar from xalan-j_1_2_2 from xerces-1_2_2.bin";
		jarVersions[new long?(1499244)] = "xerces.jar from xalan-j_2_0_0 from xerces-1_2_3.bin";
		jarVersions[new long?(1605266)] = "xerces.jar from xalan-j_2_0_1 from xerces-1_3_0.bin";
		jarVersions[new long?(904030)] = "xerces.jar from xalan-j_2_1_0 from xerces-1_4.bin";
		jarVersions[new long?(904030)] = "xerces.jar from xerces-1_4_0.bin";
		jarVersions[new long?(1802885)] = "xerces.jar from xerces-1_4_2.bin";
		jarVersions[new long?(1734594)] = "xerces.jar from Xerces-J-bin.2.0.0.beta3";
		jarVersions[new long?(1808883)] = "xerces.jar from xalan-j_2_2_D10,D11,D12 or xerces-1_4_3.bin";
		jarVersions[new long?(1812019)] = "xerces.jar from xalan-j_2_2_0";
		jarVersions[new long?(1720292)] = "xercesImpl.jar from xalan-j_2_3_D1";
		jarVersions[new long?(1730053)] = "xercesImpl.jar from xalan-j_2_3_0 or xalan-j_2_3_1 from xerces-2_0_0";
		jarVersions[new long?(1728861)] = "xercesImpl.jar from xalan-j_2_4_D1 from xerces-2_0_1";
		jarVersions[new long?(972027)] = "xercesImpl.jar from xalan-j_2_4_0 from xerces-2_1";
		jarVersions[new long?(831587)] = "xercesImpl.jar from xalan-j_2_4_1 from xerces-2_2";
		jarVersions[new long?(891817)] = "xercesImpl.jar from xalan-j_2_5_D1 from xerces-2_3";
		jarVersions[new long?(895924)] = "xercesImpl.jar from xerces-2_4";
		jarVersions[new long?(1010806)] = "xercesImpl.jar from Xerces-J-bin.2.6.2";
		jarVersions[new long?(1203860)] = "xercesImpl.jar from Xerces-J-bin.2.7.1";

		jarVersions[new long?(37485)] = "xalanj1compat.jar from xalan-j_2_0_0";
		jarVersions[new long?(38100)] = "xalanj1compat.jar from xalan-j_2_0_1";

		jarVersions[new long?(18779)] = "xalanservlet.jar from xalan-j_2_0_0";
		jarVersions[new long?(21453)] = "xalanservlet.jar from xalan-j_2_0_1";
		jarVersions[new long?(24826)] = "xalanservlet.jar from xalan-j_2_3_1 or xalan-j_2_4_1";
		jarVersions[new long?(24831)] = "xalanservlet.jar from xalan-j_2_4_1";
		// Stop recording xalanservlet.jar sizes as of Xalan Java 2.5.0; now a .war file

		// For those who've downloaded JAXP from sun
		jarVersions[new long?(5618)] = "jaxp.jar from jaxp1.0.1";
		jarVersions[new long?(136133)] = "parser.jar from jaxp1.0.1";
		jarVersions[new long?(28404)] = "jaxp.jar from jaxp-1.1";
		jarVersions[new long?(187162)] = "crimson.jar from jaxp-1.1";
		jarVersions[new long?(801714)] = "xalan.jar from jaxp-1.1";
		jarVersions[new long?(196399)] = "crimson.jar from crimson-1.1.1";
		jarVersions[new long?(33323)] = "jaxp.jar from crimson-1.1.1 or jakarta-ant-1.4.1b1";
		jarVersions[new long?(152717)] = "crimson.jar from crimson-1.1.2beta2";
		jarVersions[new long?(88143)] = "xml-apis.jar from crimson-1.1.2beta2";
		jarVersions[new long?(206384)] = "crimson.jar from crimson-1.1.3 or jakarta-ant-1.4.1b1";

		// jakarta-ant: since many people use ant these days
		jarVersions[new long?(136198)] = "parser.jar from jakarta-ant-1.3 or 1.2";
		jarVersions[new long?(5537)] = "jaxp.jar from jakarta-ant-1.3 or 1.2";
	  }

	  /// <summary>
	  /// Simple PrintWriter we send output to; defaults to System.out. </summary>
	  protected internal PrintWriter outWriter = new PrintWriter(System.out, true);

	  /// <summary>
	  /// Bottleneck output: calls outWriter.println(s). </summary>
	  /// <param name="s"> String to print </param>
	  protected internal virtual void logMsg(string s)
	  {
		outWriter.println(s);
	  }
	}

}