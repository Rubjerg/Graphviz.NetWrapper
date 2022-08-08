#!/bin/bash
dotnet tool run nunit "$@" --where "cat!=Slow and cat!=Flaky"
