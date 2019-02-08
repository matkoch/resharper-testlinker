# TestLinker

[![Build](https://img.shields.io/teamcity/codebetter/Testlinker_CompileFxCopInspectCode.svg?label=master&style=flat-square&logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAEEAAAA%2FCAYAAAC%2F36X0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTCtCgrAAAAC%2B0lEQVR4Xu2aDZHCMBSETwISkIAEJCABCUjAARKQgAQkIAEJSMhl75KbtOzLC%2FDyhpuwMx%2B0e7RNtvmj3FcIYXio%2BR9YrVZhvV7%2Fgf35Z1r5ednv98EbFLwsSI3FYhG22204Ho%2Fher1GS9btdgvn8znsdruwXC5h0XOW5A13IYioSWHmoBKo%2BCtCIFrgecNdtRBw5w%2BHQ9y0E8KQWkbecJcUAvr25XKJm%2FZCV0G3ippcM2%2B4i4WAAFDQ3kqt7O%2B6ecNd8xC8AoBwnbJrvEUIKJBXANBms8FbrvtvCBg0WtD6KirCjmOUfRP7rcI10JxRkXKdkKdQLczamNBEmmpEpcrQYyVSoVShcrUZJYOZBZ9jYczHgsydUaNHCNriB0KFHl0RIoyyhaX1Bv0sNSWsQ2hpBc8EUIK7n7ox%2FTugpoR1COWdkpSuSY%2B3gpoSliGguWqqNWFLqClhGUKapqp6pRs8AjUlLENII70orR9bQk0JyxC08UCaznpATQnPEPA8IIoeaw01JTxDSNeix1pDTYlPCBHPEFqWyFZQU8IzBK81AqCmhGUI2hSZvlPQY62hpoRlCC2LJa9xgZoSliG0LJsfOd8rUFPCMgRwOp3iW13sIcijaC2KmhLWIfT%2BKo3WloOuhUlNCesQQOtDFe1uzkFw88eBUhB3Ro0eIbQMkFmYNrWf1rRfrVgQkx2NHiGAlrGhFK6DKRYVQpnwPQP7rT%2FazIOYFEajVwjou60VsFLZve4KVKNXCAB9mD0h7qH5anRSEI2eIQAE0TJQviK2HJ%2FsaPQOAcwflVsJrextZwcJFNiqe6BctVmFmhKeIQC0CoTx7KCJWSeVmZ4%2FQ00JFAonlej5dBh3ElMh%2BjTCLscOtBh4AFMl1h4oaxQ91xxqjgY1R4Oao0HN0aDmaFBzNKg5GtQcDWqOBjVHg5qjQc3RoOZoUHM0qDka1BwNao7Gzwt7StQb7ZckT%2FKGuzz%2FHUcjb7jrE0LUJ4SoTwhRbxcCG717846zw8CEr2%2Fmetn3QDyWVAAAAABJRU5ErkJggg%3D%3D)](http://teamcity.codebetter.com/project.html?projectId=TestLinker)
[![License](https://img.shields.io/github/license/matkoch/testlinker.svg?style=flat-square&logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAHYcAAB2HAY%2Fl8WUAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTCtCgrAAAADB0lEQVR4XtWagXETMRREUwIlUAIlUAodQAl0AJ1AB9BB6AA6gA6MduKbkX%2BevKecNk525jHO3l%2Fp686xlJC70%2Bl0C942vjV%2Bn9FreVQbBc0wWujfRpW8Z78JaIb53hhJ1ygTA80w9PQ36duBMjHQHPCuoQZfutSjeqU1PAJN4E3j2pN7aVKv6pnWcgGawNfGa5N6prVcgGZBn8yvVXZXQbOgPXokXaPMNZwoc41D%2FaHZ8b7hpBrKjnCizIjD%2FaHZ8aPR6%2BeZXqqh7Agnyow43B%2BaZz40qnQ36a6rlsYgnChDLOkPzTN1z%2B9PafU0N3OAcaIMsaQ%2FNBufG1X9JyrtDMr0Y4xwokxlWX%2BPjAYdemhPrWeDvYcPJ8r0LO3v4oszNfivQQuTp2u9qJGKE2V6lvZ38UVj9q3t3oqEE2U2lvfXF4t6qPjTqDUV1fRyhw8nymws768vfOr2NtqOqFY4UUZE%2BusL6VDRX7%2FGzOHDiTIi0t9WMPsUKzNPx4kysf62gmuHir3sPXw4USbWny485ZOc2PsJ7VTro%2F3pwp5DxV7qHq2xa41TrY%2F2J7PfJkaHir3UwwdtU061PtqfTP0CUaYm2v3LxCtoDI2lMWk8p1of7Y8K0jhRJgaaYZwoE0P%2FpFUndZqtP6T4BE2zC5qtP6T4BE2zC5qtPyRN8OvhZUQae3ZBtT7anyb49PA6Ivp5wKnWR%2FvbJkncZXr6wokysf62CXRCWjmJxhqd2JwoE%2BuvTqS37JGJlB39GLzhRJmN5f31gz8XTpSJgWYYJ8rEQDOME2VioBnGiTIx0AzjRJkYaIZxokwMNMM4USYGmmGcKBMDzTBOlImBZhgnysRAM4wTZWKgGcaJMjHQDONEmRhohnGiTAw0wzhRJgaaYZwoEwPNME6UiYFmGCfKxEAzjBNlYqAZxokyMdAMoL%2FO%2BNi4bzjpT1e%2BNFb8V7gFzUXMLHqk%2BM1A8wArFj1S5GagOUly0SMtuxloTnJrUU%2B7QXOSW4t62g2ak9xa1NNu0Jzk1qKednK6%2Bw9roIB8keT%2F3QAAAABJRU5ErkJggg%3D%3D)](https://github.com/matkoch/TestLinker/blob/master/LICENSE)

TestLinker collects link data between types (i.e., production and test code) based on various [mechanisms](https://github.com/matkoch/TestLinker/blob/master/src/TestLinker/LinkedTypesProvider) and provides various features based on that. For your convenience, TestLinker automatically takes base/derived types into account when meaningful.

## Navigation

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/Demon_Navigate.gif />

- [Goto Related Files](https://www.jetbrains.com/help/resharper/2016.1/Navigation_and_Search__Go_to_Related_Files.html) is extended with navigation points to production/test classes.
- New shortcuts `ReSharper_GotoAllLinkedTypes` and `ReSharper_GotoLinkedTypesWithDerivedName` (assignable via keyboard options) that jumps between linked types. In case of multiple linked types, a dedicated popmenu is shown, which can also be displayed in [Find Results](https://www.jetbrains.com/help/resharper/2016.1/Reference__Windows__Find_Results_Window.html) window.

## Test Creation

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/Demo_Create.gif />

- Create production/test class if they don't exist
- Requires at least one matching pair of test and production class in the project

## Test Execution

- Tests can be executed from their linked production code. This feature automatically integrates with the shortcuts for executing unit tests in *run*, *debug*, *profile*, and *cover* mode.

## Configuration

Link data is currently maintained via:
- **Derived names**, as with `Calculator` and `CalculatorTest`. Pre-/Postfixes can be configured in the options page.
- **Usages of TypeofAttributes**, as in `[Subject (typeof (FirstComponent), typeof(SecondComponent)]`, which are applied to test classes. This custom attribute is especially useful for integration test and can be configured through the options page.

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/OptionsPage.png width=600px />

## Installation

Open ReSharper`s Extension Manager (<kbd>Alt</kbd>+<kbd>R</kbd>,<kbd>X</kbd>) and search for [TestLinker](https://resharper-plugins.jetbrains.com/packages/ReSharper.TestLinker/).

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/ExtensionManager.png />
