// This is where most of the global stuff stay

using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.Textures;

namespace NezTopDown
{
    public static class GameManager
    {
        public static SpriteAtlas DungeonTiles { get; set; }
        public static SpriteAtlas GardenTiles { get; set; }
        public static List<Sprite> Tiles    { get; set; }
        public static SpriteAtlas WeaponAtlas { get; set; }
        public static Effect HitFlashEffect { get; set; }
        public static Effect LightBlackEffect { get; set; }
        public static List<Weapon> WeaponsList { get; set; }
        public static SpriteAtlas ProjectilesAtlas { get; set; }
        public static BitmapFont DefaultFont { get; set; }
    }

    public enum EntityState
    {
        Free,
        Hit,
        Dead
    }

    public class LayerDepths
    {
        public enum Sorting : int
        {
            Projectiles,
            Weapons,
            WallCorner,
            WallBarrier,
            Player,
            Enemy,
            Chest,
            FrontWall,
            Tile,
        }

        public static float GetLayerDepth(Sorting sorting)
        {
            return (int)sorting / 100f;
        }
    }

    public enum PhysicsLayers : int
    {
        Player = 1,
        Tile,
        Enemy
    }

    public struct Weapon
    {
        public string name { get; set; }
        public int type { get; set; }
        public float hitPoint { get; set; }
        public float range { get; set; }
        public float firerate { get; set; }
        public float speed { get; set; }
        public Sprite sprite { get; set; }
    }
}
