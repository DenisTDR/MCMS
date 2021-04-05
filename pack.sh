#!/bin/bash

projects=("./MCMS.Base/MCMS.Base" "./MCMS/MCMS" "./MCMS.Auth/MCMS.Auth" "./MCMS.Emailing/MCMS.Emailing" "./MCMS.Common/MCMS.Common" "./MCMS.Files/MCMS.Files")

buildDir=./nuget-build

version=0.0.2

#rm -rf "$buildDir"/*.nupkg "$buildDir"/*.snupkg

for i in "${projects[@]}"; do
  dotnet pack -c Release -o "$buildDir" -p:PackageVersion=$version "$i".csproj
done

