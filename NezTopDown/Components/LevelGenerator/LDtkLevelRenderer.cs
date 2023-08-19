using LDtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Linq;

namespace NezTopDown.Components.LevelGenerator
{
    public class LDtkLevelRenderer : RenderableComponent
    {
        public override RectangleF Bounds
        {
            get
            {
                if (_areBoundsDirty)
                {
                    if (Level != null)
                        _bounds.CalculateBounds(Entity.Transform.Position, _localOffset, _origin,
                            Entity.Transform.Scale, Entity.Transform.Rotation, Level.PxWid,
                            Level.PxHei);
                    _areBoundsDirty = false;
                }

                return _bounds;
            }
        }

        public Vector2 Origin
        {
            get => _origin;
            set => SetOrigin(value);
        }

        public Vector2 OriginNormalized
        {
            get => new Vector2(_origin.X / Width * Entity.Transform.Scale.X,
                _origin.Y / Height * Entity.Transform.Scale.Y);
            set => SetOrigin(new Vector2(value.X * Width / Entity.Transform.Scale.X,
                value.Y * Height / Entity.Transform.Scale.Y));
        }

        protected Vector2 _origin;

        public LDtkLevelRenderer SetOrigin(Vector2 origin)
        {
            if (_origin != origin)
            {
                _origin = origin;
                _areBoundsDirty = true;
            }

            return this;
        }

        public LDtkLevel Level { get; private set; }

        public LDtkLevelRenderer(LDtkLevel level)
        {
            Level = level;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

        }

        public override void Render(Batcher batcher, Camera camera)
        {
            RenderLayers(batcher, Level);
        }

        private void RenderLayers(Batcher batcher, LDtkLevel level)
        {
            if (level.BgRelPath != null)
            {
                //RenderBackgroundToLayer(level);
            }

            
            // Render Tile, Auto and Int grid layers
            foreach (var layer in level.LayerInstances.Reverse())
            {
                if (layer._TilesetRelPath == null)
                {
                    continue;
                }

                if (layer._Type == LayerType.Entities)
                {
                    continue;
                }

                Texture2D texture = LDtkManager.Tilesets[layer._TilesetDefUid.Value];

                int width = layer._CWid * layer._GridSize;
                int height = layer._CHei * layer._GridSize;

                switch (layer._Type)
                {
                    case LayerType.Tiles:
                        foreach (TileInstance tile in layer.GridTiles.Where(tile => layer._TilesetDefUid.HasValue))
                        {
                            Vector2 tilePos = new(tile.Px.X + layer._PxTotalOffsetX, tile.Px.Y + layer._PxTotalOffsetY);
                            Rectangle rect = new(tile.Src.X, tile.Src.Y, layer._GridSize, layer._GridSize);
                            SpriteEffects mirror = (SpriteEffects)tile.F;
                            batcher.Draw(texture, (Transform.Position + tilePos) * Transform.Scale, rect, Color.White,
                                Transform.Rotation, Origin, Transform.Scale, mirror, LayerDepth);
                        }
                        break;

                    case LayerType.AutoLayer:
                    case LayerType.IntGrid:
                        if (layer.AutoLayerTiles.Length > 0)
                        {
                            foreach (TileInstance tile in layer.AutoLayerTiles.Where(tile => layer._TilesetDefUid.HasValue))
                            {
                                Vector2 tilePos = new(tile.Px.X + layer._PxTotalOffsetX, tile.Px.Y + layer._PxTotalOffsetY);
                                Rectangle rect = new(tile.Src.X, tile.Src.Y, layer._GridSize, layer._GridSize);
                                SpriteEffects mirror = (SpriteEffects)tile.F;
                                batcher.Draw(texture, (Transform.Position + tilePos) * Transform.Scale, rect, Color.White,
                                        Transform.Rotation, Origin, Transform.Scale, mirror, LayerDepth);
                            }
                        }
                        break;

                    case LayerType.Entities:
                    default:
                        break;
                }
            }
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
    }
}
