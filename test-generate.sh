#!/usr/bin/env bash

echo 'generating big10...'
base64 /dev/urandom | head -c 10000000 > ./test/big10.txt

echo 'generating big100...'
base64 /dev/urandom | head -c 100000000 > ./test/big100.txt

echo 'generating big1000...'
base64 /dev/urandom | head -c 1000000000 > ./test/big1000.txt

echo 'generating big10000...'
base64 /dev/urandom | head -c 10000000000 > ./test/big10000.txt

echo 'generating big20000...'
base64 /dev/urandom | head -c 20000000000 > ./test/big200000.txt
