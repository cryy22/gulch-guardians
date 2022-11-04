#!/usr/bin/env bash

aseprite -b $(find sprites -name "*.aseprite" -print0 | sort -z) --sheet spritesheets/sheet.png
