﻿/* Copyright 2016 Mark Waterman
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Condensed;

namespace UnitTests
{
    [TestClass()]
    public class IndexTypeTests
    {
        [TestMethod()]
        public void BoolIndex()
        {
            var l = new DedupedList<bool>();
            l.Add(true);
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add(false);
            Assert.AreEqual(IndexType.OneBit, l.IndexType);

        }

        [TestMethod()]
        public void ByteIndex()
        {
            var l = new DedupedList<byte>();
            l.Add((byte)43);
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add((byte)44);
            Assert.AreEqual(IndexType.OneBit, l.IndexType);
            l.Add((byte)45);
            Assert.AreEqual(IndexType.OneByte, l.IndexType);

        }

        [TestMethod()]
        public void ShortIndex()
        {
            var l = new DedupedList<short>();
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add(-1);
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add(-2);
            Assert.AreEqual(IndexType.OneBit, l.IndexType);

            for (short i = 0; i < 1000; ++i)
                l.Add((short)(i % 254));

            // should now have 256 unique values in the list.
            Assert.AreEqual(IndexType.OneByte, l.IndexType);

            // add one more and we should switch to 2-byte storage:
            l.Add(999);
            Assert.AreEqual(IndexType.TwoBytes, l.IndexType);
            
        }

        [TestMethod()]
        public void IntIndex()
        {
            var l = new DedupedList<int>();
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add(-1);
            Assert.AreEqual(IndexType.ZeroBytes, l.IndexType);
            l.Add(-2);
            Assert.AreEqual(IndexType.OneBit, l.IndexType);

            for (int i = 0; i < 100000; ++i)
                l.Add((i % 65534));

            // should now have 65536 unique values in the list.
            Assert.AreEqual(IndexType.TwoBytes, l.IndexType);

            // add one more and we should have 4-byte storage:
            l.Add(9999999);
            Assert.AreEqual(IndexType.FourBytes, l.IndexType);

        }
    }
}
