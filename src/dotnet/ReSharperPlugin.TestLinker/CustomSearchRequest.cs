using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
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
            var type = _typeElement.Type();
            Console.WriteLine(type);
        }

        public override string Title => $"Linked Types for {_typeElement.Type()?.GetPresentableName(_typeElement.PresentationLanguage)}";

        public override ISolution Solution => _typeElement.GetSolution();

        public override ICollection SearchTargets => new IDeclaredElementEnvoy[] {new DeclaredElementEnvoy<IDeclaredElement>(_typeElement)};

        public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator)
        {
            if (!_typeElement.IsValid())
                return EmptyList<IOccurrence>.InstanceList;

            var psiServices = Solution.GetPsiServices();

            var typeofs = GetAttributeTypeOfs(_typeElement);
            if (typeofs != null)
            {
                return typeofs
                    .Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence))
                    .ToArray();
            }

            var symbolCache = psiServices.Symbols.GetSymbolScope(LibrarySymbolScope.NONE, caseSensitive: true);
            var derivedNames = GetDerivedNames(new[] {"Test"}, _typeElement.ShortName);
            var linkedElements = derivedNames.SelectMany(x => symbolCache.GetElementsByShortName(x)).ToList();

            if (derivedNames.Length != 1)
            {
                var wordIndex = psiServices.WordIndex;
                var sourceFiles = wordIndex.GetFilesContainingAllWords(new[] {_typeElement.ShortName});
                var typesInFiles = sourceFiles
                    .SelectMany(x => psiServices.Symbols.GetTypesAndNamespacesInFile(x))
                    .OfType<ClassLikeTypeElement>()
                    .Where(x => GetAttributeTypeOfs(x)?.Contains(_typeElement) ?? false);

                linkedElements.AddRange(typesInFiles);
            }

            return linkedElements
                .Where(x => !x.Equals(_typeElement))
                .Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence))
                .ToArray();
        }

        [CanBeNull]
        private ClassLikeTypeElement[] GetAttributeTypeOfs(IAttributesSet attributesSet)
        {
            var attribute = attributesSet
                .GetAttributeInstances(inherit: true)
                .SingleOrDefault(x => x.GetAttributeShortName().StartsWith("Subject"));
            if (attribute == null)
                return null;

            return attribute
                .PositionParameters()
                .Where(x => x.IsArray)
                .SelectMany(x => x.ArrayValue.NotNull().Select(y =>y.TypeValue.GetTypeElement<ClassLikeTypeElement>()))
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