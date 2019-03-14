@echo off
cd Proto
set server_dest_path="..\..\MCServer\MCServer\Packer"
for %%i in (*.*) do protoc --csharp_out=%server_dest_path% %%i
echo success
pause
