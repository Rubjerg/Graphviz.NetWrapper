#!/bin/bash
msbuild && find . -wholename './**/bin/**/*Test.dll' | xargs ./nunit-console.sh
