#if RIDER
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.CodeInsights.Providers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;
using JetBrains.Util;
using ReSharperPlugin.TestLinker.Actions;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [SolutionComponent]
    public class LinkedTypesCodeInsightsProvider : ContextNavigationCodeInsightsProviderBase<GotoLinkedTypesAction, LinkedTypesNavigationProvider>
    {
        public override string ProviderId => "LinkedTypes";

        protected override IconId IconId => UnitTestingThemedIcons.TestFixtureToolWindow.Id;

        public LinkedTypesCodeInsightsProvider(ISolution solution)
            : base(solution)
        {
        }

        public override ICollection<CodeLensRelativeOrdering> RelativeOrderings =>
            new[] {new CodeLensRelativeOrderingFirst()};

        protected override string Noun(IDeclaredElement element, int count)
        {
            return $"linked {NounUtil.ToPluralOrSingular("type", count)}";
        }
    }
}
#endif