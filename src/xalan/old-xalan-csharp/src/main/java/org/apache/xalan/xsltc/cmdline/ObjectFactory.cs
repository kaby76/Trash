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
 * $Id: ObjectFactory.java 1225583 2011-12-29 16:08:00Z mrglavas $
 */

namespace org.apache.xalan.xsltc.cmdline
{



	/// <summary>
	/// This class is duplicated for each JAXP subpackage so keep it in sync.
	/// It is package private and therefore is not exposed as part of the JAXP
	/// API.
	/// <para>
	/// This code is designed to implement the JAXP 1.1 spec pluggability
	/// feature and is designed to run on JDK version 1.1 and
	/// later, and to compile on JDK 1.2 and onward.  
	/// The code also runs both as part of an unbundled jar file and
	/// when bundled as part of the JDK.
	/// </para>
	/// <para>
	/// This class was moved from the <code>javax.xml.parsers.ObjectFactory</code>
	/// class and modified to be used as a general utility for creating objects 
	/// dynamically.
	/// 
	/// @version $Id: ObjectFactory.java 1225583 2011-12-29 16:08:00Z mrglavas $
	/// </para>
	/// </summary>
	internal sealed class ObjectFactory
	{

		//
		// Constants
		//

		// name of default properties file to look for in JDK's jre/lib directory
		private const string DEFAULT_PROPERTIES_FILENAME = "xalan.properties";

		private const string SERVICES_PATH = "META-INF/services/";

		/// <summary>
		/// Set to true for debugging </summary>
		private const bool DEBUG = false;

		/// <summary>
		/// cache the contents of the xalan.properties file.
		///  Until an attempt has been made to read this file, this will
		/// be null; if the file does not exist or we encounter some other error
		/// during the read, this will be empty.
		/// </summary>
		private static Properties fXalanProperties = null;

		/// <summary>
		///*
		/// Cache the time stamp of the xalan.properties file so
		/// that we know if it's been modified and can invalidate
		/// the cache when necessary.
		/// </summary>
		private static long fLastModified = -1;

		//
		// Public static methods
		//

		/// <summary>
		/// Finds the implementation Class object in the specified order.  The
		/// specified order is the following:
		/// <ol>
		///  <li>query the system property using <code>System.getProperty</code>
		///  <li>read <code>META-INF/services/<i>factoryId</i></code> file
		///  <li>use fallback classname
		/// </ol>
		/// </summary>
		/// <returns> instance of factory, never null
		/// </returns>
		/// <param name="factoryId">             Name of the factory to find, same as
		///                              a property name </param>
		/// <param name="fallbackClassName">     Implementation class name, if nothing else
		///                              is found.  Use null to mean no fallback.
		/// </param>
		/// <exception cref="ObjectFactory.ConfigurationError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object createObject(String factoryId, String fallbackClassName) throws ConfigurationError
		internal static object createObject(string factoryId, string fallbackClassName)
		{
			return createObject(factoryId, null, fallbackClassName);
		} // createObject(String,String):Object

		/// <summary>
		/// Finds the implementation Class object in the specified order.  The
		/// specified order is the following:
		/// <ol>
		///  <li>query the system property using <code>System.getProperty</code>
		///  <li>read <code>$java.home/lib/<i>propertiesFilename</i></code> file
		///  <li>read <code>META-INF/services/<i>factoryId</i></code> file
		///  <li>use fallback classname
		/// </ol>
		/// </summary>
		/// <returns> instance of factory, never null
		/// </returns>
		/// <param name="factoryId">             Name of the factory to find, same as
		///                              a property name </param>
		/// <param name="propertiesFilename"> The filename in the $java.home/lib directory
		///                           of the properties file.  If none specified,
		///                           ${java.home}/lib/xalan.properties will be used. </param>
		/// <param name="fallbackClassName">     Implementation class name, if nothing else
		///                              is found.  Use null to mean no fallback.
		/// </param>
		/// <exception cref="ObjectFactory.ConfigurationError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object createObject(String factoryId, String propertiesFilename, String fallbackClassName) throws ConfigurationError
		internal static object createObject(string factoryId, string propertiesFilename, string fallbackClassName)
		{
			Type factoryClass = lookUpFactoryClass(factoryId, propertiesFilename, fallbackClassName);

