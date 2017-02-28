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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.Application.Progress;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Threading;
using JetBrains.Util;

namespace TestLinker
{
  [PsiComponent]
  internal class ChangedTypesProvider
  {
    private static readonly TimeSpan s_updateInterval = TimeSpan.FromMilliseconds(value: 1000);

    private readonly Lifetime _myLifetime;
    private readonly IPsiServices _myServices;
    private readonly IShellLocks _myShellLocks;
    private readonly DocumentManager _myDocumentManager;

    private readonly ConcurrentDictionary<IProjectFile, TextRange> _myChangedRanges;
    private readonly GroupingEvent _myDocumentChangedEvent;

    public ChangedTypesProvider (
        Lifetime lifetime,
        IShellLocks shellLocks,
        ChangeManager changeManager,
        DocumentManager documentManager,
        IPsiServices services)
    {
      _myServices = services;
      _myLifetime = lifetime;
      _myShellLocks = shellLocks;
      _myDocumentManager = documentManager;
      _myChangedRanges = new ConcurrentDictionary<IProjectFile, TextRange>();

      changeManager.Changed2.Advise(lifetime, OnChange);
      _myDocumentChangedEvent = shellLocks.CreateGroupingEvent(
          lifetime,
          "ChangedTypesProvider::DocumentChanged",
          s_updateInterval,
          OnProcessChangesEx);

      TypesChanged = new Signal<IReadOnlyCollection<ITypeElement>>(lifetime, "ChangedTypesProvider");
    }

    public ISignal<IReadOnlyCollection<ITypeElement>> TypesChanged { get; }

    #region Privates

    private void OnChange (ChangeEventArgs e)
    {
      var change = e.ChangeMap.GetChange<ProjectFileDocumentChange>(_myDocumentManager.ChangeProvider);
      if (change == null)
        return;

      lock (_myChangedRanges)
      {
        var changeRange = new TextRange(change.StartOffset, change.StartOffset + change.OldLength);
        _myChangedRanges.AddOrUpdate(change.ProjectFile, changeRange, (file, range) => changeRange.Join(range));
      }

      _myDocumentChangedEvent.FireIncoming();
    }

    private void OnProcessChangesEx ()
    {
      using (_myShellLocks.UsingReadLock())
      {
        _myServices.Files.CommitAllDocumentsAsync(
            () =>
            {
              var the = new InterruptableReadActivityThe(_myLifetime, _myShellLocks, () => false) { FuncRun = Invalidate };
              the.DoStart();
            },
            () => { });
      }
    }

    private void Invalidate ()
    {
      var changes = GetChanges();

      try
      {
        var allChangedTypes = new List<ITypeElement>();

        foreach (var changedRange in changes)
        {
          var sourceFile = changedRange.Key.ToSourceFile();

          var containedTypes = _myServices.Symbols.GetTypesAndNamespacesInFile(sourceFile).OfType<ITypeElement>();
          var changedTypes = containedTypes.Where(x => x.GetDeclarationsIn(sourceFile).Any(y => changedRange.Value.ContainedIn(y.GetDocumentRange().TextRange)));

          allChangedTypes.AddRange(changedTypes);
        }

        TypesChanged.Fire(allChangedTypes);
      }
      catch (ProcessCancelledException)
      {
        ReAddChanges(changes);
      }
    }

    private KeyValuePair<IProjectFile, TextRange>[] GetChanges ()
    {
      // TODO: try-finally?
      KeyValuePair<IProjectFile, TextRange>[] changes;
      lock (_myChangedRanges)
      {
        changes = _myChangedRanges.Where(x => x.Key.IsValid() && x.Value.IsValid).ToArray();
        _myChangedRanges.Clear();
      }
      return changes;
    }

    private void ReAddChanges (KeyValuePair<IProjectFile, TextRange>[] changes)
    {
      lock (_myChangedRanges)
      {
        foreach (var pair in changes)
          _myChangedRanges.AddOrUpdate(pair.Key, pair.Value, (file, range) => pair.Value.Join(range));
      }
    }

    #endregion
  }
}