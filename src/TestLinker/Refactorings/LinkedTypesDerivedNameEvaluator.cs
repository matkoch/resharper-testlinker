// Copyright 2016, 2015, 2014 Matthias Koch
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
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

    public IEnumerable<IDeclaredElement> CreateFromElement (IEnumerable<IDeclaredElement> initialElement, DerivedDeclaredElement derivedElement)
    {
      return GetRelatedTypesWithDerivedName(derivedElement.DeclaredElement);
    }

    public IEnumerable<IDeclaredElement> CreateFromReference (IReference reference, IDeclaredElement declaredElement)
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