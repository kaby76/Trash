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
namespace org.apache.xalan.lib.sql
{



	/// <summary>
	/// A Connection Pool that wraps a JDBC datasource to provide connections.
	/// 
	/// An instance of this class is created by <code>XConnection</code> when it
	/// attempts to resolves a <code>ConnectionPool</code> name as a JNDI data source.
	/// 
	/// Most methods in this implementation do nothing since configuration is handled
	/// by the underlying JDBC datasource.  Users should always call
	/// <code>XConnection.close()</code> from their stylsheet to explicitely close
	/// their connection.  However, since there is no way to enforce this
	/// (Yikes!), it is recommended that a relatively short datasource timeout
	/// be used to prevent dangling connections.
	/// </summary>
	public class JNDIConnectionPool : ConnectionPool
	{

	  /// <summary>
	  /// Reference to the datasource
	  /// </summary>
	  protected internal object jdbcSource = null;

	  /// <summary>
	  /// To maintain Java 1.3 compatibility, we need to work with the
	  /// DataSource class through Reflection. The getConnection method
	  /// is one of the methods used, and there are two different flavors.
	  /// 
	  /// </summary>
	  private System.Reflection.MethodInfo getConnectionWithArgs = null;
	  private System.Reflection.MethodInfo getConnection = null;


	  /// <summary>
	  /// The unique jndi path for this datasource.
	  /// </summary>
	  protected internal string jndiPath = null;

	  /// <summary>
	  /// User name for protected datasources.
	  /// </summary>
	  protected internal string user = null;

	  /// <summary>
	  /// Password for protected datasources.
	  /// </summary>
	  protected internal string pwd = null;

	  /// <summary>
	  /// Use of the default constructor requires the jndi path to be set via
	  /// setJndiPath().
	  /// </summary>
	  public JNDIConnectionPool()
	  {
	  }

	  /// <summary>
	  /// Creates a connection pool with a specified JNDI path. </summary>
	  /// <param name="jndiDatasourcePath"> Complete path to the JNDI datasource </param>
	  public JNDIConnectionPool(string jndiDatasourcePath)
	  {
		jndiPath = jndiDatasourcePath.Trim();
	  }

	  /// <summary>
	  /// Sets the path for the jndi datasource </summary>
	  /// <param name="jndiPath">  </param>
	  public virtual string JndiPath
	  {
		  set
		  {
			this.jndiPath = value;
		  }
		  get
		  {
			return jndiPath;
		  }
	  }


	  /// <summary>
	  /// Always returns true.
	  /// This method was intended to indicate if the pool was enabled, however, in
	  /// this implementation that is not relavant. </summary>
	  /// <returns>  </returns>
	  public virtual bool Enabled
	  {
		  get
		  {
			return true;
		  }
	  }

	  /// <summary>
	  /// Not implemented and will throw an Error if called.
	  /// 
	  /// Connection configuration is handled by the underlying JNDI DataSource. </summary>
	  /// <param name="d">  </param>
	  public virtual string Driver
	  {
		  set
		  {
			throw new Exception("This method is not supported. " + "All connection information is handled by the JDBC datasource provider");
		  }
	  }

	  /// <summary>
	  /// Not implemented and will throw an Error if called.
	  /// 
	  /// Connection configuration is handled by the underlying JNDI DataSource. </summary>
	  /// <param name="d">  </param>
	  public virtual string URL
	  {
		  set
		  {
			throw new Exception("This method is not supported. " + "All connection information is handled by the JDBC datasource provider");
		  }
	  }

	  /// <summary>
	  /// Intended to release unused connections from the pool.
	  /// Does nothing in this implementation.
	  /// </summary>
	  public virtual void freeUnused()
	  {
		//Do nothing - not an error to call this method
	  }

	  /// <summary>
	  /// Always returns false, indicating that this wrapper has no idea of what
	  /// connections the underlying JNDI source is maintaining. </summary>
	  /// <returns>  </returns>
	  public virtual bool hasActiveConnections()
	  {
		return false;
	  }

	  /// <summary>
	  /// Sets the password for the connection.
	  /// If the jndi datasource does not require a password (which is typical),
	  /// this can be left null. </summary>
	  /// <param name="p"> the password </param>
	  public virtual string Password
	  {
		  set
		  {
    
			if (!string.ReferenceEquals(value, null))
			{
				value = value.Trim();
			}
			if (!string.ReferenceEquals(value, null) && value.Length == 0)
			{
				value = null;
			}
    
			pwd = value;
		  }
	  }

