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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Condensed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Condensed.Linq;

namespace UnitTests
{
    [TestClass()]
    public class CondensedCollectionTests
    {
        [TestMethod()]
        public void CondensedCollectionTest()
        {
            var l = new DedupedList<int>();

            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);
            Assert.AreEqual(0, l.Capacity);
            Assert.AreEqual(EqualityComparer<int>.Default, l.Comparer);
            Assert.IsFalse(l.IsReadOnly);

        }

        [TestMethod()]
        public void CondensedCollectionTest1()
        {
            var l = new DedupedList<string>(1000);

            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);
            Assert.AreEqual(1000, l.Capacity);
            Assert.AreEqual(EqualityComparer<string>.Default, l.Comparer);
            Assert.IsFalse(l.IsReadOnly);
        }

        [TestMethod()]
        public void CondensedCollectionTest2()
        {
            // Custom comparer that ignores time component of DateTime:
            var customComparer = new DateOnlyEqualityComparer();
            var l = new DedupedList<DateTime>(99, customComparer);

            Assert.AreEqual(99, l.Capacity);
            Assert.AreEqual(customComparer, l.Comparer);

            var d1 = new DateTime(2015, 10, 31, 8, 0, 0);
            var d2 = new DateTime(2015, 10, 31, 15, 1, 6);

            l.Add(d1);
            l.Add(d2);

            Assert.AreEqual(1, l.UniqueCount);
            Assert.AreEqual(2, l.Count);

            // exercise setter, which also uses comparer:
            l[0] = d2;
            Assert.AreEqual(1, l.UniqueCount);
            Assert.AreEqual(2, l.Count);
            
            // After it's all said and done, neither element
            // should contain the second date:
            Assert.AreNotEqual(l[0], d2);
            Assert.AreNotEqual(l[1], d2);

        }

        [TestMethod()]
        public void IndexOfTest()
        {
            var l = new DedupedList<long>(10000);
            for (int i = 0; i < 10000; ++i)
                l.Add(i);

            for (int i = 0; i < 10000; ++i)
                Assert.AreEqual(i, l.IndexOf(i));

            Assert.AreEqual(-1, l.IndexOf(-42));
            Assert.AreEqual(-1, l.IndexOf(10000000));
        }

        [TestMethod()]
        public void InsertTest()
        {
            var l = new DedupedList<TimeSpan>();

            for (int i = 0; i < 10; ++i)
                l.Add(TimeSpan.FromSeconds(i));
            for (int i = 0; i < 10; ++i)
                Assert.AreEqual(i, l[i].TotalSeconds);

            l.Insert(0, TimeSpan.FromSeconds(99));
            Assert.AreEqual(99, l[0].TotalSeconds);

            for (int i = 1; i < 11; ++i)
                Assert.AreEqual(i-1, l[i].TotalSeconds);
        }

        
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertOutOfRangeTest1()
        {
            var l = new DedupedList<Guid>();
            for (int i = 0; i < 300; ++i)
                l.Add(Guid.NewGuid());

            Assert.AreEqual(300, l.Count);
            Assert.AreEqual(300, l.UniqueCount);

            try
            {
                l.Insert(-1, new Guid());
            }
            catch
            {
                // make sure there were no side effects.
                Assert.AreEqual(300, l.Count);
                Assert.AreEqual(300, l.UniqueCount);

                throw;
            }

            
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertOutOfRangeTest2()
        {

            var l = new DedupedList<Guid>();
            for (int i = 0; i < 10; ++i)
                l.Add(Guid.NewGuid());

            Assert.AreEqual(10, l.Count);
            Assert.AreEqual(10, l.UniqueCount);

            l.Insert(10, new Guid()); // inserts at end are OK, shouldn't throw

            try
            {
                l.Insert(12, new Guid());
            }
            catch
            {
                // make sure there were no side effects.
                Assert.AreEqual(11, l.Count);
                Assert.AreEqual(11, l.UniqueCount);

                throw;
            }

        }

        [TestMethod()]
        public void InsertAtEnd()
        {
            var l = new DedupedList<long>();
            l.Insert(0, 484848);
           
        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            var l = new DedupedList<int>();
            for (int i = 0; i < 100; ++i)
                l.Add(i);

            // remove first item.
            l.RemoveAt(0);

            Assert.AreEqual(99, l.Count);
            Assert.AreEqual(99, l.UniqueCount);
            // make sure all remaining elements were shifted down.
            for (int i = 0; i < 99; ++i)
                Assert.AreEqual(i + 1, l[i]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAtOutOfRangeTest1()
        {
            var l = new DedupedList<short>();
            for (short i = 0; i < 10; ++i)
                l.Add(i);

            try
            {
                l.RemoveAt(-1);
            }
            catch
            {
                Assert.AreEqual(10, l.Count);
                Assert.AreEqual(10, l.UniqueCount);

                throw;
            }

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemoveAtOutOfRangeTest2()
        {
            var l = new DedupedList<long>();
            for (long i = 0; i < 999; ++i)
                l.Add(i);

            try
            {
                l.RemoveAt(123456);
            }
            catch
            {
                Assert.AreEqual(999, l.Count);
                Assert.AreEqual(999, l.UniqueCount);

                throw;
            }

        }

        
        [TestMethod()]
        public void AddTest()
        {
            var l = new DedupedList<int>(1000000);
            for (int i = 0; i < 1000000; ++i)
                l.Add(i % 42);

            Assert.AreEqual(1000000, l.Count);
            Assert.AreEqual(42, l.UniqueCount);
        }

        [TestMethod()]
        public void AddNullTest()
        {
            var l = new DedupedList<string>();
            l.Add(null);
            l.Add("hello world");
            l.Add(null);

            Assert.AreEqual(3, l.Count);
            Assert.AreEqual(2, l.UniqueCount);
            Assert.IsNull(l[0]);
            Assert.AreEqual("hello world", l[1]);
            Assert.IsNull(l[2]);

        }


        [TestMethod()]
        public void ClearTest()
        {
            var l = new DedupedList<int>(100000);
            for (int i = 0; i < 54321; ++i)
                l.Add(i % 987);

            Assert.AreEqual(54321, l.Count);
            Assert.AreEqual(987, l.UniqueCount);
            Assert.IsTrue(l.Capacity >= 100000);

            l.Clear();
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);
            Assert.IsTrue(l.Capacity >= 100000);

        }

        [TestMethod()]
        public void ContainsTest()
        {
            var l = new DedupedList<long>(1000);
            for (long i = 0; i < 651; ++i)
                l.Add(i);

            Assert.AreEqual(651, l.Count);
            Assert.AreEqual(651, l.UniqueCount);
            Assert.IsTrue(l.Capacity >= 1000);

            // add and remove a value to the list a couple of times,
            // ensuring contains behaves as expected.
            Assert.IsTrue(l.Contains(333));
            l.Remove(333);
            Assert.IsFalse(l.Contains(333));
            l.Add(333);
            Assert.IsTrue(l.Contains(333));
            l.Remove(333);
            Assert.IsFalse(l.Contains(333));

            // verify our refcounting, while we're at it:
            l.Add(333);
            l.Add(333);
            Assert.IsTrue(l.Contains(333));
            l.Remove(333);
            Assert.IsTrue(l.Contains(333));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            var l = new DedupedList<int>(300);
            for (int i = 0; i < 231; ++i)
                l.Add(i % 3);

            Assert.AreEqual(231, l.Count);
            Assert.AreEqual(3, l.UniqueCount);
            Assert.IsTrue(l.Capacity >= 300);

            int[] dest = new int[231];
            l.CopyTo(dest, 0);

            for (int i = 0; i < 231; ++i)
                Assert.AreEqual(i % 3, dest[i]);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyToBadOffsetTest()
        {
            var l = new DedupedList<int>();
            for (int i = 0; i < 6543; ++i)
                l.Add(i);

            Assert.AreEqual(6543, l.Count);

            int[] dest = new int[1000];
            l.CopyTo(dest, 9000); // not enough room after specified offset, should throw.

        }

        [TestMethod()]
        public void RemoveTest()
        {
            var l = new DedupedList<string>(9876);
            for (int i = 0; i < 9876; ++i)
                l.Add((i % 5000).ToString());

            // make sure nothing changes if the item isn't found:
            Assert.IsFalse(l.Remove("I'm not in the list"));
            Assert.AreEqual(9876, l.Count);
            Assert.AreEqual(5000, l.UniqueCount);

            // remove first item:
            Assert.IsTrue(l.Remove("0"));
            Assert.AreEqual(9875, l.Count);
            Assert.AreEqual(5000, l.UniqueCount); // shouldn't have changed since there's another "0"
            // remaining elements should've shifted down:
            Assert.AreEqual("1", l[0]);
            // should be another "0" later in the list:
            Assert.IsTrue(l.Contains("0"));
            Assert.AreEqual("0", l[4999]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOutOfRange()
        {
            var l = new DedupedList<long>(100);

            for (long i = 0; i < 100; ++i )
            {
                l.Add(i % 33);
            }

            l[100] = -1;
        }

        [TestMethod()]
        public void EnumerateEmptyTest()
        {
            var l = new DedupedList<Guid>();

            int counter = 0;
            foreach (var item in l)
            {
                counter++;
            }
            Assert.AreEqual(0, counter);
        }

        [TestMethod()]
        public void EnumerateSingleItemTest()
        {
            var l = new DedupedList<IPAddress>();
            l.Add(IPAddress.Parse("8.8.8.8"));

            int counter = 0;
            foreach (var item in l)
            {
                counter++;
            }
            Assert.AreEqual(1, counter);
        }

        [TestMethod()]
        public void EnumerateNulls()
        {
            var l = new DedupedList<string>();

            for (int i = 0; i < 1732; ++i)
                l.Add(null);

            int counter = 0;
            foreach (var item in l)
            {
                counter++;
                if (item != null)
                    Assert.Fail("Expected null");
            }
            Assert.AreEqual(1732, counter);
        }

        [TestMethod()]
        public void EnumerateNullable()
        {
            var l = new DedupedList<int?>();

            for (int i = 0; i < 234; ++i)
            {
                int val = i % 11;
                if (val == 7)
                    l.Add(null);
                else
                    l.Add(val);
            }

            int index = 0;
            int nullCount = 0;
            foreach (var item in l)
            {
                Assert.AreEqual(l[index], item);
                if (item == null)
                    nullCount++;

                index++;
            }

            Assert.AreNotEqual(0, nullCount);
            Assert.AreEqual(l.Count((elem) => elem == null), nullCount);
            Assert.AreEqual(l.Count, index);
            
        }
    }
}