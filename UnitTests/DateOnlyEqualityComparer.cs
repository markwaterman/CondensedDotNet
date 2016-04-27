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

namespace UnitTests
{
    /// <summary>
    /// IEqualityComparer implementation that only looks at the 
    /// date component of a DateTime struct, ignoring time. 
    /// </summary>
    class DateOnlyEqualityComparer : IEqualityComparer<DateTime>
    {
        public bool Equals(DateTime x, DateTime y)
        {
            if (x.Date.Equals(y.Date))
                return true;
            else
                return false;
        }

        public int GetHashCode(DateTime obj)
        {
            return obj.Date.GetHashCode();
        }
    }
}
