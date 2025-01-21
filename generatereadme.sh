echo Generating README.md
cat README-src.md > README.md
printf '```cs \r\n' >> README.md
cat Rubjerg.Graphviz.Test/Tutorial.cs >> README.md
printf '```\r\n' >> README.md
