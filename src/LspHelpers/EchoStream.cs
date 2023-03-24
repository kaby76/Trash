//
// EchoStream.cs
//
// Copyright © Stephen Quattlebaum 2003
//
// You may use this code as you see fit.  No warranty is granted or implied
// for its suitability for use.  I'd appreciate it if you credited the
// original author if you use this code.
//

using System;
using System.Diagnostics;
using System.IO;

namespace LspTools.LspHelpers
{
    /// <summary>
    /// A subclass of <see cref="Stream"/> that echos values read from or
    /// written to a given primary stream to a given slave stream.  In the
    /// write-only case, this functionality is similar to what is often called
    /// a "tee" stream.
    /// </summary>
    public class EchoStream : Stream
    {
        #region StreamOwnership Enumeration

        /// <summary>
        /// Flags that specify the degree of ownership that the echo stream
        /// exerts over its primary and slave streams.  Ownership determines
        /// whether the echo stream closes its consituent streams whenever
        /// it itself is closed.
        /// </summary>
        [Flags]
        public enum StreamOwnership
        {
            /// <summary>
            /// The <see cref="EchoStream"/> does not own either of its
            /// constituent streams, and will close neither of them whenever
            /// the <see cref="EchoStream"/> is closed.
            /// </summary>
            OwnNone = 0x0,

            /// <summary>
            /// The <see cref="EchoStream"/> owns its primary stream, and will
            /// close it whenever the <see cref="EchoStream"/> is closed.
            /// </summary>
            OwnPrimaryStream = 0x1,

            /// <summary>
            /// The <see cref="EchoStream"/> owns its slave stream, and will
            /// close it whenever the <see cref="EchoStream"/> is closed.
            /// </summary>
            OwnSlaveStream = 0x2,

            /// <summary>
            /// The <see cref="EchoStream"/> owns both of its constituent
            /// streams, and will close both whenever the
            /// <see cref="EchoStream"/> is closed.  This is equivalent to
            /// <c><see cref="OwnPrimaryStream"/> | <see cref="OwnSlaveStream"/>
            /// </c>.
            /// </summary>
            OwnBoth = OwnPrimaryStream | OwnSlaveStream
        }

        #endregion // StreamOwnership Enumeration

        #region SlaveFailAction Enumeration

        /// <summary>
        /// Specifies the action that will be taken whenever a failure occurs in
        /// reading or writing the slave stream.  Failures that occur when reading
        /// or writing the primary stream are always propogated back to the caller
        /// as the original exception that was thrown.
        /// </summary>
        public enum SlaveFailAction
        {
            /// <summary>
            /// The failure will be propogated to the client as the original
            /// exception that was thrown by the underlying stream.  This is
            /// the default action for all failures, and is the most efficient,
            /// as <see cref="Read"/> and <see cref="Write"/> operations never
            /// have to enter an expensive try block to implement this behavior.
            /// </summary>
            Propogate,

            /// <summary>
            /// The failure will be ignored.  Any exceptions that are thrown will
            /// be silently swallowed.  This is a less efficient option than
            /// <see cref="Propogate"/>, but it allows for simple robustness in
            /// the face of slave stream failures.  It does not affect behavior
            /// for primary stream failures.
            /// </summary>
            Ignore,

            /// <summary>
            /// The failure will be passed on to an error handler delegate that
            /// determines how to handle it.  The performance hit for this option
            /// is equivalent to that of <see cref="Ignore"/>, which is to
            /// say that it is slower than <see cref="Propogate"/>.  This cannot
            /// be set directly; it is set automatically whenever you use the
            /// <see cref="SlaveReadFailFilter"/> or
            /// <see cref="SlaveWriteFailFilter"/> delegates.
            /// </summary>
            Filter
        }

        #endregion // SlaveFailAction Enumeration

        #region SlaveFailHandler Delegate