			if (factoryClass == null)
			{
				throw new ConfigurationError("Provider for " + factoryId + " cannot be found", null);
			}

			try
			{
				object instance = factoryClass.newInstance();
				debugPrintln("created new instance of factory " + factoryId);
				return instance;
			}
			catch (Exception x)
			{
				throw new ConfigurationError("Provider for factory " + factoryId + " could not be instantiated: " + x, x);
			}
		} // createObject(String,String,String):Object

		/// <summary>
		/// Finds the implementation Class object in the specified order.  The
		/// specified order is the following:
		/// <ol>
		///  <li>query the system property using <code>System.getProperty</code>
		///  <li>read <code>$java.home/lib/<i>propertiesFilename</i></code> file
		///  <li>read <code>META-INF/services/<i>factoryId</i></code> file
		///  <li>use fallback classname
		/// </ol>
		/// </summary>
		/// <returns> Class object of factory, never null
		/// </returns>
		/// <param name="factoryId">             Name of the factory to find, same as
		///                              a property name </param>
		/// <param name="propertiesFilename"> The filename in the $java.home/lib directory
		///                           of the properties file.  If none specified,
		///                           ${java.home}/lib/xalan.properties will be used. </param>
		/// <param name="fallbackClassName">     Implementation class name, if nothing else
		///                              is found.  Use null to mean no fallback.
		/// </param>
		/// <exception cref="ObjectFactory.ConfigurationError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Class lookUpFactoryClass(String factoryId) throws ConfigurationError
		internal static Type lookUpFactoryClass(string factoryId)
		{
			return lookUpFactoryClass(factoryId, null, null);
		} // lookUpFactoryClass(String):Class

		/// <summary>
		/// Finds the implementation Class object in the specified order.  The
		/// specified order is the following:
		/// <ol>
		///  <li>query the system property using <code>System.getProperty</code>
		///  <li>read <code>$java.home/lib/<i>propertiesFilename</i></code> file
		///  <li>read <code>META-INF/services/<i>factoryId</i></code> file
		///  <li>use fallback classname
		/// </ol>
		/// </summary>
		/// <returns> Class object that provides factory service, never null
		/// </returns>
		/// <param name="factoryId">             Name of the factory to find, same as
		///                              a property name </param>
		/// <param name="propertiesFilename"> The filename in the $java.home/lib directory
		///                           of the properties file.  If none specified,
		///                           ${java.home}/lib/xalan.properties will be used. </param>
		/// <param name="fallbackClassName">     Implementation class name, if nothing else
		///                              is found.  Use null to mean no fallback.
		/// </param>
		/// <exception cref="ObjectFactory.ConfigurationError"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Class lookUpFactoryClass(String factoryId, String propertiesFilename, String fallbackClassName) throws ConfigurationError
		internal static Type lookUpFactoryClass(string factoryId, string propertiesFilename, string fallbackClassName)
		{
			string factoryClassName = lookUpFactoryClassName(factoryId, propertiesFilename, fallbackClassName);
			ClassLoader cl = findClassLoader();

			if (string.ReferenceEquals(factoryClassName, null))
			{
				factoryClassName = fallbackClassName;
			}

			// assert(className != null);
			try
			{
				Type providerClass = findProviderClass(factoryClassName, cl, true);
				debugPrintln("created new instance of " + providerClass + " using ClassLoader: " + cl);
				return providerClass;
			}
			catch (ClassNotFoundException x)
			{
				throw new ConfigurationError("Provider " + factoryClassName + " not found", x);
			}
			catch (Exception x)
			{
				throw new ConfigurationError("Provider " + factoryClassName + " could not be instantiated: " + x, x);
			}
		} // lookUpFactoryClass(String,String,String):Class

