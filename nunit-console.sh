#!/bin/bash
packages\\nunit.consolerunner\\3.18.3\\tools\\nunit3-console.exe "$@" --trace=Off --where "cat!=Slow and cat!=Flaky"