        /// <summary>
        /// Delegate for slave-failure filters used by <see cref="EchoStream"/>.
        /// The oSender parameter is the <see cref="EchoStream"/> in which the
        /// error occurred.  The method parameter identifies whether this is a
        /// read, write or seek failure.  The exc parameter contains the
        /// exception that occurred while attempting to read or write the
        /// slave stream.  The delegate should return a
        /// <see cref="SlaveFailAction"/> that instructs the caller on how to
        /// proceed with the exception.  Note that
        /// <see cref="SlaveFailAction.Filter"/> is not a valid return value
        /// from a fail handler, and will cause an
        /// <see cref="InvalidOperationException"/> to occur.
        /// </summary>
        public delegate SlaveFailAction SlaveFailHandler(
            object oSender, SlaveFailMethod method, Exception exc
        );

        /// <summary>
        /// Identifies the method in which a slave failure occurred.
        /// </summary>
        public enum SlaveFailMethod
        {
            /// <summary>
            /// Failure occurred in the <see cref="Read"/> method.
            /// </summary>
            Read,

            /// <summary>
            /// Failure occurred in the <see cref="Write"/> method.
            /// </summary>
            Write,

            /// <summary>
            /// Failure occurred in the <see cref="Seek"/> method.
            /// </summary>
            Seek
        }

        #endregion // SlaveFailHandler Delegate

        #region Private Implementation Variables

        private readonly Stream m_primaryStream;
        private readonly Stream m_slaveStream;
        private StreamOwnership m_streamsOwned;

        private SlaveFailAction m_readFailAction = SlaveFailAction.Propogate;
        private SlaveFailAction m_writeFailAction = SlaveFailAction.Propogate;
        private SlaveFailAction m_seekFailAction = SlaveFailAction.Propogate;

        private SlaveFailHandler m_slaveReadFailFilter;
        private SlaveFailHandler m_slaveWriteFailFilter;
        private SlaveFailHandler m_slaveSeekFailFilter;

        private int m_lastReadResult;

        #endregion // Private Implementation Variables

        #region Construction / Destruction

        /// <summary>
        /// Constructs a new <see cref="EchoStream"/> object.
        /// </summary>
        /// <param name="primaryStream">
        /// The primary stream.  See <see cref="PrimaryStream"/>.
        /// </param>
        /// <param name="slaveStream">
        /// The slave stream.  See <see cref="SlaveStream"/>.
        /// </param>
        /// <param name="streamsOwned">
        /// Controls which streams are "owned" by the <see cref="EchoStream"/>.
        /// See <see cref="StreamsOwned"/>.
        /// </param>
        public EchoStream(
            Stream primaryStream, Stream slaveStream,
            StreamOwnership streamsOwned)
        {
            ValueCheck.AssertNotNullArg("primaryStream", primaryStream);
            ValueCheck.AssertNotNullArg("slaveStream", slaveStream);

            m_primaryStream = primaryStream;
            m_slaveStream = slaveStream;
            m_streamsOwned = streamsOwned;
        }

        /// <summary>
        /// Closes the slave stream and any of its constituent streams that
        /// it owns.  See <see cref="StreamsOwned"/> and
        /// <see cref="Stream.Close"/>.
        /// </summary>
        public override void Close()
        {
            // Flush all data through both streams.
            Flush();

            // Close the streams that we own.
            if ((m_streamsOwned & StreamOwnership.OwnPrimaryStream) > 0)
            {
                m_primaryStream.Close();
            }

            if ((m_streamsOwned & StreamOwnership.OwnSlaveStream) > 0)
            {
                m_slaveStream.Close();
            }

            base.Close();
        }

        #endregion // Construction / Destruction

        #region Non-Error Properties

        /// <summary>
        /// Controls which of the <see cref="EchoStream"/> object's constituent
        /// streams are closed whenever the <see cref="EchoStream"/> itself is
        /// closed.  See <see cref="StreamOwnership"/>.
        /// </summary>
        public StreamOwnership StreamsOwned
        {
            get => m_streamsOwned;
            set => m_streamsOwned = value;
        }

