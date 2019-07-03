// Copyright Matthias Koch 2017.
// Distributed under the MIT License.
// https://github.com/matkoch/Nuke/blob/master/LICENSE

using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Rider.Model.UIAutomation;
using ReSharperPlugin.TestLinker.TestLinkerIcons;

namespace ReSharperPlugin.TestLinker.Options
{
    [OptionsPage(Id, PageTitle, typeof(TestLinkerThemedIcons.TestLinker),
        ParentId = "General",
        NestingType = OptionPageNestingType.Inline,
        IsAlignedWithParent = true,
        Sequence = 0.1d)]
    public class TestLinkerOptionsPage : BeSimpleOptionsPage
    {
        private const string Id = nameof(TestLinkerOptionsPage);
        private const string PageTitle = "Test Linker";

        private readonly Lifetime _lifetime;

        public TestLinkerOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            _lifetime = lifetime;

            AddSuffixSearchOptions();
            AddTypeofSearchOptions();

            AddText("");
            AddText("Warning: After changing these settings, cleaning the solution cache (see \"General\" options page)");
            AddText("is necessary to update already analyzed code.");
        }

        private void AddSuffixSearchOptions ()
        {
            var enableSuffixSearchOption = (BeCheckbox) AddBoolOption((TestLinkerSettings x) => x.EnableSuffixSearch, "Enable Suffix Search:");

            using (Indent())
            {
                var testSuffixOption = AddTextBox((TestLinkerSettings x) => x.NamingSuffixes, "Naming Suffixes for Tests (comma-separated):");
                var namingStyleOption = AddComboEnum((TestLinkerSettings x) => x.NamingStyle, "Naming Style:");

                enableSuffixSearchOption.Property.FlowIntoRd(_lifetime, s => s.Value, testSuffixOption.Enabled);
                enableSuffixSearchOption.Property.FlowIntoRd(_lifetime, s => s.Value, namingStyleOption.Enabled);
            }
        }

        private void AddTypeofSearchOptions ()
        {
            var enableTypeofSearchOption = (BeCheckbox) AddBoolOption((TestLinkerSettings x) => x.EnableTypeofSearch, "Enable Typeof Search:");

            using (Indent())
            {
                var attributeNameOption = AddTextBox((TestLinkerSettings x) => x.TypeofAttributeName, "Attribute name:");
                enableTypeofSearchOption.Property.FlowIntoRd(_lifetime, s => s.Value, attributeNameOption.Enabled);
            }
        }

        private BeTextBox AddTextBox<TKeyClass>(Expression<Func<TKeyClass, string>> lambdaExpression, string description)
        {
            var property = new Property<string>(description);
            OptionsSettingsSmartContext.SetBinding(_lifetime, lambdaExpression, property);
            var control = property.GetBeTextBox(_lifetime);
            AddControl(control.WithDescription(description, _lifetime));
            return control;
        }
    }
}
