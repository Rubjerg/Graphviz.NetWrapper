#!/bin/bash
dotnet test --no-build -f net8.0 -v d --filter "Category != Flaky & Category != Slow" "$@"

