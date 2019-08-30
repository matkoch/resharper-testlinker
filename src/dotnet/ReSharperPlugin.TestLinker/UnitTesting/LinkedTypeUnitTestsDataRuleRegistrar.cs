// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Common;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Utils;

namespace ReSharperPlugin.TestLinker.UnitTesting
{
    [SolutionComponent]
    public class LinkedTypeUnitTestsDataRuleRegistrar
    {
        private readonly IUnitTestElementStuff _unitTestElementStuff;

        public LinkedTypeUnitTestsDataRuleRegistrar (
            Lifetime lifetime,
            DataContexts dataContexts,
            IUnitTestElementStuff unitTestElementStuff)
        {
            _unitTestElementStuff = unitTestElementStuff;
            
            var dataRule = new DataRule<UnitTestElements>.DesperateDataRule(
                "ProjectModelToUnitTestElements",
                UnitTestDataConstants.UnitTestElements.SELECTED,
                LinkedTypeUnitTestsDataRule);

            dataContexts.RegisterDataRule(lifetime, dataRule);
        }

        private UnitTestElements LinkedTypeUnitTestsDataRule (IDataContext context)
        {
            var linkedTypes = LinkedTypesUtil.GetLinkedTypes(context);
            var relevantTests = linkedTypes.Select(x => _unitTestElementStuff.GetElement(x)).WhereNotNull();
            return new UnitTestElements(new TestAncestorCriterion(relevantTests));
        }
    }
}
