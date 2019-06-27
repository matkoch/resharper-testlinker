using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using TestLinker.Navigation;

namespace TestLinker
{
    public sealed class CustomSearchRequest : SearchRequest
    {
        private readonly ITypeElement _typeElement;

        public CustomSearchRequest(ITypeElement typeElement)
        {
            _typeElement = typeElement;
        }

        public override string Title => $"Linked Types for {_typeElement.Type()?.GetPresentableName(_typeElement.PresentationLanguage)}";

        public override ISolution Solution => _typeElement.GetSolution();

        public override ICollection SearchTargets => new IDeclaredElementEnvoy[] {new DeclaredElementEnvoy<IDeclaredElement>(_typeElement)};

        public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator)
        {
            if (!_typeElement.IsValid())
                return EmptyList<IOccurrence>.InstanceList;

            var psiServices = Solution.GetPsiServices();

            var symbolCache = psiServices.Symbols.GetSymbolScope(LibrarySymbolScope.NONE, caseSensitive: true);
            var derivedNames = GetDerivedNames(new[] {"Test"}, _typeElement.ShortName);
            var derivedNameElements = derivedNames.SelectMany(x => symbolCache.GetElementsByShortName(x)).ToList();

//            var wordIndex = psiServices.WordIndex;
//            wordIndex.GetA

            return derivedNameElements
                .Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence))
                .ToArray();
        }

        private static string[] GetDerivedNames(string[] suffixes, string shortName)
        {
            if (suffixes.Any(x => shortName.StartsWith(x) || shortName.EndsWith(x)))
                return new[] { suffixes.Aggregate(shortName, (name, suffix) => name.TrimFromStart(suffix).TrimFromEnd(suffix)) };
            else
                return suffixes.SelectMany(x => new[] {shortName + x, x + shortName}).ToArray();
        }
    }
}