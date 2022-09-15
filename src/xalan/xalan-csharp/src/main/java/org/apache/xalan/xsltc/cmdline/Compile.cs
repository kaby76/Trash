using System;
using System.Collections;
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
 * $Id: Compile.java 1347633 2012-06-07 14:20:59Z ggregory $
 */

namespace org.apache.xalan.xsltc.cmdline
{

	using GetOpt = org.apache.xalan.xsltc.cmdline.getopt.GetOpt;
	using GetOptsException = org.apache.xalan.xsltc.cmdline.getopt.GetOptsException;
	using XSLTC = org.apache.xalan.xsltc.compiler.XSLTC;
	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class Compile
	{

		// Versioning numbers  for the compiler -v option output
		private static int VERSION_MAJOR = 1;
		private static int VERSION_MINOR = 4;
		private static int VERSION_DELTA = 0;


		public static void printUsage()
		{
			StringBuilder vers = new StringBuilder("XSLTC version " + VERSION_MAJOR + "." + VERSION_MINOR + ((VERSION_DELTA > 0) ? ("." + VERSION_DELTA) : ("")));
			Console.Error.WriteLine(vers + "\n" + new ErrorMsg(ErrorMsg.COMPILE_USAGE_STR));
		}

		/// <summary>
		/// This method implements the command line compiler. See the USAGE_STRING
		/// constant for a description. It may make sense to move the command-line
		/// handling to a separate package (ie. make one xsltc.cmdline.Compiler
		/// class that contains this main() method and one xsltc.cmdline.Transform
		/// class that contains the DefaultRun stuff).
		/// </summary>
		public static void Main(string[] args)
		{
		try
		{
			bool inputIsURL = false;
			bool useStdIn = false;
			bool classNameSet = false;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.cmdline.getopt.GetOpt getopt = new org.apache.xalan.xsltc.cmdline.getopt.GetOpt(args, "o:d:j:p:uxhsinv");
			GetOpt getopt = new GetOpt(args, "o:d:j:p:uxhsinv");
			if (args.Length < 1)
			{
				printUsage();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.compiler.XSLTC xsltc = new org.apache.xalan.xsltc.compiler.XSLTC();
			XSLTC xsltc = new XSLTC();
			xsltc.init();

			int c;
			while ((c = getopt.NextOption) != -1)
			{
			switch (c)
			{
			case 'i':
				useStdIn = true;
				break;
			case 'o':
				xsltc.ClassName = getopt.OptionArg;
				classNameSet = true;
				break;
			case 'd':
				xsltc.DestDirectory = getopt.OptionArg;
				break;
			case 'p':
				xsltc.PackageName = getopt.OptionArg;
				break;
			case 'j':
				xsltc.JarFileName = getopt.OptionArg;
				break;
			case 'x':
				xsltc.Debug = true;
				break;
			case 'u':
				inputIsURL = true;
				break;
			case 'n':
				xsltc.TemplateInlining = true; // used to be 'false'
				break;
			case 'v':
				// fall through to case h
			case 'h':
			default:
				printUsage();
				break;
			}
			}

			bool compileOK;

			if (useStdIn)
			{
			if (!classNameSet)
			{
				Console.Error.WriteLine(new ErrorMsg(ErrorMsg.COMPILE_STDIN_ERR));
			}
			compileOK = xsltc.compile(System.in, xsltc.ClassName);
			}
			else
			{
			// Generate a vector containg URLs for all stylesheets specified
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String[] stylesheetNames = getopt.getCmdArgs();
			string[] stylesheetNames = getopt.CmdArgs;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Vector stylesheetVector = new java.util.Vector();
			ArrayList stylesheetVector = new ArrayList();
			for (int i = 0; i < stylesheetNames.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = stylesheetNames[i];
				string name = stylesheetNames[i];
				URL url;
				if (inputIsURL)
				{
				url = new URL(name);
				}
				else
				{
				url = (new File(name)).toURL();
				}
				stylesheetVector.Add(url);
			}
			compileOK = xsltc.compile(stylesheetVector);
			}

			// Compile the stylesheet and output class/jar file(s)
			if (compileOK)
			{
			xsltc.printWarnings();
			if (!string.ReferenceEquals(xsltc.JarFileName, null))
			{
				xsltc.outputToJar();
			}
			}
			else
			{
			xsltc.printWarnings();
			xsltc.printErrors();
			}
		}
		catch (GetOptsException ex)
		{
			Console.Error.WriteLine(ex);
			printUsage();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}
		}

	}

}