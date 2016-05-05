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
using JetBrains.DataFlow;
using JetBrains.DocumentManagers.impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using JetBrains.Util.PersistentMap;

namespace TestFx.TestLinker
{
  [PsiComponent]
  public class LinkedNamesCache : SimpleICache<LinkedNamesData>
  {
    private MergeData _mergeData;

    public LinkedNamesCache (Lifetime lifetime, IPersistentIndexManager persistentIndexManager)
        : base(lifetime, persistentIndexManager, new Marshaller())
    {
    }

    public override string Version
    {
      get { return "6"; }
    }

    public OneToSetMap<string, Pair<IPsiSourceFile, string>> LinkedNamesMap
    {
      get { return _mergeData.LinkedNamesMap; }
    }

    public override object Load (IProgressIndicator progress, bool enablePersistence)
    {
      var data = new MergeData();
      Map.ForEach(x => LoadFile(data, x.Key, x.Value));
      return data;
    }

    public override void MergeLoaded (object data)
    {
      _mergeData = (MergeData) data;

      base.MergeLoaded(data);
    }

    public override object Build (IPsiSourceFile sourceFile, bool isStartup)
    {
      return GetLinkData(sourceFile);
    }

    public override void Merge (IPsiSourceFile sourceFile, object builtPart)
    {
      var linkedNamesData = (LinkedNamesData) builtPart;
      if (linkedNamesData == null)
        return;

      base.Merge(sourceFile, linkedNamesData);

      RemoveData(sourceFile);

      LoadFile(_mergeData, sourceFile, linkedNamesData);
    }

    public override void Drop (IPsiSourceFile sourceFile)
    {
      var linkedNamesData = GetLinkData(sourceFile);
      if (linkedNamesData == null)
        return;

      RemoveData(sourceFile);

      base.Drop(sourceFile);
    }

    protected override bool IsApplicable (IPsiSourceFile sourceFile)
    {
      return sourceFile.GetDominantPsiFile<CSharpLanguage>() != null;
    }

    private void LoadFile (MergeData data, IPsiSourceFile sourceFile, LinkedNamesData linkedNamesData)
    {
      var sourceNames = linkedNamesData.Keys;
      data.PreviousNamesMap.AddRange(sourceFile, sourceNames);
      foreach (var sourceName in sourceNames)
      {
        var linkedNames = linkedNamesData[sourceName];
        data.PreviousNamesMap.AddRange(sourceFile, linkedNames);

        foreach (var linkedName in linkedNames)
        {
          data.LinkedNamesMap.Add(sourceName, Pair.Of(sourceFile, linkedName));
          data.LinkedNamesMap.Add(linkedName, Pair.Of(sourceFile, sourceName));
        }
      }
    }

    private void RemoveData (IPsiSourceFile sourceFile)
    {
      var previousNames = _mergeData.PreviousNamesMap[sourceFile];
      foreach (var previousName in previousNames)
        _mergeData.LinkedNamesMap.RemoveWhere(previousName, x => x.First == sourceFile);

      _mergeData.PreviousNamesMap.RemoveKey(sourceFile);
    }

    private LinkedNamesData GetLinkData (IPsiSourceFile sourceFile)
    {
      var linkedNamesData = new LinkedNamesData();
      foreach (var sourceType in GetTypeDeclarations(sourceFile.GetPrimaryPsiFile()))
      {
        // TODO: extension
        var attributesOwnerDeclaration = sourceType as IAttributesOwnerDeclaration;
        if (attributesOwnerDeclaration == null)
          continue;

        foreach (var attribute in attributesOwnerDeclaration.Attributes)
        {
          if (attribute.Name.ShortName != "Subject" && attribute.Name.ShortName != "SubjectAttribute")
            continue;

          foreach (var typeArgument in attribute.Arguments.Select(x => x.Value).OfType<ITypeofExpression>())
          {
            var linkedType = typeArgument.ArgumentType.GetPresentableName(sourceFile.PrimaryPsiLanguage);
            linkedNamesData.Add(sourceType.DeclaredName, linkedType);
          }
        }
      }

      return linkedNamesData;
    }

    private IEnumerable<ITypeDeclaration> GetTypeDeclarations (ITreeNode node)
    {
      var namespaceDeclarationHolder = node as INamespaceDeclarationHolder;
      if (namespaceDeclarationHolder != null)
        foreach (var typeDeclaration in namespaceDeclarationHolder.NamespaceDeclarations.SelectMany(GetTypeDeclarations))
          yield return typeDeclaration;

      var typeDeclarationHolder = node as ITypeDeclarationHolder;
      if (typeDeclarationHolder != null)
        foreach (var typeDeclaration in typeDeclarationHolder.TypeDeclarations)
          yield return typeDeclaration;
    }

    private class MergeData
    {
      public readonly OneToSetMap<string, Pair<IPsiSourceFile, string>> LinkedNamesMap = new OneToSetMap<string, Pair<IPsiSourceFile, string>>();
      public readonly OneToSetMap<IPsiSourceFile, string> PreviousNamesMap = new OneToSetMap<IPsiSourceFile, string>();
    }

    private class Marshaller : IUnsafeMarshaller<LinkedNamesData>
    {
      public void Marshal (UnsafeWriter writer, LinkedNamesData value)
      {
        writer.Write(value.Count);
        foreach (var map in value)
        {
          writer.Write(map.Key);
          writer.Write(map.Value.Count);
          map.Value.ForEach(writer.Write);
        }
      }

      public LinkedNamesData Unmarshal (UnsafeReader reader)
      {
        var linkData = new LinkedNamesData();
        var count = reader.ReadInt();
        for (var i = 0; i < count; i++)
        {
          var sourceType = reader.ReadString();
          var listCount = reader.ReadInt();
          var linkedTypes = Enumerable.Range(0, listCount).Select(x => reader.ReadString());
          linkData.AddValueRange(sourceType, linkedTypes);
        }
        return linkData;
      }
    }
  }
}