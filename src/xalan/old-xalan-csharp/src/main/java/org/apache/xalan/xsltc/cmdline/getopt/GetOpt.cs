using System;
using System.Collections;

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
 * $Id: GetOpt.java 1225436 2011-12-29 05:09:31Z mrglavas $
 */

namespace org.apache.xalan.xsltc.cmdline.getopt
{


	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;


	/// <summary>
	/// GetOpt is a Java equivalent to the C getopt() library function
	/// discussed in man page getopt(3C). It provides command line
	/// parsing for Java applications. It supports the most rules of the
	/// command line standard (see man page intro(1)) including stacked
	/// options such as '-sxm' (which is equivalent to -s -x -m); it
	/// handles special '--' option that signifies the end of options.
	/// Additionally this implementation of getopt will check for
	/// mandatory arguments to options such as in the case of
	/// '-d <file>' it will throw a MissingOptArgException if the 
	/// option argument '<file>' is not included on the commandline.
	/// getopt(3C) does not check for this. 
	/// @author G Todd Miller 
	/// </summary>
	public class GetOpt
	{
		public GetOpt(string[] args, string optString)
		{
		theOptions = new ArrayList();
		int currOptIndex = 0;
		theCmdArgs = new ArrayList();
		theOptionMatcher = new OptionMatcher(optString);
		// fill in the options list
		for (int i = 0; i < args.Length; i++)
		{
			string token = args[i];
			int tokenLength = token.Length;
			if (token.Equals("--"))
			{ // end of opts
				currOptIndex = i + 1; // set index of first operand
					break; // end of options
			}
			else if (token.StartsWith("-", StringComparison.Ordinal) && tokenLength == 2)
			{
			// simple option token such as '-s' found
			theOptions.Add(new Option(token[1]));
			}
			else if (token.StartsWith("-", StringComparison.Ordinal) && tokenLength > 2)
			{
			// stacked options found, such as '-shm'
			// iterate thru the tokens after the dash and
			// add them to theOptions list
			for (int j = 1; j < tokenLength; j++)
			{
				theOptions.Add(new Option(token[j]));
			}
			}
			else if (!token.StartsWith("-", StringComparison.Ordinal))
			{
			// case 1- there are not options stored yet therefore
			// this must be an command argument, not an option argument
			if (theOptions.Count == 0)
			{
				currOptIndex = i;
				break; // stop processing options
			}
			else
			{
				// case 2- 
				// there are options stored, check to see if
				// this arg belong to the last arg stored	
				int indexoflast = 0;
				indexoflast = theOptions.Count - 1;
				Option op = (Option)theOptions[indexoflast];
				char opLetter = op.ArgLetter;
				if (!op.hasArg() && theOptionMatcher.hasArg(opLetter))
				{
					op.Arg = token;
				}
				else
				{
					// case 3 - 
					// the last option stored does not take
					// an argument, so again, this argument
					// must be a command argument, not 
					// an option argument
					currOptIndex = i;
					break; // end of options
				}
			}
			} // end option does not start with "-"
		} // end for args loop

			//  attach an iterator to list of options 
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
		theOptionsIterator = theOptions.GetEnumerator();

		// options are done, now fill out cmd arg list with remaining args
		for (int i = currOptIndex; i < args.Length; i++)
		{
			string token = args[i];
			theCmdArgs.Add(token);
		}
		}


		/// <summary>
		/// debugging routine to print out all options collected
		/// </summary>
		public virtual void printOptions()
		{
//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
		for (IEnumerator it = theOptions.GetEnumerator(); it.MoveNext();)
		{
			Option opt = (Option)it.Current;
			Console.Write("OPT =" + opt.ArgLetter);
			string arg = opt.Argument;
			if (!string.ReferenceEquals(arg, null))
			{
			   Console.Write(" " + arg);
			}
			Console.WriteLine();
		}
		}

