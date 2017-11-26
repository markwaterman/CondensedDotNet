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

namespace Condensed.Indexes
{
    /// <summary>
    /// A "fake" collection of integers that can only hold the value 0. Used
    /// by the DedupedList when it only contains a single unique value.
    /// </summary>
    [Serializable]
    class IntListZeroStorage : OffsetIndex
    {
        int _count = 0;
        // Track the user's intended capacity--useful to hold onto for later, when the DedupedList
        // needs to switch from using this collection to a "real" index (like an IntListBitStorage).
        int _capacity;

        public IntListZeroStorage(int capacity)
        {
            _capacity = capacity;
        }

        private void EnsureCapacity(int min)
        {
            // roughly equivalent to what List<T> does:
            if (_count < min)
            {
                int newCapacity = _count == 0 ? 4 : _count * 2;
                if ((uint)newCapacity > 0X7FEFFFFF) newCapacity = 0X7FEFFFFF;
                if (newCapacity < min) newCapacity = min;
                _capacity = newCapacity;
            }
        }

        public override int this[int index]
        {
            get
            {
                return 0;
            }

            set
            {
#if DEBUG
                // Useful sanity check for unit tests, but slows us down too much for release build.
                if (value > 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, string.Format("value must be positive and cannot be greater than {0}", 0));
                }
                if (index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
#endif
            }
        }

        public override int Count
        {
            get
            {
                return _count;
            }
        }

        public override int Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                if (value < _count)
                    throw new ArgumentOutOfRangeException("value", value, "Capacity cannot be set to a value that is less than Count.");

                _capacity = value;
            }

        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        public override void Add(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > 0)
                throw new ArgumentOutOfRangeException("item", item, string.Format("value must be positive and cannot be greater than {0}", 0));
#endif
            if (_count == _capacity)
                EnsureCapacity(_count + 1);

            _count++;
        }

        public override void Clear()
        {
            _count = 0;
        }

        public override bool Contains(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > 0)
                throw new ArgumentOutOfRangeException("item", item, string.Format("value must be positive and cannot be greater than {0}", 0));
#endif
            if (_count > 0 && item == 0)
                return true;
            else
                return false;
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < _count; ++i)
                yield return 0;
        }

        public override int IndexOf(int item)
        {
            if (_count > 0 && item == 0)
                return 0;
            else
                return -1;
        }

        public override void Insert(int index, int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > 0)
                throw new ArgumentOutOfRangeException("item", item, string.Format("value must be positive and cannot be greater than {0}", 0));
#endif
            // Insertions at the end are legal.
            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException("index", index, string.Format("Index was out of range. Must be non-negative and less than the size of the collection."));

            if (_count == _capacity)
                EnsureCapacity(_count + 1);

            _count++;
        }

        public override bool Remove(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > 0)
                throw new ArgumentOutOfRangeException("item", item, string.Format("value must be positive and cannot be greater than {0}", 0));
#endif
            if (_count > 0 && item == 0)
            {
                _count--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("index", index, string.Format("Index was out of range. Must be non-negative and less than the size of the collection."));

            _count--;
        }

        public override IndexType IndexType { get { return IndexType.ZeroBytes; } }
    }
}
