# TestLinker

ReSharper recently added support for [Continuous Testing](https://blog.jetbrains.com/dotnet/2015/11/19/continuous-testing-in-dotcover-and-resharper-ultimate/). However, it requires to first execute all tests in order to collect coverage information. This can be quite impractical in large codebases.

TestLinker adds support to provide these information on a different way, namely the `SubjectAttribute`. For example, you can link the class `Calculator` by applying `Subject[typeof(Calculator)]` to an arbitrary class, that involves the `Calculator` class. As soon as you change the subject under test, all related tests will be invalidated, resulting in a new test run.
