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
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using TestLinker.Utils;

namespace TestLinker.UnitTesting
{
  [PsiComponent]
  internal class LinkedTestInvalidator
  {
    private readonly LinkedTypesService _linkedTypesService;
    private readonly IUnitTestResultManager _unitTestResultManager;

    public LinkedTestInvalidator (
        Lifetime lifetime,
        ChangedTypesProvider changedTypesProvider,
        LinkedTypesService linkedTypesService,
        IUnitTestResultManager unitTestResultManager)
    {
      _linkedTypesService = linkedTypesService;
      _unitTestResultManager = unitTestResultManager;

      changedTypesProvider.TypesChanged.Advise(lifetime, OnChanged);
    }

    #region Privates

    private void OnChanged (IReadOnlyCollection<ITypeElement> changedTypes)
    {
      var testElements = _linkedTypesService.GetUnitTestElementsFrom(changedTypes).SelectMany(x => x.DescendantsAndSelf(y => y.Children));
      foreach (var x in testElements)
        _unitTestResultManager.MarkOutdated(x);
    }

    #endregion
  }
}