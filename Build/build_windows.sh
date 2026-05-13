#!/bin/bash
dotnet publish "../KeyEngine" --output "Windows" --sc true -r win-x64 -p:PublishSingleFile=true -c Release
cp -R "../KeyEngine/Content/Assets" "Windows/Assets"
cp -R "../KeyEngine/Content/Editor" "Windows/Editor"
echo "Build completed."