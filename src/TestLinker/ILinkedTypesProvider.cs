// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace TestLinker
{
    public interface ILinkedTypesProvider
    {
        IEnumerable<string> GetLinkedNames (ITypeDeclaration typeDeclaration);

        bool IsLinkedType (ITypeElement type1, ITypeElement type2);
    }
}
