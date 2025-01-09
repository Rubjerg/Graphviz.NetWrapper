#!/bin/bash
dotnet test --no-build -p:OutputPath=bin/x64/Release/net8.0 -c Release -f  net8.0 -v d --filter "Category != Flaky & Category != Slow" "$@"

