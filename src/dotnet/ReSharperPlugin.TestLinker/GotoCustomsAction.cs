using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Features.Navigation.Features.ExposingApies;
using JetBrains.ReSharper.Host.Features.CodeInsights;

namespace TestLinker
{
    [Action("Goto_CustomTypes", "custom types description", Id = 170)]
    public class GotoCustomsAction : ContextNavigationActionBase<CustomProvider>, IExecutableAction
    {
        public const string GOTO_INHERITORS_ACTION_ID = "GotoCustoms";

        public override IActionRequirement GetRequirement(IDataContext dataContext)
        {
            return EmptyRequirement.Instance;
        }
    }
}