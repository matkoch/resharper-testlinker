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
