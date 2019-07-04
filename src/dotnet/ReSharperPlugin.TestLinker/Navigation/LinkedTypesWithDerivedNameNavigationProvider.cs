using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;

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
            : base(locks, tooltipManager, manager)
        {
        }

        protected override string GetNavigationMenuTitle(IDataContext dataContext)
        {
            return "Linked Types With Derived Name";
        }
    }
}