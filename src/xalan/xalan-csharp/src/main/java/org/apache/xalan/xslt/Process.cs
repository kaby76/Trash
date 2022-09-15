using System;
using System.Collections;
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
 * $Id: Process.java 475586 2006-11-16 05:19:36Z minchau $
 */
namespace org.apache.xalan.xslt
{


	using Version = org.apache.xalan.Version;
	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;
	using PrintTraceListener = org.apache.xalan.trace.PrintTraceListener;
	using TraceManager = org.apache.xalan.trace.TraceManager;
	using XalanProperties = org.apache.xalan.transformer.XalanProperties;
	using DefaultErrorHandler = org.apache.xml.utils.DefaultErrorHandler;

	using Document = org.w3c.dom.Document;
	using Node = org.w3c.dom.Node;

	using ContentHandler = org.xml.sax.ContentHandler;
	using EntityResolver = org.xml.sax.EntityResolver;
	using InputSource = org.xml.sax.InputSource;
	using XMLReader = org.xml.sax.XMLReader;
	using XMLReaderFactory = org.xml.sax.helpers.XMLReaderFactory;

	/// <summary>
	/// The main() method handles the Xalan command-line interface.
	/// @xsl.usage general
	/// </summary>
	public class Process
	{
	  /// <summary>
	  /// Prints argument options.
	  /// </summary>
	  /// <param name="resbundle"> Resource bundle </param>
	  protected internal static void printArgOptions(ResourceBundle resbundle)
	  {
		Console.WriteLine(resbundle.getString("xslProc_option")); //"xslproc options: ");
		Console.WriteLine("\n\t\t\t" + resbundle.getString("xslProc_common_options") + "\n");
		Console.WriteLine(resbundle.getString("optionXSLTC")); //"    [-XSLTC (use XSLTC for transformation)]
		Console.WriteLine(resbundle.getString("optionIN")); //"    [-IN inputXMLURL]");
		Console.WriteLine(resbundle.getString("optionXSL")); //"   [-XSL XSLTransformationURL]");
		Console.WriteLine(resbundle.getString("optionOUT")); //"   [-OUT outputFileName]");

		// System.out.println(resbundle.getString("optionE")); //"   [-E (Do not expand entity refs)]");
		Console.WriteLine(resbundle.getString("optionV")); //"   [-V (Version info)]");

		// System.out.println(resbundle.getString("optionVALIDATE")); //"   [-VALIDATE (Set whether validation occurs.  Validation is off by default.)]");
		Console.WriteLine(resbundle.getString("optionEDUMP")); //"   [-EDUMP {optional filename} (Do stackdump on error.)]");
		Console.WriteLine(resbundle.getString("optionXML")); //"   [-XML (Use XML formatter and add XML header.)]");
		Console.WriteLine(resbundle.getString("optionTEXT")); //"   [-TEXT (Use simple Text formatter.)]");
		Console.WriteLine(resbundle.getString("optionHTML")); //"   [-HTML (Use HTML formatter.)]");
		Console.WriteLine(resbundle.getString("optionPARAM")); //"   [-PARAM name expression (Set a stylesheet parameter)]");

		Console.WriteLine(resbundle.getString("optionMEDIA"));
		Console.WriteLine(resbundle.getString("optionFLAVOR"));
		Console.WriteLine(resbundle.getString("optionDIAG"));
		Console.WriteLine(resbundle.getString("optionURIRESOLVER")); //"   [-URIRESOLVER full class name (URIResolver to be used to resolve URIs)]");
		Console.WriteLine(resbundle.getString("optionENTITYRESOLVER")); //"   [-ENTITYRESOLVER full class name (EntityResolver to be used to resolve entities)]");
		waitForReturnKey(resbundle);
		Console.WriteLine(resbundle.getString("optionCONTENTHANDLER")); //"   [-CONTENTHANDLER full class name (ContentHandler to be used to serialize output)]");
		Console.WriteLine(resbundle.getString("optionSECUREPROCESSING")); //"   [-SECURE (set the secure processing feature to true)]");

		Console.WriteLine("\n\t\t\t" + resbundle.getString("xslProc_xalan_options") + "\n");

		Console.WriteLine(resbundle.getString("optionQC")); //"   [-QC (Quiet Pattern Conflicts Warnings)]");

		// System.out.println(resbundle.getString("optionQ"));  //"   [-Q  (Quiet Mode)]"); // sc 28-Feb-01 commented out
		Console.WriteLine(resbundle.getString("optionTT")); //"   [-TT (Trace the templates as they are being called.)]");
		Console.WriteLine(resbundle.getString("optionTG")); //"   [-TG (Trace each generation event.)]");
		Console.WriteLine(resbundle.getString("optionTS")); //"   [-TS (Trace each selection event.)]");
		Console.WriteLine(resbundle.getString("optionTTC")); //"   [-TTC (Trace the template children as they are being processed.)]");
		Console.WriteLine(resbundle.getString("optionTCLASS")); //"   [-TCLASS (TraceListener class for trace extensions.)]");
		Console.WriteLine(resbundle.getString("optionLINENUMBERS")); //"   [-L use line numbers]"
		Console.WriteLine(resbundle.getString("optionINCREMENTAL"));
		Console.WriteLine(resbundle.getString("optionNOOPTIMIMIZE"));
		Console.WriteLine(resbundle.getString("optionRL"));

		Console.WriteLine("\n\t\t\t" + resbundle.getString("xslProc_xsltc_options") + "\n");
		Console.WriteLine(resbundle.getString("optionXO"));
		waitForReturnKey(resbundle);
		Console.WriteLine(resbundle.getString("optionXD"));
		Console.WriteLine(resbundle.getString("optionXJ"));
		Console.WriteLine(resbundle.getString("optionXP"));
		Console.WriteLine(resbundle.getString("optionXN"));
		Console.WriteLine(resbundle.getString("optionXX"));
		Console.WriteLine(resbundle.getString("optionXT"));
	  }

