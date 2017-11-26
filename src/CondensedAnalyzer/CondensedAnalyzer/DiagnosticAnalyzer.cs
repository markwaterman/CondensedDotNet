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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Condensed.Diagnostics
{
    /// <summary>
    /// C# analyzer that checks for DedupedList usage problems.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CondensedAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CLDN001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "http://www.markwaterman.net/docs/condenseddotnet/html/12336e43-b0f4-46fb-a17b-c640871bddab.htm"
            );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ObjectCreationExpression);
        }

        /// <summary>
        /// Quick and dirty syntax analysis method that complains if it sees a blatantly mutable type being 
        /// used in a DedupedList.
        /// </summary>
        /// <remarks>
        /// <para>
        /// At the moment I'm just checking to see if the type has any public, non-read-only properties or public fields.
        /// If there's a standard, more bullet-proof way to use Roslyn to check for mutability then I can't find it...
        /// will enhance further as comfort level with Roslyn increases.
        /// </para>
        /// <para>
        /// Note that I'm not the only one who wants a way to enforce immutability. Monitor the following ongoing proposals
        /// in the Roslyn project:
        /// <list type="bullet">
        /// <item>https://github.com/dotnet/roslyn/issues/159</item>
        /// <item>https://github.com/dotnet/roslyn/issues/7626</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="context">Syntax to be analyzed.</param>
        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            var creationExpression = context.Node as ObjectCreationExpressionSyntax;
            var createdType = context.SemanticModel.GetTypeInfo(creationExpression).Type as INamedTypeSymbol;
            if (createdType == null)
                return;
            
            if (createdType.ContainingNamespace.Name == "Condensed" &&
                 createdType.MetadataName == "DedupedList`1")
            {
                // Find what type is being stored in the DedupedList<T>:
                var elementType = createdType.TypeArguments[0];

                // Value types (primitives, structs) are safe to use in a DedupedList since
                // copies are always returned. Safe to give them a pass:
                if (elementType.IsValueType)
                    return;

                bool mutable = false;

                // look at properties & fields of the class, see if there's anything that could be modified.
                foreach (var member in elementType.GetMembers())
                {
                    // TODO: there's a lot more we could do here, just going for low-hanging fruit right now.
                    // For starters, I'm just going to check public instance properties & fields..

                    // skipping static members (also encompasses members declared as const)
                    if (member.IsStatic)
                        continue;

                    // skip non-public members... just going to trust the user for now--we're after public, mutable members.
                    if (member.DeclaredAccessibility != Accessibility.Public)
                        continue;
                    

                    if (member.Kind == SymbolKind.Property)
                    {
                        var prop = member as IPropertySymbol;
                        if (prop.SetMethod != null && prop.SetMethod.DeclaredAccessibility == Accessibility.Public)
                        {
                            // we hit a property with a public setter--complain about mutability.
                            mutable = true;
                            break;
                        }
                    }
                    else if (member.Kind == SymbolKind.Field)
                    {
                        var field = member as IFieldSymbol;
                        if (!field.IsReadOnly)
                        {
                            // we hit a public field--complain about mutability.
                            mutable = true;
                            break;
                        }
                    }
                } // end foreach (member)


                if (mutable)
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), elementType.Name);
                    context.ReportDiagnostic(diagnostic);
                }

            }


        }// method

    }// class

}// namespace

