# How to contribute
Contributions are essential for keeping NodaMoney great. It's impossible for us to be experts in every currency in each
country, so every help is appreciated. If you have a problem, a great idea or found a possible bug in NodaMoney, please
report this to us as an [Issue](https://github.com/remyvd/NodaMoney/issues).

Do you want to contribute with code changes, there are a few guidelines that we need contributors to follow so that your
change can be accepted quickly.

## Getting Started
* Make sure you have a [GitHub account](https://github.com/signup/free)
* Fork the repository on GitHub (we use the [fork & pull model](https://help.github.com/articles/using-pull-requests))
* We use as branching strategy the [GitHub Flow Workflow](https://guides.github.com/introduction/flow/).

## Making Changes
* If possible, create an issue for big improvements or features, so that people can discuss.
* Make commits of logical units, so that we can pick and choose.
* Make sure your commit messages have a good description.
* Complies with StyleCop rules and Code Analysis (runs on Release configuration).
* Complies with Coding Guidelines (see document in solution).
* Make sure you have added the necessary tests for your changes.
* Run _all_ the tests to assure nothing else was accidentally broken.
* Push your changes to your fork of the repository.
* Submit a [pull request](https://help.github.com/articles/creating-a-pull-request/) to the NodaMoney repository.
* If needed we will give feedback about the pull request or accept it right away.

## Coding guidelines
There are enough guidelines on the web, so to create or own would be a waste less undertaking and only done for our own ego.

The [Microsoft Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/?redirectedfrom=MSDN)
is a fairly thorough document of how to write managed code and should be used as a starting point.

The [C# Coding Guidelines](https://csharpcodingguidelines.com/) is a good additional document that focuses on the
C# language and its best practices.

[Code quality rules](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/) are being
[analyzed automatically](https://learn.microsoft.com/en-us/visualstudio/code-quality/roslyn-analyzers-overview?view=vs-2022)
as part of the build. We also added [Roslyn Analyzers](https://github.com/dotnet/roslyn-analyzers) to enforce an additional set of style and consistency rules.
