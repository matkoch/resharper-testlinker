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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace TestLinker.LinkedTypesProvider
{
  [PsiComponent]
  internal class SubjectAttributeLinkedTypesProvider : ILinkedTypesProvider
  {
    #region ILinkedTypesProvider

    public IEnumerable<string> GetLinkedNames (ITypeDeclaration typeDeclaration)
    {
      var attributesOwnerDeclaration = typeDeclaration as IAttributesOwnerDeclaration;
      if (attributesOwnerDeclaration == null)
        yield break;

      foreach (var attribute in attributesOwnerDeclaration.Attributes)
      {
        if (attribute.Name.ShortName != "Subject" && attribute.Name.ShortName != "SubjectAttribute")
          continue;

        foreach (var typeArgument in attribute.Arguments.Select(x => x.Value).OfType<ITypeofExpression>())
          yield return typeArgument.ArgumentType.GetPresentableName(typeDeclaration.GetSourceFile().NotNull().PrimaryPsiLanguage);
      }
    }

    public bool IsLinkedType (ITypeElement type1, ITypeElement type2)
    {
      var typeArguments = GetSubjectAttributeTypeArguments(type1);
      return typeArguments.Any(x => x.Equals(type2));
    }

    #endregion

    #region Privates

    private IEnumerable<ITypeElement> GetSubjectAttributeTypeArguments (IAttributesSet attributeSet)
    {
      return attributeSet.GetAttributeInstances(true)
          .Where(x => x.GetClrName().ShortName == "SubjectAttribute")
          .SelectMany(GetTypeArguments);
    }

    private IEnumerable<ITypeElement> GetTypeArguments (IAttributeInstance subjectAttribute)
    {
      var namedArguments = subjectAttribute.NamedParameters().Select(x => x.Second);
      var positionalArguments = subjectAttribute.PositionParameters();
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