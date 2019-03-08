@echo off
cd Proto
set client_dest_path="..\..\UnityProject\Assets\Network\Protobuf"
set server_dest_path="..\..\MCServer\MCServer\Packer"
set controller_dest_path="..\..\UnityController\Assets\Network\Protobuf"
for %%i in (*.*) do protoc --csharp_out=%client_dest_path% %%i
for %%i in (*.*) do protoc --csharp_out=%server_dest_path% %%i
for %%i in (*.*) do protoc --csharp_out=%controller_dest_path% %%i
echo success
pause
