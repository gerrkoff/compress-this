#!/usr/bin/env bash

rm -rf test/build

echo -n 'building...'
dotnet publish -c Release -o test/build --nologo -v q
echo ' done'
echo ''