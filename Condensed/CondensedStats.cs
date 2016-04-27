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
    /// Statistics provided to a cutover predicate.
    /// </summary>
    /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
    public struct CondensedStats
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

        internal CondensedStats(int count, int uniqueCount, int internCount)
        {
            Count = count;
            UniqueCount = uniqueCount;
            InternPoolCount = internCount;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The first value to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is CondensedStats))
                return false;

            return Equals((CondensedStats)obj);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The value to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public bool Equals(CondensedStats other)
        {
            if (other.Count == this.Count &&
                other.UniqueCount == this.UniqueCount &&
                other.InternPoolCount == this.InternPoolCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines whether two specified CondensedStats values are equal.
        /// </summary>
        /// <param name="stats1">The first value to compare.</param>
        /// <param name="stats2">The second value to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public static bool operator ==(CondensedStats stats1, CondensedStats stats2)
        {
            return stats1.Equals(stats2);
        }


        /// <summary>
        /// Determines whether two specified CondensedStats values are not equal.
        /// </summary>
        /// <param name="stats1">The first value to compare.</param>
        /// <param name="stats2">The second value to compare.</param>
        /// <returns>true if the objects are not considered equal; otherwise, false.</returns>
        public static bool operator !=(CondensedStats stats1, CondensedStats stats2)
        {
            return !stats1.Equals(stats2);
        }


        /// <summary>
        /// Gets the hash of the value
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return unchecked((Count ^ UniqueCount) ^ InternPoolCount);
        }
    }
}
