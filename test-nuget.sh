bash clean-build.sh
dotnet restore Rubjerg.Graphviz.NugetTests.sln
dotnet build Rubjerg.Graphviz.NugetTests.sln --configuration Release --no-restore

shopt -s globstar
ls Rubjerg.Graphviz.NugetTest/bin/**/*

dotnet test --no-build -p:OutputPath=bin/x64/Release/net8.0 -c Release -f net8.0 Rubjerg.Graphviz.NugetTest/Rubjerg.Graphviz.NugetTest.csproj
