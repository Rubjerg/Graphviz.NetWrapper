Graphviz.NetWrapper
===================

[![codecov](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper/branch/master/graph/badge.svg)](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper)

## Supported platforms

At the moment, `Rubjerg.Graphviz` ships with a bunch of precompiled Graphviz dlls built for 64 bit Windows.
This library is compatible with .NET Standard 2.0.
The unit tests run against .NET Framework 4.8 and .NET 6.0.
In the future support may be extended to other platforms.

## Contributing

This project aims to provide a thin .NET shell around the Graphviz C libraries,
together with some convenience functionality that helps abstracting away some
of the peculiarities of the Graphviz library and make it easier to integrate in
an application.
Pull request that fall within the scope of this project are welcome.

## Installation

You can either add this library as a nuget package to project, or include the source and add a
project reference.

### Adding as a Nuget package

Add the [Rubjerg.Graphviz nuget package](https://www.nuget.org/packages/Rubjerg.Graphviz/) to
your project.

### Adding the Rubjerg.Graphviz code to your project or solution
1. Make this code available to your own code, e.g. by adding this repository as a git submodule to your own repository.
2. Add the projects Rubjerg.Graphviz and GraphvizWrapper to your solution.
3. To use Rubjerg.Graphviz within a project of yours, simply add a project reference to it.

When building your project, you should now see all the Graphviz binaries show
up in your output folder. If you don't, you might try reordering the projects
in your solution, such that GraphvizWrapper and Rubjerg.Graphviz are at the
top. There is an [outstanding issue for
this](https://github.com/Rubjerg/Graphviz.NetWrapper/issues/36).

## Documentation

For a reference of attributes to instruct Graphviz have a look at
[Node, Edge and Graph Attributes](https://graphviz.gitlab.io/_pages/doc/info/attrs.html).
For more information on the inner workings of the graphviz libraries, consult the various
documents presented at the [Graphviz documentation page](https://graphviz.org/documentation/).

## Tutorial

