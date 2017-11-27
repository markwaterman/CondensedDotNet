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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Condensed
{
    


    /// <summary>
    /// A sparse array implementation of fixed size. Default values of <typeparamref name="T"/> 
    /// (null/0/default(T), etc... are stored with minimal memory overhead.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the <see cref="Clear(int)"/> method to clear an element and reset it 
    /// back to the default value of <typeparamref name="T"/> and minimize memory
    /// overhead for that element. Manually setting an element to its default value
    /// through the indexer will not result in memory savings.
    /// </para>
    /// <para>
    /// This class implements the <see cref="IList{T}"/> and <see cref="ICollection{T}"/> interfaces, 
    /// but only explicitly. The implementations are provided to improve performance of some LINQ operations.
    /// Members of these interfaces that add, insert, or remove elements throw <see cref="NotSupportedException"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    public class SparseArray<T> : IList<T>
    {
        private SparseArrayGroup<T>[] _groups;
        private static readonly int _groupSize = SparseArrayGroup<T>.GroupSize;

        private int _length = 0;

        /// <summary>
        /// Constructs a SparseArray with the specified length, initializing every
        /// element to the default value of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="length">Fixed length of the array.</param>
        public SparseArray(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            _length = length;

            int groupCount = length / SparseArrayGroup<T>.GroupSize;
            if (length % SparseArrayGroup<T>.GroupSize != 0)
                groupCount++;

            _groups = new SparseArrayGroup<T>[groupCount];
        }


        /// <summary>
        /// Private constructor used by Resize().
        /// </summary>
        /// <param name="oldArray">Old array</param>
        /// <param name="newLength">New array length</param>
        private SparseArray(SparseArray<T> oldArray, int newLength) : this(newLength)
        {
            Debug.Assert(oldArray != null);

            int groupCopyCount = 0;
            if (_groups.Length < oldArray._groups.Length)
                groupCopyCount = _groups.Length;
            else if (oldArray._groups.Length <= _groups.Length)
                groupCopyCount = oldArray._groups.Length;

            Array.Copy(oldArray._groups, _groups, groupCopyCount);
            // TODO: I need to copy the bitmask and vacated flags, too. Really need to test this.
            //       A lot of this logic belongs in the sparse group.

            // trim trailing values that might've been copied over in the last group
            if (newLength < oldArray.Length && newLength > 0)
            {
                int lastIndexInGroup = (newLength % _groupSize) - 1;
                for (int i = _groupSize - 1; i > lastIndexInGroup; i--)
                {
                    _groups[_groups.Length - 1].Clear(i);
                }

            }
        }

        /// <summary>
        /// Gets/sets the element at the specified index. Use <see cref="Clear(int)"/>
        /// to elide an element from the sparse array instead of using this setter to
        /// set the value to zero/null/default(T).
        /// </summary>
        /// <param name="index">Zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                if (index >= _length || index < 0) throw new ArgumentOutOfRangeException(nameof(index));

                return _groups[index / _groupSize][index % _groupSize];
            }

            set
            {
                if (index >= _length || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                _groups[index / _groupSize].Set(index % _groupSize, value);
            }
        }

        internal T Get(int index, out ElementState positionState)
        {
            if (index >= _length || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _groups[index / _groupSize].Get(index % _groupSize, out positionState);
        }

        /// <summary>
        /// Gets the total number of elements in the sparse array.
        /// </summary>
        public int Length
        {
            get
            {
                return _length;
            }
        }

        /// <summary>
        /// Gets the number actual values held in the sparse array, excluding
        /// the count of empty/elided items.
        /// </summary>
        public int NonEmptyCount
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < _groups.Length; i++)
                    ret += _groups[i].NonEmptyCount;

                return ret;
            }
        }

        /// <summary>
        /// Gets the number empty/elided values held in the sparse array that are
        /// using minimal memory.
        /// </summary>
        public int EmptyCount
        {
            get
            {
                return Length - NonEmptyCount;
            }
        }


        public void Clear(int index)
        {
            if (index >= _length || index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            _groups[index / _groupSize].Clear(index % _groupSize);
        }


        public IEnumerator<T> GetEnumerator()
        {
            int currentIndex = 0;
            foreach (var group in _groups)
            {
                foreach (T item in group)
                {
                    if (currentIndex < _length)
                    {
                        yield return item;
                        currentIndex++;
                    }
                    else
                        yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> NonEmptyValues
        {
            get
            {
                foreach (var group in _groups)
                    foreach (T item in group.NonEmptyValues)
                        yield return item;
            }
        }

        

        int ICollection<T>.Count
        {
            get
            {
                return Length;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the number of elements to the specified new size.
        /// </summary>
        /// <param name="sparseArray">The sparse array to resize, or <c>null</c> to create a sparse new array with the specified size.</param>
        /// <param name="newSize">The size of the new array.</param>
        /// <remarks>
        /// <para>
        /// This method allocates a new sparse array with the specified size, copying elements from the old sparse array to the new one, 
        /// and then replaces the old sparse array with the new one.
        /// </para>
        /// <para>
        /// If <paramref name="newSize"/> is greater than the length of the old sparse array, a new sparse array is allocated and all the elements are
        /// copied from the old sparse array to the new one. If <paramref name="newSize"/> is less than the length of the old sparse array, a new sparse array
        /// is allocated and elements are copied from the old to the new until the new one is filled; the rest of the elements in the old array are ignored. 
        /// If <paramref name="newSize"/> is equal to the length of the old array, this method does nothing.
        /// </para>
        /// </remarks>
        public static void Resize(ref SparseArray<T> sparseArray, int newSize)
        {
            if (newSize < 0) throw new ArgumentOutOfRangeException(nameof(newSize));

            if (sparseArray == null)
                sparseArray = new SparseArray<T>(newSize);
            else if (sparseArray.Length == newSize)
            {
                // nothing to do
            }
            else
            {
                sparseArray = new SparseArray<T>(sparseArray, newSize);
            }

        }

        int IList<T>.IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _groups.Length; i++)
            {
                int groupPos = _groups[i].IndexOf(item, comparer);
                if (groupPos != -1)
                {
                    int ret = (i * _groupSize) + groupPos;

                    // if we're looking for null/default/0/etc, make sure we didn't run off the end into
                    // a group's unused, leftover bits:
                    if (ret >= Length)
                        ret = -1;

                    return ret;
                }
            }
            return -1;
        }

        /// <summary>
        /// Not supported. SparseArray is fixed length.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException($"{nameof(SparseArray<T>)} is fixed length.");
        }

        /// <summary>
        /// Not supported. SparseArray is fixed length.
        /// </summary>
        /// <param name="index"></param>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException($"{nameof(SparseArray<T>)} is fixed length.");
        }

        /// <summary>
        /// Not supported. SparseArray is fixed length.
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException($"{nameof(SparseArray<T>)} is fixed length.");
        }

        void ICollection<T>.Clear()
        {

            _groups = new SparseArrayGroup<T>[_groups.Length];
        }

        bool ICollection<T>.Contains(T item)
        {
            IList<T> asList = this as IList<T>;
            if (asList.IndexOf(item) > -1)
                return true;
            else
                return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            // TODO: worth implementing?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not supported. SparseArray is fixed length.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException($"{nameof(SparseArray<T>)} is fixed length.");
        }
    }
}
