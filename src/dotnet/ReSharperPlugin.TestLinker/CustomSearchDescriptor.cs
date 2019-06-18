using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Diagrams;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;

namespace TestLinker
{
    public sealed class CustomSearchDescriptor : SearchDescriptor
    {
        public CustomSearchDescriptor(CustomSearchRequest request, ICollection<IOccurrence> results)
            : base(request, results)
        {
        }

        public override string ActionBarID
        {
            get { return "TreeModelBrowser.Standard"; }
        }

        public override string GetResultsTitle(OccurrenceSection section)
        {
            const string form = "exposing APIs";

            string title;
            var foundCount = section.TotalCount;
            if (foundCount == 0) title = "No exposing APIs found";
            else
            {
                title = (foundCount == section.FilteredCount)
                    ? string.Format("Found {0} {1}", foundCount, form)
                    : string.Format("Displaying {0} of {1} found {2}", section.FilteredCount, foundCount, form);
            }

            return title;
        }

        public override TypeDependenciesOptions DiagrammingOptions
        {
            get
            {
                return new TypeDependenciesOptions(new[] { TypeElementDependencyType.ReturnType }, TypeDependenciesOptions.CollapseBigFoldersFunc);
            }
        }

        protected override Func<SearchRequest, IOccurrenceBrowserDescriptor> GetDescriptorFactory()
        {
            Func<SearchRequest, IOccurrenceBrowserDescriptor> descriptorFactory = request =>
            {
                var declarationRequest = (CustomSearchRequest) request;
                var results = request.Search();
                var descriptor = new CustomSearchDescriptor(declarationRequest, results);
                return descriptor;
            };
            return descriptorFactory;
        }
    }
}