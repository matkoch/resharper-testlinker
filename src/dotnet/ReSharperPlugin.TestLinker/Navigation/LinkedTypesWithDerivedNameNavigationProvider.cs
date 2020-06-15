using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Actions;

namespace ReSharperPlugin.TestLinker.Navigation
{
    [ContextNavigationProvider]
    public class LinkedTypesWithDerivedNameNavigationProvider
        : LinkedTypesNavigationProviderBase<LinkedTypesWithDerivedNameContextSearch>
    {
        public LinkedTypesWithDerivedNameNavigationProvider(
            IShellLocks locks,
            ITooltipManager tooltipManager,
            IFeaturePartsContainer manager)
            : base(manager)
        {
        }

        protected override string ActionId => GotoLinkedTypesWithDerivedNameAction.Id;

        protected override string NavigationMenuTitle => "Linked Types With Derived Name";
    }
}
