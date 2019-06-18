using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Diagrams;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker.Navigation
{
    // TODO: what is it good for?
    public sealed class LinkedTypesSearchDescriptor : SearchDescriptor
    {
        private const string NoLinkedTypesFoundText = "No linked types found";

        public LinkedTypesSearchDescriptor(LinkedTypesSearchRequest request, ICollection<IOccurrence> results)
            : base(request, results)
        {
        }

        public override string ActionBarID => "TreeModelBrowser.Standard";

        public override string GetResultsTitle(OccurrenceSection section)
        {
            if (section.TotalCount == 0)
                return NoLinkedTypesFoundText;

            var typeOrTypes = NounUtil.ToPluralOrSingular("type", section.TotalCount);
            return section.FilteredCount == section.TotalCount
                ? $"Found {section.TotalCount} linked {typeOrTypes}"
                : $"Displaying {section.FilteredCount} of {section.TotalCount} linked {typeOrTypes}";
        }

        public override TypeDependenciesOptions DiagrammingOptions
            => new TypeDependenciesOptions(new[] { TypeElementDependencyType.ReturnType }, TypeDependenciesOptions.CollapseBigFoldersFunc);

        protected override Func<SearchRequest, IOccurrenceBrowserDescriptor> GetDescriptorFactory()
        {
            Func<SearchRequest, IOccurrenceBrowserDescriptor> descriptorFactory = request =>
            {
                var declarationRequest = (LinkedTypesSearchRequest) request;
                var results = request.Search();
                var descriptor = new LinkedTypesSearchDescriptor(declarationRequest, results);
                return descriptor;
            };
            return descriptorFactory;
        }
    }
}