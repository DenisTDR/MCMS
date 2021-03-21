#!/bin/bash

projects=("./MCMS.Base/MCMS.Base" "./MCMS/MCMS" "./MCMS.Auth/MCMS.Auth" "./MCMS.Emailing/MCMS.Emailing" "./MCMS.Common/MCMS.Common" "./MCMS.Files/MCMS.Files")

buildDir=../../nuget-build

rm -rf "$buildDir"/*.nupkg "$buildDir"/*.snupkg

for i in "${projects[@]}"; do
  dotnet pack -c Release -o "$buildDir" "$i".csproj
done

