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
 * $Id: Operators.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime
{

	/// <summary>
	/// @author Jacek Ambroziak
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public sealed class Operators
	{
		public const int EQ = 0;
		public const int NE = 1;
		public const int GT = 2;
		public const int LT = 3;
		public const int GE = 4;
		public const int LE = 5;

		private static readonly string[] names = new string[] {"=", "!=", ">", "<", ">=", "<="};

		public static string getOpNames(int @operator)
		{
			  return names[@operator];
		}

	//  Swap operator array
		private static readonly int[] swapOpArray = new int[] {EQ, NE, LT, GT, LE, GE};

		public static int swapOp(int @operator)
		{
			  return swapOpArray[@operator];
		}

	}

}