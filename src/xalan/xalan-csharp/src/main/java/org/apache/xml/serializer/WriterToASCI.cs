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
 * $Id: WriterToASCI.java 468654 2006-10-28 07:09:23Z minchau $
 */
namespace org.apache.xml.serializer
{




	/// <summary>
	/// This class writes ASCII to a byte stream as quickly as possible.  For the
	/// moment it does not do buffering, though I reserve the right to do some
	/// buffering down the line if I can prove that it will be faster even if the
	/// output stream is buffered.
	/// 
	/// This class is only used internally within Xalan.
	/// 
	/// @xsl.usage internal
	/// </summary>
	internal class WriterToASCI : Writer, WriterChain
	{

	  /// <summary>
	  /// The byte stream to write to. </summary>
	  private readonly System.IO.Stream m_os;

	  /// <summary>
	  /// Create an unbuffered ASCII writer.
	  /// 
	  /// </summary>
	  /// <param name="os"> The byte stream to write to. </param>
	  public WriterToASCI(System.IO.Stream os)
	  {
		m_os = os;
	  }

	  /// <summary>
	  /// Write a portion of an array of characters.
	  /// </summary>
	  /// <param name="chars">  Array of characters </param>
	  /// <param name="start">   Offset from which to start writing characters </param>
	  /// <param name="length">   Number of characters to write
	  /// </param>
	  /// <exception cref="IOException">  If an I/O error occurs
	  /// </exception>
	  /// <exception cref="java.io.IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(char chars[], int start, int length) throws java.io.IOException
	  public virtual void write(char[] chars, int start, int length)
	  {

		int n = length + start;

		for (int i = start; i < n; i++)
		{
		  m_os.WriteByte(chars[i]);
		}
	  }

	  /// <summary>
	  /// Write a single character.  The character to be written is contained in
	  /// the 16 low-order bits of the given integer value; the 16 high-order bits
	  /// are ignored.
	  /// 
	  /// <para> Subclasses that intend to support efficient single-character output
	  /// should override this method.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="c">  int specifying a character to be written. </param>
	  /// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(int c) throws java.io.IOException
	  public virtual void write(int c)
	  {
		m_os.WriteByte(c);
	  }

	  /// <summary>
	  /// Write a string.
	  /// </summary>
	  /// <param name="s"> String to be written
	  /// </param>
	  /// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void write(String s) throws java.io.IOException
	  public virtual void write(string s)
	  {
		int n = s.Length;
		for (int i = 0; i < n; i++)
		{
		  m_os.WriteByte(s[i]);
		}
	  }

	  /// <summary>
	  /// Flush the stream.  If the stream has saved any characters from the
	  /// various write() methods in a buffer, write them immediately to their
	  /// intended destination.  Then, if that destination is another character or
	  /// byte stream, flush it.  Thus one flush() invocation will flush all the
	  /// buffers in a chain of Writers and OutputStreams.
	  /// </summary>
	  /// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void flush() throws java.io.IOException
	  public virtual void flush()
	  {
		m_os.Flush();
	  }

	  /// <summary>
	  /// Close the stream, flushing it first.  Once a stream has been closed,
	  /// further write() or flush() invocations will cause an IOException to be
	  /// thrown.  Closing a previously-closed stream, however, has no effect.
	  /// </summary>
	  /// <exception cref="IOException">  If an I/O error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void close() throws java.io.IOException
	  public virtual void close()
	  {
		m_os.Close();
	  }

	  /// <summary>
	  /// Get the output stream where the events will be serialized to.
	  /// </summary>
	  /// <returns> reference to the result stream, or null of only a writer was
	  /// set. </returns>
	  public virtual System.IO.Stream OutputStream
	  {
		  get
		  {
			return m_os;
		  }
	  }

	  /// <summary>
	  /// Get the writer that this writer directly chains to.
	  /// </summary>
	  public virtual Writer Writer
	  {
		  get
		  {
			  return null;
		  }
	  }
	}

}