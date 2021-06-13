#!/bin/sh
PDIR=$1
if [ -z "$PDIR" ]; then
    echo "please specify target folder"
    exit 1
fi

if [ -d "$PDIR" ]; then
    echo "'$PDIR' already exists"
	exit 1
fi

mkdir "$PDIR"
dotnet publish -c Release -o "$PDIR"
cp -f -r ./config "$PDIR/config"
cp -f -r ./Purposes "$PDIR/Purposes"
cp -f -r ./input "$PDIR/input"