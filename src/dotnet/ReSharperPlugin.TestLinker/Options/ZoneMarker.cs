// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.UI.Options.OptionPages;

namespace ReSharperPlugin.TestLinker.Options
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IToolsOptionsPageImplZone>
    {
    }
}
