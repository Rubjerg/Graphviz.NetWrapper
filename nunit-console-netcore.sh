#!/bin/bash
dotnet tool run nunit "$@" --trace=Off --where "cat!=Slow and cat!=Flaky"
