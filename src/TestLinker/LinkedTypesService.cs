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
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using TestLinker.Caching;

namespace TestLinker
{
    [PsiComponent]
    public class LinkedTypesService
    {
        private readonly LinkedNamesCache _linkedNamesCache;
        private readonly IUnitTestElementStuff _unitTestElementStuff;
        private readonly IReadOnlyList<ILinkedTypesProvider> _linkedTypesProviders;

        public LinkedTypesService (
            LinkedNamesCache linkedNamesCache,
            IEnumerable<ILinkedTypesProvider> linkedTypesProviders,
            IUnitTestElementStuff unitTestElementStuff)
        {
            _linkedNamesCache = linkedNamesCache;
            _unitTestElementStuff = unitTestElementStuff;
            _linkedTypesProviders = linkedTypesProviders.ToList();
        }

        public IEnumerable<ITypeElement> GetLinkedTypes (IReadOnlyList<ITypeElement> allSourceTypes, IPsiServices services)
        {
            foreach (var sourceType in allSourceTypes)
            {
                foreach (var linkedType in GetLinkedTypes(sourceType))
                {
                    yield return linkedType;

                    if (!ReferenceEquals(sourceType, allSourceTypes[index: 0]))
                        continue;

                    foreach (var derivedLinkedType in services.Finder.FindInheritors(linkedType, NullProgressIndicator.Create()))
                        yield return derivedLinkedType;
                }
            }
        }

        public ISet<ITypeElement> GetLinkedTypes (IEnumerable<ITypeElement> sourceTypes)
        {
            return GetLinkedTypesEnumerable(sourceTypes).ToSet();
        }

        public IEnumerable<ITypeElement> GetLinkedTypesEnumerable (IEnumerable<ITypeElement> sourceTypes)
        {
            foreach (var sourceType in sourceTypes)
            {
                var sourceSuperTypes = sourceType.GetAllSuperTypes().Select(x => x.GetTypeElement()).WhereNotNull().Where(x => !x.IsObjectClass());
                var allSourceTypes = new[] { sourceType }.Concat(sourceSuperTypes).ToList();

                var services = sourceType.GetPsiServices();
                var linkedTypes = GetLinkedTypes(allSourceTypes, services);
                foreach (var linkedType in linkedTypes)
                    yield return linkedType;
            }
        }

        public ISet<IUnitTestElement> GetUnitTestElementsFrom (IEnumerable<ITypeElement> sourceTypes)
        {
            var sourceTypesList = sourceTypes.ToList();
            var linkedTypes = GetLinkedTypes(sourceTypesList);
            var affectedTypes = sourceTypesList.Concat(linkedTypes);
            return affectedTypes.Select(x => _unitTestElementStuff.GetElement(x)).WhereNotNull().ToHashSet();
        }

        #region Privates

        public IEnumerable<ITypeElement> GetLinkedTypes (ITypeElement sourceType)
        {
            var services = sourceType.GetPsiServices();
            var symbolScope = services.Symbols.GetSymbolScope(LibrarySymbolScope.FULL, caseSensitive: false);
            var linkedNames = _linkedNamesCache.LinkedNamesMap[sourceType.ShortName];
            var possibleLinkedTypes = linkedNames.SelectMany(x => symbolScope.GetElementsByShortName(x.Second).OfType<ITypeElement>());

            foreach (var possibleLinkedType in possibleLinkedTypes)
            {
                if (_linkedTypesProviders.Any(x => x.IsLinkedType(sourceType, possibleLinkedType)) ||
                    _linkedTypesProviders.Any(x => x.IsLinkedType(possibleLinkedType, sourceType)))
                    yield return possibleLinkedType;
            }
        }

        #endregion
    }
}
