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
 * $Id: ProcessorImport.java 468640 2006-10-28 06:53:53Z minchau $
 */
namespace org.apache.xalan.processor
{
	using XSLTErrorResources = org.apache.xalan.res.XSLTErrorResources;

	/// <summary>
	/// This class processes parse events for an xsl:import element. </summary>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.dtd">XSLT DTD</a>"/>
	/// <seealso cref="<a href="http://www.w3.org/TR/xslt.import">import in XSLT Specification</a>"
	/// 
	/// @xsl.usage internal/>
	[Serializable]
	public class ProcessorImport : ProcessorInclude
	{
		internal new const long serialVersionUID = -8247537698214245237L;

	  /// <summary>
	  /// Get the stylesheet type associated with an imported stylesheet
	  /// </summary>
	  /// <returns> the type of the stylesheet </returns>
	  protected internal override int StylesheetType
	  {
		  get
		  {
			return StylesheetHandler.STYPE_IMPORT;
		  }
	  }

	  /// <summary>
	  /// Get the error number associated with this type of stylesheet importing itself
	  /// </summary>
	  /// <returns> the appropriate error number </returns>
	  protected internal override string StylesheetInclErr
	  {
		  get
		  {
			return XSLTErrorResources.ER_IMPORTING_ITSELF;
		  }
	  }

	}

}