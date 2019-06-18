using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Features.CodeInsights.Providers;
using JetBrains.ReSharper.Host.Features.CodeInsights.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework.Resources;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace TestLinker
{
    [SolutionComponent]
    public class CustomTypesCodeInsightsProvider : ContextNavigationCodeInsightsProviderBase<GotoCustomsAction, CustomProvider>
    {
        public const string Id = "Custom Types";

        public override string ProviderId => "CustomTypes";

        protected override IconId IconId => UnitTestingThemedIcons.TestFixtureToolWindow.Id;

        public CustomTypesCodeInsightsProvider(ISolution solution)
            : base(solution)
        {
        }

        public override ICollection<CodeLensRelativeOrdering> RelativeOrderings =>
            (ICollection<CodeLensRelativeOrdering>) new CodeLensRelativeOrderingAfter[1]
            {
                new CodeLensRelativeOrderingAfter("Extension methods")
            };

        protected override string Noun(IDeclaredElement element, int count)
        {
            return "custom types" + this.Arity(count);
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