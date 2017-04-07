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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using TestLinker.Options;

namespace TestLinker.LinkedTypesProvider
{
    [PsiComponent]
    internal class TypeofAttributeLinkedTypesProvider : ILinkedTypesProvider
    {
        private readonly string _attributeNameShort;
        private readonly string _attributeNameLong;

        public TypeofAttributeLinkedTypesProvider (ISolution solution, ISettingsStore settingsStore, ISettingsOptimization settingsOptimization)
        {
            var contextBoundSettingsStore = settingsStore.BindToContextTransient(ContextRange.Smart(solution.ToDataContext()));
            var settings = contextBoundSettingsStore.GetKey<TestLinkerSettings>(settingsOptimization);

            var attributeNameLong = settings.TypeofAttributeName;
            if (!attributeNameLong.EndsWith("Attribute"))
                attributeNameLong += "Attribute";

            _attributeNameLong = attributeNameLong;
            _attributeNameShort = attributeNameLong.Substring(startIndex: 0, length: attributeNameLong.Length - "Attribute".Length);
        }

        #region ILinkedTypesProvider

        [ItemNotNull]
        // ReSharper disable once CyclomaticComplexity
        public IEnumerable<string> GetLinkedNames (ITypeDeclaration typeDeclaration)
        {
            var attributesOwnerDeclaration = typeDeclaration as IAttributesOwnerDeclaration;
            if (attributesOwnerDeclaration == null)
                yield break;

            foreach (var attribute in attributesOwnerDeclaration.Attributes)
            {
                if (attribute.Name.ShortName != _attributeNameShort && attribute.Name.ShortName != _attributeNameLong)
                    continue;

                foreach (var typeArgument in attribute.Arguments.Select(x => x.Value).OfType<ITypeofExpression>())
                    yield return typeArgument.ArgumentType.GetPresentableName(typeDeclaration.GetSourceFile().NotNull().PrimaryPsiLanguage);
            }
        }

        public bool IsLinkedType (ITypeElement type1, ITypeElement type2)
        {
            var typeArguments = GetTypeArguments(type1);
            return typeArguments.Any(x => x.Equals(type2));
        }

        #endregion

        #region Privates

        private IEnumerable<ITypeElement> GetTypeArguments (IAttributesSet attributeSet)
        {
            var attributeInstance = attributeSet.GetAttributeInstances(inherit: true)
                    .Where(x => x.GetClrName().ShortName == _attributeNameLong)
                    .FirstOrDefault();

            if (attributeInstance == null)
                return EmptyList<ITypeElement>.InstanceList;

            var namedArguments = attributeInstance.NamedParameters().Select(x => x.Second);
            var positionalArguments = attributeInstance.PositionParameters();
            var flattenedArguments = FlattenArguments(namedArguments.Concat(positionalArguments));

            return flattenedArguments
                    .Where(x => x.IsType && !x.IsBadValue)
                    .Select(x => x.TypeValue.GetTypeElement())
                    .WhereNotNull();
        }

        private IEnumerable<AttributeValue> FlattenArguments (IEnumerable<AttributeValue> attributeValues)
        {
            foreach (var attributeValue in attributeValues)
            {
                if (!attributeValue.IsArray)
                {
                    yield return attributeValue;
                }
                else
                {
                    foreach (var innerAttributeValue in FlattenArguments(attributeValue.ArrayValue.NotNull()))
                        yield return innerAttributeValue;
                }
            }
        }

        #endregion
    }
}
