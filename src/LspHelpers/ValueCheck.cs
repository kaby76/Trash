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
    /// Contains value-checking methods that provide simple,
    /// <see cref="System.Diagnostics.Assert">Assert</see>-like interfaces
    /// to various kinds of value checks.  These methods are not DEBUG-
    /// conditional; they will always be compiled in, so they are suitable
    /// for checking parameter values in library-level code.
    /// </summary>
    public class ValueCheck
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given value is
        /// <c>null</c>.
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="val">The value to check for null</param>
        public static void AssertNotNullArg(string argName, object val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given value
        /// is <c>null</c>.
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="val">The value to check for <c>null</c>.</param>
        /// <param name="message">
        /// A message to include in the exception that is thrown.
        /// </param>
        public static void AssertNotNullArg(
            string argName, object val, string message)
        {
            if (val == null)
            {
                throw new ArgumentNullException(argName, message);
            }
        }

        /// <summary>
        /// Throws <see cref="AssertFailedException"/> if the given
        /// object is null.
        /// </summary>
        /// <param name="val">The value to check for <c>null</c>.</param>
        /// <param name="message">
        /// A message to include in the exception that is thrown.
        /// </param>
        public static void AssertNotNull(object val, string message)
        {
            if (val == null)
            {
                throw new AssertFailedException(message);
            }
        }

        /// <summary>
        /// Throws <see cref="AssertFailedException"/> if the given
        /// condition value is false.
        /// </summary>
        /// <param name="cond">The condition to check.</param>
        /// <param name="message">
        /// A message to include in the exception that is thrown.
        /// </param>
        public static void AssertTrue(bool cond, string message)
        {
            if (!cond)
            {
                throw new AssertFailedException(message);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the given condition
        /// value is false.
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentException"/>.
        /// </param>
        /// <param name="cond">The condition to check.</param>
        /// <param name="message">
        /// A message to include in the exception that is thrown.
        /// </param>
        public static void AssertTrueArg(
            string argName, bool cond, string message)
        {
            if (!cond)
            {
                throw new ArgumentException(message, argName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if the given string value
        /// is null or empty.
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="strValue">The string value to check.</param>
        public static void AssertNonEmptyStringArg(
            string argName, string strValue)
        {
            if (strValue == null || strValue == "")
            {
                throw new ArgumentNullException(
                    argName, NullOrZeroLengthMessage
                );
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given array
        /// is null or empty.
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="arr">The array to check.</param>
        public static void AssertNonEmptyArrayArg(string argName, object[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                throw new ArgumentNullException(
                    argName, NullOrZeroLengthMessage
                );
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> if the given
        /// value is not in the given range (inclusive).
        /// </summary>
        /// <param name="argName">
        /// The value to pass as the argument name to
        /// <see cref="ArgumentNullException"/>.
        /// </param>
        /// <param name="val">The value to check.</param>
        /// <param name="min">The minimum for the value.</param>
        /// <param name="max">The maximum for the value.</param>
        public static void AssertInRangeArg(
            string argName, double val, double min, double max)
        {
            if (val < min || val > max)
            {
                throw new ArgumentOutOfRangeException(argName);
            }
        }

        /// <summary>
        /// Retrieves a string containing "Cannot be null or zero-length.".
        /// </summary>
        public static string NullOrZeroLengthMessage => "Cannot be null or zero-length.";
    }
}