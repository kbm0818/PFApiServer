set DST_C#_DIR=.\\
set SRC_DIR=.\\
protoc -I=%SRC_DIR% --csharp_out=%DST_C#_DIR% %SRC_DIR%Packet.proto
protoc -I=%SRC_DIR% --csharp_out=%DST_C#_DIR% %SRC_DIR%SystemPacket.proto

pause