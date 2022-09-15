using System.IO;
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

namespace org.apache.xalan.xsltc.dom
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
			public object run()
			{
				ClassLoader cl = null;
				try
				{
					cl = Thread.CurrentThread.getContextClassLoader();
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
			public object run()
			{
				ClassLoader cl = null;
				try
				{
					cl = ClassLoader.getSystemClassLoader();
				}
				catch (SecurityException)
				{
				}
				return cl;
			}
		}

		internal static ClassLoader getParentClassLoader(in ClassLoader cl)
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

			public object run()
			{
				ClassLoader parent = null;
				try
				{
					parent = cl.getParent();
				}
				catch (SecurityException)
				{
				}

				// eliminate loops in case of the boot
				// ClassLoader returning itself as a parent
				return (parent == cl) ? null : parent;
			}
		}

		internal static string getSystemProperty(in string propName)
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

			public object run()
			{
				return System.getProperty(propName);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: static java.io.FileInputStream getFileInputStream(final java.io.File file) throws java.io.FileNotFoundException
		internal static FileStream getFileInputStream(in File file)
		{
			try
			{
				return (FileStream) AccessController.doPrivileged(new PrivilegedExceptionActionAnonymousInnerClass(file));
			}
			catch (PrivilegedActionException e)
			{
				throw (FileNotFoundException)e.getException();
			}
		}

		private class PrivilegedExceptionActionAnonymousInnerClass : PrivilegedExceptionAction
		{
			private File file;

			public PrivilegedExceptionActionAnonymousInnerClass(File file)
			{
				this.file = file;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Object run() throws java.io.FileNotFoundException
			public object run()
			{
				return new FileStream(file, FileMode.Open, FileAccess.Read);
			}
		}

		internal static Stream getResourceAsStream(in ClassLoader cl, in string name)
		{
			return (Stream) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass5(cl, name));
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

			public object run()
			{
				Stream ris;
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

		internal static bool getFileExists(in File f)
		{
		return ((bool?) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass6(f))).Value;
		}

		private class PrivilegedActionAnonymousInnerClass6 : PrivilegedAction
		{
			private File f;

			public PrivilegedActionAnonymousInnerClass6(File f)
			{
				this.f = f;
			}

			public object run()
			{
				return f.exists() ? true : false;
			}
		}

		internal static long getLastModified(in File f)
		{
		return ((long?) AccessController.doPrivileged(new PrivilegedActionAnonymousInnerClass7(f))).Value;
		}

		private class PrivilegedActionAnonymousInnerClass7 : PrivilegedAction
		{
			private File f;

			public PrivilegedActionAnonymousInnerClass7(File f)
			{
				this.f = f;
			}

			public object run()
			{
				return new long?(f.lastModified());
			}
		}

		private SecuritySupport()
		{
		}
	}

}