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

namespace org.apache.xpath.objects
{

	using DTM = org.apache.xml.dtm.DTM;
	/*
	 * 
	 * @author igorh
	 *
	 * Simple wrapper to DTM and XPathContext objects.
	 * Used in XRTreeFrag for caching references to the objects.
	 */
	 public sealed class DTMXRTreeFrag
	 {
	  private DTM m_dtm;
	  private int m_dtmIdentity = org.apache.xml.dtm.DTM_Fields.NULL;
	  private XPathContext m_xctxt;

	  public DTMXRTreeFrag(int dtmIdentity, XPathContext xctxt)
	  {
		  m_xctxt = xctxt;
		  m_dtmIdentity = dtmIdentity;
		  m_dtm = xctxt.getDTM(dtmIdentity);
	  }

	  public void destruct()
	  {
		m_dtm = null;
		m_xctxt = null;
	  }

	internal DTM DTM
	{
		get
		{
			return m_dtm;
		}
	}
	public int DTMIdentity
	{
		get
		{
			return m_dtmIdentity;
		}
	}
	internal XPathContext XPathContext
	{
		get
		{
			return m_xctxt;
		}
	}

	public override int GetHashCode()
	{
		return m_dtmIdentity;
	}
	public override bool Equals(object obj)
	{
	   if (obj is DTMXRTreeFrag)
	   {
		   return (m_dtmIdentity == ((DTMXRTreeFrag)obj).DTMIdentity);
	   }
	   return false;
	}

	 }

}