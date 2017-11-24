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
    /// Contains cutover predicates that implement reasonable heuristics that can be used for common types
    /// when deciding whether a CondensedCollection should stop performing deduplication and just store values
    /// in an ordinary <see cref="List{T}"/>.
    /// </summary>
    /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
    public static class StandardCutoverPredicates
    {
        // empirically, I'm looking at about 72 bytes of overhead for each unique value.
        const int OVERHEAD_PER_UNIQUE_ITEM = 72;

        /// <summary>
        /// Convenience method that returns a cutover predicate for the specified type.
        /// </summary>
        /// <param name="type">Type of object being stored in a <see cref="CondensedCollection{T}"/>.</param>
        /// <returns>Default predicate used for the type.</returns>
        public static Predicate<CondensedStats> GetDefaultCutoverPredicate(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(bool))
                return StandardCutoverPredicates.NeverCutover;
            else if (type == typeof(byte) ||
                     type == typeof(char))
                return StandardCutoverPredicates.BytePredicate;
            else if (type == typeof(short) ||
                     type == typeof(ushort))
                return StandardCutoverPredicates.ShortPredicate;
            else if (type == typeof(int) ||
                     type == typeof(uint) ||
                     type == typeof(float))
                return StandardCutoverPredicates.IntPredicate;
            else if (type == typeof(long) ||
                     type == typeof(ulong) ||
                     type == typeof(double) ||
                     type == typeof(DateTime) ||
                     type == typeof(DateTimeOffset) || // well, 10 bytes instead of 8, but close enough
                     type == typeof(TimeSpan))
                return StandardCutoverPredicates.LongPredicate;
            else if (type == typeof(Decimal) ||
                     type == typeof(Guid))
                return StandardCutoverPredicates.DecimalPredicate;
            else if (type == typeof(string))
                return StandardCutoverPredicates.StringPredicate;
            else
                throw new NotSupportedException("No default predicate for type " + type.ToString());
        }

        /// <summary>
        /// Cutover predicate to use with single-byte data types.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool BytePredicate(CondensedStats stats)
        {
            if (stats.UniqueCount >= 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cutover predicate to use with two-byte data types.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool ShortPredicate(CondensedStats stats)
        {
            
            if (stats.UniqueCount > byte.MaxValue)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cutover predicate to use with four-byte data types.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool IntPredicate(CondensedStats stats)
        {
            
            if (stats.UniqueCount > ushort.MaxValue)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Cutover predicate to use with eight-byte data types.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool LongPredicate(CondensedStats stats)
        {
            
            if (stats.UniqueCount < 65536)
                return false;
            else if (stats.UniqueCount == 65536 && stats.Count == 65536)
                return true;
            else if ((8 * stats.Count) < (((OVERHEAD_PER_UNIQUE_ITEM + 8) * stats.UniqueCount) + (4 * stats.Count)))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Cutover predicate to use with 16-byte data types.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool DecimalPredicate(CondensedStats stats)
        {
            // decimals are 16 bytes
            if (stats.UniqueCount < 65536)
                return false;
            else if (stats.UniqueCount == 65536 && stats.Count == 65536)
                return true;
            else if ((16 * stats.Count) < (((OVERHEAD_PER_UNIQUE_ITEM + 16) * stats.UniqueCount) + (4 * stats.Count)))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Default cutover predicate to use with strings.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>True if the condensed list to cutover to normal <see cref="List{T}"/> storage.</returns>
        public static bool StringPredicate(CondensedStats stats)
        {
            // assuming a string consumes 40 bytes(?)... that's about 20 bytes going
            // toward string overhead and enough room for about 10 2-byte characters.
            // Anyone expecting bigger strings should use their own custom predicate.

            if (stats.UniqueCount < 65536)
                return false;
            else if (stats.UniqueCount == 65536 && stats.Count == 65536)
                return true;
            else if ((48 * stats.Count) < (((OVERHEAD_PER_UNIQUE_ITEM + 40) * stats.UniqueCount) + (4 * stats.Count)))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Cutover predicate to use when the <see cref="CondensedCollection{T}"/> should never stop performing deduplication.
        /// </summary>
        /// <param name="stats">Current collection statistics (prior to the item that's currently being added).</param>
        /// <returns>Always false.</returns>
        public static bool NeverCutover(CondensedStats stats)
        {
            return false;
        }
    }
}
