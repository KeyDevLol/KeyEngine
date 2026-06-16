#!/bin/bash
dotnet publish "../KeyEngine" --output "FreeBSD" --sc true -r freebsd-x64 -p:PublishSingleFile=true -c Release
cp -R "../KeyEngine/Content/Assets" "FreeBSD/Assets"
cp -R "../KeyEngine/Content/Editor" "FreeBSD/Editor"
echo "Build completed."