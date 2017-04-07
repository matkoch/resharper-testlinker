// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Refactorings.Rename;
using JetBrains.Util;

namespace TestLinker.Refactorings
{
    [DerivedRenamesEvaluator]
    public class LinkedTypesDerivedNameEvaluator : IDerivedRenamesEvaluator
    {
        public bool SuggestedElementsHaveDerivedName => true;

        public IEnumerable<IDeclaredElement> CreateFromElement (
            [NotNull] IEnumerable<IDeclaredElement> initialElement,
            [NotNull] DerivedDeclaredElement derivedElement)
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

            var solution = declaredElement.GetSolution().NotNull();
            var linkedTypesService = solution.GetComponent<LinkedTypesService>();
            var relatedTypes = linkedTypesService.GetLinkedTypes(typeElement);

            return relatedTypes.Where(x => typeElement.ShortName.Contains(x.ShortName) || x.ShortName.Contains(typeElement.ShortName));
        }
    }
}
