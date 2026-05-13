@echo off
dotnet publish "../KeyEngine" --output "MacOS" --sc true -r osx-arm64 -p:PublishSingleFile=true -c Release
robocopy "../KeyEngine/Content/Assets" "Windows/Assets" /E /COPY:DAT /R:3 /W:10
robocopy "../KeyEngine/Content/Editor" "Windows/Editor" /E /COPY:DAT /R:3 /W:10
echo Build completed.
pause