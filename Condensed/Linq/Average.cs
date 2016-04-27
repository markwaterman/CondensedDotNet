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

namespace Condensed.Linq
{
    /// <summary>
    /// Provides a set of extension methods for querying <see cref="CondensedCollection{T}"/> objects.
    /// </summary>
    public static partial class CondensedCollectionExtensions
    {
        /// <summary>
        /// Computes the average of a sequence of Int32 values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of Int32 values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static double Average(this CondensedCollection<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return (double)sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable Int32 values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of nullable Int32 values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{int?})"/> implementation.
        /// </remarks>
        public static double? Average(this CondensedCollection<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            long count = 0L;
            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    if (kvp.Key != null)
                    {
                        sum += (kvp.Key.GetValueOrDefault() * kvp.Value.RefCount);
                        count += kvp.Value.RefCount;
                    }
                }
            }

            if (count == 0)
                return null;
            else
                return (double)sum / count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, int})"/> implementation.
        /// </remarks>
        public static double Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, int> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");
            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return (double)sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of nullable Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, int?})"/> implementation.
        /// </remarks>
        public static double? Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            long sum = 0L;
            int valCount = 0;
            int? val = 0;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    val = selector(kvp.Key);
                    if (val.HasValue)
                    {
                        sum += (val.Value * kvp.Value.RefCount);
                        valCount += kvp.Value.RefCount;
                    }
                }
            }

            if (valCount == 0)
                return null;
            else
                return (double)sum / valCount;
        }



        /// <summary>
        /// Computes the average of a sequence of Decimal values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of Decimal values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{decimal})"/> implementation.
        /// </remarks>
        public static Decimal Average(this CondensedCollection<Decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            decimal sum = 0M;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable Decimal values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of nullable Decimal values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{decimal?})"/> implementation.
        /// </remarks>
        public static Decimal? Average(this CondensedCollection<Decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            decimal sum = 0M;
            long count = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    if (kvp.Key != null)
                    {
                        sum += (kvp.Key.GetValueOrDefault() * kvp.Value.RefCount);
                        count += kvp.Value.RefCount;
                    }

                }
            }

            if (count == 0)
                return null;
            else
                return sum / count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/> implementation.
        /// </remarks>
        public static Decimal Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, Decimal> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");
            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            decimal sum = 0M;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of nullable Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, decimal?})"/> implementation.
        /// </remarks>
        public static Decimal? Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, Decimal?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            decimal sum = 0M;
            int valCount = 0;
            decimal? val = 0;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    val = selector(kvp.Key);
                    if (val.HasValue)
                    {
                        sum += (val.Value * kvp.Value.RefCount);
                        valCount += kvp.Value.RefCount;
                    }
                }
            }

            if (valCount == 0)
                return null;
            else
                return sum / valCount;
        }




        /// <summary>
        /// Computes the average of a sequence of Int64 values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of Int64 values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{long})"/> implementation.
        /// </remarks>
        public static double Average(this CondensedCollection<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return (double)sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable Int64 values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of nullable Int64 values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{long?})"/> implementation.
        /// </remarks>
        public static double? Average(this CondensedCollection<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            long sum = 0L;
            long count = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    if (kvp.Key != null)
                    {
                        sum += (kvp.Key.GetValueOrDefault() * kvp.Value.RefCount);
                        count += kvp.Value.RefCount;
                    }
                }
            }

            return (double)sum / count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, long})"/> implementation.
        /// </remarks>
        public static double Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, long> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");
            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return (double)sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of nullable Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, long?})"/> implementation.
        /// </remarks>
        public static double? Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            long sum = 0L;
            int valCount = 0;
            long? val = 0;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    val = selector(kvp.Key);
                    if (val.HasValue)
                    {
                        sum += (val.Value * kvp.Value.RefCount);
                        valCount += kvp.Value.RefCount;
                    }
                }
            }

            if (valCount == 0)
                return null;
            else
                return (double)sum / valCount;
        }


        /// <summary>
        /// Computes the average of a sequence of double values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of double values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{double})"/> implementation.
        /// </remarks>
        public static double Average(this CondensedCollection<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a sequence of nullable double values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of nullable double values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{double?})"/> implementation.
        /// </remarks>
        public static double? Average(this CondensedCollection<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            long count = 0L;
            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
               
                foreach (var kvp in source._objToInternLookup)
                {
                    if (kvp.Key != null)
                    {
                        sum += (kvp.Key.GetValueOrDefault() * kvp.Value.RefCount);
                        count += kvp.Value.RefCount;
                    }
                }
            }

            if (count == 0)
                return null;
            else
                return sum / count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, double})"/> implementation.
        /// </remarks>
        public static double Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");
            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum / source.Count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of nullable double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, double?})"/> implementation.
        /// </remarks>
        public static double? Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            double sum = 0D;
            int valCount = 0;
            double? val = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    val = selector(kvp.Key);
                    if (val.HasValue)
                    {
                        sum += (val.Value * kvp.Value.RefCount);
                        valCount += kvp.Value.RefCount;
                    }
                }
            }

            if (valCount == 0)
                return null;
            else
                return sum / valCount;
        }




        /// <summary>
        /// Computes the average of a sequence of float values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of float values to calculate the average of.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{float})"/> implementation.
        /// </remarks>
        public static float Average(this CondensedCollection<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return (float)(sum / source.Count);
        }

        /// <summary>
        /// Computes the average of a sequence of nullable float values. Optimized for a CondensedCollection.
        /// </summary>
        /// <param name="source">A CondensedCollection of nullable float values to calculate the average of.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average(IEnumerable{float?})"/> implementation.
        /// </remarks>
        public static float? Average(this CondensedCollection<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average();

            long count = 0L;
            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    if (kvp.Key != null)
                    {
                        sum += (kvp.Key.GetValueOrDefault() * kvp.Value.RefCount);
                    }
                }
            }

            if (count == 0)
                return null;
            else
                return (float)sum / count;
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, float})"/> implementation.
        /// </remarks>
        public static float Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, float> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");
            if (source.Count == 0) throw new InvalidOperationException("source contains no elements");

            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return (float)(sum / source.Count);
        }

        /// <summary>
        /// Computes the average of a CondensedCollection of nullable float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a CondensedCollection.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A CondensedCollection of values to calculate the average of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Average() is specialized for a CondensedCollection and tries to
        /// take advantage of the CondensedCollection's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the CondensedCollection to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Average{TSource}(IEnumerable{TSource}, Func{TSource, float?})"/> implementation.
        /// </remarks>
        public static float? Average<TSource>(this CondensedCollection<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source.HasCutover)
                return source._unindexedValues.Average(selector);

            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            double sum = 0D;
            int valCount = 0;
            double? val = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                {
                    val = selector(kvp.Key);
                    if (val.HasValue)
                    {
                        sum += (val.Value * kvp.Value.RefCount);
                        valCount += kvp.Value.RefCount;
                    }
                }
            }

            if (valCount == 0)
                return null;
            else
                return (float)(sum / valCount);
        }
    }
}
