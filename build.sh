#! /bin/bash

pushd ~/source/repos/StreamDeckActionSpike/ >/dev/null

printf "\nRestoring NuGet packages...\n"
dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/nuget

if [ $? -ne 0 ]; then
	exit 1
fi

printf "\nBuilding...\n"
dotnet build

if [ $? -ne 0 ]; then
	exit 1
fi

pushd ./StreamDeckActionSpike.ConsoleApp/ >/dev/null

rm --force --recursive ./bin/Release/net5.0/publish/*

for runtime in linux-musl-x64 osx.10.11-x64 win10-x64
do
	printf "\nPublishing for $runtime...\n"
	dotnet publish --configuration "Debug" --no-self-contained --output "./bin/Release/net5.0/publish/" --runtime "$runtime"
done

if [ $? -ne 0 ]; then
	exit 1
fi

pushd ${APPDATA} >/dev/null
rm --force --recursive ./Elgato/StreamDeck/Plugins/com.bob.helloworld.sdPlugin/*
popd >/dev/null

printf "\nDeploying...\n"
cp --preserve=all --recursive --update --verbose "./bin/Release/net5.0/publish/." "${APPDATA}/Elgato/StreamDeck/Plugins/com.bob.helloworld.sdPlugin/"

popd >/dev/null
popd >/dev/null
