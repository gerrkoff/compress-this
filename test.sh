#!/usr/bin/env bash

#FILE="big10.txt"
#FILE="big100.txt"
FILE="big1000.txt"
#FILE="big10000.txt"
#FILE="big20000.txt"

APP="test/build/CompressThis.dll"
ORIGINAL="test/$FILE"
COMPRESSED="test/$FILE.gz"
DECOMPRESSED="test/$FILE.d"

./test-build.sh || exit $?

SECONDS=0
dotnet "$APP" compress "$ORIGINAL" "$COMPRESSED" "$1" "$2"
DURATION_COMPRESS=$SECONDS

SECONDS=0
dotnet "$APP" decompress "$COMPRESSED" "$DECOMPRESSED" "$1" "$2"
DURATION_DECOMPRESS=$SECONDS

echo "    ORIGINAL: $(wc -c < "$ORIGINAL")"
echo "  COMPRESSED: $(wc -c < "$COMPRESSED")"
echo "DECOMPRESSED: $(wc -c < "$DECOMPRESSED")"

diff -q "$ORIGINAL" "$DECOMPRESSED" || exit $?

echo "               SUCCESS"

echo "  COMPRESS TIME $((DURATION_COMPRESS / 60))m $((DURATION_COMPRESS % 60))s"
echo "DECOMPRESS TIME $((DURATION_DECOMPRESS / 60))m $((DURATION_DECOMPRESS % 60))s"