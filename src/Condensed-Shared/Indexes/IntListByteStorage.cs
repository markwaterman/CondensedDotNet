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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Condensed.Indexes
{
    /// <summary>
    /// Collection of integers with values that range from 0 to 255 (inclusive). Values are stored internally as System.Byte values.
    /// </summary>
    [Serializable]
    class IntListByteStorage : OffsetIndex
    {
        
        private List<Byte> _byteIndex;

        internal IntListByteStorage()
        {
            _byteIndex = new List<byte>();
        }

        internal IntListByteStorage(int capacity)
        {
            _byteIndex = new List<byte>(capacity);
        }

        public override int this[int index]
        {
            get
            {
                return _byteIndex[index];
            }

            set
            {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (value > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("value", value, string.Format("value must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
                _byteIndex[index] = (Byte)value;
            }
        }

        public override int Count
        {
            get
            {
                return _byteIndex.Count;
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
            if (item > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
            _byteIndex.Add((Byte)item);
        }

        public override void Clear()
        {
            _byteIndex.Clear();
        }

        public override bool Contains(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
            return _byteIndex.Contains((byte)item);
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<int> GetEnumerator()
        {
            foreach (byte item in _byteIndex)
                yield return (int)item;
        }

        public override int IndexOf(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
            return _byteIndex.IndexOf((byte)item);
        }

        public override void Insert(int index, int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
            _byteIndex.Insert(index, (Byte)item);
        }

        public override bool Remove(int item)
        {
#if DEBUG
            // Useful sanity check for unit tests, but slows us down too much for release build.
            if (item > Byte.MaxValue)
                throw new ArgumentOutOfRangeException("item", item, string.Format("item must be positive and cannot be greater than {0}", Byte.MaxValue));
#endif
            return _byteIndex.Remove((byte)item);
        }

        public override void RemoveAt(int index)
        {
            _byteIndex.RemoveAt(index);
        }

        public override int Capacity
        {
            get
            {
                return _byteIndex.Capacity;
            }
            set
            {
                _byteIndex.Capacity = value;
            }
        }

        public override IndexType IndexType { get { return IndexType.OneByte; } }
    }

    
}
