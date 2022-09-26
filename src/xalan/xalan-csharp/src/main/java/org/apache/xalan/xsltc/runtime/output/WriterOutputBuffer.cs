using System;
using System.IO;

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
 * $Id: WriterOutputBuffer.java 468652 2006-10-28 07:05:17Z minchau $
 */

namespace org.apache.xalan.xsltc.runtime.output
{

	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	internal class WriterOutputBuffer : OutputBuffer
	{
		private const int KB = 1024;
		private static int BUFFER_SIZE = 4 * KB;

		static WriterOutputBuffer()
		{
		// Set a larger buffer size for Solaris
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String osName = System.getProperty("os.name");
		string osName = System.getProperty("os.name");
		if (osName.Equals("solaris", StringComparison.OrdinalIgnoreCase))
		{
			BUFFER_SIZE = 32 * KB;
		}
		}

		private Writer _writer;

		/// <summary>
		/// Initializes a WriterOutputBuffer by creating an instance of a 
		/// BufferedWriter. The size of the buffer in this writer may have 
		/// a significant impact on throughput. Solaris prefers a larger
		/// buffer, while Linux works better with a smaller one.
		/// </summary>
		public WriterOutputBuffer(Writer writer)
		{
		_writer = new StreamWriter(writer, BUFFER_SIZE);
		}

		public virtual string close()
		{
		try
		{
			_writer.flush();
		}
		catch (IOException e)
		{
			throw new Exception(e.ToString());
		}
		return "";
		}

		public virtual OutputBuffer append(string s)
		{
		try
		{
			_writer.write(s);
		}
		catch (IOException e)
		{
			throw new Exception(e.ToString());
		}
		return this;
		}

		public virtual OutputBuffer append(char[] s, int from, int to)
		{
		try
		{
			_writer.write(s, from, to);
		}
		catch (IOException e)
		{
			throw new Exception(e.ToString());
		}
		return this;
		}

		public virtual OutputBuffer append(char ch)
		{
		try
		{
			_writer.write(ch);
		}
		catch (IOException e)
		{
			throw new Exception(e.ToString());
		}
		return this;
		}
	}



}