#!/bin/bash
git clean -dfx && nuget restore && msbuild //p:configuration=release
