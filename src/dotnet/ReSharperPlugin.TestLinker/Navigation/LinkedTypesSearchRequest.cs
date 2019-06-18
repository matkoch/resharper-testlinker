using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public sealed class LinkedTypesSearchRequest : SearchRequest
    {
        private readonly ITypeElement _typeElement;
        private readonly ITextControl _textControl;
        private readonly bool _derivedNamesOnly;

        public LinkedTypesSearchRequest(ITypeElement typeElement, ITextControl textControl, bool derivedNamesOnly)
        {
            _typeElement = typeElement;
            _textControl = textControl;
            _derivedNamesOnly = derivedNamesOnly;
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

            var linkedTypes = LinkedTypesUtil.GetLinkedTypes(_typeElement);
            if (linkedTypes.Count == 0)
                ModificationUtility.TryCreateTestOrProductionClass(_typeElement, _textControl);

            bool IsDerivedName(ITypeElement typeElement) =>
                _typeElement.ShortName.Contains(typeElement.ShortName) ||
                typeElement.ShortName.Contains(_typeElement.ShortName);

            return linkedTypes
                .Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence, IsDerivedName(x)))
                .Where(x => !_derivedNamesOnly || x.HasNameDerived)
                .ToArray();
        }
    }
}