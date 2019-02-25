// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.Application.UI.DataContext;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Feature.Services.Navigation.ExecutionHosting;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using TestLinker.Navigation;
using TestLinker.Utils;

namespace TestLinker.Actions
{
    public abstract class GotoLinkedTypesActionBase : IActionWithExecuteRequirement, IExecutableAction, IInsertLast<NavigateContextualInMainMenuGroup>
    {
        //private GroupingEvent _executionGroupingEvent;
        private ITypesFromTextControlService _typesFromTextControlService;

        private ISolution _solution;
        private ITextControl _textControl;
        private LinkedTypesService _linkedTypesService;
        private PopupWindowContextSource _popupWindowContextSource;

        protected abstract ISet<ITypeElement> GetLinkedTypes (LinkedTypesService linkedTypesService, List<ITypeElement> typesInContext);

        public IActionRequirement GetRequirement (IDataContext dataContext)
        {
            //_executionGroupingEvent = _executionGroupingEvent ?? CreateExecuteGroupingEvent(dataContext);
            return CurrentPsiFileRequirement.FromDataContext(dataContext);
        }

        public bool Update (IDataContext context, ActionPresentation presentation, [NotNull] DelegateUpdate nextUpdate)
        {
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            return solution != null;
        }

        public void Execute (IDataContext context, [NotNull] DelegateExecute nextExecute)
        {
            _typesFromTextControlService = context.GetComponent<ITypesFromTextControlService>().NotNull();
            _solution = context.GetData(ProjectModelDataConstants.SOLUTION).NotNull();
            _textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
            _linkedTypesService = _solution.GetComponent<LinkedTypesService>();
            _popupWindowContextSource = context.GetData(UIDataConstants.PopupWindowContextSource);

            //_executionGroupingEvent.FireIncoming();
            ExecuteProlonged();
        }

        private void ExecuteProlonged ()
        {
            var typesInContext = _typesFromTextControlService.GetTypesFromCaretOrFile(_textControl, _solution).ToList();
            var linkedTypes = GetLinkedTypes(_linkedTypesService, typesInContext);
            ExecuteProlonged(_solution, linkedTypes, typesInContext, _textControl, _popupWindowContextSource);
        }

        public static void ExecuteProlonged(
            ISolution solution,
            ISet<ITypeElement> linkedTypes,
            List<ITypeElement> sourceTypes,
            ITextControl textControl,
            PopupWindowContextSource popupWindowContextSource)
        {
            solution.GetPsiServices().Files.CommitAllDocuments();

            using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
            {
                var occurrences = linkedTypes.Select(x => new LinkedTypesOccurrence(x, OccurrenceType.Occurrence))
                    .ToList<IOccurrence>();
                    
                if (occurrences.Count == 0 && textControl != null)
                    TryCreateProductionOrTestClass(sourceTypes, textControl);
                else if (occurrences.Count == 1)
                    NavigateToSingleOccurrence(solution, occurrences.Single(), popupWindowContextSource);
                else
                    ShowOccurrencePopupMenu(solution, sourceTypes, occurrences, popupWindowContextSource);
            }
        }

        private static void TryCreateProductionOrTestClass (
            IReadOnlyCollection<ITypeElement> typesInContext,
            ITextControl textControl)
        {
            if (typesInContext.Count != 1)
            {
                MessageBox.ShowInfo("There is no single class in context to create production/test class from.");
                return;
            }

            ModificationUtility.TryCreateTestOrProductionClass(typesInContext.Single(), textControl);
        }

        private static void NavigateToSingleOccurrence (
            ISolution solution,
            IOccurrence occurrence,
            PopupWindowContextSource popupWindowContextSource)
        {
            occurrence.Navigate(solution, popupWindowContextSource, transferFocus: true);
        }

        private static void ShowOccurrencePopupMenu (
            ISolution solution,
            ICollection<ITypeElement> sourceTypes,
            ICollection<IOccurrence> occurrences,
            PopupWindowContextSource popupWindowContextSource)
        {
            Func<OccurrenceBrowserDescriptor> descriptorBuilder =
                    () => new LinkedTypesOccurrenceBrowserDescriptor(solution, sourceTypes, occurrences);
            var navigationExecutionHost = solution.GetComponent<DefaultNavigationExecutionHost>();
            navigationExecutionHost.ShowGlobalPopupMenu(
                solution,
                occurrences,
                activate: true,
                windowContext: popupWindowContextSource,
                descriptorBuilder: descriptorBuilder,
                options: new OccurrencePresentationOptions(),
                skipMenuIfSingleEnabled: true,
                title: "Go to linked types ");
        }

        //private GroupingEvent CreateExecuteGroupingEvent (IDataContext dataContext)
        //{
        //  var shellLocks = dataContext.GetComponent<IShellLocks>();
        //  return shellLocks.GroupingEvents.CreateEvent(
        //      Lifetimes.Define().Lifetime,
        //      "GotoLinkedTypes",
        //      TimeSpan.FromMilliseconds(value: 1000),
        //      Rgc.Guarded,
        //      ExecuteProlongated);
        //}
    } 
}
