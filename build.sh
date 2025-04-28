# FIXNOW
git clean -dfx
dotnet tool restore
dotnet restore Rubjerg.Graphviz.sln
dotnet build Rubjerg.Graphviz.sln --configuration Release --no-restore
dotnet build Rubjerg.Graphviz.sln --configuration Release --no-restore
ls Rubjerg.Graphviz/bin/**/*
