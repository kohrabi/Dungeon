using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using NezTopDown.Components;
using System;
using System.Collections.Generic;

namespace NezTopDown
{
    public class Game1 : Core
    {

        public static List<Sprite> Tiles { get; private set; }
        public static SpriteAtlas WeaponAtlas { get; private set; }

        public Game1() : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true;

            Core.DebugRenderEnabled = true;

            var scene = Scene.CreateWithDefaultRenderer(Color.CornflowerBlue);


            var co = scene.Content.Load<Texture2D>("Sprites/co");

            var texture = scene.Content.Load<Texture2D>("Sprites/Tiles/Garden/Tiles");
            Tiles = Sprite.SpritesFromAtlas(texture, 16, 16);

            WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons/Weapons.atlas");

            var entity = scene.CreateEntity("test");
            entity.Transform.Position = new Vector2(300, 300);
            entity.AddComponent(new SpriteRenderer(WeaponAtlas.Sprites[0]));
            entity.AddComponent(new BoxCollider());

            var wtf = scene.CreateEntity("tiles");
            wtf.AddComponent(new LevelGenerator());
            wtf.Transform.Position = new Vector2(200, 200);

            var test = scene.CreateEntity("player");
            test.AddComponent(new Player());

            scene.Camera.Entity.AddComponent(new FollowCamera(test));
            scene.Camera.SetZoom(0.8f);

            Core.Scene = scene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //entity.Rotation += 5 * Time.DeltaTime;
        }
    }
}