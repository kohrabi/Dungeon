using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Nez;
using Nez.ImGuiTools;
using Nez.Sprites;
using Nez.Textures;
using Nez.UI;
using NezTopDown.Components;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Nez.Textures.RenderTexture;

namespace NezTopDown
{
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

    public class Game1 : Core
    {

        public static List<Sprite> Tiles { get; private set; }
        public static SpriteAtlas WeaponAtlas { get; private set; }
        public static Effect HitFlashEffect { get; private set; }
        public static List<Weapon> WeaponsList { get; private set; }

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

            

            //CreateGame();
            CreateUITest();

        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //entity.Rotation += 5 * Time.DeltaTime;
        }

        public const int ScreenSpaceRenderLayer = 999;
        ScreenSpaceRenderer _screenSpaceRenderer;

        void CreateUITest()
        {
            var scene = new Scene();
            Core.DebugRenderEnabled = true;
            //scene.SetDesignResolution(640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);

            WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");

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
                .AddComponent(new SpriteRenderer(WeaponAtlas.Sprites[0]))
                .AddComponent(new BoxCollider())
                .Transform.SetScale(2f);
            

            Core.Scene = scene;
        }

        void CreateGame()
        {
            WeaponsList = new List<Weapon>();
            WeaponsList.Add(new Weapon());

            //Core.DebugRenderEnabled = true;

            var scene = Scene.CreateWithDefaultRenderer(Color.CornflowerBlue);
            scene.SetDesignResolution(640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);

            HitFlashEffect = scene.Content.Load<Effect>("Effects/HitFlash");
            var texture = scene.Content.Load<Texture2D>("Sprites/Tiles/Garden/Tiles");
            Tiles = Sprite.SpritesFromAtlas(texture, 16, 16);

            WeaponAtlas = scene.Content.LoadSpriteAtlas("Content/Sprites/Weapons.atlas");
            LoadWeapons();

            var entity = scene.CreateEntity("test");
            entity.Transform.Position = new Vector2(300, 300);
            entity.AddComponent(new SpriteRenderer(WeaponAtlas.Sprites[0]));
            entity.AddComponent(new BoxCollider());

            var wtf = scene.CreateEntity("tiles");
            wtf.AddComponent(new LevelGenerator());
            wtf.Transform.Position = new Vector2(200, 200);

            var player = scene.CreateEntity("player");
            player.AddComponent(new Player());

            scene.Camera.AddComponent(new CameraShake());
            scene.AddSceneComponent(new CameraController());
            scene.Camera.SetZoom(3f);

            Core.Scene = scene;
        }

        void LoadWeapons()
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