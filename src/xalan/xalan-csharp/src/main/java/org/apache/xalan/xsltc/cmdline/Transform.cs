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
 * $Id: Transform.java 468647 2006-10-28 06:59:33Z minchau $
 */

namespace org.apache.xalan.xsltc.cmdline
{



	using ErrorMsg = org.apache.xalan.xsltc.compiler.util.ErrorMsg;
	using XSLTCDTMManager = org.apache.xalan.xsltc.dom.XSLTCDTMManager;
	using AbstractTranslet = org.apache.xalan.xsltc.runtime.AbstractTranslet;
	using Constants = org.apache.xalan.xsltc.runtime.Constants;
	using Parameter = org.apache.xalan.xsltc.runtime.Parameter;
	using TransletOutputHandlerFactory = org.apache.xalan.xsltc.runtime.output.TransletOutputHandlerFactory;
	using SerializationHandler = org.apache.xml.serializer.SerializationHandler;

	using InputSource = org.xml.sax.InputSource;
	using SAXException = org.xml.sax.SAXException;
	using XMLReader = org.xml.sax.XMLReader;

	using DTMWSFilter = org.apache.xml.dtm.DTMWSFilter;
	using DOMWSFilter = org.apache.xalan.xsltc.dom.DOMWSFilter;

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// @author G. Todd Miller
	/// @author Morten Jorgensen
	/// </summary>
	public sealed class Transform
	{

		private SerializationHandler _handler;

		private string _fileName;
		private string _className;
		private string _jarFileSrc;
		private bool _isJarFileSpecified = false;
		private ArrayList _params = null;
		private bool _uri, _debug;
		private int _iterations;

		public Transform(string className, string fileName, bool uri, bool debug, int iterations)
		{
		_fileName = fileName;
		_className = className;
		_uri = uri;
		_debug = debug;
		_iterations = iterations;
		}

	   public string FileName
	   {
		   get
		   {
			   return _fileName;
		   }
	   }
	   public string ClassName
	   {
		   get
		   {
			   return _className;
		   }
	   }

		public ArrayList Parameters
		{
			set
			{
			_params = value;
			}
		}

		private void setJarFileInputSrc(bool flag, string jarFile)
		{
		// TODO: at this time we do not do anything with this
		// information, attempts to add the jarfile to the CLASSPATH
		// were successful via System.setProperty, but the effects
		// were not visible to the running JVM. For now we add jarfile
		// to CLASSPATH in the wrapper script that calls this program. 
		_isJarFileSpecified = flag;
		// TODO verify jarFile exists...
		_jarFileSrc = jarFile;
		}

