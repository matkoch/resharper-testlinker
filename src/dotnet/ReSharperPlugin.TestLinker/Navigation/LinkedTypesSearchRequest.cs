using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public sealed class LinkedTypesSearchRequest : SearchRequest
    {
        private readonly ITypeElement _typeElement;

        public LinkedTypesSearchRequest(ITypeElement typeElement)
        {
            _typeElement = typeElement;
        }

        // TODO: LABEL
        // TODO: GetPresentableName ??
        public override string Title => $"Linked Types for {_typeElement.ShortName}";

        public override ISolution Solution => _typeElement.GetSolution();

        public override ICollection SearchTargets => new IDeclaredElementEnvoy[] {new DeclaredElementEnvoy<IDeclaredElement>(_typeElement)};

        public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator)
        {
            if (!_typeElement.IsValid())
                return EmptyList<IOccurrence>.InstanceList;

            return LinkedTypesUtil.GetLinkedTypes(_typeElement)
                .Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence))
                .ToArray();
        }
    }
}