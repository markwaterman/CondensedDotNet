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

namespace Samples
{
    class BrokenEquality
    {
        #region BasicCake
        enum Flavor { Chocolate, Vanilla, Carrot, DevilsFood, RedVelvet }

        class Cake
        {
            public Flavor Flavor { get; }
            public byte CandleCount { get; }

            public Cake(Flavor flavor, byte candleCount)
            {
                Flavor = flavor;
                CandleCount = candleCount;
            }
        }

        class Program
        {
            static void Main()
            {
                var cc = new Condensed.DedupedList<Cake>();
                cc.Add(new Cake(Flavor.Chocolate, 42));
                cc.Add(new Cake(Flavor.Chocolate, 42));
                Console.WriteLine(cc.InternPoolCount);
                // Output: 2
            }
        }
        #endregion
    }

    class FixedEquality
    {
        enum Flavor { Chocolate, Vanilla, Carrot, DevilsFood, RedVelvet }

        #region FixedCake
        class Cake : IEquatable<Cake>
        {
            public Flavor Flavor { get; }
            public byte CandleCount { get; }

            public Cake(Flavor flavor, byte candleCount)
            {
                Flavor = flavor;
                CandleCount = candleCount;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Flavor.GetHashCode();
                    hash = hash * 23 + CandleCount.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Cake);
            }

            public bool Equals(Cake other)
            {
                return (Flavor == other.Flavor &&
                        CandleCount == other.CandleCount);
            }
        }
        #endregion
    }
}
