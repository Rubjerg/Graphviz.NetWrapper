Graphviz.NetWrapper
===================

[![Build status](https://ci.appveyor.com/api/projects/status/4bhyr3dvo6kap9mn?svg=true)](https://ci.appveyor.com/project/Chiel92/graphviz-netwrapper)
[![codecov](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper/branch/master/graph/badge.svg)](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper)

## Contributing

## Installation

### Adding the Rubjerg.Graphviz code into your solution
If you want to add Rubjerg.Graphviz to your solution you have to take a few hurdles because msbuild can't deal
with unmanaged compilation artifacts.
1. Make this code available to your solution, e.g. by adding this repository as a git submodule to your repository.
2. Add both the projects Rubjerg.Graphviz and GraphvizWrapper to your solution.
3. Add GraphvizWrapper as build dependency of Rubjerg.Graphviz in your solution.
4. For your project that wants to reference Rubjerg.Graphviz, add a reference to it, like with normal project references.
5. To the bottom (but within the `<Project>` tags) of your project file (.csproj), add the following post build
   step (make sure you use the correct path to the GraphvizWrapper artifacts):
```xml
  <Target Name="AfterBuild">
    <ItemGroup>
      <GraphvizFiles Include="..\Graphviz.NetWrapper\GraphvizWrapper\bin\$(Configuration)\**" />
    </ItemGroup>
    <Copy SourceFiles="@(GraphvizFiles)" DestinationFolder="$(TargetDir)\%(RecursiveDir)" SkipUnchangedFiles="true" />
  </Target>
```

When building your project, you should now see all the Graphviz binaries show up in your output
folder.

### Adding the Nuget package to your project

## Documentation

For a reference of attributes to instruct Graphviz have a look at
[Node, Edge and Graph Attributes](https://graphviz.gitlab.io/_pages/doc/info/attrs.html).
For more information on the inner workings of the graphviz libraries, consult the various
documents presented at the [Graphviz documentation page](https://graphviz.org/documentation/).

## Tutorial

