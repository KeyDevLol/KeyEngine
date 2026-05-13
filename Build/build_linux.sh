#!/bin/bash
dotnet publish "../KeyEngine" --output "Linux" --sc true -r linux-x64 -p:PublishSingleFile=true -c Release
cp -R "../KeyEngine/Content/Assets" "Linux/Assets"
cp -R "../KeyEngine/Content/Editor" "Linux/Editor"
echo "Build completed."