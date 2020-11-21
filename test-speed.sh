#!/usr/bin/env bash

FILE="big1000.txt"

APP="test/build/CompressThis.dll"
ORIGINAL="test/$FILE"
COMPRESSED="test/$FILE.gz"

./test-build.sh || exit $?

DURATION=0
X=1
while [ $X -le 5 ]
do
  SECONDS=0
  echo "=== Iteration $X"
  dotnet "$APP" compress "$ORIGINAL" "$COMPRESSED" "$1" "$2"
  DURATION=$(( DURATION + SECONDS ))
  X=$(( X + 1 ))
done

X=$(( X - 1 ))

echo "TIME $((DURATION / 60))m $((DURATION % 60))s | AVG $((DURATION / X)).$((DURATION % X))/$((X))s "