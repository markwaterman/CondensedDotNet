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
    /// Collection of integers with values that range from 0 to 65535 (inclusive). Values are stored internally as System.UShort values.
    /// </summary>
    [Serializable]
    class IntListUShortStorage : OffsetIndex
    {
        private List<UInt16> _ushortIndex;

        internal IntListUShortStorage()
        {
            _ushortIndex = new List<UInt16>();
        }

        internal IntListUShortStorage(int capacity)
        {
            _ushortIndex = new List<UInt16>(capacity);
        }

        public override int this[int index]
        {
            get
            {
                return _ushortIndex[index];
            }

            set
            {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (value > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, string.Format("value must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif

                _ushortIndex[index] = (UInt16)value;
            }
        }

        public override int Count
        {
            get
            {
                return _ushortIndex.Count;
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
            if (item > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif
            _ushortIndex.Add((UInt16)item);
        }

        public override void Clear()
        {
            _ushortIndex.Clear();
        }

        public override bool Contains(int item)
        {
#if DEBUG
            if (item > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif
            return _ushortIndex.Contains((UInt16)item);
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<int> GetEnumerator()
        {
            foreach (ushort item in _ushortIndex)
                yield return (int)item;
        }

        public override int IndexOf(int item)
        {
#if DEBUG
            if (item > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif
            return _ushortIndex.IndexOf((UInt16)item);
        }

        public override void Insert(int index, int item)
        {
#if DEBUG
            if (item > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif
            _ushortIndex.Insert(index, (UInt16)item);
        }

        public override bool Remove(int item)
        {
#if DEBUG
            if (item > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", UInt16.MaxValue));
#endif
            return _ushortIndex.Remove((UInt16)item);
        }

        public override void RemoveAt(int index)
        {
            _ushortIndex.RemoveAt(index);
        }

        public override int Capacity
        {
            get
            {
                return _ushortIndex.Capacity;
            }
            set
            {
                _ushortIndex.Capacity = value;
            }
        }

        public override IndexType IndexType { get { return IndexType.TwoBytes; } }
    }
}
