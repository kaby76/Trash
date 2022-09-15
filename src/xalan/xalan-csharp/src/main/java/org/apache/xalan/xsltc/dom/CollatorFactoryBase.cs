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
 * $Id: CollatorFactoryBase.java 468651 2006-10-28 07:04:25Z minchau $
 */

namespace org.apache.xalan.xsltc.dom
{

	using CollatorFactory = org.apache.xalan.xsltc.CollatorFactory;

	/// <summary>
	/// @author W. Eliot Kimber (eliot@isogen.com)
	/// </summary>
	public class CollatorFactoryBase : CollatorFactory
	{

		public static readonly Locale DEFAULT_LOCALE = Locale.getDefault();
		public static readonly Collator DEFAULT_COLLATOR = Collator.getInstance();

		public CollatorFactoryBase()
		{
		}

		public virtual Collator getCollator(string lang, string country)
		{
			return Collator.getInstance(new Locale(lang, country));
		}

		public virtual Collator getCollator(Locale locale)
		{
			if (locale == DEFAULT_LOCALE)
			{
				return DEFAULT_COLLATOR;
			}
			else
			{
				return Collator.getInstance(locale);
			}
		}
	}

}