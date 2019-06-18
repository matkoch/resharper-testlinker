// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TextControl;

namespace ReSharperPlugin.TestLinker
{
    [ZoneMarker]
    public class ZoneMarker
            : IRequire<IUnitTestingZone>,
                    IRequire<ILanguageCSharpZone>,
                    IRequire<DaemonEngineZone>,
                    IRequire<ITextControlsZone>
    {
    }
}
