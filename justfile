set shell := ["sh", "-cu"]
is-windows := `uname -s | grep -qi 'mingw\|msys' && echo true || echo false`

# Complete build and test
default: test-all

find-msbuild:
    powershell -NoProfile -Command '& { & "${env:ProgramFiles(x86)}\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe }'
    
# Restore .NET tools
restore-tools:
    dotnet tool restore

# Restore main solution packages
restore: restore-tools
    dotnet restore Rubjerg.Graphviz.sln

# Build main app
build-package: restore
    if {{is-windows}}; then \
        MSBuild.exe Rubjerg.Graphviz.sln //p:Configuration=Release; \
    else \
        dotnet build Rubjerg.Graphviz.sln --configuration Release --no-restore; \
    fi

# Restore test project packages
restore-tests: build-package
    dotnet restore Rubjerg.Graphviz.Tests.sln

# Build test projects
build-tests: restore-tests
    if {{is-windows}}; then \
        MSBuild.exe Rubjerg.Graphviz.Tests.sln //p:Configuration=Release; \
    else \
        dotnet build Rubjerg.Graphviz.Tests.sln --configuration Release --no-restore; \
    fi

# Run unit tests for a project
test PROJECT:
    dotnet test --no-build \
        -p:OutputPath=bin/x64/Release/net8.0 \
        -c Release \
        -f net8.0 \
        -v d \
        --filter "Category != Flaky & Category != Slow" \
        {{PROJECT}}

# Run both test projects
test-all: build-tests
    just test Rubjerg.Graphviz.Test/Rubjerg.Graphviz.Test.csproj
    just test Rubjerg.Graphviz.TransitiveTest/Rubjerg.Graphviz.TransitiveTest.csproj

locate-nupkg GITHUB_OUTPUT:
    echo "package=$(find . -name "Rubjerg.Graphviz.*.nupkg" | head -1)" >> "{{GITHUB_OUTPUT}}"

# Normalize line endings to crlf
crlf:
    bash -c "git ls-files -- ':!GraphvizWrapper/graphvizfiles/*' ':!*.sh' | xargs unix2dos"

# Format the code
format:
    dotnet format whitespace -v diag Rubjerg.Graphviz.sln
    dotnet format whitespace -v diag Rubjerg.Graphviz.Tests.sln

# Check that none of the actions generated a diff
check-diff:
    git diff --exit-code

# Check for unfinished work
check-fixme:
    bash -c "! git grep 'FIX''NOW'"

check-all: crlf format check-diff check-fixme

