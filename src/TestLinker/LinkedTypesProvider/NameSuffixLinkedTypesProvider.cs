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
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using TestLinker.Options;

namespace TestLinker.LinkedTypesProvider
{
  [PsiComponent]
  internal class NameSuffixLinkedTypesProvider : ILinkedTypesProvider
  {
    private readonly NamingStyle _namingStyle;
    private readonly List<Pair<string, int>> _namingSuffixes;

    public NameSuffixLinkedTypesProvider (ISolution solution, ISettingsStore settingsStore, ISettingsOptimization settingsOptimization)
    {
      var contextBoundSettingsStore = settingsStore.BindToContextTransient(ContextRange.Smart(solution.ToDataContext()));
      var settings = contextBoundSettingsStore.GetKey<TestLinkerSettings>(settingsOptimization);

      _namingStyle = settings.NamingStyle;
      _namingSuffixes = settings.NamingSuffixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(x => Pair.Of(x, x.Length)).ToList();
    }

    #region ILinkedTypesProvider

    public IEnumerable<string> GetLinkedNames (ITypeDeclaration typeDeclaration)
    {
      var linkedName = GetLinkedName(typeDeclaration.DeclaredName);
      return linkedName != null
          ? new[] { linkedName }
          : EmptyList<string>.InstanceList;
    }

    [CanBeNull]
    private string GetLinkedName (string name)
    {
      var isBase = false;
      var baseName = name;
      if (baseName.EndsWith("Base"))
      {
        isBase = true;
        baseName = baseName.Substring(startIndex: 0, length: baseName.Length - 4);
      }

      var linkedName = default(string);
      foreach (var namingSuffix in _namingSuffixes)
      {
        if (_namingStyle == NamingStyle.Prefix && baseName.StartsWith(namingSuffix.First))
        {
          linkedName = baseName.Substring(namingSuffix.Second);
          break;
        }

        if (_namingStyle == NamingStyle.Postfix && baseName.EndsWith(namingSuffix.First))
        {
          linkedName = baseName.Substring(startIndex: 0, length: baseName.Length - namingSuffix.Second);
          break;
        }
      }

      if (linkedName == null)
        return null;

      if (isBase)
        linkedName = "I" + linkedName;
      return linkedName;
    }

    public bool IsLinkedType (ITypeElement type1, ITypeElement type2)
    {
      return type1.ShortName.Equals(GetLinkedName(type2.ShortName), StringComparison.InvariantCultureIgnoreCase);
    }

    #endregion
  }
}