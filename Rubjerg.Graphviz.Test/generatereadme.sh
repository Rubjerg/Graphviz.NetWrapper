echo Generating README.md
cat ../README-src.md > ../README.md
printf '```cs\n' >> ../README.md
cat Tutorial.cs >> ../README.md
printf '```' >> ../README.md
