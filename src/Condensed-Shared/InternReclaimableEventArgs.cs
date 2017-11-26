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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Condensed
{
    /// <summary>
    /// Event args provided to a <see cref="DedupedList{T}.InternedValueReclaimable"/> event handler, intended
    /// to help decide whether a cleanup operation should be performed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Set the <see cref="InternReclaimableEventArgs.Cleanup"/> property to <c>true</c> in your event handler 
    /// to have the collection perform a cleanup operation when the event handler completes.
    /// </para><para>
    /// Cleanups are expensive and typically should not be performed every time 
    /// the event is raised; consider cleaning up only when the <see cref="InternReclaimableEventArgs.InternPoolCount"/>
    /// is significantly larger than the <see cref="InternReclaimableEventArgs.UniqueCount"/>.
    /// </para>
    /// </remarks>
    /// <conceptualLink target="606626e5-fb28-47c5-939f-a87c14d4f99a" />
    public class InternReclaimableEventArgs : EventArgs
    {
        /// <summary>
        /// Count of items currently in the collection.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Count of unique items currently in the collection.
        /// </summary>
        public int UniqueCount { get; private set; }

        /// <summary>
        /// Count of items currently in the collection's intern pool.
        /// </summary>
        public int InternPoolCount { get; private set; }

        /// <summary>
        /// Count of unused items in the collection's intern pool that 
        /// would be freed (made eligible for GC) by a cleanup operation.
        /// </summary>
        public int ReclaimableInternsCount { get; private set; }

        /// <summary>
        /// Gets/sets a value indicating whether the collection should be cleaned up after the event handler completes. 
        /// Default is <c>false</c>.
        /// </summary>
		/// <remarks>
        /// <para>
        /// Set the <see cref="InternReclaimableEventArgs.Cleanup"/> property to <c>true</c> in your event handler 
        /// to have the collection perform a cleanup operation when the event handler completes.
        /// </para><para>
        /// Cleanups are expensive and typically should not be performed every time 
        /// the event is raised; consider cleaning up only when the <see cref="InternReclaimableEventArgs.InternPoolCount"/>
        /// is significantly larger than the <see cref="InternReclaimableEventArgs.UniqueCount"/>.
        /// </para>
        /// </remarks>
        /// <conceptualLink target="606626e5-fb28-47c5-939f-a87c14d4f99a" />
        public bool Cleanup { get; set; }

        internal InternReclaimableEventArgs(int count, int uniqueCount, int internCount, int reclaimableInternsCount)
        {
            Count = count;
            UniqueCount = uniqueCount;
            InternPoolCount = internCount;
            ReclaimableInternsCount = reclaimableInternsCount;
            Cleanup = false;
        }
    }
}
