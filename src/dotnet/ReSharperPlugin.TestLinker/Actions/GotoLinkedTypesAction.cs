using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Navigation;

namespace ReSharperPlugin.TestLinker.Actions
{
    [Action(Id, "Goto Linked Types", Id = 170)]
    public class GotoLinkedTypesAction
        : ContextNavigationActionBase<LinkedTypesNavigationProvider>
    {
        public const string Id = nameof(GotoLinkedTypesAction);
    }
}