using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Condensed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greenlake.Collections.Tests
{
    [TestClass]
    public class SparseArrayGroupTests
    {
        [TestMethod]
        public void Empty()
        {
            var g = new SparseArrayGroup<int>();
            for (int i = 0; i<SparseArrayGroup<int>.GroupSize; ++i)
                Assert.AreEqual(0, g[i]);
        }

        [TestMethod]
        public void SetClear1()
        {
            var g = new SparseArrayGroup<string>();

            g.Set(20, "foo");
            Assert.AreEqual("foo", g[20]);
            for (int i = 0; i < SparseArrayGroup<int>.GroupSize; ++i)
            {
                if (i == 20) continue;
                Assert.AreEqual(null, g[i]);
            }

            g.Clear(20);
            for (int i = 0; i < SparseArrayGroup<int>.GroupSize; ++i)
                Assert.AreEqual(null, g[i]);
        }

        [TestMethod]
        public void SetClear2()
        {
            var g = new SparseArrayGroup<string>();

            g.Set(30, "foo");
            g.Set(10, "bar");
            Assert.AreEqual("foo", g[30]);
            Assert.AreEqual("bar", g[10]);
            g.Clear(10);

            Assert.IsNull(g[10]);
            Assert.AreEqual("foo", g[30]);
        }

        [TestMethod]
        public void SetClear3()
        {
            var g = new SparseArrayGroup<string>();

            g.Set(10, "ten");
            g.Set(20, "twenty");
            g.Set(30, "thirty");
            Assert.AreEqual("ten", g[10]);
            Assert.AreEqual("twenty", g[20]);
            Assert.AreEqual("thirty", g[30]);

            g.Clear(20);
            Assert.AreEqual("ten", g[10]);
            Assert.IsNull(g[20]);
            Assert.AreEqual("thirty", g[30]);
        }

        [TestMethod]
        public void SetClear4()
        {
            var g = new SparseArrayGroup<int>();
            int groupSize = SparseArrayGroup<int>.GroupSize;

            for (int i = 0; i < groupSize; i++)
                g.Set(i, i);

            for (int i = 0; i < groupSize; i++)
                Assert.AreEqual(i, g[i]);

            for (int i = 0; i < groupSize; i++)
                g.Clear(i);

            for (int i = 0; i < groupSize; i++)
                Assert.AreEqual(0, g[i]);

            Assert.AreEqual(0, g.NonEmptyCount);
        }

        [TestMethod]
        public void Enumerate()
        {
            var g = new SparseArrayGroup<int>();

            g.Set(30, 30);

            Assert.AreEqual(30, g[30]);

            int i = 0;
            foreach (var val in g)
            {
                Assert.AreEqual(g[i], val);
                i++;
            }
        }

        [TestMethod]
        public void EnumerateNonEmpty()
        {
            var g = new SparseArrayGroup<int>();

            g.Set(31, 31);
            Assert.AreEqual(31, g[31]);

            int count = 0;
            foreach (var val in g.NonEmptyValues)
                count++;

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void IndexOf1()
        {
            var g = new SparseArrayGroup<int>();

            for (int i = 0; i < 20; i++)
                if (i % 2 == 0) g.Set(i, i);

            Assert.AreEqual(10, g.IndexOf(10, EqualityComparer<int>.Default));
        }

        [TestMethod]
        public void IndexOf2()
        {
            var g = new SparseArrayGroup<int>();

            for (int i = 0; i < 20; i++)
                g.Set(i, i + 1);

            g.Clear(10);
            Assert.AreEqual(10, g.IndexOf(0, EqualityComparer<int>.Default));
        }

        [TestMethod]
        public void IndexOf3()
        {
            var g = new SparseArrayGroup<int>();

            g.Set(0, 0);
            Assert.AreEqual(0, g.IndexOf(0, EqualityComparer<int>.Default));

            g.Clear(0);
            g.Set(1, 0);
            Assert.AreEqual(0, g.IndexOf(0, EqualityComparer<int>.Default));
        }
    }
}
