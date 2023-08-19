using BulletMLLib;
using LDtk;
using LDtkTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Nez;
using Nez.AI.GOAP;
using Nez.BitmapFonts;
using Nez.ImGuiTools;
using Nez.Sprites;
using Nez.Textures;
using Nez.UI;
using NezTopDown.Components;
using NezTopDown.Components.LevelGenerator;
using NezTopDown.Components.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static Nez.Textures.RenderTexture;
using static NezTopDown.GameManager;

namespace NezTopDown
{
    public class Game1 : Core
    {
        public Game1() : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true;

            //----------------IMGUI SETUP------------------
            //System.Reflection.Assembly.Load("Nez.ImGui");
            var imGuiManager = new ImGuiManager();
            Core.RegisterGlobalManager(imGuiManager);

            // toggle ImGui rendering on/off. It starts out enabled.
            imGuiManager.SetEnabled(false);
            //---------------------------------------------
            Core.DebugRenderEnabled = true;


            //Core.Scene = CreateGame();
            //CreateUITest();
            //CreateBulletTest();
            CreateGeneratorTest();
        }

        static bool transitioning = false;
        static float delay = 1f;
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Input.IsKeyPressed(Keys.F5))
            {
                CreateGeneratorTest();

            }

            Vector2 input = Vector2.Zero;
            if (Input.IsKeyDown(Keys.W))
                input.Y += -1f;
            if (Input.IsKeyDown(Keys.S))
                input.Y += 1F;
            if (Input.IsKeyDown(Keys.A))
                input.X += -1f;
            if (Input.IsKeyDown(Keys.D))
                input.X += 1f;
            Core.Scene.Camera.Position += input * 10f;
            /*
            if (LevelGenerator.enemyCount == 0 && !transitioning)
            {
                if (delay <= 0)
                {
                    Console.WriteLine("trans");
                    var transition = new TextureWipeTransition(() => CreateGame(), Core.Content.Load<Texture2D>("nez/textures/textureWipeTransition/noise"));
                    transition.Duration = 0.4f;
                    Core.StartSceneTransition(transition);
                    transitioning = true;
                }
                delay = Math.Max(0, delay - Time.DeltaTime);
            }
            if (Input.IsKeyReleased(Keys.F5) && !transitioning)
            {
                var transition = new TextureWipeTransition(() => CreateGame(), Core.Content.Load<Texture2D>("nez/textures/textureWipeTransition/noise"));
                transition.Duration = 0.4f;
                Core.StartSceneTransition(transition);
                transitioning = true;
            }
            */

            //entity.Rotation += 5 * Time.DeltaTime;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        Entity entity;

        void CreateGeneratorTest()
        {
            var file = LDtkFile.FromFile("Content/Maps/Room1.ldtk");
            var scene = Scene.CreateWithDefaultRenderer(file.BgColor);

            _bulletTexture = scene.Content.Load<Texture2D>("Sprites/Projectiles/Bullet");
            //scene.CreateEntity("ldtktest").AddComponent(new LDtkManager(file, Worlds.World.Iid, true));
            scene.CreateEntity("generator").AddComponent(new LevelGenerator(file, Worlds.World.Iid, new Vector2(300f, 300f)));

            GameManager.WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");
            entity = scene.CreateEntity("test");
            entity.Transform.Position = new Vector2(300, 300);
            entity.AddComponent(new SpriteRenderer(GameManager.WeaponAtlas.Sprites[0]));
            entity.AddComponent(new BoxCollider());
            //scene.CreateEntity("tiles").AddComponent(new LDtkRenderer(file, Worlds.World.Iid, true));


            Core.Scene = scene;
        }

        BulletMLManager _bulletManager;
        BulletMover _bulletMover;
        BulletPattern _bulletPattern;
        Texture2D _bulletTexture;

        void CreateBulletTest()
        {
            var scene = Scene.CreateWithDefaultRenderer(Microsoft.Xna.Framework.Color.CornflowerBlue);
    
            _bulletTexture = scene.Content.Load<Texture2D>("Sprites/Projectiles/Bullet");

            GameManager.WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");
            var entity = scene.CreateEntity("test");
            entity.Transform.Position = new Vector2(300, 300);
            entity.AddComponent(new SpriteRenderer(GameManager.WeaponAtlas.Sprites[0]));
            entity.AddComponent(new BoxCollider());
            
            _bulletManager = scene.AddSceneComponent(new BulletMLManager(entity.Transform));
            _bulletManager.Difficulty = 1.0; 

            _bulletPattern = new BulletPattern(_bulletManager);
            _bulletPattern.ParseXML("Content/Data/Bullets/TestBullet.xml");

            //clear out all the bulelts
            _bulletManager.Clear();

            //add a new bullet in the center of the screen
            _bulletMover = (BulletMover)_bulletManager.CreateTopBullet();
            _bulletMover.Transform.Position = new Vector2(Screen.PreferredBackBufferWidth / 2f, Screen.PreferredBackBufferHeight / 2f);
            _bulletMover.InitTopNode(_bulletPattern.RootNode);
            Core.Scene = scene;
        }

        public const int ScreenSpaceRenderLayer = 999;
        ScreenSpaceRenderer _screenSpaceRenderer;

        void CreateUITest()
        {
            var scene = new Scene();
            //scene.SetDesignResolution(640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);

            GameManager.WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");

            var healthbarTex = scene.Content.Load<Texture2D>("Sprites/GUI/HealthBar");
            var healthbar = new SpriteDrawable(healthbarTex);
            var healthTex = scene.Content.Load<Texture2D>("Sprites/GUI/Health");
            var health = new SpriteDrawable(healthTex);

            _screenSpaceRenderer = scene.AddRenderer(new ScreenSpaceRenderer(-4, ScreenSpaceRenderLayer)
            {
                RenderTexture = new RenderTexture()
            });
            _screenSpaceRenderer.RenderTexture.ResizeBehavior = RenderTextureResizeBehavior.None;
            scene.AddRenderer(new RenderLayerRenderer(2, 0));

            var canvas = scene.CreateEntity("ui").AddComponent(new UICanvas());
            canvas.SetRenderLayer(ScreenSpaceRenderLayer);
            // tables are very flexible and make good candidates to use at the root of your UI. They work much like HTML tables but with more flexibility.
            var table = canvas.Stage.AddElement(new Table());

            // tell the table to fill all the available space. In this case that would be the entire screen.
            table.SetFillParent(true);

            // add a ProgressBar
            var style = new ProgressBarStyle();
            style.Background = healthbar;
            style.KnobBefore = health;
            var bar = new ProgressBar(0, 100f, 1f, false, style);
            bar.Value = 50f;
            table.Add("this is the progressbar");
            table.Add(bar);

            // this tells the table to move on to the next row
            table.Row();

            // add a Slider
            var slider = new Slider(0, 1, 0.1f, false, SliderStyle.Create(Color.DarkGray, Color.LightYellow));
            table.Add(slider);
            table.Row();

            // if creating buttons with just colors (PrimitiveDrawables) it is important to explicitly set the minimum size since the colored textures created
            // are only 1x1 pixels
            var button = new Button(ButtonStyle.Create(Color.Black, Color.DarkGray, Color.Green));
            table.Add(button).SetMinWidth(100).SetMinHeight(30);

            scene.CreateEntity("uitexture")
                .AddComponent(new SpriteRenderer(_screenSpaceRenderer.RenderTexture))
                .Transform.SetParent(scene.Camera.Transform);

            scene.CreateEntity("test")
                .SetPosition(300, 300)
                .AddComponent(new SpriteRenderer(GameManager.WeaponAtlas.Sprites[0]))
                .AddComponent(new BoxCollider())
                .Transform.SetScale(2f);
            

            Core.Scene = scene;
        }

        public static Scene CreateGame()
        {
            WeaponsList = new List<Weapon>();

            var scene = Scene.CreateWithDefaultRenderer(Color.Black);
            //scene.SetDesignResolution(640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);

            DefaultFont = scene.Content.Load<BitmapFont>("nez/NezDefaultBMFont");
            HitFlashEffect = scene.Content.Load<Effect>("Effects/HitFlash");
            LightBlackEffect = scene.Content.Load<Effect>("Effects/LightBlack");

            WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");
            ProjectilesAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Projectiles/Projectiles.atlas");
            LoadWeapons();

            var entity = scene.CreateEntity("test");
            entity.Transform.Position = new Vector2(300, 300);
            entity.AddComponent(new SpriteRenderer(WeaponAtlas.Sprites[0]));
            entity.AddComponent(new BoxCollider());

            //var wtf = scene.CreateEntity("tiles");
            //wtf.AddComponent(new LevelGenerator());
            //wtf.Transform.Position = new Vector2(200, 200);

            var player = scene.CreateEntity("player");
            player.AddComponent(new Player());

            scene.Camera.AddComponent(new CameraShake());
            scene.AddSceneComponent(new CameraController());
            scene.Camera.SetZoom(1f);

            transitioning = false;
            delay = 1f;


            return scene;
        }

        public static void LoadWeapons()
        {
            // FUCK ME FOR NAMING THING LIKE SHITTTTTTTT
            var json = JObject.Parse(File.ReadAllText("Content/Data/Weapons.json"));
            IList<JToken> weapons = json["weapons"].Children().ToList();
            foreach (JToken weapon in weapons)
            {
                var currentWeapon = weapon.ToObject<Weapon>();
                currentWeapon.sprite = WeaponAtlas.GetSprite(currentWeapon.name);
                WeaponsList.Add(currentWeapon);
            }
        }
    }
}