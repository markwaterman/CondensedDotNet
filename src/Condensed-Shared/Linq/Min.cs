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
    public static partial class DedupedListExtensions
    {
        /// <summary>
        /// Computes the minimum value of a sequence of Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int32 values to determine the minimum value of.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static int Min(this DedupedList<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }

        /// <summary>
        /// Computes the minimum value of a sequence of nullable Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int32 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{int?})"/> implementation.
        /// </remarks>
        public static int? Min(this DedupedList<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, int})"/> implementation.
        /// </remarks>
        public static int Min<TSource>(this DedupedList<TSource> source, Func<TSource, int> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of nullable Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, int?})"/> implementation.
        /// </remarks>
        public static int? Min<TSource>(this DedupedList<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }


        /// <summary>
        /// Computes the minimum value of a sequence of Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Decimal values to determine the minimum value of.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{decimal})"/> implementation.
        /// </remarks>
        public static decimal Min(this DedupedList<decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }

        /// <summary>
        /// Computes the minimum value of a sequence of nullable Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Decimal values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{decimal?})"/> implementation.
        /// </remarks>
        public static decimal? Min(this DedupedList<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/> implementation.
        /// </remarks>
        public static decimal Min<TSource>(this DedupedList<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of nullable Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, decimal?})"/> implementation.
        /// </remarks>
        public static decimal? Min<TSource>(this DedupedList<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }




        /// <summary>
        /// Computes the minimum value of a sequence of Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int64 values to determine the minimum value of.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{long})"/> implementation.
        /// </remarks>
        public static long Min(this DedupedList<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }

        /// <summary>
        /// Computes the minimum value of a sequence of nullable Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int64 values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{long?})"/> implementation.
        /// </remarks>
        public static long? Min(this DedupedList<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, long})"/> implementation.
        /// </remarks>
        public static long Min<TSource>(this DedupedList<TSource> source, Func<TSource, long> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of nullable Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, long?})"/> implementation.
        /// </remarks>
        public static long? Min<TSource>(this DedupedList<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }




        /// <summary>
        /// Computes the minimum value of a sequence of double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of double values to determine the minimum value of.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{double})"/> implementation.
        /// </remarks>
        public static double Min(this DedupedList<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }

        /// <summary>
        /// Computes the minimum value of a sequence of nullable double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable double values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{double?})"/> implementation.
        /// </remarks>
        public static double? Min(this DedupedList<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, double})"/> implementation.
        /// </remarks>
        public static double Min<TSource>(this DedupedList<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of nullable double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, double?})"/> implementation.
        /// </remarks>
        public static double? Min<TSource>(this DedupedList<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }



        /// <summary>
        /// Computes the minimum value of a sequence of float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of float values to determine the minimum value of.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{float})"/> implementation.
        /// </remarks>
        public static float Min(this DedupedList<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }

        /// <summary>
        /// Computes the minimum value of a sequence of nullable float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable float values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{float?})"/> implementation.
        /// </remarks>
        public static float? Min(this DedupedList<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();
        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, float})"/> implementation.
        /// </remarks>
        public static float Min<TSource>(this DedupedList<TSource> source, Func<TSource, float> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the minimum value of a DedupedList of nullable float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The minimum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, float?})"/> implementation.
        /// </remarks>
        public static float? Min<TSource>(this DedupedList<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }



        /// <summary>
        /// Returns the minimum value in a generic sequence. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>The minimum value in the sequence or, if TSource is reference type, null if source is empty or contains only null values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source is a value type contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Min(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static TSource Min<TSource>(this DedupedList<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Min();

        }


        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>The minimum value in the sequence or, if TSource is reference type, null if source is empty or contains only null values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TResult"/> is a value type and the source collection contains no elements</exception>
        /// <remarks>
        /// This implementation of Min() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/> implementation.
        /// </remarks>
        public static TResult Min<TSource, TResult>(this DedupedList<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Min(source._objToInternLookup.Keys, selector);
        }

    } // class
}
