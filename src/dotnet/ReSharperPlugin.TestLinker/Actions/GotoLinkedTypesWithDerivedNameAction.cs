using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Navigation;

namespace ReSharperPlugin.TestLinker.Actions
{
    [Action(Id, "Goto Linked Types With Derived Name", Id = 171, IdeaShortcuts = new[] {"Shift+Control+O"}, VsShortcuts = new[] {"Shift+Control+O"})]
    public class GotoLinkedTypesWithDerivedNameAction
        : ContextNavigationActionBase<LinkedTypesWithDerivedNameNavigationProvider>
    {
        public const string Id = nameof(GotoLinkedTypesWithDerivedNameAction);
    }
}