		private void doTransform()
		{
		try
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Class clazz = ObjectFactory.findProviderClass(_className, ObjectFactory.findClassLoader(), true);
				Type clazz = ObjectFactory.findProviderClass(_className, ObjectFactory.findClassLoader(), true);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.runtime.AbstractTranslet translet = (org.apache.xalan.xsltc.runtime.AbstractTranslet)clazz.newInstance();
			AbstractTranslet translet = (AbstractTranslet)clazz.newInstance();
				translet.postInitialization();

			// Create a SAX parser and get the XMLReader object it uses
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();
			SAXParserFactory factory = SAXParserFactory.newInstance();
			try
			{
			factory.setFeature(org.apache.xalan.xsltc.runtime.Constants_Fields.NAMESPACE_FEATURE,true);
			}
			catch (Exception)
			{
			factory.NamespaceAware = true;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final javax.xml.parsers.SAXParser parser = factory.newSAXParser();
			SAXParser parser = factory.newSAXParser();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.xml.sax.XMLReader reader = parser.getXMLReader();
			XMLReader reader = parser.XMLReader;

			// Set the DOM's DOM builder as the XMLReader's SAX2 content handler
				XSLTCDTMManager dtmManager = (XSLTCDTMManager)XSLTCDTMManager.DTMManagerClass.newInstance();

			DTMWSFilter wsfilter;
			if (translet != null && translet is StripFilter)
			{
				wsfilter = new DOMWSFilter(translet);
			}
				else
				{
				wsfilter = null;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.xalan.xsltc.DOMEnhancedForDTM dom = (org.apache.xalan.xsltc.DOMEnhancedForDTM)dtmManager.getDTM(new javax.xml.transform.sax.SAXSource(reader, new org.xml.sax.InputSource(_fileName)), false, wsfilter, true, false, translet.hasIdCall());
				DOMEnhancedForDTM dom = (DOMEnhancedForDTM)dtmManager.getDTM(new SAXSource(reader, new InputSource(_fileName)), false, wsfilter, true, false, translet.hasIdCall());

			dom.DocumentURI = _fileName;
				translet.prepassDocument(dom);

			// Pass global parameters
			int n = _params.Count;
			for (int i = 0; i < n; i++)
			{
			Parameter param = (Parameter) _params[i];
			translet.addParameter(param._name, param._value);
			}

			// Transform the document
			TransletOutputHandlerFactory tohFactory = TransletOutputHandlerFactory.newInstance();
			tohFactory.OutputType = TransletOutputHandlerFactory.STREAM;
			tohFactory.Encoding = translet._encoding;
			tohFactory.OutputMethod = translet._method;

			if (_iterations == -1)
			{
			translet.transform(dom, tohFactory.SerializationHandler);
			}
			else if (_iterations > 0)
			{
			long mm = DateTimeHelperClass.CurrentUnixTimeMillis();
			for (int i = 0; i < _iterations; i++)
			{
				translet.transform(dom, tohFactory.SerializationHandler);
			}
			mm = DateTimeHelperClass.CurrentUnixTimeMillis() - mm;

			Console.Error.WriteLine("\n<!--");
			Console.Error.WriteLine("  transform  = " + (((double) mm) / ((double) _iterations)) + " ms");
			Console.Error.WriteLine("  throughput = " + (1000.0 / (((double) mm) / ((double) _iterations))) + " tps");
			Console.Error.WriteLine("-->");
			}
		}
		catch (TransletException e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + e.Message);
		}
		catch (Exception e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + e.Message);
		}
		catch (FileNotFoundException e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			ErrorMsg err = new ErrorMsg(ErrorMsg.FILE_NOT_FOUND_ERR, _fileName);
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + err.ToString());
		}
		catch (MalformedURLException e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_URI_ERR, _fileName);
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + err.ToString());
		}
		catch (ClassNotFoundException e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			ErrorMsg err = new ErrorMsg(ErrorMsg.CLASS_NOT_FOUND_ERR,_className);
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + err.ToString());
		}
			catch (UnknownHostException e)
			{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			ErrorMsg err = new ErrorMsg(ErrorMsg.INVALID_URI_ERR, _fileName);
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + err.ToString());
			}
		catch (SAXException e)
		{
			Exception ex = e.Exception;
			if (_debug)
			{
			if (ex != null)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
			}
			Console.Error.Write(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY));
			if (ex != null)
			{
			Console.Error.WriteLine(ex.Message);
			}
			else
			{
			Console.Error.WriteLine(e.Message);
			}
		}
		catch (Exception e)
		{
			if (_debug)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			Console.Error.WriteLine(new ErrorMsg(ErrorMsg.RUNTIME_ERROR_KEY) + e.Message);
		}
		}

		public static void printUsage()
		{
		Console.Error.WriteLine(new ErrorMsg(ErrorMsg.TRANSFORM_USAGE_STR));
		}

		public static void Main(string[] args)
		{
		try
		{
			if (args.Length > 0)
			{
			int i;
			int iterations = -1;
			bool uri = false, debug = false;
			bool isJarFileSpecified = false;
			string jarFile = null;

			// Parse options starting with '-'
			for (i = 0; i < args.Length && args[i][0] == '-'; i++)
			{
				if (args[i].Equals("-u"))
				{
				uri = true;
				}
				else if (args[i].Equals("-x"))
				{
				debug = true;
				}
				else if (args[i].Equals("-j"))
				{
				isJarFileSpecified = true;
				jarFile = args[++i];
				}
				else if (args[i].Equals("-n"))
				{
				try
				{
					iterations = int.Parse(args[++i]);
				}
				catch (System.FormatException)
				{
					// ignore
				}
				}
				else
				{
				printUsage();
				}
			}

			// Enough arguments left ?
			if (args.Length - i < 2)
			{
				printUsage();
			}

			// Get document file and class name
			Transform handler = new Transform(args[i + 1], args[i], uri, debug, iterations);
			handler.setJarFileInputSrc(isJarFileSpecified, jarFile);

			// Parse stylesheet parameters
			ArrayList @params = new ArrayList();
			for (i += 2; i < args.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int equal = args[i].indexOf('=');
				int equal = args[i].IndexOf('=');
				if (equal > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = args[i].substring(0, equal);
				string name = args[i].Substring(0, equal);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String value = args[i].substring(equal+1);
				string value = args[i].Substring(equal + 1);
				@params.Add(new Parameter(name, value));
				}
				else
				{
				printUsage();
				}
			}

			if (i == args.Length)
			{
				handler.Parameters = @params;
				handler.doTransform();
			}
			}
			else
			{
			printUsage();
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}
		}
	}

}