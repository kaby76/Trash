using System;
using System.Text;

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
 * $Id: DTMException.java 468653 2006-10-28 07:07:05Z minchau $
 */
namespace org.apache.xml.dtm
{



	using XMLErrorResources = org.apache.xml.res.XMLErrorResources;
	using XMLMessages = org.apache.xml.res.XMLMessages;


	/// <summary>
	/// This class specifies an exceptional condition that occured
	/// in the DTM module.
	/// </summary>
	public class DTMException : Exception
	{
		internal const long serialVersionUID = -775576419181334734L;

		/// <summary>
		/// Field locator specifies where the error occured.
		///  @serial 
		/// </summary>
		internal SourceLocator locator;

		/// <summary>
		/// Method getLocator retrieves an instance of a SourceLocator
		/// object that specifies where an error occured.
		/// </summary>
		/// <returns> A SourceLocator object, or null if none was specified. </returns>
		public virtual SourceLocator Locator
		{
			get
			{
				return locator;
			}
			set
			{
				locator = value;
			}
		}


		/// <summary>
		/// Field containedException specifies a wrapped exception.  May be null.
		///  @serial 
		/// </summary>
		internal Exception containedException;

		/// <summary>
		/// This method retrieves an exception that this exception wraps.
		/// </summary>
		/// <returns> An Throwable object, or null. </returns>
		/// <seealso cref= #getCause </seealso>
		public virtual Exception Exception
		{
			get
			{
				return containedException;
			}
		}

		/// <summary>
		/// Returns the cause of this throwable or <code>null</code> if the
		/// cause is nonexistent or unknown.  (The cause is the throwable that
		/// caused this throwable to get thrown.)
		/// </summary>
		public virtual Exception Cause
		{
			get
			{
    
				return ((containedException == this) ? null : containedException);
			}
		}

		/// <summary>
		/// Initializes the <i>cause</i> of this throwable to the specified value.
		/// (The cause is the throwable that caused this throwable to get thrown.)
		/// 
		/// <para>This method can be called at most once.  It is generally called from
		/// within the constructor, or immediately after creating the
		/// throwable.  If this throwable was created
		/// with <seealso cref="#DTMException(Throwable)"/> or
		/// <seealso cref="#DTMException(String,Throwable)"/>, this method cannot be called
		/// even once.
		/// 
		/// </para>
		/// </summary>
		/// <param name="cause"> the cause (which is saved for later retrieval by the
		///         <seealso cref="#getCause()"/> method).  (A <tt>null</tt> value is
		///         permitted, and indicates that the cause is nonexistent or
		///         unknown.) </param>
		/// <returns>  a reference to this <code>Throwable</code> instance. </returns>
		/// <exception cref="IllegalArgumentException"> if <code>cause</code> is this
		///         throwable.  (A throwable cannot
		///         be its own cause.) </exception>
		/// <exception cref="IllegalStateException"> if this throwable was
		///         created with <seealso cref="#DTMException(Throwable)"/> or
		///         <seealso cref="#DTMException(String,Throwable)"/>, or this method has already
		///         been called on this throwable. </exception>
		public virtual Exception initCause(Exception cause)
		{
			lock (this)
			{
        
				if ((this.containedException == null) && (cause != null))
				{
					throw new System.InvalidOperationException(XMLMessages.createXMLMessage(XMLErrorResources.ER_CANNOT_OVERWRITE_CAUSE, null)); //"Can't overwrite cause");
				}
        
				if (cause == this)
				{
					throw new System.ArgumentException(XMLMessages.createXMLMessage(XMLErrorResources.ER_SELF_CAUSATION_NOT_PERMITTED, null)); //"Self-causation not permitted");
				}
        
				this.containedException = cause;
        
				return this;
			}
		}

		/// <summary>
		/// Create a new DTMException.
		/// </summary>
		/// <param name="message"> The error or warning message. </param>
		public DTMException(string message) : base(message)
		{


			this.containedException = null;
			this.locator = null;
		}

		/// <summary>
		/// Create a new DTMException wrapping an existing exception.
		/// </summary>
		/// <param name="e"> The exception to be wrapped. </param>
		public DTMException(Exception e) : base(e.Message)
		{


			this.containedException = e;
			this.locator = null;
		}

		/// <summary>
		/// Wrap an existing exception in a DTMException.
		/// 
		/// <para>This is used for throwing processor exceptions before
		/// the processing has started.</para>
		/// </summary>
		/// <param name="message"> The error or warning message, or null to
		///                use the message from the embedded exception. </param>
		/// <param name="e"> Any exception </param>
		public DTMException(string message, Exception e) : base(((string.ReferenceEquals(message, null)) || (message.Length == 0)) ? e.Message : message)
		{


			this.containedException = e;
			this.locator = null;
		}

