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
    public class SparseArrayTests
    {
        [TestMethod]
        public void Empty()
        {
            var sa = new SparseArray<int>(0);
            Assert.AreEqual(0, sa.Length);
        }

        [TestMethod]
        public void Basics()
        {
            var sa = new SparseArray<int>(200);
            Assert.AreEqual(200, sa.Length);

            sa[100] = 100;
            Assert.AreEqual(100, sa[100]);

            sa.Clear(100);
            Assert.AreEqual(0, sa[100]);
        }

        [TestMethod]
        public void Enumerate1()
        {
            var sa = new SparseArray<int>(100);
            int count = 0;
            foreach (var item in sa)
            {
                count++;
            }
            Assert.AreEqual(100, count);
        }

        [TestMethod]
        public void Enumerate2()
        {
            var sa = new SparseArray<int>(1000);
            sa[500] = 500;
            Assert.AreEqual(500, sa[500]);

            int count = 0;
            foreach (var val in sa.NonEmptyValues)
                count++;

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void ResizeToSmaller1()
        {
            var sa = new SparseArray<int>(2);
            sa[0] = 1;
            sa[1] = 2;

            SparseArray<int>.Resize(ref sa, 1);
            Assert.AreEqual(1, sa.Length);
            Assert.AreEqual(1, sa[0]);
        }

        [TestMethod]
        public void ResizeToSmaller2()
        {
            var sa = new SparseArray<int>(200);
            for (int i = 0; i < sa.Length; i++)
                if (i % 2 == 0) sa[i] = i;

            Assert.AreEqual(198, sa[198]);
            Assert.AreEqual(0, sa[199]);

            SparseArray<int>.Resize(ref sa, 100);
            Assert.AreEqual(100, sa.Length);
            Assert.AreEqual(98, sa[98]);
            Assert.AreEqual(0, sa[99]);
        }

        [TestMethod]
        public void ResizeToLarger()
        {
            var sa = new SparseArray<int>(100);
            for (int i = 0; i < sa.Length; i++)
                if (i % 2 == 0) sa[i] = i;

            SparseArray<int>.Resize(ref sa, 200);
            Assert.AreEqual(200, sa.Length);
            Assert.AreEqual(98, sa[98]);
            Assert.AreEqual(0, sa[99]);
            Assert.AreEqual(0, sa[100]);
            Assert.AreEqual(0, sa[199]);
        }

        [TestMethod]
        public void ResizeFromNull()
        {
            SparseArray<int> sa = null;
            SparseArray<int>.Resize(ref sa, 100);
            Assert.AreEqual(100, sa.Length);
        }

        [TestMethod]
        public void ResizeFromZero()
        {
            SparseArray<int> sa = new SparseArray<int>(0);
            SparseArray<int>.Resize(ref sa, 100);
            Assert.AreEqual(100, sa.Length);
        }

        [TestMethod]
        public void ResizeToZero()
        {
            SparseArray<int> sa = new SparseArray<int>(100);
            SparseArray<int>.Resize(ref sa, 0);
            Assert.AreEqual(0, sa.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void NoInsert()
        {
            IList<int> sa = new SparseArray<int>(100);
            sa.Insert(10, 10);
        }

        [TestMethod]
        public void IndexOf()
        {
            var sa = new SparseArray<int>(200);
            for (int i = 0; i < sa.Length; i++)
                sa[i] = i + 1;

            sa.Clear(100);

            var list = sa as IList<int>;
            Assert.AreEqual(150, list.IndexOf(151));
            Assert.AreEqual(100, list.IndexOf(0));

            int last = sa.Last();
        }
    }
}
