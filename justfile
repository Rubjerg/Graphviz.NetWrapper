set shell := ["sh", "-cu"]
is-windows := `uname -s | grep -qi 'mingw\|msys' && echo true || echo false`

# Complete build and test
default: test-all

find-msbuild:
    powershell -NoProfile -Command '& { & "${env:ProgramFiles(x86)}\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe }'

# Restore .NET tools and main solution packages
restore:
    dotnet tool restore
    dotnet restore Rubjerg.Graphviz.sln

build SOLUTION:
    if {{is-windows}}; then \
        MSBuild.exe {{SOLUTION}} //p:Configuration=Release; \
    else \
        dotnet build {{SOLUTION}} --configuration Release --no-restore; \
    fi

build-rid SOLUTION RID:
    dotnet build {{SOLUTION}} --configuration Release --no-restore -r {{RID}};

# Build nuget package
build-package: restore
    just build Rubjerg.Graphviz.sln

# Restore test project packages
restore-tests: build-package
    dotnet restore Rubjerg.Graphviz.Tests.sln

# Build test projects
build-tests: restore-tests
    just build Rubjerg.Graphviz.Tests.sln

# Run unit tests for a project
test PROJECT OUTPUT_PATH='bin/x64/Release/net8.0/':
    dotnet test --no-build \
        -p:OutputPath={{OUTPUT_PATH}} \
        -c Release \
        -f net8.0 \
        -v d \
        --filter "Category != Flaky & Category != Slow" \
        {{PROJECT}}

# Run both test projects
test-all: build-tests
    just test Rubjerg.Graphviz.Test/Rubjerg.Graphviz.Test.csproj
    just test Rubjerg.Graphviz.TransitiveTest/Rubjerg.Graphviz.TransitiveTest.csproj

# Run the nuget.org tests
test-nugetorg:
    dotnet tool restore
    dotnet restore NugetOrgTests/Rubjerg.Graphviz.NugetOrgTests.sln
    just build NugetOrgTests/Rubjerg.Graphviz.NugetOrgTests.sln
    just test NugetOrgTests/Rubjerg.Graphviz.NugetOrgTest/Rubjerg.Graphviz.NugetOrgTest.csproj
    just test NugetOrgTests/Rubjerg.Graphviz.NugetOrgTransitiveTest/Rubjerg.Graphviz.NugetOrgTransitiveTest.csproj

# Build and test nonportable win-x64 deployment
test-win-x64: restore-tests
    dotnet build Rubjerg.Graphviz.Test/Rubjerg.Graphviz.Test.csproj --configuration Release --no-restore -r win-x64;
    dotnet build Rubjerg.Graphviz.TransitiveTest/Rubjerg.Graphviz.TransitiveTest.csproj --configuration Release --no-restore -r win-x64;
    just test Rubjerg.Graphviz.Test/Rubjerg.Graphviz.Test.csproj bin/Release/net8.0/win-x64/
    just test Rubjerg.Graphviz.TransitiveTest/Rubjerg.Graphviz.TransitiveTest.csproj bin/Release/net8.0/win-x64/

locate-nupkg GITHUB_OUTPUT:
    echo "package=$(find . -name "Rubjerg.Graphviz.*.nupkg" | head -1)" >> "{{GITHUB_OUTPUT}}"

generate-readme:
    echo Generating README.md
    cat README-src.md > README.md
    printf '```cs \r\n' >> README.md
    cat Rubjerg.Graphviz.Test/Tutorial.cs >> README.md
    printf '```\r\n' >> README.md

# Strip trailing spaces and normalize line endings to crlf
normalize:
    bash -c "git ls-files -- ':!GraphvizWrapper/*' | xargs sed -i -b 's/[ \t]*$//' "
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

check-tutorial-width:
    awk 'length($0) > 100 { print FILENAME ":" NR ": " $0; found=1 } END { exit found }' Rubjerg.Graphviz.Test/Tutorial.cs

check-all: generate-readme normalize format check-diff check-fixme check-tutorial-width

