#!/bin/bash
dotnet publish "../KeyEngine" --output "MacOS" --sc true -r osx-arm64 -p:PublishSingleFile=true -c Release
cp -R "../KeyEngine/Content/Assets" "MacOS/Assets"
cp -R "../KeyEngine/Content/Editor" "MacOS/Editor"
echo "Build completed."