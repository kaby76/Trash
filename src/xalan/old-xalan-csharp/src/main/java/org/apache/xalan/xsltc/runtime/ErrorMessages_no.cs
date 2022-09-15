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
 * $Id: ErrorMessages_no.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Morten Jorgensen
	/// </summary>
	public class ErrorMessages_no : ListResourceBundle
	{


		// Disse feilmeldingene maa korrespondere med konstantene some er definert
		// nederst i kildekoden til BasisLibrary.
		/// <summary>
		/// Get the lookup table for error messages.   
		/// </summary>
		/// <returns> The message lookup table. </returns>
		public virtual object[][] Contents
		{
			get
			{
			  return new object[][]
			  {
				  new object[] {BasisLibrary.RUN_TIME_INTERNAL_ERR, "Intern programfeil i ''{0}''"},
				  new object[] {BasisLibrary.RUN_TIME_COPY_ERR, "Programfeil under utf\u00f8ing av <xsl:copy>."},
				  new object[] {BasisLibrary.DATA_CONVERSION_ERR, "Ugyldig konvertering av ''{0}'' fra ''{1}''."},
				  new object[] {BasisLibrary.EXTERNAL_FUNC_ERR, "Ekstern funksjon ''{0}'' er ikke st\u00f8ttet av XSLTC."},
				  new object[] {BasisLibrary.EQUALITY_EXPR_ERR, "Ugyldig argument i EQUALITY uttrykk."},
				  new object[] {BasisLibrary.INVALID_ARGUMENT_ERR, "Ugyldig argument ''{0}'' i kall til ''{1}''"},
				  new object[] {BasisLibrary.FORMAT_NUMBER_ERR, "Fors\u00f8k p\u00e5 \u00e5 formattere nummer ''{0}'' med ''{1}''."},
				  new object[] {BasisLibrary.ITERATOR_CLONE_ERR, "Kan ikke klone iterator ''{0}''."},
				  new object[] {BasisLibrary.AXIS_SUPPORT_ERR, "Iterator for axis ''{0}'' er ikke st\u00e8ttet."},
				  new object[] {BasisLibrary.TYPED_AXIS_SUPPORT_ERR, "Iterator for typet axis ''{0}'' er ikke st\u00e8ttet."},
				  new object[] {BasisLibrary.STRAY_ATTRIBUTE_ERR, "Attributt ''{0}'' utenfor element."},
				  new object[] {BasisLibrary.STRAY_NAMESPACE_ERR, "Navnedeklarasjon ''{0}''=''{1}'' utenfor element."},
				  new object[] {BasisLibrary.NAMESPACE_PREFIX_ERR, "Prefix ''{0}'' er ikke deklartert."},
				  new object[] {BasisLibrary.DOM_ADAPTER_INIT_ERR, "Fors\u00f8k p\u00e5 \u00e5 instansiere DOMAdapter med feil type DOM."}
			  };
			}
		}
	}

}