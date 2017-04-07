// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions;
using JetBrains.UI.Options.OptionsDialog2.SimpleOptions.ViewModel;

namespace TestLinker.Options
{
    [OptionsPage(c_pageId, c_pageTitle, typeof(TestLinkerThemedIcons.TestLinker), ParentId = UnitTestingOptionsPageIdHolder.PID, Sequence = 0.1d)]
    public class TestLinkerOptionsPage : SimpleOptionsPage
    {
        private const string c_pageId = "TestLinkerOptions";
        private const string c_pageTitle = "Test Linker";

        private readonly Lifetime _lifetime;

        public TestLinkerOptionsPage (Lifetime lifetime, OptionsSettingsSmartContext settings)
            : base(lifetime, settings)
        {
            _lifetime = lifetime;

            AddSuffixSearchOptions();
            AddTypeofSearchOptions();

            AddText(
                "\nWarning: After changing these settings, " +
                "cleaning the solution cache (see \"General\" options page) " +
                "is necessary to update already analyzed code.");

            FinishPage();
        }

        #region Privates

        private void AddSuffixSearchOptions ()
        {
            var enableSuffixSearchOption = AddBoolOption((TestLinkerSettings x) => x.EnableSuffixSearch, "Enable Suffix Search:");

            var testSuffixOption = AddStringOption((TestLinkerSettings x) => x.NamingSuffixes, "Naming Suffixes for Tests, separated by comma:");
            var namingStyleOption = AddRadioOption(
                (TestLinkerSettings x) => x.NamingStyle,
                "Naming Style:",
                null,
                new RadioOptionPoint(NamingStyle.Postfix, "Postfix"),
                new RadioOptionPoint(NamingStyle.Prefix, "Prefix"));
            SetIndent(testSuffixOption, indent: 2);
            SetIndent(namingStyleOption, indent: 2);
            enableSuffixSearchOption.CheckedProperty.FlowInto(_lifetime, testSuffixOption.GetIsEnabledProperty());
            enableSuffixSearchOption.CheckedProperty.FlowInto(_lifetime, namingStyleOption.GetIsEnabledProperty());
        }

        private void AddTypeofSearchOptions ()
        {
            var enableTypeofSearchOption = AddBoolOption((TestLinkerSettings x) => x.EnableTypeofSearch, "Enable Typeof Search:");

            var attributeNameOption = AddStringOption((TestLinkerSettings x) => x.TypeofAttributeName, "Attribute name:");
            SetIndent(attributeNameOption, indent: 2);
            enableTypeofSearchOption.CheckedProperty.FlowInto(_lifetime, attributeNameOption.GetIsEnabledProperty());
        }

        #endregion
    }
}
