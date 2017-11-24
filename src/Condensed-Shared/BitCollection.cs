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

namespace Condensed
{
    /// <summary>
    /// A bit vector implementation with IList semantics. Behaves like a <see cref="System.Collections.Generic.List{T}"/>
    /// of booleans but with more efficient storage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The BitCollection is used internally by the <see cref="CondensedCollection{T}"/>, and, while not part of 
    /// the CondensedCollection's public API, it's marked public in case others find it useful.
    /// </para>
    /// <para>
    /// The main noteworthy difference between this collection and System.Collections.BitArray is the
    /// ability to insert and remove individual bit elements. Performance of insert/remove operations
    /// is not terrific (a lot of bitwise shifting may be required), but it's tolerable for infrequent use or
    /// when removing/adding items at/near the end of the collection.
    /// </para>
    /// <para>
    /// Also, word size is 64 bits instead of BitArray's 32 bits. This was done primarily to cut shift
    /// operations in half, but overall performance will suffer on 32-bit platforms.
    /// </para>
    /// <para>
    /// Contains some logic inspired by the very useful "Bit Twiddling Hacks" compiled by Sean Eron Anderson:
    /// <see href="http://graphics.stanford.edu/~seander/bithacks.html"/> (code
    /// is in the public domain).
    /// </para>
    /// </remarks>
    /// <threadsafety static="true" instance="false" />
    [Serializable]
    public class BitCollection : IList<bool>
    {
        const int BITS_PER_WORD = 64;
        const int BYTES_PER_WORD = 8;
        private int _count = 0;
        ulong[] _words;

        /// <summary>
        /// Constructs a new instance of the <see cref="BitCollection"/> class.
        /// </summary>
        public BitCollection() : this(0, false, 0)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BitCollection"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacityInBits">The number of elements that the new collection can initially store.</param>
        public BitCollection(int capacityInBits) : this(0, false, capacityInBits)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="BitCollection"/> class with the specified initial values.
        /// </summary>
        /// <param name="count">The number of bit values in the new <see cref="BitCollection"/>.</param>
        /// <param name="defaultValue">The Boolean value to assign to each bit.</param>
        /// <param name="capacityInBits">The number of elements that the new collection can initially store.</param>
        public BitCollection(int count, bool defaultValue, int capacityInBits)
        {
            if (capacityInBits < count)
                throw new ArgumentException("Initial capacity must be greater than or equal to initial count.");

            if (capacityInBits % BITS_PER_WORD == 0)
                _words = new ulong[capacityInBits / BITS_PER_WORD];
            else
                _words = new ulong[(capacityInBits / BITS_PER_WORD) + 1];

            _count = count;

            if (defaultValue)
            {
                for (int i = 0; i < _words.Length; ++i)
                    _words[i] = ulong.MaxValue;
            }
        }

        /// <summary>
        /// Gets/sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity
        {
            get { return _words.Length * BITS_PER_WORD; }
            set
            {
                if (value < _count)
                    throw new ArgumentOutOfRangeException("value", value, "Capacity cannot be set to a value that is less than Count.");

                EnsureSize(value - 1);
            }
        }

        /// <summary>
        /// Ensures there's enough room in the _words array to
        /// fit an entry at the specified index.
        /// </summary>
        /// <param name="index">Index of the entry that must fit in the collection.</param>
        private void EnsureSize(int index)
        {
            if (index >= _words.Length * BITS_PER_WORD)
            {
                // need to grow. Add enough room for index + 25%
                int newArrayCount = index / BITS_PER_WORD;

                if (index % BITS_PER_WORD != 0)
                    newArrayCount++;

                newArrayCount = (int)Math.Ceiling(newArrayCount * 1.25);

                // _words.Length might have been 0, in which case we need increment.
                if (newArrayCount == 0) newArrayCount++;

                ulong[] newArray = new ulong[newArrayCount];
                Buffer.BlockCopy(_words, 0, newArray, 0, (BYTES_PER_WORD * _words.Length));
                _words = newArray;
            }
        }

        /// <summary>
        /// Gets or sets the value of the bit at a specific position in the <see cref="BitCollection"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <returns>The value of the bit at position index.</returns>
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index", index, "Index must be nonnegative and less than Count");

                return (_words[index / BITS_PER_WORD] & (1UL << (index % BITS_PER_WORD))) != 0;
            }

            set
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index", index, "Index must be nonnegative and less than Count");

