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
    public class BitwiseTests
    {
        [TestMethod]
        public void CountSetBits()
        {
            UInt64 bitset = 3UL;
            var count = SparseBitwise.PositionToOffset(bitset, 50);
            Assert.AreEqual(2, count);

            bitset = 1125899906842624;
            count = SparseBitwise.PositionToOffset(bitset, 10);
            Assert.AreEqual(0, count);

            count = SparseBitwise.PositionToOffset(bitset, 63);
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CountEmpty()
        {
            UInt64 bitset = 0UL;
            var count = SparseBitwise.PositionToOffset(bitset, 0);
            Assert.AreEqual(0, count);
        }
    }
}

