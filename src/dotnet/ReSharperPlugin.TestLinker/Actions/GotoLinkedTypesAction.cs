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
        Text: "Goto Linked Types",
        Id = 17022,
        IdeaShortcuts = new[] {"Shift+Control+I"},
        VsShortcuts = new[] {"Shift+Control+I"})]
    public class GotoLinkedTypesAction
        : ContextNavigationActionBase<LinkedTypesNavigationProvider>
    {
        public const string Id = nameof(GotoLinkedTypesAction);
        
        public override IActionRequirement GetRequirement(IDataContext dataContext)
        {
            return CurrentPsiFileRequirementNoCaches.FromDataContext(dataContext);
        }
    }
}