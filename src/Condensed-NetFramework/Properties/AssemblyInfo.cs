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
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Condensed Collection Library for .NET")]
[assembly: AssemblyDescription("An IList implementation that uses interning (deduplication) to store large numbers of immutable items.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("www.markwaterman.net")]
[assembly: AssemblyProduct("Condensed")]
[assembly: AssemblyCopyright("Copyright © 2016 Mark Waterman")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100e39866bb986655e308e2c6927ff6032f6e071fb959e282e1bff01017ba325b9f137c77b132bfa5a13e7deddc1945ae74781bf9950140404b23e6a59760113e2524ac59f003b2356b8ec8a4519a83a01a45858f270aa670584167b149cb5bdf095cafb9dfb228ecae7b123a4e0259e4cf68b1af2db60dc29742311e83b47dbed0")]