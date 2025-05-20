set shell := ["sh", "-cu"]
is-windows := `uname -s | grep -qi 'mingw\|msys' && echo true || echo false`

# Full pipeline
default: restore-tools test-all

find-msbuild:
    powershell -NoProfile -Command '& { & "${env:ProgramFiles(x86)}\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe }'
    
# Restore .NET tools
restore-tools:
    dotnet tool restore

# Restore main solution packages
restore:
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

locate-nupkg GITHUB_OUTPUT: test-all
    echo "package=$(find . -name "Rubjerg.Graphviz.*.nupkg" | head -1)" >> {{GITHUB_OUTPUT}}

# Check for CRLF line endings (only reports changes)
check-line-endings:
    bash -c "git ls-files -- ':!GraphvizWrapper/graphvizfiles/*' ':!*.sh' | xargs unix2dos"
    git diff --exit-code

# Check if README.md is up to date
check-readme:
    git diff --exit-code -- README.md

# Check formatting in main solution
check-format-main: restore-tools
    dotnet format whitespace --verify-no-changes -v diag Rubjerg.Graphviz.sln

# Check formatting in test solution
check-format-tests: restore-tools
    dotnet format whitespace --verify-no-changes -v diag Rubjerg.Graphviz.Tests.sln

# Check for leftover tags like FIX'NOW
check-fixme:
    bash -c "! git grep 'FIX''NOW'"

check-all: check-line-endings check-readme check-format-main check-format-tests check-fixme