        /// <summary>
        /// Gets the primary stream for the <see cref="EchoStream"/>.  When
        /// data is written to the <see cref="EchoStream"/>, it is written
        /// to the primary stream.  When data is read from the
        /// <see cref="EchoStream"/>, it is read from the primary stream.
        /// </summary>
        public Stream PrimaryStream => m_primaryStream;

        /// <summary>
        /// Gets the slave stream for the <see cref="EchoStream"/>.  When
        /// data is written to the <see cref="EchoStream"/>, it is written
        /// to the slave stream.  When data is read from the
        /// <see cref="EchoStream"/>, however, it is not read from the slave
        /// stream; it is read from the primary stream instead.  Whatever
        /// data is read from the primary stream is then written (echoed) into
        /// the slave stream.
        /// </summary>
        public Stream SlaveStream => m_slaveStream;

        #endregion // Non-Error Properties

        #region Error Handling Properties

        /// <summary>
        /// At all times, this property reflects the return value of the last
        /// call to <see cref="Stream.Read"/> that was made on the primary
        /// stream.
        /// </summary>
        /// <remarks>
        /// The primary stream is read whenever <see cref="Read"/>
        /// is called on this stream.  This property is useful because it
        /// provides a way to tell how much data was successfully read from
        /// the primary stream in the case where a subsequent echo to the
        /// slave stream caused an exception and the
        /// <see cref="SlaveWriteFailAction"/> setting for the object allowed
        /// the exception to propogate out of <see cref="Read"/>, effectively
        /// losing the return value.  A caller can use the more efficient
        /// <see cref="SlaveFailAction.Propogate"/> setting and still have
        /// robust behavior at the cost of more complex client code and the
        /// use of this property to recover from reads that failed because of
        /// a downed slave stream.
        /// </remarks>
        public int LastReadResult => m_lastReadResult;

        /// <summary>
        /// Sets the action to use for all possible failures.  It is more
        /// maintainable to use this property whenever you want to handle
        /// all possible slave exceptions in the same way for a stream,
        /// because you do not need to modify your code later if new
        /// exception-related properties are added to <see cref="EchoStream"/>.
        /// See <see cref="SlaveReadFailAction"/> and the other properties
        /// like it.
        /// </summary>
        public SlaveFailAction SlaveFailActions
        {
            set
            {
                SlaveReadFailAction = value;
                SlaveWriteFailAction = value;
                SlaveSeekFailAction = value;
            }
        }

        /// <summary>
        /// Sets the filter to use for all possible failures.  It is more
        /// maintainable to use this property whenever you want to handle
        /// all possible slave exceptions in the same way for a stream,
        /// because you do not need to modify your code later if new
        /// exception-related properties are added to <see cref="EchoStream"/>.
        /// See <see cref="SlaveReadFailAction"/> and the other properties
        /// like it.
        /// </summary>
        public SlaveFailHandler SlaveFailFilters
        {
            set
            {
                SlaveReadFailFilter = value;
                SlaveWriteFailFilter = value;
                SlaveSeekFailFilter = value;
            }
        }

        /// <summary>
        /// Controls what action is taken whenever a failure occurs while
        /// trying to echo data read from the primary stream back to the
        /// slave stream.  See <see cref="SlaveFailAction"/> and
        /// <see cref="Read"/>.
        /// </summary>
        public SlaveFailAction SlaveReadFailAction
        {
            get => m_readFailAction;

            set
            {
                if (value == SlaveFailAction.Filter)
                {
                    throw new InvalidOperationException(
                        "You cannot set this property to "
                        + "SlaveFailAction.Filter manually.  Use the "
                        + "SlaveReadFailFilter property instead."
                    );
                }
                else
                {
                    // Unset any read filter that may have been set and set
                    // the new read fail behavior.
                    m_slaveReadFailFilter = null;
                    m_readFailAction = value;
                }
            }
        }

