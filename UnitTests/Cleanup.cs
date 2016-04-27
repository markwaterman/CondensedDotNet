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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Condensed;
using System.Linq;

namespace UnitTests
{
    /// <summary>
    /// Summary description for Cleanup
    /// </summary>
    [TestClass]
    public class Cleanup
    {

        [TestMethod]
        public void Cleanup0()
        {
            var cc = new CondensedCollection<int>();
            cc.Cleanup();
            Assert.AreEqual(0, cc.Count);
            Assert.AreEqual(0, cc.UniqueCount);
            Assert.AreEqual(0, cc.InternPoolCount);

        }

        [TestMethod]
        public void Cleanup1()
        {
            var cc = new CondensedCollection<int>();
            cc.Insert(0, 99);
            cc.Cleanup();
            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(1, cc.InternPoolCount);

            cc.RemoveAt(0);
            Assert.AreEqual(0, cc.Count);
            Assert.AreEqual(0, cc.UniqueCount);
            Assert.AreEqual(1, cc.InternPoolCount);
            cc.Cleanup();
            Assert.AreEqual(0, cc.Count);
            Assert.AreEqual(0, cc.UniqueCount);
            Assert.AreEqual(0, cc.InternPoolCount);
        }

        [TestMethod]
        public void Cleanup2()
        {
            var cc = new CondensedCollection<string>();
            cc.Add("Hello");
            cc.Add("world");

            cc.RemoveAt(0);

            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);

            cc.Cleanup();

            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(1, cc.InternPoolCount);
        }

        [TestMethod]
        public void Cleanup3()
        {
            var cc = new CondensedCollection<string>();
            cc.Add("Hello");
            cc.Add("world");

            // Make an interned value's refcount go to zero and back up
            // again. Make sure Cleanup doesn't break it..
            cc.RemoveAt(0);
            cc.Add("Hello");

            Assert.AreEqual(2, cc.Count);
            Assert.AreEqual(2, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);

            cc.Cleanup();

            Assert.AreEqual(2, cc.Count);
            Assert.AreEqual(2, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);
            Assert.AreEqual("world", cc[0]);
            Assert.AreEqual("Hello", cc[1]);
        }

        [TestMethod]
        public void CleanupMany()
        {
            var cc = new CondensedCollection<int>();
            for (int i = 0; i < 100; ++i)
                cc.Add(i % 10);

            Assert.AreEqual(100, cc.Count);
            Assert.AreEqual(10, cc.UniqueCount);
            Assert.AreEqual(10, cc.InternPoolCount);

            // remove all elements divisible by three
            for (int i = 99; i >= 0; --i)
            {
                if (cc[i] % 3 == 0)
                    cc.RemoveAt(i);
            }

            Assert.AreEqual(60, cc.Count);
            Assert.AreEqual(6, cc.UniqueCount);
            Assert.AreEqual(10, cc.InternPoolCount);

            cc.Cleanup();

            Assert.AreEqual(60, cc.Count);
            Assert.AreEqual(6, cc.UniqueCount);
            Assert.AreEqual(6, cc.InternPoolCount); // down from 10
            Assert.AreEqual(1, cc[0]);
            Assert.AreEqual(8, cc[59]);
        }

        [TestMethod]
        public void CleanupManyNullable()
        {
            var cc = new CondensedCollection<int?>();
            for (int i = 0; i < 100; ++i)
                cc.Add(i % 10);

            Assert.AreEqual(100, cc.Count);
            Assert.AreEqual(10, cc.UniqueCount);
            Assert.AreEqual(10, cc.InternPoolCount);

            // set all elements divisible by 3 to null.
            for (int i = 0; i < 100; ++i)
            {
                if (cc[i] % 3 == 0)
                    cc[i] = null;
            }

            Assert.AreEqual(100, cc.Count); 
            Assert.AreEqual(7, cc.UniqueCount); // null is counted as a unique value
            Assert.AreEqual(11, cc.InternPoolCount);

            cc.Cleanup();

            Assert.AreEqual(100, cc.Count);
            Assert.AreEqual(7, cc.UniqueCount);
            Assert.AreEqual(7, cc.InternPoolCount); // down from 10

            Assert.IsNull(cc[0]);
            Assert.AreEqual(1, cc[1]);
            Assert.AreEqual(8, cc[98]);
            Assert.IsNull(cc[99]);
        }

        [TestMethod]
        public void CleanupNarrowToZero()
        {
            var cc = new CondensedCollection<int>();
            cc.Add(0);

            Assert.AreEqual(IndexType.ZeroBytes, cc.IndexType);

            cc.Add(1);
            Assert.AreEqual(IndexType.OneBit, cc.IndexType);

            cc.Remove(1);
            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);
            Assert.AreEqual(IndexType.OneBit, cc.IndexType);

