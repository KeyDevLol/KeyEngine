@echo off
dotnet publish "../KeyEngine" --output "FreeBSD" --sc true -r freebsd-x64 -p:PublishSingleFile=true -c Release
robocopy "../KeyEngine/Content/Assets" "FreeBSD/Assets" /E /COPY:DAT /R:3 /W:10
robocopy "../KeyEngine/Content/Editor" "FreeBSD/Editor" /E /COPY:DAT /R:3 /W:10
echo Build completed.
pause