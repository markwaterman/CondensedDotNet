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
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass()]
    public class BitCollectionTests
    {
        [TestMethod()]
        public void BitCollectionTest()
        {
            var bl = new BitCollection();
            bl.Add(true);
            bl.Add(false);
            Assert.IsTrue(bl[0]);
            Assert.IsFalse(bl[1]);
        }

        [TestMethod()]
        public void BitCollectionTest1()
        {
            var bl = new BitCollection(100);
            for (int i = 0; i < 150; ++i)
            {
                bl.Add((i % 2 == 0));
            }
            Assert.IsTrue(bl[50]);
            Assert.IsFalse(bl[123]);
            

        }

        [TestMethod()]
        public void BitCollectionTest2()
        {
            var bl = new BitCollection(50000, true, 50000);

            Assert.AreEqual(50000, bl.Count);
            Assert.IsTrue(bl[0]);
            Assert.IsTrue(bl[25000]);
            Assert.IsTrue(bl[49999]);
        }


        [TestMethod()]
        public void AddTest()
        {
            var bl = new BitCollection(0, false, 0);
            bl.Add(true);

            Assert.IsTrue(bl[0]);
            Assert.AreEqual(1, bl.Count);
            Assert.AreEqual(64, bl.Capacity);
        }

        [TestMethod()]
        public void ClearTest()
        {
            var bl = new BitCollection(10);
            for (int i = 0; i < 1000; ++i)
                bl.Add((i % 3 == 0));

            Assert.AreEqual(1000, bl.Count);
            bl.Clear();
            Assert.AreEqual(0, bl.Count);

        }

        [TestMethod()]
        public void ContainsTest()
        {
            var bl = new BitCollection(1000);
            for (int i = 0; i < 1000; ++i)
                bl.Add(false);

            Assert.IsFalse(bl.Contains(true));
            bl[999] = true;
            Assert.IsTrue(bl.Contains(true));
            bl.RemoveAt(999);
            Assert.IsFalse(bl.Contains(true));

            // Test inverse:
            bl.Clear();
            for (int i = 0; i < 1000; ++i)
                bl.Add(true);

            Assert.IsFalse(bl.Contains(false));
            bl[999] = false;
            Assert.IsTrue(bl.Contains(false));
            bl.RemoveAt(999);
            Assert.IsFalse(bl.Contains(false));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            var bl = new BitCollection(12345);
            for (int i = 0; i < 12345; ++i)
                bl.Add((i % 13 == 0));

            var dest = new bool[12345];
            bl.CopyTo(dest, 0);


            for (int i = 0; i < 12345; ++i)
            {
                if (dest[i] != (i % 13 == 0))
                    Assert.Fail();
            }
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            var bl = new BitCollection();
            for (int i = 0; i < 10000; ++i)
                bl.Add((i % 3) == 0);

            int index = 0;
            foreach (bool val in bl)
            {
                if (val != ((index % 3) == 0))
                    Assert.Fail();
                index++;
            }
            Assert.AreEqual(bl.Count, index);
        }

        [TestMethod()]
        public void GetEmptyEnumeratorTest()
        {
            var bl = new BitCollection();

            int counter = 0;
            foreach (bool val in bl)
            {
                counter++;
            }
            Assert.AreEqual(0, counter);
        }

        [TestMethod()]
        public void GetEnumeratorSingleItemTest()
        {
            var bl = new BitCollection();
            bl.Add(false);

            int counter = 0;
            foreach (bool val in bl)
            {
                Assert.IsFalse(val);
                counter++;
            }
            Assert.AreEqual(1, counter);
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            var bl = new BitCollection();
            for (int i = 0; i < 1000; ++i)
                bl.Add(false);

            bl[501] = true;
            Assert.AreEqual(501, bl.IndexOf(true));

            bl.Clear();
            for (int i = 0; i < 1000; ++i)
                bl.Add(true);

            bl[501] = false;
            Assert.AreEqual(501, bl.IndexOf(false));
        }

        [TestMethod()]
        public void InsertTest()
        {
            var bl = new BitCollection();
            for (int i = 0; i < 1234; ++i)
                bl.Add(false);

            Assert.AreEqual(1234, bl.Count);
            bl.Insert(500, true);
            bl.Insert(500, false);
            bl.Insert(500, true);

            Assert.IsTrue(bl[500]);
            Assert.IsFalse(bl[501]);
            Assert.IsTrue(bl[502]);
            Assert.AreEqual(1237, bl.Count);
        }

        [TestMethod()]
        public void InsertEndTest()
        {
            var bl = new BitCollection();

            // insert into empty collection
            bl.Insert(0, true);
            Assert.AreEqual(1, bl.Count);
            Assert.IsTrue(bl[0]);

            // insert at end of single word
            bl = new BitCollection();
            for (int i = 0; i < 64; ++i)
                bl.Add((i % 2) == 0);

            Assert.AreEqual(64, bl.Capacity);
            bl.Insert(64, true);
            Assert.IsTrue(bl[64]);
            Assert.AreEqual(128, bl.Capacity);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var bl = new BitCollection();
            Assert.IsFalse(bl.Remove(true));
            Assert.IsFalse(bl.Remove(false));

            bl.Add(true);
            Assert.IsFalse(bl.Remove(false));
            Assert.IsTrue(bl.Remove(true));

            bl.Add(false);
            Assert.IsFalse(bl.Remove(true));
            Assert.IsTrue(bl.Remove(false));

            bl = new BitCollection(100, true, 100);
            bl[99] = false;
            Assert.IsTrue(bl.Remove(false));
            Assert.IsFalse(bl.Contains(false));
            Assert.AreEqual(99, bl.Count);

            bl = new BitCollection(100, false, 100);
            bl[63] = true;
            bl[64] = true;
            Assert.IsTrue(bl.Remove(true));
            Assert.IsTrue(bl[63]);
            Assert.IsFalse(bl[64]);
            Assert.AreEqual(99, bl.Count);
            Assert.IsTrue(bl.Remove(true));
            Assert.AreEqual(98, bl.Count);

        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            var bl = new BitCollection();
            var control = new List<bool>();
            for (int i = 0; i < 1000; ++i)
            {
                bool val = (i % 7 == 0) ? true : false;
                bl.Add(val);
                control.Add(val);
            }

            bl.RemoveAt(234);
            control.RemoveAt(234);

            for (int i = 0; i < bl.Count; ++i)
            {
                if (bl[i] != control[i])
                    Assert.Fail("value at index {1} is incorrect", i);
            }
        }
    }
}