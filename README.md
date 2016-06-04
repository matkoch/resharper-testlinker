# TestLinker

[![Build](https://img.shields.io/teamcity/codebetter/Testlinker_Ci.svg?label=master&style=flat-square)](http://teamcity.codebetter.com/project.html?projectId=TestLinker)
[![License](https://img.shields.io/badge/license-Apache License 2.0-blue.svg?style=flat-square)](https://github.com/matkoch/TestLinker/blob/master/LICENSE)

TestLinker collects link data between types (i.e., production and test code) based on various [mechanisms](https://github.com/matkoch/TestLinker/blob/master/src/TestLinker/LinkedTypesProvider) and provides various features based on that. For your convenience, TestLinker automatically takes base/derived types into account.

Link data is currently maintained via:
- **Derived names**, as with `Calculator` and `CalculatorTest`. Other supported postfixes are `Tests`, `Spec`, `Specs`, `TestBase`, and `SpecBase`.
- **Usages of SubjectAttribute**, as in `[Subject (typeof (FirstComponent), typeof(SecondComponent)]`, which are applied to test classes. This attribute is especially useful for integration test. If not available, it can be declared in your own codebase.

## Navigation

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/Demo.gif />

- [Goto Related Files](https://www.jetbrains.com/help/resharper/2016.1/Navigation_and_Search__Go_to_Related_Files.html) is extended with navigation points to production/test classes
- New shortcut `ReSharper_GotoLinkedTypes` (assignable via keyboard options) that jumps between linked types. For multiple linked types, a dedicated popmenu is shown, which can also be displayed in [Find Results](https://www.jetbrains.com/help/resharper/2016.1/Reference__Windows__Find_Results_Window.html) window

## Test Execution

- Tests can be executed from their linked production code. This feature automatically integrates with the shortcuts for executing unit tests in *run*, *debug*, *profile*, and *cover* mode.

## Roadmap

- Support for [Continuous Testing](https://blog.jetbrains.com/dotnet/2015/11/19/continuous-testing-in-dotcover-and-resharper-ultimate/) as alternative to initial coverage run
- Create production/test class if it doesn't exist

## Installation

Open ReSharper`s Extension Manager (<kbd>Alt</kbd>+<kbd>R</kbd>,<kbd>X</kbd>) and search for [TestLinker](https://resharper-plugins.jetbrains.com/packages/TestLinker.ReSharper/).

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/ExtensionManager.png />
