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
    /// Collection of integers with values of 0 or 1. Values are stored internally in a bit vector.
    /// </summary>
    [Serializable]
    class IntListBitStorage : OffsetIndex
    {
        private BitCollection _bits;

        public IntListBitStorage()
        {
            _bits = new BitCollection();
        }

        public IntListBitStorage(int initialCount, int capacity)
        {
            _bits = new BitCollection(initialCount, false, capacity);
        }


        public override int this[int index]
        {
            get
            {
                return _bits[index] ? 1 : 0;
            }

            set
            {
                if (value == 0)
                    _bits[index] = false;
                else
                    _bits[index] = true;

            }
        }

        public override int Count
        {
            get
            {
                return _bits.Count;
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
            switch (item)
            {
                case 0:
                    _bits.Add(false);
                    break;
                case 1:
                    _bits.Add(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("item", "Can't add a value greater than 1 to this type of index");
                    
            }
        }

        public override void Clear()
        {
            _bits.Clear();
        }

        public override bool Contains(int item)
        {
            switch (item)
            {
                case 0:
                    return _bits.Contains(false);
                case 1:
                    return _bits.Contains(true);
                default:
                    throw new ArgumentOutOfRangeException("item", "Searching for a value that can't be held by this index.");

            }
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<int> GetEnumerator()
        {
            foreach (bool item in _bits)
                yield return (item) ? 1 : 0;
        }

        public override int IndexOf(int item)
        {
            switch (item)
            {
                case 0:
                    return _bits.IndexOf(false);
                case 1:
                    return _bits.IndexOf(true);
                default:
                    throw new ArgumentOutOfRangeException("item", "Searching for a value that can't be held by this index.");

            }
        }

        public override void Insert(int index, int item)
        {
            switch (item)
            {
                case 0:
                    _bits.Insert(index, false);
                    break;
                case 1:
                    _bits.Insert(index, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("item", "Can't insert a value greater than 1 to this type of index");

            }
        }

        public override bool Remove(int item)
        {
            switch (item)
            {
                case 0:
                    return _bits.Remove(false);
                case 1:
                    return _bits.Remove(true);
                default:
                    throw new ArgumentOutOfRangeException("item", "Can't remove a value greater than 1 from this type of index");

            }
        }

        public override void RemoveAt(int index)
        {
            _bits.RemoveAt(index);
        }

        public override int Capacity
        {
            get
            {
                return _bits.Capacity;
            }
            set
            {
                _bits.Capacity = value;
            }
        }

        public override IndexType IndexType { get { return IndexType.OneBit; } }
    }
}