	  /// <summary>
	  /// Command line interface to transform an XML document according to
	  /// the instructions found in an XSL stylesheet.  
	  /// <para>The Process class provides basic functionality for 
	  /// performing transformations from the command line.  To see a 
	  /// list of arguments supported, call with zero arguments.</para>
	  /// <para>To set stylesheet parameters from the command line, use 
	  /// <code>-PARAM name expression</code>. If you want to set the 
	  /// parameter to a string value, simply pass the string value 
	  /// as-is, and it will be interpreted as a string.  (Note: if 
	  /// the value has spaces in it, you may need to quote it depending 
	  /// on your shell environment).</para>
	  /// </summary>
	  /// <param name="argv"> Input parameters from command line </param>
	  public static void Main(string[] argv)
	  {

		// Runtime.getRuntime().traceMethodCalls(false); // turns Java tracing off
		bool doStackDumpOnError = false;
		bool setQuietMode = false;
		bool doDiag = false;
		string msg = null;
		bool isSecureProcessing = false;

		// Runtime.getRuntime().traceMethodCalls(false);
		// Runtime.getRuntime().traceInstructions(false);

		/// <summary>
		/// The default diagnostic writer...
		/// </summary>
		PrintWriter diagnosticsWriter = new PrintWriter(System.err, true);
		PrintWriter dumpWriter = diagnosticsWriter;
		ResourceBundle resbundle = (XSLMessages.loadResourceBundle(org.apache.xml.utils.res.XResourceBundle.ERROR_RESOURCES));
		string flavor = "s2s";

		if (argv.Length < 1)
		{
		  printArgOptions(resbundle);
		}
		else
		{
		  bool useXSLTC = false;
		  for (int i = 0; i < argv.Length; i++)
		  {
			if ("-XSLTC".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  useXSLTC = true;
			}
		  }

		  TransformerFactory tfactory;
		  if (useXSLTC)
		  {
		 string key = "javax.xml.transform.TransformerFactory";
		 string value = "org.apache.xalan.xsltc.trax.TransformerFactoryImpl";
		 Properties props = System.getProperties();
		 props.put(key, value);
		 System.setProperties(props);
		  }

		  try
		  {
			tfactory = TransformerFactory.newInstance();
			tfactory.setErrorListener(new DefaultErrorHandler(false));
		  }
		  catch (TransformerFactoryConfigurationError pfe)
		  {
			pfe.printStackTrace(dumpWriter);
	//      "XSL Process was not successful.");
			msg = XSLMessages.createMessage(XSLTErrorResources.ER_NOT_SUCCESSFUL, null);
			diagnosticsWriter.println(msg);

			tfactory = null; // shut up compiler

			doExit(msg);
		  }

		  bool formatOutput = false;
		  bool useSourceLocation = false;
		  string inFileName = null;
		  string outFileName = null;
		  string dumpFileName = null;
		  string xslFileName = null;
		  string treedumpFileName = null;
		  PrintTraceListener tracer = null;
		  string outputType = null;
		  string media = null;
		  ArrayList @params = new ArrayList();
		  bool quietConflictWarnings = false;
		  URIResolver uriResolver = null;
		  EntityResolver entityResolver = null;
		  ContentHandler contentHandler = null;
		  int recursionLimit = -1;

		  for (int i = 0; i < argv.Length; i++)
		  {
			if ("-XSLTC".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  // The -XSLTC option has been processed.
			}
			else if ("-TT".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				if (null == tracer)
				{
				  tracer = new PrintTraceListener(diagnosticsWriter);
				}

				tracer.m_traceTemplates = true;
			  }
			  else
			  {
				printInvalidXSLTCOption("-TT");
			  }

			  // tfactory.setTraceTemplates(true);
			}
			else if ("-TG".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				if (null == tracer)
				{
				  tracer = new PrintTraceListener(diagnosticsWriter);
				}

				tracer.m_traceGeneration = true;
			  }
			  else
			  {
				printInvalidXSLTCOption("-TG");
			  }

			  // tfactory.setTraceSelect(true);
			}
			else if ("-TS".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				if (null == tracer)
				{
				  tracer = new PrintTraceListener(diagnosticsWriter);
				}

				tracer.m_traceSelection = true;
			  }
			  else
			  {
				printInvalidXSLTCOption("-TS");
			  }

			  // tfactory.setTraceTemplates(true);
			}
			else if ("-TTC".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				if (null == tracer)
				{
				  tracer = new PrintTraceListener(diagnosticsWriter);
				}

				tracer.m_traceElements = true;
			  }
			  else
			  {
				printInvalidXSLTCOption("-TTC");
			  }

