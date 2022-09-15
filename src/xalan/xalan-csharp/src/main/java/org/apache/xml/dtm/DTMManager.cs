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
 * $Id: DTMManager.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{
	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;
	using PrefixResolver = org.apache.xml.utils.PrefixResolver;
	using XMLStringFactory = org.apache.xml.utils.XMLStringFactory;

	/// <summary>
	/// A DTMManager instance can be used to create DTM and
	/// DTMIterator objects, and manage the DTM objects in the system.
	/// 
	/// <para>The system property that determines which Factory implementation
	/// to create is named "org.apache.xml.utils.DTMFactory". This
	/// property names a concrete subclass of the DTMFactory abstract
	///  class. If the property is not defined, a platform default is be used.</para>
	/// 
	/// <para>An instance of this class <emph>must</emph> be safe to use across
	/// thread instances.  It is expected that a client will create a single instance
	/// of a DTMManager to use across multiple threads.  This will allow sharing
	/// of DTMs across multiple processes.</para>
	/// 
	/// <para>Note: this class is incomplete right now.  It will be pretty much
	/// modeled after javax.xml.transform.TransformerFactory in terms of its
	/// factory support.</para>
	/// 
	/// <para>State: In progress!!</para>
	/// </summary>
	public abstract class DTMManager
	{

	  /// <summary>
	  /// The default property name to load the manager. </summary>
	  private const string defaultPropName = "org.apache.xml.dtm.DTMManager";

	  /// <summary>
	  /// The default class name to use as the manager. </summary>
	  private static string defaultClassName = "org.apache.xml.dtm.ref.DTMManagerDefault";

	  /// <summary>
	  /// Factory for creating XMLString objects.
	  ///  %TBD% Make this set by the caller.
	  /// </summary>
	  protected internal XMLStringFactory m_xsf = null;

	  /// <summary>
	  /// Default constructor is protected on purpose.
	  /// </summary>
	  protected internal DTMManager()
	  {
	  }

	  /// <summary>
	  /// Get the XMLStringFactory used for the DTMs.
	  /// 
	  /// </summary>
	  /// <returns> a valid XMLStringFactory object, or null if it hasn't been set yet. </returns>
	  public virtual XMLStringFactory XMLStringFactory
	  {
		  get
		  {
			return m_xsf;
		  }
		  set
		  {
			m_xsf = value;
		  }
	  }


	  /// <summary>
	  /// Obtain a new instance of a <code>DTMManager</code>.
	  /// This static method creates a new factory instance
	  /// This method uses the following ordered lookup procedure to determine
	  /// the <code>DTMManager</code> implementation class to
	  /// load:
	  /// <ul>
	  /// <li>
	  /// Use the <code>org.apache.xml.dtm.DTMManager</code> system
	  /// property.
	  /// </li>
	  /// <li>
	  /// Use the JAVA_HOME(the parent directory where jdk is
	  /// installed)/lib/xalan.properties for a property file that contains the
	  /// name of the implementation class keyed on the same value as the
	  /// system property defined above.
	  /// </li>
	  /// <li>
	  /// Use the Services API (as detailed in the JAR specification), if
	  /// available, to determine the classname. The Services API will look
	  /// for a classname in the file
	  /// <code>META-INF/services/org.apache.xml.dtm.DTMManager</code>
	  /// in jars available to the runtime.
	  /// </li>
	  /// <li>
	  /// Use the default <code>DTMManager</code> classname, which is
	  /// <code>org.apache.xml.dtm.ref.DTMManagerDefault</code>.
	  /// </li>
	  /// </ul>
	  /// 
	  /// Once an application has obtained a reference to a <code>
	  /// DTMManager</code> it can use the factory to configure
	  /// and obtain parser instances.
	  /// </summary>
	  /// <returns> new DTMManager instance, never null.
	  /// </returns>
	  /// <exception cref="DTMConfigurationException">
	  /// if the implementation is not available or cannot be instantiated. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static DTMManager newInstance(org.apache.xml.utils.XMLStringFactory xsf) throws DTMConfigurationException
	  public static DTMManager newInstance(XMLStringFactory xsf)
	  {
		DTMManager factoryImpl = null;
		try
		{
		  factoryImpl = (DTMManager) ObjectFactory.createObject(defaultPropName, defaultClassName);
		}
		catch (ObjectFactory.ConfigurationError e)
		{
		  throw new DTMConfigurationException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NO_DEFAULT_IMPL, null), e.Exception);
			//"No default implementation found");
		}

		if (factoryImpl == null)
		{
		  throw new DTMConfigurationException(XMLMessages.createXMLMessage(XMLErrorResources.ER_NO_DEFAULT_IMPL, null));
			//"No default implementation found");
		}

		factoryImpl.XMLStringFactory = xsf;

		return factoryImpl;
	  }

	  /// <summary>
	  /// Get an instance of a DTM, loaded with the content from the
	  /// specified source.  If the unique flag is true, a new instance will
	  /// always be returned.  Otherwise it is up to the DTMManager to return a
	  /// new instance or an instance that it already created and may be being used
	  /// by someone else.
	  /// 
	  /// (More parameters may eventually need to be added for error handling
	  /// and entity resolution, and to better control selection of implementations.)
	  /// </summary>
	  /// <param name="source"> the specification of the source object, which may be null,
	  ///               in which case it is assumed that node construction will take
	  ///               by some other means. </param>
	  /// <param name="unique"> true if the returned DTM must be unique, probably because it
	  /// is going to be mutated. </param>
	  /// <param name="whiteSpaceFilter"> Enables filtering of whitespace nodes, and may
	  ///                         be null. </param>
	  /// <param name="incremental"> true if the DTM should be built incrementally, if
	  ///                    possible. </param>
	  /// <param name="doIndexing"> true if the caller considers it worth it to use 
	  ///                   indexing schemes.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public abstract DTM getDTM(javax.xml.transform.Source source, bool unique, DTMWSFilter whiteSpaceFilter, bool incremental, bool doIndexing);

	  /// <summary>
	  /// Get the instance of DTM that "owns" a node handle.
	  /// </summary>
	  /// <param name="nodeHandle"> the nodeHandle.
	  /// </param>
	  /// <returns> a non-null DTM reference. </returns>
	  public abstract DTM getDTM(int nodeHandle);

	  /// <summary>
	  /// Given a W3C DOM node, try and return a DTM handle.
	  /// Note: calling this may be non-optimal.
	  /// </summary>
	  /// <param name="node"> Non-null reference to a DOM node.
	  /// </param>
	  /// <returns> a valid DTM handle. </returns>
	  public abstract int getDTMHandleFromNode(org.w3c.dom.Node node);

	  /// <summary>
	  /// Creates a DTM representing an empty <code>DocumentFragment</code> object. </summary>
	  /// <returns> a non-null DTM reference. </returns>
	  public abstract DTM createDocumentFragment();

	  /// <summary>
	  /// Release a DTM either to a lru pool, or completely remove reference.
	  /// DTMs without system IDs are always hard deleted.
	  /// State: experimental.
	  /// </summary>
	  /// <param name="dtm"> The DTM to be released. </param>
	  /// <param name="shouldHardDelete"> True if the DTM should be removed no matter what. </param>
	  /// <returns> true if the DTM was removed, false if it was put back in a lru pool. </returns>
	  public abstract bool release(DTM dtm, bool shouldHardDelete);

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based on an XPath
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </summary>
	  /// <param name="xpathCompiler"> ??? Somehow we need to pass in a subpart of the
	  /// expression.  I hate to do this with strings, since the larger expression
	  /// has already been parsed.
	  /// </param>
	  /// <param name="pos"> The position in the expression. </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public abstract DTMIterator createDTMIterator(object xpathCompiler, int pos);

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based on an XPath
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </summary>
	  /// <param name="xpathString"> Must be a valid string expressing a
	  /// <a href="http://www.w3.org/TR/xpath#NT-LocationPath>LocationPath</a> or
	  /// a <a href="http://www.w3.org/TR/xpath#NT-UnionExpr">UnionExpr</a>.
	  /// </param>
	  /// <param name="presolver"> An object that can resolve prefixes to namespace URLs.
	  /// </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public abstract DTMIterator createDTMIterator(string xpathString, PrefixResolver presolver);

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> based only on a whatToShow
	  /// and a DTMFilter.  The traversal semantics are defined as the
	  /// descendant access.
	  /// <para>
	  /// Note that DTMIterators may not be an exact match to DOM
	  /// NodeIterators. They are initialized and used in much the same way
	  /// as a NodeIterator, but their response to document mutation is not
	  /// currently defined.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="whatToShow"> This flag specifies which node types may appear in
	  ///   the logical view of the tree presented by the iterator. See the
	  ///   description of <code>NodeFilter</code> for the set of possible
	  ///   <code>SHOW_</code> values.These flags can be combined using
	  ///   <code>OR</code>. </param>
	  /// <param name="filter"> The <code>NodeFilter</code> to be used with this
	  ///   <code>DTMFilter</code>, or <code>null</code> to indicate no filter. </param>
	  /// <param name="entityReferenceExpansion"> The value of this flag determines
	  ///   whether entity reference nodes are expanded.
	  /// </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public abstract DTMIterator createDTMIterator(int whatToShow, DTMFilter filter, bool entityReferenceExpansion);

	  /// <summary>
	  /// Create a new <code>DTMIterator</code> that holds exactly one node.
	  /// </summary>
	  /// <param name="node"> The node handle that the DTMIterator will iterate to.
	  /// </param>
	  /// <returns> The newly created <code>DTMIterator</code>. </returns>
	  public abstract DTMIterator createDTMIterator(int node);

	  /* Flag indicating whether an incremental transform is desired */
	  public bool m_incremental = false;

	  /*
	   * Flag set by FEATURE_SOURCE_LOCATION.
	   * This feature specifies whether the transformation phase should
	   * keep track of line and column numbers for the input source
	   * document. 
	   */
	  public bool m_source_location = false;

	  /// <summary>
	  /// Get a flag indicating whether an incremental transform is desired </summary>
	  /// <returns> incremental boolean.
	  ///  </returns>
	  public virtual bool Incremental
	  {
		  get
		  {
			return m_incremental;
		  }
		  set
		  {
			m_incremental = value;
		  }
	  }


	  /// <summary>
	  /// Get a flag indicating whether the transformation phase should
	  /// keep track of line and column numbers for the input source
	  /// document. </summary>
	  /// <returns> source location boolean
	  ///  </returns>
	  public virtual bool Source_location
	  {
		  get
		  {
			return m_source_location;
		  }
		  set
		  {
			m_source_location = value;
		  }
	  }



	  // -------------------- private methods --------------------

	   /// <summary>
	   /// Temp debug code - this will be removed after we test everything
	   /// </summary>
	  private static bool debug;

	  static DTMManager()
	  {
		try
		{
		  debug = System.getProperty("dtm.debug") != null;
		}
		catch (SecurityException)
		{
		}
	  }

	  /// <summary>
	  /// This value, set at compile time, controls how many bits of the
	  /// DTM node identifier numbers are used to identify a node within a
	  /// document, and thus sets the maximum number of nodes per
	  /// document. The remaining bits are used to identify the DTM
	  /// document which contains this node.
	  /// 
	  /// If you change IDENT_DTM_NODE_BITS, be sure to rebuild _ALL_ the
	  /// files which use it... including the IDKey testcases.
	  /// 
	  /// (FuncGenerateKey currently uses the node identifier directly and
	  /// thus is affected when this changes. The IDKEY results will still be
	  /// _correct_ (presuming no other breakage), but simple equality
	  /// comparison against the previous "golden" files will probably
	  /// complain.)
	  /// 
	  /// </summary>
	  public const int IDENT_DTM_NODE_BITS = 16;


	  /// <summary>
	  /// When this bitmask is ANDed with a DTM node handle number, the result
	  /// is the low bits of the node's index number within that DTM. To obtain
	  /// the high bits, add the DTM ID portion's offset as assigned in the DTM 
	  /// Manager.
	  /// </summary>
	  public static readonly int IDENT_NODE_DEFAULT = (1 << IDENT_DTM_NODE_BITS) - 1;


	  /// <summary>
	  /// When this bitmask is ANDed with a DTM node handle number, the result
	  /// is the DTM's document identity number.
	  /// </summary>
	  public static readonly int IDENT_DTM_DEFAULT = ~IDENT_NODE_DEFAULT;

	  /// <summary>
	  /// This is the maximum number of DTMs available.  The highest DTM is
	  /// one less than this.
	  /// </summary>
	  public static readonly int IDENT_MAX_DTMS = ((int)((uint)IDENT_DTM_DEFAULT >> IDENT_DTM_NODE_BITS)) + 1;


	  /// <summary>
	  /// %TBD% Doc
	  /// </summary>
	  /// NEEDSDOC <param name="dtm">
	  /// 
	  /// NEEDSDOC ($objectName$) @return </param>
	  public abstract int getDTMIdentity(DTM dtm);

	  /// <summary>
	  /// %TBD% Doc
	  /// 
	  /// NEEDSDOC ($objectName$) @return
	  /// </summary>
	  public virtual int DTMIdentityMask
	  {
		  get
		  {
			return IDENT_DTM_DEFAULT;
		  }
	  }

	  /// <summary>
	  /// %TBD% Doc
	  /// 
	  /// NEEDSDOC ($objectName$) @return
	  /// </summary>
	  public virtual int NodeIdentityMask
	  {
		  get
		  {
			return IDENT_NODE_DEFAULT;
		  }
	  }

	}

}