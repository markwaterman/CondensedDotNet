/* Copyright 2016 Mark Waterman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
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

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Condensed
{
    /// <summary>
    /// An exception that is thrown when the internal state of a <see cref="DedupedList{T}"/> is
    /// irrevocably corrupted, typically caused by unsynchronized multithreaded accesses.
    /// </summary>
    /// <remarks>
    /// This exception cannot be recovered from, and trying to handle it may cause further corruption to the
    /// state of your application--consider calling
    /// <see cref="Environment.FailFast(string, Exception)"/> if it is encountered.
    /// </remarks>
    [Serializable]
    public class InternalCorruptionException : Exception
    {
        /// <summary>
        /// Default error message.
        /// </summary>
        private const string DefaultErrorMessage = "An unrecoverable inconsistency in the DedupedList's internal reference counting was detected. A typical cause is earlier multi-threaded access to the collection where a modification was made without an exclusive lock.";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InternalCorruptionException() : this (DefaultErrorMessage, null)
        {
        }

        /// <summary>
        /// Constructs a new instance of the exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InternalCorruptionException(string message) : this (message, null)
        {
        }

        /// <summary>
        /// Constructs a new instance of the exception.
        /// </summary>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InternalCorruptionException(string message, Exception inner) : base (message, inner)
        {
        }

#if !PORTABLE
        /// <summary>
        /// Initializes a new instance of the InternalCorruptionException class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected InternalCorruptionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            :base(info, context)
        {
        }
#endif


    }
}
