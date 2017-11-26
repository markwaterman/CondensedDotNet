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

namespace Condensed
{
    /// <summary>
    /// Specifies the width of the internal index currently used by a <see cref="DedupedList{T}"/>.
    /// </summary>
    public enum IndexType
    {
        /// <summary>
        /// The index is zero bytes wide (the DedupedList only contains
        /// zero or one unique values).
        /// </summary>
        ZeroBytes,
        /// <summary>
        /// The index is a single bit wide (the DedupedList contains up to
        /// two unique values).
        /// </summary>
        OneBit,
        /// <summary>
        /// The index is a single byte wide (the DedupedList contains up to
        /// 256 unique values).
        /// </summary>
        OneByte,
        /// <summary>
        /// The index is two bytes wide (the DedupedList contains up to
        /// 65536 unique values).
        /// </summary>
        TwoBytes,
        /// <summary>
        /// The index is four bytes wide (the DedupedList contains up to
        /// 2,147,483,648 unique values).
        /// </summary>
        FourBytes
    }
}
