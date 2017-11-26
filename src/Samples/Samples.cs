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
using Condensed;

namespace Samples
{
    public class Samples
    {
        public void Welcome()
        {
            #region WelcomeSample
			// Add 100 million dates spanning December 2016:
            var cc = new Condensed.DedupedList<DateTime>(capacity: 100000000);

            for (int i = 0; i < 100000000; i++)
                cc.Add(new DateTime(2016, 12, i % 30 + 1));

            // Memory usage: 100 MB (compare to ~780 MB for an ordinary List<DateTime>)
            #endregion
        }

        public void SimpleCutover()
        {
            #region SimpleCutover
            var cutover = new Predicate<CondensedStats>(delegate(CondensedStats stats)
            {
                // Return true to make a DedupedList stop performing deduplication.
                if (stats.UniqueCount > ushort.MaxValue)
                    return true;
                else
                    return false;
            });

            // Provide the cutover predicate to the DedupedList:
            var cc = new DedupedList<int>(cutoverPredicate: cutover);
            #endregion
        }

        public void StringCutover()
        {
            #region StringCutover
            var cutover = new Predicate<CondensedStats>(delegate (CondensedStats stats)
            {
                // Don't consider stopping deduplication until we have
                // at least 1 million elements in the population to look at:
                if (stats.Count < 1000000)
                    return false;
                
                // Stop deduplication if we get less than 4 elements
                // in the collection for every unique value.
                if ((double)stats.Count / stats.UniqueCount < 4 )
                    return true;
                else
                    return false;
            });

            // Provide the cutover predicate to the DedupedList:
            var cc = new DedupedList<string>(cutoverPredicate: cutover, 
                                                     comparer: StringComparer.Ordinal);
            #endregion
        }

        public void StandardCutover()
        {
            #region StandardCutover
            var cc = new DedupedList<decimal>(cutoverPredicate: StandardCutoverPredicates.DecimalPredicate);
            #endregion
        }

        public void ReconstructFromCutover()
        {
            var myCondensedColl = new DedupedList<string>(cutoverPredicate: StandardCutoverPredicates.StringPredicate);
            #region ReconstructFromCutover
            if (myCondensedColl.HasCutover)
            {
                // Reconstruct collection from itself to restart deduplication.
                myCondensedColl = new DedupedList<string>(collection: myCondensedColl);
            }
            else
            {
                // Do a normal cleanup:
                myCondensedColl.Cleanup();
            }
            #endregion
        }



    }
}
