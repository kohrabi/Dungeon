using LDtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NezTopDown.Components.LevelGenerator
{
    public class LDtkManager : Component
    {
        public LDtkWorld World { get; private set; }
        // this is retarded
        public static Dictionary<int, Texture2D> Tilesets { get; private set; } = new();

        TilesetDefinition[] _tilesets;

        bool _renderAllLevels = false;

        /// <summary> This is used to intizialize the renderer for use with direct file loading </summary>
        public LDtkManager(LDtkFile file, Guid worldIid, bool renderAllLevels = false)
        {
            World = file.LoadWorld(worldIid);
            _tilesets = file.Defs.Tilesets;
            _renderAllLevels = renderAllLevels;
        }

        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
            Tilesets.Clear();
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            foreach (var tileset in _tilesets)
            {
                if (tileset.RelPath == null)
                    continue;
                Tilesets.Add(tileset.Uid, Entity.Scene.Content.LoadTexture("Content/Maps/" + tileset.RelPath));
            }

            if (_renderAllLevels)
            {
                foreach(var level in World.Levels)
                {
                    AddLevelToRender(level, level.Position.ToVector2());
                }
            }
            
            _tilesets = null;
        }

        /// <summary> Add level and a new position to render </summary>
        /// <param name="level">The level to render</param>
        /// <param name="levelPos">a new position to render(if levelPos == Vector2.Zero then it will use the default WorldPos)</param>
        public LDtkManager AddLevelToRender(LDtkLevel level, Vector2 levelPos)
        {
            var renderLevel = Entity.Scene.CreateEntity("ldtklevel");
            renderLevel.AddComponent(new LDtkLevelRenderer(level));
            renderLevel.Transform.Position = levelPos;
            renderLevel.SetParent(Entity);
            return this;
        }

        /// <summary> Add level and a new position to render </summary>
        /// <param name="levelIid">The level's IdentifierID to render</param>
        /// <param name="levelPos">a new position to render(if levelPos == Vector2.Zero then it will use the default WorldPos)</param>
        public LDtkManager AddLevelToRender(Guid levelIid, Vector2 levelPos)
        {
            return AddLevelToRender(World.LoadLevel(levelIid), levelPos);
        }

        /// <summary> Add level and a new position to render </summary>
        /// <param name="identifier">The level's Identifier to render</param>
        /// <param name="levelPos">a new position to render(if levelPos == Vector2.Zero then it will use the default WorldPos)</param>
        public LDtkManager AddLevelToRender(string identifier, Vector2 levelPos)
        {
            return AddLevelToRender(World.LoadLevel(identifier), levelPos);
        }

        //private Texture2D RenderBackgroundToLayer(LDtkLevel level)
        //{
        //    Texture2D texture = GetTexture(level, level.BgRelPath);
        //    LevelBackgroundPosition bg = level._BgPos;
        //    Vector2 pos = bg.TopLeftPx.ToVector2();
        //    Batcher.Draw(texture, pos, new Rectangle((int)bg.CropRect[0], (int)bg.CropRect[1], (int)bg.CropRect[2], (int)bg.CropRect[3]), 
        //                    Color.White, 0, Vector2.Zero, bg.Scale, SpriteEffects.None, 0);
        //    return layer;
        //}

        ///// <summary> Render intgrids by displaying the intgrid as solidcolor squares </summary>
        //public void RenderIntGrid(LDtkIntGrid intGrid)
        //{
        //    for (int x = 0; x < intGrid.GridSize.X; x++)
        //    {
        //        for (int y = 0; y < intGrid.GridSize.Y; y++)
        //        {
        //            int cellValue = intGrid.Values[(y * intGrid.GridSize.X) + x];

        //            if (cellValue != 0)
        //            {
        //                // Color col = intGrid.GetColorFromValue(cellValue);

        //                int spriteX = intGrid.WorldPosition.X + (x * intGrid.TileSize);
        //                int spriteY = intGrid.WorldPosition.Y + (y * intGrid.TileSize);
        //                Batcher.Draw(pixel, new Vector2(spriteX, spriteY), null, Color.Pink /*col*/, 0, Vector2.Zero, new Vector2(intGrid.TileSize), SpriteEffects.None, 0);
        //            }
        //        }
        //    }
        //}

        ///// <summary> Renders the entity with the tile it includes </summary>
        ///// <param name="entity">The entity you want to render</param>
        ///// <param name="texture">The spritesheet/texture for rendering the entity</param>
        //public void RenderEntity<T>(T entity, Texture2D texture) where T : ILDtkEntity => Batcher.Draw(texture, entity.Position, entity.Tile, Color.White, 0, entity.Pivot * entity.Size, 1, SpriteEffects.None, 0);

        ///// <summary> Renders the entity with the tile it includes </summary>
        ///// <param name="entity">The entity you want to render</param>
        ///// <param name="texture">The spritesheet/texture for rendering the entity</param>
        ///// <param name="flipDirection">The direction to flip the entity when rendering</param>
        //public void RenderEntity<T>(T entity, Texture2D texture, SpriteEffects flipDirection) where T : ILDtkEntity => Batcher.Draw(texture, entity.Position, entity.Tile, Color.White, 0, entity.Pivot * entity.Size, 1, flipDirection, 0);

        ///// <summary> Renders the entity with the tile it includes </summary>
        ///// <param name="entity">The entity you want to render</param>
        ///// <param name="texture">The spritesheet/texture for rendering the entity</param>
        ///// <param name="animationFrame">The current frame of animation. Is a very basic entity animation frames must be to the right of them and be the same size</param>
        //public void RenderEntity<T>(T entity, Texture2D texture, int animationFrame) where T : ILDtkEntity
        //{
        //    Rectangle animatedTile = entity.Tile;
        //    animatedTile.Offset(animatedTile.Width * animationFrame, 0);
        //    Batcher.Draw(texture, entity.Position, animatedTile, Color.White, 0, entity.Pivot * entity.Size, 1, SpriteEffects.None, 0);
        //}

        ///// <summary> Renders the entity with the tile it includes </summary>
        ///// <param name="entity">The entity you want to render</param>
        ///// <param name="texture">The spritesheet/texture for rendering the entity</param>
        ///// <param name="flipDirection">The direction to flip the entity when rendering</param>
        ///// <param name="animationFrame">The current frame of animation. Is a very basic entity animation frames must be to the right of them and be the same size</param>
        //public void RenderEntity<T>(T entity, Texture2D texture, SpriteEffects flipDirection, int animationFrame) where T : ILDtkEntity
        //{
        //    Rectangle animatedTile = entity.Tile;
        //    animatedTile.Offset(animatedTile.Width * animationFrame, 0);
        //    Batcher.Draw(texture, entity.Position, animatedTile, Color.White, 0, entity.Pivot * entity.Size, 1, flipDirection, 0);
        //}
    }
}