	  /// <summary>
	  /// Sets the user name for the connection.
	  /// If the jndi datasource does not require a user name (which is typical),
	  /// this can be left null. </summary>
	  /// <param name="u"> the user name </param>
	  public virtual string User
	  {
		  set
		  {
    
			if (!string.ReferenceEquals(value, null))
			{
				value = value.Trim();
			}
			if (!string.ReferenceEquals(value, null) && value.Length == 0)
			{
				value = null;
			}
    
			user = value;
		  }
	  }

	  /// <summary>
	  /// Returns a connection from the JDNI DataSource found at the JNDI Datasource
	  /// path.
	  /// </summary>
	  /// <returns> </returns>
	  /// <exception cref="SQLException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.sql.Connection getConnection() throws java.sql.SQLException
	  public virtual Connection Connection
	  {
		  get
		  {
			if (jdbcSource == null)
			{
			  try
			  {
				findDatasource();
			  }
			  catch (NamingException ne)
			  {
				throw new SQLException("Could not create jndi context for " + jndiPath + " - " + ne.getLocalizedMessage());
			  }
			}
    
			try
			{
			  if (!string.ReferenceEquals(user, null) || !string.ReferenceEquals(pwd, null))
			  {
				object[] arglist = new object[] {user, pwd};
				return (Connection) getConnectionWithArgs.invoke(jdbcSource, arglist);
			  }
			  else
			  {
				object[] arglist = new object[] {};
				return (Connection) getConnection.invoke(jdbcSource, arglist);
			  }
			}
			catch (Exception e)
			{
			  throw new SQLException("Could not create jndi connection for " + jndiPath + " - " + e.getLocalizedMessage());
			}
    
		  }
	  }

	  /// <summary>
	  /// Internal method used to look up the datasource. </summary>
	  /// <exception cref="NamingException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected void findDatasource() throws javax.naming.NamingException
	  protected internal virtual void findDatasource()
	  {
		try
		{
		  InitialContext context = new InitialContext();
		  jdbcSource = context.lookup(jndiPath);

		  Type[] withArgs = new Type[] {typeof(string), typeof(string)};
		  getConnectionWithArgs = jdbcSource.GetType().getDeclaredMethod("getConnection", withArgs);

		  Type[] noArgs = new Type[] {};
		  getConnection = jdbcSource.GetType().getDeclaredMethod("getConnection", noArgs);

		}
		catch (NamingException e)
		{
		  throw e;
		}
		catch (NoSuchMethodException e)
		{
		  // For simpleification, we will just throw a NamingException. We will only
		  // use the message part of the exception anyway.
		  throw new NamingException("Unable to resolve JNDI DataSource - " + e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void releaseConnection(java.sql.Connection con) throws java.sql.SQLException
	  public virtual void releaseConnection(Connection con)
	  {
		con.close();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void releaseConnectionOnError(java.sql.Connection con) throws java.sql.SQLException
	  public virtual void releaseConnectionOnError(Connection con)
	  {
		con.close();
	  }

	  /// <summary>
	  /// Releases the reference to the jndi datasource.
	  /// The original intention of this method was to actually turn the pool *off*.
	  /// Since we are not managing the pool, we simply release our reference to
	  /// the datasource.  Future calls to the getConnection will simply recreate
	  /// the datasource. </summary>
	  /// <param name="flag"> If false, the reference to the datasource is released. </param>
	  public virtual bool PoolEnabled
	  {
		  set
		  {
			if (!value)
			{
				jdbcSource = null;
			}
		  }
	  }

	  /// <summary>
	  /// Ignored in this implementation b/c the pooling is determined by the jndi dataosource. </summary>
	  /// <param name="p"> </param>
	  public virtual Properties Protocol
	  {
		  set
		  {
			/* ignore - properties are determined by datasource */
		  }
	  }

	  /// <summary>
	  /// Ignored in this implementation b/c the pooling is determined by the jndi dataosource. </summary>
	  /// <param name="n">  </param>
	  public virtual int MinConnections
	  {
		  set
		  {
			/* ignore - pooling is determined by datasource */
		  }
	  }

	  /// <summary>
	  /// A simple test to see if the jndi datasource exists.
	  /// 
	  /// Note that this test does not ensure that the datasource will return valid
	  /// connections.
	  /// </summary>
	  public virtual bool testConnection()
	  {
		if (jdbcSource == null)
		{
		  try
		  {
			findDatasource();
		  }
		  catch (NamingException)
		  {
			return false;
		  }
		}

		return true;
	  }



	}

}