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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace TestLinker.Utils
{
  public interface ITypesInContextProvider
  {
    IEnumerable<ITypeElement> GetTypesInContext (ITextControl textControl, ISolution solution);
  }

  [PsiComponent]
  public class TypesInContextProvider : ITypesInContextProvider
  {
    #region ITypesInContextProvider

    public IEnumerable<ITypeElement> GetTypesInContext (ITextControl textControl, ISolution solution)
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