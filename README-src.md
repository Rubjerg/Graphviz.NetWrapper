Graphviz.NetWrapper
===================

## Supported platforms

`Rubjerg.Graphviz` ships with precompiled Graphviz binaries to ensure the exact graphviz version is used that we have tested and to make deployment more predictable.
The current version we ship is Graphviz 11.0.0.
This library is compatible with .NET Standard 2.0, but the nuget package only supports .NET5.0 and higher.
The unit tests run on windows and linux against .NET Framework .NET 8.0.

### Windows
`Rubjerg.Graphviz` ships with the necessary Graphviz binaries and dependencies built for 64 bit Windows.

### Linux
In the same vein as our windows build, we ship Graphviz binaries to make sure that this library is deployed with the same binaries as we've tested it.
However, we do not ship all the dependencies of Graphviz.
You will have to make sure these are available on your system, if you need them.
[Here is a list of all the graphviz dependencies.](https://packages.fedoraproject.org/pkgs/graphviz/graphviz/fedora-rawhide.html#dependencies)
In practice you may not need all of those.
In particular, if you only want to read graphs and run the DOT layout algorithm, libtool-ldtl, libc and libz are enough.
To run our tests successfully you will also need libgts and libpcre2 (for the neato algorithm).
For more details, check the dependencies of any graphviz binaries with `ldd`.

It is currently not possible to use this package on linux in a non-portable application, i.e. an application that targets linux-x64.
The reason for this is that [dotnet flattens the directory structure of native dependencies when targetting a single runtime](https://github.com/dotnet/sdk/issues/9643), and this breaks graphviz.

## Installation

Add the [Rubjerg.Graphviz nuget package](https://www.nuget.org/packages/Rubjerg.Graphviz/) to your project.

To run the code from this library on windows, you must have the Microsoft Visual C++ Redistributable (2015-2022) installed, which provides the required runtime libraries.
You can download it from the [official Microsoft website](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist).

## Contributing

This project aims to provide a thin .NET shell around the Graphviz C libraries,
together with some convenience functionality that helps abstracting away some
of the peculiarities of the Graphviz library and make it easier to integrate in
an application.
Pull request that fall within the scope of this project are welcome.

## Documentation

For a reference of attributes to instruct Graphviz have a look at
[Node, Edge and Graph Attributes](https://graphviz.gitlab.io/_pages/doc/info/attrs.html).
For more information on the inner workings of the graphviz libraries, consult the various
documents presented at the [Graphviz documentation page](https://graphviz.org/documentation/).

## Tutorial

