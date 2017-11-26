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
    /// Provides a set of extension methods for querying <see cref="DedupedList{T}"/> objects.
    /// </summary>
    public static partial class DedupedListExtensions
    {
        /// <summary>
        /// Computes the sum of a sequence of Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int32 values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int32.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static int Sum(this DedupedList<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            int sum = 0;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int32 values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{int?})"/> implementation.
        /// </remarks>
        public static int? Sum(this DedupedList<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            int sum = 0;

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

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int32.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int})"/> implementation.
        /// </remarks>
        public static int Sum<TSource>(this DedupedList<TSource> source, Func<TSource, int> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            int sum = 0;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of nullable Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int32.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, int?})"/> implementation.
        /// </remarks>
        public static int? Sum<TSource>(this DedupedList<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            int sum = 0;
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
                    }
                }
            }

            return sum;
        }



        /// <summary>
        /// Computes the sum of a sequence of Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Decimal values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Decimal.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{decimal})"/> implementation.
        /// </remarks>
        public static Decimal Sum(this DedupedList<Decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            decimal sum = 0M;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Decimal values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Decimal.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{decimal?})"/> implementation.
        /// </remarks>
        public static Decimal? Sum(this DedupedList<Decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            decimal sum = 0M;

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

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Decimal.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/> implementation.
        /// </remarks>
        public static Decimal Sum<TSource>(this DedupedList<TSource> source, Func<TSource, Decimal> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            decimal sum = 0M;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of nullable Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Decimal.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, decimal?})"/> implementation.
        /// </remarks>
        public static Decimal? Sum<TSource>(this DedupedList<TSource> source, Func<TSource, Decimal?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            decimal sum = 0M;
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
                    }
                }
            }

            return sum;
        }




        /// <summary>
        /// Computes the sum of a sequence of Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int64 values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{long})"/> implementation.
        /// </remarks>
        public static long Sum(this DedupedList<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int64 values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{long?})"/> implementation.
        /// </remarks>
        public static long? Sum(this DedupedList<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
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
                    }
                }
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, long})"/> implementation.
        /// </remarks>
        public static long Sum<TSource>(this DedupedList<TSource> source, Func<TSource, long> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            long sum = 0L;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of nullable Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, long?})"/> implementation.
        /// </remarks>
        public static long? Sum<TSource>(this DedupedList<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            long sum = 0L;
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
                    }
                }
            }

            return sum;
        }


        /// <summary>
        /// Computes the sum of a sequence of double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of double values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{double})"/> implementation.
        /// </remarks>
        public static double Sum(this DedupedList<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable double values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{double?})"/> implementation.
        /// </remarks>
        public static double? Sum(this DedupedList<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
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

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, double})"/> implementation.
        /// </remarks>
        public static double Sum<TSource>(this DedupedList<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            double sum = 0D;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of nullable double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, double?})"/> implementation.
        /// </remarks>
        public static double? Sum<TSource>(this DedupedList<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            double sum = 0D;
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
                    }
                }
            }

            return sum;
        }




        /// <summary>
        /// Computes the sum of a sequence of float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of float values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{float})"/> implementation.
        /// </remarks>
        public static float Sum(this DedupedList<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            float sum = 0F;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (kvp.Key * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a sequence of nullable float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable float values to calculate the sum of.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum(IEnumerable{float?})"/> implementation.
        /// </remarks>
        public static float? Sum(this DedupedList<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            float sum = 0F;

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

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, float})"/> implementation.
        /// </remarks>
        public static float Sum<TSource>(this DedupedList<TSource> source, Func<TSource, float> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            float sum = 0F;

            // we want to throw OverflowExceptions here like the normal LINQ implementation, so
            // get the sum in a checked block just in case we go over sum's MaxValue.
            checked
            {
                foreach (var kvp in source._objToInternLookup)
                    sum += (selector(kvp.Key) * kvp.Value.RefCount);
            }

            return sum;
        }

        /// <summary>
        /// Computes the sum of a DedupedList of nullable float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to calculate the sum of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The sum of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Sum() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Sum{TSource}(IEnumerable{TSource}, Func{TSource, float?})"/> implementation.
        /// </remarks>
        public static float? Sum<TSource>(this DedupedList<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector", "selector cannot be null");

            float sum = 0F;
            float? val = 0F;

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
                    }
                }
            }

            return sum;
        }
    }
}
