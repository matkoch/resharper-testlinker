// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Psi;
using TestLinker.Utils;

namespace TestLinker.Actions
{
    [Action("Goto_LinkedTypesWithDerivedNames", "Go to Derived-Name Linked Types", Id = 9855)]
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
