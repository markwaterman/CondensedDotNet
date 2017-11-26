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

    }
}