        /// <summary>
        /// Controls what action is taken whenever a failure occurs while
        /// trying to write data to the slave stream.  See
        /// <see cref="SlaveFailAction"/> and <see cref="Write"/>.
        /// </summary>
        public SlaveFailAction SlaveWriteFailAction
        {
            get => m_writeFailAction;

            set
            {
                if (value == SlaveFailAction.Filter)
                {
                    throw new InvalidOperationException(
                        "You cannot set this property to "
                        + "SlaveFailAction.Filter manually.  Use the "
                        + "SlaveWriteFailFilter property instead."
                        );
                }
                else
                {
                    // Unset any write filter that may have been set and set
                    // the new write fail behavior.
                    m_slaveWriteFailFilter = null;
                    m_writeFailAction = value;
                }
            }
        }

        /// <summary>
        /// Controls what action is taken whenever a failure occurs while
        /// trying to seek in the slave stream.  See
        /// <see cref="SlaveFailAction"/> and <see cref="Seek"/>.
        /// </summary>
        public SlaveFailAction SlaveSeekFailAction
        {
            get => m_seekFailAction;

            set
            {
                if (value == SlaveFailAction.Filter)
                {
                    throw new InvalidOperationException(
                        "You cannot set this property to "
                        + "SlaveFailAction.Filter manually.  Use the "
                        + "SlaveSeekFailFilter property instead."
                        );
                }
                else
                {
                    // Unset any write filter that may have been set and set
                    // the new write fail behavior.
                    m_slaveSeekFailFilter = null;
                    m_seekFailAction = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the filter delegate that will be called whenever a
        /// failure occurs while trying to while trying to to write data to
        /// the slave stream.  See <see cref="SlaveFailAction"/> and
        /// <see cref="Write"/>.
        /// </summary>
        public SlaveFailHandler SlaveWriteFailFilter
        {
            get => m_slaveWriteFailFilter;

            set
            {
                // The somewhat roundabout way in which this is written helps
                // to preserve the existing fail action in the case where there
                // was no previous fail handler and the user calls this with a
                // null handler (which should just be a no-op from the user's
                // point of view).

                // Reset the fail action to a potentially-temporary default
                // if there was a previous slave (so that the fail action
                // was Filter).
                if (m_slaveWriteFailFilter != null)
                {
                    m_writeFailAction = SlaveFailAction.Propogate;
                }

                m_slaveWriteFailFilter = value;

                // Automatically set the fail action to Filter if we now have
                // a slave filter.
                if (value != null)
                {
                    m_writeFailAction = SlaveFailAction.Filter;
                }
            }
        }

        /// <summary>
        /// Gets and sets the filter delegate that will be called whenever a
        /// failure occurs while trying to echo data read from the primary
        /// stream back to the slave stream.  See <see cref="SlaveFailAction"/>
        /// and <see cref="Read"/>.
        /// </summary>
        public SlaveFailHandler SlaveReadFailFilter
        {
            get => m_slaveReadFailFilter;

            set
            {
                // The somewhat roundabout way in which this is written helps
                // to preserve the existing fail action in the case where there
                // was no previous fail handler and the user calls this with a
                // null handler (which should just be a no-op from the user's
                // point of view).

                // Reset the fail action to a potentially-temporary default
                // if there was a previous slave (so that the fail action
                // was Filter).
                if (m_slaveReadFailFilter != null)
                {
                    m_readFailAction = SlaveFailAction.Propogate;
                }

                m_slaveReadFailFilter = value;

                // Automatically set the fail action to Filter if we now have
                // a slave filter.
                if (value != null)
                {
                    m_readFailAction = SlaveFailAction.Filter;
                }
            }
        }

        /// <summary>
        /// Gets and sets the filter delegate that will be called whenever a
        /// failure occurs while trying to seek within the slave stream.
        /// See <see cref="SlaveFailAction"/> and <see cref="Seek"/>.
        /// </summary>
        public SlaveFailHandler SlaveSeekFailFilter
        {
            get => m_slaveSeekFailFilter;

            set
            {
                // The somewhat roundabout way in which this is written helps
                // to preserve the existing fail action in the case where there
                // was no previous fail handler and the user calls this with a
                // null handler (which should just be a no-op from the user's
                // point of view).

                // Reset the fail action to a potentially-temporary default
                // if there was a previous slave (so that the fail action
                // was Filter).
                if (m_slaveSeekFailFilter != null)
                {
                    m_seekFailAction = SlaveFailAction.Propogate;
                }

                m_slaveSeekFailFilter = value;

                // Automatically set the fail action to Filter if we now have
                // a slave filter.
                if (value != null)
                {
                    m_seekFailAction = SlaveFailAction.Filter;
                }
            }
        }

        #endregion // Error Handling Properties

        #region Stream Implementation

        /// <summary>
        /// Returns the value of CanRead provided by the primary stream.
        /// See <see cref="Stream.CanRead"/>.
        /// </summary>
        public override bool CanRead => m_primaryStream.CanRead;

        /// <summary>
        /// Returns true if CanSeek on both the primary stream and the slave
        /// stream returns true.
        /// </summary>
        public override bool CanSeek => m_primaryStream.CanSeek && m_slaveStream.CanSeek;

        /// <summary>
        /// Returns true if CanWrite on both the primary stream and the slave
        /// stream returns true.  Note that an <see cref="EchoStream"/> whose
        /// <see cref="SlaveStream"/>'s CanWrite returns false is not
        /// very useful.
        /// </summary>
        public override bool CanWrite => m_primaryStream.CanWrite && m_slaveStream.CanWrite;

        /// <summary>
        /// Flushes both constituent streams.  See <see cref="Stream.Flush"/>.
        /// Because Flush is just a special case of delayed <see cref="Write"/>,
        /// this method uses the exception handling framework put in place for
        /// <see cref="Write"/>.  See that method for more information.
        /// </summary>
        public override void Flush()
        {
            m_primaryStream.Flush();

            if (m_writeFailAction == SlaveFailAction.Propogate)
            {
                // This is the simple and most efficient case.
                m_slaveStream.Flush();
            }
            else
            {
                // This is the case that involves more expensive error
                // handling.

                try
                {
                    m_slaveStream.Flush();
                }
                catch (Exception exc)
                {
                    HandleSlaveException(
                        exc, SlaveFailMethod.Write,
                        m_writeFailAction
                    );
                }
            }
        }

        /// <summary>
        /// Gets the length of the stream.  For <see cref="EchoStream"/>, this
        /// is the length of the primary stream, since the slave stream is never
        /// read by the <see cref="EchoStream"/>.
        /// </summary>
        public override long Length => m_primaryStream.Length;

        /// <summary>
        /// Sets the length of the stream.  See <see cref="Stream.SetLength"/>.
        /// This method sets the length of the slave stream relative to the
        /// length that it sets on the primary stream, in the same spirit as
        /// the behavior of the <see cref="Seek"/> and <see cref="Position"/>
        /// members.  It also uses the same error handling mechanism used
        /// by those members.
        /// </summary>
        public override void SetLength(long len)
        {
            long diff = len - m_primaryStream.Length;

            m_primaryStream.SetLength(len);

            if (m_seekFailAction == SlaveFailAction.Propogate)
            {
                m_slaveStream.SetLength(m_slaveStream.Length + diff);
            }
            else
            {
                try
                {
                    m_slaveStream.SetLength(m_slaveStream.Length + diff);
                }
                catch (Exception exc)
                {
                    HandleSlaveException(
                        exc, SlaveFailMethod.Seek, m_seekFailAction
                    );
                }
            }
        }

        /// <summary>
        /// Reads from the primary stream, and echos anything that was read into
        /// the slave stream.  See <see cref="Stream.Read"/>.
        /// </summary>
        /// <remarks>
        /// This method is not exception-safe in the sense that it is possible
        /// that data read from the primary stream might not be echoed to the
        /// slave stream if the slave stream throws an exception.  This reflects
        /// the reality that there is no way to "unread" data from a stream, and
        /// there is no way to know for certain that the slave stream will not
        /// throw an exception before reading from the primary stream.  Because
        /// of this, you may find that your buffer contains good data that has
        /// been read even though an exception is thrown by the slave stream.
        /// To facilitate situations in which it is important to continue
        /// reading even if the slave stream goes down, and still retain good
        /// performance in the common case where both streams must work for
        /// all reads, <see cref="EchoStream"/> supports the
        /// <see cref="SlaveReadFailAction"/>, <see cref="SlaveReadFailFilter"/>
        /// and <see cref="LastReadResult"/>
        /// properties.  See those properties for more information.
        /// </remarks>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Read from the primary stream.
            m_lastReadResult = m_primaryStream.Read(buffer, offset, count);
            if (m_lastReadResult != 0)
            {
                // Echo to the slave stream.
                if (m_readFailAction == SlaveFailAction.Propogate)
                {
                    // This is the simple and most efficent case.
                    m_slaveStream.Write(buffer, offset, m_lastReadResult);
                }
                else
                {
                    // This is the case that involves more expensive error
                    // handling.

                    try
                    {
                        m_slaveStream.Write(buffer, offset, m_lastReadResult);
                    }
                    catch (Exception exc)
                    {
                        HandleSlaveException(
                            exc, SlaveFailMethod.Read, m_readFailAction
                        );
                    }
                }
            }

            return m_lastReadResult;
        }

        /// <summary>
        /// Writes to both the main stream and the slave stream.  See
        /// <see cref="Stream.Write"/>.
        /// </summary>
        /// <remarks>
        /// This method is not exception-safe in the sense that it is possible
        /// that data written to the primary stream might not be also written to
        /// the slave stream if the slave stream throws an exception.  This
        /// reflects the reality that there is no way to "unwrite" data from a
        /// stream, and there is no way to know for certain that the slave
        /// stream will not throw an exception before writing to the primary
        /// stream.  To facilitate situations in which it is important to
        /// continue writing even if the slave stream goes down, and still
        /// retain good performance in the common case where both streams must
        /// work for all writes, <see cref="EchoStream"/> supports the
        /// <see cref="SlaveWriteFailAction"/> and
        /// <see cref="SlaveWriteFailFilter"/>
        /// properties.  See those properties for more information.
        /// </remarks>
        public override void Write(byte[] buffer, int offset, int count)
        {
            m_primaryStream.Write(buffer, offset, count);

            if (m_writeFailAction == SlaveFailAction.Propogate)
            {
                // This is the simple and most efficient case.
                m_slaveStream.Write(buffer, offset, count);
            }
            else
            {
                // This is the case that involves more expensive error
                // handling.

                try
                {
                    m_slaveStream.Write(buffer, offset, count);
                }
                catch (Exception exc)
                {
                    HandleSlaveException(
                        exc, SlaveFailMethod.Write,
                        m_writeFailAction
                    );
                }
            }
        }

        /// <summary>
        /// Gets and sets the current position of the stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For <see cref="EchoStream"/>, this gets the position of the primary
        /// stream.  When the position is set, <see cref="EchoStream"/>
        /// calculates the difference between the old and new positions
        /// in the primary stream, and makes the same relative change in both
        /// streams.  In this way, changing the position of an
        /// <see cref="EchoStream"/> will cause future writes to happen at
        /// the correct place in both constituent streams, even if the two
        /// streams had different amounts of data written to them at some point
        /// in the past (before they were unioned by the echo stream, for
        /// instance).
        /// </para>
        /// <para>
        /// Failures to set the position of the slave stream are handled using
        /// the <see cref="SlaveSeekFailAction"/> and
        /// <see cref="SlaveSeekFailFilter"/> properties.
        /// </para>
        /// </remarks>
        public override long Position
        {
            get => m_primaryStream.Position;

            set
            {
                // The position of the primary stream is set directly.  The
                // position of the slave stream, which tracks that of the
                // primary stream but may have a different actual value based
                // on things that happened to the streams before they were
                // unioned by this class, is calculated based on the requested
                // change in position in the primary stream, rather than being
                // set directly to the given value.

                long diff = value - m_primaryStream.Position;

                m_primaryStream.Position = value;

                if (m_seekFailAction == SlaveFailAction.Propogate)
                {
                    m_slaveStream.Position += diff;
                }
                else
                {
                    try
                    {
                        m_slaveStream.Position += diff;
                    }
                    catch (Exception exc)
                    {
                        HandleSlaveException(
                            exc, SlaveFailMethod.Seek, m_seekFailAction
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Performs a seek on both streams.  The seek is handled in the same
        /// way as a change in the <see cref="Position"/> property in regards
        /// to the relationship between the two constituent streams.  See
        /// <see cref="Position"/> and <see cref="Stream.Seek"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Failures to set the position of the slave stream are handled using
        /// the <see cref="SlaveSeekFailAction"/> and
        /// <see cref="SlaveSeekFailFilter"/> properties, just as with the
        /// <see cref="Position"/> property.
        /// </para>
        /// </remarks>
        public override long Seek(long offset, SeekOrigin origin)
        {
            // This may be a little backwards from usual, but we implement
            // this in terms of our Position property, rather than the other
            // way around, because Position properly calculates changes in
            // both streams for us.
            if (origin == SeekOrigin.Begin)
            {
                Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                Position += offset;
            }
            else if (origin == SeekOrigin.End)
            {
                Position = Length + offset;
            }

            return Position;
        }

        #endregion // Stream Implementation

        #region Private Implementation Methods

        private void FilterException(Exception exc, SlaveFailMethod method)
        {
            // Allow a user-provided filter function to
            // handle the errors.
            SlaveFailAction action = SlaveFailAction.Filter;

            if (method == SlaveFailMethod.Read)
            {
                action = m_slaveReadFailFilter(this, method, exc);
            }
            else if (method == SlaveFailMethod.Write)
            {
                action = m_slaveWriteFailFilter(this, method, exc);
            }
            else if (method == SlaveFailMethod.Seek)
            {
                action = m_slaveSeekFailFilter(this, method, exc);
            }
            else
            {
                Debug.Assert(false, "Unhandled SlaveFailMethod.");
            }

            if (action == SlaveFailAction.Filter)
            {
                throw new InvalidOperationException(
                    "SlaveFailAction.Filter is not a valid return "
                    + "value for the ReadFailFilter delegate.",
                    exc
                );
            }

            // Handle the exception in the manner specified by the filter.
            // This will always be an indirect recursive call into
            // HandleSlaveException since this method is always called by
            // HandleSlaveException itself, but this time the action can't
            // possibly be 'Filter' (see above), so there can be no
            // infinite recursion.
            HandleSlaveException(exc, method, action);
        }

        private void HandleSlaveException(
            Exception exc, SlaveFailMethod method, SlaveFailAction action)
        {
            if (action == SlaveFailAction.Propogate)
            {
                throw exc;
            }
            else if (action == SlaveFailAction.Ignore)
            {
                // Intentionally Empty
            }
            else if (action == SlaveFailAction.Filter)
            {
                FilterException(exc, method);
            }
            else
            {
                Debug.Assert(false, "Unhandled SlaveFailAction");
            }
        }

        #endregion // Private Implementation Methods
    }
}