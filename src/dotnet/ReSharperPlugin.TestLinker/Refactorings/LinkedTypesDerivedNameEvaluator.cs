// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Refactorings.Rename;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.Refactorings
{
    [DerivedRenamesEvaluator]
    public class LinkedTypesDerivedNameEvaluator : IDerivedRenamesEvaluator
    {
        public bool SuggestedElementsHaveDerivedName => true;

        public IEnumerable<IDeclaredElement> CreateFromElement (
            [NotNull] IEnumerable<IDeclaredElement> initialElement,
            [NotNull] DerivedElement derivedElement)
        {
            return GetRelatedTypesWithDerivedName(derivedElement.DeclaredElement);
        }

        public IEnumerable<IDeclaredElement> CreateFromReference ([NotNull] IReference reference, [NotNull] IDeclaredElement declaredElement)
        {
            return GetRelatedTypesWithDerivedName(declaredElement);
        }

        private IEnumerable<IDeclaredElement> GetRelatedTypesWithDerivedName (IDeclaredElement declaredElement)
        {
            var typeElement = declaredElement as ITypeElement;
            if (typeElement == null)
                return Enumerable.Empty<IDeclaredElement>();

            // TODO get linked types by name
            var linkedTypes = LinkedTypesUtil.GetLinkedTypes(typeElement);
            return linkedTypes.Where(x => typeElement.ShortName.Contains(x.ShortName) || x.ShortName.Contains(typeElement.ShortName));
        }
    }
}
