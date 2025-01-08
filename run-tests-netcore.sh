#!/bin/bash
dotnet test --no-build -p:OutputPath=bin/x64/Debug/net8.0 -f  net8.0 -v d --filter "Category != Flaky & Category != Slow" "$@"