		/// <summary>
		/// gets the next option found in the commandline. Distinguishes
		/// between two bad cases, one case is when an illegal option
		/// is found, and then other case is when an option takes an
		/// argument but no argument was found for that option.
		/// If the option found was not declared in the optString, then 
		/// an IllegalArgumentException will be thrown (case 1). 
		/// If the next option found has been declared to take an argument, 
		/// and no such argument exists, then a MissingOptArgException
		/// is thrown (case 2). </summary>
		/// <returns> int - the next option found. </returns>
		/// <exception cref="IllegalArgumentException">, MissingOptArgException.  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getNextOption() throws IllegalArgumentException, MissingOptArgException
		public virtual int NextOption
		{
			get
			{
			int retval = -1;
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (theOptionsIterator.hasNext())
			{
	//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				theCurrentOption = (Option)theOptionsIterator.next();
				char c = theCurrentOption.ArgLetter;
				bool shouldHaveArg = theOptionMatcher.hasArg(c);
				string arg = theCurrentOption.Argument;
				if (!theOptionMatcher.match(c))
				{
						ErrorMsg msg = new ErrorMsg(ErrorMsg.ILLEGAL_CMDLINE_OPTION_ERR, new char?(c));
				throw (new IllegalArgumentException(msg.ToString()));
				}
				else if (shouldHaveArg && (string.ReferenceEquals(arg, null)))
				{
						ErrorMsg msg = new ErrorMsg(ErrorMsg.CMDLINE_OPT_MISSING_ARG_ERR, new char?(c));
				throw (new MissingOptArgException(msg.ToString()));
				}
				retval = c;
			}
			return retval;
			}
		}

		/// <summary>
		/// gets the argument for the current parsed option. For example,
		/// in case of '-d <file>', if current option parsed is 'd' then
		/// getOptionArg() would return '<file>'. </summary>
		/// <returns> String - argument for current parsed option. </returns>
		public virtual string OptionArg
		{
			get
			{
			string retval = null;
			string tmp = theCurrentOption.Argument;
			char c = theCurrentOption.ArgLetter;
			if (theOptionMatcher.hasArg(c))
			{
				retval = tmp;
			}
			return retval;
			}
		}

		/// <summary>
		/// gets list of the commandline arguments. For example, in command
		/// such as 'cmd -s -d file file2 file3 file4'  with the usage
		/// 'cmd [-s] [-d <file>] <file>...', getCmdArgs() would return
		/// the list {file2, file3, file4}. </summary>
		/// <returns> String[] - list of command arguments that may appear
		///                    after options and option arguments. </returns>
		public virtual string[] CmdArgs
		{
			get
			{
			string[] retval = new string[theCmdArgs.Count];
			int i = 0;
	//JAVA TO C# CONVERTER WARNING: Unlike Java's ListIterator, enumerators in .NET do not allow altering the collection:
				for (IEnumerator it = theCmdArgs.GetEnumerator(); it.MoveNext();)
				{
					retval[i++] = (string)it.Current;
				}
			return retval;
			}
		}


		private Option theCurrentOption = null;
		private IEnumerator theOptionsIterator;
		private IList theOptions = null;
		private IList theCmdArgs = null;
		private OptionMatcher theOptionMatcher = null;

		///////////////////////////////////////////////////////////
		//
		//   Inner Classes
		//
		///////////////////////////////////////////////////////////

		// inner class to model an option
		internal class Option
		{
			internal char theArgLetter;
			internal string theArgument = null;
			public Option(char argLetter)
			{
				theArgLetter = argLetter;
			}
			public virtual string Arg
			{
				set
				{
				theArgument = value;
				}
			}
			public virtual bool hasArg()
			{
				return (!string.ReferenceEquals(theArgument, null));
			}
			public virtual char ArgLetter
			{
				get
				{
					return theArgLetter;
				}
			}
			public virtual string Argument
			{
				get
				{
					return theArgument;
				}
			}
		} // end class Option


		// inner class to query optString for a possible option match,
		// and whether or not a given legal option takes an argument. 
		//  
		internal class OptionMatcher
		{
			public OptionMatcher(string optString)
			{
			theOptString = optString;
			}
			public virtual bool match(char c)
			{
			bool retval = false;
			if (theOptString.IndexOf(c) != -1)
			{
				retval = true;
			}
			return retval;
			}
			public virtual bool hasArg(char c)
			{
			bool retval = false;
			int index = theOptString.IndexOf(c) + 1;
			if (index == theOptString.Length)
			{
				// reached end of theOptString
				retval = false;
			}
				else if (theOptString[index] == ':')
				{
					retval = true;
				}
				return retval;
			}
			internal string theOptString = null;
		} // end class OptionMatcher
	} // end class GetOpt


}