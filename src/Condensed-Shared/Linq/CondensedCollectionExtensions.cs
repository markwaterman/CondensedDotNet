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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Condensed.Linq
{
    public static partial class CondensedCollectionExtensions
    {
        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
        /// <remarks>
        /// This implementation of First() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.First{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static TSource First<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.First(predicate);

            if (source.Count == 0) throw new InvalidOperationException("The source sequence is empty.");

            int firstMatchedIndex = IndexOfFirst(source, predicate);
            if (firstMatchedIndex != -1)
            {
                return source[firstMatchedIndex];
            }
            else
            {
                throw new InvalidOperationException("No element satisfies the condition in predicate.");
            }
        }


        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a default value if
        /// no such element is found. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(T) if source is empty or if no element passes the test specified by predicate; 
        /// otherwise, the first element in source that passes the test specified by predicate.
        /// </returns>
        /// <remarks>
        /// This implementation of FirstOrDefault() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static TSource FirstOrDefault<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.FirstOrDefault(predicate);

            int firstMatchedIndex = IndexOfFirst(source, predicate);
            if (firstMatchedIndex != -1)
            {
                return source[firstMatchedIndex];
            }
            else
            {
                return default(TSource);
            }
        }


        /// <summary>
        /// Finds first match of a predicate, supports First() and FirstOrDefault().
        /// </summary>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>Index of matching item in the condensed list.</returns>
        internal static int IndexOfFirst<TSource>(DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate", "Predicate cannot be null.");
            if (source.HasCutover)
                throw new InvalidOperationException("Internal error: index type not supported.");

            BitArray evaluatedInterns = new BitArray(source._internPool.Count);
            int firstMatch = -1;
            int internsEvaluated = 0;
            int currentInternIndex;
            for (int index = 0; index < source._indexList.Count; ++index)
            {
                currentInternIndex = source._indexList[index];

                // don't bother running the predicate against a value we've already checked:
                if (evaluatedInterns[currentInternIndex])
                    continue;
                else
                    evaluatedInterns.Set(currentInternIndex, true);

                // run the predicate:
                if (predicate(source._internPool[currentInternIndex]))
                {
                    // we found our first match!
                    firstMatch = index;
                    break;
                }

                internsEvaluated++;
                if (internsEvaluated == source._objToInternLookup.Count)
                {
                    // we've checked every unique item. Safe to
                    // drop out of the loop:
                    break;
                }
            }

            return firstMatch;
        }



        /// <summary>
        /// Returns the last element in a sequence that satisfies a specified condition. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The last element in the sequence that passes the test in the specified predicate function.</returns>
        /// <remarks>
        /// This implementation of Last() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Last{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static TSource Last<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Last(predicate);

            if (source.Count == 0) throw new InvalidOperationException("The source sequence is empty.");

            int lastMatchedIndex = IndexOfLast(source, predicate);
            if (lastMatchedIndex < source.Count)
            {
                return source[lastMatchedIndex];
            }
            else
            {
                throw new InvalidOperationException("No element satisfies the condition in predicate.");
            }
        }

        /// <summary>
        /// Returns the last element of the sequence that satisfies a condition or a default value if
        /// no such element is found. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(T) if source is empty or if no element passes the test specified by predicate; 
        /// otherwise, the last element in source that passes the test specified by predicate.
        /// </returns>
        /// <remarks>
        /// This implementation of LastOrDefault() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.LastOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static TSource LastOrDefault<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.LastOrDefault(predicate);

            int lastMatchedIndex = IndexOfLast(source, predicate);
            if (lastMatchedIndex < source.Count)
            {
                return source[lastMatchedIndex];
            }
            else
            {
                return default(TSource);
            }
        }

        /// <summary>
        /// Finds last match of a predicate, supports Last() and LastOrDefault().
        /// </summary>
        /// <param name="source">The collection to return an element from.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>Index of last matching item in the condensed list, or one past the last element (the count) if not found.</returns>
        internal static int IndexOfLast<TSource>(DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate", "Predicate cannot be null.");
            if (source.HasCutover)
                throw new InvalidOperationException("Internal error: index type not supported.");

            BitArray evaluatedInterns = new BitArray(source._internPool.Count);
            int lastMatch = source._indexList.Count;
            int internsEvaluated = 0;
            int currentInternIndex;
            for (int index = source._indexList.Count - 1; index >= 0 ; --index)
            {
                currentInternIndex = source._indexList[index];

                // don't bother running the predicate against a value we've already checked:
                if (evaluatedInterns[currentInternIndex])
                    continue;
                else
                    evaluatedInterns.Set(currentInternIndex, true);

                // run the predicate:
                if (predicate(source._internPool[currentInternIndex]))
                {
                    // we found the last match!
                    lastMatch = index;
                    break;
                }

                internsEvaluated++;
                if (internsEvaluated == source._objToInternLookup.Count)
                {
                    // we've checked every unique item. Safe to
                    // drop out of the loop:
                    break;
                }
            }

            return lastMatch;
        }


        /// <summary>
        /// Determines whether all elements of the collection satisfy a condition. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to evaluate.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><c>true</c> if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This implementation of All() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.All{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static bool All<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.All(predicate);
            if (predicate == null)
                throw new ArgumentNullException("predicate", "Predicate cannot be null.");

            foreach (var item in source._objToInternLookup.Keys)
            {
                if (!predicate(item))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Determines whether any element of the collection satisfies a condition. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to evaluate.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This implementation of Any() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> implementation.
        /// </remarks>
        public static bool Any<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Any(predicate);
            if (predicate == null)
                throw new ArgumentNullException("predicate", "Predicate cannot be null.");

            foreach (var item in source._objToInternLookup.Keys)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Returns count of elements in the DedupedList that satisfy a condition. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the DedupedList.</typeparam>
        /// <param name="source">DedupedList that contains elements to be tested and counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>Count of how many elements in the DedupedList satisfy the condition in the predicate function.</returns>
        public static int Count<TSource>(this DedupedList<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Count(predicate);

            if (predicate == null) throw new ArgumentNullException("predicate", "predicate cannot be null");

            int count = 0;

            foreach (var kvp in source._objToInternLookup)
            {
                if (predicate(kvp.Key))
                {
                    checked
                    {
                        count += kvp.Value.RefCount;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Returns distinct elements from a <see cref="DedupedList{T}"/> as determined by the collection's equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The collection to remove duplicate elements from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the <paramref name="source"/> collection.</returns>
        public static IEnumerable<TSource> Distinct<TSource>(this DedupedList<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Distinct();
            else
                return source._objToInternLookup.Keys;

        }

        /// <summary>
        /// Determines whether a sequence contains a specified value by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList in which to locate a value.</param>
        /// <param name="value">The value to locate in the DedupedList.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>true if the collection contains an element that has the specified value; otherwise, false.</returns>
        public static bool Contains<TSource>(this DedupedList<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            comparer = comparer ?? EqualityComparer<TSource>.Default;

            foreach (var uniqueVal in source._objToInternLookup.Keys)
            {
                if (comparer.Equals(uniqueVal, value))
                    return true;
            }
            return false;
        }
    }
}
