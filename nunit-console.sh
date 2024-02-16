#!/bin/bash
packages\\nunit.consolerunner\\3.17.0\\tools\\nunit3-console.exe "$@" --trace=Off --where "cat!=Slow and cat!=Flaky"
