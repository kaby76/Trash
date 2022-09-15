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
 * $Id: ConnectionPool.java 468638 2006-10-28 06:52:06Z minchau $
 */

namespace org.apache.xalan.lib.sql
{

	/// <summary>
	/// An interface used to build wrapper classes around existing
	/// Connection Pool libraries.
	/// Title:     ConnectionPool<para>
	/// @author John Gentilin
	/// @version 1.0
	/// </para>
	/// </summary>
	public interface ConnectionPool
	{

	  /// <summary>
	  /// Determine if a Connection Pool has been disabled. If a Connection pool
	  /// is disabled, then it will only manage connections that are in use.
	  /// 
	  /// </summary>
	  bool Enabled {get;}

	  /// <summary>
	  /// The Driver and URL are the only required parmeters. </summary>
	  /// <param name="d">
	  ///  </param>
	  string Driver {set;}

	  /// <param name="url">
	  ///  </param>
	  string URL {set;}

	  /// <summary>
	  /// Start downsizeing the pool, this usally happens right after the
	  /// pool has been marked as Inactive and we are removing connections
	  /// that are not currently inuse.
	  /// 
	  /// </summary>
	  void freeUnused();


	  /// <summary>
	  /// Provide an indicator to the PoolManager when the Pool can be removed
	  /// from the Pool Table.
	  /// 
	  /// </summary>
	  bool hasActiveConnections();

	  /// <summary>
	  /// The rest of the protocol parameters can eiter be passed in as
	  /// just Username and Password or as a property collection. If the
	  /// property collection is used, then the sperate username and password
	  /// may be ignored, it is up to the wrapper implementation to handle
	  /// the situation. If the connection information changes while after the
	  /// pool has been established, the wrapper implementation should ignore
	  /// the change and throw an error. </summary>
	  /// <param name="p">
	  ///  </param>
	  string Password {set;}

	  /// <param name="u">
	  ///  </param>
	  string User {set;}


	  /// <summary>
	  /// Set tne minimum number of connections that are to be maintained in the
	  /// pool. </summary>
	  /// <param name="n">
	  ///  </param>
	  int MinConnections {set;}

	  /// <summary>
	  /// Test to see if the connection info is valid to make a real connection
	  /// to the database. This method may cause the pool to be crated and filled
	  /// with min connections.
	  /// 
	  /// </summary>
	  bool testConnection();

	  /// <summary>
	  /// Retrive a database connection from the pool
	  /// </summary>
	  /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public java.sql.Connection getConnection()throws java.sql.SQLException;
	  Connection Connection {get;}

	   /// <summary>
	   /// Return a connection to the pool, the connection may be closed if the
	   /// pool is inactive or has exceeded the max number of free connections </summary>
	   /// <param name="con">
	   /// </param>
	   /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void releaseConnection(java.sql.Connection con)throws java.sql.SQLException;
	  void releaseConnection(Connection con);

	   /// <summary>
	   /// Provide a mechinism to return a connection to the pool on Error.
	   /// A good default behaviour is to close this connection and build
	   /// a new one to replace it. Some JDBC impl's won't allow you to
	   /// reuse a connection after an error occurs. </summary>
	   /// <param name="con">
	   /// </param>
	   /// <exception cref="SQLException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void releaseConnectionOnError(java.sql.Connection con)throws java.sql.SQLException;
	  void releaseConnectionOnError(Connection con);


	  /// <summary>
	  /// The Pool can be Enabled and Disabled. Disabling the pool
	  /// closes all the outstanding Unused connections and any new
	  /// connections will be closed upon release. </summary>
	  /// <param name="flag"> Control the Connection Pool. If it is enabled
	  /// then Connections will actuall be held around. If disabled
	  /// then all unused connections will be instantly closed and as
	  /// connections are released they are closed and removed from the pool.
	  ///  </param>
	  in bool PoolEnabled {set;}

	  /// <summary>
	  /// Used to pass in extra configuration options during the
	  /// database connect phase.
	  /// </summary>
	  Properties Protocol {set;}


	}

}