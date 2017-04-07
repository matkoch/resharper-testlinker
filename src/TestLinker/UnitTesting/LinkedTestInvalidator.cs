// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
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
