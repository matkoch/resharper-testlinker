// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using JetBrains.Application.Settings;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.TestLinker.Options
{
    [SettingsKey(typeof(UnitTestingSettings), "Settings for TestLinker")]
    public class TestLinkerSettings
    {
        [SettingsEntry(DefaultValue: true, Description: "Use Suffix Search")]
        public bool EnableSuffixSearch;

        [SettingsEntry(NamingStyle.Postfix, "Naming style for tests")]
        public NamingStyle NamingStyle;

        [SettingsEntry("Test,Spec,Tests,Specs", "Naming Suffixes")]
        public string NamingSuffixes;

        [SettingsEntry(DefaultValue: true, Description: "Use Typeof Search")]
        public bool EnableTypeofSearch;

        [SettingsEntry("SubjectAttribute", "Typeof Attribute")]
        public string TypeofAttributeName;
    }
}
