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
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.UnitTestFramework;

namespace TestLinker.Options
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
