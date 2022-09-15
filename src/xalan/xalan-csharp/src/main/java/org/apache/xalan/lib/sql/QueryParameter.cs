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
 * $Id: QueryParameter.java 468638 2006-10-28 06:52:06Z minchau $
 */

/* This class holds a parameter definition for a JDBC PreparedStatement or CallableStatement. */

namespace org.apache.xalan.lib.sql
{

	public class QueryParameter
	{
	  private int m_type;
	  private string m_name;
	  private string m_value;
	  private bool m_output;
	  private string m_typeName;
	  private static Hashtable m_Typetable = null;

	  public QueryParameter()
	  {
		m_type = -1;
		m_name = null;
		m_value = null;
		m_output = false;
		m_typeName = null;
	  }

	  /// <param name="v"> The parameter value. </param>
	  /// <param name="t"> The type of the parameter. </param>
	  public QueryParameter(string v, string t)
	  {
		m_name = null;
		m_value = v;
		m_output = false;
		TypeName = t;
	  }

	  public QueryParameter(string name, string value, string type, bool out_flag)
	  {
		m_name = name;
		m_value = value;
		m_output = out_flag;
		TypeName = type;
	  }

	  /// 
	  public virtual string Value
	  {
		  get
		  {
			return m_value;
		  }
		  set
		  {
			m_value = value;
		  }
	  }


	  /// <summary>
	  /// Used to set the parameter type when the type information is provided in the query. </summary>
	  /// <param name="newType"> The parameter type.
	  ///  </param>
	  public virtual string TypeName
	  {
		  set
		  {
			m_type = map_type(value);
			m_typeName = value;
		  }
		  get
		  {
			return m_typeName;
		  }
	  }


	  /// 
	  public virtual int Type
	  {
		  get
		  {
			return m_type;
		  }
	  }

	  /// 
	  public virtual string Name
	  {
		  get
		  {
			return m_name;
		  }
		  set
		  {
			m_name = value;
		  }
	  }


	  /// 
	  public virtual bool Output
	  {
		  get
		  {
			return m_output;
		  }
	  }

	  /// <summary>
	  /// Set Name, this should really be covered in the constructor but the
	  /// QueryParser has a State issue where the name is discoverd after the
	  /// Parameter object needs to be created
	  /// </summary>
	  public virtual bool IsOutput
	  {
		  set
		  {
			m_output = value;
		  }
	  }

	  private static int map_type(string typename)
	  {
		if (m_Typetable == null)
		{
		  // Load up the type mapping table.
		  m_Typetable = new Hashtable();
		  m_Typetable["BIGINT"] = new int?(java.sql.Types.BIGINT);
		  m_Typetable["BINARY"] = new int?(java.sql.Types.BINARY);
		  m_Typetable["BIT"] = new int?(java.sql.Types.BIT);
		  m_Typetable["CHAR"] = new int?(java.sql.Types.CHAR);
		  m_Typetable["DATE"] = new int?(java.sql.Types.DATE);
		  m_Typetable["DECIMAL"] = new int?(java.sql.Types.DECIMAL);
		  m_Typetable["DOUBLE"] = new int?(java.sql.Types.DOUBLE);
		  m_Typetable["FLOAT"] = new int?(java.sql.Types.FLOAT);
		  m_Typetable["INTEGER"] = new int?(java.sql.Types.INTEGER);
		  m_Typetable["LONGVARBINARY"] = new int?(java.sql.Types.LONGVARBINARY);
		  m_Typetable["LONGVARCHAR"] = new int?(java.sql.Types.LONGVARCHAR);
		  m_Typetable["NULL"] = new int?(java.sql.Types.NULL);
		  m_Typetable["NUMERIC"] = new int?(java.sql.Types.NUMERIC);
		  m_Typetable["OTHER"] = new int?(java.sql.Types.OTHER);
		  m_Typetable["REAL"] = new int?(java.sql.Types.REAL);
		  m_Typetable["SMALLINT"] = new int?(java.sql.Types.SMALLINT);
		  m_Typetable["TIME"] = new int?(java.sql.Types.TIME);
		  m_Typetable["TIMESTAMP"] = new int?(java.sql.Types.TIMESTAMP);
		  m_Typetable["TINYINT"] = new int?(java.sql.Types.TINYINT);
		  m_Typetable["VARBINARY"] = new int?(java.sql.Types.VARBINARY);
		  m_Typetable["VARCHAR"] = new int?(java.sql.Types.VARCHAR);

		  // Aliases from Xalan SQL extension.
		  m_Typetable["STRING"] = new int?(java.sql.Types.VARCHAR);
		  m_Typetable["BIGDECIMAL"] = new int?(java.sql.Types.NUMERIC);
		  m_Typetable["BOOLEAN"] = new int?(java.sql.Types.BIT);
		  m_Typetable["BYTES"] = new int?(java.sql.Types.LONGVARBINARY);
		  m_Typetable["LONG"] = new int?(java.sql.Types.BIGINT);
		  m_Typetable["SHORT"] = new int?(java.sql.Types.SMALLINT);
		}

		int? type = (int?) m_Typetable[typename.ToUpper()];
		int rtype;
		if (type == null)
		{
		  rtype = java.sql.Types.OTHER;
		}
		else
		{
		  rtype = type.Value;
		}

		return (rtype);
	  }

