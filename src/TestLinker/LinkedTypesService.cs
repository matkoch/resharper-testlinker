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
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using TestFx.TestLinker.Caching;

namespace TestFx.TestLinker
{
  [PsiComponent]
  public class LinkedTypesService
  {
    private readonly LinkedNamesCache _linkedNamesCache;
    private readonly IReadOnlyList<ILinkedTypesProvider> _linkedTypesProviders;

    public LinkedTypesService (LinkedNamesCache linkedNamesCache, IEnumerable<ILinkedTypesProvider> linkedTypesProviders)
    {
      _linkedNamesCache = linkedNamesCache;
      _linkedTypesProviders = linkedTypesProviders.ToList();
    }

    public IEnumerable<ITypeElement> GetLinkedTypes (ITypeElement sourceType)
    {
      var sourceSuperTypes = sourceType.GetAllSuperTypes().Select(x => x.GetTypeElement()).WhereNotNull().Where(x => !x.IsObjectClass());
      var allSourceTypes = new[] { sourceType }.Concat(sourceSuperTypes);

      var services = sourceType.GetPsiServices();
      var linkedTypes = GetLinkedTypes(services, allSourceTypes).ToList();
      var linkedDerivedTypes = linkedTypes.SelectMany(x => services.Finder.FindInheritors(x, NullProgressIndicator.Instance));

      var allLinkedTypes = new HashSet<ITypeElement>(linkedTypes);
      allLinkedTypes.AddRange(linkedDerivedTypes);
      return allLinkedTypes;
    }

    #region Privates

    private IEnumerable<ITypeElement> GetLinkedTypes (IPsiServices services, IEnumerable<ITypeElement> sourceTypes)
    {
      var symbolScope = services.Symbols.GetSymbolScope(LibrarySymbolScope.FULL, false);
      foreach (var sourceType in sourceTypes)
      {
        var linkedNames = _linkedNamesCache.LinkedNamesMap[sourceType.ShortName];
        var possibleLinkedTypes = linkedNames.SelectMany(x => symbolScope.GetElementsByShortName(x.Second).OfType<ITypeElement>());

        foreach (var possibleLinkedType in possibleLinkedTypes)
        {
          if (_linkedTypesProviders.Any(x => x.IsLinkedType(sourceType, possibleLinkedType)) ||
              _linkedTypesProviders.Any(x => x.IsLinkedType(possibleLinkedType, sourceType)))
            yield return possibleLinkedType;
        }
      }
    }

    #endregion
  }
}