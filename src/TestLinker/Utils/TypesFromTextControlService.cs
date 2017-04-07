// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace TestLinker.Utils
{
    public interface ITypesFromTextControlService
    {
        IEnumerable<ITypeElement> GetTypesFromCaretOrFile (ITextControl textControl, ISolution solution);
        IEnumerable<ITypeElement> GetTypesNearCaretOrFile (ITextControl textControl, ISolution solution);
    }

    [PsiComponent]
    public class TypesFromTextControlService : ITypesFromTextControlService
    {
        #region ITypesFromTextControlService

        public IEnumerable<ITypeElement> GetTypesFromCaretOrFile (ITextControl textControl, ISolution solution)
        {
            var classDeclaration = TextControlToPsi.GetElementFromCaretPosition<ITypeDeclaration>(solution, textControl);
            if (classDeclaration != null)
                return new[] { classDeclaration.DeclaredElement };

            var symbolCache = solution.GetPsiServices().Symbols;
            var psiSourceFile = textControl.Document.GetPsiSourceFile(solution);
            if (psiSourceFile != null)
                return symbolCache.GetTypesAndNamespacesInFile(psiSourceFile).OfType<ITypeElement>();

            return EmptyList<ITypeElement>.InstanceList;
        }

        public IEnumerable<ITypeElement> GetTypesNearCaretOrFile (ITextControl textControl, ISolution solution)
        {
            var typesFromCaretOrFile = GetTypesFromCaretOrFile(textControl, solution);
            var typesFromNamespace = GetTypesFromNamespace(textControl, solution);
            var typesFromProject = GetTypesFromProject(textControl, solution);

            return typesFromCaretOrFile
                    .Concat(typesFromNamespace)
                    .Concat(typesFromProject);
        }

        private IEnumerable<ITypeElement> GetTypesFromNamespace (ITextControl textControl, ISolution solution)
        {
            var symbolCache = solution.GetPsiServices().Symbols;
            var namespaceDeclaration = TextControlToPsi.GetElementFromCaretPosition<INamespaceDeclaration>(solution, textControl);
            if (namespaceDeclaration?.DeclaredElement != null)
            {
                var symbolScope = symbolCache.GetSymbolScope(LibrarySymbolScope.FULL, caseSensitive: true);
                return namespaceDeclaration.DeclaredElement.GetNestedTypeElements(symbolScope);
            }

            return EmptyList<ITypeElement>.InstanceList;
        }

        private IEnumerable<ITypeElement> GetTypesFromProject (ITextControl textControl, ISolution solution)
        {
            var symbolCache = solution.GetPsiServices().Symbols;
            var psiSourceFile = textControl.Document.GetPsiSourceFile(solution).NotNull();
            var symbolScope = symbolCache.GetSymbolScope(psiSourceFile.PsiModule, withReferences: false, caseSensitive: false);

            foreach (var name in symbolScope.GetAllShortNames())
            {
                foreach (var element in symbolScope.GetElementsByShortName(name).OfType<ITypeElement>())
                {
                    yield return element;
                }
            }
        }

        #endregion
    }
}
