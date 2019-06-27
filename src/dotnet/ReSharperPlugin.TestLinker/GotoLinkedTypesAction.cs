using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Features.Navigation.Features.ExposingApies;
using JetBrains.ReSharper.Host.Features.CodeInsights;

namespace TestLinker
{
    [Action(ID, "Linked Types", Id = 170)]
    public class GotoLinkedTypesAction : ContextNavigationActionBase<CustomProvider>
    {
        public const string ID = "NavigateToLinkedTypes";
    }
}