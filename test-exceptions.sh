#!/usr/bin/env bash

./test-build.sh || exit $?

APP="test/build/CompressThis.dll"
FILE_10="test/big10.txt"
FILE_20000="test/big20000.txt"

echo '=== case 0'
dotnet "$APP"

echo '=== case 1'
dotnet "$APP" compress "$FILE_20000" "$FILE_20000.gz" --block-size 1

echo '=== case 2'
dotnet "$APP" decompress "$FILE_10" "$FILE_10.d" -s

echo '=== case 3'
dotnet "$APP" decompress "$FILE_10" "$FILE_10.d"

echo '=== case 4'
dotnet "$APP" compress "$FILE_10" "$FILE_10.gz" -s
dotnet "$APP" decompress "$FILE_10.gz" "$FILE_10.d"