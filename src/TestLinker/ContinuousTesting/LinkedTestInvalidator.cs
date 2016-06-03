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
using JetBrains.dotCover.Core.Vs.ContinuousTesting.Model.Interface;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace TestLinker.ContinuousTesting
{
  [PsiComponent]
  internal class LinkedTestInvalidator
  {
    private readonly LinkedTypesService _linkedTypesService;
    private readonly IUnitTestElementStuff _unitTestElementStuff;
    private readonly IUnitTestResultManager _unitTestResultManager;
    private readonly IContinuousTestingUnitTestSessionHolder _continuousTestingUnitTestSessionHolder;

    public LinkedTestInvalidator (
        Lifetime lifetime,
        ChangedTypesProvider changedTypesProvider,
        LinkedTypesService linkedTypesService,
        IUnitTestElementStuff unitTestElementStuff,
        IUnitTestResultManager unitTestResultManager,
        IContinuousTestingUnitTestSessionHolder continuousTestingUnitTestSessionHolder)
    {
      _linkedTypesService = linkedTypesService;
      _unitTestElementStuff = unitTestElementStuff;
      _unitTestResultManager = unitTestResultManager;
      _continuousTestingUnitTestSessionHolder = continuousTestingUnitTestSessionHolder;

      changedTypesProvider.TypesChanged.Advise(lifetime, OnChanged);
    }

    #region Privates

    private void OnChanged (IReadOnlyCollection<ITypeElement> changedTypes)
    {
      var testElements = _linkedTypesService.GetUnitTestElementsFrom(changedTypes).SelectMany(GetChildrenAndSelf);
      testElements.ForEach(x => _unitTestResultManager.MarkOutdated(x, _continuousTestingUnitTestSessionHolder.Session.Value));
    }

    private IEnumerable<IUnitTestElement> GetChildrenAndSelf (IUnitTestElement parent)
    {
      yield return parent;
      foreach (var children in parent.Children.SelectMany(GetChildrenAndSelf))
        yield return children;
    }

    #endregion
  }
}