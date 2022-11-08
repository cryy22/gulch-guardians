#!/usr/bin/env zsh

for color in green sage yellow purple
do
	aseprite -b ./templates/TEMPLATE-gg-player.aseprite --palette "./palettes/gg-palette-player_$color.aseprite" --save-as "./sprites/gg-player-$color.aseprite"
done

for color in green beige blue
do
	aseprite -b ./templates/TEMPLATE-gg-enemy_grunt.aseprite --palette "./palettes/gg-palette-enemy_grunt_$color.aseprite" --save-as "./sprites/gg-enemy_grunt-$color.aseprite"
done

for color in green yellow purple
do
	aseprite -b ./templates/TEMPLATE-gg-enemy_knight.aseprite --palette "./palettes/gg-palette-enemy_knight_$color.aseprite" --save-as "./sprites/gg-enemy_knight-$color.aseprite"
done

for color in yellow
do
	aseprite -b ./templates/TEMPLATE-gg-enemy_knight_boss.aseprite --palette "./palettes/gg-palette-enemy_knight_boss_$color.aseprite" --save-as "./sprites/gg-enemy_knight_boss-$color.aseprite"
done

for color in purple yellow blue
do
	aseprite -b ./templates/TEMPLATE-gg-enemy_stilter.aseprite --palette "./palettes/gg-palette-enemy_stilter_$color.aseprite" --save-as "./sprites/gg-enemy_stilter-$color.aseprite"
done