		/// <summary>
		/// Create a new DTMException from a message and a Locator.
		/// 
		/// <para>This constructor is especially useful when an application is
		/// creating its own exception from within a DocumentHandler
		/// callback.</para>
		/// </summary>
		/// <param name="message"> The error or warning message. </param>
		/// <param name="locator"> The locator object for the error or warning. </param>
		public DTMException(string message, SourceLocator locator) : base(message)
		{


			this.containedException = null;
			this.locator = locator;
		}

		/// <summary>
		/// Wrap an existing exception in a DTMException.
		/// </summary>
		/// <param name="message"> The error or warning message, or null to
		///                use the message from the embedded exception. </param>
		/// <param name="locator"> The locator object for the error or warning. </param>
		/// <param name="e"> Any exception </param>
		public DTMException(string message, SourceLocator locator, Exception e) : base(message)
		{


			this.containedException = e;
			this.locator = locator;
		}

		/// <summary>
		/// Get the error message with location information
		/// appended.
		/// </summary>
		public virtual string MessageAndLocation
		{
			get
			{
    
				StringBuilder sbuffer = new StringBuilder();
				string message = base.Message;
    
				if (null != message)
				{
					sbuffer.Append(message);
				}
    
				if (null != locator)
				{
					string systemID = locator.SystemId;
					int line = locator.LineNumber;
					int column = locator.ColumnNumber;
    
					if (null != systemID)
					{
						sbuffer.Append("; SystemID: ");
						sbuffer.Append(systemID);
					}
    
					if (0 != line)
					{
						sbuffer.Append("; Line#: ");
						sbuffer.Append(line);
					}
    
					if (0 != column)
					{
						sbuffer.Append("; Column#: ");
						sbuffer.Append(column);
					}
				}
    
				return sbuffer.ToString();
			}
		}

		/// <summary>
		/// Get the location information as a string.
		/// </summary>
		/// <returns> A string with location info, or null
		/// if there is no location information. </returns>
		public virtual string LocationAsString
		{
			get
			{
    
				if (null != locator)
				{
					StringBuilder sbuffer = new StringBuilder();
					string systemID = locator.SystemId;
					int line = locator.LineNumber;
					int column = locator.ColumnNumber;
    
					if (null != systemID)
					{
						sbuffer.Append("; SystemID: ");
						sbuffer.Append(systemID);
					}
    
					if (0 != line)
					{
						sbuffer.Append("; Line#: ");
						sbuffer.Append(line);
					}
    
					if (0 != column)
					{
						sbuffer.Append("; Column#: ");
						sbuffer.Append(column);
					}
    
					return sbuffer.ToString();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Print the the trace of methods from where the error
		/// originated.  This will trace all nested exception
		/// objects, as well as this object.
		/// </summary>
		public virtual void printStackTrace()
		{
			printStackTrace(new java.io.PrintWriter(System.err, true));
		}

		/// <summary>
		/// Print the the trace of methods from where the error
		/// originated.  This will trace all nested exception
		/// objects, as well as this object. </summary>
		/// <param name="s"> The stream where the dump will be sent to. </param>
		public virtual void printStackTrace(java.io.PrintStream s)
		{
			printStackTrace(new java.io.PrintWriter(s));
		}

		/// <summary>
		/// Print the the trace of methods from where the error
		/// originated.  This will trace all nested exception
		/// objects, as well as this object. </summary>
		/// <param name="s"> The writer where the dump will be sent to. </param>
		public virtual void printStackTrace(java.io.PrintWriter s)
		{

			if (s == null)
			{
				s = new java.io.PrintWriter(System.err, true);
			}

			try
			{
				string locInfo = LocationAsString;

				if (null != locInfo)
				{
					s.println(locInfo);
				}

				base.printStackTrace(s);
			}
			catch (Exception)
			{
			}

			bool isJdk14OrHigher = false;
			try
			{
				typeof(Exception).GetMethod("getCause",null);
				isJdk14OrHigher = true;
			}
			catch (NoSuchMethodException)
			{
				// do nothing
			}

			// The printStackTrace method of the Throwable class in jdk 1.4 
			// and higher will include the cause when printing the backtrace.
			// The following code is only required when using jdk 1.3 or lower                
			if (!isJdk14OrHigher)
			{
				Exception exception = Exception;

				for (int i = 0; (i < 10) && (null != exception); i++)
				{
					s.println("---------");

					try
					{
						if (exception is DTMException)
						{
							string locInfo = ((DTMException) exception).LocationAsString;

							if (null != locInfo)
							{
								s.println(locInfo);
							}
						}

						exception.printStackTrace(s);
					}
					catch (Exception)
					{
						s.println("Could not print stack trace...");
					}

					try
					{
						Method meth = ((object) exception).GetType().GetMethod("getException", null);

						if (null != meth)
						{
							Exception prev = exception;

							exception = (Exception) meth.invoke(exception, null);

							if (prev == exception)
							{
								break;
							}
						}
						else
						{
							exception = null;
						}
					}
					catch (InvocationTargetException)
					{
						exception = null;
					}
					catch (IllegalAccessException)
					{
						exception = null;
					}
					catch (NoSuchMethodException)
					{
						exception = null;
					}
				}
			}
		}
	}

}