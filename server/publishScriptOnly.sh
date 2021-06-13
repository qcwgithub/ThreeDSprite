#!/bin/sh
PDIR=$1
if [ -z "$PDIR" ]; then
    echo "please specify target folder"
    exit 1
fi

if [ ! -d "$PDIR" ]; then
    echo "'$PDIR' does not exist"
    exit 1
fi

dotnet publish --no-dependencies -c Release -o "$PDIR" ./Script/Script.csproj
