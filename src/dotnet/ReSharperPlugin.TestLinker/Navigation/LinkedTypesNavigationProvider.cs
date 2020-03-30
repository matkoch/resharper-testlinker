using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Actions;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [ContextNavigationProvider]
    public class LinkedTypesNavigationProvider
        : LinkedTypesNavigationProviderBase<LinkedTypesContextSearch>
    {
        public LinkedTypesNavigationProvider(
            IShellLocks locks,
            ITooltipManager tooltipManager,
            IFeaturePartsContainer manager)
            : base(manager)
        {
        }

        protected override string ActionId
        {
            get { return GotoLinkedTypesAction.Id; }
        }

        protected override string NavigationMenuTitle
        {
            get { return "Linked Types"; }
        }
    }
}