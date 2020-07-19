using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Navigation;

namespace ReSharperPlugin.TestLinker.Actions
{
    [Action(
        ActionId: Id,
        Text: "Goto Linked Types With Derived Name",
        Id = 17023,
        IdeaShortcuts = new[] {"Shift+Control+O"},
        VsShortcuts = new[] {"Shift+Control+O"})]
    public class GotoLinkedTypesWithDerivedNameAction
        : ContextNavigationActionBase<LinkedTypesWithDerivedNameNavigationProvider>
    {
        public const string Id = nameof(GotoLinkedTypesWithDerivedNameAction);

        public override IActionRequirement GetRequirement(IDataContext dataContext)
        {
            return CurrentPsiFileRequirementNoCaches.FromDataContext(dataContext);
        }
    }
}
