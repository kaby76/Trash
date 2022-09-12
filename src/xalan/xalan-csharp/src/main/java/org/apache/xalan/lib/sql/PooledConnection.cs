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
 * $Id: PooledConnection.java 468638 2006-10-28 06:52:06Z minchau $
 */
namespace org.apache.xalan.lib.sql
{


	public class PooledConnection
	{

	  // Real JDBC Connection
	  private Connection connection = null;
	  // boolean flag used to determine if connection is in use
	  private bool inuse = false;

	  // Constructor that takes the passed in JDBC Connection
	  // and stores it in the connection attribute.
	  /// <param name="value"> </param>
	  public PooledConnection(Connection value)
	  {
		if (value != null)
		{
			connection = value;
		}
	  }

	  /// <summary>
	  /// Returns a reference to the JDBC Connection </summary>
	  /// <returns> Connection </returns>
	  public virtual Connection Connection
	  {
		  get
		  {
			// get the JDBC Connection
			return connection;
		  }
	  }

	  /// <summary>
	  /// Set the status of the PooledConnection.
	  /// </summary>
	  /// <param name="value">
	  ///  </param>
	  public virtual bool InUse
	  {
		  set
		  {
			inuse = value;
		  }
	  }

	  /// <summary>
	  /// Returns the current status of the PooledConnection.
	  /// 
	  /// </summary>
	  public virtual bool inUse()
	  {
		  return inuse;
	  }

	  /// <summary>
	  ///  Close the real JDBC Connection
	  /// 
	  /// </summary>
	  public virtual void close()
	  {
		try
		{
		  connection.close();
		}
		catch (SQLException sqle)
		{
		  Console.Error.WriteLine(sqle.Message);
		}
	  }
	}

}