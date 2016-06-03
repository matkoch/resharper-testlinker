// Copyright 2016, 2015, 2014 Matthias Koch
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Feature.Services.Navigation.ExecutionHosting;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Threading;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.DataContext;
using JetBrains.UI.PopupWindowManager;
using JetBrains.Util;
using TestLinker.Navigation;
using TestLinker.Utils;

namespace TestLinker.Actions
{
  [Action ("Goto_LinkedTypes", "Goto Linked Types", Id = 9854)]
  public class GotoLinkedTypesAction : IActionWithExecuteRequirement, IExecutableAction
  {
    private GroupingEvent _executionGroupingEvent;

    private ITypesInContextProvider _typesInContextProvider;
    private ISolution _solution;
    private ITextControl _textControl;
    private LinkedTypesService _linkedTypesService;
    private PopupWindowContextSource _popupWindowContextSource;

    #region IActionWithExecuteRequirement

    public IActionRequirement GetRequirement (IDataContext dataContext)
    {
      _executionGroupingEvent = _executionGroupingEvent ?? CreateExecuteGroupingEvent(dataContext);
      return CurrentPsiFileRequirement.FromDataContext(dataContext);
    }

    #endregion

    #region IExecutableAction

    public bool Update (IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
      return solution != null;
    }

    public void Execute (IDataContext context, DelegateExecute nextExecute)
    {
      _typesInContextProvider = context.GetComponent<ITypesInContextProvider>().NotNull();
      _solution = context.GetData(ProjectModelDataConstants.SOLUTION).NotNull();
      _textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
      _linkedTypesService = _solution.GetComponent<LinkedTypesService>();
      _popupWindowContextSource = context.GetData(UIDataConstants.PopupWindowContextSource);

      //_executionGroupingEvent.FireIncoming();
      ExecuteProlongated();
    }

    #endregion

    #region Privates

    private void ExecuteProlongated ()
    {
      var typesInContext = _typesInContextProvider.GetTypesInContext(_textControl, _solution).ToList();
      var linkedTypes = _linkedTypesService.GetLinkedTypes(typesInContext);
      if (linkedTypes.IsEmpty())
        return;

      var occurrences = linkedTypes.Select(x => new LinkedTypesOccurrence(x, OccurenceType.Occurence)).ToList<IOccurence>();
      if (occurrences.Count == 1)
      {
        occurrences.Single().Navigate(_solution, _popupWindowContextSource, transferFocus: true);
      }
      else
      {
        Func<OccurenceBrowserDescriptor> descriptorBuilder = () => new LinkedTypesOccurrenceBrowserDescriptor(_solution, typesInContext, occurrences);
        var navigationExecutionHost = _solution.GetComponent<DefaultNavigationExecutionHost>();
        navigationExecutionHost.ShowGlobalPopupMenu(
            _solution,
            occurrences,
            activate: true,
            windowContext: _popupWindowContextSource,
            descriptorBuilder: descriptorBuilder,
            options: new OccurencePresentationOptions(),
            skipMenuIfSingleEnabled: true,
            title: "Go to linked types ");
      }
    }

    private GroupingEvent CreateExecuteGroupingEvent (IDataContext dataContext)
    {
      var shellLocks = dataContext.GetComponent<IShellLocks>();
      return shellLocks.GroupingEvents.CreateEvent(
          Lifetimes.Define().Lifetime,
          "GotoLinkedTypes",
          TimeSpan.FromMilliseconds(value: 1000),
          Rgc.Guarded,
          ExecuteProlongated);
    }

    #endregion
  }
}