#!/bin/bash
dotnet test  -p:Platform=x64 --no-build -c Release -f net8.0 -v d --filter "Category != Flaky & Category != Slow" "$@"