		/// <summary>
		/// Finds the name of the required implementation class in the specified
		/// order.  The specified order is the following:
		/// <ol>
		///  <li>query the system property using <code>System.getProperty</code>
		///  <li>read <code>$java.home/lib/<i>propertiesFilename</i></code> file
		///  <li>read <code>META-INF/services/<i>factoryId</i></code> file
		///  <li>use fallback classname
		/// </ol>
		/// </summary>
		/// <returns> name of class that provides factory service, never null
		/// </returns>
		/// <param name="factoryId">             Name of the factory to find, same as
		///                              a property name </param>
		/// <param name="propertiesFilename"> The filename in the $java.home/lib directory
		///                           of the properties file.  If none specified,
		///                           ${java.home}/lib/xalan.properties will be used. </param>
		/// <param name="fallbackClassName">     Implementation class name, if nothing else
		///                              is found.  Use null to mean no fallback.
		/// </param>
		/// <exception cref="ObjectFactory.ConfigurationError"> </exception>
		internal static string lookUpFactoryClassName(string factoryId, string propertiesFilename, string fallbackClassName)
		{
			// Use the system property first
			try
			{
				string systemProp = SecuritySupport.getSystemProperty(factoryId);
				if (!string.ReferenceEquals(systemProp, null))
				{
					debugPrintln("found system property, value=" + systemProp);
					return systemProp;
				}
			}
			catch (SecurityException)
			{
				// Ignore and continue w/ next location
			}

			// Try to read from propertiesFilename, or
			// $java.home/lib/xalan.properties
			string factoryClassName = null;
			// no properties file name specified; use
			// $JAVA_HOME/lib/xalan.properties:
			if (string.ReferenceEquals(propertiesFilename, null))
			{
				File propertiesFile = null;
				bool propertiesFileExists = false;
				try
				{
					string javah = SecuritySupport.getSystemProperty("java.home");
					propertiesFilename = javah + File.separator + "lib" + File.separator + DEFAULT_PROPERTIES_FILENAME;
					propertiesFile = new File(propertiesFilename);
					propertiesFileExists = SecuritySupport.getFileExists(propertiesFile);
				}
				catch (SecurityException)
				{
					// try again...
					fLastModified = -1;
					fXalanProperties = null;
				}

				lock (typeof(ObjectFactory))
				{
					bool loadProperties = false;
					System.IO.FileStream fis = null;
					try
					{
						// file existed last time
						if (fLastModified >= 0)
						{
							if (propertiesFileExists && (fLastModified < (fLastModified = SecuritySupport.getLastModified(propertiesFile))))
							{
								loadProperties = true;
							}
							else
							{
								// file has stopped existing...
								if (!propertiesFileExists)
								{
									fLastModified = -1;
									fXalanProperties = null;
								} // else, file wasn't modified!
							}
						}
						else
						{
							// file has started to exist:
							if (propertiesFileExists)
							{
								loadProperties = true;
								fLastModified = SecuritySupport.getLastModified(propertiesFile);
							} // else, nothing's changed
						}
						if (loadProperties)
						{
							// must never have attempted to read xalan.properties
							// before (or it's outdeated)
							fXalanProperties = new Properties();
							fis = SecuritySupport.getFileInputStream(propertiesFile);
							fXalanProperties.load(fis);
						}
					}
				catch (Exception)
				{
					fXalanProperties = null;
					fLastModified = -1;
						// assert(x instanceof FileNotFoundException
					//        || x instanceof SecurityException)
					// In both cases, ignore and continue w/ next location
				}
					finally
					{
						// try to close the input stream if one was opened.
						if (fis != null)
						{
							try
							{
								fis.Close();
							}
							// Ignore the exception.
							catch (IOException)
							{
							}
						}
					}
				}
				if (fXalanProperties != null)
				{
					factoryClassName = fXalanProperties.getProperty(factoryId);
				}
			}
			else
			{
				System.IO.FileStream fis = null;
				try
				{
					fis = SecuritySupport.getFileInputStream(new File(propertiesFilename));
					Properties props = new Properties();
					props.load(fis);
					factoryClassName = props.getProperty(factoryId);
				}
				catch (Exception)
				{
					// assert(x instanceof FileNotFoundException
					//        || x instanceof SecurityException)
					// In both cases, ignore and continue w/ next location
				}
				finally
				{
					// try to close the input stream if one was opened.
					if (fis != null)
					{
						try
						{
							fis.Close();
						}
						// Ignore the exception.
						catch (IOException)
						{
						}
					}
				}
			}
			if (!string.ReferenceEquals(factoryClassName, null))
			{
				debugPrintln("found in " + propertiesFilename + ", value=" + factoryClassName);
				return factoryClassName;
			}

			// Try Jar Service Provider Mechanism
			return findJarServiceProviderName(factoryId);
		} // lookUpFactoryClass(String,String):String

