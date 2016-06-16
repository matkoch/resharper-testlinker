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
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TextControl;
using JetBrains.UI.PopupWindowManager;
using JetBrains.UI.StatusBar;
using JetBrains.Util;

namespace TestLinker.Utils
{
  public static class ModificationUtility
  {
    public static void TryCreateTestOrProductionClass (ITypeElement sourceType, ITextControl textControl)
    {
      var solution = sourceType.GetSolution();
      var linkedTypesService = solution.GetComponent<LinkedTypesService>();

      var typesNearCaretOrFile = solution.GetComponent<ITypesFromTextControlService>().GetTypesNearCaretOrFile(textControl, solution);
      var templateTypes = typesNearCaretOrFile.Select(x => GetLinkedTypeWithDerivedName(linkedTypesService, x)).WhereNotNull().FirstOrDefault();

      if (templateTypes == null)
      {
        MessageBox.ShowInfo("Could not find a template to create production/test class from.");
        return;
      }

      var templateSourceType = templateTypes.Item1;
      var templateLinkedType = templateTypes.Item2;

      var linkedTypeName = DerivedNameUtility.GetDerivedName(sourceType.ShortName, templateSourceType.ShortName, templateLinkedType.ShortName);
      var linkedTypeNamespace = DerivedNameUtility.GetDerivedNamespace(sourceType, templateLinkedType);
      var linkedTypeProject = templateLinkedType.GetSingleOrDefaultSourceFile().GetProject().NotNull();
      var linkedTypeKind = !solution.GetComponent<IUnitTestElementStuff>().IsElementOfKind(templateLinkedType, UnitTestElementKind.TestContainer)
          ? TypeKind.Production
          : TypeKind.Test;

      if (!MessageBox.ShowYesNo(
          $"Class: {linkedTypeName}\r\nProject: {linkedTypeProject.Name}\r\nNamespace: {linkedTypeNamespace}\r\n",
          $"Create {linkedTypeKind} Class for {sourceType.ShortName}?"))
      {
        return;
      }

      var linkedTypeProjectFolder = GetLinkedTypeFolder(linkedTypeNamespace, linkedTypeProject);
      var linkedTypeFile = GetLinkedTypeFile(linkedTypeName, linkedTypeNamespace, templateLinkedType);
      var linkedTypeProjectFile = AddNewItemUtil.AddFile(linkedTypeProjectFolder, linkedTypeName + ".cs", linkedTypeFile.GetText());
      linkedTypeProjectFile.Navigate(Shell.Instance.GetComponent<MainWindowPopupWindowContext>().Source, transferFocus: true);
    }

    [CanBeNull]
    private static Tuple<ITypeElement, ITypeElement> GetLinkedTypeWithDerivedName (LinkedTypesService linkedTypesService, ITypeElement sourceType)
    {
      return linkedTypesService.GetLinkedTypes(sourceType)
          .Where(x => DerivedNameUtility.IsDerivedNameAny(sourceType.ShortName, x.ShortName))
          .Select(x => Tuple.Create(sourceType, x))
          .FirstOrDefault();
    }

    private static ICSharpFile GetLinkedTypeFile (string linkedTypeName, string linkedTypeNamespace, ITypeElement templateLinkedType)
    {
      var elementFactory = CSharpElementFactory.GetInstance(templateLinkedType.Module);
      var templateFile = templateLinkedType.GetSingleOrDefaultSourceFile().GetPrimaryPsiFile().NotNull();

      var fileText = templateFile.GetText()
          .Replace(templateLinkedType.GetContainingNamespace().QualifiedName, linkedTypeNamespace)
          .Replace(templateLinkedType.ShortName, linkedTypeName);

      var linkedTypeFile = elementFactory.CreateFile(fileText);

      var typeDeclarations = GetTypeDeclarations(linkedTypeFile);
      var linkedType = (IClassDeclaration) typeDeclarations.Single(x => x.DeclaredName == linkedTypeName);

      // Remove base types
      linkedType.SuperTypes.ForEach(x => linkedType.RemoveSuperInterface(x));

      // Clear body
      linkedType.SetBody(((IClassLikeDeclaration) elementFactory.CreateTypeMemberDeclaration("class C{}")).Body);

      // Remove unrelated types
      linkedTypeFile.TypeDeclarations.Where(x => x.DeclaredName != linkedTypeName).ForEach(ModificationUtil.DeleteChild);

      return linkedTypeFile;
    }

    private static IProjectFolder GetLinkedTypeFolder (string linkedTypeNamespace, IProject linkedTypeProject)
    {
      var linkedTypeFolder = linkedTypeNamespace.TrimFromStart(linkedTypeProject.Name)
          .Split('.')
          .Aggregate(linkedTypeProject.Location, (currentFolder, nextFolder) => currentFolder.Combine(nextFolder));

      return linkedTypeProject.GetOrCreateProjectFolder(linkedTypeFolder).NotNull();
    }

    private static IReadOnlyCollection<ITypeDeclaration> GetTypeDeclarations (ICSharpFile csharpFile)
    {
      var namespaceDeclarations = csharpFile.NamespaceDeclarations.SelectMany(x => x.DescendantsAndSelf(y => y.NamespaceDeclarations));
      return namespaceDeclarations.Cast<ITypeDeclarationHolder>().SelectMany(x => x.TypeDeclarations)
          .SelectMany(x => x.DescendantsAndSelf(y => y.TypeDeclarations)).ToList();
    }
  }
}