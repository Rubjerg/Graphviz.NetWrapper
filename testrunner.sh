#!/bin/bash
packages\\nunit.consolerunner\\3.12.0\\tools\\nunit3-console.exe "$@" --where "cat!=Slow and cat=Flaky"
