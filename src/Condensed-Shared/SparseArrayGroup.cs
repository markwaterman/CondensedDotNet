using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Condensed
{
    internal enum ElementState : byte { Occupied, Empty }

    internal struct SparseArrayGroup<T> : IEnumerable<T>
    {
        // Bitmap stuff. Currently using a 32-bit bitmap since smaller groups are 2x faster for
        // many operations at the cost of a modest increase in bits per element.
        // Consider changing these types to UInt64 if memory savings are more important.
        internal static readonly int GroupSize = sizeof(UInt32) * 8;
        private const UInt32 FULL_GROUP = UInt32.MaxValue;
        private const UInt32 ONE = 1u;
        private UInt32 _bitmap;

        private T[] _group;
        private int _groupCount;

        public T this[int index]
        {
            get
            {
                Debug.Assert(index < GroupSize);
                if (IsOccupied(index))
                {
                    return _group[PositionToOffset(index)];
                }
                else
                    return default(T);
            }

        }

        internal T Get(int index, out ElementState positionState)
        {
            Debug.Assert(index < GroupSize);
            if (IsOccupied(index))
            {
                positionState = ElementState.Occupied;
                return _group[PositionToOffset(index)];
            }
            else
            {
                positionState = ElementState.Empty;

            }
            return default(T);
        }

        public void Set(int index, T value)
        {
            Debug.Assert(index < GroupSize);
            Debug.Assert(_groupCount <= GroupSize);

            // Note: it's up to the caller to check
            // for null/0/default(T)/whatever using his EqualityComparer<T>
            // and then clear the element by calling Clear().
            // Passing null into this setter will just cause the default
            // value to be stored in the _group array, reducing memory savings.

            var offset = PositionToOffset(index);
            if (!IsOccupied(index))
            {
                // We need to create the array.
                if (_group == null)
                    _group = GetNewArray(1);
                else
                {
                    if (_groupCount == _group.Length)
                    {
                        // we need to grow the array.

                        var newGroup = GetNewArray(_groupCount + 1);

                        // copy over old values, leaving the element open for the new value.
                        int j = 0;
                        for (int i = 0; i < _groupCount; i++)
                        {
                            newGroup[j] = _group[i];
                            if (i == offset)
                                j += 2;
                            else
                                j++;
                        }

                        _group = newGroup;
                    }
                    else
                    {
                        // there's enough spare room in the array to accommodate the value. Make room.
                        for (int i = _groupCount; i > offset; i--)
                        {
                            _group[i] = _group[i - 1];
                        }
                    }

                }
                SetBit(index);
                _groupCount++;
            }
            _group[offset] = value;
        }

        public void Clear(int index)
        {
            Debug.Assert(index < GroupSize);

            if (IsOccupied(index))
            {
                var offset = PositionToOffset(index);

                if (TryShrink(_group.Length, _groupCount - 1, out T[] newArray))
                {
                    // We're shrinking the array to a smaller, non-zero length.
                    int j = 0;
                    for (int i = 0; i < _groupCount; i++)
                    {
                        if (i != offset)
                        {
                            newArray[j] = _group[i];
                            j++;
                        }
                    }

                    _group = newArray;
                }
                else
                {
                    // Keep the existing array, removing the cleared item.
                    for (int i = offset; i < _groupCount - 1; i++) // count - 2???
                    {
                        _group[i] = _group[i + 1];
                    }
                    _group[_groupCount - 1] = default(T);
                }

                _groupCount--;
                ClearBit(index);
            }

        }



        private void SetBit(int position)
        {
            _bitmap |= (ONE << position);
        }

        private void ClearBit(int position)
        {
            _bitmap &= ~(ONE << position);
        }

        private bool IsOccupied(int index)
        {
            return (_bitmap & (ONE << index)) != 0;
        }

        private void ClearVacated(int position)
        {
            _bitmap &= ~(ONE << position);
        }


        int PositionToOffset(int position)
        {
            return SparseBitwise.PositionToOffset(_bitmap, position);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < GroupSize; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> NonEmptyValues
        {
            get
            {
                if (_group == null)
                    yield break;

                for (int i = 0; i < _groupCount; i++)
                {
                    yield return _group[i];
                }
            }
        }

        public int NonEmptyCount
        {
            get
            {
                return _groupCount;
            }
        }

        public int IndexOf(T item, IEqualityComparer<T> comparer)
        {
            int ret = -1;

            if (_group != null)
            {
                int offset = -1;
                for (int i = 0; i < _groupCount; i++)
                {
                    if (comparer.Equals(item, _group[i]))
                        offset = i;
                }
                if (offset != -1)
                    ret = SparseBitwise.OffsetToPosition(_bitmap, offset);
            }

            // we have to check our empty elements, too, if they're looking for null/0/default.
            if (comparer.Equals(item, default(T)))
            {
                int firstClearBitPos = -1;
                if (_bitmap == 0)
                    firstClearBitPos = 0;
                else if (_bitmap == FULL_GROUP)
                    firstClearBitPos = -1;
                else
                    firstClearBitPos = SparseBitwise.PositionOfFirstCleared(_bitmap);

                if (ret == -1)
                    ret = firstClearBitPos;
                else
                {
                    if (firstClearBitPos != -1 && firstClearBitPos < ret)
                        ret = firstClearBitPos;
                }
            }

            return ret;
        }


        private static T[] GetNewArray(int minElements)
        {
            /*
             * 0-7:   8 elements
             * 8-15:  16 elements
             * 16-23: 24 elements
             *  ...etc.
             **/
            return new T[((minElements / 8) * 8) + 8];
        }


        private static bool TryShrink(int arraySize, int elementsInUse, out T[] smallerArray)
        {
            int suggestedSize = ((elementsInUse / 8) * 8) + 8;

            if (arraySize > suggestedSize)
            {
                smallerArray = new T[suggestedSize];
                return true;
            }
            else
            {
                smallerArray = null;
                return false;
            }
        }
    }
}
