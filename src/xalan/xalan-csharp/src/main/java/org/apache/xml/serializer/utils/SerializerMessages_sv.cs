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
 * $Id: SerializerMessages_sv.java 468654 2006-10-28 07:09:23Z minchau $
 */

namespace org.apache.xml.serializer.utils
{

	public class SerializerMessages_sv : ListResourceBundle
	{
	  public virtual object[][] Contents
	  {
		  get
		  {
			object[][] contents = new object[][]
			{
				new object[] {MsgKey.ER_INVALID_PORT, "Ogiltigt portnummer"},
				new object[] {MsgKey.ER_PORT_WHEN_HOST_NULL, "Port kan inte s\u00e4ttas n\u00e4r v\u00e4rd \u00e4r null"},
				new object[] {MsgKey.ER_HOST_ADDRESS_NOT_WELLFORMED, "V\u00e4rd \u00e4r inte en v\u00e4lformulerad adress"},
				new object[] {MsgKey.ER_SCHEME_NOT_CONFORMANT, "Schemat \u00e4r inte likformigt."},
				new object[] {MsgKey.ER_SCHEME_FROM_NULL_STRING, "Kan inte s\u00e4tta schema fr\u00e5n null-str\u00e4ng"},
				new object[] {MsgKey.ER_PATH_CONTAINS_INVALID_ESCAPE_SEQUENCE, "V\u00e4g inneh\u00e5ller ogiltig flyktsekvens"},
				new object[] {MsgKey.ER_PATH_INVALID_CHAR, "V\u00e4g inneh\u00e5ller ogiltigt tecken: {0}"},
				new object[] {MsgKey.ER_FRAG_INVALID_CHAR, "Fragment inneh\u00e5ller ogiltigt tecken"},
				new object[] {MsgKey.ER_FRAG_WHEN_PATH_NULL, "Fragment kan inte s\u00e4ttas n\u00e4r v\u00e4g \u00e4r null"},
				new object[] {MsgKey.ER_FRAG_FOR_GENERIC_URI, "Fragment kan bara s\u00e4ttas f\u00f6r en allm\u00e4n URI"},
				new object[] {MsgKey.ER_NO_SCHEME_IN_URI, "Schema saknas i URI: {0}"},
				new object[] {MsgKey.ER_CANNOT_INIT_URI_EMPTY_PARMS, "Kan inte initialisera URI med tomma parametrar"},
				new object[] {MsgKey.ER_NO_FRAGMENT_STRING_IN_PATH, "Fragment kan inte anges i b\u00e5de v\u00e4gen och fragmentet"},
				new object[] {MsgKey.ER_NO_QUERY_STRING_IN_PATH, "F\u00f6rfr\u00e5gan-str\u00e4ng kan inte anges i v\u00e4g och f\u00f6rfr\u00e5gan-str\u00e4ng"},
				new object[] {MsgKey.ER_NO_PORT_IF_NO_HOST, "Port f\u00e5r inte anges om v\u00e4rden inte \u00e4r angiven"},
				new object[] {MsgKey.ER_NO_USERINFO_IF_NO_HOST, "Userinfo f\u00e5r inte anges om v\u00e4rden inte \u00e4r angiven"},
				new object[] {MsgKey.ER_SCHEME_REQUIRED, "Schema kr\u00e4vs!"}
			};
			return contents;
		  }
	  }
	}

}