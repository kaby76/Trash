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
 * $Id: JavaCupRedirect.java 468653 2006-10-28 07:07:05Z minchau $
 */

namespace org.apache.xalan.xsltc.util
{


	/// <summary>
	/// Utility class to redirect input to JavaCup program.
	/// 
	/// Usage-command line: 
	/// <code>java org.apache.xalan.xsltc.utils.JavaCupRedirect [args] -stdin filename.ext</code>
	/// 
	/// @author Morten Jorgensen
	/// @version $Id: JavaCupRedirect.java 468653 2006-10-28 07:07:05Z minchau $
	/// </summary>
	public class JavaCupRedirect
	{

		private const string ERRMSG = "You must supply a filename with the -stdin option.";

		public static void Main(string[] args)
		{

			 // If we should call System.exit or not
			 //@todo make this settable for use inside other java progs
			 bool systemExitOK = true;

			 // This is the stream we'll set as our System.in
			 System.IO.Stream input = null;

			 // The number of arguments
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int argc = args.length;
			 int argc = args.Length;

			 // The arguments we'll pass to the real 'main()'
			 string[] new_args = new string[argc - 2];
			 int new_argc = 0;

			 // Parse all parameters passed to this class
			 for (int i = 0; i < argc; i++)
			 {
				 // Parse option '-stdin <filename>'
				 if (args[i].Equals("-stdin"))
				 {
					  // This option must have an argument
					  if ((++i >= argc) || (args[i].StartsWith("-", StringComparison.Ordinal)))
					  {
						  Console.Error.WriteLine(ERRMSG);
						 throw new Exception(ERRMSG);
					  }
					  try
					  {
						  input = new System.IO.FileStream(args[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);
					  }
					  catch (FileNotFoundException e)
					  {
						  Console.Error.WriteLine("Could not open file " + args[i]);
						 throw new Exception(e.Message);
					  }
					  catch (SecurityException e)
					  {
						  Console.Error.WriteLine("No permission to file " + args[i]);
						 throw new Exception(e.Message);
					  }
				 }
				 else
				 {
					  if (new_argc == new_args.Length)
					  {
						  Console.Error.WriteLine("Missing -stdin option!");
						 throw new Exception();
					  }
					  new_args[new_argc++] = args[i];
				 }
			 }

			 System.In = input;
			 try
			 {
				 java_cup.Main.main(new_args);
			 }
			 catch (Exception e)
			 {
				 Console.Error.WriteLine("Error running JavaCUP:");
				 Console.WriteLine(e.ToString());
				 Console.Write(e.StackTrace);
			 }
		}
	}

}