	  /// <summary>
	  /// This code was in the XConnection, it is included for reference but it
	  /// should not be used.
	  /// 
	  /// @TODO Remove this code as soon as it is determined that its Use Case is
	  /// resolved elsewhere.
	  /// </summary>
	  /// <summary>
	  /// Set the parameter for a Prepared Statement </summary>
	  /// <param name="pos"> </param>
	  /// <param name="stmt"> </param>
	  /// <param name="p">
	  /// </param>
	  /// <exception cref="SQLException"> </exception>
	  /*
	  private void setParameter( int pos, PreparedStatement stmt, QueryParameter p )throws SQLException
	  {
	    String type = p.getType();
	    if (type.equalsIgnoreCase("string"))
	    {
	      stmt.setString(pos, p.getValue());
	    }
	
	    if (type.equalsIgnoreCase("bigdecimal"))
	    {
	      stmt.setBigDecimal(pos, new BigDecimal(p.getValue()));
	    }
	
	    if (type.equalsIgnoreCase("boolean"))
	    {
	      Integer i = new Integer( p.getValue() );
	      boolean b = ((i.intValue() != 0) ? false : true);
	      stmt.setBoolean(pos, b);
	    }
	
	    if (type.equalsIgnoreCase("bytes"))
	    {
	      stmt.setBytes(pos, p.getValue().getBytes());
	    }
	
	    if (type.equalsIgnoreCase("date"))
	    {
	      stmt.setDate(pos, Date.valueOf(p.getValue()));
	    }
	
	    if (type.equalsIgnoreCase("double"))
	    {
	      Double d = new Double(p.getValue());
	      stmt.setDouble(pos, d.doubleValue() );
	    }
	
	    if (type.equalsIgnoreCase("float"))
	    {
	      Float f = new Float(p.getValue());
	      stmt.setFloat(pos, f.floatValue());
	    }
	
	    if (type.equalsIgnoreCase("long"))
	    {
	      Long l = new Long(p.getValue());
	      stmt.setLong(pos, l.longValue());
	    }
	
	    if (type.equalsIgnoreCase("short"))
	    {
	      Short s = new Short(p.getValue());
	      stmt.setShort(pos, s.shortValue());
	    }
	
	    if (type.equalsIgnoreCase("time"))
	    {
	      stmt.setTime(pos, Time.valueOf(p.getValue()) );
	    }
	
	    if (type.equalsIgnoreCase("timestamp"))
	    {
	
	      stmt.setTimestamp(pos, Timestamp.valueOf(p.getValue()) );
	    }
	
	  }
	  */

	}



}