		//
		// Private static methods
		//

		/// <summary>
		/// Prints a message to standard error if debugging is enabled. </summary>
		private static void debugPrintln(string msg)
		{
			if (DEBUG)
			{
				Console.Error.WriteLine("JAXP: " + msg);
			}
		} // debugPrintln(String)

		/// <summary>
		/// Figure out which ClassLoader to use.  For JDK 1.2 and later use
		/// the context ClassLoader.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static ClassLoader findClassLoader() throws ConfigurationError
		internal static ClassLoader findClassLoader()
		{
			// Figure out which ClassLoader to use for loading the provider
			// class.  If there is a Context ClassLoader then use it.
			ClassLoader context = SecuritySupport.ContextClassLoader;
			ClassLoader system = SecuritySupport.SystemClassLoader;

			ClassLoader chain = system;
			while (true)
			{
				if (context == chain)
				{
					// Assert: we are on JDK 1.1 or we have no Context ClassLoader
					// or any Context ClassLoader in chain of system classloader
					// (including extension ClassLoader) so extend to widest
					// ClassLoader (always look in system ClassLoader if Xalan
					// is in boot/extension/system classpath and in current
					// ClassLoader otherwise); normal classloaders delegate
					// back to system ClassLoader first so this widening doesn't
					// change the fact that context ClassLoader will be consulted
					ClassLoader current = typeof(ObjectFactory).ClassLoader;

					chain = system;
					while (true)
					{
						if (current == chain)
						{
							// Assert: Current ClassLoader in chain of
							// boot/extension/system ClassLoaders
							return system;
						}
						if (chain == null)
						{
							break;
						}
						chain = SecuritySupport.getParentClassLoader(chain);
					}

					// Assert: Current ClassLoader not in chain of
					// boot/extension/system ClassLoaders
					return current;
				}

				if (chain == null)
				{
					// boot ClassLoader reached
					break;
				}

				// Check for any extension ClassLoaders in chain up to
				// boot ClassLoader
				chain = SecuritySupport.getParentClassLoader(chain);
			};

			// Assert: Context ClassLoader not in chain of
			// boot/extension/system ClassLoaders
			return context;
		} // findClassLoader():ClassLoader

		/// <summary>
		/// Create an instance of a class using the specified ClassLoader
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Object newInstance(String className, ClassLoader cl, boolean doFallback) throws ConfigurationError
		internal static object newInstance(string className, ClassLoader cl, bool doFallback)
		{
			// assert(className != null);
			try
			{
				Type providerClass = findProviderClass(className, cl, doFallback);
				object instance = providerClass.newInstance();
				debugPrintln("created new instance of " + providerClass + " using ClassLoader: " + cl);
				return instance;
			}
			catch (ClassNotFoundException x)
			{
				throw new ConfigurationError("Provider " + className + " not found", x);
			}
			catch (Exception x)
			{
				throw new ConfigurationError("Provider " + className + " could not be instantiated: " + x, x);
			}
		}

		/// <summary>
		/// Find a Class using the specified ClassLoader
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static Class findProviderClass(String className, ClassLoader cl, boolean doFallback) throws ClassNotFoundException, ConfigurationError
		internal static Type findProviderClass(string className, ClassLoader cl, bool doFallback)
		{
			//throw security exception if the calling thread is not allowed to access the
			//class. Restrict the access to the package classes as specified in java.security policy.
			SecurityManager security = System.SecurityManager;
			try
			{
					if (security != null)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastDot = className.lastIndexOf('.');
						int lastDot = className.LastIndexOf('.');
						string packageName = className;
						if (lastDot != -1)
						{
						packageName = className.Substring(0, lastDot);
						}
						security.checkPackageAccess(packageName);
					}
			}
			catch (SecurityException e)
			{
				throw e;
			}

