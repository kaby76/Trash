//
// Debug.cs
//
// Copyright © Stephen Quattlebaum 2003
//
// You may use this code as you see fit.  No warranty is granted or implied
// for its suitability for use.  I'd appreciate it if you credited the
// original author if you use this code.
//

using System;

namespace LspTools.LspHelpers
{
    /// <summary>
    /// Exception thrown whenever certain <see cref="ValueCheck"/> assertions
    /// fail.
    /// </summary>
    public class AssertFailedException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="AssertFailedException"/> object.
        /// </summary>
        /// <param name="message">
        /// The message to assign to the <see cref="Exception.Message"/>
        /// property.
        /// </param>
        public AssertFailedException(string message)
            : base(message)
        { }

        /// <summary>
        /// Constructs a new <see cref="AssertFailedException"/> object.
        /// </summary>
        /// <param name="message">
        /// The message to assign to the <see cref="Exception.Message"/>
        /// property.
        /// </param>
        /// <param name="innerException">
        /// The exception that caused this exception to occur.
        /// </param>
        public AssertFailedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}