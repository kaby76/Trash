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
 * $Id: XConnection.java 468638 2006-10-28 06:52:06Z minchau $
 */
namespace org.apache.xalan.lib.sql
{



	using ExpressionContext = org.apache.xalan.extensions.ExpressionContext;
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using DTMManager = org.apache.xml.dtm.DTMManager;
	using DTMManagerDefault = org.apache.xml.dtm.@ref.DTMManagerDefault;
	using DTMNodeIterator = org.apache.xml.dtm.@ref.DTMNodeIterator;
	using DTMNodeProxy = org.apache.xml.dtm.@ref.DTMNodeProxy;
	using XPathContext = org.apache.xpath.XPathContext;
	using XBooleanStatic = org.apache.xpath.objects.XBooleanStatic;
	using XNodeSet = org.apache.xpath.objects.XNodeSet;
	using Element = org.w3c.dom.Element;
	using NamedNodeMap = org.w3c.dom.NamedNodeMap;
	using Node = org.w3c.dom.Node;
	using NodeList = org.w3c.dom.NodeList;

	/// <summary>
	/// An XSLT extension that allows a stylesheet to
	/// access JDBC data.
	/// 
	/// It is accessed by specifying a namespace URI as follows:
	/// <pre>
	///    xmlns:sql="http://xml.apache.org/xalan/sql"
	/// </pre>
	/// 
	/// From the stylesheet perspective,
	/// XConnection provides 3 extension functions: new(),
	/// query(), and close().
	/// Use new() to call one of XConnection constructors, which
	/// establishes a JDBC driver connection to a data source and
	/// returns an XConnection object.
	/// Then use the XConnection object query() method to return a
	/// result set in the form of a row-set element.
	/// When you have finished working with the row-set, call the
	/// XConnection object close() method to terminate the connection.
	/// </summary>
	public class XConnection
	{

	  /// <summary>
	  /// Flag for DEBUG mode
	  /// </summary>
	  private const bool DEBUG = false;

	  /// <summary>
	  /// The Current Connection Pool in Use. An XConnection can only
	  /// represent one query at a time, prior to doing some type of query.
	  /// </summary>
	  private ConnectionPool m_ConnectionPool = null;

	  /// <summary>
	  /// The DBMS Connection used to produce this SQL Document.
	  /// Will be used to clear free up the database resources on
	  /// close.
	  /// </summary>
	  private Connection m_Connection = null;

	  /// <summary>
	  /// If a default Connection Pool is used. i.e. A connection Pool
	  /// that is created internally, then do we actually allow pools
	  /// to be created. Due to the archititure of the Xalan Extensions,
	  /// there is no notification of when the Extension is being unloaded and
	  /// as such, there is a good chance that JDBC COnnections are not closed.
	  /// A finalized is provided to try and catch this situation but since
	  /// support of finalizers is inconsistant across JVM's this may cause
	  /// a problem. The robustness of the JDBC Driver is also at issue here.
	  /// if a controlled shutdown is provided by the driver then default
	  /// conntectiom pools are OK.
	  /// </summary>
	  private bool m_DefaultPoolingEnabled = false;


	  /// <summary>
	  /// As we do queries, we will produce SQL Documents. Any ony may produce
	  /// one or more SQL Documents so that the current connection information
	  /// may be easilly reused. This collection will hold a collection of all
	  /// the documents created. As Documents are closed, they will be removed
	  /// from the collection and told to free all the used resources.
	  /// </summary>
	  private ArrayList m_OpenSQLDocuments = new ArrayList();


	  /// <summary>
	  /// Let's keep a copy of the ConnectionPoolMgr in
	  /// alive here so we are keeping the static pool alive
	  /// We will also use this Pool Manager to register our default pools.
	  /// </summary>
	  private ConnectionPoolManager m_PoolMgr = new ConnectionPoolManager();

	  /// <summary>
	  /// For PreparedStatements, we need a place to
	  /// to store the parameters in a vector.
	  /// </summary>
	  private ArrayList m_ParameterList = new ArrayList();

	  /// <summary>
	  /// Allow the SQL Extensions to return null on error. The Error information will
	  /// be stored in a seperate Error Document that can easily be retrived using the
	  /// getError() method.
	  /// %REVIEW% This functionality will probably be buried inside the SQLDocument.
	  /// </summary>
	  private Exception m_Error = null;

	  /// <summary>
	  /// When the Stylesheet wants to review the errors from a paticular
	  /// document, it asks the XConnection. We need to track what document
	  /// in the list of managed documents caused the last error. As SetError
	  /// is called, it will record the document that had the error.
	  /// </summary>
	  private SQLDocument m_LastSQLDocumentWithError = null;

	  /// <summary>
	  /// If true then full information should be returned about errors and warnings
	  /// in getError(). This includes chained errors and warnings.
	  /// If false (the default) then getError() returns just the first SQLException.
	  /// </summary>
	  private bool m_FullErrors = false;



	  /// <summary>
	  /// One a per XConnection basis there is a master QueryParser that is responsible
	  /// for generating Query Parsers. This will allow us to cache previous instances
	  /// so the inline parser execution time is minimized.
	  /// </summary>
	  private SQLQueryParser m_QueryParser = new SQLQueryParser();

