// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Common;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using TestLinker.Utils;

namespace TestLinker.DataContext
{
    [SolutionComponent]
    public class LinkedTypeUnitTestsDataRuleRegistrar
    {
        public LinkedTypeUnitTestsDataRuleRegistrar (Lifetime lifetime, DataContexts dataContexts)
        {
            var dataRule = new DataRule<UnitTestElements>.DesperateDataRule(
                "ProjectModelToUnitTestElements",
                UnitTestDataConstants.UnitTestElements.SELECTED,
                LinkedTypeUnitTestsDataRule);

            dataContexts.RegisterDataRule(lifetime, dataRule);
        }

        #region Privates

        private UnitTestElements LinkedTypeUnitTestsDataRule (IDataContext context)
        {
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION).NotNull();

            var typesInContextProvider = context.GetComponent<ITypesFromTextControlService>().NotNull();
            var typesInContext = typesInContextProvider.GetTypesFromCaretOrFile(textControl, solution);

            var linkedTypesService = context.GetComponent<LinkedTypesService>().NotNull();
            var testElements = linkedTypesService.GetUnitTestElementsFrom(typesInContext);

            return new UnitTestElements(new TestAncestorCriterion(testElements));
        }

        #endregion
    }
}
