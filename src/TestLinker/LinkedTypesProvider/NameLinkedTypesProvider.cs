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
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace TestLinker.LinkedTypesProvider
{
  [PsiComponent]
  internal class NameLinkedTypesProvider : ILinkedTypesProvider
  {
    #region ILinkedTypesProvider

    public IEnumerable<string> GetLinkedNames (ITypeDeclaration typeDeclaration)
    {
      var baseName = typeDeclaration.DeclaredName;

      if (baseName.EndsWith("Test") || baseName.EndsWith("Spec"))
        return new[] { baseName.Substring(0, baseName.Length - 4) };

      if (baseName.EndsWith("Tests") || baseName.EndsWith("Specs"))
        return new[] { baseName.Substring(0, baseName.Length - 5) };

      if (baseName.EndsWith("TestBase") || baseName.EndsWith("SpecBase"))
      {
        var name = baseName.Substring(0, baseName.Length - 8);
        return new[] { name, "I" + name };
      }

      return EmptyList<string>.InstanceList;
    }

    public bool IsLinkedType (ITypeElement type1, ITypeElement type2)
    {
      return type1.ShortName.StartsWith(type2.ShortName);
    }

    #endregion
  }
}