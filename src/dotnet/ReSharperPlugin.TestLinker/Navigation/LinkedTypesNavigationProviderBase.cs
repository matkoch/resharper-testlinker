using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.DataFlow;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Diagrams;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker.Navigation
{
    public abstract class LinkedTypesNavigationProviderBase<T> : ContextNavigationProviderBase<T>, INavigateFromHereProvider
        where T : LinkedTypesContextSearchBase
    {
        private readonly IShellLocks myLocks;
        private readonly ITooltipManager myTooltipManager;

        // TODO: LABEL
        private const string NoResultsFoundText = "No linked types found";

        protected LinkedTypesNavigationProviderBase(
            IShellLocks locks,
            ITooltipManager tooltipManager,
            IFeaturePartsContainer manager)
            : base(manager)
        {
            myLocks = locks;
            myTooltipManager = tooltipManager;
        }

        protected override NavigationActionGroup ActionGroup => NavigationActionGroup.Important;

        protected override void Execute(IDataContext dataContext, IEnumerable<T> searches,
            INavigationExecutionHost host)
        {
            var request = searches.SelectNotNull(item => item.CreateSearchRequest(dataContext)).SingleOrDefault();
            var occurrences = request?.Search();
            if (occurrences == null)
                return;

            if (occurrences.IsEmpty())
            {
                ShowToolTip(dataContext, NoResultsFoundText);
                return;
            }

            if (host.ProcessImmediateResultHighlightUsages(dataContext, occurrences))
                return;

            host.ShowContextPopupMenu(
                dataContext,
                occurrences,
                () => new LinkedTypesSearchDescriptor(request, occurrences),
                OccurrencePresentationOptions.DefaultOptions,
                skipMenuIfSingleEnabled: true,
                request.Title,
                () =>
                {
                    var typeElementsForDiagram =
                        OccurrenceUtil.GetTypeElementsForDiagram(request, occurrences).ToList();
                    return new Pair<ICollection<ITypeElement>, TypeDependenciesOptions>(typeElementsForDiagram,
                        new TypeDependenciesOptions(new[] {TypeElementDependencyType.ReturnType},
                            TypeDependenciesOptions.CollapseBigFoldersFunc));
                });
        }

        private void ShowToolTip(IDataContext dataContext, string tooltip)
        {
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);
            if (textControl != null)
                myTooltipManager.ShowAtCaret(Lifetime.Eternal, tooltip, textControl, myLocks);
            else
                myTooltipManager.ShowIfPopupWindowContext(tooltip, dataContext);
        }
    }
}
