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

using Condensed;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass()]
    public class NullableTests
    {
        [TestMethod()]
        public void CondensedCollectionTest()
        {
            var l = new DedupedList<Nullable<int>>();
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);
            Assert.AreEqual(0, l.Capacity);
            Assert.AreEqual(EqualityComparer<int?>.Default, l.Comparer);
            Assert.IsFalse(l.IsReadOnly);
        }

        [TestMethod()]
        public void CondensedCollectionTest1()
        {
            var l = new DedupedList<Nullable<int>>(1000);

            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);
            Assert.AreEqual(1000, l.Capacity);
            Assert.AreEqual(EqualityComparer<int?>.Default, l.Comparer);
            Assert.IsFalse(l.IsReadOnly);
        }


        [TestMethod()]
        public void IndexOfTest()
        {
            var l = new DedupedList<string>(100);
            for (int i = 0; i < 100; ++i)
                l.Add(i.ToString());

            l[42] = null;
            Assert.IsNull(l[42]);
            Assert.AreEqual(42, l.IndexOf(null));
            Assert.AreEqual(-1, l.IndexOf("42"));
        }

        [TestMethod()]
        public void IndexOfTest1()
        {
            var l = new DedupedList<string>(10);
            l.Add(null);
            for (int i = 1; i < 10; ++i)
                l.Add("foo");

            Assert.IsNull(l[0]);
            Assert.AreEqual(0, l.IndexOf(null));
            Assert.AreEqual(1, l.IndexOf("foo"));
        }

        [TestMethod()]
        public void IndexOfTest2()
        {
            var l = new DedupedList<string>(10);
            for (int i = 0; i < 9; ++i)
                l.Add("foo");

            l.Add(null);

            Assert.IsNull(l[9]);
            Assert.AreEqual(9, l.IndexOf(null));
            Assert.AreEqual(0, l.IndexOf("foo"));
        }

        [TestMethod()]
        public void IndexOfTest3()
        {
            var l = new DedupedList<string>(10);
            for (int i = 0; i < 10; ++i)
                l.Add(null);


            Assert.AreEqual(0, l.IndexOf(null));
            Assert.AreEqual(-1, l.IndexOf("foo"));

        }

        [TestMethod()]
        public void IndexOfTest4()
        {
            var l = new DedupedList<string>(10);
            for (int i = 0; i < 9; ++i)
                l.Add(null);
            l.Add("foo");


            Assert.AreEqual(0, l.IndexOf(null));
            Assert.AreEqual(9, l.IndexOf("foo"));
            Assert.AreEqual(-1, l.IndexOf("bar"));

        }

        [TestMethod()]
        public void InsertTest()
        {
            var l = new DedupedList<int?>(1000);
            for (int i = 0; i < 1000; ++i)
                l.Add(i);

            l.Insert(500, -42);
            Assert.AreEqual(1001, l.Count);
            Assert.AreEqual(1001, l.UniqueCount);
            Assert.AreEqual(-42, l[500]);

        }

        [TestMethod()]
        public void InsertTest1()
        {
            var l = new DedupedList<int?>(1000);
            for (int i = 0; i < 1000; ++i)
                l.Add(i);

            l.Insert(500, null);
            Assert.AreEqual(1001, l.Count);
            Assert.AreEqual(1001, l.UniqueCount);
            Assert.AreEqual(null, l[500]);

            l.Insert(500, null);
            Assert.AreEqual(1002, l.Count);
            Assert.AreEqual(1001, l.UniqueCount);
            Assert.AreEqual(null, l[500]);
            Assert.AreEqual(null, l[501]);

            l.Insert(1001, null);
            Assert.AreEqual(1003, l.Count);
            Assert.AreEqual(1001, l.UniqueCount);
            Assert.AreEqual(null, l[500]);
            Assert.AreEqual(null, l[501]);
            Assert.AreEqual(null, l[1001]);
            Assert.AreEqual(999, l[1002]);

        }


        [TestMethod()]
        public void InsertTest2()
        {
            var l = new DedupedList<int?>(10);
            l.Add(null);

            // insert non-null items in front of the null
            // element, make sure the null one gets shifted up correctly.
            for (int i = 0; i < 9; ++i)
                l.Insert(0, i);

            Assert.AreEqual(null, l[9]);

        }


        [TestMethod()]
        public void RemoveAtTest()
        {
            var l = new DedupedList<string>(1000);
            for (int i = 0; i < 1000; ++i)
                l.Add(null);

            l[500] = "foo";
            l.RemoveAt(0);
            Assert.AreEqual(999, l.Count);
            Assert.AreEqual(2, l.UniqueCount);
            Assert.AreEqual("foo", l[499]);
            Assert.IsNull(l[500]);
        }

        [TestMethod()]
        public void ClearTest()
        {
            var l = new DedupedList<int?>();

            l.Add(42);
            l.Add(null);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l.UniqueCount);

            l.Clear();
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);

            l.Add(null);
            l.Add(43);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l.UniqueCount);
            Assert.IsNull(l[0]);
            Assert.AreEqual(43, l[1]);

            l.Clear();
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);

            l.Add(44);
            l.Add(null);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l.UniqueCount);
            Assert.AreEqual(44, l[0]);
            Assert.IsNull(l[1]);

            l.Clear();
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);

        }

        [TestMethod()]
        public void ContainsTest()
        {
            var l = new DedupedList<string>();
            Assert.IsFalse(l.Contains("foo"));
            Assert.IsFalse(l.Contains(null));

            l.Add("foo");
            l.Add(null);

            Assert.IsTrue(l.Contains("foo"));
            Assert.IsTrue(l.Contains(null));
            Assert.IsFalse(l.Contains("bar"));

            l.RemoveAt(0);
            Assert.IsFalse(l.Contains("foo"));
            Assert.IsTrue(l.Contains(null));

            l.RemoveAt(0);
            Assert.IsFalse(l.Contains(null));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            var l = new DedupedList<long?>();
            for (long i = 0; i < 100; ++i)
            {
                if (i % 42 == 0)
                    l.Add(null);
                else
                    l.Add(i);
            }

            long?[] target = new long?[100];
            l.CopyTo(target, 0);
            for (int i = 0; i < 100; ++i)
            {
                if (i % 42 == 0)
                    Assert.IsNull(target[i]);
                else
                    Assert.AreEqual(i, target[i]);
            }

        }

        [TestMethod()]
        public void RemoveTest()
        {
            bool ret;
            var l = new DedupedList<int?>();

            l.Add(42);
            l.Add(43);
            l.Add(null);

            ret = l.Remove(null);
            Assert.IsTrue(ret);
            Assert.AreEqual(42, l[0]);
            Assert.AreEqual(43, l[1]);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l.UniqueCount);

            l.Clear();
            ret = l.Remove(null);
            Assert.IsFalse(ret);
            ret = l.Remove(43);
            Assert.IsFalse(ret);

            l.Add(null);
            l.Add(42);
            l.Add(43);

            ret = l.Remove(42);
            Assert.IsTrue(ret);
            Assert.IsNull(l[0]);
            Assert.AreEqual(43, l[1]);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l.UniqueCount);
            l.Clear();

            l.Add(null);
            l.Add(43);

            l.Remove(null);
            Assert.AreEqual(43, l[0]);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l.UniqueCount);
            l.Remove(43);
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);

            l.Add(null);
            l.Add(null);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(1, l.UniqueCount);
            l.Remove(null);
            Assert.IsNull(l[0]);
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l.UniqueCount);
            l.Remove(null);
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, l.UniqueCount);

           
        }


    }
}
