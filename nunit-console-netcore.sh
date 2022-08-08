#!/bin/bash
packages\\nunit.consolerunner.netcore\\3.15.2\\tools\\net6.0\\any\\nunit3-console.exe "$@" --where "cat!=Slow and cat!=Flaky"
