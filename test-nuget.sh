bash clean-build.sh
dotnet restore Rubjerg.Graphviz.NugetOrgTests.sln
dotnet build Rubjerg.Graphviz.NugetOrgTests.sln --configuration Release --no-restore

shopt -s globstar
ls Rubjerg.Graphviz.NugetOrgTest/bin/**/*

dotnet test --no-build -p:OutputPath=bin/x64/Release/net8.0 -c Release -f net8.0 Rubjerg.Graphviz.NugetOrgTest/Rubjerg.Graphviz.NugetOrgTest.csproj
