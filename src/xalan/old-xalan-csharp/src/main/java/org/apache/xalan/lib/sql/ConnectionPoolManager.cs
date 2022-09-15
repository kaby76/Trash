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
 * $Id: ConnectionPoolManager.java 468638 2006-10-28 06:52:06Z minchau $
 */


 namespace org.apache.xalan.lib.sql
 {

	using XSLMessages = org.apache.xalan.res.XSLMessages;
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	public class ConnectionPoolManager
	{
	  private static Hashtable m_poolTable = null;

	  public ConnectionPoolManager()
	  {
		init();
	  }

	  /// <summary>
	  /// Initialize the internal structures of the Pool Manager
	  /// 
	  /// </summary>
	  private void init()
	  {
		  lock (this)
		  {
			/// <summary>
			/// Only do this process once
			/// Initialize the pool table
			/// </summary>
			if (m_poolTable == null)
			{
					m_poolTable = new Hashtable();
			}
		  }
	  }

	  /// <summary>
	  /// Register a nuew connection pool to the global pool table.
	  /// If a pool by that name currently exists, then throw an
	  /// IllegalArgumentException stating that the pool already
	  /// exist. </summary>
	  /// <param name="name"> </param>
	  /// <param name="pool">
	  /// 
	  /// @link org.apache.xalan.lib.sql.ConnectionPool}
	  /// </param>
	  /// @throws <code>IllegalArgumentException</code>, throw this exception
	  /// if a pool with the same name currently exists. </exception>
	  public virtual void registerPool(string name, ConnectionPool pool)
	  {
		  lock (this)
		  {
			if (m_poolTable.ContainsKey(name))
			{
			  throw new System.ArgumentException(XSLMessages.createMessage(XSLTErrorResources.ER_POOL_EXISTS, null)); //"Pool already exists");
			}
        
			m_poolTable[name] = pool;
		  }
	  }

	  /// <summary>
	  /// Remove a pool from the global table. If the pool still has
	  /// active connections, then only mark this pool as inactive and
	  /// leave it around until all the existing connections are closed. </summary>
	  /// <param name="name">
	  ///  </param>
	  public virtual void removePool(string name)
	  {
		  lock (this)
		  {
			ConnectionPool pool = getPool(name);
        
			if (null != pool)
			{
			  //
			  // Disable future use of this pool under the Xalan
			  // extension only. This flag should only exist in the
			  // wrapper and not in the actual pool implementation.
			  pool.PoolEnabled = false;
        
        
			  //
			  // Remove the pool from the Hashtable if we don'd have
			  // any active connections.
			  //
			  if (!pool.hasActiveConnections())
			  {
				  m_poolTable.Remove(name);
			  }
			}
        
		  }
	  }


	  /// <summary>
	  /// Return the connection pool referenced by the name </summary>
	  /// <param name="name">
	  /// </param>
	  /// <returns> <code>ConnectionPool</code> a reference to the ConnectionPool
	  /// object stored in the Pool Table. If the named pool does not exist, return
	  /// null </returns>
	  public virtual ConnectionPool getPool(string name)
	  {
		  lock (this)
		  {
			return (ConnectionPool) m_poolTable[name];
		  }
	  }

	}

 }