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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Condensed.Diagnostics;

namespace Condensed.Diagnostics.Test
{
    [TestClass]
    public class AnalyzerUnitTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"
using Condensed;

class Program
{
    static void Main(string[] args)
    {
        var cl = new CondensedCollection<MyImmutableClass>();
    }
}

class MyImmutableClass
{
    public const int MY_CONST = 1234;
    public static int s_member = 5678;
    public readonly string ReadOnlyString = ""Hello World"";
    public string PrivateSetterString { get; private set; }
}";

            VerifyCSharpDiagnostic(test); // expect no diagnostic complaints.
        }


        //Diagnostic should be triggered
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
using Condensed;

class Program
{
    static void Main(string[] args)
    {
        var cl = new CondensedCollection<MyMutableClass>();
    }
}

class MyMutableClass
{
    public string Name { get; set; }
}";
            var expected = new DiagnosticResult
            {
                Id = "CLDN001",
                Message = String.Format("Type '{0}' is a mutable reference type and should not be used in a CondensedCollection<T>.", "MyMutableClass"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 18)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }



        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CondensedAnalyzer();
        }
    }
}