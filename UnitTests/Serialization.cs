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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Condensed;

namespace UnitTests
{
    [TestClass()]
    public class Serialization
    {
        /* // not supporting built-in binary serialization now since it isn't available in PCLs...
        [TestMethod]
        public void SerializeSingleUnique()
        {
            var l = new CondensedCollection<DateTime>();
            var now = DateTime.Now;
            for (int i = 0; i < 100; ++i)
                l.Add(now);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, l);

            stream.Seek(0, 0);
            var l2 = bf.Deserialize(stream) as CondensedCollection<DateTime>;

            Assert.IsNotNull(l2);
            Assert.AreEqual(100, l2.Count);
            Assert.AreEqual(1, l2.UniqueCount);
            Assert.AreEqual(IndexType.ZeroBytes, l2.IndexType);
            Assert.AreEqual(now, l2[42]);
        }

        [TestMethod]
        public void SerializeTwoUnique()
        {
            var l = new CondensedCollection<string>();
            var now = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
            {
                if (i % 2 == 0)
                    l.Add("foo");
                else
                    l.Add(null);
            }

            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, l);

            stream.Seek(0, 0);
            var l2 = bf.Deserialize(stream) as CondensedCollection<string>;

            Assert.IsNotNull(l2);
            Assert.AreEqual(1000, l2.Count);
            Assert.AreEqual(2, l2.UniqueCount);
            Assert.AreEqual(IndexType.OneBit, l2.IndexType);
            Assert.AreEqual("foo", l2[400]);
            Assert.AreEqual(null, l2[401]);
        }


        [TestMethod]
        public void SerializeByteIndex()
        {
            var l = new CondensedCollection<Nullable<long>>();
            var now = DateTime.Now;
            for (int i = 0; i < 1234; ++i)
            {
                l.Add(i % 42);
            }
            l[999] = null;


            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, l);

            stream.Seek(0, 0);
            var l2 = bf.Deserialize(stream) as CondensedCollection<long?>;

            Assert.IsNotNull(l2);
            Assert.AreEqual(1234, l2.Count); 
            Assert.AreEqual(43, l2.UniqueCount); // null counts as a unique value
            Assert.AreEqual(IndexType.OneByte, l2.IndexType);
            Assert.AreEqual(0, l2[84]);
            Assert.AreEqual(1, l2[85]);
            Assert.IsNull(l2[999]);
        }

        [TestMethod]
        public void SerializeShortIndex()
        {
            var l = new CondensedCollection<TimeSpan>();
            var now = DateTime.Now;
            for (int i = 0; i < 23456; ++i)
            {
                l.Add(TimeSpan.FromSeconds(i % 10000));
            }


            MemoryStream stream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, l);

            stream.Seek(0, 0);
            var l2 = bf.Deserialize(stream) as CondensedCollection<TimeSpan>;

            Assert.IsNotNull(l2);
            Assert.AreEqual(23456, l2.Count);
            Assert.AreEqual(10000, l2.UniqueCount);
            Assert.AreEqual(IndexType.TwoBytes, l2.IndexType);
            Assert.AreEqual(TimeSpan.FromSeconds(0), l2[10000]);
            Assert.AreEqual(TimeSpan.FromSeconds(1), l2[10001]);
        }
        */
    }
}
