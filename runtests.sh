#!/bin/bash
msbuild.exe Rubjerg.Graphviz.sln && find . -wholename './Rubjerg.Graphviz.Test/bin/**/net48/*Test.dll' | xargs ./nunit-console.sh
rm *.log
