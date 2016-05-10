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
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;

namespace TestFx.TestLinker.Navigation
{
  [RelatedFilesProvider (typeof(KnownProjectFileType))]
  public class LinkedTypesRelatedFilesProvider : IRelatedFilesProvider
  {
    public IEnumerable<Tuple<IProjectFile, string, IProjectFile>> GetRelatedFiles (IProjectFile projectFile)
    {
      var sourceFile = projectFile.ToSourceFile();
      if (sourceFile == null)
        yield break;

      var services = sourceFile.GetPsiServices();
      var linkedTypesProvider = services.GetComponent<LinkedTypesService>();
      var sourceElements = services.Symbols.GetTypesAndNamespacesInFile(sourceFile).OfType<ITypeElement>();
      var targetElements = sourceElements.SelectMany(x => linkedTypesProvider.GetLinkedTypes(x));
      foreach (var targetElement in targetElements)
      {
        var targetFile = targetElement.GetSingleOrDefaultSourceFile().ToProjectFile();
        yield return Tuple.Create(targetFile, "Linked Type", projectFile);
      }
    }
  }
}