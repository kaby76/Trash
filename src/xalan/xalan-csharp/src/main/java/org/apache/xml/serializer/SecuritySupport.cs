using System.Threading;

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
 * $Id: SecuritySupport.java 1225414 2011-12-29 02:38:30Z mrglavas $
 */

namespace org.apache.xml.serializer
{


	/// <summary>
	/// This class is duplicated for each Xalan-Java subpackage so keep it in sync.
	/// It is package private and therefore is not exposed as part of the Xalan-Java
	/// API.
	/// 
	/// Security related methods that only work on J2SE 1.2 and newer.
	/// </summary>
	internal sealed class SecuritySupport
	{

		internal static ClassLoader ContextClassLoader
		{
			get
			{
				return (ClassLoader) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass());
			}
		}

		private class PrivilegedActionAnonymousInnerClass : PrivilegedAction
		{
			public PrivilegedActionAnonymousInnerClass()
			{
			}

			public virtual object run()
			{
				ClassLoader cl = null;
				try
				{
					cl = Thread.CurrentThread.ContextClassLoader;
				}
				catch (SecurityException)
				{
				}
				return cl;
			}
		}

		internal static ClassLoader SystemClassLoader
		{
			get
			{
				return (ClassLoader) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass2());
			}
		}

		private class PrivilegedActionAnonymousInnerClass2 : PrivilegedAction
		{
			public PrivilegedActionAnonymousInnerClass2()
			{
			}

			public virtual object run()
			{
				ClassLoader cl = null;
				try
				{
					cl = ClassLoader.SystemClassLoader;
				}
				catch (SecurityException)
				{
				}
				return cl;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static ClassLoader getParentClassLoader(final ClassLoader cl)
		internal static ClassLoader getParentClassLoader(ClassLoader cl)
		{
			return (ClassLoader) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass3(cl));
		}

		private class PrivilegedActionAnonymousInnerClass3 : PrivilegedAction
		{
			private ClassLoader cl;

			public PrivilegedActionAnonymousInnerClass3(ClassLoader cl)
			{
				this.cl = cl;
			}

			public virtual object run()
			{
				ClassLoader parent = null;
				try
				{
					parent = cl.Parent;
				}
				catch (SecurityException)
				{
				}

				// eliminate loops in case of the boot
				// ClassLoader returning itself as a parent
				return (parent == cl) ? null : parent;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static String getSystemProperty(final String propName)
		internal static string getSystemProperty(string propName)
		{
			return (string) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass4(propName));
		}

		private class PrivilegedActionAnonymousInnerClass4 : PrivilegedAction
		{
			private string propName;

			public PrivilegedActionAnonymousInnerClass4(string propName)
			{
				this.propName = propName;
			}

			public virtual object run()
			{
				return System.getProperty(propName);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static java.io.FileInputStream getFileInputStream(final java.io.File file) throws java.io.FileNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
		internal static System.IO.FileStream getFileInputStream(File file)
		{
			try
			{
				return (System.IO.FileStream) AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClass(file));
			}
			catch (PrivilegedActionException e)
			{
				throw (FileNotFoundException)e.Exception;
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClass : PrivilegedExceptionAction
		{
			private File file;

			public PrivilegedExceptionActionAnonymousInnerClass(File file)
			{
				this.file = file;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object run() throws java.io.FileNotFoundException
			public virtual object run()
			{
				return new System.IO.FileStream(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static java.io.InputStream getResourceAsStream(final ClassLoader cl, final String name)
		internal static System.IO.Stream getResourceAsStream(ClassLoader cl, string name)
		{
			return (System.IO.Stream) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass5(cl, name));
		}

		private class PrivilegedActionAnonymousInnerClass5 : PrivilegedAction
		{
			private ClassLoader cl;
			private string name;

			public PrivilegedActionAnonymousInnerClass5(ClassLoader cl, string name)
			{
				this.cl = cl;
				this.name = name;
			}

			public virtual object run()
			{
				System.IO.Stream ris;
				if (cl == null)
				{
					ris = ClassLoader.getSystemResourceAsStream(name);
				}
				else
				{
					ris = cl.getResourceAsStream(name);
				}
				return ris;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static boolean getFileExists(final java.io.File f)
		internal static bool getFileExists(File f)
		{
		return ((bool?) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass6(f))).Value;
			   .booleanValue();
		}

		private class PrivilegedActionAnonymousInnerClass6 : PrivilegedAction
		{
			private File f;

			public PrivilegedActionAnonymousInnerClass6(File f)
			{
				this.f = f;
			}

			public virtual object run()
			{
				return f.exists() ? true : false;
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: static long getLastModified(final java.io.File f)
		internal static long getLastModified(File f)
		{
		return ((long?) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass7(f))).Value;
			   .longValue();
		}

		private class PrivilegedActionAnonymousInnerClass7 : PrivilegedAction
		{
			private File f;

			public PrivilegedActionAnonymousInnerClass7(File f)
			{
				this.f = f;
			}

			public virtual object run()
			{
				return new long?(f.lastModified());
			}
		}

		private SecuritySupport()
		{
		}
	}

}