using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Diagrams;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;

namespace TestLinker
{
    [ContextNavigationProvider]
    public sealed class CustomProvider : ContextNavigationProviderBase<ICustomContextSearch>, INavigateFromHereProvider
    {
        private readonly IShellLocks myLocks;
        private readonly ITooltipManager myTooltipManager;

        private const string NO_EXPOSING_APIES_FOUND = "No exposing APIs found";
        private const string NO_EXPOSING_APIES_FOR_STATIC = "Exposing APIs are not available for static types";

        public CustomProvider(IShellLocks locks, ITooltipManager tooltipManager, IFeaturePartsContainer manager)
            : base(manager)
        {
            myLocks = locks;
            myTooltipManager = tooltipManager;
        }

        protected override string GetNavigationMenuTitle(IDataContext dataContext)
        {
            return "Custom Types";
        }

        protected override string GetActionId(IDataContext dataContext)
        {
            return "NavigateToCustoms";
        }

        protected override NavigationActionGroup ActionGroup
        {
            get { return NavigationActionGroup.Important; }
        }

        protected override void Execute(IDataContext dataContext, IEnumerable<ICustomContextSearch> searches,
            INavigationExecutionHost host)
        {
            var request = searches.SelectNotNull(item => item.CreateSearchRequest(dataContext)).SingleOrDefault();
            if (request != null)
            {
                var occurrences = request.Search();
                if (occurrences == null) return;

                if (occurrences.IsEmpty())
                {
                    ShowToolTip(dataContext, NO_EXPOSING_APIES_FOUND);
                    return;
                }

                Func<CustomSearchDescriptor> descriptorBuilder = () => new CustomSearchDescriptor(request, occurrences);
                host.ShowContextPopupMenu(dataContext, occurrences, descriptorBuilder, OccurrencePresentationOptions.DefaultOptions, true, request.Title, () =>
                {
                    var typeElementsForDiagram = OccurrenceUtil.GetTypeElementsForDiagram(request, occurrences).ToList();
                    return new Pair<ICollection<ITypeElement>, TypeDependenciesOptions>(typeElementsForDiagram, new TypeDependenciesOptions(new[] { TypeElementDependencyType.ReturnType }, TypeDependenciesOptions.CollapseBigFoldersFunc));
                });
            }
        }

        private void ShowToolTip(IDataContext dataContext, string tooltip)
        {
            var textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);
            if (textControl != null)
            {
                myTooltipManager.ShowAtCaret(Lifetime.Eternal, tooltip, textControl, myLocks);
            }
            else
            {
                myTooltipManager.ShowIfPopupWindowContext(tooltip, dataContext);
            }
        }
    }
}