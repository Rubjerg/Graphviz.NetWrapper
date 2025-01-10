echo Generating README.md
cat README-src.md > README.md
printf '```cs\n' >> README.md
cat Rubjerg.Graphviz.Test/Tutorial.cs >> README.md
printf '```' >> README.md
