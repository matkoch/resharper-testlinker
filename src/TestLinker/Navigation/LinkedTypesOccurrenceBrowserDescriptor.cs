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
using JetBrains.IDE.TreeBrowser;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TreeModels;
using JetBrains.Util;

namespace TestLinker.Navigation
{
  public class LinkedTypesOccurrenceBrowserDescriptor : OccurrenceBrowserDescriptor
  {
    private readonly TreeSectionModel _model;

    public LinkedTypesOccurrenceBrowserDescriptor (
        ISolution solution,
        ICollection<ITypeElement> typesInContext,
        ICollection<IOccurrence> linkedTypeOccurrences,
        IProgressIndicator indicator = null)
        : base(solution)
    {
      _model = new TreeSectionModel();

      DrawElementExtensions = true;
      Title.Value =
          $"Linked types for {typesInContext.Take(count: 3).Select(x => x.GetClrName().ShortName).Join(", ")}{(typesInContext.Count > 3 ? "..." : string.Empty)}";

      using (ReadLockCookie.Create())
      {
        // ReSharper disable once VirtualMemberCallInConstructor
        SetResults(linkedTypeOccurrences, indicator);
      }
    }

    public override TreeModel Model
    {
      get { return _model; }
    }

    protected override void SetResults (ICollection<IOccurrence> items, IProgressIndicator indicator = null, bool mergeItems = true)
    {
      base.SetResults(items, indicator, mergeItems);
      RequestUpdate(UpdateKind.Structure, immediate: true);
    }
  }
}