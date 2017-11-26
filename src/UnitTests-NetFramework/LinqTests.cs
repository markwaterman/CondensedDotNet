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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Condensed;
using Condensed.Linq;

namespace UnitTests
{
    [TestClass]
    public class LinqTests
    {
        [TestMethod]
        public void First()
        {
            DedupedList<int> l = new DedupedList<int>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i);

            var index = l.First((i) => i == 422);
            Assert.AreEqual(422, index);
        }

        [TestMethod]
        public void FirstNullable()
        {
            var l = new DedupedList<int?>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i);

            l[0] = null;
            var item = l.First((i) => i == null);
            Assert.IsNull(item);
            item = l.First((i) => i != null);
            Assert.AreEqual(1, item);
            item = l.First((i) => (i != null || i == null));
            Assert.IsNull(item);

            l = new DedupedList<int?>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i % 2);

            l[10] = null;
            item = l.First((i) => i == null);
            Assert.IsNull(item);
            item = l.First((i) => i != null);
            Assert.AreEqual(0, item);
            item = l.First((i) => (i != null || i == null));
            Assert.AreEqual(0, item);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FirstOnEmpty()
        {
            var l = new DedupedList<int>();
            var item = l.First((i) => i == 0);
            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FirstNotfound()
        {
            var l = new DedupedList<int?>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i % 2);

            var item = l.First((i) => i == null);

        }

        [TestMethod]
        public void FirstOrDefault()
        {
            DedupedList<int> l = new DedupedList<int>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i);

            var item = l.FirstOrDefault((i) => i == 422);
            Assert.AreEqual(422, item);

            item = l.FirstOrDefault((i) => i == 99999);
            Assert.AreEqual(0, item);
        }

        [TestMethod]
        public void Last()
        {
            DedupedList<int> l = new DedupedList<int>();
            for (int i = 0; i < 100; ++i)
                l.Add(i);

            var lastItem = l.Last((i) => i == 99);
            Assert.AreEqual(99, lastItem);

            var firstItem = l.Last((i) => i == 0);
            Assert.AreEqual(0, firstItem);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LastNotFound()
        {
            var l = new DedupedList<int?>();
            for (int i = 0; i < 1000; ++i)
                l.Add(i % 2);

            var item = l.Last((i) => i == 3);
        }

        [TestMethod]
        public void LastOrDefault()
        {
            var l = new DedupedList<string>();
            for (int i = 0; i < 100; ++i)
                l.Add(i.ToString());

            var item = l.LastOrDefault((i) => i == "42");
            Assert.AreEqual("42", item);

            item = l.LastOrDefault((i) => i == "99999");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Average()
        {
            var l = new DedupedList<int>();
            var control = new List<int>();

            // add sequence of 0,1,2,3 repeating 25 times
            for (int i = 0; i < 100; ++i)
            {
                l.Add(i % 4);
                control.Add(i % 4);
            }

            var avg = l.Average();
            var cAvg = control.Average();

            Assert.AreEqual(1.5D, avg, 0.00001);
            Assert.AreEqual(cAvg, avg, 0.00001);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AverageEmpty()
        {
            var l = new DedupedList<int>();
            var avg = l.Average();
        }

        [TestMethod]
        public void AverageEmptyNullable()
        {
            var l = new DedupedList<int?>();
            var avg = l.Average();
            Assert.IsNull(avg);


            for (int i = 0; i < 100; ++i)
                l.Add(null);

            avg = l.Average();
            Assert.IsNull(avg);

        }

        [TestMethod]
        public void AverageNullable()
        {
            var l = new DedupedList<int?>();
            var control = new List<int?>();

            // add some integers with null mixed in.
            for (int i = 0; i < 100; ++i)
            {
                int? val = i % 7;
                if (val == 3) val = null;

                l.Add(val);
                control.Add(val);
            }

            var avg = l.Average();
            var cAvg = control.Average();

            Assert.AreEqual(2.941860465116279, avg);
            Assert.AreEqual(cAvg, avg);
        }

        [TestMethod]
        public void AverageOfTransform()
        {
            var l = new DedupedList<TimeSpan>();
            var control = new List<TimeSpan>();

            for (int i = 0; i < 100; ++i)
            {
                l.Add(TimeSpan.FromMinutes(i % 42));
                control.Add(TimeSpan.FromMinutes(i % 42));
            }

            var avg = l.Average((ts) => (int)ts.TotalMinutes);
            var cAvg = control.Average((ts) => ts.TotalMinutes);

            Assert.AreEqual(18.42D, avg, 0.00001);
            Assert.AreEqual(cAvg, avg, 0.00001);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AverageTransformEmpty()
        {
            var l = new DedupedList<DateTime>();
            var avg = l.Average((dt) => dt.DayOfYear);
        }


        int? NullableIntSelector(TimeSpan ts)
        {
            if (ts.TotalMinutes == 13)
                return null;
            else
                return (int)ts.TotalMinutes;
        }

        int IntSelectorForNullable(TimeSpan? ts)
        {
            if (ts == null)
                return 0;
            else
                return (int)ts.Value.TotalMinutes;
        }

        [TestMethod]
        public void AverageOfNullableTransform()
        {
            var l = new DedupedList<TimeSpan>();
            var control = new List<TimeSpan>();

            for (int i = 0; i < 100; ++i)
            {
                l.Add(TimeSpan.FromMinutes(i % 42));
                control.Add(TimeSpan.FromMinutes(i % 42));
            }

            var avg = l.Average((ts) => NullableIntSelector(ts));
            var cAvg = control.Average((ts) => NullableIntSelector(ts));

            Assert.AreEqual(18.587628865979383, avg);
            Assert.AreEqual(cAvg, avg);
        }

        [TestMethod]
        public void AverageOfTransformNullable()
        {
            var l = new DedupedList<TimeSpan?>();
            var control = new List<TimeSpan?>();

            for (int i = 0; i < 100; ++i)
            {
                TimeSpan? value = null;
                if (i % 13 != 0) value = TimeSpan.FromMinutes(i % 42);
                l.Add(value);
                control.Add(value);
            }

            var avg = l.Average((ts) => IntSelectorForNullable(ts));
            var cAvg = control.Average((ts) => IntSelectorForNullable(ts));

            Assert.AreEqual(16.88, avg, 0.00001);
            Assert.AreEqual(cAvg, avg, 0.00001);
        }

        [TestMethod]
        public void AverageTransformToAllNulls()
        {
            var l = new DedupedList<TimeSpan>();
            var avg = l.Average((ts) => null);
            Assert.IsNull(avg);


            for (int i = 0; i < 100; ++i)
                l.Add(TimeSpan.MaxValue);

            avg = l.Average((ts) => null);
            Assert.IsNull(avg);
        }

        [TestMethod]
        public void SumNullable()
        {
            var l = new DedupedList<int?>();
            var control = new List<int?>();

            // add some integers with null mixed in.
            for (int i = 0; i < 100; ++i)
            {
                int? val = i % 7;
                if (val == 3) val = null;

                l.Add(val);
                control.Add(val);
            }

            var sum = l.Sum();
            var cSum = control.Sum();

            Assert.AreEqual(253, sum);
            Assert.AreEqual(cSum, sum);
        }


        [TestMethod]
        public void DistinctNullable()
        {
            var l = new DedupedList<int?>();
            var control = new List<int?>();

            // add some integers with null mixed in.
            for (int i = 0; i < 100; ++i)
            {
                int? val = i % 7;
                if (val == 3) val = null;

                l.Add(val);
                control.Add(val);
            }

            var distinct = l.Distinct();
            Assert.AreEqual(7, distinct.Count());

            int count = 0;
            int nullCount = 0;
            foreach (int? uniqueItem in distinct)
            {
                count++;
                if (uniqueItem == null) nullCount++;
            }

            Assert.AreEqual(7, count);
            Assert.AreEqual(1, nullCount);
        }

        [TestMethod]
        public void Max()
        {
            var l = new DedupedList<int>();
            for (int i = 0; i < 100; ++i)
                l.Add(i % 7);

            Assert.AreEqual(6, l.Max());
        }

        [TestMethod]
        public void MaxNullable()
        {
            var l = new DedupedList<int?>();
            for (int i = 0; i < 100; ++i)
            {
                int? val = i % 7;
                if (val == 6) val = null;

                l.Add(val);
            }

            Assert.AreEqual(5, l.Max());
        }

        [TestMethod]
        public void MaxTransform()
        {
            var l = new DedupedList<TimeSpan>();

            for (int i = 0; i < 100; ++i)
            {
                l.Add(TimeSpan.FromMinutes(i % 42));
            }

            Assert.AreEqual(41, l.Max((ts) => (int)ts.TotalMinutes));
        }

        [TestMethod]
        public void MaxNullableTransform()
        {
            var l = new DedupedList<TimeSpan>();

            for (int i = 0; i < 100; ++i)
            {
                l.Add(TimeSpan.FromMinutes(i % 42));
            }

            Assert.AreEqual(41, l.Max((ts) => NullableIntSelector(ts)));
        }

        [TestMethod]
        public void MaxTransformNullable()
        {
            var l = new DedupedList<TimeSpan?>();

            for (int i = 0; i < 100; ++i)
            {
                TimeSpan? value = null;
                if (i % 13 != 0) value = TimeSpan.FromMinutes(i % 42);
                l.Add(value);
            }

            var max = l.Max((ts) => IntSelectorForNullable(ts));

            Assert.AreEqual(41, max);
        }

        [TestMethod]
        public void MaxNullStrings()
        {
            var l = new DedupedList<string>();

            l.Add(null);

            Assert.IsNull(l.Max());

        }

        [TestMethod]
        public void MaxTransformedToString()
        {
            var l = new DedupedList<DateTime>();
            var max = l.Max((dt) => dt.ToString());
            Assert.IsNull(max);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaxTransformedDateTime()
        {
            var l = new DedupedList<string>();
            var max = l.Max((dt) => DateTime.Now);
        }

        [TestMethod]
        public void MaxTransformedDateTimeToNull()
        {
            var l = new DedupedList<string>();
            l.Add(null);

            var max = l.Max((dt) =>
            {
                if (dt == null)
                    return DateTime.MinValue;
                else
                    return DateTime.Now;
            });

            Assert.AreEqual(max, DateTime.MinValue);
        }


    }
}
