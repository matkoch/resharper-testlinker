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
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties.Managed;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace TestLinker.Utils
{
  public static class DerivedNameUtility
  {
    public static string GetDerivedNamespace (ITypeElement sourceType, ITypeElement templateLinkedType)
    {
      var sourceDefaultNamespace = GetDefaultNamespace(sourceType.GetSingleOrDefaultSourceFile().GetProject().NotNull());
      var linkedDefaultNamespace = GetDefaultNamespace(templateLinkedType.GetSingleOrDefaultSourceFile().GetProject().NotNull());
      var sourceNamespaceTail = sourceType.GetContainingNamespace().QualifiedName.TrimFromStart(sourceDefaultNamespace);

      return linkedDefaultNamespace + sourceNamespaceTail;
    }

    private static string GetDefaultNamespace (IProject sourceProject)
    {
      var projectBuildSettings = (IManagedProjectBuildSettings) sourceProject.ProjectProperties.BuildSettings;
      return projectBuildSettings?.DefaultNamespace ?? string.Empty;
    }

    public static string GetDerivedName (string sourceName, string templateSourceName, string templateLinkedName)
    {
      return templateSourceName.Contains(templateLinkedName)
          ? sourceName.Replace(templateSourceName.Replace(templateLinkedName, string.Empty), string.Empty)
          : templateLinkedName.Replace(templateSourceName, sourceName);
    }

    public static bool IsDerivedName(string baseName, string derivedName)
    {
      return derivedName.Contains(baseName);
    }

    public static bool IsDerivedNameAny(string name1, string name2)
    {
      if (name1.Length == name2.Length)
        return name1 == name2;

      return name1.Length > name2.Length
          ? IsDerivedName(name2, name1)
          : IsDerivedName(name1, name2);
    }
  }
}