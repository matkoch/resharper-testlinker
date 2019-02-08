// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Psi;

namespace TestLinker.Actions
{
    [Action("Goto_AllLinkedTypes", "Go to All Linked Types", Id = 9854)]
    public class GotoAllLinkedTypesAction : GotoLinkedTypesActionBase
    {
        protected override ISet<ITypeElement> GetLinkedTypes (LinkedTypesService linkedTypesService, List<ITypeElement> typesInContext)
        {
            return linkedTypesService.GetLinkedTypes(typesInContext);
        }
    }
}