	  private bool m_IsDefaultPool = false;

	  /// <summary>
	  /// This flag will be used to indicate to the SQLDocument to use
	  /// Streaming mode. Streeaming Mode will reduce the memory footprint
	  /// to a fixed amount but will not let you traverse the tree more than
	  /// once since the Row data will be reused for every Row in the Query.
	  /// </summary>
	  private bool m_IsStreamingEnabled = true;

	  /// 
	   private bool m_InlineVariables = false;

	  /// <summary>
	  /// This flag will be used to indicate if multiple result sets are
	  /// supported from the database. If they are, then the metadata element is
	  /// moved to insude the row-set element and multiple row-set elements may
	  /// be included under the sql root element.
	  /// </summary>
	  private bool m_IsMultipleResultsEnabled = false;

	  /// <summary>
	  /// This flag will be used to indicate if database preparedstatements
	  /// should be cached. This also controls if the Java statement object
	  /// should be cached.
	  /// </summary>
	  private bool m_IsStatementCachingEnabled = false;

	  public XConnection()
	  {
	  }

	  /// <summary>
	  /// Constructs a new XConnection and attempts to connect to a datasource as
	  /// defined in the
	  /// <code>connect(ExpressionContext exprContext, String connPoolName)</code>
	  /// method.
	  /// <code>org.apache.xalan.lib.sql.ConnectionPool</code> or a JNDI datasource.
	  /// </summary>
	  /// <param name="exprContext"> Context automatically passed from the XSLT sheet. </param>
	  /// <param name="name"> The name of the ConnectionPool or the JNDI DataSource path.
	  ///  </param>
	  public XConnection(ExpressionContext exprContext, string connPoolName)
	  {
		connect(exprContext, connPoolName);
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> </param>
	  /// <param name="dbURL"> </param>
	  public XConnection(ExpressionContext exprContext, string driver, string dbURL)
	  {
		connect(exprContext, driver, dbURL);
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="list"> </param>
	  public XConnection(ExpressionContext exprContext, NodeList list)
	  {
		connect(exprContext, list);
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> </param>
	  /// <param name="dbURL"> </param>
	  /// <param name="user"> </param>
	  /// <param name="password"> </param>
	  public XConnection(ExpressionContext exprContext, string driver, string dbURL, string user, string password)
	  {
		connect(exprContext, driver, dbURL, user, password);
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> </param>
	  /// <param name="dbURL"> </param>
	  /// <param name="protocolElem"> </param>
	  public XConnection(ExpressionContext exprContext, string driver, string dbURL, Element protocolElem)
	  {
		connect(exprContext, driver, dbURL, protocolElem);
	  }

	  /// <summary>
	  /// Returns an XConnection from either a user created
	  /// <code>org.apache.xalan.lib.sql.ConnectionPool</code> or a JNDI datasource.
	  /// 
	  /// 
	  /// This method first tries to resolve the passed name against
	  /// <code>ConnectionPool</code>s registered with
	  /// <code>ConnectionPoolManager</code>.
	  /// If that fails, it attempts to find the name as a JNDI DataSource path.
	  /// </summary>
	  /// <param name="exprContext"> Context automatically passed from the XSLT sheet. </param>
	  /// <param name="name"> The name of the ConnectionPool or the JNDI DataSource path.
	  ///   </param>
	   public virtual XBooleanStatic connect(ExpressionContext exprContext, string name)
	   {
		 try
		 {
		   m_ConnectionPool = m_PoolMgr.getPool(name);

		   if (m_ConnectionPool == null)
		   {
			 //Try to create a jndi source with the passed name
			 ConnectionPool pool = new JNDIConnectionPool(name);

			 if (pool.testConnection())
			 {

			   //JNDIConnectionPool seems good, so register it with the pool manager.
			   //Once registered, it will show up like other named ConnectionPool's,
			   //so the m_PoolMgr.getPool(name) method (above) will find it.
			   m_PoolMgr.registerPool(name, pool);
			   m_ConnectionPool = pool;

			   m_IsDefaultPool = false;
			   return new XBooleanStatic(true);
			 }
			 else
			 {
			   throw new System.ArgumentException("Invalid ConnectionPool name or JNDI Datasource path: " + name);
			 }
		   }
		   else
		   {
			 m_IsDefaultPool = false;
			 return new XBooleanStatic(true);
		   }
		 }
		 catch (Exception e)
		 {
		   setError(e, exprContext);
		   return new XBooleanStatic(false);
		 }

	   }


	  /// <summary>
	  /// Create an XConnection object with just a driver and database URL. </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> JDBC driver of the form foo.bar.Driver. </param>
	  /// <param name="dbURL"> database URL of the form jdbc:subprotocol:subname.
	  ///  </param>
	  public virtual XBooleanStatic connect(ExpressionContext exprContext, string driver, string dbURL)
	  {
		try
		{
		  init(driver, dbURL, new Properties());
		  return new XBooleanStatic(true);
		}
		catch (SQLException e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
		catch (Exception e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="protocolElem">
	  ///  </param>
	  public virtual XBooleanStatic connect(ExpressionContext exprContext, Element protocolElem)
	  {
		try
		{
		  initFromElement(protocolElem);
		  return new XBooleanStatic(true);
		}
		catch (SQLException e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
		catch (Exception e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="list">
	  ///  </param>
	  public virtual XBooleanStatic connect(ExpressionContext exprContext, NodeList list)
	  {
		try
		{
		  initFromElement((Element) list.item(0));
		  return new XBooleanStatic(true);
		}
		catch (SQLException e)
		{
		  setError(e, exprContext);
		  return new XBooleanStatic(false);
		}
		catch (Exception e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
	  }

	  /// <summary>
	  /// Create an XConnection object with user ID and password. </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> JDBC driver of the form foo.bar.Driver. </param>
	  /// <param name="dbURL"> database URL of the form jdbc:subprotocol:subname. </param>
	  /// <param name="user"> user ID. </param>
	  /// <param name="password"> connection password.
	  ///  </param>
	  public virtual XBooleanStatic connect(ExpressionContext exprContext, string driver, string dbURL, string user, string password)
	  {
		try
		{
		  Properties prop = new Properties();
		  prop.put("user", user);
		  prop.put("password", password);

		  init(driver, dbURL, prop);

		  return new XBooleanStatic(true);
		}
		catch (SQLException e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
		catch (Exception e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
	  }


	  /// <summary>
	  /// Create an XConnection object with a connection protocol </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="driver"> JDBC driver of the form foo.bar.Driver. </param>
	  /// <param name="dbURL"> database URL of the form jdbc:subprotocol:subname. </param>
	  /// <param name="protocolElem"> list of string tag/value connection arguments,
	  /// normally including at least "user" and "password".
	  ///  </param>
	  public virtual XBooleanStatic connect(ExpressionContext exprContext, string driver, string dbURL, Element protocolElem)
	  {
		try
		{
		  Properties prop = new Properties();

		  NamedNodeMap atts = protocolElem.Attributes;

		  for (int i = 0; i < atts.Length; i++)
		  {
			prop.put(atts.item(i).NodeName, atts.item(i).NodeValue);
		  }

		  init(driver, dbURL, prop);

		  return new XBooleanStatic(true);
		}
		catch (SQLException e)
		{
		  setError(e,exprContext);
		  return new XBooleanStatic(false);
		}
		catch (Exception e)
		{
		  setError(e, exprContext);
		  return new XBooleanStatic(false);
		}
	  }


	  /// <summary>
	  /// Allow the database connection information to be sepcified in
	  /// the XML tree. The connection information could also be
	  /// externally originated and passed in as an XSL Parameter.
	  /// The required XML Format is as follows.
	  /// A document fragment is needed to specify the connection information
	  /// the top tag name is not specific for this code, we are only interested
	  /// in the tags inside.
	  /// <DBINFO-TAG>
	  /// Specify the driver name for this connection pool
	  /// <dbdriver>drivername</dbdriver>
	  /// Specify the URL for the driver in this connection pool
	  /// <dburl>url</dburl>
	  /// Specify the password for this connection pool
	  /// <password>password</password>
	  /// Specify the username for this connection pool
	  /// <user>username</user>
	  /// You can add extra protocol items including the User Name & Password
	  /// with the protocol tag. For each extra protocol item, add a new element
	  /// where the name of the item is specified as the name attribute and
	  /// and its value as the elements value.
	  /// <protocol name="name of value">value</protocol>
	  /// </DBINFO-TAG> </summary>
	  /// <param name="e">
	  /// </param>
	  /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void initFromElement(org.w3c.dom.Element e)throws java.sql.SQLException
	  private void initFromElement(Element e)
	  {

		Properties prop = new Properties();
		string driver = "";
		string dbURL = "";
		Node n = e.FirstChild;

		if (null == n)
		{
			return; // really need to throw an error
		}

		do
		{
		  string nName = n.NodeName;

		  if (nName.Equals("dbdriver", StringComparison.CurrentCultureIgnoreCase))
		  {
			driver = "";
			Node n1 = n.FirstChild;
			if (null != n1)
			{
			  driver = n1.NodeValue;
			}
		  }

		  if (nName.Equals("dburl", StringComparison.CurrentCultureIgnoreCase))
		  {
			dbURL = "";
			Node n1 = n.FirstChild;
			if (null != n1)
			{
			  dbURL = n1.NodeValue;
			}
		  }

		  if (nName.Equals("password", StringComparison.CurrentCultureIgnoreCase))
		  {
			string s = "";
			Node n1 = n.FirstChild;
			if (null != n1)
			{
			  s = n1.NodeValue;
			}
			prop.put("password", s);
		  }

		  if (nName.Equals("user", StringComparison.CurrentCultureIgnoreCase))
		  {
			string s = "";
			Node n1 = n.FirstChild;
			if (null != n1)
			{
			  s = n1.NodeValue;
			}
			prop.put("user", s);
		  }

		  if (nName.Equals("protocol", StringComparison.CurrentCultureIgnoreCase))
		  {
			string Name = "";

			NamedNodeMap attrs = n.Attributes;
			Node n1 = attrs.getNamedItem("name");
			if (null != n1)
			{
			  string s = "";
			  Name = n1.NodeValue;

			  Node n2 = n.FirstChild;
			  if (null != n2)
			  {
				  s = n2.NodeValue;
			  }

			  prop.put(Name, s);
			}
		  }

		} while ((n = n.NextSibling) != null);

		init(driver, dbURL, prop);
	  }



	  /// <summary>
	  /// Initilize is being called because we did not have an
	  /// existing Connection Pool, so let's see if we created one
	  /// already or lets create one ourselves. </summary>
	  /// <param name="driver"> </param>
	  /// <param name="dbURL"> </param>
	  /// <param name="prop">
	  /// </param>
	  /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void init(String driver, String dbURL, java.util.Properties prop)throws java.sql.SQLException
	  private void init(string driver, string dbURL, Properties prop)
	  {
		Connection con = null;

		if (DEBUG)
		{
		  Console.WriteLine("XConnection, Connection Init");
		}

		string user = prop.getProperty("user");
		if (string.ReferenceEquals(user, null))
		{
			user = "";
		}

		string passwd = prop.getProperty("password");
		if (string.ReferenceEquals(passwd, null))
		{
			passwd = "";
		}


		string poolName = driver + dbURL + user + passwd;
		ConnectionPool cpool = m_PoolMgr.getPool(poolName);

		if (cpool == null)
		{

		  if (DEBUG)
		  {
			Console.WriteLine("XConnection, Creating Connection");
			Console.WriteLine(" Driver  :" + driver);
			Console.WriteLine(" URL     :" + dbURL);
			Console.WriteLine(" user    :" + user);
			Console.WriteLine(" passwd  :" + passwd);
		  }


		  DefaultConnectionPool defpool = new DefaultConnectionPool();

		  if ((DEBUG) && (defpool == null))
		  {
			Console.WriteLine("Failed to Create a Default Connection Pool");
		  }

		  defpool.Driver = driver;
		  defpool.URL = dbURL;
		  defpool.Protocol = prop;

		  // Only enable pooling in the default pool if we are explicatly
		  // told too.
		  if (m_DefaultPoolingEnabled)
		  {
			  defpool.PoolEnabled = true;
		  }

		  m_PoolMgr.registerPool(poolName, defpool);
		  m_ConnectionPool = defpool;
		}
		else
		{
		  m_ConnectionPool = cpool;
		}

		m_IsDefaultPool = true;

		//
		// Let's test to see if we really can connect
		// Just remember to give it back after the test.
		//
		try
		{
		  con = m_ConnectionPool.Connection;
		}
		catch (SQLException e)
		{
		  if (con != null)
		  {
			m_ConnectionPool.releaseConnectionOnError(con);
			con = null;
		  }
		  throw e;
		}
		finally
		{
		  if (con != null)
		  {
			  m_ConnectionPool.releaseConnection(con);
		  }
		}
	  }

	  /// <summary>
	  /// Allow the SQL Document to retrive a connection to be used
	  /// to build the SQL Statement.
	  /// </summary>
	  public virtual ConnectionPool ConnectionPool
	  {
		  get
		  {
			return m_ConnectionPool;
		  }
	  }


	  /// <summary>
	  /// Execute a query statement by instantiating an </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="queryString"> the SQL query. </param>
	  /// <returns> XStatement implements NodeIterator. </returns>
	  /// <exception cref="SQLException">
	  /// @link org.apache.xalan.lib.sql.XStatement XStatement}
	  /// object. The XStatement executes the query, and uses the result set
	  /// to create a
	  /// @link org.apache.xalan.lib.sql.RowSet RowSet},
	  /// a row-set element. </exception>
	  public virtual DTM query(ExpressionContext exprContext, string queryString)
	  {
		SQLDocument doc = null;

		try
		{
		  if (DEBUG)
		  {
			  Console.WriteLine("pquery()");
		  }

		  // Build an Error Document, NOT Connected.
		  if (null == m_ConnectionPool)
		  {
			  return null;
		  }

		  SQLQueryParser query = m_QueryParser.parse(this, queryString, SQLQueryParser.NO_INLINE_PARSER);

		  doc = SQLDocument.getNewDocument(exprContext);
		  doc.execute(this, query);

		  // also keep a local reference
		  m_OpenSQLDocuments.Add(doc);
		}
		catch (Exception e)
		{
		  // OK We had an error building the document, let try and grab the
		  // error information and clean up our connections.

		  if (DEBUG)
		  {
			  Console.WriteLine("exception in query()");
		  }

		  if (doc != null)
		  {
			if (doc.hasErrors())
			{
			  setError(e, doc, doc.checkWarnings());
			}

			doc.close(m_IsDefaultPool);
			doc = null;
		  }
		}
		finally
		{
		  if (DEBUG)
		  {
			  Console.WriteLine("leaving query()");
		  }
		}

		// Doc will be null if there was an error
		return doc;
	  }

	  /// <summary>
	  /// Execute a parameterized query statement by instantiating an </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="queryString"> the SQL query. </param>
	  /// <returns> XStatement implements NodeIterator. </returns>
	  /// <exception cref="SQLException">
	  /// @link org.apache.xalan.lib.sql.XStatement XStatement}
	  /// object. The XStatement executes the query, and uses the result set
	  /// to create a
	  /// @link org.apache.xalan.lib.sql.RowSet RowSet},
	  /// a row-set element. </exception>
	  public virtual DTM pquery(ExpressionContext exprContext, string queryString)
	  {
		return (pquery(exprContext, queryString, null));
	  }

	  /// <summary>
	  /// Execute a parameterized query statement by instantiating an </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="queryString"> the SQL query. </param>
	  /// <param name="typeInfo"> </param>
	  /// <returns> XStatement implements NodeIterator. </returns>
	  /// <exception cref="SQLException">
	  /// @link org.apache.xalan.lib.sql.XStatement XStatement}
	  /// object. The XStatement executes the query, and uses the result set
	  /// to create a
	  /// @link org.apache.xalan.lib.sql.RowSet RowSet},
	  /// a row-set element.
	  /// This method allows for the user to pass in a comma seperated
	  /// String that represents a list of parameter types. If supplied
	  /// the parameter types will be used to overload the current types
	  /// in the current parameter list. </exception>
	  public virtual DTM pquery(ExpressionContext exprContext, string queryString, string typeInfo)
	  {
		SQLDocument doc = null;

		try
		{
		  if (DEBUG)
		  {
			  Console.WriteLine("pquery()");
		  }

		  // Build an Error Document, NOT Connected.
		  if (null == m_ConnectionPool)
		  {
			  return null;
		  }

		  SQLQueryParser query = m_QueryParser.parse(this, queryString, SQLQueryParser.NO_OVERRIDE);

		  // If we are not using the inline parser, then let add the data
		  // to the parser so it can populate the SQL Statement.
		  if (!m_InlineVariables)
		  {
			addTypeToData(typeInfo);
			query.Parameters = m_ParameterList;
		  }

		  doc = SQLDocument.getNewDocument(exprContext);
		  doc.execute(this, query);

		  // also keep a local reference
		  m_OpenSQLDocuments.Add(doc);
		}
		catch (Exception e)
		{
		  // OK We had an error building the document, let try and grab the
		  // error information and clean up our connections.

		  if (DEBUG)
		  {
			  Console.WriteLine("exception in query()");
		  }

		  if (doc != null)
		  {
			if (doc.hasErrors())
			{
			  setError(e, doc, doc.checkWarnings());
			}

			// If we are using the Default Connection Pool, then
			// force the connection pool to flush unused connections.
			doc.close(m_IsDefaultPool);
			doc = null;
		  }
		}
		finally
		{
		  if (DEBUG)
		  {
			  Console.WriteLine("leaving query()");
		  }
		}

		// Doc will be null if there was an error
		return doc;
	  }

	  /// <summary>
	  /// The purpose of this routine is to force the DB cursor to skip forward
	  /// N records. You should call this function after [p]query to help with
	  /// pagination. i.e. Perfrom your select, then skip forward past the records
	  /// you read previously.
	  /// </summary>
	  /// <param name="exprContext"> </param>
	  /// <param name="o"> </param>
	  /// <param name="value"> </param>
	  public virtual void skipRec(ExpressionContext exprContext, object o, int value)
	  {
		SQLDocument sqldoc = null;
		DTMNodeIterator nodei = null;

		sqldoc = locateSQLDocument(exprContext, o);
		if (sqldoc != null)
		{
			sqldoc.skip(value);
		}
	  }



	  private void addTypeToData(string typeInfo)
	  {
		  int indx;

		  if (!string.ReferenceEquals(typeInfo, null) && m_ParameterList != null)
		  {
			  // Parse up the parameter types that were defined
			  // with the query
			  StringTokenizer plist = new StringTokenizer(typeInfo);

			  // Override the existing type that is stored in the
			  // parameter list. If there are more types than parameters
			  // ignore for now, a more meaningfull error should occur
			  // when the actual query is executed.
			  indx = 0;
			  while (plist.hasMoreTokens())
			  {
				string value = plist.nextToken();
				QueryParameter qp = (QueryParameter) m_ParameterList[indx];
				if (null != qp)
				{
				  qp.TypeName = value;
				}

				indx++;
			  }
		  }
	  }

	  /// <summary>
	  /// Add an untyped value to the parameter list. </summary>
	  /// <param name="value">
	  ///  </param>
	  public virtual void addParameter(string value)
	  {
		addParameterWithType(value, null);
	  }

	  /// <summary>
	  /// Add a typed parameter to the parameter list. </summary>
	  /// <param name="value"> </param>
	  /// <param name="Type">
	  ///  </param>
	  public virtual void addParameterWithType(string value, string Type)
	  {
		m_ParameterList.Add(new QueryParameter(value, Type));
	  }


	  /// <summary>
	  /// Add a single parameter to the parameter list
	  /// formatted as an Element </summary>
	  /// <param name="e">
	  ///  </param>
	  public virtual void addParameterFromElement(Element e)
	  {
		NamedNodeMap attrs = e.Attributes;
		Node Type = attrs.getNamedItem("type");
		Node n1 = e.FirstChild;
		if (null != n1)
		{
		  string value = n1.NodeValue;
		  if (string.ReferenceEquals(value, null))
		  {
			  value = "";
		  }
		  m_ParameterList.Add(new QueryParameter(value, Type.NodeValue));
		}
	  }


	  /// <summary>
	  /// Add a section of parameters to the Parameter List
	  /// Do each element from the list </summary>
	  /// <param name="nl">
	  ///  </param>
	  public virtual void addParameterFromElement(NodeList nl)
	  {
		//
		// Each child of the NodeList represents a node
		// match from the select= statment. Process each
		// of them as a seperate list.
		// The XML Format is as follows
		//
		// <START-TAG>
		//   <TAG1 type="int">value</TAG1>
		//   <TAGA type="int">value</TAGA>
		//   <TAG2 type="string">value</TAG2>
		// </START-TAG>
		//
		// The XSL to process this is formatted as follows
		// <xsl:param name="plist" select="//START-TAG" />
		// <sql:addParameter( $plist );
		//
		int count = nl.Length;
		for (int x = 0; x < count; x++)
		{
		  addParameters((Element) nl.item(x));
		}
	  }

	  /// <param name="elem">
	  ///  </param>
	  private void addParameters(Element elem)
	  {
		//
		// Process all of the Child Elements
		// The format is as follows
		//
		//<TAG type ="typeid">value</TAG>
		//<TAG1 type ="typeid">value</TAG1>
		//<TAGA type ="typeid">value</TAGA>
		//
		// The name of the Node is not important just is value
		// and if it contains a type attribute

		Node n = elem.FirstChild;

		if (null == n)
		{
			return;
		}

		do
		{
		  if (n.NodeType == Node.ELEMENT_NODE)
		  {
			NamedNodeMap attrs = n.Attributes;
			Node Type = attrs.getNamedItem("type");
			string TypeStr;

			if (Type == null)
			{
				TypeStr = "string";
			}
			else
			{
				TypeStr = Type.NodeValue;
			}

			Node n1 = n.FirstChild;
			if (null != n1)
			{
			  string value = n1.NodeValue;
			  if (string.ReferenceEquals(value, null))
			  {
				  value = "";
			  }


			  m_ParameterList.Add(new QueryParameter(value, TypeStr));
			}
		  }
		} while ((n = n.NextSibling) != null);
	  }

	  /// 
	  public virtual void clearParameters()
	  {
		m_ParameterList.Clear();
	  }

	  /// <summary>
	  /// There is a problem with some JDBC drivers when a Connection
	  /// is open and the JVM shutsdown. If there is a problem, there
	  /// is no way to control the currently open connections in the
	  /// pool. So for the default connection pool, the actuall pooling
	  /// mechinsm is disabled by default. The Stylesheet designer can
	  /// re-enabled pooling to take advantage of connection pools.
	  /// The connection pool can even be disabled which will close all
	  /// outstanding connections.
	  /// </summary>
	  /// @deprecated Use setFeature("default-pool-enabled", "true"); 
	  public virtual void enableDefaultConnectionPool()
	  {

		if (DEBUG)
		{
		  Console.WriteLine("Enabling Default Connection Pool");
		}

		m_DefaultPoolingEnabled = true;

		if (m_ConnectionPool == null)
		{
			return;
		}
		if (m_IsDefaultPool)
		{
			return;
		}

		m_ConnectionPool.PoolEnabled = true;

	  }

	  /// <summary>
	  /// See enableDefaultConnectionPool
	  /// </summary>
	  /// @deprecated Use setFeature("default-pool-enabled", "false"); 
	  public virtual void disableDefaultConnectionPool()
	  {
		if (DEBUG)
		{
		  Console.WriteLine("Disabling Default Connection Pool");
		}

		m_DefaultPoolingEnabled = false;

		if (m_ConnectionPool == null)
		{
			return;
		}
		if (!m_IsDefaultPool)
		{
			return;
		}

		m_ConnectionPool.PoolEnabled = false;
	  }


	  /// <summary>
	  /// Control how the SQL Document uses memory. In Streaming Mode,
	  /// memory consumption is greatly reduces so you can have queries
	  /// of unlimited size but it will not let you traverse the data
	  /// more than once.
	  /// </summary>
	  /// @deprecated Use setFeature("streaming", "true"); 
	  public virtual void enableStreamingMode()
	  {

		if (DEBUG)
		{
		  Console.WriteLine("Enabling Streaming Mode");
		}

		m_IsStreamingEnabled = true;
	  }

	  /// <summary>
	  /// Control how the SQL Document uses memory. In Streaming Mode,
	  /// memory consumption is greatly reduces so you can have queries
	  /// of unlimited size but it will not let you traverse the data
	  /// more than once.
	  /// </summary>
	  /// @deprecated Use setFeature("streaming", "false"); 
	  public virtual void disableStreamingMode()
	  {

		if (DEBUG)
		{
		  Console.WriteLine("Disable Streaming Mode");
		}

		m_IsStreamingEnabled = false;
	  }

	  /// <summary>
	  /// Provide access to the last error that occued. This error
	  /// may be over written when the next operation occurs.
	  /// 
	  /// </summary>
	  public virtual DTM Error
	  {
		  get
		  {
			if (m_FullErrors)
			{
			  for (int idx = 0 ; idx < m_OpenSQLDocuments.Count ; idx++)
			  {
				SQLDocument doc = (SQLDocument)m_OpenSQLDocuments[idx];
				SQLWarning warn = doc.checkWarnings();
				if (warn != null)
				{
					setError(null, doc, warn);
				}
			  }
			}
    
			return (buildErrorDocument());
		  }
	  }

	  /// <summary>
	  /// Close the connection to the data source.
	  /// </summary>
	  /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close()throws java.sql.SQLException
	  public virtual void close()
	  {
		if (DEBUG)
		{
		  Console.WriteLine("Entering XConnection.close()");
		}

		//
		// This function is included just for Legacy support
		// If it is really called then we must me using a single
		// document interface, so close all open documents.

		while (m_OpenSQLDocuments.Count != 0)
		{
		  SQLDocument d = (SQLDocument) m_OpenSQLDocuments[0];
		  try
		  {
			// If we are using the Default Connection Pool, then
			// force the connection pool to flush unused connections.
			d.close(m_IsDefaultPool);
		  }
		  catch (Exception)
		  {
		  }

		  m_OpenSQLDocuments.RemoveAt(0);
		}

		if (null != m_Connection)
		{
		  m_ConnectionPool.releaseConnection(m_Connection);
		  m_Connection = null;
		}

		if (DEBUG)
		{
		  Console.WriteLine("Exiting XConnection.close");
		}
	  }

	  /// <summary>
	  /// Close the connection to the data source. Only close the connections
	  /// for a single document.
	  /// </summary>
	  /// <exception cref="SQLException"> </exception>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close(org.apache.xalan.extensions.ExpressionContext exprContext, Object doc) throws java.sql.SQLException
	  public virtual void close(ExpressionContext exprContext, object doc)
	  {
		if (DEBUG)
		{
			Console.WriteLine("Entering XConnection.close(" + doc + ")");
		}

		SQLDocument sqlDoc = locateSQLDocument(exprContext, doc);
		if (sqlDoc != null)
		{
		  // If we are using the Default Connection Pool, then
		  // force the connection pool to flush unused connections.
		  sqlDoc.close(m_IsDefaultPool);
		  m_OpenSQLDocuments.Remove(sqlDoc);
		}
	  }


	  /// <summary>
	  /// When an SQL Document is returned as a DTM object, the XSL variable is actually 
	  /// assigned as a DTMIterator. This is a helper function that will allow you to get
	  /// a reference to the original SQLDocument from the iterator.
	  /// 
	  /// Original code submitted by 
	  ///  Moraine Didier mailto://didier.moraine@winterthur.be </summary>
	  /// <param name="doc">
	  /// @return </param>
	  private SQLDocument locateSQLDocument(ExpressionContext exprContext, object doc)
	  {
		try
		{
		  if (doc is DTMNodeIterator)
		  {
			DTMNodeIterator dtmIter = (DTMNodeIterator)doc;
			try
			{
			  DTMNodeProxy root = (DTMNodeProxy)dtmIter.Root;
			  return (SQLDocument) root.DTM;
			}
			catch (Exception)
			{
			  XNodeSet xNS = (XNodeSet)dtmIter.DTMIterator;
			  DTMIterator iter = (DTMIterator)xNS.ContainedIter;
			  DTM dtm = iter.getDTM(xNS.nextNode());
			  return (SQLDocument)dtm;
			}
		  }

	/*
	      XNodeSet xNS = (XNodeSet)dtmIter.getDTMIterator();
	      OneStepIterator iter = (OneStepIterator)xNS.getContainedIter();
	      DTMManager aDTMManager = (DTMManager)iter.getDTMManager();
	      return (SQLDocument)aDTMManager.getDTM(xNS.nextNode());
	*/
		  setError(new Exception("SQL Extension:close - Can Not Identify SQLDocument"), exprContext);
		  return null;
		}
		catch (Exception e)
		{
		  setError(e, exprContext);
		  return null;
		}
	  }

	  /// <param name="exprContext"> </param>
	  /// <param name="excp">
	  ///  </param>
	  private SQLErrorDocument buildErrorDocument()
	  {
		SQLErrorDocument eDoc = null;

		if (m_LastSQLDocumentWithError != null)
		{
		  // @todo
		  // Do we need to do something with this ??
		  //    m_Error != null || (m_FullErrors && m_Warning != null) )

		  ExpressionContext ctx = m_LastSQLDocumentWithError.ExpressionContext;
		  SQLWarning warn = m_LastSQLDocumentWithError.checkWarnings();


		  try
		  {
			DTMManager mgr = ((XPathContext.XPathExpressionContext)ctx).DTMManager;
			DTMManagerDefault mgrDefault = (DTMManagerDefault) mgr;
			int dtmIdent = mgrDefault.FirstFreeDTMID;

			eDoc = new SQLErrorDocument(mgr, dtmIdent << DTMManager.IDENT_DTM_NODE_BITS, m_Error, warn, m_FullErrors);

			// Register our document
			mgrDefault.addDTM(eDoc, dtmIdent);

			// Clear the error and warning.
			m_Error = null;
			m_LastSQLDocumentWithError = null;
		  }
		  catch (Exception)
		  {
			eDoc = null;
		  }
		}

		return (eDoc);
	  }


	  /// <summary>
	  /// This is an internal version of Set Error that is called withen
	  /// XConnection where there is no SQLDocument created yet. As in the
	  /// Connect statement or creation of the ConnectionPool.
	  /// </summary>
	  public virtual void setError(Exception excp, ExpressionContext expr)
	  {
		try
		{
		  ErrorListener listen = expr.ErrorListener;
		  if (listen != null && excp != null)
		  {

			listen.warning(new TransformerException(excp.ToString(), expr.XPathContext.SAXLocator, excp));
		  }
		}
		catch (Exception)
		{
		}
	  }

	  /// <summary>
	  /// Set an error and/or warning on this connection.
	  /// 
	  /// </summary>
	  public virtual void setError(Exception excp, SQLDocument doc, SQLWarning warn)
	  {

		ExpressionContext cont = doc.ExpressionContext;
		m_LastSQLDocumentWithError = doc;

		try
		{
		  ErrorListener listen = cont.ErrorListener;
		  if (listen != null && excp != null)
		  {
		  listen.warning(new TransformerException(excp.ToString(), cont.XPathContext.SAXLocator, excp));
		  }

		  if (listen != null && warn != null)
		  {
			listen.warning(new TransformerException(warn.ToString(), cont.XPathContext.SAXLocator, warn));
		  }

		  // Assume there will be just one error, but perhaps multiple warnings.
		  if (excp != null)
		  {
			  m_Error = excp;
		  }

		  if (warn != null)
		  {
			// Because the log may not have processed the previous warning yet
			// we need to make a new one.
			SQLWarning tw = new SQLWarning(warn.Message, warn.SQLState, warn.ErrorCode);
			SQLWarning nw = warn.NextWarning;
			while (nw != null)
			{
			  tw.NextWarning = new SQLWarning(nw.Message, nw.SQLState, nw.ErrorCode);

			  nw = nw.NextWarning;
			}

			tw.NextWarning = new SQLWarning(warn.Message, warn.SQLState, warn.ErrorCode);

	//        m_Warning = tw;

		  }
		}
		catch (Exception)
		{
		  //m_Error = null;
		}
	  }

	  /// <summary>
	  /// Set feature options for this XConnection. </summary>
	  /// <param name="feature"> The name of the feature being set, currently supports (streaming, inline-variables, multiple-results, cache-statements, default-pool-enabled). </param>
	  /// <param name="setting"> The new setting for the specified feature, currently "true" is true and anything else is false.
	  ///  </param>
	  public virtual void setFeature(string feature, string setting)
	  {
		bool value = false;

		if ("true".Equals(setting, StringComparison.CurrentCultureIgnoreCase))
		{
			value = true;
		}

		if ("streaming".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_IsStreamingEnabled = value;
		}
		else if ("inline-variables".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_InlineVariables = value;
		}
		else if ("multiple-results".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_IsMultipleResultsEnabled = value;
		}
		else if ("cache-statements".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_IsStatementCachingEnabled = value;
		}
		else if ("default-pool-enabled".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_DefaultPoolingEnabled = value;

		  if (m_ConnectionPool == null)
		  {
			  return;
		  }
		  if (m_IsDefaultPool)
		  {
			  return;
		  }

		  m_ConnectionPool.PoolEnabled = value;
		}
		else if ("full-errors".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  m_FullErrors = value;
		}
	  }

	  /// <summary>
	  /// Get feature options for this XConnection. </summary>
	  /// <param name="feature"> The name of the feature to get the setting for. </param>
	  /// <returns> The setting of the specified feature. Will be "true" or "false" (null if the feature is not known) </returns>
	  public virtual string getFeature(string feature)
	  {
		string value = null;

		if ("streaming".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_IsStreamingEnabled ? "true" : "false";
		}
		else if ("inline-variables".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_InlineVariables ? "true" : "false";
		}
		else if ("multiple-results".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_IsMultipleResultsEnabled ? "true" : "false";
		}
		else if ("cache-statements".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_IsStatementCachingEnabled ? "true" : "false";
		}
		else if ("default-pool-enabled".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_DefaultPoolingEnabled ? "true" : "false";
		}
		else if ("full-errors".Equals(feature, StringComparison.CurrentCultureIgnoreCase))
		{
		  value = m_FullErrors ? "true" : "false";
		}

		return (value);
	  }



	  /// 
	  ~XConnection()
	  {
		if (DEBUG)
		{
			Console.WriteLine("In XConnection, finalize");
		}
		try
		{
		  close();
		}
		catch (Exception)
		{
		  // Empty We are final Anyway
		}
	  }

	}

}