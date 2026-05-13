@echo off
dotnet publish "../KeyEngine" --output "Linux" --sc true -r linux-x64 -p:PublishSingleFile=true -c Release
robocopy "../KeyEngine/Content/Assets" "Linux/Assets" /E /COPY:DAT /R:3 /W:10
robocopy "../KeyEngine/Content/Editor" "Linux/Editor" /E /COPY:DAT /R:3 /W:10
echo Build completed.
pause