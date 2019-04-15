@echo off
cd Proto
set server_dest_path="..\..\Assets\MagiCloud\NetWorks\Scripts\Protobuf"
for %%i in (*.*) do protoc --csharp_out=%server_dest_path% %%i
echo success
pause
