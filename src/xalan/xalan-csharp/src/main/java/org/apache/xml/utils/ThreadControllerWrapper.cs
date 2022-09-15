using System.Threading;

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
 * $Id: ThreadControllerWrapper.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xml.utils
{
	/// <summary>
	/// A utility class that wraps the ThreadController, which is used
	/// by IncrementalSAXSource for the incremental building of DTM.
	/// </summary>
	public class ThreadControllerWrapper
	{

	  /// <summary>
	  /// The ThreadController pool </summary>
	  private static ThreadController m_tpool = new ThreadController();

	  public static Thread runThread(ThreadStart runnable, int priority)
	  {
		return m_tpool.run(runnable, priority);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void waitThread(Thread worker, Runnable task) throws InterruptedException
	  public static void waitThread(Thread worker, ThreadStart task)
	  {
		m_tpool.waitThread(worker, task);
	  }

	  /// <summary>
	  /// Thread controller utility class for incremental SAX source. Must 
	  /// be overriden with a derived class to support thread pooling.
	  /// 
	  /// All thread-related stuff is in this class.
	  /// </summary>
	  public class ThreadController
	  {

		/// <summary>
		/// Will get a thread from the pool, execute the task
		///  and return the thread to the pool.
		/// 
		///  The return value is used only to wait for completion
		/// 
		/// </summary>
		/// NEEDSDOC <param name="task"> </param>
		/// <param name="priority"> if >0 the task will run with the given priority
		///  ( doesn't seem to be used in xalan, since it's allways the default ) </param>
		/// <returns>  The thread that is running the task, can be used
		///          to wait for completion </returns>
		public virtual Thread run(ThreadStart task, int priority)
		{

		  Thread t = new Thread(task);

		  t.Start();

		  //       if( priority > 0 )
		  //      t.setPriority( priority );
		  return t;
		}

		/// <summary>
		///  Wait until the task is completed on the worker
		///  thread.
		/// </summary>
		/// NEEDSDOC <param name="worker"> </param>
		/// NEEDSDOC <param name="task">
		/// </param>
		/// <exception cref="InterruptedException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void waitThread(Thread worker, Runnable task) throws InterruptedException
		public virtual void waitThread(Thread worker, ThreadStart task)
		{

		  // This should wait until the transformThread is considered not alive.
		  worker.Join();
		}
	  }

	}

}