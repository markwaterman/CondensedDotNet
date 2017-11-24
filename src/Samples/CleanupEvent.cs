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
    class CleanupEvent
    {
        #region CleanupEvent
        static void Main()
        {
            var cc = new CondensedCollection<string>(comparer: StringComparer.Ordinal);
            cc.InternedValueReclaimable += HandleInternedValueReclaimable;   
        }

        static void HandleInternedValueReclaimable(object sender, InternReclaimableEventArgs e)
        {
            // Perform cleanup as soon as we have 1000 unused strings in our intern pool.
            // After cleanup completes, those unused values will be eligible for garbage collection.
            if (e.ReclaimableInternsCount > 999)
                e.Cleanup = true;
        }
        #endregion
    }
}