                if (value)
                {
                    _words[index / BITS_PER_WORD] |= (1UL << (index % BITS_PER_WORD));
                }
                else
                {
                    _words[index / BITS_PER_WORD] &= ~(1UL << (index % BITS_PER_WORD));
                }
            }
        }


        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The value to add to the collection.</param>
        public void Add(bool item)
        {
            EnsureSize(_count);
            _count++;
            this[_count - 1] = item;
            
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _words = new ulong[0];
            _count = 0;
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The value to locate in the collection.</param>
        /// <returns>true if value is found in the collection; otherwise, false.</returns>
        public bool Contains(bool item)
        {
            return (IndexOf(item) != -1);
        }

        /// <summary>
        /// Copies the entire BitCollection to a compatible one-dimensional array of bools, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the BitCollection.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array argument at which copying begins.</param>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex argument cannot be less than 0.");
            if ((array.Length - arrayIndex) < this.Count)
                throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");


            foreach (bool item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<bool> GetEnumerator()
        {
            return new BitCollectionEnumerator(this);
        }

        /// <summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence within the collection.
        /// </summary>
        /// <param name="item">The value to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the collection, if found; otherwise, -1.</returns>
        public int IndexOf(bool item)
        {
            if (_count == 0)
                return -1;

            int wordIndex = 0;
            int index = -1;
            if (item)
            {
                // find first non-zero element in _words:
                for (wordIndex = 0; wordIndex < _words.Length; wordIndex++)
                {
                    if (_words[wordIndex] != 0x0000000000000000)
                    {
                        // find where the first bit is set to 1:
                        int bitPosition = CountTrailingZeros(_words[wordIndex]);
                        index = (wordIndex * BITS_PER_WORD) + bitPosition;
                        break;
                    }
                }
            }
            else
            {
                // find first element in _words that contains a zero:
                for (wordIndex = 0; wordIndex < _words.Length; wordIndex++)
                {
                    if (_words[wordIndex] != 0xFFFFFFFFFFFFFFFF)
                    {
                        // find the first zero'd bit:
                        int bitPosition = CountTrailingOnes(_words[wordIndex]);
                        index = (wordIndex * BITS_PER_WORD) + bitPosition;
                        break;
                    }
                }
            }

            // make sure we didn't go past _count
            if (index >= _count)
                index = -1;

            return index;
        }

        /// <summary>
        /// Count the consecutive zero bits (trailing) on the right linearly.
        /// </summary>
        /// <param name="val">input to count trailing zero bits</param>
        /// <returns>count of val's trailing zero bits</returns>
        /// <remarks>
        /// C# port of logic from "Bit Twiddling Hacks" by Sean Eron Anderson:
        /// http://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightLinear -- code
        /// is in the public domain.
        /// </remarks>
        private static int CountTrailingZeros(ulong val)
        {
            int c;  // return value: count v's trailing zero bits,
                    // so if v is 1101000 (base 2), then c will be 3
            if (val != 0)
            {
                val = (val ^ (val - 1)) >> 1;  // Set val's trailing 0s to 1s and zero the rest
                for (c = 0; val != 0; c++)
                {
                    val >>= 1;
                }
            }
            else
            {
                c = BITS_PER_WORD; // 64
            }

            return c;
        }

        private static int CountTrailingOnes(ulong val)
        {
            return CountTrailingZeros(~val);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The value to insert into the collection.</param>
        public void Insert(int index, bool item)
        {
            // inserting at the end is OK
            if (index < 0 || index > _count) 
                throw new ArgumentOutOfRangeException("index", index, "Index out of range. Must be non-negative and less or equal to the size of the collection.");

            EnsureSize(_count);

            // First we shift all the bits left in the words that follow the one being modified.
            LeftShiftSubsequentWords(index);

            // Insert into the word being modified
            InsertBitIntoWord(index, item);

            _count++;
        }

        /// <summary>
        /// Removes the first occurrence of the specified value from the collection.
        /// </summary>
        /// <param name="item">The value to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.</returns>
        public bool Remove(bool item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false; // not found
            else
            {
                RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Removes the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("index", index, "Index out of range. Must be non-negative and less than the size of the collection.");

            // remove bit from the word that it resides in.
            RemoveBitFromWord(index);
            // shift all subsequent words:
            RightShiftSubsequentWords(index);

            _count--;
        }

        /// <summary>
        /// Removes bit from the word that it resides in. The LSB from the
        /// subsequent word is moved into the most significant position of the
        /// word being modified.
        /// </summary>
        /// <param name="index"></param>
        private void RemoveBitFromWord(int index)
        {
            int wordIndex = index / BITS_PER_WORD;
            ulong word = _words[wordIndex];

            // shift everything right, starting at the bit we're 
            // removing. This loop isn't all that efficient (will research
            // a faster way once I have some time), but we only have to go 
            // through this gyration once per RemoveAt call.
            int bitIndex = index % BITS_PER_WORD;
            while (bitIndex < (BITS_PER_WORD - 1))
            {
                // get the value of the next bit:
                if ((word & (1UL << (bitIndex + 1))) > 0)
                {
                    // the next bit is set so we set current bit index
                    word |= 1UL << bitIndex;
                }
                else
                {
                    // the next bit is clear so we clear the current bit index
                    word &= ~(1UL << bitIndex);
                }
                bitIndex++;
            }

            // set the last bit by looking the first bit in the next word (unless we're already at the last word)
            if (wordIndex < (_words.Length - 1))
            {
                ulong nextWord = _words[wordIndex + 1];
                if ((nextWord & 1UL) > 0)
                {
                    // the next bit is set so we set current bit index
                    word |= 1UL << (BITS_PER_WORD - 1);
                }
                else
                {
                    // the next bit is clear so we clear the current bit index
                    word &= ~(1UL << (BITS_PER_WORD - 1));
                }
            }

            _words[wordIndex] = word;
        }

        private void InsertBitIntoWord(int index, bool value)
        {
            int bitIndexBeingModified = index % BITS_PER_WORD;
            int wordIndex = index / BITS_PER_WORD;
            ulong word = _words[wordIndex];

            // Shift everything left, starting at the last bit and then
            // working back to the location of the bit being inserted.
            int currentBitIndex = BITS_PER_WORD - 1;
            while (currentBitIndex >= bitIndexBeingModified)
            {
                // get the value of the preceding bit:
                if ((word & (1UL << (currentBitIndex - 1))) > 0)
                {
                    // the prior bit is set so we set current bit index
                    word |= 1UL << currentBitIndex;
                }
                else
                {
                    // the next prior bit is clear so we clear the current bit index
                    word &= ~(1UL << currentBitIndex);
                }
                currentBitIndex--;
            }

            //set the bit being inserted:
            if (value)
            {
                word |= (1UL << (bitIndexBeingModified));
            }
            else
            {
                word &= ~(1UL << (bitIndexBeingModified));
            }

            _words[wordIndex] = word;
        }

        private void RightShiftSubsequentWords(int index)
        {
            // we start our big shift operation at the word that follows the
            // word that contained the deleted bit.
            int wordIndex = (index / BITS_PER_WORD) + 1;

            if (wordIndex >= _words.Length)
                return;

            while (wordIndex < (_words.Length - 1))
            {
                ulong currentWord = _words[wordIndex];
                currentWord = currentWord >> 1;

                // get LSB from next word and move it into our current one:
                if ((_words[wordIndex + 1] & 1UL) == 1UL)
                {
                    currentWord |= 0x8000000000000000;
                }
                _words[wordIndex] = currentWord;
                wordIndex++;
            }

            // shift the last word 1 bit to the right:
            _words[wordIndex] = _words[wordIndex] >> 1;
        }

        private void LeftShiftSubsequentWords(int index)
        {
            int wordIndexBeingModified = index / BITS_PER_WORD;
            if (wordIndexBeingModified >= _words.Length)
                return;

            // we start our big shift operation at the very end of the word array.
            // (We're assuming the caller has called EnsureSize prior to calling this 
            // method so that it's safe to do this shift). Then we work our way back
            // to the word that immediately follows the one being modified.
            int currentWordIndex = _words.Length - 1;

            while (currentWordIndex > wordIndexBeingModified)
            {
                ulong currentWord = _words[currentWordIndex];
                currentWord = currentWord << 1;

                // get most significant bit from preceding word and move it into our current one:
                if ((_words[currentWordIndex - 1] & 0x8000000000000000) > 0)
                {
                    currentWord |= 1UL;
                }
                _words[currentWordIndex] = currentWord;
                currentWordIndex--;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An enumerator that can be used to iterate through elements in a <see cref="BitCollection"/>.
        /// </summary>
        /// <exclude />
        public sealed class BitCollectionEnumerator : IEnumerator<bool>
        {
            private BitCollection _list;
            private ulong _currentWord;
            private int _position = -1;
            private int _bitPosition = -1;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="list">Collection to be enumerated.</param>
            public BitCollectionEnumerator(BitCollection list)
            {
                if (list == null) throw new ArgumentNullException("list");
                _list = list;
                if (_list._words.Length > 0)
                    _currentWord = _list._words[0];
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public bool Current
            {
                get
                {
                    
                    return (_currentWord & (1UL << (_bitPosition))) != 0;
                    
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            /// <summary>
            /// Disposes the enumerator.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                const int LAST_BIT_INDEX = BITS_PER_WORD - 1;
                _position++;
                if (_position >= _list.Count)
                    return false;
                else
                {
                    if (_bitPosition == LAST_BIT_INDEX)
                    {
                        _currentWord = _list._words[_position / BITS_PER_WORD];
                        _bitPosition = 0;
                    }
                    else
                    {
                        _bitPosition++;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                _position = -1;
                _bitPosition = -1;
                if (_list._words.Length > 0)
                    _currentWord = _list._words[0];
            }
        }
    }

}
