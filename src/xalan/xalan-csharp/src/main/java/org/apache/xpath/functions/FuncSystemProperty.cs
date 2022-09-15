using System;
using System.IO;

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
 * $Id: FuncSystemProperty.java 1581426 2014-03-25 17:47:08Z ggregory $
 */
namespace org.apache.xpath.functions
{

	using XPathContext = org.apache.xpath.XPathContext;
	using XObject = org.apache.xpath.objects.XObject;
	using XString = org.apache.xpath.objects.XString;
	using XPATHErrorResources = org.apache.xpath.res.XPATHErrorResources;

	/// <summary>
	/// Execute the SystemProperty() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncSystemProperty : FunctionOneArg
	{
		internal new const long serialVersionUID = 3694874980992204867L;
	  /// <summary>
	  /// The path/filename of the property file: XSLTInfo.properties
	  /// Maintenance note: see also
	  /// org.apache.xalan.processor.TransformerFactoryImpl.XSLT_PROPERTIES
	  /// </summary>
	  internal const string XSLT_PROPERTIES = "org/apache/xalan/res/XSLTInfo.properties";

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {

		string fullName = m_arg0.execute(xctxt).str();
		int indexOfNSSep = fullName.IndexOf(':');
		string result = null;
		string propName = "";

		// List of properties where the name of the
		// property argument is to be looked for.
		Properties xsltInfo = new Properties();

		loadPropertyFile(XSLT_PROPERTIES, xsltInfo);

		if (indexOfNSSep > 0)
		{
		  string prefix = (indexOfNSSep >= 0) ? fullName.Substring(0, indexOfNSSep) : "";
		  string @namespace;

		  @namespace = xctxt.NamespaceContext.getNamespaceForPrefix(prefix);
		  propName = (indexOfNSSep < 0) ? fullName : fullName.Substring(indexOfNSSep + 1);

		  if (@namespace.StartsWith("http://www.w3.org/XSL/Transform", StringComparison.Ordinal) || @namespace.Equals("http://www.w3.org/1999/XSL/Transform"))
		  {
			result = xsltInfo.getProperty(propName);

			if (null == result)
			{
			  warn(xctxt, XPATHErrorResources.WG_PROPERTY_NOT_SUPPORTED, new object[]{fullName}); //"XSL Property not supported: "+fullName);

			  return XString.EMPTYSTRING;
			}
		  }
		  else
		  {
			warn(xctxt, XPATHErrorResources.WG_DONT_DO_ANYTHING_WITH_NS, new object[]{@namespace, fullName}); //"Don't currently do anything with namespace "+namespace+" in property: "+fullName);

			try
			{
				//if secure procession is enabled only handle required properties do not not map any valid system property
				if (!xctxt.SecureProcessing)
				{
					result = System.getProperty(propName);
				}
				else
				{
					warn(xctxt, XPATHErrorResources.WG_SECURITY_EXCEPTION, new object[]{fullName}); //"SecurityException when trying to access XSL system property: "+fullName);
				}
				if (null == result)
				{
					return XString.EMPTYSTRING;
				}
			}
			catch (SecurityException)
			{
			  warn(xctxt, XPATHErrorResources.WG_SECURITY_EXCEPTION, new object[]{fullName}); //"SecurityException when trying to access XSL system property: "+fullName);

			  return XString.EMPTYSTRING;
			}
		  }
		}
		else
		{
		  try
		  {
			  //if secure procession is enabled only handle required properties do not not map any valid system property
			  if (!xctxt.SecureProcessing)
			  {
				  result = System.getProperty(fullName);
			  }
			  else
			  {
				  warn(xctxt, XPATHErrorResources.WG_SECURITY_EXCEPTION, new object[]{fullName}); //"SecurityException when trying to access XSL system property: "+fullName);
			  }
			  if (null == result)
			  {
				  return XString.EMPTYSTRING;
			  }
		  }
		  catch (SecurityException)
		  {
			warn(xctxt, XPATHErrorResources.WG_SECURITY_EXCEPTION, new object[]{fullName}); //"SecurityException when trying to access XSL system property: "+fullName);

			return XString.EMPTYSTRING;
		  }
		}

		if (propName.Equals("version") && result.Length > 0)
		{
		  try
		  {
			// Needs to return the version number of the spec we conform to.
			return new XString("1.0");
		  }
		  catch (Exception)
		  {
			return new XString(result);
		  }
		}
		else
		{
		  return new XString(result);
		}
	  }

	  /// <summary>
	  /// Retrieve a propery bundle from a specified file
	  /// </summary>
	  /// <param name="file"> The string name of the property file.  The name 
	  /// should already be fully qualified as path/filename </param>
	  /// <param name="target"> The target property bag the file will be placed into. </param>
	  public virtual void loadPropertyFile(string file, Properties target)
	  {
		try
		{
		  // Use SecuritySupport class to provide privileged access to property file
		  Stream @is = SecuritySupport.getResourceAsStream(ObjectFactory.findClassLoader(), file);

		  // get a buffered version
		  BufferedInputStream bis = new BufferedInputStream(@is);

		  target.load(bis); // and load up the property bag from this
		  bis.close(); // close out after reading
		}
		catch (Exception ex)
		{
		  // ex.printStackTrace();
		  throw new org.apache.xml.utils.WrappedRuntimeException(ex);
		}
	  }
	}

}