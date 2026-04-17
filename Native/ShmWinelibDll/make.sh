#!/bin/sh
winegcc -shared -o lincomm.dll lincomm.c lincomm.spec
mv lincomm.dll.so lincomm.dll
