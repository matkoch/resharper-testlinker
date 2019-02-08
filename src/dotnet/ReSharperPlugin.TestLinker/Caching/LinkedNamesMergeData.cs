// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace TestLinker.Caching
{
    public class LinkedNamesMergeData
    {
        public readonly OneToSetMap<string, Pair<IPsiSourceFile, string>> LinkedNamesMap = new OneToSetMap<string, Pair<IPsiSourceFile, string>>();
        public readonly OneToSetMap<IPsiSourceFile, string> PreviousNamesMap = new OneToSetMap<IPsiSourceFile, string>();
    }
}
