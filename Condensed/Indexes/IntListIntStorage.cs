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

namespace Condensed.Indexes
{
    /// <summary>
    /// Collection of integers with values that range from 0 to 2,147,483,647 (inclusive). Values are stored internally as System.Int32 values.
    /// </summary>
    [Serializable]
    class IntListIntStorage : OffsetIndex
    {
        private List<Int32> _intIndex;

        internal IntListIntStorage()
        {
            _intIndex = new List<Int32>();
        }

        internal IntListIntStorage(int capacity)
        {
            _intIndex = new List<Int32>(capacity);
        }

        public override int this[int index]
        {
            get
            {
                return _intIndex[index];
            }

            set
            {
                _intIndex[index] = value;
            }
        }

        public override int Count
        {
            get
            {
                return _intIndex.Count;
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
            _intIndex.Add(item);
        }

        public override void Clear()
        {
            _intIndex.Clear();
        }

        public override bool Contains(int item)
        {
            return _intIndex.Contains(item);
        }

        public override void CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<int> GetEnumerator()
        {
            foreach (int item in _intIndex)
                yield return item;
        }

        public override int IndexOf(int item)
        {
            return _intIndex.IndexOf(item);
        }

        public override void Insert(int index, int item)
        {
            _intIndex.Insert(index, item);
        }

        public override bool Remove(int item)
        {
            return _intIndex.Remove(item);
        }

        public override void RemoveAt(int index)
        {
            _intIndex.RemoveAt(index);
        }

        public override int Capacity
        {
            get
            {
                return _intIndex.Capacity;
            }
            set
            {
                _intIndex.Capacity = value;
            }
        }

        public override IndexType IndexType { get { return IndexType.FourBytes; } }
    }
}
