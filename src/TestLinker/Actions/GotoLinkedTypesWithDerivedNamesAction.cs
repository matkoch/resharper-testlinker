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
using JetBrains.ReSharper.Psi;
using JetBrains.UI.ActionsRevised;
using TestLinker.Utils;

namespace TestLinker.Actions
{
  [Action ("Goto_LinkedTypesWithDerivedNames", "Goto Linked Types with Derived Names", Id = 9855)]
  public class GotoLinkedTypesWithDerivedNamesAction : GotoLinkedTypesActionBase
  {
    protected override ISet<ITypeElement> GetLinkedTypes (LinkedTypesService linkedTypesService, List<ITypeElement> typesInContext)
    {
      return typesInContext.SelectMany(
          x => linkedTypesService.GetLinkedTypes(x)
              .Where(y => DerivedNameUtility.IsDerivedNameAny(x.ShortName, y.ShortName)))
          .ToSet();
    }
  }
}