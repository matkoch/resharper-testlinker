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
using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Feature.Services.Occurences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.PopupWindowManager;
using JetBrains.Util;
using TestFx.TestLinker.Navigation;

namespace TestFx.TestLinker.Actions
{
  [Action ("Goto_LinkedTypes", "Goto Linked Types", Id = 9854)]
  public class GotoLinkedTypesAction : IActionWithExecuteRequirement, IExecutableAction
  {
    #region IActionWithExecuteRequirement

    public IActionRequirement GetRequirement (IDataContext dataContext)
    {
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
      var solution = context.GetData(ProjectModelDataConstants.SOLUTION).NotNull();
      var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL).NotNull();
      var typesInContext = GetTypesInContext(textControl, solution).ToList();

      var linkedTypesProvider = solution.GetComponent<LinkedTypesService>();
      var linkedTypes = typesInContext.SelectMany(x => linkedTypesProvider.GetLinkedTypes(x)).ToHashSet();
      if (linkedTypes.IsEmpty())
        return;

      var occurrences = linkedTypes.Select(x => new LinkedTypesOccurrence(x, OccurenceType.Occurence)).ToList<IOccurence>();
      if (occurrences.Count == 1)
      {
        occurrences.Single().Navigate(solution, Shell.Instance.GetComponent<MainWindowPopupWindowContext>().Source, true);
      }
      else
      {
        Func<OccurenceBrowserDescriptor> descriptorBuilder = () => new LinkedTypesOccurrenceBrowserDescriptor(solution, typesInContext, occurrences);
        var popup = context.GetComponent<OccurencePopupMenu>();
        popup.ShowMenuFromTextControl(context, occurrences, descriptorBuilder, new OccurencePresentationOptions(), true, "Go to linked types ");
      }
    }

    #endregion

    #region Privates

    private IEnumerable<ITypeElement> GetTypesInContext (ITextControl textControl, ISolution solution)
    {
      var classDeclaration = TextControlToPsi.GetElementFromCaretPosition<ITypeDeclaration>(solution, textControl);
      if (classDeclaration != null)
        return new[] { classDeclaration.DeclaredElement };

      var symbolCache = solution.GetPsiServices().Symbols;
      //var namespaceDeclaration = TextControlToPsi.GetElementFromCaretPosition<INamespaceDeclaration>(solution, textControl);
      //if (namespaceDeclaration != null && namespaceDeclaration.DeclaredElement != null)
      //{
      //  var symbolScope = symbolCache.GetSymbolScope(LibrarySymbolScope.FULL, caseSensitive: true);
      //  return namespaceDeclaration.DeclaredElement.GetNestedTypeElements(symbolScope);
      //}

      var psiSourceFile = textControl.Document.GetPsiSourceFile(solution);
      if (psiSourceFile != null)
        return symbolCache.GetTypesAndNamespacesInFile(psiSourceFile).OfType<ITypeElement>();

      return Enumerable.Empty<ITypeElement>();
    }

    #endregion
  }
}