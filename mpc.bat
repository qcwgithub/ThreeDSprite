REM ECHO OFF
mpc.exe -i ./Assembly-CSharp.csproj -o ./Assets/Scripts/Gen/mpc.cs
mpc.exe -i ./server/Data/Data.csproj -o ./server/Script/Common/Gen/mpc.cs

dotnet "run" "--project" ./server/CodeGen/CodeGen.csproj ./Assets/Scripts/Gen/mpc.cs ./Assets/Scripts/Gen/MessageCode.cs ./Assets/Scripts/Gen/BinaryMessagePackerGen.cs ./server/Script/Common/Gen/mpc.cs ./server/Data/Common/Gen/MessageCode.cs ./server/Script/Common/Gen/BinaryMessagePackerGen.cs