#!/bin/bash
packages\\nunit.consolerunner\\3.15.2\\tools\\nunit3-console.exe "$@" --trace=Off --where "cat!=Slow and cat!=Flaky"
