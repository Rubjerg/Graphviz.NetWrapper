#!/bin/bash
git clean -dfx && nuget restore Rubjerg.Graphviz.sln && C:\\Program\ Files\\Microsoft\ Visual\ Studio\\2022\\Professional\\Msbuild\\Current\\Bin\\MSBuild.exe //p:configuration=release Rubjerg.Graphviz.sln
