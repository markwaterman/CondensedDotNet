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
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Condensed;
using Condensed.Indexes;

namespace UnitTests
{
    [TestClass]
    public class Basics
    {
        [TestMethod]
        public void AddAndReadSingle()
        {
            CondensedCollection<int> dlist = new CondensedCollection<int>();
            dlist.Add(42);
            Assert.AreEqual(42, dlist[0]);
        }

        [TestMethod]
        public void RemoveSingle()
        {
            CondensedCollection<string> dlist = new CondensedCollection<string>();
            dlist.Add("test");
            Assert.AreEqual("test", dlist[0]);
            Assert.AreEqual<int>(1, dlist.Count);
            Assert.AreEqual<int>(1, dlist.UniqueCount);

            dlist.Remove("test");
            Assert.AreEqual<int>(0, dlist.Count);
            Assert.AreEqual<int>(0, dlist.UniqueCount);
        }

        [TestMethod]
        public void RemoveAtSingle()
        {
            CondensedCollection<string> dlist = new CondensedCollection<string>();
            dlist.Add("test");
            Assert.AreEqual("test", dlist[0]);
            Assert.AreEqual<int>(1, dlist.Count);
            Assert.AreEqual<int>(1, dlist.UniqueCount);

            dlist.RemoveAt(0);
            Assert.AreEqual<int>(0, dlist.Count);
            Assert.AreEqual<int>(0, dlist.UniqueCount);
        }

        [TestMethod]
        public void AddAndReadDuplicates()
        {
            CondensedCollection<int> dlist = new CondensedCollection<int>();
            dlist.Add(43);
            dlist.Add(43);
            Assert.AreEqual(43, dlist[0]);
            Assert.AreEqual(43, dlist[1]);
        }

        [TestMethod]
        public void Add1000()
        {
            CondensedCollection<int> dlist = new CondensedCollection<int>();
            for (int i = 0; i < 1000; ++i)
            {
                dlist.Add(i);
            }

            for (int i = 0; i < 1000; ++i)
            {
                Assert.AreEqual<int>(i, dlist[i]);
            }
        }

        [TestMethod]
        public void IndexOf()
        {
            CondensedCollection<int> dlist = new CondensedCollection<int>();
            for (int i = 0; i < 1000; ++i)
            {
                dlist.Add(i);
            }

            Assert.AreEqual<int>(444, dlist.IndexOf(444));
            Assert.AreEqual<int>(-1, dlist.IndexOf(9999));
        }

        [TestMethod]
        public void MutableStruct()
        {
            var point = new System.Drawing.Point(42, 42);
            var l = new CondensedCollection<System.Drawing.Point>();
            l.Add(point);
            l.Add(point);
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(1, l.UniqueCount);

            var p2 = l[0];
            p2.X = 43;
            l[0] = p2;
            Assert.AreEqual(43, l[0].X);
            Assert.AreEqual(42, l[1].X);

        }




    }
}
