using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Svg;
using Nez.Textures;
using System;
using System.Collections.Generic;
using static NezTopDown.GameManager;

namespace NezTopDown.Components.LevelGen
{
    public static class Levels
    {
        public enum Sides : int
        {
            TopLeft,
            Top,
            TopRight,
            MidLeft,
            Mid,
            MidRight,
            BottomLeft,
            Bottom,
            BottomRight
        }

        // extra sprite for walls
        public enum WallCorners : int
        {
            InnerCornerBottomLeft = 9, InnerCornerBottomRight, InnerCornerTopLeft, InnerCornerTopRight,
        }

        public static Sprite[] Walls = new Sprite[13];
        public static Sprite[] Floors = new Sprite[13];

        static Scene currentScene;
        static Entity parentEntity;
        static float scale;

        public static void SetupDungeon(LevelGenerator level)
        {
            currentScene = level.Entity.Scene;
            parentEntity = level.Entity;
            scale = level.Scale;

            DungeonTiles = currentScene.Content.LoadSpriteAtlas("Content/Sprites/Tiles/Dungeon.atlas");

            foreach (Sides side in Enum.GetValues(typeof(Sides))) 
            {
                Walls[(int)side] = DungeonTiles.GetSprite(side.ToString() + "Wall");
            }

            foreach (WallCorners corner in Enum.GetValues(typeof(WallCorners)))
            {
                Walls[(int)corner] = DungeonTiles.GetSprite(corner.ToString() + "Wall");
            }

            foreach (Sides side in Enum.GetValues(typeof(Sides)))
            {
                Floors[(int)side] = DungeonTiles.GetSprite(side.ToString() + "Floor");
            }
        }

        public static void SpawnAllFloorTiles(IEnumerable<Vector2> positions)
        {
            foreach (Vector2 position in positions)
            {
                SpawnFloorTile(position);
            }
        }

        public static void SpawnFloorTile(Vector2 position)
        {
            // Todo: Variation
            var entity = currentScene.CreateEntity("tile");
            entity.SetParent(parentEntity);
            entity.Position = position * 16 * scale;
            entity.Scale = new Vector2(scale);
            var sprite = entity.AddComponent(new SpriteRenderer(Floors[(int)Sides.Mid]));
            sprite.LayerDepth = LayerDepths.GetLayerDepth(LayerDepths.Sorting.Tile);
        }

        private static void AddTile(Entity entity, Sprite sprite, bool prioritize = false)
        {
            var spriteRenderer = entity.AddComponent(new SpriteRenderer(sprite));
            spriteRenderer.LayerDepth = LayerDepths.GetLayerDepth(LayerDepths.Sorting.WallBarrier);
            if (prioritize)
                spriteRenderer.LayerDepth += spriteRenderer.LayerDepth / 10;
        }

        public static void PaintWall(Vector2 position, FastList<int> binaryType)
        {
            var entity = currentScene.CreateEntity("tile");
            entity.SetParent(parentEntity);
            entity.Position = position * 16 * scale;
            var collider = entity.AddComponent(new BoxCollider());
            collider.SetSize(16f, 16f);
            Flags.SetFlag(ref collider.PhysicsLayer, (int)PhysicsLayers.Tile);
            Sprite tile = null;

            bool sideCheck = false;

            if (RulesCheck(binaryType, WallRules.TopLeft))
            {
                AddTile(entity, Walls[(int)Sides.TopLeft], true);
                if (RulesCheck(binaryType, WallRules.BottomLeft))
                {
                    AddTile(entity, Walls[(int)Sides.BottomLeft], true);
                    sideCheck = true;
                }
            }
            if (RulesCheck(binaryType, WallRules.TopRight))
            {
                AddTile(entity, Walls[(int)Sides.TopRight], true);
                if (RulesCheck(binaryType, WallRules.BottomRight))
                {
                    AddTile(entity, Walls[(int)Sides.BottomRight], true);
                    sideCheck = true;
                }
            }
            if (RulesCheck(binaryType, WallRules.BottomLeft))
            {
                AddTile(entity, Walls[(int)Sides.BottomLeft], true);
                if (RulesCheck(binaryType, WallRules.TopLeft))
                {
                    AddTile(entity, Walls[(int)Sides.TopLeft], true);
                    sideCheck = true;
                }
            }
            if (RulesCheck(binaryType, WallRules.BottomRight))
            {
                AddTile(entity, Walls[(int)Sides.BottomRight], true); 
                if (RulesCheck(binaryType, WallRules.TopRight))
                {
                    AddTile(entity, Walls[(int)Sides.TopRight], true);
                    sideCheck = true;
                }
            }

            if (RulesCheck(binaryType, WallRules.BottomLeftCorner))
                tile = Walls[(int)WallCorners.InnerCornerBottomLeft];
            if (RulesCheck(binaryType, WallRules.BottomRightCorner))
                tile = Walls[(int)WallCorners.InnerCornerBottomRight];
            if (RulesCheck(binaryType, WallRules.TopLeftCorner))
                tile = Walls[(int)WallCorners.InnerCornerTopLeft];
            if (RulesCheck(binaryType, WallRules.TopRightCorner))
                tile = Walls[(int)WallCorners.InnerCornerTopRight];

            if (!sideCheck)
            {
                if (RulesCheck(binaryType, WallRules.Top))
                    AddTile(entity, Walls[(int)Sides.Top]);
                if (RulesCheck(binaryType, WallRules.Bottom))
                    AddTile(entity, Walls[(int)Sides.Bottom]);
                if (RulesCheck(binaryType, WallRules.Left))
                    AddTile(entity, Walls[(int)Sides.MidLeft]);
                if (RulesCheck(binaryType, WallRules.Right))
                    AddTile(entity, Walls[(int)Sides.MidRight]);
            }
   

            entity.Scale = new Vector2(scale);

            if (tile != null)
                AddTile(entity, tile);
        }

        public static bool RulesCheck(FastList<int> source, int[] rule)
        {
            for (int i = 0; i < source.Length; i++) 
            {
                if (rule[i] == -1 && source[i] == 0)
                    continue;
                if (rule[i] != 0 && source[i] != rule[i])
                    return false;
            }
            return true;
        }
    }
}
