using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.CodeInsights.Providers;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;
using ReSharperPlugin.TestLinker.Actions;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [SolutionComponent]
    public class CustomTypesCodeInsightsProvider : ContextNavigationCodeInsightsProviderBase<GotoLinkedTypesAction, LinkedTypesNavigationProvider>
    {
        public const string Id = "Custom Types";

        public override string ProviderId => "CustomTypes";

        protected override IconId IconId => UnitTestingThemedIcons.TestFixtureToolWindow.Id;

        public CustomTypesCodeInsightsProvider(ISolution solution)
            : base(solution)
        {
        }

        public override ICollection<CodeLensRelativeOrdering> RelativeOrderings =>
            new[]
            {
                new CodeLensRelativeOrderingAfter("Extension methods")
            };

        protected override string Noun(IDeclaredElement element, int count)
        {
            return "custom types2" + this.Arity(count);
        }

        protected override string FormatLong(IDeclaredElement elt, int ownCount, int inheritorsCount)
        {
            if (this.IsSpecial(ownCount))
                return base.FormatLong(elt, ownCount, inheritorsCount);
            string str = string.Format("{0} own ", (object) ownCount) + this.Noun(elt, ownCount);
            if (inheritorsCount > 0)
                str += string.Format(" + {0} {1} of inheritor{2}", (object) inheritorsCount, (object) this.Noun(elt, inheritorsCount), (object) this.Arity(inheritorsCount));
            return str;
        }
    }
}