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
        /// Computes the maximum value of a sequence of Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int32 values to determine the maximum value of.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static int Max(this DedupedList<int> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a sequence of nullable Int32 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int32 values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{int?})"/> implementation.
        /// </remarks>
        public static int? Max(this DedupedList<int?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, int})"/> implementation.
        /// </remarks>
        public static int Max<TSource>(this DedupedList<TSource> source, Func<TSource, int> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of nullable Int32 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, int?})"/> implementation.
        /// </remarks>
        public static int? Max<TSource>(this DedupedList<TSource> source, Func<TSource, int?> selector)
        {
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }




        /// <summary>
        /// Computes the maximum value of a sequence of Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Decimal values to determine the maximum value of.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{decimal})"/> implementation.
        /// </remarks>
        public static decimal Max(this DedupedList<decimal> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a sequence of nullable Decimal values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Decimal values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{decimal?})"/> implementation.
        /// </remarks>
        public static decimal? Max(this DedupedList<decimal?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, decimal})"/> implementation.
        /// </remarks>
        public static decimal Max<TSource>(this DedupedList<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of nullable Decimal values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, decimal?})"/> implementation.
        /// </remarks>
        public static decimal? Max<TSource>(this DedupedList<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }




        /// <summary>
        /// Computes the maximum value of a sequence of Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of Int64 values to determine the maximum value of.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{long})"/> implementation.
        /// </remarks>
        public static long Max(this DedupedList<long> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
            
        }

        /// <summary>
        /// Computes the maximum value of a sequence of nullable Int64 values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable Int64 values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{long?})"/> implementation.
        /// </remarks>
        public static long? Max(this DedupedList<long?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, long})"/> implementation.
        /// </remarks>
        public static long Max<TSource>(this DedupedList<TSource> source, Func<TSource, long> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of nullable Int64 values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="OverflowException">The sum of the elements in the sequence is larger than Int64.MaxValue.</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, long?})"/> implementation.
        /// </remarks>
        public static long? Max<TSource>(this DedupedList<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }




        /// <summary>
        /// Computes the maximum value of a sequence of double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of double values to determine the maximum value of.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{double})"/> implementation.
        /// </remarks>
        public static double Max(this DedupedList<double> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();

        }

        /// <summary>
        /// Computes the maximum value of a sequence of nullable double values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable double values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{double?})"/> implementation.
        /// </remarks>
        public static double? Max(this DedupedList<double?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, double})"/> implementation.
        /// </remarks>
        public static double Max<TSource>(this DedupedList<TSource> source, Func<TSource, double> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of nullable double values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, double?})"/> implementation.
        /// </remarks>
        public static double? Max<TSource>(this DedupedList<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }



        /// <summary>
        /// Computes the maximum value of a sequence of float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of float values to determine the maximum value of.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{float})"/> implementation.
        /// </remarks>
        public static float Max(this DedupedList<float> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();

        }

        /// <summary>
        /// Computes the maximum value of a sequence of nullable float values. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A DedupedList of nullable float values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{float?})"/> implementation.
        /// </remarks>
        public static float? Max(this DedupedList<float?> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();
        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <exception cref="InvalidOperationException">source contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, float})"/> implementation.
        /// </remarks>
        public static float Max<TSource>(this DedupedList<TSource> source, Func<TSource, float> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));

        }

        /// <summary>
        /// Computes the maximum value of a DedupedList of nullable float values that are obtained by invoking a transform 
        /// function on each element of the input sequence. Optimized for a DedupedList.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A DedupedList of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The maximum value in the sequence, or null if the source sequence is empty or contains only values that are null.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource}(IEnumerable{TSource}, Func{TSource, float?})"/> implementation.
        /// </remarks>
        public static float? Max<TSource>(this DedupedList<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(Enumerable.Select(source._objToInternLookup.Keys, selector));
        }



        /// <summary>
        /// Returns the maximum value in a generic sequence. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>The maximum value in the sequence or, if TSource is reference type, null if source is empty or contains only null values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException">source is a value type contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max(IEnumerable{int})"/> implementation.
        /// </remarks>
        public static TSource Max<TSource>(this DedupedList<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return source._objToInternLookup.Keys.Max();

        }


        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value. Optimized for a DedupedList.
        /// </summary>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <returns>The maximum value in the sequence or, if TSource is reference type, null if source is empty or contains only null values.</returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        /// <exception cref="InvalidOperationException"><typeparamref name="TResult"/> is a value type and the source collection contains no elements</exception>
        /// <remarks>
        /// This implementation of Max() is specialized for a DedupedList and tries to
        /// take advantage of the DedupedList's knowledge of unique values in order to improve performance.
        /// If you would rather use the normal LINQ extension method then cast the DedupedList to an <see cref="IList{T}"/>
        /// to use the <see cref="Enumerable.Max{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/> implementation.
        /// </remarks>
        public static TResult Max<TSource, TResult>(this DedupedList<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Enumerable.Max(source._objToInternLookup.Keys, selector);
        }

    } // class

} // namespace
