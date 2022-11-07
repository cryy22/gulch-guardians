#!/usr/bin/env zsh

for asepriteFile in $(find sprites -name "*.aseprite") 
do
	imageName=$(echo "$asepriteFile" | sed 's/.*\/\([^\/]*\)-\([a-zA-Z]*\)\.aseprite/\1/') 
	imageColor=$(echo "$asepriteFile" | sed 's/.*\/\([^\/]*\)-\([a-zA-Z]*\)\.aseprite/\2/') 

	exportDir="exports/$imageName/$imageColor"
	mkdir -p $exportDir
	aseprite -b $asepriteFile --save-as "$exportDir/sprite0001.png"
done
