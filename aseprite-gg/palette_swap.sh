#!/usr/bin/env zsh

for color in green firtree yellow purple
do
	aseprite -b ./templates/turtle_template.aseprite --palette "./palettes/turtle_palette-$color.aseprite" --save-as "./sprites/turtle-$color.aseprite"
done

for color in green gray blue
do
	aseprite -b ./templates/grunt_template.aseprite --palette "./palettes/grunt_palette-$color.aseprite" --save-as "./sprites/grunt-$color.aseprite"
done

for color in green yellow purple
do
	aseprite -b ./templates/knight_template.aseprite --palette "./palettes/knight_palette-$color.aseprite" --save-as "./sprites/knight-$color.aseprite"
done

for color in purple orange blue
do
	aseprite -b ./templates/stilter_template.aseprite --palette "./palettes/stilter_palette-$color.aseprite" --save-as "./sprites/stilter-$color.aseprite"
done
