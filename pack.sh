#!/bin/bash

if [ $# -eq 0 ] || [[ $1 != "publish-"* ]]; then
  echo "Invalid git tag argument supplied. Usage: pack.sh publish-x.y.z"
  exit 1
fi

version=${1#publish-}

projects=("./MCMS.Base/MCMS.Base" "./MCMS/MCMS" "./MCMS.Auth/MCMS.Auth" "./MCMS.Emailing/MCMS.Emailing" "./MCMS.Common/MCMS.Common" "./MCMS.Files/MCMS.Files" "./MCMS.Logging/MCMS.Logging")

buildDir=nuget-build

export VERSION=$version
export ENV_TYPE=CI_BUILD
rm -rf "$buildDir"/*.nupkg "$buildDir"/*.snupkg

mkdir -p $buildDir
dotnet nuget add source "$(pwd)/$buildDir" -n "Temporary build dir" || exit 1

for i in "${projects[@]}"; do
  printf "\n Packing %s...\n" "$i"
  dotnet pack -c Release -o "$buildDir" -p:PackageVersion="$version" "$i".csproj
done

dotnet nuget remove source "Temporary build dir" || exit 1
