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
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;

using Condensed.Indexes;

namespace Condensed
{
    /// <summary>
    /// An <see cref="System.Collections.Generic.IList{T}"/> implementation that uses interning (deduplication) for efficient storage of large 
    /// numbers of immutable elements. Interning items makes the collection more memory-efficient and can often improve performance for 
    /// certain compute-intensive tasks. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The CondensedCollection class provides a generalized form of interning for immutable types. It provides the following features:
    /// <list type="bullet">
    /// <item>Nullable types are supported.</item>
    /// <item>Optional fallback ("cutover") to non-deduplicated list behavior if the items in the list aren't sufficiently unique.</item>
    /// <item>Specialized LINQ operators in <see cref="Condensed.Linq"/> namespace that are optimized to work on the deduplicated collection.</item>
    /// </list>
    /// </para>
    /// <para>
    /// The CondensedCollection class is best suited for relatively static collection of elements, or, at the very least, a population whose set of unique
    /// values is relatively fixed. The collection maintains an "intern pool" of unique values, and this pool is not automatically cleaned up.
    /// </para>
    /// <para>
    /// CondensedCollection is not thread-safe and does not perform any internal synchronization. Multiple simultaneous readers are allowed, 
    /// (provided there is no active writer), so a <see cref="System.Threading.ReaderWriterLockSlim"/> (or an ordinary exclusive lock) should be used to synchronize 
    /// multi-threaded access to an instance of the collection.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <threadsafety static="true" instance="false" />
    /// <conceptualLink target="a2dd277a-ec2e-4f4b-a42d-e31f23b94bdd" />
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class CondensedCollection<T> : IList<T>
    {
        /// <summary>
        /// List containing unique/distinct values or "interns", in the sense of a more generalized implementation of
        /// string interning (https://en.wikipedia.org/wiki/String_interning).
        /// The _indexList collection (defined below) contains offsets that point into _internPool.
        /// Used for performing fast gets into this CondensedCollection via indexer.
        /// </summary>
        internal List<T> _internPool;

        /// <summary>
        /// Lookup table for finding whether an equal value already exists in this CondensedCollection. Its ValueInfo value
        /// contains the index of the equivalent value in the _interns list and a refcount of the entries in the index
        /// (defined below).
        /// </summary>
        internal IDictionary<T, UniqueValueInfo> _objToInternLookup;

        /// <summary>
        /// Lookup table for leftover values in the intern pool. We hold on to these so (1) we can get back at their
        /// InternPoolOffsets in case the value is re-added later, and (2) so we can get at the InternPoolOffsets 
        /// with minimal fuss during Cleanup().
        /// </summary>
        internal IDictionary<T, UniqueValueInfo> _reclaimableInterns;

        /// <summary>
        /// Equality comparer used to determine whether items in the collection are equal.
        /// </summary>
        internal readonly IEqualityComparer<T> _comparer = null;

        /// <summary>
        /// Index containing integer pointers that identify elements in the _internPool collection
        /// of unique values. The width of the integers held in _indexList varies depending on 
        /// the number of unique elements in the _intern list.
        /// </summary>
        internal OffsetIndex _indexList;

        /// <summary>
        /// tracks whether the collection has stopped performing deduplication
        /// and has internally cutover to normal <see cref="List{T}"/> storage.
        /// </summary>
        private bool _hasCutover;

        /// <summary>
        /// Regular list used to hold values if this collection's "cutover" threshold is met and 
        /// there isn't enough repetition of elements to warrant deduplication.
        /// </summary>
        internal List<T> _unindexedValues;

        /// <summary>
        /// Callback that is periodically called to determine whether it's still worthwhile for
        /// this collection to perform deduplication. Called immediately before a new, unique item
        /// is added to the intern pool. Return <c>true</c>
        /// if the condensed list needs to cutover to normal <see cref="List{T}"/> storage. 
        /// </summary>
        readonly Predicate<CondensedStats> _cutoverPredicate;

        /// <summary>
        /// Event that is raised when an item in the intern pool of 
        /// unique values is no longer stored in the collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Stats provided via an event handler's <see cref="InternReclaimableEventArgs"/>
        /// are intended to be used to decide whether a cleanup should be performed.
        /// Set the <see cref="InternReclaimableEventArgs.Cleanup"/> 
        /// property to <c>true</c> to perform a cleanup after the event hander returns.
        /// </para><para>
        /// Cleanups are expensive and typically should not be performed every time 
        /// the event is raised; consider cleaning up only when the <see cref="InternReclaimableEventArgs.InternPoolCount"/>
        /// is significantly larger than the <see cref="InternReclaimableEventArgs.UniqueCount"/>.
        /// </para>
        /// <para>
        /// This event will not be fired if the collection has cutover to a non-deduplicated list.
        /// </para>
        /// <code language="cs" source="..\Samples\CleanupEvent.cs" region="CleanupEvent"/>
        /// </remarks>
        /// <conceptualLink target="606626e5-fb28-47c5-939f-a87c14d4f99a" />
        public event EventHandler<InternReclaimableEventArgs> InternedValueReclaimable;

        /// <summary>
        /// Initializes a new instance of the <see cref="CondensedCollection{T}"/> class with the 
        /// default capacity, default comparer for the type, and no cutover predicate.
        /// </summary>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public CondensedCollection()
            : this(capacity: 0, comparer: null, cutoverPredicate: null, collection: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CondensedCollection{T}"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing elements, or null to use the default <see cref="IEqualityComparer{T}"/> for the type of the object.</param>
        /// <param name="cutoverPredicate">A callback used to decide when a CondensedCollection should stop attempting to perform deduplication and store values directly. If null or unspecified, the collection will never cutover.</param>
        /// <param name="collection">The collection whose elements are copied to the new <see cref="CondensedCollection{T}"/>, or null to create an empty collection.</param>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public CondensedCollection(int capacity = 0, IEqualityComparer<T> comparer = null, Predicate<CondensedStats> cutoverPredicate = null, IEnumerable<T> collection = null)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", capacity, "capacity cannot be less than zero.");

            if (default(T) == null)
            {
                _objToInternLookup = new NullKeyDictionary<T, UniqueValueInfo>(comparer);
                _reclaimableInterns = new NullKeyDictionary<T, UniqueValueInfo>(comparer);
            }
            else
            {
                _objToInternLookup = new Dictionary<T, UniqueValueInfo>(comparer);
                _reclaimableInterns = new Dictionary<T, UniqueValueInfo>(comparer);
            }

            _internPool = new List<T>();

            if (collection == null)
            {
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _cutoverPredicate = cutoverPredicate ?? StandardCutoverPredicates.NeverCutover;
                InitializeIndex(capacity);
            }
            else
            {
                // We're being initialized with another collection.
                // check to see if it's another CondensedCollection. If so, use its comparer/cutover as defaults.
                CondensedCollection<T> otherCondensedColl = collection as CondensedCollection<T>;

                if (comparer == null)
                {
                    if (otherCondensedColl != null)
                        _comparer = otherCondensedColl.Comparer;
                    else
                        _comparer = EqualityComparer<T>.Default;
                }
                else
                {
                    _comparer = comparer;
                }

                if (cutoverPredicate == null)
                {
                    if (otherCondensedColl != null)
                        _cutoverPredicate = otherCondensedColl.CutoverPredicate;
                    else
                        _cutoverPredicate = StandardCutoverPredicates.GetDefaultCutoverPredicate(typeof(T));
                }
                else
                {
                    _cutoverPredicate = cutoverPredicate;
                }

                int indexCapacity = 0;
                ICollection icoll = collection as ICollection;
                if (icoll != null && capacity < icoll.Count)
                {
                    indexCapacity = icoll.Count;
                }
                else
                {
                    indexCapacity = capacity;
                }

                InitializeIndex(indexCapacity);

                foreach (var element in collection)
                    this.Add(element);
            }
        }
        

        private void InitializeIndex(int capacity)
        {
            _indexList = new IntListZeroStorage(capacity);
            _hasCutover = false;
        }


        private void CheckToWiden()
        {
            // TODO: this method evolved quite a bit during experimentation. Refactor.

            IntListZeroStorage singleValueIndex;
            IntListBitStorage twoValueIndex;
            IntListByteStorage byteIndex;
            IntListUShortStorage ushortIndex;
            IntListIntStorage intIndex;

            // do we need to widen the index?
            if (_internPool.Count == 1)
            {
                // we're going from one to two unique items.
                singleValueIndex = _indexList as IntListZeroStorage;
                if (CutoverPredicate(new CondensedStats(singleValueIndex.Count, UniqueCount, _internPool.Count)))
                {
                    CutoverToList(singleValueIndex.Capacity);
                    return;
                }
                twoValueIndex = new IntListBitStorage(singleValueIndex.Count, singleValueIndex.Capacity);

                _indexList = twoValueIndex;
                return;

            }
            else if (_internPool.Count == 2)
            {
                // we're going from 2 to 3 unique items:
                
                twoValueIndex = _indexList as IntListBitStorage;
                if (CutoverPredicate(new CondensedStats(twoValueIndex.Count, UniqueCount, _internPool.Count)))
                {
                    CutoverToList(twoValueIndex.Capacity);
                    return;
                }
                byteIndex = new IntListByteStorage(twoValueIndex.Capacity);
                foreach (int item in twoValueIndex)
                    byteIndex.Add(item);
                
                _indexList = byteIndex;
                return;
            }
            else if (_internPool.Count == 256)
            {
                // switch from byte to ushort index
                byteIndex = _indexList as IntListByteStorage;

                if (CutoverPredicate(new CondensedStats(byteIndex.Count, UniqueCount, _internPool.Count)))
                {
                    CutoverToList(byteIndex.Capacity);
                    return;
                }

                ushortIndex = new IntListUShortStorage(byteIndex.Capacity);
                foreach (int item in byteIndex)
                    ushortIndex.Add(item);

                _indexList = ushortIndex;
                return;
            }
            else if (_internPool.Count == 65536)
            {

                // switch from ushort to int32 index
                ushortIndex = _indexList as IntListUShortStorage;

                if (CutoverPredicate(new CondensedStats(ushortIndex.Count, UniqueCount, _internPool.Count)))
                {
                    CutoverToList(ushortIndex.Capacity);
                    return;
                }

                intIndex = new IntListIntStorage(ushortIndex.Capacity);
                foreach (int item in ushortIndex)
                    intIndex.Add(item);

                _indexList = intIndex;
                return;
            }
            else if (_internPool.Count > 65536 && _internPool.Count % 1000 == 0)
            {
                // check every 1000 unique elements to see if we should stop deduplication
                if (CutoverPredicate(new CondensedStats(this.Count, UniqueCount, _internPool.Count)))
                {
                    CutoverToList((int)(this.Count * 1.25));
                    return;
                }

            }


        }

        private void CutoverToList(int capacity)
        {
            _unindexedValues = new List<T>(capacity);
            foreach (int internPoolOffset in _indexList)
                _unindexedValues.Add(_internPool[internPoolOffset]);

            _indexList = null;
            _internPool = null;
            _objToInternLookup = null;
            _reclaimableInterns = null;
            _hasCutover = true;
        }


        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the collection, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            if (_hasCutover)
            {
                return _unindexedValues.IndexOf(item);
            }

            UniqueValueInfo info = new UniqueValueInfo();
            bool found = _objToInternLookup.TryGetValue(item, out info);
            if (!found)
            {
                return -1;
            }

            
            int position = 0;
            foreach (int internPoolOffset in _indexList)
            {
                if (internPoolOffset == info.InternPoolOffset)
                    break;

                position++;
            }

            // sanity check:
            if (position >= _indexList.Count)
            {
                // should've found that intern index somewhere in the lookup table. 
                throw new InternalCorruptionException();
            }

            return position;

        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection.</param>
        public void Insert(int index, T item)
        {
            if (_hasCutover)
            {
                _unindexedValues.Insert(index, item);
                return;
            }

            // Ordinarily we just let the underlying index do the range checking, but we have to validate here 
            // to avoid side effects... Otherwise the GetOrCreateIndexFor() call that we make first would change
            // our bookkeeping state before the exception is thrown and ruin the refcounting.
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException("index", index, "Index out of range. Must be non-negative and less than or equal to the size of the collection.");

            UniqueValueInfo info = GetOrCreateIndexFor(item);

            if (_hasCutover)
                _unindexedValues.Insert(index, item);
            else
                _indexList.Insert(index, info.InternPoolOffset);
            
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (_hasCutover)
            {
                _unindexedValues.RemoveAt(index);
                return;
            }

            int internPoolOffset = _indexList[index];
            _indexList.RemoveAt(index);

            // Sync up our internal dictionaries. Need to find
            // the item so we can reduce its refcount.
            T item = _internPool[internPoolOffset];

            UniqueValueInfo info = new UniqueValueInfo();
            bool foundUniqueValueInfo = _objToInternLookup.TryGetValue(item, out info);

            if (!foundUniqueValueInfo)
            {
                // This is a big problem that indicates that our ref counting was off...
                // We thought we had at least one copy of the item, but it wasn't found.
                // So, the whole collection is basically shot. Complain loudly.
                throw new InternalCorruptionException();
            }

            // decrement the refcount.
            info.RefCount--;
            if (info.RefCount == 0)
            {
                _objToInternLookup.Remove(item);
                _reclaimableInterns.Add(item, info);
                OnZeroRefCount(item, info.InternPoolOffset);
            }
            else if (info.RefCount < 0)
            {
                throw new InternalCorruptionException();
            }

        }


        /// <summary>
        /// Private getter. Does not perform null checking.
        /// </summary>
        /// <param name="index">zero-based index of the element to get</param>
        /// <returns>The element at the specified index.</returns>
        private T GetInternal(int index)
        {
            if (_hasCutover)
                return _unindexedValues[index];
            else
                return _internPool[_indexList[index]];
           
        }


        /// <summary>
        /// Called when an items's refcount hits zero and it's eligible to be removed from internal
        /// data structures.
        /// </summary>
        /// <param name="item">The object that can be removed from the intern collection.</param>
        /// <param name="internOffset">Offset of the removable item in the intern list of unique values.</param>
        internal virtual void OnZeroRefCount(T item, int internOffset)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<InternReclaimableEventArgs> handler = InternedValueReclaimable;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                var eventArgs = new InternReclaimableEventArgs(this.Count, this.UniqueCount, this.InternPoolCount, this.InternPoolCount);
                handler(this, eventArgs);

                if (eventArgs.Cleanup)
                {
                    this.Cleanup();
                }
            }
        }


        UniqueValueInfo GetOrCreateIndexFor(T item)
        {
            UniqueValueInfo ret;
            bool dedupedItemExists = _objToInternLookup.TryGetValue(item, out ret);

            if (!dedupedItemExists)
            {
                if (_reclaimableInterns.Count > 0)
                {
                    // Check in the table of reclaimable values. We might be able to resurrect:
                    dedupedItemExists = _reclaimableInterns.TryGetValue(item, out ret);
                    if (dedupedItemExists)
                    {
                        ret.RefCount = 1;
                        _objToInternLookup.Add(item, ret);
                        _reclaimableInterns.Remove(item);
                        return ret;
                    }
                }

                ret = new UniqueValueInfo();

                // We're adding a new interned value. Ensure the index is wide enough to accommodate.
                CheckToWiden();

                if (_hasCutover)
                {
                    // CheckToWiden just expanded collection to not do indexing. Returning null.
                    return null;
                }

                ret.InternPoolOffset = _internPool.Count;
                ret.RefCount = 1;
                _internPool.Add(item);
                _objToInternLookup.Add(item, ret);
            }
            else
            {
                // increment refcount.
                ret.RefCount++;
            }

            return ret;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                   return GetInternal(index);
            }

            set
            {
                if (_hasCutover)
                {
                    _unindexedValues[index] = value;
                    return;
                }

                
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException("index", index, "Index out of range. Must be non-negative and less than the size of the collection.");

                
                // get the old value. We'll need to decrement its refcount.
                T oldValue = GetInternal(index);

                if (_comparer.Equals(oldValue, value))
                {
                    // Assigning the same value. Nothing to do.
                    return;
                }
                else
                {
                    // Assigning a new value. Decrement refcount for old value.
                    var oldIndexInfo = _objToInternLookup[oldValue];
                    oldIndexInfo.RefCount--;
                    if (oldIndexInfo.RefCount == 0)
                    {
                        // remove the item from our internal collections.
                        _objToInternLookup.Remove(oldValue);
                        _reclaimableInterns.Add(oldValue, oldIndexInfo);
                        OnZeroRefCount(oldValue, oldIndexInfo.InternPoolOffset);
                    }
                    else if (oldIndexInfo.RefCount < 0)
                    {
                        throw new InternalCorruptionException();
                    }

                }


                // Set the new value:
                var indexInfo = GetOrCreateIndexFor(value);

                if (_hasCutover)
                    _unindexedValues[index] = value;
                else
                    _indexList[index] = indexInfo.InternPoolOffset;

            } // end set
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        public void Add(T item)
        {
            if (_hasCutover)
            {
                _unindexedValues.Add(item);
            }
            else
            {
                UniqueValueInfo info;

                info = GetOrCreateIndexFor(item);

                if (_hasCutover)
                    _unindexedValues.Add(item);
                else
                    _indexList.Add(info.InternPoolOffset);
                
            }

        }


        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The capacity of the collection is not changed.
        /// </para><para>
        /// This method does not raise the <see cref="InternedValueReclaimable"/> event.
        /// </para>
        /// </remarks>
        public void Clear()
        {
            int latestCapacity = this.Capacity;

            _internPool.Clear();
            _objToInternLookup.Clear();
            _reclaimableInterns.Clear();
            _unindexedValues = null;

            _indexList = null;
            InitializeIndex(latestCapacity);
        }


        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>The object to locate in the collection.</returns>
        public bool Contains(T item)
        {
            if (_hasCutover)
            {
                return _unindexedValues.Contains(item);
            }

            UniqueValueInfo info;
            bool found = _objToInternLookup.TryGetValue(item, out info);
            if (found && info.RefCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from <see cref="CondensedCollection{T}"/>. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "arrayIndex argument cannot be less than 0.");
            if ((array.Length - arrayIndex) < this.Count)
                throw new ArgumentException("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");


            foreach (var item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }

        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (_hasCutover)
                    return _unindexedValues.Count;
                else
                    return _indexList.Count;
            }
        }


        /// <summary>
        /// Gets count of unique values held in the collection. If T is nullable and null element(s) are present,
        /// null will be counted as a unique value.
        /// </summary>
        public int UniqueCount
        {
            get
            {
                if (_hasCutover)
                {
                    return _unindexedValues.Distinct().Count();
                }
                else
                {
                    int ret = _objToInternLookup.Count;

                    return ret;
                }
            }
        }


        /// <summary>
        /// Gets count of values held internally in the collection's intern pool of unique values,
        /// or -1 if the collection has cutover and is no longer performing deduplication.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="CondensedCollection{T}"/> does not automatically reclaim values from the 
        /// intern pool. An intern pool that is significantly larger then the <see cref="UniqueCount"/>
        /// indicates that many values being held in the pool are no longer being used by the CondensedCollection.
        /// Call <see cref="Cleanup"/> as needed to reclaim memory from the intern pool or subscribe to the
        /// <see cref="InternedValueReclaimable"/> event.
        /// </para>
        /// </remarks>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public int InternPoolCount
        {
            get
            {
                if (_hasCutover)
                {
                    return -1;
                }
                else
                {
                    return _internPool.Count;
                }
            }
        }


        /// <summary>
        /// Gets count of unused items in the collection's intern pool that 
        /// would be freed (made eligible for GC) by a cleanup operation,
        /// or -1 if the collection has cutover and is no longer performing deduplication.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="CondensedCollection{T}"/> does not automatically reclaim values from the 
        /// intern pool. A high <see cref="ReclaimableInternsCount"/> value
        /// indicates that many values being held in the pool are no longer being used by the CondensedCollection.
        /// Call <see cref="Cleanup"/> as needed to reclaim memory from the intern pool or subscribe to the
        /// <see cref="InternedValueReclaimable"/> event.
        /// </para>
        /// <para>
        /// The <see cref="ReclaimableInternsCount"/> property is equivalent to subtracting the <see cref="UniqueCount"/>
        /// property from the <see cref="InternPoolCount"/> property.
        /// </para>
        /// </remarks>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public int ReclaimableInternsCount
        {
            get
            {
                if (_hasCutover)
                {
                    return -1;
                }
                else
                {
                    return _reclaimableInterns.Count;
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the original collection.</returns>
        public bool Remove(T item)
        {
            if (_hasCutover)
            {
                return _unindexedValues.Remove(item);
            }

            int indexToRemove = this.IndexOf(item);
            if (indexToRemove < 0)
                return false;
            else
            {
                this.RemoveAt(indexToRemove);
                return true;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (_hasCutover)
            {
                foreach (var item in _unindexedValues)
                    yield return item;
            }
            else
            {
                for (int i = 0; i < Count; ++i)
                    yield return _internPool[_indexList[i]];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used by the <see cref="CondensedCollection{T}"/>
        /// to compare elements while performing deduplication.
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        /// <summary>
        /// Gets the width of the internal index currently used by a <see cref="CondensedCollection{T}"/>.
        /// </summary>
        public IndexType IndexType
        {
            get {
                if (_hasCutover)
                    return IndexType.NoIndex;
                else
                    return _indexList.IndexType;
            }
        }

        /// <summary>
        /// Gets whether the collection has stopped performing deduplication
        /// and has internally cutover to normal <see cref="List{T}"/> storage.
        /// </summary>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public bool HasCutover
        {
            get { return _hasCutover; }
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity
        {
            get
            {
                if (_hasCutover)
                    return _unindexedValues.Capacity;
                else
                    return _indexList.Capacity;
            }
            set
            {
                if (_hasCutover)
                    _unindexedValues.Capacity = value;
                else
                   _indexList.Capacity = value;
            }
        }

        /// <summary>
        /// Gets the callback that is periodically called to determine whether it's worthwhile for
        /// this collection to perform deduplication. Called immediately before a new, unique item
        /// is added to the intern pool. Return <c>true</c>
        /// if the condensed list needs to cutover to normal <see cref="List{T}"/> storage. 
        /// </summary>
        /// <conceptualLink target="23ce826c-4e23-4c13-9ac9-63cdccb22d86" />
        public Predicate<CondensedStats> CutoverPredicate
        {
            get
            {
                return _cutoverPredicate;
            }
        }

        #region Cleanup routines
        /// <summary>
        /// Rebuilds the collection's internal data structures, reclaiming unused items and 
        /// potentially freeing memory.
        /// </summary>
        /// <returns>Count of items removed from the intern pool, or -1 if the collection has
        /// already cutover to a non-deduplicated list.</returns>
        /// <remarks>
        /// <para>
        /// Cleanups are potentially expensive operations that should only be performed on an
        /// occasional basis. Instead of calling this method directly, you can use the 
        /// <see cref="CondensedCollection{T}.InternedValueReclaimable"/> event
        /// as an occasion to decide whether a cleanup should be performed--consider performing cleanups
        /// only when the <see cref="CondensedCollection{T}.InternPoolCount"/> is substantially larger 
        /// than the <see cref="CondensedCollection{T}.UniqueCount"/>.
        /// </para>
        /// <para>
        /// Cleanup cannot be performed on a CondensedCollection that has cutover to normal
        /// (non-deduplicated) list storage because its internal pool and refcounting information
        /// has been lost. Reconstruct the collection from itself to rebuild it and 
        /// restart deduplication.
        /// </para>
        /// </remarks>
        /// <conceptualLink target="606626e5-fb28-47c5-939f-a87c14d4f99a" />
        public virtual int Cleanup()
        {
            if (_hasCutover)
            {
                return -1;
            }
            if (_reclaimableInterns.Count == 0)
            {
                return 0;
            }
            if (_internPool.Count < _objToInternLookup.Count)
            {
                // This shouldn't be possible. Something has gone horribly wrong.
                throw new InternalCorruptionException(string.Format("Unrecoverable inconsistency detected during cleanup. Intern pool count: {0}, obj lookup count: {1}.", _internPool.Count, _objToInternLookup.Count));
            }

            // TODO: optimize for cleaning up single item?

            // Remove and get collection of intern pool offsets that are no longer used:
            var removedInternIndexes = CleanupUnreferencedFromPool();

            // Fix up internal indexes with new offsets into the intern pool
            int[] newIndexes = CreateIndexConversionLookup(removedInternIndexes);
            Reindex(newIndexes);
            ReindexObjToInternLookup(newIndexes);
            _reclaimableInterns.Clear();

            return removedInternIndexes.Count;
        }


        /// <summary>
        /// Removes unreferenced elements from the intern pool.
        /// </summary>
        /// <returns>Collection of indexes into the intern pool that were removed.</returns>
        private List<int> CleanupUnreferencedFromPool()
        {
            // First, figure out which items in the intern pool need to be removed.
            BitArray internsToRemove = new BitArray(_internPool.Count, false);

            foreach (UniqueValueInfo info in _reclaimableInterns.Values)
            {
                internsToRemove.Set(info.InternPoolOffset, true);
            }

            // Next, remove unused items from the pool.

            var removedInternIndexes = new List<int>();
            // TODO: Repeatedly calling RemoveAt() is a *very* inefficient way to remove multiple items from a list. Improve.
            for (int i = _internPool.Count - 1; i >= 0; --i)
            {
                if (internsToRemove[i])
                {
                    _internPool.RemoveAt(i);
                    removedInternIndexes.Add(i);
                }
            }

            removedInternIndexes.Reverse();
            return removedInternIndexes;

        }


        private int[] CreateIndexConversionLookup(List<int> removedInternIndexes)
        {
            if (_hasCutover)
                throw new NotImplementedException();

            int arraySize = _internPool.Count + removedInternIndexes.Count;
            int[] ret = new int[arraySize];

            int nextToRemove = removedInternIndexes[0];
            int indexToRemoveIndex = 0;

            int j = 0; // translated lookup val
            for (int i = 0; i < arraySize; ++i)
            {
                if (i == nextToRemove)
                {
                    ret[i] = -42; // checked in debug assert later
                    if (indexToRemoveIndex == removedInternIndexes.Count - 1)
                    {
                        // that was the last index we were removing
                        nextToRemove = int.MinValue;
                    }
                    else
                    {
                        nextToRemove = removedInternIndexes[++indexToRemoveIndex];
                    }
                }
                else
                {
                    ret[i] = j;
                    j++;
                }

            }

            return ret;
        }


        void ReindexObjToInternLookup(int[] newIndexes)
        {
            if (_hasCutover)
                throw new NotImplementedException();

            foreach (var uniqueInfo in _objToInternLookup.Values)
            {
                uniqueInfo.InternPoolOffset = newIndexes[uniqueInfo.InternPoolOffset];
                Debug.Assert(uniqueInfo.InternPoolOffset != -42);
            }
        }

        void Reindex(int[] newIndexes)
        {
            // TODO: opportunity for parallelization? Should probably offer some kind cleanup options.
            if (CanIndexBeNarrowed())
            {
                OffsetIndex narrowedIndex = CreateEmptyNarrowedIndex();
                foreach (int oldOffset in _indexList)
                {
                    narrowedIndex.Add(newIndexes[oldOffset]);
                    Debug.Assert(newIndexes[oldOffset] != -42);
                }
                _indexList = narrowedIndex;
            }
            else
            {
                for (int i = 0; i < _indexList.Count; ++i)
                {
                    _indexList[i] = newIndexes[_indexList[i]];
                    Debug.Assert(_indexList[i] != -42);
                }
            }
        }

        internal bool CanIndexBeNarrowed()
        {
            switch (this.IndexType)
            {
                case IndexType.ZeroBytes:
                    return false;
                case IndexType.OneBit:
                    if (_internPool.Count <= 1)
                        return true;
                    break;
                case IndexType.OneByte:
                    if (_internPool.Count <= 2)
                        return true;
                    break;
                case IndexType.TwoBytes:
                    if (_internPool.Count <= byte.MaxValue + 1)
                        return true;
                    break;
                case IndexType.FourBytes:
                    if (_internPool.Count <= ushort.MaxValue + 1)
                        return true;
                    break;
                case IndexType.NoIndex:
                    throw new NotSupportedException("Cleanup not supported for collection that has been cutover.");
                default:
                    throw new NotImplementedException("Unknown index type encountered during cleanup.");
            }

            return false;
        }


        internal OffsetIndex CreateEmptyNarrowedIndex()
        {
            if (_internPool.Count <= 1)
            {
                return new IntListZeroStorage(_indexList.Count);
            }
            else if (_internPool.Count <= 2)
            {
                return new IntListBitStorage(0, _indexList.Count);
            }
            else if (_internPool.Count <= byte.MaxValue + 1)
            {
                return new IntListByteStorage(_indexList.Count);
            }
            else if (_internPool.Count <= ushort.MaxValue + 1)
            {
                return new IntListUShortStorage(_indexList.Count);
            }
            else
            {
                // shouldn't be possible. Count of _internPool items must've gone
                // up since the call to CanIndexBeNarrowed.
                throw new InternalCorruptionException("Inconsistency detected during cleanup's index recreation.");
            }
        }
        #endregion // Cleanup routines

    } // class CondensedCollection

    [Serializable]
    internal class UniqueValueInfo
    {
        internal int InternPoolOffset;
        internal int RefCount;
    }

    

}