			  // tfactory.setTraceTemplateChildren(true);
			}
			else if ("-INDENT".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  int indentAmount;

			  if (((i + 1) < argv.Length) && (argv[i + 1][0] != '-'))
			  {
				indentAmount = int.Parse(argv[++i]);
			  }
			  else
			  {
				indentAmount = 0;
			  }

			  // TBD:
			  // xmlProcessorLiaison.setIndent(indentAmount);
			}
			else if ("-IN".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length && argv[i + 1][0] != '-')
			  {
				inFileName = argv[++i];
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-IN"})); //"Missing argument for);
			  }
			}
			else if ("-MEDIA".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length)
			  {
				media = argv[++i];
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-MEDIA"})); //"Missing argument for);
			  }
			}
			else if ("-OUT".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length && argv[i + 1][0] != '-')
			  {
				outFileName = argv[++i];
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-OUT"})); //"Missing argument for);
			  }
			}
			else if ("-XSL".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length && argv[i + 1][0] != '-')
			  {
				xslFileName = argv[++i];
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-XSL"})); //"Missing argument for);
			  }
			}
			else if ("-FLAVOR".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length)
			  {
				flavor = argv[++i];
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-FLAVOR"})); //"Missing argument for);
			  }
			}
			else if ("-PARAM".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 2 < argv.Length)
			  {
				string name = argv[++i];

				@params.Add(name);

				string expression = argv[++i];

				@params.Add(expression);
			  }
			  else
			  {
				Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-PARAM"})); //"Missing argument for);
			  }
			}
			else if ("-E".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{

			  // TBD:
			  // xmlProcessorLiaison.setShouldExpandEntityRefs(false);
			}
			else if ("-V".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  diagnosticsWriter.println(resbundle.getString("version") + Version.Version + ", " + resbundle.getString("version2")); // "<<<<<<<");
			}
			else if ("-QC".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				quietConflictWarnings = true;
			  }
			  else
			  {
				printInvalidXSLTCOption("-QC");
			  }
			}
			else if ("-Q".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  setQuietMode = true;
			}
			else if ("-DIAG".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  doDiag = true;
			}
			else if ("-XML".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  outputType = "xml";
			}
			else if ("-TEXT".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  outputType = "text";
			}
			else if ("-HTML".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  outputType = "html";
			}
			else if ("-EDUMP".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  doStackDumpOnError = true;

			  if (((i + 1) < argv.Length) && (argv[i + 1][0] != '-'))
			  {
				dumpFileName = argv[++i];
			  }
			}
			else if ("-URIRESOLVER".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length)
			  {
				try
				{
				  uriResolver = (URIResolver) ObjectFactory.newInstance(argv[++i], ObjectFactory.findClassLoader(), true);

				  tfactory.setURIResolver(uriResolver);
				}
				catch (ObjectFactory.ConfigurationError)
				{
					msg = XSLMessages.createMessage(XSLTErrorResources.ER_CLASS_NOT_FOUND_FOR_OPTION, new object[]{"-URIResolver"});
				  Console.Error.WriteLine(msg);
				  doExit(msg);
				}
			  }
			  else
			  {
				msg = XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-URIResolver"}); //"Missing argument for);
				Console.Error.WriteLine(msg);
				doExit(msg);
			  }
			}
			else if ("-ENTITYRESOLVER".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length)
			  {
				try
				{
				  entityResolver = (EntityResolver) ObjectFactory.newInstance(argv[++i], ObjectFactory.findClassLoader(), true);
				}
				catch (ObjectFactory.ConfigurationError)
				{
					msg = XSLMessages.createMessage(XSLTErrorResources.ER_CLASS_NOT_FOUND_FOR_OPTION, new object[]{"-EntityResolver"});
				  Console.Error.WriteLine(msg);
				  doExit(msg);
				}
			  }
			  else
			  {
	//            "Missing argument for);
				  msg = XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-EntityResolver"});
				Console.Error.WriteLine(msg);
				doExit(msg);
			  }
			}
			else if ("-CONTENTHANDLER".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (i + 1 < argv.Length)
			  {
				try
				{
				  contentHandler = (ContentHandler) ObjectFactory.newInstance(argv[++i], ObjectFactory.findClassLoader(), true);
				}
				catch (ObjectFactory.ConfigurationError)
				{
					msg = XSLMessages.createMessage(XSLTErrorResources.ER_CLASS_NOT_FOUND_FOR_OPTION, new object[]{"-ContentHandler"});
				  Console.Error.WriteLine(msg);
				  doExit(msg);
				}
			  }
			  else
			  {
	//            "Missing argument for);
				  msg = XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-ContentHandler"});
				Console.Error.WriteLine(msg);
				doExit(msg);
			  }
			}
			else if ("-L".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				tfactory.setAttribute(XalanProperties.SOURCE_LOCATION, true);
			  }
			  else
			  {
				printInvalidXSLTCOption("-L");
			  }
			}
			else if ("-INCREMENTAL".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				tfactory.setAttribute("http://xml.apache.org/xalan/features/incremental", true);
			  }
			  else
			  {
				printInvalidXSLTCOption("-INCREMENTAL");
			  }
			}
			else if ("-NOOPTIMIZE".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  // Default is true.
			  //
			  // %REVIEW% We should have a generalized syntax for negative
			  // switches...  and probably should accept the inverse even
			  // if it is the default.
			  if (!useXSLTC)
			  {
				tfactory.setAttribute("http://xml.apache.org/xalan/features/optimize", false);
			  }
			  else
			  {
				printInvalidXSLTCOption("-NOOPTIMIZE");
			  }
			}
			else if ("-RL".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (!useXSLTC)
			  {
				if (i + 1 < argv.Length)
				{
				  recursionLimit = int.Parse(argv[++i]);
				}
				else
				{
				  Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-rl"})); //"Missing argument for);
				}
			  }
			  else
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				 i++;
				}

				printInvalidXSLTCOption("-RL");
			  }
			}
			// Generate the translet class and optionally specify the name
			// of the translet class.
			else if ("-XO".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				  tfactory.setAttribute("generate-translet", "true");
				  tfactory.setAttribute("translet-name", argv[++i]);
				}
				else
				{
				  tfactory.setAttribute("generate-translet", "true");
				}
			  }
			  else
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				 i++;
				}
				printInvalidXalanOption("-XO");
			  }
			}
			// Specify the destination directory for the translet classes.
			else if ("-XD".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				  tfactory.setAttribute("destination-directory", argv[++i]);
				}
				else
				{
				  Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-XD"})); //"Missing argument for);
				}

			  }
			  else
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				 i++;
				}

				printInvalidXalanOption("-XD");
			  }
			}
			// Specify the jar file name which the translet classes are packaged into.
			else if ("-XJ".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				  tfactory.setAttribute("generate-translet", "true");
				  tfactory.setAttribute("jar-name", argv[++i]);
				}
				else
				{
				  Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-XJ"})); //"Missing argument for);
				}
			  }
			  else
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				 i++;
				}

				printInvalidXalanOption("-XJ");
			  }

			}
			// Specify the package name prefix for the generated translet classes.
			else if ("-XP".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				  tfactory.setAttribute("package-name", argv[++i]);
				}
				else
				{
				  Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_MISSING_ARG_FOR_OPTION, new object[]{"-XP"})); //"Missing argument for);
				}
			  }
			  else
			  {
				if (i + 1 < argv.Length && argv[i + 1][0] != '-')
				{
				 i++;
				}

				printInvalidXalanOption("-XP");
			  }

			}
			// Enable template inlining.
			else if ("-XN".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				tfactory.setAttribute("enable-inlining", "true");
			  }
			  else
			  {
				printInvalidXalanOption("-XN");
			  }
			}
			// Turns on additional debugging message output
			else if ("-XX".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				tfactory.setAttribute("debug", "true");
			  }
			  else
			  {
				printInvalidXalanOption("-XX");
			  }
			}
			// Create the Transformer from the translet if the translet class is newer
			// than the stylesheet.
			else if ("-XT".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  if (useXSLTC)
			  {
				tfactory.setAttribute("auto-translet", "true");
			  }
			  else
			  {
				printInvalidXalanOption("-XT");
			  }
			}
			else if ("-SECURE".Equals(argv[i], StringComparison.OrdinalIgnoreCase))
			{
			  isSecureProcessing = true;
			  try
			  {
				tfactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
			  }
			  catch (TransformerConfigurationException)
			  {
			  }
			}
			else
			{
			  Console.Error.WriteLine(XSLMessages.createMessage(XSLTErrorResources.ER_INVALID_OPTION, new object[]{argv[i]})); //"Invalid argument:);
			}
		  }

		  // Print usage instructions if no xml and xsl file is specified in the command line
		  if (string.ReferenceEquals(inFileName, null) && string.ReferenceEquals(xslFileName, null))
		  {
			  msg = resbundle.getString("xslProc_no_input");
			Console.Error.WriteLine(msg);
			doExit(msg);
		  }

		  // Note that there are usage cases for calling us without a -IN arg
		  // The main XSL transformation occurs here!
		  try
		  {
			long start = DateTimeHelper.CurrentUnixTimeMillis();

			if (null != dumpFileName)
			{
			  dumpWriter = new PrintWriter(new StreamWriter(dumpFileName));
			}

			Templates stylesheet = null;

			if (null != xslFileName)
			{
			  if (flavor.Equals("d2d"))
			  {

				// Parse in the xml data into a DOM
				DocumentBuilderFactory dfactory = DocumentBuilderFactory.newInstance();

				dfactory.setNamespaceAware(true);

				if (isSecureProcessing)
				{
				  try
				  {
					dfactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
				  }
				  catch (ParserConfigurationException)
				  {
				  }
				}

				DocumentBuilder docBuilder = dfactory.newDocumentBuilder();
				Node xslDOM = docBuilder.parse(new InputSource(xslFileName));

				stylesheet = tfactory.newTemplates(new DOMSource(xslDOM, xslFileName));
			  }
			  else
			  {
				// System.out.println("Calling newTemplates: "+xslFileName);
				stylesheet = tfactory.newTemplates(new StreamSource(xslFileName));
				// System.out.println("Done calling newTemplates: "+xslFileName);
			  }
			}

			PrintWriter resultWriter;
			StreamResult strResult;

			if (null != outFileName)
			{
			  strResult = new StreamResult(new FileStream(outFileName, FileMode.Create, FileAccess.Write));
			  // One possible improvement might be to ensure this is 
			  //  a valid URI before setting the systemId, but that 
			  //  might have subtle changes that pre-existing users 
			  //  might notice; we can think about that later -sc r1.46
			  strResult.setSystemId(outFileName);
			}
			else
			{
			  strResult = new StreamResult(System.out);
		  // We used to default to incremental mode in this case.
		  // We've since decided that since the -INCREMENTAL switch is
		  // available, that default is probably not necessary nor
		  // necessarily a good idea.
			}

			SAXTransformerFactory stf = (SAXTransformerFactory) tfactory;

			// This is currently controlled via TransformerFactoryImpl.
			if (!useXSLTC && useSourceLocation)
			{
			   stf.setAttribute(XalanProperties.SOURCE_LOCATION, true);
			}

			// Did they pass in a stylesheet, or should we get it from the 
			// document?
			if (null == stylesheet)
			{
			  Source source = stf.getAssociatedStylesheet(new StreamSource(inFileName), media, null, null);

			  if (null != source)
			  {
				stylesheet = tfactory.newTemplates(source);
			  }
			  else
			  {
				if (null != media)
				{
				  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_STYLESHEET_IN_MEDIA, new object[]{inFileName, media})); //"No stylesheet found in: "
				}
												// + inFileName + ", media="
												// + media);
				else
				{
				  throw new TransformerException(XSLMessages.createMessage(XSLTErrorResources.ER_NO_STYLESHEET_PI, new object[]{inFileName})); //"No xml-stylesheet PI found in: "
				}
												 //+ inFileName);
			  }
			}

			if (null != stylesheet)
			{
			  Transformer transformer = flavor.Equals("th") ? null : stylesheet.newTransformer();
			  transformer.setErrorListener(new DefaultErrorHandler(false));

			  // Override the output format?
			  if (null != outputType)
			  {
				transformer.setOutputProperty(OutputKeys.METHOD, outputType);
			  }

			  if (transformer is org.apache.xalan.transformer.TransformerImpl)
			  {
				org.apache.xalan.transformer.TransformerImpl impl = (org.apache.xalan.transformer.TransformerImpl)transformer;
				TraceManager tm = impl.TraceManager;

				if (null != tracer)
				{
				  tm.addTraceListener(tracer);
				}

				impl.QuietConflictWarnings = quietConflictWarnings;

				// This is currently controlled via TransformerFactoryImpl.
				if (useSourceLocation)
				{
				  impl.setProperty(XalanProperties.SOURCE_LOCATION, true);
				}

			if (recursionLimit > 0)
			{
			  impl.RecursionLimit = recursionLimit;
			}

				// sc 28-Feb-01 if we re-implement this, please uncomment helpmsg in printArgOptions
				// impl.setDiagnosticsOutput( setQuietMode ? null : diagnosticsWriter );
			  }

			  int nParams = @params.Count;

			  for (int i = 0; i < nParams; i += 2)
			  {
				transformer.setParameter((string) @params[i], (string) @params[i + 1]);
			  }

			  if (uriResolver != null)
			  {
				transformer.setURIResolver(uriResolver);
			  }

			  if (null != inFileName)
			  {
				if (flavor.Equals("d2d"))
				{

				  // Parse in the xml data into a DOM
				  DocumentBuilderFactory dfactory = DocumentBuilderFactory.newInstance();

				  dfactory.setCoalescing(true);
				  dfactory.setNamespaceAware(true);

				  if (isSecureProcessing)
				  {
					try
					{
					  dfactory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
					}
					catch (ParserConfigurationException)
					{
					}
				  }

				  DocumentBuilder docBuilder = dfactory.newDocumentBuilder();

				  if (entityResolver != null)
				  {
					docBuilder.setEntityResolver(entityResolver);
				  }

				  Node xmlDoc = docBuilder.parse(new InputSource(inFileName));
				  Document doc = docBuilder.newDocument();
				  org.w3c.dom.DocumentFragment outNode = doc.createDocumentFragment();

				  transformer.transform(new DOMSource(xmlDoc, inFileName), new DOMResult(outNode));

				  // Now serialize output to disk with identity transformer
				  Transformer serializer = stf.newTransformer();
				  serializer.setErrorListener(new DefaultErrorHandler(false));

				  Properties serializationProps = stylesheet.getOutputProperties();

				  serializer.setOutputProperties(serializationProps);

				  if (contentHandler != null)
				  {
					SAXResult result = new SAXResult(contentHandler);

					serializer.transform(new DOMSource(outNode), result);
				  }
				  else
				  {
					serializer.transform(new DOMSource(outNode), strResult);
				  }
				}
				else if (flavor.Equals("th"))
				{
				  for (int i = 0; i < 1; i++) // Loop for diagnosing bugs with inconsistent behavior
				  {
				  // System.out.println("Testing the TransformerHandler...");

				  // ===============
				  XMLReader reader = null;

				  // Use JAXP1.1 ( if possible )      
				  try
				  {
					javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();

					factory.setNamespaceAware(true);

					if (isSecureProcessing)
					{
					  try
					  {
						factory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
					  }
					  catch (org.xml.sax.SAXException)
					  {
					  }
					}

					javax.xml.parsers.SAXParser jaxpParser = factory.newSAXParser();

					reader = jaxpParser.getXMLReader();
				  }
				  catch (ParserConfigurationException ex)
				  {
					throw new org.xml.sax.SAXException(ex);
				  }
				  catch (javax.xml.parsers.FactoryConfigurationError ex1)
				  {
					throw new org.xml.sax.SAXException(ex1.ToString());
				  }
				  catch (System.MissingMethodException)
				  {
				  }
				  catch (AbstractMethodError)
				  {
				  }

				  if (null == reader)
				  {
					reader = XMLReaderFactory.createXMLReader();
				  }

				  if (!useXSLTC)
				  {
					stf.setAttribute(org.apache.xalan.processor.TransformerFactoryImpl.FEATURE_INCREMENTAL, true);
				  }

				  TransformerHandler th = stf.newTransformerHandler(stylesheet);

				  reader.setContentHandler(th);
				  reader.setDTDHandler(th);

				  if (th is org.xml.sax.ErrorHandler)
				  {
					reader.setErrorHandler((org.xml.sax.ErrorHandler)th);
				  }

				  try
				  {
					reader.setProperty("http://xml.org/sax/properties/lexical-handler", th);
				  }
				  catch (org.xml.sax.SAXNotRecognizedException)
				  {
				  }
				  catch (org.xml.sax.SAXNotSupportedException)
				  {
				  }
				  try
				  {
					reader.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
				  }
				  catch (org.xml.sax.SAXException)
				  {
				  }

				  th.setResult(strResult);

				  reader.parse(new InputSource(inFileName));
				  }
				}
				else
				{
				  if (entityResolver != null)
				  {
					XMLReader reader = null;

					// Use JAXP1.1 ( if possible )      
					try
					{
					  javax.xml.parsers.SAXParserFactory factory = javax.xml.parsers.SAXParserFactory.newInstance();

					  factory.setNamespaceAware(true);

					  if (isSecureProcessing)
					  {
						try
						{
						  factory.setFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
						}
						catch (org.xml.sax.SAXException)
						{
						}
					  }

					  javax.xml.parsers.SAXParser jaxpParser = factory.newSAXParser();

					  reader = jaxpParser.getXMLReader();
					}
					catch (ParserConfigurationException ex)
					{
					  throw new org.xml.sax.SAXException(ex);
					}
					catch (javax.xml.parsers.FactoryConfigurationError ex1)
					{
					  throw new org.xml.sax.SAXException(ex1.ToString());
					}
					catch (System.MissingMethodException)
					{
					}
					catch (AbstractMethodError)
					{
					}

					if (null == reader)
					{
					  reader = XMLReaderFactory.createXMLReader();
					}

					reader.setEntityResolver(entityResolver);

					if (contentHandler != null)
					{
					  SAXResult result = new SAXResult(contentHandler);

					  transformer.transform(new SAXSource(reader, new InputSource(inFileName)), result);
					}
					else
					{
					  transformer.transform(new SAXSource(reader, new InputSource(inFileName)), strResult);
					}
				  }
				  else if (contentHandler != null)
				  {
					SAXResult result = new SAXResult(contentHandler);

					transformer.transform(new StreamSource(inFileName), result);
				  }
				  else
				  {
					// System.out.println("Starting transform");
					transformer.transform(new StreamSource(inFileName), strResult);
					// System.out.println("Done with transform");
				  }
				}
			  }
			  else
			  {
				StringReader reader = new StringReader("<?xml version=\"1.0\"?> <doc/>");

				transformer.transform(new StreamSource(reader), strResult);
			  }
			}
			else
			{
	//          "XSL Process was not successful.");
				msg = XSLMessages.createMessage(XSLTErrorResources.ER_NOT_SUCCESSFUL, null);
			  diagnosticsWriter.println(msg);
			  doExit(msg);
			}

		// close output streams
		if (null != outFileName && strResult != null)
		{
			 Stream @out = strResult.getOutputStream();
			 java.io.Writer writer = strResult.getWriter();
			 try
			 {
				  if (@out != null)
				  {
					  @out.Close();
				  }
				  if (writer != null)
				  {
					  writer.close();
				  }
			 }
			 catch (java.io.IOException)
			 {
			 }
		}

			long stop = DateTimeHelper.CurrentUnixTimeMillis();
			long millisecondsDuration = stop - start;

			if (doDiag)
			{
				object[] msgArgs = new object[]{inFileName, xslFileName, new long?(millisecondsDuration)};
				msg = XSLMessages.createMessage("diagTiming", msgArgs);
				diagnosticsWriter.println('\n');
				  diagnosticsWriter.println(msg);
			}

		  }
		  catch (Exception throwable)
		  {
			while (throwable is org.apache.xml.utils.WrappedRuntimeException)
			{
			  throwable = ((org.apache.xml.utils.WrappedRuntimeException) throwable).Exception;
			}

			if ((throwable is System.NullReferenceException) || (throwable is System.InvalidCastException))
			{
			  doStackDumpOnError = true;
			}

			diagnosticsWriter.println();

			if (doStackDumpOnError)
			{
			  throwable.printStackTrace(dumpWriter);
			}
			else
			{
			  DefaultErrorHandler.printLocation(diagnosticsWriter, throwable);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  diagnosticsWriter.println(XSLMessages.createMessage(XSLTErrorResources.ER_XSLT_ERROR, null) + " (" + throwable.GetType().FullName + "): " + throwable.Message);
			}

			// diagnosticsWriter.println(XSLMessages.createMessage(XSLTErrorResources.ER_NOT_SUCCESSFUL, null)); //"XSL Process was not successful.");
			if (null != dumpFileName)
			{
			  dumpWriter.close();
			}

			doExit(throwable.Message);
		  }

		  if (null != dumpFileName)
		  {
			dumpWriter.close();
		  }

		  if (null != diagnosticsWriter)
		  {

			// diagnosticsWriter.close();
		  }

		  // if(!setQuietMode)
		  //  diagnosticsWriter.println(resbundle.getString("xsldone")); //"Xalan: done");
		  // else
		  // diagnosticsWriter.println("");  //"Xalan: done");
		}
	  }

	  /// <summary>
	  /// It is _much_ easier to debug under VJ++ if I can set a single breakpoint 
	  /// before this blows itself out of the water...
	  /// (I keep checking this in, it keeps vanishing. Grr!)
	  /// 
	  /// </summary>
	  internal static void doExit(string msg)
	  {
		throw new Exception(msg);
	  }

	  /// <summary>
	  /// Wait for a return key to continue
	  /// </summary>
	  /// <param name="resbundle"> The resource bundle </param>
	  private static void waitForReturnKey(ResourceBundle resbundle)
	  {
		Console.WriteLine(resbundle.getString("xslProc_return_to_continue"));
		try
		{
		  while (Console.Read() != '\n')
		  {
				  ;
		  }
		}
		catch (java.io.IOException)
		{
		}
	  }

	  /// <summary>
	  /// Print a message if an option cannot be used with -XSLTC.
	  /// </summary>
	  /// <param name="option"> The option String </param>
	  private static void printInvalidXSLTCOption(string option)
	  {
		Console.Error.WriteLine(XSLMessages.createMessage("xslProc_invalid_xsltc_option", new object[]{option}));
	  }

	  /// <summary>
	  /// Print a message if an option can only be used with -XSLTC.
	  /// </summary>
	  /// <param name="option"> The option String </param>
	  private static void printInvalidXalanOption(string option)
	  {
		Console.Error.WriteLine(XSLMessages.createMessage("xslProc_invalid_xalan_option", new object[]{option}));
	  }
	}

}