            cc.Cleanup();
            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(1, cc.InternPoolCount);
            Assert.AreEqual(IndexType.ZeroBytes, cc.IndexType);
        }


        [TestMethod]
        public void CleanupNarrowToBit()
        {
            var cc = new CondensedCollection<int>();
            cc.Add(0);
            cc.Add(1);

            Assert.AreEqual(IndexType.OneBit, cc.IndexType);

            cc.Add(2);
            Assert.AreEqual(IndexType.OneByte, cc.IndexType);

            cc.Remove(2);
            Assert.AreEqual(2, cc.Count);
            Assert.AreEqual(2, cc.UniqueCount);
            Assert.AreEqual(3, cc.InternPoolCount);
            Assert.AreEqual(IndexType.OneByte, cc.IndexType);

            cc.Cleanup();
            Assert.AreEqual(2, cc.Count);
            Assert.AreEqual(2, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);
            Assert.AreEqual(IndexType.OneBit, cc.IndexType);
        }


        [TestMethod]
        public void CleanupNarrowToByte()
        {
            var cc = new CondensedCollection<int>();
            for (int i = 0; i < 256; ++i)
                cc.Add(i);

            Assert.AreEqual(IndexType.OneByte, cc.IndexType);

            cc.Add(256);
            Assert.AreEqual(IndexType.TwoBytes, cc.IndexType);

            cc.Remove(256);
            Assert.AreEqual(256, cc.Count);
            Assert.AreEqual(256, cc.UniqueCount);
            Assert.AreEqual(257, cc.InternPoolCount);
            Assert.AreEqual(IndexType.TwoBytes, cc.IndexType);

            cc.Cleanup();
            Assert.AreEqual(256, cc.Count);
            Assert.AreEqual(256, cc.UniqueCount);
            Assert.AreEqual(256, cc.InternPoolCount);
            Assert.AreEqual(IndexType.OneByte, cc.IndexType);
        }

        [TestMethod]
        public void CleanupNarrowToTwoBytes()
        {
            var cc = new CondensedCollection<int>();
            for (int i = 0; i < 65536; ++i)
                cc.Add(i);

            Assert.AreEqual(IndexType.TwoBytes, cc.IndexType);

            cc.Add(65536);
            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);

            cc.Remove(65536);
            Assert.AreEqual(65536, cc.Count);
            Assert.AreEqual(65536, cc.UniqueCount);
            Assert.AreEqual(65537, cc.InternPoolCount);
            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);

            cc.Cleanup();
            Assert.AreEqual(65536, cc.Count);
            Assert.AreEqual(65536, cc.UniqueCount);
            Assert.AreEqual(65536, cc.InternPoolCount);
            Assert.AreEqual(IndexType.TwoBytes, cc.IndexType);
            
        }

        [TestMethod]
        public void CleanupRepopulate()
        {
            var cc = new CondensedCollection<int>();
            for (int i = 0; i < 100000; ++i)
                cc.Add(i);

            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);

            for (int i = 99999; i >= 0; --i)
                cc.RemoveAt(i);

            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);
            Assert.AreEqual(0, cc.Count);
            Assert.AreEqual(0, cc.UniqueCount);
            Assert.AreEqual(100000, cc.InternPoolCount);
            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);

            cc.Cleanup();
            Assert.AreEqual(IndexType.ZeroBytes, cc.IndexType);

            for (int i = 0; i < 100000; ++i)
                cc.Add(i);

            Assert.AreEqual(100000, cc.Count);
            Assert.AreEqual(100000, cc.UniqueCount);
            Assert.AreEqual(100000, cc.InternPoolCount);
            Assert.AreEqual(IndexType.FourBytes, cc.IndexType);

        }

        bool eventFired = false;
        [TestMethod]
        public void ZeroRefcountFires()
        {
            var cc = new CondensedCollection<int>();
            cc.InternedValueReclaimable += ZeroRefcountFiresEventHandler;
            cc.Add(42);

            eventFired = false;
            cc.Remove(42);
            Assert.IsTrue(eventFired);

            Assert.AreEqual(0, cc.Count);
            Assert.AreEqual(0, cc.UniqueCount);
            Assert.AreEqual(0, cc.InternPoolCount);
        }

        private void ZeroRefcountFiresEventHandler(object sender, InternReclaimableEventArgs e)
        {
            eventFired = true;
            Assert.AreEqual(0, e.Count);
            Assert.AreEqual(0, e.UniqueCount);
            Assert.AreEqual(1, e.InternPoolCount);

            e.Cleanup = true;
        }

        [TestMethod]
        public void ZeroRefcountNoCleanup()
        {
            var cc = new CondensedCollection<int>();
            cc.InternedValueReclaimable += ZeroRefcountNoCleanupEventHandler;
            cc.Add(42);
            cc.Add(24);

            eventFired = false;
            cc.Remove(42);
            Assert.IsTrue(eventFired);

            Assert.AreEqual(1, cc.Count);
            Assert.AreEqual(1, cc.UniqueCount);
            Assert.AreEqual(2, cc.InternPoolCount);
        }

        private void ZeroRefcountNoCleanupEventHandler(object sender, InternReclaimableEventArgs e)
        {
            eventFired = true;
            Assert.AreEqual(1, e.Count);
            Assert.AreEqual(1, e.UniqueCount);
            Assert.AreEqual(2, e.InternPoolCount);

            e.Cleanup = false;
        }
    }
}
