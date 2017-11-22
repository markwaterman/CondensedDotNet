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

namespace Condensed
{
    [Serializable]
    sealed class NullKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dict;
        TValue _valForNullKey;
        bool _hasNullKey;
        readonly IEqualityComparer<TKey> _comparer;

        private KeyCollection _keys;
        private ValueCollection _values;

        public NullKeyDictionary() : this(null)
        {
        }

        public NullKeyDictionary(IEqualityComparer<TKey> comparer)
        {
            if (default(TKey) != null)
                throw new NotSupportedException("NullKeyDictionary is only for use with nullable key types");

            _comparer = comparer ?? EqualityComparer<TKey>.Default;

            _dict = new Dictionary<TKey, TValue>(_comparer);
            _valForNullKey = default(TValue);
            _hasNullKey = false;
        }

        public TValue this[TKey key]
        {
            get
            {
                //if (_comparer.Equals(default(TKey), key)) // this is probably a more correct way to check for null, but profiling shows it to be much too slow.
                if (key == null)
                {
                    if (_hasNullKey)
                        return _valForNullKey;
                    else
                        throw new KeyNotFoundException("key does not exist in the collection");
                }
                else
                    return _dict[key];
            }

            set
            {
                if (key == null)
                {
                    _valForNullKey = value;
                    _hasNullKey = true;
                }
                else
                {
                    _dict[key] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                if (_hasNullKey)
                    return _dict.Count + 1;
                else
                    return _dict.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_keys == null)
                    _keys = new KeyCollection(this);

                return _keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_values == null)
                    _values = new ValueCollection(this);

                return _values;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                if (_hasNullKey)
                    throw new ArgumentException("An element with the same key already exists.", "key");

                _valForNullKey = value;
                _hasNullKey = true;
            }
            else
            {
                _dict.Add(key, value);
            }
        }

        public void Clear()
        {
            _dict.Clear();
            _hasNullKey = false;
            _valForNullKey = default(TValue);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                if (_hasNullKey)
                {
                    if (EqualityComparer<TValue>.Default.Equals(_valForNullKey, item.Value))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
            {
                var dictAsColl = _dict as ICollection<KeyValuePair<TKey, TValue>>;
                return dictAsColl.Contains(item);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                if (_hasNullKey)
                    return true;
                else
                    return false;
            }
            else
            {
                return _dict.ContainsKey(key);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex must reside within the bounds of the destination array");
            if ((array.Length - arrayIndex) < this.Count)
                throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");

            if (_hasNullKey)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(default(TKey), _valForNullKey);
                arrayIndex++;
            }

            var dictAsColl = _dict as ICollection<KeyValuePair<TKey, TValue>>;
            dictAsColl.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_hasNullKey)
                yield return new KeyValuePair<TKey, TValue>(default(TKey), _valForNullKey);

            foreach (var kvp in _dict)
                yield return kvp;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
            {
                if (_hasNullKey)
                {
                    if (EqualityComparer<TValue>.Default.Equals(_valForNullKey, item.Value))
                    {
                        _hasNullKey = false;
                        _valForNullKey = default(TValue);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
            {
                var dictAsColl = _dict as ICollection<KeyValuePair<TKey, TValue>>;
                return dictAsColl.Remove(item);
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                if (_hasNullKey)
                {
                    _hasNullKey = false;
                    _valForNullKey = default(TValue);
                    return true;
                }
                else
                    return false;
            }
            else
                return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                if (_hasNullKey)
                {
                    value = _valForNullKey;
                    return true;
                }
                else
                {
                    value = default(TValue);
                    return false;
                }
            }
            else
                return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        sealed class KeyCollection : ICollection<TKey>
        {
            private NullKeyDictionary<TKey, TValue> _nkDict;
            public KeyCollection(NullKeyDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null) throw new ArgumentNullException("dictionary");
                _nkDict = dictionary;
            }

            public int Count
            {
                get
                {
                    return _nkDict.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public void Add(TKey item)
            {
                throw new NotSupportedException("Collection is read-only");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only");
            }

            public bool Contains(TKey item)
            {
                return _nkDict.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (arrayIndex < 0 || arrayIndex > array.Length)
                    throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex must reside within the bounds of the destination array");
                if ((array.Length - arrayIndex) < this.Count)
                    throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");

                if (_nkDict._hasNullKey)
                {
                    array[arrayIndex] = default(TKey);
                    arrayIndex++;
                }

                _nkDict._dict.Keys.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                if (_nkDict._hasNullKey)
                    yield return default(TKey);

                foreach (var key in _nkDict._dict.Keys)
                    yield return key;
            }

            public bool Remove(TKey item)
            {
                throw new NotSupportedException("Collection is read-only");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Serializable]
        sealed class ValueCollection : ICollection<TValue>
        {
            private NullKeyDictionary<TKey, TValue> _nkDict;
            public ValueCollection(NullKeyDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null) throw new ArgumentNullException("dictionary");
                _nkDict = dictionary;
            }
            public int Count
            {
                get
                {
                    return _nkDict.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public void Add(TValue item)
            {
                throw new NotSupportedException("Collection is read-only");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only");
            }

            public bool Contains(TValue item)
            {
                if (_nkDict._hasNullKey && EqualityComparer<TValue>.Default.Equals(_nkDict._valForNullKey, item))
                    return true;

                return _nkDict._dict.ContainsValue(item);
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException("array");
                if (arrayIndex < 0 || arrayIndex > array.Length)
                    throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex must reside within the bounds of the destination array");
                if ((array.Length - arrayIndex) < this.Count)
                    throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");

                if (_nkDict._hasNullKey)
                {
                    array[arrayIndex] = _nkDict._valForNullKey;
                    arrayIndex++;
                }

                _nkDict._dict.Values.CopyTo(array, arrayIndex);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                if (_nkDict._hasNullKey)
                    yield return _nkDict._valForNullKey;

                foreach (var val in _nkDict._dict.Values)
                    yield return val;
            }

            public bool Remove(TValue item)
            {
                throw new NotSupportedException("Collection is read-only");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}