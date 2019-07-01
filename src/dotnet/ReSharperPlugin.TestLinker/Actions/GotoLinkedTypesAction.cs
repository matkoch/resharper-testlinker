using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker.Navigation;

namespace ReSharperPlugin.TestLinker.Actions
{
    [Action(ID, "Linked Types", Id = 170)]
    public class GotoLinkedTypesAction : ContextNavigationActionBase<LinkedTypesNavigationProvider>
    {
        public const string ID = "NavigateToLinkedTypes";
    }
}