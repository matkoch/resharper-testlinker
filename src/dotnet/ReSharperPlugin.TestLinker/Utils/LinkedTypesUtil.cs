using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Options;

namespace ReSharperPlugin.TestLinker.Utils
{
    public class LinkedTypesUtil
    {
        public static IReadOnlyCollection<ITypeElement> GetLinkedTypes(IDataContext dataContext)
        {
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
            var solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION).NotNull();
            var typesInContextProvider = dataContext.GetComponent<ITypesFromTextControlService>().NotNull();

            var typesInContext = typesInContextProvider.GetTypesFromCaretOrFile(textControl, solution);
            return typesInContext.SelectMany(GetLinkedTypes).ToList();
        }

        public static IReadOnlyCollection<ITypeElement> GetLinkedTypes(ITypeElement source)
        {
            var sources = new[] {source}
                .Concat(source.GetAllSuperTypes()
                    .Select(x => x.GetTypeElement())
                    .WhereNotNull()
                    .Where(x => !x.IsObjectClass()));

            var linkedTypes = sources.SelectMany(GetLinkedTypesInternal).ToList();

            // TODO move to Internal method
            var services = source.GetPsiServices();
            linkedTypes.ForEach(x => services.Finder.FindInheritors(x, new FindResultConsumer(y =>
            {
                if ((y as FindResultDeclaredElement)?.DeclaredElement is ITypeElement typeElement)
                    linkedTypes.Add(typeElement);

                return FindExecution.Continue;
            }), NullProgressIndicator.Create()));

            return linkedTypes;
        }

        private static IReadOnlyCollection<ITypeElement> GetLinkedTypesInternal(ITypeElement source)
        {
            var settings = GetSettings(source.GetSolution());
            var derivedNames = GetDerivedNames(source, settings.NamingSuffixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            var attributeName = settings.TypeofAttributeName.TrimFromEnd("Attribute");

            var typeofs = GetAttributeLinkedTypes(source, attributeName);
            if (typeofs != null)
                return typeofs.ToList();

            var psiServices = source.GetPsiServices();

            var symbolCache = psiServices.Symbols.GetSymbolScope(LibrarySymbolScope.NONE, caseSensitive: true);
            var linkedTypes = derivedNames.SelectMany(x => symbolCache.GetElementsByShortName(x)).ToList();

            var wordIndex = psiServices.WordIndex;
            var sourceFiles = wordIndex.GetFilesContainingAllWords(new[] {source.ShortName});
            var typesInFiles = sourceFiles
                .SelectMany(x => psiServices.Symbols.GetTypesAndNamespacesInFile(x))
                .OfType<ClassLikeTypeElement>()
                .Where(x => GetAttributeLinkedTypes(x, attributeName)?.Contains(source) ?? false);
            linkedTypes.AddRange(typesInFiles);

            return linkedTypes.Where(x => !x.Equals(source)).Cast<ITypeElement>().ToList();
        }

        [CanBeNull]
        private static IEnumerable<ITypeElement> GetAttributeLinkedTypes(IAttributesSet attributesSet, string attributeName)
        {
            var attribute = attributesSet
                .GetAttributeInstances(inherit: true)
                .SingleOrDefault(x => x.GetAttributeShortName()?.StartsWith(attributeName) ?? false);
            if (attribute == null)
                return null;

            var namedArguments = attribute.NamedParameters().Select(x => x.Second);
            var positionalArguments = attribute.PositionParameters();
            var flattenedArguments = FlattenArguments(namedArguments.Concat(positionalArguments));

            return flattenedArguments
                .Where(x => x.IsType && !x.IsBadValue)
                .Select(x => x.TypeValue.GetTypeElement())
                .WhereNotNull();
        }

        private static string[] GetDerivedNames(ITypeElement source, string[] suffixes)
        {
            var shortName = source.ShortName;
            return suffixes.Any(x => shortName.StartsWith(x) || shortName.EndsWith(x))
                ? new[] { suffixes.Aggregate(shortName, (name, suffix) => name.TrimFromStart(suffix).TrimFromEnd(suffix)) }
                : suffixes.SelectMany(x => new[] {shortName + x, x + shortName}).ToArray();
        }

        private static TestLinkerSettings GetSettings(ISolution solution)
        {
            var settingsStore = solution.GetComponent<ISettingsStore>();
            var settingsOptimization = solution.GetComponent<ISettingsOptimization>();
            var contextBoundSettingsStore = settingsStore.BindToContextTransient(ContextRange.Smart(solution.ToDataContext()));
            return contextBoundSettingsStore.GetKey<TestLinkerSettings>(settingsOptimization);
        }

        private static IEnumerable<AttributeValue> FlattenArguments (IEnumerable<AttributeValue> attributeValues)
        {
            foreach (var attributeValue in attributeValues)
            {
                if (!attributeValue.IsArray)
                {
                    yield return attributeValue;
                }
                else
                {
                    foreach (var innerAttributeValue in FlattenArguments(attributeValue.ArrayValue.NotNull()))
                        yield return innerAttributeValue;
                }
            }
        }
    }
}