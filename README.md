Graphviz.NetWrapper
===================

[![Build status](https://ci.appveyor.com/api/projects/status/4bhyr3dvo6kap9mn?svg=true)](https://ci.appveyor.com/project/Chiel92/graphviz-netwrapper)
[![codecov](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper/branch/master/graph/badge.svg)](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper)

## Installation

### Adding the Rubjerg.Graphviz project to your solution
If you want to add Rubjerg.Graphviz you have to take a few hurdles because msbuild can't deal
easily with unmanaged compilation artifacts.
1. Add to projects Rubjerg.Graphviz and GraphvizWrapper to your solution.
2. Add GraphvizWrapper as build dependency of Rubjerg.Graphviz.
3. For your project that wants to use Rubjerg.Graphviz, add a reference to it, like normal.
4. To the bottom (but within the `<Project>` tags) of your project file (.csproj), add the following build
   step (make sure you use the correct path to the GraphvizWrapper artifacts):
```xml
  <Target Name="AfterBuild">
    <ItemGroup>
      <GraphVizFiles Include="..\Graphviz.NetWrapper\GraphvizWrapper\bin\$(Configuration)\**" />
    </ItemGroup>
    <Copy SourceFiles="@(GraphVizFiles)" DestinationFolder="$(TargetDir)\%(RecursiveDir)" SkipUnchangedFiles="true" />
  </Target>
```
