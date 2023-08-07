echo off
echo Cleaning up old SpriteAtlases for everything to be updated

del %cd%\Sprites\Player\Player.png
del %cd%\Sprites\Player\Player.atlas
del %cd%\Sprites\Weapons.png
del %cd%\Sprites\Weapons.atlas
del %cd%\Sprites\Enemy\Goblin\Goblin.png
del %cd%\Sprites\Enemy\Goblin\Goblin.atlas
del %cd%\Sprites\Tiles\Garden.png
del %cd%\Sprites\Tiles\Garden.atlas
del %cd%\Sprites\Tiles\Dungeon.png
del %cd%\Sprites\Tiles\Dungeon.atlas
del %cd%\Sprites\Projectiles\Projectiles.png
del %cd%\Sprites\Projectiles\Projectiles.atlas

echo Creating SpriteAtlases

SpriteAtlasPacker.exe -image:Sprites/Player/Player.png -map:Sprites/Player/Player.atlas -fps:9 Sprites/Player
SpriteAtlasPacker.exe -image:Sprites/Weapons.png -map:Sprites/Weapons.atlas Sprites/Weapons
SpriteAtlasPacker.exe -image:Sprites/Enemy/Goblin/Goblin.png -map:Sprites/Enemy/Goblin/Goblin.atlas -fps:9 Sprites/Enemy/Goblin
SpriteAtlasPacker.exe -image:Sprites/Tiles/Garden.png -map:Sprites/Tiles/Garden.atlas Sprites/Tiles/Garden
SpriteAtlasPacker.exe -image:Sprites/Tiles/Dungeon.png -map:Sprites/Tiles/Dungeon.atlas Sprites/Tiles/Dungeon
SpriteAtlasPacker.exe -image:Sprites/Projectiles/Projectiles.png -map:Sprites/Projectiles/Projectiles.atlas -fps:9 Sprites/Projectiles/

echo Done!!!! :)

timeout /t 5