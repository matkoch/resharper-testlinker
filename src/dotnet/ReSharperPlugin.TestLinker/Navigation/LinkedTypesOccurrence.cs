// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public class LinkedTypesOccurrence : DeclaredElementOccurrence
    {
        public LinkedTypesOccurrence (
            [NotNull] IDeclaredElement element,
            OccurrenceType occurrenceKind,
            bool hasNameDerived)
            : base(element, occurrenceKind)
        {
            HasNameDerived = hasNameDerived;
        }

        public bool HasNameDerived { get; }
    }
}