			Type providerClass;
			if (cl == null)
			{
				// XXX Use the bootstrap ClassLoader.  There is no way to
				// load a class using the bootstrap ClassLoader that works
				// in both JDK 1.1 and Java 2.  However, this should still
				// work b/c the following should be true:
				//
				// (cl == null) iff current ClassLoader == null
				//
				// Thus Class.forName(String) will use the current
				// ClassLoader which will be the bootstrap ClassLoader.
				providerClass = Type.GetType(className);
			}
			else
			{
				try
				{
					providerClass = cl.loadClass(className);
				}
				catch (ClassNotFoundException x)
				{
					if (doFallback)
					{
						// Fall back to current classloader
						ClassLoader current = typeof(ObjectFactory).ClassLoader;
						if (current == null)
						{
							providerClass = Type.GetType(className);
						}
						else if (cl != current)
						{
							cl = current;
							providerClass = cl.loadClass(className);
						}
						else
						{
							throw x;
						}
					}
					else
					{
						throw x;
					}
				}
			}

			return providerClass;
		}

		/// <summary>
		/// Find the name of service provider using Jar Service Provider Mechanism
		/// </summary>
		/// <returns> instance of provider class if found or null </returns>
		private static string findJarServiceProviderName(string factoryId)
		{
			string serviceId = SERVICES_PATH + factoryId;
			System.IO.Stream @is = null;

			// First try the Context ClassLoader
			ClassLoader cl = findClassLoader();

			@is = SecuritySupport.getResourceAsStream(cl, serviceId);

			// If no provider found then try the current ClassLoader
			if (@is == null)
			{
				ClassLoader current = typeof(ObjectFactory).ClassLoader;
				if (cl != current)
				{
					cl = current;
					@is = SecuritySupport.getResourceAsStream(cl, serviceId);
				}
			}

			if (@is == null)
			{
				// No provider found
				return null;
			}

			debugPrintln("found jar resource=" + serviceId + " using ClassLoader: " + cl);

			// Read the service provider name in UTF-8 as specified in
			// the jar spec.  Unfortunately this fails in Microsoft
			// VJ++, which does not implement the UTF-8
			// encoding. Theoretically, we should simply let it fail in
			// that case, since the JVM is obviously broken if it
			// doesn't support such a basic standard.  But since there
			// are still some users attempting to use VJ++ for
			// development, we have dropped in a fallback which makes a
			// second attempt using the platform's default encoding. In
			// VJ++ this is apparently ASCII, which is a subset of
			// UTF-8... and since the strings we'll be reading here are
			// also primarily limited to the 7-bit ASCII range (at
			// least, in English versions), this should work well
			// enough to keep us on the air until we're ready to
			// officially decommit from VJ++. [Edited comment from
			// jkesselm]
			System.IO.StreamReader rd;
			try
			{
				rd = new System.IO.StreamReader(@is, Encoding.UTF8);
			}
			catch (java.io.UnsupportedEncodingException)
			{
				rd = new System.IO.StreamReader(@is);
			}

			string factoryClassName = null;
			try
			{
				// XXX Does not handle all possible input as specified by the
				// Jar Service Provider specification
				factoryClassName = rd.ReadLine();
			}
			catch (IOException)
			{
				// No provider found
				return null;
			}
			finally
			{
				try
				{
					// try to close the reader.
					rd.Close();
				}
				// Ignore the exception.
				catch (IOException)
				{
				}
			}

			if (!string.ReferenceEquals(factoryClassName, null) && !"".Equals(factoryClassName))
			{
				debugPrintln("found in resource, value=" + factoryClassName);

				// Note: here we do not want to fall back to the current
				// ClassLoader because we want to avoid the case where the
				// resource file was found using one ClassLoader and the
				// provider class was instantiated using a different one.
				return factoryClassName;
			}

			// No provider found
			return null;
		}

		//
		// Classes
		//

		/// <summary>
		/// A configuration error.
		/// </summary>
		internal class ConfigurationError : Exception
		{
					internal const long serialVersionUID = -6072257854297546607L;
			//
			// Data
			//

			/// <summary>
			/// Exception. </summary>
			internal Exception exception;

			//
			// Constructors
			//

			/// <summary>
			/// Construct a new instance with the specified detail string and
			/// exception.
			/// </summary>
			internal ConfigurationError(string msg, Exception x) : base(msg)
			{
				this.exception = x;
			} // <init>(String,Exception)

			//
			// Public methods
			//

			/// <summary>
			/// Returns the exception associated to this error. </summary>
			internal virtual Exception Exception
			{
				get
				{
					return exception;
				}
			} // getException():Exception

		} // class ConfigurationError

	} // class ObjectFactory

}